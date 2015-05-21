using System.Threading.Tasks;
using BuildRevisionCounter.DAL.Repositories;
using MongoDB.Driver;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
	[TestFixture]
	public class MongoDBStorageTest
	{
	    private UserRepository _userRepo;
	    private RevisionRepository _revisionRepo;

	    [TestFixtureSetUp]
		public void SetUp()
		{
			SetUpAsync().Wait();
		}

		private async Task SetUpAsync()
        {
            _userRepo = RepositoriesFactory.UserRepoInstance;
            _revisionRepo = RepositoriesFactory.RevisionRepoInstance;

            await _revisionRepo.DropDatabaseAsync();
            await _userRepo.CreateOneAsync();
            await _userRepo.EnsureAdminUser();
        }

		[Test]
		public async Task EnsureAdminUserCreated()
		{
            var user = await _userRepo.FindUser(UserRepository.AdminName);
            Assert.AreEqual(UserRepository.AdminName, user.Name);
		}

		[Test]
		public async Task CreateUser()
		{
            await _userRepo.CreateUser("test", "test", new[] { "testRole" });
		}

		[Test]
		public async Task FindUserReturnsNullIfNoUserFound()
		{
            var user = await _userRepo.FindUser("FindUserReturnsNullIfNoUserFound");
			Assert.IsNull(user);
		}

		[Test]
		public async Task FindUserReturnsCreatedUser()
		{
            await _userRepo.CreateUser("FindUserReturnsCreatedUser", "FindUserReturnsCreatedUser", new[] { "testRole" });
            var user = await _userRepo.FindUser("FindUserReturnsCreatedUser");
			Assert.AreEqual("FindUserReturnsCreatedUser", user.Name);
		}

		[Test]
		public async Task EnsureAdminUserMayBeInvokedMultipleTimes()
		{
            await _userRepo.EnsureAdminUser();
            await _userRepo.EnsureAdminUser();
		}

		[Test]
		public async Task CreateUserMustThrowExceptionIfUserExists()
		{
            await _userRepo.CreateUser(
				"CreateUserMustThrowExceptionIfUserExists",
				"CreateUserMustThrowExceptionIfUserExists",
				new[] {"testRole"});

			try
			{
                await _userRepo.CreateUser(
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