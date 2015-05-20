using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestContext = NUnit.Framework.TestContext;

namespace BuildRevisionCounter.Tests.IntegrationTests
{
    [TestClass]
    public class WebApiTests
    {
        private static IDisposable _webApp;

        [AssemblyInitialize]
        public static void SetUp(TestContext context)
        {
            //_webApp = TestServer.Start<Startup>();
        }

        [AssemblyCleanup]
        public static void TearDown()
        {
            _webApp.Dispose();
        }

        [TestMethod]
        public async Task TestMethod()
        {
            //using (var httpClient = new HttpClient())
            //{
            //    var accessToken = GetAccessToken();
            //    httpClient.DefaultRequestHeaders.Authorization =
            //        new AuthenticationHeaderValue("Bearer", accessToken);
            //    var requestUri = new Uri("http://localhost:9443/api/values");
            //    await httpClient.GetStringAsync(requestUri);
            //}
        }
    }
}
