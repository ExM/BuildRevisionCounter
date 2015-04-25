using System.Web.Http;
using BuildRevisionCounter.Model;
using BuildRevisionCounter.Controllers;
using MongoDB.Driver;
using NUnit.Framework;

namespace BuildRevisionCounter
{
    [TestFixture]
    public class CounterTests
    {
        [Test]
        public static async void CurrentTest()
        {
            //counter1 in db is for this test, don't bump it
            int expected = 1;
            CounterController testedController = new CounterController();
            long actual = await testedController.Current("counter1");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(HttpResponseException))]
        public static async void BumpingReturn0IfCreatedTest()
        {
            //there must be no documents with _id == newCounterName 
            string newCounterName = "new_counter";

            int expected = 0;
            CounterController testedController = new CounterController();
            long actual = await testedController.Bumping(newCounterName);
            Assert.AreEqual(expected, actual);

            //expect current == 1
            expected = 1;
            actual = await testedController.Current(newCounterName);
            Assert.AreEqual(expected, actual);

            //delete new_counter
            MongoClient client = new MongoClient("mongodb://localhost");
            IMongoDatabase database = client.GetDatabase("brCounter");
            IMongoCollection<RevisionModel> revisions = database.GetCollection<RevisionModel>("revisions");
            await revisions.DeleteOneAsync<RevisionModel>(_ => _.Id == newCounterName);

            //expect exception
            await testedController.Current(newCounterName);
        }

        [Test]
        public static async void BumpingIncrementNextNumberCorrectly()
        {
            //there must be no documents with _id == newCounterName 
            string newCounterName = "new_counter";

            CounterController testedController = new CounterController();

            //increment 10 times
            for (int i = 1; i <= 10; i++)
            {
                await testedController.Bumping(newCounterName);
                long actual = await testedController.Current(newCounterName);
                Assert.AreEqual(i, actual);
            }

            //delete new_counter
            MongoClient client = new MongoClient("mongodb://localhost/brCounter");
            IMongoDatabase database = client.GetDatabase("brCounter");
            IMongoCollection<RevisionModel> revisions = database.GetCollection<RevisionModel>("revisions");
            await revisions.DeleteOneAsync<RevisionModel>(_ => _.Id == newCounterName);
        }
    }
}