using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildRevisionCounter.Core.Repositories.Impl;
using System.Net;
using System.Web.Http;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Model;
using BuildRevisionCounter.Security;
using MongoDB.Driver;

namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication]
	public class CounterController : ApiController
	{

		[HttpGet]
		[Route("")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<IReadOnlyCollection<RevisionModel>> GetAllRevision([FromUri] Int32 pageSize = 20, [FromUri] Int32 pageNumber = 1)
		{
			if (pageSize < 1 || pageNumber < 1)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			var revisions = await _mongoDbStorage.Revisions
				.Find(r => true)
				.Skip(pageSize * (pageNumber - 1))
				.Limit(pageSize)
				.ToListAsync();

			if (revisions == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revisions;
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<long> Current([FromUri] string revisionName)
		{
			var revision = await _mongoDbStorage.Revisions
				.Find(r => r.Id == revisionName)
				.SingleOrDefaultAsync();

			if (revision == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revision.CurrentNumber;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public async Task<long> Bumping([FromUri] string revisionName)
		{
			// попробуем обновить документ
			var result = await FindOneAndUpdateRevisionModelAsync(revisionName);
			if (result != null)
				return result.CurrentNumber;

			// если не получилось, значит документ еще не был создан
			// создадим его с начальным значением 0
			try
			{
				await _mongoDbStorage.Revisions
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

		/// <summary>
		///		Инкриментит каунтер в БД
		/// </summary>
		/// <param name="revisionName"></param>
		/// <returns></returns>
		private async Task<RevisionModel> FindOneAndUpdateRevisionModelAsync(string revisionName)
		{
			var result = await _mongoDbStorage.Revisions
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