namespace Api.Models;

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