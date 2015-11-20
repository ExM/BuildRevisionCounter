using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace BuildRevisionCounter.Web.Filters
{
	/// <summary>
	/// Фильтр для установки http кода ответа
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	internal class RewriteResponseCodeFilterAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// Occurs after the action method is invoked.
		/// </summary>
		/// <param name="actionExecutedContext">The action executed context.</param>
		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			if (actionExecutedContext.Response == null) 
				return;

			if (actionExecutedContext.Response.Content != null
				&& actionExecutedContext.Response.StatusCode == HttpStatusCode.OK
				&& actionExecutedContext.Request.Method == HttpMethod.Post)
			{
				actionExecutedContext.Response.StatusCode = HttpStatusCode.Created;
			}
		}
	}
}