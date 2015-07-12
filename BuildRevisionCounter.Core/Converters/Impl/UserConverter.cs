using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters.Impl
{
	internal class UserConverter : IUserConverter
	{
		public Protocol.User ToProtocol(User user)
		{
			var userProtocol = new Protocol.User { Name = user.Name, Password = user.Password, Roles = user.Roles };

			return userProtocol;
		}

		public User ToDomain(Protocol.User user)
		{
			var userDomain = new User { Name = user.Name, Password = user.Password, Roles = user.Roles };

			return userDomain;
		}
	}
}