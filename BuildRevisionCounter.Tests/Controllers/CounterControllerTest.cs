using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildRevisionCounter.Controllers;
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

		public async Task SetUpAsync()
		{
			var storage = MongoDBStorageFactory.DefaultInstance;
			await storage.Revisions.Database.Client.DropDatabaseAsync(
				storage.Revisions.Database.DatabaseNamespace.DatabaseName);

			await storage.SetUp();

			_controller = new CounterController(storage);
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
		public async Task BumpingNewRevisionReturnsZero()
		{
			var rev = await _controller.Bumping("BumpingNewRevisionReturnsZero");
			Assert.AreEqual(0, rev);
		}

		[Test]
		public async Task BumpingNewRevisionMayBeInvokedMultipleTimes()
		{
			try
			{
				const string revName = "BumpingNewRevisionMayBeInvokedMultipleTimes";
				await _controller.Bumping(revName);
				await _controller.Bumping(revName);
			}
			catch (Exception)
			{
				Assert.Fail("Ошибка при создании одного каунтера из разных потоков");
			}
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

		[Test]
		public async Task GetAllRevisionReturnsAllRevision()
		{
			const string revName1 = "BumpingIncrementsRevisionNumber1";
			const string revName2 = "BumpingIncrementsRevisionNumber2";
			const string revName3 = "BumpingIncrementsRevisionNumber3";

			await _controller.Bumping(revName1);
			await _controller.Bumping(revName1);
			await _controller.Bumping(revName2);
			await _controller.Bumping(revName3);

			var rev1 = await _controller.Current(revName1);
			var rev2 = await _controller.Current(revName2);
			var rev3 = await _controller.Current(revName3);

			var result = await _controller.GetAllRevision();

			Assert.IsTrue(result.Any(x => x.Id == revName1 && x.NextNumber == rev1));
			Assert.IsTrue(result.Any(x => x.Id == revName2 && x.NextNumber == rev2));
			Assert.IsTrue(result.Any(x => x.Id == revName3 && x.NextNumber == rev3));
		}

		[Test]
		public async Task GetAllRevisionReturnsSecondPage()
		{
			// пересоздадим БД перед этим тестом,
			// он должен проводиться на чистой БД
			SetUpAsync().Wait();

			for (int i = 0; i < 10; i++)
			{
				string revName = "GetAllRevisionReturnsSecondPage" + i.ToString();
				await _controller.Bumping(revName);
			}

			var result = await _controller.GetAllRevision(3, 2);

			Assert.IsTrue(result.Any(x => x.Id == "GetAllRevisionReturnsSecondPage3"));
			Assert.IsTrue(result.Any(x => x.Id == "GetAllRevisionReturnsSecondPage4"));
			Assert.IsTrue(result.Any(x => x.Id == "GetAllRevisionReturnsSecondPage5"));
		}

		[Test]
		public async Task GetAllRevisionArgumentExceptionPageSize()
		{
			try
			{
				await _controller.GetAllRevision(0, 2);
				Assert.Fail();
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
			}
		}

		[Test]
		public async Task GetAllRevisionArgumentExceptionPageNumber()
		{
			try
			{
				await _controller.GetAllRevision(3, 0);
				Assert.Fail();
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
			}
		}
	}
}