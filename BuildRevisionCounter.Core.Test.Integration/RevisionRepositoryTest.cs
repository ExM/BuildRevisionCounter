using System.Threading.Tasks;
using BuildRevisionCounter.Core.DomainObjects;
using BuildRevisionCounter.Core.Repositories.Impl;
using MongoDB.Driver;
using NUnit.Framework;

namespace BuildRevisionCounter.Core.Test.Integration
{
    [TestFixture]
    public class RevisionRepositoryTest
    {
        private RevisionTestHelper _dbHelper;
        private RevisionRepository _repository;
        
        [TestFixtureSetUp]
        public void ClassSetUp()
        {
            _dbHelper = new RevisionTestHelper();
            _repository = new RevisionRepository();
        }
        
        [SetUp]
        public void TestSetUp()
        {
            _dbHelper.ClearRevisionCollectionAsync().Wait();
        }
        
        [Test]
        public async void GetRevisionByIdAsync_WhenIdNotExists_ReturnNull()
        {
            // Act
            var revision = await _repository.GetRevisionByIdAsync("unknownRev");

            // Assert
            Assert.IsNull(revision);
        }
        
        [Test]
        public async void GetRevisionByIdAsync_WhenIdExists_ReturnRevision()
        {
            // Arrange
            const string expectedRevId = "testRev";
            var expectedRev = new Revision { Id = expectedRevId };
            await _dbHelper.AddRevisionAsync(expectedRev);

            // Act
            var actualRev = await _repository.GetRevisionByIdAsync(expectedRevId);
            var actualRevId = actualRev.Id;

            // Asert
            Assert.AreEqual(expectedRevId, actualRevId);
        }

        [Test]
        public async void IncrementRevisionAsync_WhenNotExist_CreateWithNumberOne()
        {
            // Arrange
            const int expectedNextNumber = 1;
            const string revId = "testRev";
            
            // Act
            var newRevision = await _repository.IncrementRevisionAsync(revId);
            var actualNextNumber = newRevision.NextNumber;

            // Assert
            Assert.AreEqual(expectedNextNumber, actualNextNumber);
        }

        [Test]
        public async void IncrementRevisionAsync_WhenExist_IncrementNumber()
        {
            // Arrange
            const string revId = "testRev";
            const int expectedNextNumber = 4;
            var expectedRev = new Revision { Id = revId, NextNumber = expectedNextNumber - 1 };
            await _dbHelper.AddRevisionAsync(expectedRev);

            // Act
            var actualRev = await _repository.IncrementRevisionAsync(revId);
            var actualNextNumber = actualRev.NextNumber;

            // Assert
            Assert.AreEqual(expectedNextNumber, actualNextNumber);
        }
    }

    internal sealed class RevisionTestHelper
    {
        private readonly MongoContext _storage;

        internal RevisionTestHelper()
        {
            _storage = MongoContext.Instance;
        }

        internal async Task AddRevisionAsync(Revision revision)
        {
            await _storage.Revisions.InsertOneAsync(revision);
        }

        internal async Task ClearRevisionCollectionAsync()
        {
            await _storage.Revisions.DeleteManyAsync(l => true);
        }
    }
}
