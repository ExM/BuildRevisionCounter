using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
    public abstract class InMemoryTest
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
            var port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        protected string SendGetRequest(string apiUri, string userName = "admin", string password = "admin")
        {
            using (HttpClient = new HttpClient { BaseAddress = new Uri(Uri) })
            {

                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", userName, password))));

                var response = HttpClient.GetAsync(apiUri).Result;
                var body = response.Content.ReadAsStringAsync().Result;

                return body;
            }
        }

        protected string SendPostRequest(string apiUri, string userName = "admin", string password = "admin")
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

                var response = HttpClient.PostAsync(apiUri, content).Result;
                var body = response.Content.ReadAsStringAsync().Result;

                return body;
            }
        }
    }

}
