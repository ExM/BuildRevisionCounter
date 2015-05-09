using BuildRevisionCounter.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Wrappers;

namespace BuildRevisionCounter
{
	public class MongoDBStorage
	{
		private static readonly Object SLock = new Object();
		private static MongoDBStorage _instance = null;

		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;

		private MongoDBStorage()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["MongoDBStorage"].ConnectionString;
			var url = new MongoUrl(connectionString);

			_client = new MongoClient(url);
			_database = _client.GetDatabase(url.DatabaseName);

			CreateAdmin();
		}

		public static MongoDBStorage Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (SLock)
					{
						if (_instance == null)
							_instance = new MongoDBStorage();
					}
				}
				return _instance;
			}
		}

		public IMongoCollection<RevisionModel> Revisions
		{
			get { return _database.GetCollection<RevisionModel>("revisions"); }
		}

		public IMongoCollection<UserModel> Users
		{
			get { return _database.GetCollection<UserModel>("users"); }
		}

		private void CreateAdmin()
		{
			var anyUser = Users.Find(l => true).SingleOrDefaultAsync();
			anyUser.Wait();

			if (anyUser.Result == null)
			{
				Users.InsertOneAsync(new UserModel
				{
					Name = "admin",
					Password = "admin",
					Roles = new[] {"admin", "buildserver", "editor"}
				});
			}
		}
	}
}