using System.Threading.Tasks;

namespace BuildRevisionCounter.Core.Repositories
{
	public interface IUserRepository
	{
		Task<Protocol.User> GetUserByNameAsync(string userName);

		Task CreateUserAsync(Protocol.User user);

		Task<long> CountAsync();
	}
}