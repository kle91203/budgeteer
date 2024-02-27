namespace Api.Models;

public class CsvTransaction
{
    public DateTime Date {get;}
    public string CheckNumber {get; private init; }
    public string Description {get;}
    public string TransactionType {get;}
    public double Amount {get;}
    public bool IsValid {get;}
    public bool IsPendingTransaction {get;}

    public static CsvTransaction CreateNew(string csvLine)
    {
        return new CsvTransaction(csvLine);
    }

    public CsvTransaction(string csvLine)
    {       
        var fields = csvLine.Split("\",\"");
        if (fields.Length != 5)
        {
            Date = DateTime.MinValue;
            CheckNumber = "";
            Description = "";
            TransactionType = "None";
            return;
        }

        if (fields[0] == "\"Date")
        {
            Date = DateTime.MinValue;
            CheckNumber = "";
            Description = "";
            TransactionType = "None";
            return;
        }

        IsValid = true;

        Date = ProcessDate(fields[0]);
        CheckNumber = fields[1];
        Description = fields[2];
        IsPendingTransaction = Description.StartsWith("Pending - ");

        if (fields[3].Length > 2)
        {
            TransactionType = "Debit";
            Amount = double.Parse(fields[3]);
        }
        else
        {
            TransactionType = "Credit";
            Amount = ProcessCredit(fields[4]);
        }
    }

    private static DateTime ProcessDate(string value) 
    {
        var scrubbed = value[1..];
        return DateTime.Parse(scrubbed);
    }

    private static double ProcessCredit(string value) 
    {
        var scrubbed = value[..^1];
        return double.Parse(scrubbed);
    }

    public override string ToString()
    {
        return $"Date: {Date}, CheckNumber: {CheckNumber}, Description: {Description}, : Type:{TransactionType}, : Amount:{Amount}";
    }
}