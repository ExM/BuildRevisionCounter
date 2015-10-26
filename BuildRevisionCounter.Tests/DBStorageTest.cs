using System.Threading.Tasks;
using BuildRevisionCounter.Data;
using BuildRevisionCounter.Interfaces;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
	[TestFixture]
	public class DBStorageTest
	{
		private IUserDatabaseTestProvider _storage;

		[TestFixtureSetUp]
		public void SetUp()
		{
			SetUpAsync().Wait();
		}

		public async Task SetUpAsync()
		{
			_storage = DBStorageFactory.GetInstance<MongoDBUserStorage>();
			await _storage.SetUp();
		}

		[Test]
		public async Task EnsureAdminUserCreated()
		{
			var adminName = _storage.GetAdminName();
			var user = await _storage.FindUser(adminName);
			Assert.IsNotNull(user);
			Assert.AreEqual(adminName, user.Name);
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
			const string userName = "CreateUserMustThrowExceptionIfUserExists";
			const string userPass = "CreateUserMustThrowExceptionIfUserExists";
			var userRole = new[] {"testRole"};
			await _storage.CreateUser(userName, userPass, userRole);
			try
			{
				await _storage.CreateUser(userName, userPass, userRole);
				Assert.Fail();
			}
			catch (DuplicateKeyException)
			{
			}
		}

		[TestFixtureTearDown]
		public void DropDatabaseAsync()
		{
			_storage.DropDatabaseAsync().Wait();
		}
	}
}