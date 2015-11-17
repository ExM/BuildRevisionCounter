using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
	public abstract class IntegrationTest
	{
		private IDisposable _application;

		private string _uri;
		protected string Uri { get { return _uri; } }

		[TestFixtureSetUp]
		public void Setup()
		{
			var port = GetFreeTcpPort();
			_uri = string.Format("http://localhost:{0}", port);
			_application = WebApp.Start<Web.Startup>(_uri);
		}


		[TestFixtureTearDown]
		public void TearDown()
		{
			_application.Dispose();
		}

		private static int GetFreeTcpPort()
		{
			var l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();
			var port = ((IPEndPoint)l.LocalEndpoint).Port;
			l.Stop();
			return port;
		}

		protected async Task<HttpResponseMessage> SendGetRequest(string apiUri, string userName = "admin", string password = "admin")
		{
			using (var httpClient = new HttpClient { BaseAddress = new Uri(Uri) })
			{
				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
						Convert.ToBase64String(
							Encoding.ASCII.GetBytes(
								string.Format("{0}:{1}", userName, password))));

				return await httpClient.GetAsync(apiUri);
			}
		}

		protected async Task<HttpResponseMessage> SendPostRequest(string apiUri, string userName = "admin", string password = "admin")
		{
			using (var httpClient = new HttpClient { BaseAddress = new Uri(Uri) })
			{
				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
						Convert.ToBase64String(
							Encoding.ASCII.GetBytes(
								string.Format("{0}:{1}", userName, password))));

				var content = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("", "")
				});

				return await httpClient.PostAsync(apiUri, content);
			}
		}
	}

}
