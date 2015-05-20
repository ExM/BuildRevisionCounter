using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Api.Controllers;
using BuildRevisionCounter.DAL.Repositories;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests.Controllers
{
    [TestFixture]
    public class CounterControllerTest
    {
        private CounterController _controller;

        [TestFixtureSetUp]
        public void SetUp()
        {
            SetUpAsync().Wait();
        }

        private async Task SetUpAsync()
        {
            UserRepository userRepo = RepositoriesFactory.UserRepoInstance;
            RevisionRepository revisionRepo = RepositoriesFactory.RevisionRepoInstance;

            await revisionRepo.DropDatabaseAsync();
            await userRepo.CreateOneAsync();
            await userRepo.EnsureAdminUser();

            _controller = new CounterController(userRepo, revisionRepo);
        }

        [Test]
        public async Task CurrentThrowsExceptionIfRevisionNotFound()
        {
            try
            {
                var rev = await _controller.Current("CurrentThrowsExceptionIfRevisionNotFound");
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }

        [Test]
        public async Task BumpingNewRevisionReturnsOne()
        {
            var rev = await _controller.Bumping("BumpingNewRevisionReturnsZero");
            Assert.AreEqual(1, rev);
        }

        [Test]
        public async Task BumpingIncrementsRevisionNumber()
        {
            var rev1 = await _controller.Bumping("BumpingIncrementsRevisionNumber");
            var rev2 = await _controller.Bumping("BumpingIncrementsRevisionNumber");
            Assert.AreEqual(rev1 + 1, rev2);
        }

        [Test]
        public async Task CurrentReturnsSameValueAsPreviousBumping()
        {
            var rev1 = await _controller.Bumping("CurrentReturnSameValueAsPreviousBumping");
            var rev2 = await _controller.Current("CurrentReturnSameValueAsPreviousBumping");
            Assert.AreEqual(rev1, rev2);
        }
    }
}