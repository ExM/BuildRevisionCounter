using MongoDB.Driver;
using System.Threading.Tasks;
using BuildRevisionCounter.MongoDB.Model;

namespace BuildRevisionCounter.MongoDB
{
	public class MongoUserRepository : IUserRepository
	{
		public static readonly string AdminName = "admin";
		public static readonly string AdminPassword = "admin";
		public static readonly string[] AdminRoles = { "admin", "buildserver", "editor" };

		public MongoUserRepository(IMongoDatabase database)
		{
			_users = database.GetCollection<UserModel>("users");
			Task.Run(()=>CreateAdmin(_users)).Wait();
		}

		public async Task<DTO.User> FindUserByName(string name)
		{
			var result = await _users.Find(Builders<UserModel>.Filter.Where(u => u.Name == name)).SingleOrDefaultAsync();
			return result == null ? null : new DTO.User(result.Name, result.Password, result.Roles);
		}

		public async Task CreateUser(string name, string password, string[] roles)
		{
			try
			{
				await _users
					.InsertOneAsync(
						new UserModel
						{
							Name = name,
							Password = password,
							Roles = roles
						});
			}
			catch (MongoWriteException ex)
			{
				if(ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
					throw new DuplicateKeyException("Duplicate", ex);

				throw;
			}
		}
		
		private async Task CreateAdmin(IMongoCollection<UserModel> users)
		{
 			await users.Indexes.CreateOneAsync(
				Builders<UserModel>.IndexKeys.Ascending(u => u.Name),
				new CreateIndexOptions { Unique = true, });

			if (await users.CountAsync(_ => true) == 0)
			{
				await users
					.InsertOneAsync(
					new UserModel
					{
						Name = AdminName,
						Password = AdminPassword,
						Roles = AdminRoles
					});
			}
		}

		private readonly IMongoCollection<UserModel> _users;
	}
}
