using System.Threading.Tasks;
using BuildRevisionCounter.Core;
using BuildRevisionCounter.Core.Converters.Impl;
using BuildRevisionCounter.Core.Repositories.Impl;
using BuildRevisionCounter.Protocol;
using MongoDB.Driver;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
	[TestFixture]
	public class MongoDBStorageTest
	{
		private UserRepository _userRepository;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_userRepository = new UserRepository(new UserConverter());
			SetUpAsync().Wait();
		}

		public async Task SetUpAsync()
		{
			await MongoContext.Instance.Client.DropDatabaseAsync(
				MongoContext.Instance.Database.DatabaseNamespace.DatabaseName);

			await MongoContext.Instance.SetUpAsync();
		}

		[Test]
		public async Task EnsureAdminUserCreated()
		{
			var user = await _userRepository.GetUserByNameAsync(MongoContext.AdminName);
			Assert.AreEqual(MongoContext.AdminName, user.Name);
		}

		[Test]
		public async Task CreateUser()
		{
			await _userRepository.CreateUserAsync(new User { Name = "test", Password = "test", Roles = new[] { "testRole" } });
		}

		[Test]
		public async Task FindUserReturnsNullIfNoUserFound()
		{
			var user = await _userRepository.GetUserByNameAsync("FindUserReturnsNullIfNoUserFound");
			Assert.IsNull(user);
		}

		[Test]
		public async Task FindUserReturnsCreatedUser()
		{
			await
				_userRepository.CreateUserAsync(new User
				{
					Name = "FindUserReturnsCreatedUser",
					Password = "FindUserReturnsCreatedUser",
					Roles = new[] { "testRole" }
				});
			var user = await _userRepository.GetUserByNameAsync("FindUserReturnsCreatedUser");
			Assert.AreEqual("FindUserReturnsCreatedUser", user.Name);
		}

		[Test]
		public async Task EnsureAdminUserMayBeInvokedMultipleTimes()
		{
			await MongoContext.Instance.EnsureAdminUser();
			await MongoContext.Instance.EnsureAdminUser();
		}

		[Test]
		public async Task CreateUserMustThrowExceptionIfUserExists()
		{
			await _userRepository.CreateUserAsync(new User
			{
				Name = "CreateUserMustThrowExceptionIfUserExists",
				Password = "CreateUserMustThrowExceptionIfUserExists",
				Roles = new[] { "testRole" }
			});

			try
			{
				await _userRepository.CreateUserAsync(new User
				{
					Name = "CreateUserMustThrowExceptionIfUserExists",
					Password = "CreateUserMustThrowExceptionIfUserExists",
					Roles = new[] { "testRole" }
				});
				Assert.Fail();
			}
			catch (MongoWriteException ex)
			{
				Assert.AreEqual(ServerErrorCategory.DuplicateKey, ex.WriteError.Category);
			}
		}
	}
}