using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using System.Threading.Tasks;
using BuildRevisionCounter.DTO;
using BuildRevisionCounter.Exceptions;
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

		public async Task<User> FindUserByName(string name)
		{
			var result = await _users.Find(Builders<UserModel>.Filter.Where(u => u.Name == name)).SingleOrDefaultAsync();
			return result == null ? null : new User(result.Name, result.Roles);
		}

		public async Task<User> FindUserByNameAndPassword(string userName, string password)
		{
			var result = await _users.Find(Builders<UserModel>.Filter
				.Where(u => u.Name == userName && u.Password == password))
				.SingleOrDefaultAsync();

			return result == null ? null : new User(result.Name, result.Roles);
		}

		public async Task<User> CreateUser(string name, string password, string[] roles)
		{
			AssertRoleListCorrectness(roles);

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
				return new User(name, roles);
			}
			catch (MongoWriteException ex)
			{
				if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
					throw new DuplicateKeyException();

				throw;
			}
		}

		public Task DeleteUser(string name)
		{
			return _users.DeleteOneAsync(u => u.Name == name);
		}

		public Task UpdateUserRoles(string name, string[] roles)
		{
			AssertRoleListCorrectness(roles);

			var update = Builders<UserModel>.Update.Set("Roles", roles);
			return _users.UpdateOneAsync(u => u.Name == name, update);
		}

		public async Task<IReadOnlyCollection<User>> GetAllUsers(int pageIndex, int pageSize)
		{
			var users = await _users
				.Find(u => true)
				.Skip(pageSize * (pageIndex - 1))
				.Limit(pageSize)
				.ToListAsync();

			return users.Select(u => new User(u.Name, u.Roles)).ToList();
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

		/// <summary>
		/// Убедиться в корректности списка ролей пользователя
		/// </summary>
		/// <param name="roles">Список ролей</param>
		/// <exception cref="IncorrectRoleListException">Список ролей некорректен</exception>
		private static void AssertRoleListCorrectness(IReadOnlyCollection<string> roles)
		{
			if (roles.Contains("buildserver") && roles.Count > 1)
			{
				throw new IncorrectRoleListException();
			}
		}

		private readonly IMongoCollection<UserModel> _users;
	}
}
