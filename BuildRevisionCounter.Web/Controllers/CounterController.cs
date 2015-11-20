using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.DTO;
using BuildRevisionCounter.Web.Security;

namespace BuildRevisionCounter.Web.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication]
	public class CounterController : ApiController
	{
		private readonly IRevisionRepository _revisionRepo;

		/// <summary>
		/// Конструктор контроллера номеров ревизий.
		/// </summary>
		/// <param name="revisionRepo">Репозиторий ревизий.</param>
		public CounterController(IRevisionRepository revisionRepo)
		{
			if (revisionRepo == null)
				throw new ArgumentNullException("revisionRepo");

			_revisionRepo = revisionRepo;
		}

		[HttpGet]
		[Route("")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public Task<IReadOnlyCollection<Revision>> GetAllRevision([FromUri] int pageSize = 20, [FromUri] int pageNumber = 1)
		{
			if (pageSize < 1 || pageNumber < 1)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			return _revisionRepo.GetAllRevision(pageSize, pageNumber);
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public Task<long> Current([FromUri] string revisionName)
		{
			return _revisionRepo.Current(revisionName);
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public Task<long> Bumping([FromUri] string revisionName)
		{
			return _revisionRepo.Bumping(revisionName);
		}
	}
}