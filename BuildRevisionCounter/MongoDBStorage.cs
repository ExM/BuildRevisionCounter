using System.Threading.Tasks;
using BuildRevisionCounter.Model;
using MongoDB.Driver;

namespace BuildRevisionCounter
{
	public class MongoDBStorage
	{
		public static readonly string AdminName = "admin";
		public static readonly string AdminPassword = "admin";
		public static readonly string[] AdminRoles = {"admin", "buildserver", "editor"};
		public readonly IMongoCollection<RevisionModel> Revisions;
		public readonly IMongoCollection<UserModel> Users;

		public MongoDBStorage(IMongoDatabase database)
		{
			Revisions = database.GetCollection<RevisionModel>("revisions");
			Users = database.GetCollection<UserModel>("users");
		}

		public async Task Setup()
		{
			await EnsureAdminUser();
		}

		public async Task EnsureAdminUser()
		{
			if (await Users.Find(u => u.Name == AdminName).CountAsync() == 0)
			{
				await CreateUser(AdminName, AdminPassword, AdminRoles);
			}
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