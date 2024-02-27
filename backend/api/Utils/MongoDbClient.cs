using Api.Models;
using MongoDB.Driver;

namespace Edwards.Kevin.Budgeteer.Utils;

public interface IMongoDbClient
{
    void InsertOne(CsvTransaction csvTransaction);
}

public class MongoDbClient : IMongoDbClient
{

    private readonly IMongoCollection<CsvTransaction> expencesCollection;
    private readonly string MONGO_DB_CONECTION_STRING = "mongodb://admin:M4ch1n31Q@localhost:27017";

    public MongoDbClient()
    {
        var client = new MongoClient(MONGO_DB_CONECTION_STRING);
        expencesCollection = client.GetDatabase("Budgeteer").GetCollection<CsvTransaction>("Expences");
    }

    public void InsertOne(CsvTransaction csvTransaction)
    {
        expencesCollection.InsertOne(csvTransaction);
    }
}
