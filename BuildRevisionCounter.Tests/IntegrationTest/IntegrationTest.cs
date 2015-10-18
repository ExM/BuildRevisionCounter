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

		protected HttpClient HttpClient;

		private string _uri;
		protected string Uri { get { return _uri; } }

		[TestFixtureSetUp]
		public void Setup()
		{
			var port = GetFreeTcpPort();
			_uri = string.Format("http://localhost:{0}", port);
			_application = WebApp.Start<Startup>(_uri);

            DBStorageFactory.DefaultInstance.SetUpAsync().Wait();
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

		protected async Task<string> SendGetRequest(string apiUri, string userName = "admin", string password = "admin")
		{
			using (HttpClient = new HttpClient { BaseAddress = new Uri(Uri) })
			{
				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
					HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
						Convert.ToBase64String(
							Encoding.ASCII.GetBytes(
								string.Format("{0}:{1}", userName, password))));

				var responseMessage = await HttpClient.GetAsync(apiUri);
				return await responseMessage.Content.ReadAsStringAsync();
			}
		}

		protected async Task<string> SendPostRequest(string apiUri, string userName = "admin", string password = "admin")
		{
			using (HttpClient = new HttpClient { BaseAddress = new Uri(Uri) })
			{
				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
					HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
						Convert.ToBase64String(
							Encoding.ASCII.GetBytes(
								string.Format("{0}:{1}", userName, password))));

				var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("", "")
                });

				var responseMessage = await HttpClient.PostAsync(apiUri, content);
				return await responseMessage.Content.ReadAsStringAsync();
			}
		}
	}

}
