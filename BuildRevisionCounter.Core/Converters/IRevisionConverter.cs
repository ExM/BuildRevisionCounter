using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters
{
	internal interface IRevisionConverter
	{
		Protocol.Revision ToProtocol(Revision revision);

		Revision ToDomain(Protocol.Revision revision);
	}
}