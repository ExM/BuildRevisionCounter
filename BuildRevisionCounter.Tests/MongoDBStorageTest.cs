using System.Threading.Tasks;
using MongoDB.Driver;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
	[TestFixture]
	public class MongoDBStorageTest
	{
		private MongoDBStorage _storage;

		[TestFixtureSetUp]
		public void SetUp()
		{
			SetUpAsync().Wait();
		}

		public async Task SetUpAsync()
		{
			_storage = MongoDBStorageFactory.DefaultInstance;

			await _storage.Revisions.Database.Client.DropDatabaseAsync(
				_storage.Revisions.Database.DatabaseNamespace.DatabaseName);

			await _storage.SetUp();
		}

		[Test]
		public async Task EnsureAdminUserCreated()
		{
			var user = await _storage.FindUser(MongoDBStorage.AdminName);
			Assert.AreEqual(MongoDBStorage.AdminName, user.Name);
		}

		[Test]
		public async Task CreateUser()
		{
			await _storage.CreateUser("test", "test", new[] {"testRole"});
		}

		[Test]
		public async Task FindUserReturnsNullIfNoUserFound()
		{
			var user = await _storage.FindUser("FindUserReturnsNullIfNoUserFound");
			Assert.IsNull(user);
		}

		[Test]
		public async Task FindUserReturnsCreatedUser()
		{
			await _storage.CreateUser("FindUserReturnsCreatedUser", "FindUserReturnsCreatedUser", new[] {"testRole"});
			var user = await _storage.FindUser("FindUserReturnsCreatedUser");
			Assert.AreEqual("FindUserReturnsCreatedUser", user.Name);
		}

		[Test]
		public async Task EnsureAdminUserMayBeInvokedMultipleTimes()
		{
			await _storage.EnsureAdminUser();
			await _storage.EnsureAdminUser();
		}

		[Test]
		public async Task CreateUserMustThrowExceptionIfUserExists()
		{
			await _storage.CreateUser(
				"CreateUserMustThrowExceptionIfUserExists",
				"CreateUserMustThrowExceptionIfUserExists",
				new[] {"testRole"});

			try
			{
				await _storage.CreateUser(
					"CreateUserMustThrowExceptionIfUserExists",
					"CreateUserMustThrowExceptionIfUserExists",
					new[] {"testRole"});
				Assert.Fail();
			}
			catch (MongoWriteException ex)
			{
				Assert.AreEqual(ServerErrorCategory.DuplicateKey, ex.WriteError.Category);
			}
		}
	}
}