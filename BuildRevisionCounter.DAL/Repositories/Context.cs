using System.Configuration;
using BuildRevisionCounter.Model.BuildRevisionStorage;
using MongoDB.Driver;

namespace BuildRevisionCounter.DAL.Repositories
{
    public class Context
    {
        protected static MongoDBStorage DbStorage
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = new MongoDBStorage(Database);
                }
                return _dbContext;
            }
        }

        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString; }
        }

        private static MongoUrl MongoUrl
        {
            get
            {
                if (_mongoUrl == null)
                {
                    _mongoUrl = MongoUrl.Create(ConnectionString);
                }
                return _mongoUrl;
            }
        }

        private static IMongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = Client.GetDatabase(MongoUrl.DatabaseName);
                }
                return _database;
            }
        }

        private static MongoClient Client
        {
            get
            {
                if (_mongoClient == null)
                {
                    _mongoClient = new MongoClient(MongoUrl);
                }
                return _mongoClient;
            }
        }

        private static MongoClient _mongoClient;
        private static MongoDBStorage _dbContext;
        private static MongoUrl _mongoUrl;
        private static IMongoDatabase _database;
        private static readonly string _connectionStringName = "MongoDBStorage";
    }
}
