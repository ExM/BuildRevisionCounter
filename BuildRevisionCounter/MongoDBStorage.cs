using System.Threading.Tasks;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Model;
using MongoDB.Driver;

namespace BuildRevisionCounter
{
	public class MongoDBStorage : IMongoDBStorage
	{
		public static readonly string AdminName = "admin";
		public static readonly string AdminPassword = "admin";
		public static readonly string[] AdminRoles = {"admin", "buildserver", "editor"};

		public IMongoCollection<RevisionModel> Revisions { get; private set; }
		public IMongoCollection<UserModel> Users { get; private set; }

		public MongoDBStorage(IMongoDatabase database)
		{
			Revisions = database.GetCollection<RevisionModel>("revisions");
			Users = database.GetCollection<UserModel>("users");
		}

		public async Task SetUp()
		{
			await Users.Indexes.CreateOneAsync(
				Builders<UserModel>.IndexKeys.Ascending(u => u.Name),
				new CreateIndexOptions {Unique = true});

			await EnsureAdminUser();
		}

		public async Task EnsureAdminUser()
		{
			if (await Users.CountAsync(_ => true) == 0)
			{
				await CreateUser(AdminName, AdminPassword, AdminRoles);
			}
		}

		public async Task<UserModel> FindUser(string name)
		{
			return await Users.Find(u => u.Name == name).SingleOrDefaultAsync();
		}

		public Task CreateUser(string name, string password, string[] roles)
		{
			return Users
				.InsertOneAsync(
					new UserModel
					{
						Name = name,
						Password = password,
						Roles = roles
					});
		}
	}
}