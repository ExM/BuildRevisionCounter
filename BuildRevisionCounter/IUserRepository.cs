using System.Collections.Generic;
using BuildRevisionCounter.DTO;
using System.Threading.Tasks;
using BuildRevisionCounter.Exceptions;

namespace BuildRevisionCounter
{
	/// <summary>
	/// Интерфейс репозитория пользователей.
	/// </summary>
	public interface IUserRepository
	{
		/// <summary>
		/// Производит поиск пользователя по имени.
		/// </summary>
		/// <param name="name">Имя пользователя.</param>
		/// <returns>Найденый пользватель или null.</returns>
		Task<User> FindUserByName(string name);

		/// <summary>
		/// Создает пользователя.
		/// </summary>
		/// <param name="name">Наименование пользователя.</param>
		/// <param name="password">Пароль пользователя.</param>
		/// <param name="roles">Роли пользователя.</param>
		/// <returns>Таск.</returns>
		/// <exception cref="DuplicateKeyException">Генерироуется в случае попытки создать пользователя с именем которое уже есть в хранилище.</exception>
		/// <exception cref="IncorrectRoleListException">Передан некорректный список ролей</exception>
		/// <returns>Объект, который был вставлен в БД</returns>
		Task<User> CreateUser(string name, string password, string[] roles);

		/// <summary>
		/// Удаляет пользователя.
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		Task DeleteUser(string name);

		/// <summary>
		/// Обновляет роли пользователя
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		/// <param name="roles">Требуемые роли пользователя</param>
		/// <exception cref="IncorrectRoleListException">Передан некорректный список ролей</exception>
		Task UpdateUserRoles(string name, string[] roles);

		/// <summary>
		/// Получить список всех существующих пользователей
		/// </summary>
		/// <param name="pageIndex">Номер страницы списка</param>
		/// <param name="pageSize">Размер страницы списка</param>
		Task<IReadOnlyCollection<User>> GetAllUsers(int pageIndex, int pageSize);

		/// <summary>
		/// Получить пользователя по имени и паролю
		/// </summary>
		/// <param name="userName">Имя пользователя</param>
		/// <param name="password">Пароль пользователя</param>
		Task<User> FindUserByNameAndPassword(string userName, string password);
	}
}
