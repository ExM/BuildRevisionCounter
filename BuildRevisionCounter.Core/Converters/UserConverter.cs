using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters
{
	public static class UserConverter
	{
		public static Contract.User ToContract(this User user)
		{
			var userContract = new Contract.User {Name = user.Name, Password = user.Password, Roles = user.Roles};

			return userContract;
		}
	}
}