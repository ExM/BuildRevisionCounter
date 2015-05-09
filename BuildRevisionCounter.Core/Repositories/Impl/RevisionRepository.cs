using System;
using BuildRevisionCounter.Core.Converters;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BuildRevisionCounter.Core.Repositories.Impl
{
	internal class RevisionRepository : IRevisionRepository
	{
		private readonly MongoContext _storage;

		public RevisionRepository()
		{
			_storage = MongoContext.Instance;
		}

		private void CheckRevisionId(string revisionId)
		{
			if (String.IsNullOrEmpty(revisionId)) throw new ArgumentException("revisionId can't be null or empty", "revisionId");
		}

		public Contract.Revision GetRevisionById(string revisionId)
		{
			CheckRevisionId(revisionId);

			var q = Query<Revision>.Where(_ => _.Id == revisionId);
			var revision = _storage.Revisions.FindOne(q);

			if (revision == null) return null;
			return revision.ToContract();
		}

		public Contract.Revision IncrementRevision(string revisionId)
		{
			CheckRevisionId(revisionId);

			var result = _storage.Revisions.FindAndModify(new FindAndModifyArgs()
			{
				Query = Query<Revision>.Where(_ => _.Id == revisionId),
				Upsert = true,
				Update = Update<Revision>
					.SetOnInsert(_ => _.Created, DateTime.UtcNow)
					.Inc(_ => _.NextNumber, 1)
					.Set(_ => _.Updated, DateTime.UtcNow),
				VersionReturned = FindAndModifyDocumentVersion.Modified
			});

			var revision = result.GetModifiedDocumentAs<Revision>();

			return revision.ToContract();
		}
	}
}
