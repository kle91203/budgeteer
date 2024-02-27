using Api.Models;
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