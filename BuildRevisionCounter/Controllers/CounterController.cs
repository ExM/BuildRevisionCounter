using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Core;
using BuildRevisionCounter.Protocol;
using BuildRevisionCounter.Security;

namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication]
	public class CounterController : ApiController
	{
		[HttpGet]
		[Route("")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<IReadOnlyCollection<Revision>> GetAllRevision([FromUri] Int32 pageSize = 20, [FromUri] Int32 pageNumber = 1)
		{
			if (pageSize < 1 || pageNumber < 1)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			var repository = RepositoryFactory.Instance.GetRevisionRepository();
			var revisions = await repository.GetAllRevisionAsync(pageSize, pageNumber);

			if (revisions == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revisions;
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<long> Current([FromUri] string revisionName)
		{
			var repository = RepositoryFactory.Instance.GetRevisionRepository();
			var revision = await repository.GetRevisionByIdAsync(revisionName);

			if (revision == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revision.CurrentNumber;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public async Task<long> Bumping([FromUri] string revisionName)
		{
			var repository = RepositoryFactory.Instance.GetRevisionRepository();
			var revision = await repository.IncrementRevisionAsync(revisionName);

			if (revision == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revision.CurrentNumber;
		}
	}
}