using System.Threading.Tasks;
using BuildRevisionCounter.Core.Converters.Impl;
using BuildRevisionCounter.Core.DomainObjects;
using BuildRevisionCounter.Core.Repositories.Impl;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests.Repositories
{
	[TestFixture]
	public class RevisionRepositoryTest
	{
		private RevisionRepository _repository;

		[TestFixtureSetUp]
		public void ClassSetUp()
		{
			_repository = new RevisionRepository(new RevisionConverter());
		}

		[SetUp]
		public void TestSetUp()
		{
			_repository.ClearRevisionCollectionAsync().Wait();
		}

		[Test]
		public async Task GetDomainRevisionByIdAsync_WhenIdNotExists_ReturnNull()
		{
			var revision = await _repository.GetDomainRevisionByIdAsync("unknownRev");

			Assert.IsNull(revision);
		}

		[Test]
		public async Task GetDomainRevisionByIdAsync_WhenIdExists_ReturnRevision()
		{
			const string expectedRevId = "testRev";
			var expectedRev = new Revision {Id = expectedRevId};
			await _repository.AddDomainRevisionAsync(expectedRev);

			var actualRev = await _repository.GetDomainRevisionByIdAsync(expectedRevId);
			var actualRevId = actualRev.Id;

			Assert.AreEqual(expectedRevId, actualRevId);
		}

		[Test]
		public async Task IncrementDomainRevisionAsync_WhenNotExist_CreateWithNumberOne()
		{
			const int expectedNextNumber = 1;
			const string revId = "testRev";

			var newRevision = await _repository.IncrementDomainRevisionAsync(revId);
			var actualNextNumber = newRevision.NextNumber;

			Assert.AreEqual(expectedNextNumber, actualNextNumber);
		}

		[Test]
		public async Task IncrementDomainRevisionAsync_WhenExist_IncrementNumber()
		{
			const string revId = "testRev";
			const int expectedNextNumber = 4;
			var expectedRev = new Revision {Id = revId, NextNumber = expectedNextNumber - 1};
			await _repository.AddDomainRevisionAsync(expectedRev);

			var actualRev = await _repository.IncrementDomainRevisionAsync(revId);
			var actualNextNumber = actualRev.NextNumber;

			Assert.AreEqual(expectedNextNumber, actualNextNumber);
		}
	}
}
