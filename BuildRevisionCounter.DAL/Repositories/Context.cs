using System.Configuration;
using MongoDB.Driver;

namespace BuildRevisionCounter.DAL.Repositories
{
    public abstract class Context
    {
        protected IMongoDatabase Database
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

        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString; }
        }

        private MongoUrl MongoUrl
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

        private IMongoClient Client
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

        private MongoClient _mongoClient;
        private MongoUrl _mongoUrl;
        private IMongoDatabase _database;
        private readonly string _connectionStringName = "MongoDBStorage";
    }
}
