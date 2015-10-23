using System.Threading.Tasks;
using BuildRevisionCounter.Model;

namespace BuildRevisionCounter.Interfaces
{
	/// <summary>
	/// Интерфейс для проверки операций с БД.
	/// </summary>
	public interface IDatabaseTestProvider
	{
			Task SetUp();

			Task DropDatabaseAsync();

			Task<UserModel> FindUser(string name);

			Task CreateUser(string name, string password, string[] roles);
	}
}