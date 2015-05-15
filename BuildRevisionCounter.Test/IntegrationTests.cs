using System;
using NUnit.Framework;

namespace BuildRevisionCounterTest
{
    [TestFixture]
    public class IntegrationTest : InMemoryTest
    {
        [Test]
        public void Bump_New_Revision()
        {
            var revName = "revision_" + Guid.NewGuid();
            var apiUri = "api/counter/" + revName;

            var body = SendPostRequest(apiUri);
            Assert.AreEqual(1, int.Parse(body));
        }

        [Test]
        public void Bump_Existing_Revision()
        {
            var revName = "revision_" + DateTime.Now.Ticks;
            var apiUri = "api/counter/" + revName;

            SendPostRequest(apiUri); //counter == 1

            var body = SendPostRequest(apiUri); //counter == 2
            Assert.AreEqual(2, int.Parse(body));
        }

        [Test]
        public void Get_Current_Revision()
        {
            var revName = "revision_" + Guid.NewGuid();
            var apiUri = "api/counter/" + revName;

            SendPostRequest(apiUri); //counter == 1
            SendPostRequest(apiUri); // counter == 2

            var body = SendGetRequest(apiUri);
            Assert.AreEqual(2, int.Parse(body));
        }

        [Test]
        public void Get_Current_Revision_NotFound()
        {
            var body = SendGetRequest("api/counter/nonexistingrevision");
            Assert.AreEqual("", body);
        }     
    }
}