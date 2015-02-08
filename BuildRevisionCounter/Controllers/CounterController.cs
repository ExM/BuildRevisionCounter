using System.Threading.Tasks;
using System.Web.Http;
namespace BuildRevisionCounter.Controllers
{
	[RoutePrefix("api/counter")]
	public class CounterController : ApiController
	{
		[HttpGet]
		[Route("{revisionName}")]
		public async Task<int> Current([FromUri] string revisionName)
		{
			//TODO: возвращает текущее значение счетчика "revisionName", если счетчик отсутствует, то возвращать код 404
			return await Task.FromResult<int>(123);
		}
	}
}