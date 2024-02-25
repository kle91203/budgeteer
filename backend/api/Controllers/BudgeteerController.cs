using Edwards.Kevin.Budgeteer.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BudgeteerController : ControllerBase
{
    private readonly ILogger<BudgeteerController> _logger;
    private readonly string DOWNLOAD_PATH = "/Users/kevin.edwards/Documents/Personal/Finance/America First/Transactions/";
    private readonly string MONGO_DB_CONECTION_STRING = "mongodb://admin:M4ch1n31Q@localhost:27017";
    private readonly IMongoCollection<CsvTransaction> expencesCollection;

    public BudgeteerController(ILogger<BudgeteerController> logger)
    {
        _logger = logger;

        var client = new MongoClient(MONGO_DB_CONECTION_STRING);
        expencesCollection = client.GetDatabase("Budgeteer").GetCollection<CsvTransaction>("Expences");
        // var filter = Builders<BsonDocument>.Filter.Eq("title", "Back to the Future");
        // var document = collection.Find(filter).First();
        // Console.WriteLine(document);
    }


    [HttpPost]
    [Route("MigrateDownload")]
    public IActionResult MigrateDownload()
    {

        InnerMigrateDownload();

        return new OkResult();
    }

    public void InnerMigrateDownload()
    {
        var fileNames = GetFiles();
        foreach (var fileName in fileNames)
        {
            Console.WriteLine($"Migrating {fileName}");
            MigrateFile(fileName);
            Console.WriteLine($"Migrated {fileName}");
        }
    }

    private void MigrateFile(string fileName)
    {
        if (fileName.EndsWith("csv"))
            MigrateCsvFile(fileName);
        // if (fileName.EndsWith("ofx"))
        //     MigrateOfxFile(fileName);
    }
    private void MigrateCsvFile(string fileName)
    {
        string? line;
        try
        {


            using var sr = new StreamReader(fileName);
            // using var reader = new StreamReader(fileName);
            // using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            // {
            //     var records = csv.GetRecords<CsvTransaction>();
            // }


            line = sr.ReadLine();
            while (line != null)
            {
                var fields = line.Split("\",\"");
                if (fields[0] == "Date")
                {
                    line = sr.ReadLine();
                    continue;
                }
                var csvTransaction = new CsvTransaction(fields);
                if (csvTransaction.Description.StartsWith("Pending - "))
                {
                    Console.WriteLine(csvTransaction);
                    try 
                    {
                        expencesCollection.InsertOne(csvTransaction);
                    }
                    catch(Exception e) 
                    {
                        Console.WriteLine(e);
                        //TODO return info to front end on which ones failed
                    }
                }
                line = sr.ReadLine();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    private void MigrateOfxFile(string fileName)
    {
        string? line;
        try
        {
            using StreamReader sr = new StreamReader(fileName);
            line = sr.ReadLine();
            OfxTransaction transaction = new OfxTransaction();
            while (line != null)
            {
                if (IsTransactionStart(line))
                    transaction = new OfxTransaction();
                if (IsElementWeCareAbout(line))
                    transaction.AddField(line);
                if (IsTransactionEnd(line))
                    Console.WriteLine(transaction);
                    //TODO Save to DB
                line = sr.ReadLine();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    private bool IsTransactionStart(string line) => line.StartsWith("<STMTTRN>");
    private bool IsTransactionEnd(string line) => line.StartsWith("</STMTTRN>");

    private string[] GetFiles() => Directory.GetFiles(DOWNLOAD_PATH);

    private bool IsElementWeCareAbout(string line)
    {
        return 
            line.StartsWith("<TRNTYPE>") || 
            line.StartsWith("<DTPOSTED>") || 
            line.StartsWith("<TRNAMT>") || 
            line.StartsWith("<FITID>") || 
            line.StartsWith("<NAME>") || 
            line.StartsWith("<MEMO>");            
    }

    //todo: how do you put the id into the route? use the id to control what gets returned
    [HttpGet]
    [Route("GetExpences/{dataSetId}")]
    public IEnumerable<Expence> Get(int dataSetId)
    {
        var dataSet1 = new List<Expence> {
            new (1, 8500, Category.Electricity, "whatevs"),
            new (2, 5299, Category.Restauraunts, "don't care"),
            new (3, 20084, Category.Groceries, "doesn't matter"),
            new (4, 10022, Category.Groceries, "meh"),
            new (5, 5203, Category.Shopping, "wat"),
        };

        if (dataSetId == 1)
            return dataSet1;
        else
            return [];
    }
}

public class CsvTransaction
{
    public DateTime Date {get; init;}
    public string CheckNumber {get; init;}
    public string Description {get; init;}
    public string TransactionType {get; set;}
    public double Amount {get; init;}
    public bool IsValid {get; init;}
    public bool IsPendingTransaction {get; init;}

    public static CsvTransaction createNew(string csvLine)
    {
        return new CsvTransaction(csvLine);
    }

    public CsvTransaction(string csvLine)
    {       
        if (csvLine == null)
            return;
        var fields = csvLine.Split("\",\"");
        if (fields.Length != 5)
            return;
        if (fields[0] == "\"Date")
            return;

        IsValid = true;

        Date = ProcessDate(fields[0]);
        CheckNumber = fields[1];
        Description = fields[2];
        IsPendingTransaction = Description.StartsWith("Pending - ");

        if (fields[3].Length > 2)
        {
            TransactionType = "Debit";
            Amount = Double.Parse(fields[3]);
        }
        else
        {
            TransactionType = "Credit";
            Amount = ProcessCredit(fields[4]);
        }
    }

    public CsvTransaction(string[] fields)
    {
        throw new Exception("Use the other ");
    }

    private DateTime ProcessDate(string value) 
    {
        var scrubbed = value.Substring(1);
        return DateTime.Parse(scrubbed);
    }

    private double ProcessCredit(string value) 
    {
        var scrubbed = value.Substring(0, value.Length - 1);
        return Double.Parse(scrubbed);
    }

    public override string ToString()
    {
        return $"Date: {Date}, CheckNumber: {CheckNumber}, Description: {Description}, : Type:{TransactionType}, : Amount:{Amount}";
    }
}

public class OfxTransaction
{
    public string? Id {get; set;}
    public string? Date {get; set;}
    public string? Name {get; set;}
    public string? Memo {get; set;}
    public string? TransactionType {get; set;}
    public string? Amount {get; set;}

    public override string ToString()
    {
        return $"Id: {Id}, Date: {Date}, Memo: {Memo}, Type: {TransactionType}, Amount: {Amount}";
    }

    public void AddField(string line)
    {
        if (line.StartsWith("<TRNTYPE>"))
            TransactionType = line.Replace("<TRNTYPE>", "");
        else if (line.StartsWith("<DTPOSTED>"))
            Date = line.Replace("<DTPOSTED>", "");
        else if (line.StartsWith("<TRNAMT>"))
            Amount = line.Replace("<TRNAMT>", "");
        else if (line.StartsWith("<FITID>"))
            Id = line.Replace("<FITID>", "");
        else if (line.StartsWith("<NAME>"))
            Name = line.Replace("<NAME>", "");
        else if (line.StartsWith("<MEMO>"))
            Memo = line.Replace("<MEMO>", "");
        else
            throw new Exception($"{line} is not a recognized field");
    }
}
