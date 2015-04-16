using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using BuildRevisionCounter.Core;
using BuildRevisionCounter.Core.Repositories.Impl;
using MongoDB.Driver.Builders;

namespace BuildRevisionCounter.Security
{
	public class BasicAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		private const string AuthorizationScheme = "Basic";

		private static readonly IPrincipal _anonymousPrincipal = new GenericPrincipal(new GenericIdentity("anonymous"), new[] { "anonymous" });

		// The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
		// However, the current draft updated specification for HTTP 1.1 indicates this encoding is infrequently
		// used in practice and defines behavior only for ASCII.
		private static readonly Encoding _asciiEncodingWithExceptionFallback =
			Encoding.GetEncoding(Encoding.ASCII.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
        
		static BasicAuthenticationAttribute()
		{
		}

		public bool AllowMultiple { get { return false; } }

		public string Realm { get; set; }

		public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			HttpRequestMessage request = context.Request;
			AuthenticationHeaderValue authorization = request.Headers.Authorization;

			if (authorization == null || authorization.Scheme != AuthorizationScheme)
			{
				context.Principal = _anonymousPrincipal;
				return Task.FromResult(0);
			}

			string user;
			string password;

			if (!ExtractUserNameAndPassword(authorization.Parameter, out user, out password))
			{
				context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
				return Task.FromResult(0);
			}
            
			context.Principal = Authenticate(user, password).Result;

			if (context.Principal == null)
				context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);

			return Task.FromResult(0);
		}

		private async Task<IPrincipal> Authenticate(string userName, string password)
		{
            var repository = new UserRepository();

			IPrincipal principal = null;
			var user = await repository.GetUserByNameAsync(userName);
			if (user != null && user.Password == password)
			{
				principal = new GenericPrincipal(new GenericIdentity(userName), user.Roles);
			}
			return principal;
		}

		private static bool ExtractUserNameAndPassword(string authorizationParameter, out string user, out string password)
		{
			user = null;
			password = null;

			if (string.IsNullOrWhiteSpace(authorizationParameter))
				return false;

			byte[] credentialBytes;

			try
			{
				credentialBytes = Convert.FromBase64String(authorizationParameter);
			}
			catch (FormatException)
			{
				return false;
			}

			string decodedCredentials;

			try
			{
				decodedCredentials = _asciiEncodingWithExceptionFallback.GetString(credentialBytes);
			}
			catch (DecoderFallbackException)
			{
				return false;
			}

			if (String.IsNullOrEmpty(decodedCredentials))
				return false;

			var colonIndex = decodedCredentials.IndexOf(':');

			if (colonIndex == -1)
				return false;

			user = decodedCredentials.Substring(0, colonIndex);
			password = decodedCredentials.Substring(colonIndex + 1);
			return true;
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			var authHeader = new AuthenticationHeaderValue(AuthorizationScheme, "realm=\"" + (Realm ?? "default") + "\""); // TODO: экранировать кавычки
			context.Result = new AddChallengeOnUnauthorizedResult(authHeader, context.Result);
			return Task.FromResult(0);
		}
	}
}