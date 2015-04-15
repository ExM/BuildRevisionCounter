using BuildRevisionCounter.Model;
using MongoDB.Driver;

namespace BuildRevisionCounter
{
	public class MongoDBStorage
	{
		public static readonly string AdminName = "admin";
		public readonly IMongoCollection<RevisionModel> Revisions;
		public readonly IMongoCollection<UserModel> Users;

		public MongoDBStorage(IMongoDatabase database)
		{
			Revisions = database.GetCollection<RevisionModel>("revisions");
			Users = database.GetCollection<UserModel>("users");

			CreateAdmin();
		}

		private void CreateAdmin()
		{
			if (Users.Find(u => u.Name == AdminName).CountAsync().Result == 0)
			{
				Users
					.InsertOneAsync(
						new UserModel
						{
							Name = AdminName,
							Password = AdminName,
							Roles = new[] {AdminName, "buildserver", "editor"}
						})
					.Wait();
			}
		}
	}
}