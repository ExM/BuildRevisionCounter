using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Model;
using BuildRevisionCounter.Security;
using BuildRevisionCounter.Data;

namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	[BasicAuthentication]
	public class CounterController : ApiController
	{
		private readonly IRevisionStorage _dataStorage;

		/// <summary>
		/// Конструктор контроллера номеров ревизий.
		/// </summary>
		/// <param name="dataStorage">Объект для получения данных из БД.</param>
		public CounterController(IRevisionStorage dataStorage)
		{
			if (dataStorage == null)
				throw new ArgumentNullException("dataStorage");

			_dataStorage = dataStorage;
		}

		[HttpGet]
		[Route("")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<IReadOnlyCollection<RevisionModel>> GetAllRevision([FromUri] Int32 pageSize = 20, [FromUri] Int32 pageNumber = 1)
		{
			if (pageSize < 1 || pageNumber < 1)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			var revisions = await _dataStorage.GetAllRevision(pageSize, pageNumber);

			if (revisions == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revisions;
		}

		[HttpGet]
		[Route("{revisionName}")]
		[Authorize(Roles = "admin, editor, anonymous")]
		public async Task<long> Current([FromUri] string revisionName)
		{
			var revisionNumber = await _dataStorage.CurrentRevision(revisionName);

			if (revisionNumber == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			return revisionNumber.Value;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public async Task<long> Bumping([FromUri] string revisionName)
		{
			return await _dataStorage.Bumping(revisionName);
		}
	}
}