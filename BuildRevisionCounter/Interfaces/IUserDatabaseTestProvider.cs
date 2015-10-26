using System.Threading.Tasks;
using BuildRevisionCounter.Model;

namespace BuildRevisionCounter.Interfaces
{
	/// <summary>
	/// Интерфейс для проверки операций с БД.
	/// </summary>
	public interface IUserDatabaseTestProvider
	{
			string ConnectionString { get; }
			
			Task SetUp();
			
			Task<UserModel> FindUser(string name);
			
			Task CreateUser(string name, string password, string[] roles);
	}
}