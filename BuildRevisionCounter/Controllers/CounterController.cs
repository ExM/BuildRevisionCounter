﻿using System.Configuration;
using System.Threading.Tasks;
using BuildRevisionCounter.Core.Repositories.Impl;
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
		public async Task<long> Current([FromUri] string revisionName)
		{
            var repository = new RevisionRepository();
            var revision = await repository.GetRevisionByIdAsync(revisionName);

            if (revision == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return revision.NextNumber;
		}

		[HttpPost]
		[Route("{revisionName}")]
		[Authorize(Roles = "buildserver")]
		public async Task<long> Bumping([FromUri] string revisionName)
		{
            var repository = new RevisionRepository();
            var revision = await repository.IncrementRevisionAsync(revisionName);

            if (revision == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return revision.NextNumber;
		}
	}
}