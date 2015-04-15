using System;
using System.Net;
using System.Web.Http;
using BuildRevisionCounter.Model;
using BuildRevisionCounter.Security;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BuildRevisionCounter.Controllers
{
    [RoutePrefix("api/counter")]
    [BasicAuthentication]
    public class CounterController : ApiController
    {
        private readonly MongoDBStorage _storage;

        public CounterController(MongoDBStorage mongoDbStorage = null)
        {
            _storage = mongoDbStorage ?? new MongoDBStorage();
        }

        [HttpGet]
        [Route("{revisionName}")]
        [Authorize(Roles = "admin, editor, anonymous")]
        public long Current([FromUri] string revisionName)
        {
            var q = Query<RevisionModel>.Where(_ => _.Id == revisionName);
            var revision = _storage.Revisions.FindOne(q);

            if (revision == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return revision.NextNumber;
        }

        [HttpPost]
        [Route("{revisionName}")]
        [Authorize(Roles = "buildserver")]
        public long Bumping([FromUri] string revisionName)
        {
            var result = _storage.Revisions.FindAndModify(
                new FindAndModifyArgs
                {
                    Query = Query<RevisionModel>.Where(_ => _.Id == revisionName),
                    Upsert = true,
                    Update = Update<RevisionModel>
                        .SetOnInsert(_ => _.Created, DateTime.UtcNow)
                        .Inc(_ => _.NextNumber, 1)
                        .Set(_ => _.Updated, DateTime.UtcNow),
                    VersionReturned = FindAndModifyDocumentVersion.Modified
                });

            var revision = result.GetModifiedDocumentAs<RevisionModel>();

            return revision.NextNumber;
        }
    }
}