using Api.Models;
using Edwards.Kevin.Budgeteer.Models;
using Edwards.Kevin.Budgeteer.Utils;
using Microsoft.AspNetCore.Mvc;
// ReSharper disable CheckNamespace
// ReSharper disable ConvertToPrimaryConstructor

namespace Edwards.Kevin.Budgeteer.Controllers;

[ApiController]
[Route("[controller]")]
public class BudgeteerController : ControllerBase
{
    private const string DownloadPath = "/Users/kevin.edwards/Documents/Personal/Finance/America First/Transactions/";

    private readonly IMongoDbClient _mongoClient;
    private readonly IUtils _utils;

    public BudgeteerController(IMongoDbClient mongoClient, IUtils utils)
    {
        _mongoClient = mongoClient;
        _utils = utils;
    }


    [HttpPost]
    [Route("MigrateDownload")]
    public IActionResult MigrateDownload()
    {
        InnerMigrateDownload(DownloadPath);
        return new OkResult();
    }

    public void InnerMigrateDownload(string dirPath)
    {
        var filePaths = _utils.GetFiles(dirPath);
        foreach (var filePath in filePaths)
            MigrateFile(filePath);
    }

    private void MigrateFile(string filePath)
    {
        if (filePath.EndsWith("csv"))
            MigrateCsvFile(filePath);
        if (filePath.EndsWith("ofx"))
            MigrateOfxFile(filePath);
    }

    private void MigrateCsvFile(string filePath)
    {
        var fileLines = _utils.LoadFile(filePath);
        var transactions = new List<CsvTransaction>();
        foreach(var fileLine in fileLines)
            transactions.Add(new CsvTransaction(fileLine));
        foreach(var transaction in transactions)
            if (transaction.IsValid && transaction.IsPendingTransaction)
                _mongoClient.InsertOne(transaction);
    }


    private void MigrateOfxFile(string fileName)
    {
        try
        {
            using var sr = new StreamReader(fileName);
            var line = sr.ReadLine();
            var transaction = new OfxTransaction();
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

    private static bool IsTransactionStart(string line) => line.StartsWith("<STMTTRN>");
    private static bool IsTransactionEnd(string line) => line.StartsWith("</STMTTRN>");

    private static bool IsElementWeCareAbout(string line)
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
    [Route("GetExpenses/{dataSetId:int}")]
    public IEnumerable<Expense> Get(int dataSetId)
    {
        var dataSet1 = new List<Expense> {
            new (1, 8500, Category.Electricity, "whatevs"),
            new (2, 5299, Category.Restaurants, "don't care"),
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