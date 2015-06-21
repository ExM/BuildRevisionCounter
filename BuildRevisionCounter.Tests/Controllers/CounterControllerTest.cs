using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Controllers;
using BuildRevisionCounter.Core;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests.Controllers
{
	[TestFixture]
	public class CounterControllerTest
	{
		private CounterController _controller;
		private IRepository _repository;

		[TestFixtureSetUp]
		public void SetUp()
		{
			SetUpAsync().Wait();
		}

		public async Task SetUpAsync()
		{
			_repository = RepositoryFactory.Instance.GetRepository();
			await _repository.DropDatabaseAsync();
			await _repository.SetUpAsync();

			_controller = new CounterController();
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