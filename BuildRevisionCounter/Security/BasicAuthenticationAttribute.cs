using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using BuildRevisionCounter.Model;
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

		private static readonly MongoDBStorage _storage;

		static BasicAuthenticationAttribute()
		{
			_storage = new MongoDBStorage();
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

			if (String.IsNullOrEmpty(authorization.Parameter))
			{
				// заголовок есть, но логина и пароля нет
				context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
				return Task.FromResult(0);
			}

			var userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);

			if (userNameAndPasword == null)
			{
				context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
				return Task.FromResult(0);
			}

			var principal = Authenticate(userNameAndPasword.Item1, userNameAndPasword.Item2);

			if (principal == null)
			{
				context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
			}
			else
			{
				context.Principal = principal;
			}

			return Task.FromResult(0);
		}

		private IPrincipal Authenticate(string userName, string password)
		{
			IPrincipal principal = null;
			var user = _storage.Users.FindOne(Query<UserModel>.Where(u => u.Name == userName));
			if (user != null && user.Password == password)
			{
				principal = new GenericPrincipal(new GenericIdentity(userName), user.Roles);
			}
			return principal;
		}

		private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
		{
			byte[] credentialBytes;

			try
			{
				credentialBytes = Convert.FromBase64String(authorizationParameter);
			}
			catch (FormatException)
			{
				return null;
			}

			string decodedCredentials;

			try
			{
				decodedCredentials = _asciiEncodingWithExceptionFallback.GetString(credentialBytes);
			}
			catch (DecoderFallbackException)
			{
				return null;
			}

			if (String.IsNullOrEmpty(decodedCredentials))
			{
				return null;
			}

			var colonIndex = decodedCredentials.IndexOf(':');

			if (colonIndex == -1)
			{
				return null;
			}

			var userName = decodedCredentials.Substring(0, colonIndex);
			var password = decodedCredentials.Substring(colonIndex + 1);
			return new Tuple<string, string>(userName, password);
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			Challenge(context);
			return Task.FromResult(0);
		}

		private void Challenge(HttpAuthenticationChallengeContext context)
		{
			string parameter;

			if (String.IsNullOrEmpty(Realm))
			{
				parameter = null;
			}
			else
			{
				// TODO: экранировать ковычки
				parameter = "realm=\"" + Realm + "\"";
			}

			context.Result = new AddChallengeOnUnauthorizedResult(new AuthenticationHeaderValue(AuthorizationScheme, parameter), context.Result);
		}
	}
}