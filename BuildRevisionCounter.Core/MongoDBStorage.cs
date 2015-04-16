using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BuildRevisionCounter.Core
{
	public class MongoContext
	{
        private static readonly Object SLock = new Object();
        private static MongoContext _instance = null;

        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        private MongoContext()
		{
            var mongoHost = ConfigurationManager.ConnectionStrings["MongoDBStorage"].ConnectionString;
            var mongoDbName = ConfigurationManager.AppSettings.GetValues("MongoDbName").First();

            _client = new MongoClient(mongoHost);
            _database = _client.GetDatabase(mongoDbName);
            
            CreateAdmin();
		}

        public static MongoContext Instance
	    {
	        get
	        {
	            if (_instance != null) return _instance;
	            Monitor.Enter(SLock);
                var temp = new MongoContext();
	            Interlocked.Exchange(ref _instance, temp);
	            Monitor.Exit(SLock);
	            return _instance;
	        }
	    }

	    public IMongoClient Client
        {
            get { return _client; }
        }

        public IMongoCollection<Revision> Revisions
        {
            get { return _database.GetCollection<Revision>("revisions"); }
        }

        public IMongoCollection<User> Users
        {
            get { return _database.GetCollection<User>("users"); }
        }

        private void CreateAdmin()
        {
            if (Users.Find(l => true).SingleOrDefaultAsync() == null)
            {
                Users.InsertOneAsync(new User
                {
                    Name = "admin",
                    Password = "admin",
                    Roles = new[] { "admin", "buildserver", "editor" }
                });
            }
        }
	}
}