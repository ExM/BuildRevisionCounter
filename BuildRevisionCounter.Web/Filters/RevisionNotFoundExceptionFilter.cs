using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace BuildRevisionCounter.Web.Filters
{
	/// <summary>
	/// Фильтра для обработки исключения о том что ревизия не была найдена.
	/// </summary>
	public class RevisionNotFoundExceptionFilter: ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext actionExecutedContext)
		{
			if (actionExecutedContext.Exception is RevisionNotFoundException)
			{
				actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
			}
		}
	}
}