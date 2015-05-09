using BuildRevisionCounter.Core;
using System.Net;
using System.Web.Http;
using BuildRevisionCounter.Security;

namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication()]
	public class CounterController : ApiController
	{

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public long Current([FromUri] string revisionName)
		{
			var repository = RepositoryFactory.Instance.GetRevisionRepository();
			var revision = repository.GetRevisionById(revisionName);

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

			if (revision == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revision.NextNumber;
		}
	}
}