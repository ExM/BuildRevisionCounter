using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters
{
	internal interface IUserConverter
	{
		Protocol.User ToProtocol(User user);

		User ToDomain(Protocol.User user);
	}
}