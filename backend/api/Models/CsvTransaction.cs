namespace Api.Models;

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