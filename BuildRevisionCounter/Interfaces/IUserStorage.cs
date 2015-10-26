using System.Threading.Tasks;
using BuildRevisionCounter.Model;

namespace BuildRevisionCounter.Interfaces
{
	public interface IUserStorage
	{
		Task<UserModel> FindUser(string name);

		Task CreateUser(string name, string password, string[] roles);
	}
}