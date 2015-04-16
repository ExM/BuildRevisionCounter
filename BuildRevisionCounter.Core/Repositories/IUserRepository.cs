using System.Threading.Tasks;
using BuildRevisionCounter.Contract;

namespace BuildRevisionCounter.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByNameAsync(string userName);
    }
}