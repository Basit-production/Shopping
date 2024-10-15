
using Ahmed_mart.Models.v1.MONGODB;
using MongoDB.Driver;

namespace Ahmed_mart.DbContexts.v1
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Student> Students => _database.GetCollection<Student>("Students");
    }
}
