using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using BuildRevisionCounter.Exceptions;

namespace BuildRevisionCounter.Web.Filters
{
	/// <summary>
	/// Фильтр для обработки исключений
	/// </summary>
	public class KnownExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private const string IncorrectRoleListErrorMessage = "Если аккаунт имеет роль buildserver, то ему нельзя назначить другие роли";
		private const string DuplicateKeyErrorMessage = "Пользователь с указанным именем уже существует";
		private const string GenericErrorMessage = "При обработке запроса произошла ошибка";
		private const string RevisionNotFoundErrorMessage = "Ревизия не найдена";

		public override void OnException(HttpActionExecutedContext actionExecutedContext)
		{
			if (actionExecutedContext.Exception is IncorrectRoleListException)
			{
				actionExecutedContext.Response = 
					actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, IncorrectRoleListErrorMessage);
			}
			else if (actionExecutedContext.Exception is DuplicateKeyException)
			{
				actionExecutedContext.Response =
					actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, DuplicateKeyErrorMessage);
			}
			else if (actionExecutedContext.Exception is RevisionNotFoundException)
			{
				actionExecutedContext.Response =
					actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, RevisionNotFoundErrorMessage);
			}
			else
			{
				actionExecutedContext.Response =
					actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GenericErrorMessage);
			}
		}
	}
}