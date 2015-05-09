using BuildRevisionCounter.Core.Converters;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver.Builders;

namespace BuildRevisionCounter.Core.Repositories.Impl
{
	internal class UserRepository : IUserRepository
	{
		private readonly MongoContext _storage;

		public UserRepository()
		{
			_storage = MongoContext.Instance;
		}

		public Contract.User GetUserByName(string userName)
		{
			var user = _storage.Users.FindOne(Query<User>.Where(u => u.Name == userName));

			return user.ToContract();
		}
	}
}