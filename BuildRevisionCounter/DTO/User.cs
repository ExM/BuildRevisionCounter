using System.Collections.Generic;
using System.Linq;

namespace BuildRevisionCounter.DTO
{
	/// <summary>
	/// DTO пользователя, неизменяемый.
	/// </summary>
	public class User
	{
		/// <summary>
		/// Имя пользователя.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Роли пользователя.
		/// </summary>
		public IReadOnlyCollection<string> Roles { get; private set; }

		/// <summary>
		/// Создает новый экземпляр пользователя.
		/// </summary>
		/// <param name="name">Наименование пользователя.</param>
		/// <param name="roles">Роли  пользователя.</param>
		public User(string name, IEnumerable<string> roles)
		{
			Name = name;
			Roles = roles.ToList();
		}
	}
}