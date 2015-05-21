using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Api.Security;
using BuildRevisionCounter.DAL.Repositories.Interfaces;
using BuildRevisionCounter.Model.BuildRevisionStorage;
using MongoDB.Driver;

namespace BuildRevisionCounter.Api.Controllers
{
    [RoutePrefix("api/counter")]
    [BasicAuthentication]
    public class CounterController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private IRevisionRepository _revisionRepository;

        /// <summary>
        /// Конструктор контроллера номеров ревизий.
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="revisionRepository"></param>
        public CounterController(IUserRepository userRepository, IRevisionRepository revisionRepository)
        {
            _userRepository = userRepository;
            _revisionRepository = revisionRepository;
        }

        [HttpGet]
        [Route("{revisionName}")]
        [Authorize(Roles = "admin, editor, anonymous")]
        public async Task<long> Current([FromUri] string revisionName)
        {
            RevisionModel revision = await _revisionRepository.Get(r => r.Id == revisionName);
            if (revision == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return revision.NextNumber;
        }

        [HttpPost]
        [Route("{revisionName}")]
        [Authorize(Roles = "buildserver")]
        public async Task<long> Bumping([FromUri] string revisionName)
        {
            RevisionModel revision = await _revisionRepository.FindOneAndUpdate(
                r => r.Id == revisionName, GetUpdateBuilder(), GetUpdateOptions());
            if (revision != null)
            {
                return revision.NextNumber;
            }
            return long.MinValue;
        }

        private FindOneAndUpdateOptions<RevisionModel> GetUpdateOptions()
        {
            return new FindOneAndUpdateOptions<RevisionModel>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
        }

        private UpdateDefinition<RevisionModel> GetUpdateBuilder()
        {
            return Builders<RevisionModel>.Update
                .Inc(r => r.NextNumber, 1)
                .SetOnInsert(r => r.Created, DateTime.UtcNow)
                .Set(r => r.Updated, DateTime.UtcNow);
        }
    }
}