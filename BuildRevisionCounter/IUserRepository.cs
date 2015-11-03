using BuildRevisionCounter.DTO;
using System.Threading.Tasks;

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
		Task CreateUser(string name, string password, string[] roles);
	}
}
