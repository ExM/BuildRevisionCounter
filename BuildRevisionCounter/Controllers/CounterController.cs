using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
		private readonly IMongoDBStorage _mongoDbStorage;

		/// <summary>
		/// Конструктор контроллера номеров ревизий.
		/// </summary>
		/// <param name="mongoDbStorage">Объект для получения данных из БД Монго.</param>
		public CounterController(IMongoDBStorage mongoDbStorage)
		{
			if (mongoDbStorage == null)
				throw new ArgumentNullException("mongoDbStorage");

			_mongoDbStorage = mongoDbStorage;
		}

		[HttpGet]
		[Route("")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<IReadOnlyCollection<RevisionModel>> GetAllRevision([FromUri] Int32 pageSize=20, [FromUri] Int32 pageNumber=1)
		{
			if (pageSize < 1 || pageNumber < 1)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			CancellationTokenSource cts = new CancellationTokenSource();
			var task = _mongoDbStorage.Revisions
				.Find(r => true)
				.Skip(pageSize * (pageNumber - 1))
				.Limit(pageSize)
				.ToListAsync(cts.Token);

			// запустим паралельно 2 таска, второй таск в роли таймаута (30 сек)
			const int timeout = 30*1000;
			if (await Task.WhenAny(task, Task.Delay(timeout, cts.Token)) != task)
			{
				// посылаем команду прервать поток поиска
				cts.Cancel();
				// генерируем сообщение о том что не дождались ответа от монги
				throw new HttpResponseException(HttpStatusCode.GatewayTimeout);
			}
			// прерываем поток таймаута
			cts.Cancel();

			if (task.Result == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return task.Result;
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

			return revision.NextNumber;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public async Task<long> Bumping([FromUri] string revisionName)
		{
			// попробуем обновить документ
			var result = await FindOneAndUpdateRevisionModelAsync(revisionName);

			if (result == null)
			{
				// если не получилось, значит документ еще не был создан
				// создадим его с начальным значением 0
				try
				{
					await _mongoDbStorage.Revisions
						.InsertOneAsync(new RevisionModel
						{
							Id = revisionName,
							NextNumber = 0,
							Created = DateTime.UtcNow
						});
					return 0;
				}
				catch (MongoWriteException)
				{
					// если прои вставке произошла ошибка значит мы не успели и запись там уже есть
					// и теперь попытка обновления должна пройти без ошибок
					result = FindOneAndUpdateRevisionModelAsync(revisionName).Result;
				}
			}

			return result.NextNumber;
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
						.Inc(r => r.NextNumber, 1)
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