using BuildRevisionCounter.Contract;

namespace BuildRevisionCounter.Core.Repositories
{
	public interface IUserRepository
	{
		User GetUserByName(string userName);
	}
}