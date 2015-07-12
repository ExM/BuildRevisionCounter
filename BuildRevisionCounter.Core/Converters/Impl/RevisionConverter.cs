using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters.Impl
{
	internal class RevisionConverter : IRevisionConverter
	{
		public Protocol.Revision ToProtocol(Revision revision)
		{
			var revisionProtocol = new Protocol.Revision
			{
				Id = revision.Id,
				CurrentNumber = revision.CurrentNumber,
				Updated = revision.Updated,
				Created = revision.Created
			};

			return revisionProtocol;
		}

		public Revision ToDomain(Protocol.Revision revision)
		{
			var revisionDomain = new Revision
			{
				Id = revision.Id,
				CurrentNumber = revision.CurrentNumber,
				Updated = revision.Updated,
				Created = revision.Created
			};

			return revisionDomain;
		}
	}
}