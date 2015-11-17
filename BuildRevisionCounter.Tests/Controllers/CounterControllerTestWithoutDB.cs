using BuildRevisionCounter.Web.Controllers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildRevisionCounter.Tests.Controllers
{
	[TestFixture]
	public class CounterControllerTestWithoutDB
	{
		private CounterController _controller;

		[TestFixtureSetUp]
		public void SetUp()
		{			
		}

		[Test]
		public async Task BumpingNewRevisionReturnsZero()
		{	
			var rr = new Moq.Mock<IRevisionRepository>();
			rr.Setup(m=>m.Bumping(Moq.It.IsAny<string>())).Returns<string>(revName=> Task.Run<long>(()=>0));
			_controller = new CounterController(rr.Object);

			var rev = await _controller.Bumping("BumpingNewRevisionReturnsZero");
			Assert.AreEqual(0, rev);
		}

		[Test]
		public async Task BumpingIncrementsRevisionNumber()
		{
			var revs = new Dictionary<string, long>();
			var rr = new Moq.Mock<IRevisionRepository>();
			rr.Setup(m=>m.Bumping(Moq.It.IsAny<string>())).Returns<string>(
				revName=> Task.Run(()=>
				{
					if(!revs.ContainsKey(revName))
						revs.Add(revName, 0);
					else
						revs[revName]++;

					 return revs[revName];
				}));
			_controller = new CounterController(rr.Object);

			var rev1 = await _controller.Bumping("BumpingIncrementsRevisionNumber");			
			var rev2 = await _controller.Bumping("BumpingIncrementsRevisionNumber");
			Assert.AreEqual(rev1 + 1, rev2);
		}
	}
}
