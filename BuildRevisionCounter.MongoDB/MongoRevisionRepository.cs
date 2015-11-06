using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using BuildRevisionCounter.MongoDB.Model;

namespace BuildRevisionCounter.MongoDB
{
	public class MongoRevisionRepository : IRevisionRepository
	{
		public MongoRevisionRepository(IMongoDatabase database)
		{
			_revisions = database.GetCollection<RevisionModel>("revisions");
		}

		public async Task<IReadOnlyCollection<DTO.Revision>> GetAllRevision(int pageSize = 20, int pageNumber = 1)
		{
			var revisions = await _revisions
				.Find(r => true)
				.Skip(pageSize * (pageNumber - 1))
				.Limit(pageSize)
				.ToListAsync();

			return revisions.Select(itr=> new DTO.Revision(itr.Id, itr.Created, itr.Updated, itr.CurrentNumber)).ToList();
		}

		public async Task<long> Current(string revisionName)
		{
			var revision = await _revisions
				.Find(r => r.Id == revisionName)
				.SingleOrDefaultAsync();

			if (revision == null)
				throw new RevisionNotFoundException();

			return revision.CurrentNumber;
		}

		public async Task<long> Bumping(string revisionName)
		{
			// попробуем обновить документ
			var result = await FindOneAndUpdateRevisionModelAsync(revisionName);
			if (result != null)
				return result.CurrentNumber;

			// если не получилось, значит документ еще не был создан
			// создадим его с начальным значением 0
			try
			{
				await _revisions
					.InsertOneAsync(new RevisionModel
					{
						Id = revisionName,
						CurrentNumber = 0,
						Created = DateTime.UtcNow
					});
				return 0;
			}
			catch (MongoWriteException ex)
			{
				if (ex.WriteError.Category != ServerErrorCategory.DuplicateKey)
					throw;
			}

			// если при вставке произошла ошибка значит мы не успели и запись там уже есть
			// и теперь попытка обновления должна пройти без ошибок
			result = await FindOneAndUpdateRevisionModelAsync(revisionName);

			return result.CurrentNumber;
		}

		private async Task<RevisionModel> FindOneAndUpdateRevisionModelAsync(string revisionName)
		{
			var result = await _revisions
				.FindOneAndUpdateAsync<RevisionModel>(
					r => r.Id == revisionName,
					Builders<RevisionModel>.Update
						.Inc(r => r.CurrentNumber, 1)
						.SetOnInsert(r => r.Created, DateTime.UtcNow)
						.Set(r => r.Updated, DateTime.UtcNow),
					new FindOneAndUpdateOptions<RevisionModel>
					{
						IsUpsert = false,
						ReturnDocument = ReturnDocument.After
					});

			return result;
		}

		private readonly IMongoCollection<RevisionModel> _revisions;
	}
}
