using System;
using System.Configuration;
using System.Threading.Tasks;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver;

namespace BuildRevisionCounter.Core
{
	public class MongoContext : IRepository
	{
		public static readonly string AdminName = "admin";
		public static readonly string AdminPassword = "admin";
		public static readonly string[] AdminRoles = { "admin", "buildserver", "editor" };

		private static readonly Object SLock = new Object();
		private static volatile MongoContext _instance = null;
		
		private readonly IMongoClient _client;
		internal IMongoClient Client
		{
			get { return _client; }
		}

		private readonly IMongoDatabase _database;
		internal IMongoDatabase Database
		{
			get { return _database; }
		}

		private MongoContext()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["MongoDBStorage"].ConnectionString;
			var url = new MongoUrl(connectionString);

			_client = new MongoClient(url);
			_database = _client.GetDatabase(url.DatabaseName);

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

		public async Task SetUpAsync()
		{
			await Users.Indexes.CreateOneAsync(
				Builders<User>.IndexKeys.Ascending(u => u.Name),
				new CreateIndexOptions { Unique = true });

			await EnsureAdminUser();
		}

		public async Task DropDatabaseAsync()
		{
			await Client.DropDatabaseAsync(Database.DatabaseNamespace.DatabaseName);
		}

		public async Task EnsureAdminUser()
		{
			var repository = RepositoryFactory.Instance.GetUserRepository();

			if (await repository.CountAsync() == 0)
			{
				await repository.CreateUserAsync(new Protocol.User
				{
					Name = AdminName,
					Password = AdminPassword,
					Roles = AdminRoles
				});
			}
		}

		internal IMongoCollection<Revision> Revisions
		{
			get { return _database.GetCollection<Revision>("revisions"); }
		}

		internal IMongoCollection<User> Users
		{
			get { return _database.GetCollection<User>("users"); }
		}
		
		private void CreateAdmin()
		{
			var anyUser = Users.Find(l => true).FirstOrDefaultAsync();
			anyUser.Wait();

			if (anyUser.Result == null)
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

	public interface IRepository
	{
		Task SetUpAsync();

		Task DropDatabaseAsync();
	}
}