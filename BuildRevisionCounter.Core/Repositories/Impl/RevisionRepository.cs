using System;
using System.Threading.Tasks;
using BuildRevisionCounter.Core.Converters;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver;


namespace BuildRevisionCounter.Core.Repositories.Impl
{
	internal class RevisionRepository : IRevisionRepository
	{
		private readonly MongoContext _storage;
		private readonly IRevisionConverter _revisionConverter;

		public RevisionRepository(IRevisionConverter revisionConverter)
		{
			_storage = MongoContext.Instance;
			_revisionConverter = revisionConverter;
		}

		private void CheckRevisionId(string revisionId)
		{
			if (String.IsNullOrEmpty(revisionId)) throw new ArgumentException("revisionId can't be null or empty", "revisionId");
		}

		#region IRevisionRepository

		public async Task<Protocol.Revision> GetRevisionByIdAsync(string revisionId)
		{
			var domainRevision = await GetDomainRevisionByIdAsync(revisionId);
			if (domainRevision == null) return null;
			return _revisionConverter.ToProtocol(domainRevision);
		}

		public async Task<Protocol.Revision> IncrementRevisionAsync(string revisionId)
		{
			var domainRevision = await IncrementDomainRevisionAsync(revisionId);

			return _revisionConverter.ToProtocol(domainRevision);
		}

		public async Task AddRevisionAsync(Protocol.Revision revision)
		{
			var domainRevision = _revisionConverter.ToDomain(revision);
			await AddDomainRevisionAsync(domainRevision);
		}

		public async Task ClearRevisionCollectionAsync()
		{
			await _storage.Revisions.DeleteManyAsync(l => true);
		}

		#endregion

		internal async Task<Revision> GetDomainRevisionByIdAsync(string revisionId)
		{
			CheckRevisionId(revisionId);

			var revision = await _storage.Revisions
				.Find(_ => _.Id == revisionId)
				.SingleOrDefaultAsync();
			
			return revision;
		}

		internal async Task<Revision> IncrementDomainRevisionAsync(string revisionId)
		{
			CheckRevisionId(revisionId);

			var result = await _storage.Revisions
				.FindOneAndUpdateAsync<Revision>(
					r => r.Id == revisionId,
					Builders<Revision>.Update
						.Inc(r => r.NextNumber, 1)
						.SetOnInsert(r => r.Created, DateTime.UtcNow)
						.Set(r => r.Updated, DateTime.UtcNow),
					new FindOneAndUpdateOptions<Revision>
					{
						IsUpsert = true,
						ReturnDocument = ReturnDocument.After
					});

			return result;
		}

		internal async Task AddDomainRevisionAsync(Revision revision)
		{
			await _storage.Revisions.InsertOneAsync(revision);
		}

		
	}
}
