using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Model;
using MongoDB.Driver;

namespace BuildRevisionCounter.Data
{
	public class MongoDBStorage : IDataProvider, IDatabaseTestProvider
	{
		public string AdminName
		{
			get { return "admin"; }
		}

		public static readonly string AdminPassword = "admin";
		public static readonly string[] AdminRoles = {"admin", "buildserver", "editor"};

		private readonly IMongoCollection<RevisionModel> _revisions;
		private readonly IMongoCollection<UserModel> _users;

		public MongoDBStorage(string connectionString)
		{
			var mongoUrl = MongoUrl.Create(connectionString);
			var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
			
			_revisions = database.GetCollection<RevisionModel>("revisions");
			_users = database.GetCollection<UserModel>("users");

			Task.Run(() => SetUp()).Wait();
		}

		public async Task SetUp()
		{
			await EnsureUsersIndex();
			await EnsureAdminUser();
		}

		public Task DropDatabaseAsync()
		{
			return _revisions.Database.Client.DropDatabaseAsync(_revisions.Database.DatabaseNamespace.DatabaseName);
		}

		public async Task<IReadOnlyCollection<RevisionModel>> GetAllRevision(Int32 pageSize, Int32 pageNumber)
		{
			var revisions = await _revisions
				.Find(r => true)
				.Skip(pageSize * (pageNumber - 1))
				.Limit(pageSize)
				.ToListAsync();

			return revisions;
		}

		public async Task<long?> CurrentRevision(string revisionName)
		{
			var revision = await _revisions
				.Find(r => r.Id == revisionName)
				.SingleOrDefaultAsync();

			if (revision == null)
				return null;

			return revision.CurrentNumber;
		}

		public async Task RevisionInsertAsync(string revisionName)
		{
			try
			{
				await _revisions
					.InsertOneAsync(new RevisionModel
					{
						Id = revisionName,
						CurrentNumber = 0,
						Created = DateTime.UtcNow
					});
			}
			catch (MongoWriteException ex)
			{
				if (ex.WriteError != null && ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
					throw new DuplicateKeyException();
				throw;
			}
		}

		public async Task<RevisionModel> FindOneAndUpdateRevisionModelAsync(string revisionName)
		{
			var result = await _revisions
				.FindOneAndUpdateAsync<RevisionModel>(
					r => r.Id == revisionName,
					Builders<RevisionModel>.Update
						.Inc(r => r.CurrentNumber, 1)
						.SetOnInsert(r => r.Created, DateTime.UtcNow)
						.Set(r => r.Updated, DateTime.UtcNow),
					new FindOneAndUpdateOptions<RevisionModel>
					{
						IsUpsert = false,
						ReturnDocument = ReturnDocument.After
					});

			return result;
		}

		public async Task EnsureUsersIndex()
		{
			await _users.Indexes.CreateOneAsync(
				Builders<UserModel>.IndexKeys.Ascending(u => u.Name),
				new CreateIndexOptions { Unique = true,  });
		}

		public async Task EnsureAdminUser()
		{
			if (await _users.CountAsync(_ => true) == 0)
			{
				await CreateUser(AdminName, AdminPassword, AdminRoles);
			}
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