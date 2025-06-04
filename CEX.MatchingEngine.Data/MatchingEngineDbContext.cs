using CEX.MatchingEngine.Core.Models;
using MongoDB.Driver;

namespace CEX.MatchingEngine.Data
{
    public class MatchingEngineDbContext
    {
        private readonly IMongoDatabase _database;

        public MatchingEngineDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Trade> Trades => _database.GetCollection<Trade>("Trades");
    }
}
