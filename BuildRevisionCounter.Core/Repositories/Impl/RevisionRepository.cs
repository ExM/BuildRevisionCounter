using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<List<Protocol.Revision>> GetAllRevisionAsync(Int32 pageSize = 20, Int32 pageNumber = 1)
		{
			var domainRevisions = await GetAllDomainRevisionAsync(pageSize, pageNumber);
			
			if (domainRevisions == null) return null;
			var protocolRevisions = domainRevisions.Select(l => _revisionConverter.ToProtocol(l)).ToList();
			return protocolRevisions;
		}

		#endregion

		internal async Task<List<Revision>> GetAllDomainRevisionAsync(Int32 pageSize = 20, Int32 pageNumber = 1)
		{
			var revisions = await _storage.Revisions
				.Find(r => true)
				.Skip(pageSize * (pageNumber - 1))
				.Limit(pageSize)
				.ToListAsync();

			return revisions;
		}

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

			// попробуем обновить документ
			var result = await FindOneAndUpdateRevisionAsync(revisionId);
			if (result != null) return result;

			// если не получилось, значит документ еще не был создан
			// создадим его с начальным значением 0
			try
			{
				await _storage.Revisions
					.InsertOneAsync(new Revision
					{
						Id = revisionId,
						CurrentNumber = 0,
						Created = DateTime.UtcNow
					});

				result = await GetDomainRevisionByIdAsync(revisionId);
				return result;
			}
			catch (MongoWriteException ex)
			{
				if (ex.WriteError.Category != ServerErrorCategory.DuplicateKey)
					throw;
			}

			// если при вставке произошла ошибка значит мы не успели и запись там уже есть
			// и теперь попытка обновления должна пройти без ошибок
			result = await FindOneAndUpdateRevisionAsync(revisionId);

			return result;
		}

		/// <summary>
		/// Инкриментит каунтер в БД
		/// </summary>
		/// <param name="revisionName"></param>
		/// <returns></returns>
		internal async Task<Revision> FindOneAndUpdateRevisionAsync(string revisionName)
		{
			var result = await _storage.Revisions
				.FindOneAndUpdateAsync<Revision>(
					r => r.Id == revisionName,
					Builders<Revision>.Update
						.Inc(r => r.CurrentNumber, 1)
						.SetOnInsert(r => r.Created, DateTime.UtcNow)
						.Set(r => r.Updated, DateTime.UtcNow),
					new FindOneAndUpdateOptions<Revision>
					{
						IsUpsert = false,
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
