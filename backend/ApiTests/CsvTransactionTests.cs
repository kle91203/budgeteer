using Api.Models;

namespace Tests;

public class CsvTransactionTests
{

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("	")]
    [InlineData("\r")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void EmptyNullAndWhiteSpaceInput(string csvLine)
    {
        var transaction = CsvTransaction.createNew(csvLine);
        Assert.False(transaction.IsValid);
    }

    [Fact]
    public void HeadersLine()
    {
        var transaction = CsvTransaction.createNew("\"Date\",\"No.\",\"Description\",\"Debit\",\"Credit\"");
        Assert.False(transaction.IsValid);
    }

    [Fact]
    public void ValidPendingDebitLine()
    {
        var transaction = CsvTransaction.createNew("\"2/19/2024\",\"\",\"Pending - 02/14 - PAPA MURPHY'S UT069 OL\",\"46.05\",\"\"");
        Assert.True(transaction.IsValid);
        Assert.True(transaction.IsPendingTransaction);
        Assert.Equal(DateTime.Parse("2/19/2024"), transaction.Date);
        Assert.Equal("", transaction.CheckNumber);
        Assert.Equal("Pending - 02/14 - PAPA MURPHY'S UT069 OL", transaction.Description);
        Assert.Equal("Debit", transaction.TransactionType);
        Assert.Equal(46.05, transaction.Amount);
    }

    [Fact]
    public void ValidCreditLine()
    {
        var transaction = CsvTransaction.createNew("\"2/14/2024\",\"\",\"Walmart Refund\",\"\",\"23.44\"");
        Assert.True(transaction.IsValid);
        Assert.False(transaction.IsPendingTransaction);
        Assert.Equal(DateTime.Parse("2/14/2024"), transaction.Date);
        Assert.Equal("", transaction.CheckNumber);
        Assert.Equal("Walmart Refund", transaction.Description);
        Assert.Equal("Credit", transaction.TransactionType);
        Assert.Equal(23.44, transaction.Amount);
    }
}
