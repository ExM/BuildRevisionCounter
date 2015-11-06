using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Web.Security;
using BuildRevisionCounter.DTO;
using BuildRevisionCounter.Web.Filters;

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
		public  Task<IReadOnlyCollection<Revision>> GetAllRevision([FromUri] Int32 pageSize = 20, [FromUri] Int32 pageNumber = 1)
		{
			if (pageSize < 1 || pageNumber < 1)
				throw new HttpResponseException(HttpStatusCode.BadRequest);			

			return _revisionRepo.GetAllRevision(pageSize, pageNumber);
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		[RevisionNotFoundExceptionFilter]
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