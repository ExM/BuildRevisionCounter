using System.Threading.Tasks;
using BuildRevisionCounter.Core.Converters;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver;


namespace BuildRevisionCounter.Core.Repositories.Impl
{
	internal class UserRepository : IUserRepository
	{
		private readonly MongoContext _storage;
		private readonly IUserConverter _userConverter;

		public UserRepository(IUserConverter userConverter)
		{
			_storage = MongoContext.Instance;
			_userConverter = userConverter;
		}

		#region IUserRepository

		public async Task<Protocol.User> GetUserByNameAsync(string userName)
		{
			var domainUser = await GetDomainUserByNameAsync(userName);
			
			if (domainUser == null) return null;
			return _userConverter.ToProtocol(domainUser);
		}

		public async Task CreateUserAsync(Protocol.User user)
		{
			var domainUser = _userConverter.ToDomain(user);

			await CreateDomainUserAsync(domainUser);
		}

		public async Task<long> CountAsync()
		{
			return await _storage.Users.CountAsync(_ => true);
		}

		#endregion

		internal async Task<User> GetDomainUserByNameAsync(string userName)
		{
			var user = await _storage.Users
				.Find(u => u.Name == userName)
				.FirstOrDefaultAsync();

			return user;
		}

		internal async Task CreateDomainUserAsync(User domainUser)
		{
			await _storage.Users.InsertOneAsync(domainUser);
		}
		
	}
}