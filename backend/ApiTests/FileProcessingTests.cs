using Api.Models;
using Edwards.Kevin.Budgeteer.Controllers;
using Edwards.Kevin.Budgeteer.Utils;
using Moq;
// ReSharper disable CheckNamespace

namespace Edwards.Kevin.Budgeteer.Models.Tests;

public class FileProcessingTests
{
    [Fact]
    public void HappyPath_SavesFileLinesToDatabase()
    {
        var fileLines = new[] {
            "\"Date\",\"No.\",\"Description\",\"Debit\",\"Credit\"",
            "\"2/19/2024\",\"\",\"Pending - 02/14 - PAPA MURPHY'S UT069 OL\",\"46.05\",\"\"",
            "\"2/14/2024\",\"\",\"Walmart Refund\",\"\",\"23.44\""
        };
        var utilsMock = new Mock<IUtils>();
        utilsMock
            .Setup(u => u.GetFiles(It.IsAny<string>()))
            .Returns(["a.csv"]);
        utilsMock
            .Setup(u => u.LoadFile(It.IsAny<string>()))
            .Returns(fileLines);
        var mongoClientMock = new Mock<IMongoDbClient>();
        var controller = new BudgeteerController(mongoClientMock.Object, utilsMock.Object);

        controller.InnerMigrateDownload("some/file/path");

        utilsMock.Verify(u => u.GetFiles(It.IsAny<string>()), Times.Once());
        mongoClientMock.Verify(m => m.InsertOne(It.IsAny<CsvTransaction>()), Times.Once());
    }

    [Fact]
    public void HeadersLine()
    {
        var transaction = CsvTransaction.CreateNew("\"Date\",\"No.\",\"Description\",\"Debit\",\"Credit\"");
        Assert.False(transaction.IsValid);
    }

    [Fact]
    public void ValidPendingDebitLine()
    {
        var transaction = CsvTransaction.CreateNew("\"2/19/2024\",\"\",\"Pending - 02/14 - PAPA MURPHY'S UT069 OL\",\"46.05\",\"\"");
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
        var transaction = CsvTransaction.CreateNew("\"2/14/2024\",\"\",\"Walmart Refund\",\"\",\"23.44\"");
        Assert.True(transaction.IsValid);
        Assert.False(transaction.IsPendingTransaction);
        Assert.Equal(DateTime.Parse("2/14/2024"), transaction.Date);
        Assert.Equal("", transaction.CheckNumber);
        Assert.Equal("Walmart Refund", transaction.Description);
        Assert.Equal("Credit", transaction.TransactionType);
        Assert.Equal(23.44, transaction.Amount);
    }
}
