using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using BuildRevisionCounter;

namespace BuildRevisionCounterTest
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
    }

}
