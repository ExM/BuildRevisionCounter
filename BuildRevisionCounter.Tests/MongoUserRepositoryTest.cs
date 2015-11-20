using System;
using System.Linq;
using System.Threading;
using BuildRevisionCounter.MongoDB;
using NUnit.Framework;
using System.Threading.Tasks;
using BuildRevisionCounter.DTO;
using BuildRevisionCounter.Exceptions;
using BuildRevisionCounter.MongoDB.Model;
using MongoDB.Driver;
using Moq;

namespace BuildRevisionCounter.Tests
{
	[TestFixture]
	public class MongoUserRepositoryTest
	{
		private MongoUserRepository _repository;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_repository = new MongoUserRepository(MongoDatabaseFactory.DefaultInstance);
		}

		[Test]
		public async Task DeleteUser()
		{
			var userName = "deleteUserTest_" + Guid.NewGuid();
			await _repository.CreateUser(userName, userName, new[] { "testRole" });

			await _repository.DeleteUser(userName);
			var deletedUser = await _repository.FindUserByName(userName);

			Assert.AreEqual(deletedUser, null);
		}

		[Test]
		public async Task UpdateUser()
		{
			var userName = "updateUserTest_" + Guid.NewGuid();
			var initialRoleList = new[] {"testRole"};
			var newRoleList = new[] { "testRole", "testRole2" };

			await _repository.CreateUser(userName, userName, initialRoleList);
			await _repository.UpdateUserRoles(userName, newRoleList);
			var updatedUser = await _repository.FindUserByName(userName);

			Assert.True(updatedUser.Roles.SequenceEqual(newRoleList));
		}

		[Test]
		[ExpectedException(typeof(IncorrectRoleListException))]
		public async Task UpdateUserWithIncorrectRoleList()
		{
			var userName = "updateUserIncorrectRoleTest_" + Guid.NewGuid();
			var initialRoleList = new[] { "testRole" };
			var newRoleList = new[] { "testRole", "buildserver" };

			await _repository.CreateUser(userName, userName, initialRoleList);
			await _repository.UpdateUserRoles(userName, newRoleList);
		}

		[Test]
		public async Task CreateUser()
		{
			var newUser = new User("createUserTestName_" + Guid.NewGuid(), new[] { "testRole" });
			var newUserPassword = "test";

			await _repository.CreateUser(newUser.Name, newUserPassword, newUser.Roles.ToArray());
			var createdUser = await _repository.FindUserByNameAndPassword(newUser.Name, newUserPassword);

			Assert.IsNotNull(createdUser);
			Assert.AreEqual(createdUser.Name, newUser.Name);
			Assert.True(createdUser.Roles.SequenceEqual(newUser.Roles));
		}

		[Test]
		[ExpectedException(typeof(IncorrectRoleListException))]
		public async Task CreateUserWithIncorrectRoleList()
		{
			var newUser = new User("createUserWithIncorrectRoleTestName_" + Guid.NewGuid(), new[] { "testRole", "buildserver" });
			var newUserPassword = "test";

			await _repository.CreateUser(newUser.Name, newUserPassword, newUser.Roles.ToArray());
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
			var userName = "FindUserTestUserName_" + Guid.NewGuid();
			await _repository.CreateUser(userName, "password", new[] { "testRole" });
			
			var user = await _repository.FindUserByName(userName);

			Assert.IsNotNull(user);
			Assert.AreEqual(userName, user.Name);
		}

		[Test]
		[ExpectedException(typeof (DuplicateKeyException))]
		public async Task CreateUserMustThrowExceptionIfUserExists()
		{
			var name = "duplicateUserNameTest_" + Guid.NewGuid();
			var password = "pwd";
			var roles = new[] {"testRole"};

			await _repository.CreateUser(name, password, roles);
			await _repository.CreateUser(name, password, roles);
		}

		[Test]
		public void InitRepoMustCreateAdmin()
		{
			var dbMock = new Mock<IMongoDatabase>();
			var usersCollectionMock = new Mock<IMongoCollection<UserModel>>();
			usersCollectionMock.SetupGet(c => c.Indexes).Returns(new Mock<IMongoIndexManager<UserModel>>().Object);
			dbMock.Setup(db => db.GetCollection<UserModel>(It.IsAny<string>(), null)).Returns(usersCollectionMock.Object);

			new MongoUserRepository(dbMock.Object);

			usersCollectionMock.Verify(c => c.InsertOneAsync(It.Is<UserModel>(u => u.Name == MongoUserRepository.AdminName), It.IsAny<CancellationToken>()));
		}
	}
}
