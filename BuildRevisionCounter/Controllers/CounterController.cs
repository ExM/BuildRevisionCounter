using System.Configuration;
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
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public long Current([FromUri] string revisionName)
		{
			var repository = RepositoryFactory.Instance.GetRevisionRepository();
			var revision = repository.GetRevisionById(revisionName);
				.SingleOrDefaultAsync();

			if (revision == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revision.NextNumber;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public long Bumping([FromUri] string revisionName)
		{
			var repository = RepositoryFactory.Instance.GetRevisionRepository();
			var revision = repository.IncrementRevision(revisionName);
					r => r.Id == revisionName,
					Builders<RevisionModel>.Update
						.Inc(r => r.NextNumber, 1)
						.SetOnInsert(r => r.Created, DateTime.UtcNow)
						.Set(r => r.Updated, DateTime.UtcNow),
					new FindOneAndUpdateOptions<RevisionModel>

			if (revision == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revision.NextNumber;
		}
	}
}