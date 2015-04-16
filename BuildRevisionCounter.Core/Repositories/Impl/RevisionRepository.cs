using System;
using System.Threading.Tasks;
using BuildRevisionCounter.Core.Converters;
using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver;

namespace BuildRevisionCounter.Core.Repositories.Impl
{
    public class RevisionRepository : IRevisionRepository
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

        public async Task<Contract.Revision> GetRevisionByIdAsync(string revisionId)
        {
            CheckRevisionId(revisionId);

            var revision = await _storage.Revisions.Find(l => l.Id == revisionId).SingleOrDefaultAsync();

            if (revision == null) return null;
            return revision.ToContract();
        }

        public async Task<Contract.Revision> IncrementRevisionAsync(string revisionId)
        {
            CheckRevisionId(revisionId);

            var options = new FindOneAndUpdateOptions<Revision>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var revision = await _storage.Revisions.FindOneAndUpdateAsync(
                Builders<Revision>.Filter.Eq(r => r.Id, revisionId),
                Builders<Revision>.Update
                    .SetOnInsert(l => l.Created, DateTime.UtcNow)
                    .Inc(l => l.NextNumber, 1)
                    .Set(l => l.Updated, DateTime.UtcNow),
                options); 

            return revision.ToContract();
        }
    }
}
