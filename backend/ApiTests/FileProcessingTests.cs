using Api.Models;
using Edwards.Kevin.Budgeteer.Utils;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TodoApi.Controllers;

namespace Tests;

public class FileProcessingTests
{
    [Fact]
    public void HappyPath_SavesFileLinesToDatabase()
    {
        string[] fileLines = new string[] {
            "\"Date\",\"No.\",\"Description\",\"Debit\",\"Credit\"",
            "\"2/19/2024\",\"\",\"Pending - 02/14 - PAPA MURPHY'S UT069 OL\",\"46.05\",\"\"",
            "\"2/14/2024\",\"\",\"Walmart Refund\",\"\",\"23.44\""
        };
        Mock<IUtils> utilsMock = new Mock<IUtils>();
        utilsMock
            .Setup(u => u.GetFiles(Moq.It.IsAny<string>()))
            .Returns(new string[] {"a.csv"});
        utilsMock
            .Setup(u => u.LoadFile(Moq.It.IsAny<string>()))
            .Returns(fileLines);
        Mock<IMongoDbClient> mongoClientMock = new Mock<IMongoDbClient>();
        var controller = new BudgeteerController(NullLogger<BudgeteerController>.Instance, mongoClientMock.Object, utilsMock.Object);

        controller.InnerMigrateDownload("some/file/path");

        utilsMock.Verify(u => u.GetFiles(Moq.It.IsAny<string>()), Times.Once());
        mongoClientMock.Verify(m => m.InsertOne(Moq.It.IsAny<CsvTransaction>()), Times.Once());
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
