using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Model;
using BuildRevisionCounter.Security;
using MongoDB.Driver;

namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication]
	public class CounterController : ApiController
	{
		private readonly MongoDBStorage _storage;

		public CounterController() : this(MongoDBStorageFactory.DefaultInstance)
		{
		}

		public CounterController(MongoDBStorage mongoDbStorage)
		{
			_storage = mongoDbStorage;
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<long> Current([FromUri] string revisionName)
		{
			var revision = await _storage.Revisions
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
			var result = await _storage.Revisions
				.FindOneAndUpdateAsync<RevisionModel>(
					r => r.Id == revisionName,
					Builders<RevisionModel>.Update
						.Inc(r => r.NextNumber, 1)
						.SetOnInsert(r => r.Created, DateTime.UtcNow)
						.Set(r => r.Updated, DateTime.UtcNow),
					new FindOneAndUpdateOptions<RevisionModel>
					{
						IsUpsert = true,
						ReturnDocument = ReturnDocument.After
					});

            if (result.NextNumber == 1) return 0;
			return result.NextNumber;
		}
	}
}