using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Model;
using BuildRevisionCounter.Security;
using MongoDB.Driver;

namespace BuildRevisionCounter.Data
{
	public class MongoDBRevisionStorage : IRevisionStorage
	{
		private readonly IMongoCollection<RevisionModel> _revisions;

		public string ConnectionString { get; private set; }

		public MongoDBRevisionStorage(string connectionString)
		{
			ConnectionString = connectionString;

			var mongoUrl = MongoUrl.Create(connectionString);
			var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);

			_revisions = database.GetCollection<RevisionModel>("revisions");
		}

		public async Task<IReadOnlyCollection<RevisionModel>> GetAllRevision(Int32 pageSize, Int32 pageNumber)
		{
			var revisions = await _revisions
				.Find(r => true)
				.Skip(pageSize * (pageNumber - 1))
				.Limit(pageSize)
				.ToListAsync();

			return revisions;
		}

		public async Task<long?> CurrentRevision(string revisionName)
		{
			var revision = await _revisions
				.Find(r => r.Id == revisionName)
				.SingleOrDefaultAsync();

			if (revision == null)
				return null;

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
			if (await RevisionInsertAsync(revisionName))
				return 0;
			
			// если при вставке произошла ошибка значит мы не успели и запись там уже есть
			// и теперь попытка обновления должна пройти без ошибок
			result = await FindOneAndUpdateRevisionModelAsync(revisionName);

			return result.CurrentNumber;
		}

		private async Task<bool> RevisionInsertAsync(string revisionName)
		{
			try
			{
				await _revisions
					.InsertOneAsync(new RevisionModel
					{
						Id = revisionName,
						CurrentNumber = 0,
						Created = DateTime.UtcNow
					});
				return true;
			}
			catch (MongoWriteException ex)
			{
				if (ex.WriteError != null && ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
					return false;
				throw;
			}
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
	}
}