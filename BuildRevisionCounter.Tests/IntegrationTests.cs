using System;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
    [TestFixture]
    public class IntegrationTest : InMemoryTest
    {
        private const int _counterStartValue = 0; //Первоначальное значение счетчика равно 0

        [Test]
        public void Bump_New_Revision()
        {
            var revName = "revision_" + Guid.NewGuid();
            var apiUri = "api/counter/" + revName;

            var body = SendPostRequest(apiUri);

            Assert.AreEqual(_counterStartValue, int.Parse(body));
        }

        [Test]
        public void Bump_Existing_Revision()
        {
            var revName = "revision_" + Guid.NewGuid();
            var apiUri = "api/counter/" + revName;

            SendPostRequest(apiUri); //counter == 0

            var body = SendPostRequest(apiUri); //counter == 1
            
            var currentCounter = 1;
            Assert.AreEqual(currentCounter, int.Parse(body));
        }

        [Test]
        public void Get_Current_Revision()
        {
            var revName = "revision_" + Guid.NewGuid();
            var apiUri = "api/counter/" + revName;

            SendPostRequest(apiUri); //counter == 0
            SendPostRequest(apiUri); // counter == 1

            var body = SendGetRequest(apiUri);

            var currentCounter = 1;
            Assert.AreEqual(currentCounter, int.Parse(body));
        }

        [Test]
        public void Get_Current_Revision_NotFound()
        {
            var body = SendGetRequest("api/counter/nonexistingrevision");
            Assert.AreEqual("", body);
        }     
    }
}