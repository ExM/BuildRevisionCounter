using System.Web;
using System.Web.Http;
using BuildRevisionCounter.Api.Helpers;
using BuildRevisionCounter.Api.TransportObject;

namespace BuildRevisionCounter.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        protected HttpRequest CurrentRequest { get; set; }

        protected Response Response { get; set; }

        public BaseApiController()
        {
            Response = ResponseHelper.GetResponse();
            CurrentRequest = HttpContext.Current.Request;
        }
    }
}