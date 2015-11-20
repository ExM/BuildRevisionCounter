using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.DTO;
using BuildRevisionCounter.Web.Filters;
using BuildRevisionCounter.Web.Model;
using BuildRevisionCounter.Web.Security;

namespace BuildRevisionCounter.Web.Controllers
{
	[RoutePrefix("api/users")]
	[BasicAuthentication]
	[Authorize(Roles = "admin")]
	public class UserController : ApiController
	{
		private readonly IUserRepository _userRepository;

		/// <summary>
		/// Конструктор контроллера пользователей
		/// </summary>
		/// <param name="userRepository">Репозиторий пользователей</param>
		public UserController(IUserRepository userRepository)
		{
			if (userRepository == null)
				throw new ArgumentException("userRepository");

			_userRepository = userRepository;
		}

		/// <summary>
		/// Получить список всех пользователей
		/// </summary>
		/// <param name="pageIndex">Номер страницы списка</param>
		/// <param name="pageSize">Размер страницы списка</param>
		[HttpGet]
		[Route("")]
		public Task<IReadOnlyCollection<User>> GetUsers([FromUri] int pageIndex = 1, [FromUri] int pageSize = 20)
		{
			return _userRepository.GetAllUsers(pageIndex, pageSize);
		}

		/// <summary>
		/// Получить детальную информацию о пользователе
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		[HttpGet]
		[Route("{name}", Name = "GetUserByName")]
		public async Task<User> GetUser(string name)
		{
			var user = await _userRepository.FindUserByName(name);

			if (user == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return user;
		}

		/// <summary>
		/// Создать пользователя
		/// </summary>
		/// <param name="request">Запрос на добавление пользователя</param>
		/// <returns>Результат выполнения операции</returns>
		[HttpPost]
		[Route("")]
		public Task<User> CreateUser([FromBody] CreateUserRequest request)
		{
			return _userRepository.CreateUser(request.Name, request.Password, request.Roles);
		}

		/// <summary>
		/// Удалить пользователя
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		/// <returns>Результат выполнения операции</returns>
		[HttpDelete]
		[Route("{name}")]
		public async Task DeleteUser(string name)
		{
			await _userRepository.DeleteUser(name);
		}

		/// <summary>
		/// Обновить список ролей пользователя
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		/// <param name="roles">Новый список ролей</param>
		/// <returns>Результат выполнения операции</returns>
		[HttpPut]
		[Route("{name}")]
		public async Task UpdateUser(string name, [FromBody] string[] roles)
		{
			await _userRepository.UpdateUserRoles(name, roles);
		}
	}
}
