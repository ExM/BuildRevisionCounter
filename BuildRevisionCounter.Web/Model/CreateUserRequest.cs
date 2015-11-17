namespace BuildRevisionCounter.Web.Model
{
	/// <summary>
	/// Запрос на создание пользователя
	/// </summary>
	public class CreateUserRequest
	{
		/// <summary>
		/// Имя пользователя
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Пароль пользователя
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Роли пользователя
		/// </summary>
		public string[] Roles { get; set; }
	}
}