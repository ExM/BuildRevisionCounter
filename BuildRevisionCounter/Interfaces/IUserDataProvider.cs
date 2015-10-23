using System.Threading.Tasks;
using BuildRevisionCounter.Model;

namespace BuildRevisionCounter.Interfaces
{
	public interface IUserDataProvider
	{
		Task<UserModel> FindUser(string name);

		Task CreateUser(string name, string password, string[] roles);
	}
}