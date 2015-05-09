using BuildRevisionCounter.Core.DomainObjects;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using BuildRevisionCounter.Core.Repositories;

namespace BuildRevisionCounter.Core.Test.Integration
{
	[TestFixture]
	public class RevisionRepositoryTest
	{
		private RevisionTestHelper _dbHelper;
		private IRevisionRepository _repository;

		[TestFixtureSetUp]
		public void ClassSetUp()
		{
			_dbHelper = new RevisionTestHelper();
			_repository = RepositoryFactory.Instance.GetRevisionRepository();
		}

		[SetUp]
		public void TestSetUp()
		{
			_dbHelper.ClearRevisionCollection();
		}

		[Test]
		public void GetRevisionByIdAsync_WhenIdNotExists_ReturnNull()
		{
			// Act
			var revision = _repository.GetRevisionById("unknownRev");

			// Assert
			Assert.IsNull(revision);
		}

		[Test]
		public void GetRevisionByIdAsync_WhenIdExists_ReturnRevision()
		{
			// Arrange
			const string expectedRevId = "testRev";
			var expectedRev = new Revision {Id = expectedRevId};
			_dbHelper.AddRevision(expectedRev);

			// Act
			var actualRev = _repository.GetRevisionById(expectedRevId);
			var actualRevId = actualRev.Id;

			// Asert
			Assert.AreEqual(expectedRevId, actualRevId);
		}

		[Test]
		public void IncrementRevisionAsync_WhenNotExist_CreateWithNumberOne()
		{
			// Arrange
			const int expectedNextNumber = 1;
			const string revId = "testRev";

			// Act
			var newRevision = _repository.IncrementRevision(revId);
			var actualNextNumber = newRevision.NextNumber;

			// Assert
			Assert.AreEqual(expectedNextNumber, actualNextNumber);
		}

		[Test]
		public void IncrementRevisionAsync_WhenExist_IncrementNumber()
		{
			// Arrange
			const string revId = "testRev";
			const int expectedNextNumber = 4;
			var expectedRev = new Revision {Id = revId, NextNumber = expectedNextNumber - 1};
			_dbHelper.AddRevision(expectedRev);

			// Act
			var actualRev = _repository.IncrementRevision(revId);
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

		internal void AddRevision(Revision revision)
		{
			_storage.Revisions.Insert(revision);
		}

		internal void ClearRevisionCollection()
		{
			var q = Query<Revision>.Where(l => true);
			_storage.Revisions.Remove(q);
		}
	}
}
