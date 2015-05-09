using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters
{
	public static class RevisionConverter
	{
		public static Contract.Revision ToContract(this Revision revision)
		{
			var revisionContract = new Contract.Revision
			{
				Id = revision.Id,
				NextNumber = revision.NextNumber,
				Updated = revision.Updated,
				Created = revision.Created
			};

			return revisionContract;
		}
	}
}