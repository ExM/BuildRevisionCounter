using System.Threading.Tasks;
using BuildRevisionCounter.Core.Converters;
using MongoDB.Driver;

namespace BuildRevisionCounter.Core.Repositories.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoContext _storage;

        public UserRepository()
        {
            _storage = MongoContext.Instance;
        }

        public async Task<Contract.User> GetUserByNameAsync(string userName)
        {
            var user = await _storage.Users.Find(l => l.Name == userName).SingleOrDefaultAsync();

            return user.ToContract();
        }
    }
}