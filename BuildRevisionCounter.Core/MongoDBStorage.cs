using System;
using System.Configuration;
using System.Linq;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BuildRevisionCounter.Core
{
	public class MongoContext
	{
		private static readonly Object SLock = new Object();
		private static volatile MongoContext _instance = null;

		private MongoContext()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["MongoDBStorage"].ConnectionString;
			var database = MongoDatabase.Create(connectionString);

			Revisions = database.GetCollection<Revision>("revisions");
			Users = database.GetCollection<User>("users");

			CreateAdmin();
		}

		public static MongoContext Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (SLock)
					{
						if (_instance == null)
							_instance = new MongoContext();
					}
				}
				return _instance;
			}
		}

		public readonly MongoCollection<Revision> Revisions;
		public readonly MongoCollection<User> Users;

		private void CreateAdmin()
		{
			if (!Users.AsQueryable().Any())
			{
				Users.Insert(new User
				{
					Name = "admin",
					Password = "admin",
					Roles = new[] { "admin", "buildserver", "editor" }
				});
			}
		}
	}
}