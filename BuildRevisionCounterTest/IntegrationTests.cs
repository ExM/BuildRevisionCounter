using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using NUnit.Framework;

namespace BuildRevisionCounterTest
{
    [TestFixture]
    public class IntegrationTest : InMemoryTest
    {
        [Test]
        public void Bump_New_Revision()
        {
            var revName = "revision_" + DateTime.Now.Ticks;
            var apiUri = "api/counter/" + revName;

            var body = SendPostRequest(apiUri);
            Assert.AreEqual(0, int.Parse(body));
        }

        [Test]
        public void Bump_Existing_Revision()
        {
            var revName = "revision_" + DateTime.Now.Ticks;
            var apiUri = "api/counter/" + revName;

            SendPostRequest(apiUri); //counter == 0

            var body = SendPostRequest(apiUri); //counter == 1
            Assert.AreEqual(1, int.Parse(body));
        }

        [Test]
        public void Get_Current_Revision()
        {
            var revName = "revision_" + DateTime.Now.Ticks;
            var apiUri = "api/counter/" + revName;

            SendPostRequest(apiUri); //counter == 0
            SendPostRequest(apiUri); // counter == 1

            var body = SendGetRequest(apiUri);
            Assert.AreEqual(1, int.Parse(body));
        }

        [Test]
        public void Get_Current_Revision_NotFound()
        {
            var body = SendGetRequest("api/counter/nonexistingrevision");
            Assert.AreEqual("", body);
        }

        [Test]
        public void Integration_Test()
        {
            Bump_New_Revision();
            Bump_Existing_Revision();

            Get_Current_Revision();
            Get_Current_Revision_NotFound();
        }

        private string SendGetRequest(string apiUri, string userName = "admin", string password = "admin")
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

        private string SendPostRequest(string apiUri, string userName = "admin", string password = "admin")
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