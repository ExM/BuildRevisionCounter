using System.Threading.Tasks;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Model;
using MongoDB.Driver;

namespace BuildRevisionCounter.Data
{
	public class MongoDBUserStorage : IUserStorage, IUserDatabaseTestProvider
	{
		private readonly IMongoCollection<UserModel> _users;

		public string ConnectionString { get; private set; }

		public MongoDBUserStorage(string connectionString)
		{
			ConnectionString = connectionString;

			var mongoUrl = MongoUrl.Create(connectionString);
			var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);

			_users = database.GetCollection<UserModel>("users");

			Task.Run(() => SetUp()).Wait();
		}

		public async Task SetUp()
		{
			await EnsureUsersIndex();
			await this.EnsureAdminUser();
		}

		private async Task EnsureUsersIndex()
		{
			await _users.Indexes.CreateOneAsync(
				Builders<UserModel>.IndexKeys.Ascending(u => u.Name),
				new CreateIndexOptions {Unique = true,});
		}

		public async Task<UserModel> FindUser(string name)
		{
			return await _users.Find(u => u.Name == name).SingleOrDefaultAsync();
		}

		/// <summary>
		/// создать пользователя
		/// </summary>
		/// <param name="name">имя</param>
		/// <param name="password">пароль</param>
		/// <param name="roles">роли</param>
		/// <returns></returns>
		/// <exception cref="DuplicateKeyException">В случае дублирования имени пользователя</exception>
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
				if (ex.WriteError != null && ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
					throw new DuplicateKeyException();
				throw;
			}
		}
	}
}