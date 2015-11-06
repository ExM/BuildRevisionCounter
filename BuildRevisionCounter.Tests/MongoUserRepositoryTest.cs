using BuildRevisionCounter.MongoDB;
using MongoDB.Driver;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BuildRevisionCounter.Tests
{
	[TestFixture]
	public class MongoUserRepositoryTest
	{
		private MongoUserRepository _repository;

		[TestFixtureSetUp]
		public void SetUp()
		{
			SetUpAsync().Wait();
		}

		public async Task SetUpAsync()
		{
			MongoDBStorageUtils.SetUpAsync().Wait();			
			_repository = new MongoUserRepository(MongoDatabaseFactory.DefaultInstance);
		}


		[Test]
		public async Task EnsureAdminUserCreated()
		{
			var user = await _repository.FindUserByName(MongoUserRepository.AdminName);
			Assert.AreEqual(MongoUserRepository.AdminName, user.Name);
		}

		[Test]
		public async Task CreateUser()
		{
			await _repository.CreateUser("test", "test", new[] { "testRole" });
		}

		[Test]
		public async Task FindUserReturnsNullIfNoUserFound()
		{
			var user = await _repository.FindUserByName("FindUserReturnsNullIfNoUserFound");
			Assert.IsNull(user);
		}

		[Test]
		public async Task FindUserReturnsCreatedUser()
		{
			await _repository.CreateUser("FindUserReturnsCreatedUser", "FindUserReturnsCreatedUser", new[] { "testRole" });
			var user = await _repository.FindUserByName("FindUserReturnsCreatedUser");
			Assert.AreEqual("FindUserReturnsCreatedUser", user.Name);
		}

		[Test]
		[ExpectedException(typeof(DuplicateKeyException))]
		public async Task CreateUserMustThrowExceptionIfUserExists()
		{
			await _repository.CreateUser(
				"CreateUserMustThrowExceptionIfUserExists",
				"CreateUserMustThrowExceptionIfUserExists",
				new[] { "testRole" });


			await _repository.CreateUser(
				"CreateUserMustThrowExceptionIfUserExists",
				"CreateUserMustThrowExceptionIfUserExists",
				new[] { "testRole" });
		}
	}
}
