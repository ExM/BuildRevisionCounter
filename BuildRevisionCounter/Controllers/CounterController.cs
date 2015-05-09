using BuildRevisionCounter.Model;
using MongoDB.Driver;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Security;

namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication()]
	public class CounterController : ApiController
	{
		private static MongoDBStorage _storage;

		static CounterController()
		{
			_storage = MongoDBStorage.Instance;
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<long> Current([FromUri] string revisionName)
		{
			var q = Builders<RevisionModel>.Filter.Eq(_ => _.Id, revisionName);
			var revision = await _storage.Revisions.Find(q).FirstOrDefaultAsync();

			if (revision == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revision.NextNumber;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public async Task<long> Bumping([FromUri] string revisionName)
		{
			var options = new FindOneAndUpdateOptions<RevisionModel>
			{
				IsUpsert = true,
				ReturnDocument = ReturnDocument.After
			};

			var revision = await _storage.Revisions.FindOneAndUpdateAsync(
				Builders<RevisionModel>.Filter.Eq(r => r.Id, revisionName),
				Builders<RevisionModel>.Update
					.SetOnInsert(l => l.Created, DateTime.UtcNow)
					.Inc(l => l.NextNumber, 1)
					.Set(l => l.Updated, DateTime.UtcNow),
				options);

			return revision.NextNumber;
		}
	}
}