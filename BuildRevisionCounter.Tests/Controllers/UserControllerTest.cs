using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.DTO;
using BuildRevisionCounter.Web.Controllers;
using BuildRevisionCounter.Web.Model;
using Moq;
using NUnit.Framework;
using UrlHelper = System.Web.Http.Routing.UrlHelper;

namespace BuildRevisionCounter.Tests.Controllers
{
	[TestFixture]
	public class UserControllerTest
	{
		[Test]
		public async Task GetAllUsersAlwaysSucceed()
		{
			var mock = new Mock<IUserRepository>();
			var controller = new UserController(mock.Object);

			await controller.GetUsers(1, 10);

			mock.Verify(r => r.GetAllUsers(1, 10), Times.Once);
		}

		[Test]
		public async Task DetailsWithCorrectName()
		{
			var name = "test";
			var mock = new Mock<IUserRepository>();
			mock.Setup(u => u.FindUserByName(name)).Returns(Task.FromResult(new User(name, new []{"test"})));
			var controller = new UserController(mock.Object);
			
			var response = await controller.GetUser(name);

			mock.Verify(u => u.FindUserByName(name), Times.Once);
			Assert.IsNotNull(response);
			Assert.AreEqual(response.Name, name);
		}

		[Test]
		public void DetailsWithIncorrectName()
		{
			var name = "test";
			var mock = new Mock<IUserRepository>();
			mock.Setup(u => u.FindUserByName(name)).Returns(Task.FromResult<User>(null));
			var controller = new UserController(mock.Object);

			var ex = Assert.Catch(async () => await controller.GetUser(name)) as HttpResponseException;

			mock.Verify(u => u.FindUserByName(name), Times.Once);

			Assert.IsNotNull(ex);
			Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
		}

		[Test]
		public async Task CreateUserMustCallRepository()
		{
			var userRepoMock = new Mock<IUserRepository>();
			var urlHelperMock = new Mock<UrlHelper>();

			urlHelperMock.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://test");
			var name = "test";
			var pwd = "password";
			var roles = new[] { "test" };
			var controller = new UserController(userRepoMock.Object) { Url = urlHelperMock.Object };

			await controller.CreateUser(new CreateUserRequest{ Name = name, Password = pwd, Roles = roles });

			userRepoMock.Verify(u => u.CreateUser(name, pwd, It.Is<string[]>(l => l.SequenceEqual(roles))), Times.Once);
		}

		[Test]
		public async Task UpdateUserMustCallRepository()
		{
			var mock = new Mock<IUserRepository>();
			var controller = new UserController(mock.Object);
			var name = "test";
			var roles = new[] { "test" };

			await controller.UpdateUser(name, roles);

			mock.Verify(u => u.UpdateUserRoles(name, It.Is<string[]>(l => l.SequenceEqual(roles))), Times.Once);
		}

		[Test]
		public async Task DeleteAlwaysSucceed()
		{
			var mock = new Mock<IUserRepository>();
			var controller = new UserController(mock.Object);
			var name = "test";

			await controller.DeleteUser(name);

			mock.Verify(r => r.DeleteUser(name), Times.Once);
		}
	}
}