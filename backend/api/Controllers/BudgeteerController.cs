using Edwards.Kevin.Budgeteer.Models;
using Edwards.Kevin.Budgeteer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BudgeteerController : ControllerBase
{
    private readonly ILogger<BudgeteerController> Logger;
    private readonly IMongoDbClient MongoClient;
    private readonly string DOWNLOAD_PATH = "/Users/kevin.edwards/Documents/Personal/Finance/America First/Transactions/";
    private readonly IUtils Utils;

    public BudgeteerController(ILogger<BudgeteerController> logger, IMongoDbClient mongoClient, IUtils utils)
    {
        Logger = logger;
        MongoClient = mongoClient;
        Utils = utils;
    }


    [HttpPost]
    [Route("MigrateDownload")]
    public IActionResult MigrateDownload()
    {
        InnerMigrateDownload(DOWNLOAD_PATH);
        return new OkResult();
    }

    public void InnerMigrateDownload(string dirPath)
    {
        var filePaths = Utils.GetFiles(dirPath);
        foreach (var filePath in filePaths)
            MigrateFile(filePath);
    }

    private void MigrateFile(string filePath)
    {
        if (filePath.EndsWith("csv"))
            MigrateCsvFile(filePath);
        // if (filePath.EndsWith("ofx"))
        //     MigrateOfxFile(filePath);
    }

    private void MigrateCsvFile(string filePath)
    {
        var fileLines = Utils.LoadFile(filePath);
        List<CsvTransaction> transactions = new List<CsvTransaction>();
        foreach(string fileLine in fileLines)
            transactions.Add(new CsvTransaction(fileLine));
        foreach(CsvTransaction transaction in transactions)
            if (transaction.IsValid && transaction.IsPendingTransaction)
                MongoClient.InsertOne(transaction);
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
