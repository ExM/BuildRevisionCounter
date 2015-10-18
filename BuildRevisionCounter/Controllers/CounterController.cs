using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Model;
using BuildRevisionCounter.Security;
using BuildRevisionCounter.Data;

namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication]
	public class CounterController : ApiController
	{
		private readonly DbStorage _dbStorage;

		/// <summary>
		/// Конструктор контроллера номеров ревизий.
		/// </summary>
        /// <param name="dbStorage">Объект для получения данных из БД.</param>
		public CounterController(DbStorage dbStorage)
		{
            if (dbStorage == null)
                throw new ArgumentNullException("dbStorage");

            _dbStorage = dbStorage;
		}

		[HttpGet]
		[Route("")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<IReadOnlyCollection<RevisionModel>> GetAllRevision([FromUri] Int32 pageSize = 20, [FromUri] Int32 pageNumber = 1)
		{
			if (pageSize < 1 || pageNumber < 1)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

            var revisions = await _dbStorage.GetAllRevision(pageSize, pageNumber);

			if (revisions == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revisions;
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<long> Current([FromUri] string revisionName)
		{
            var revisionNumber = await _dbStorage.CurrentRevision(revisionName);

            if (revisionNumber == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

            return revisionNumber.Value;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public async Task<long> Bumping([FromUri] string revisionName)
		{
			return await _dbStorage.Bumping(revisionName);
		}
	}
}