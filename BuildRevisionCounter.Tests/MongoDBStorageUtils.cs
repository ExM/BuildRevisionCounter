using System;
using System.Threading.Tasks;

namespace BuildRevisionCounter.Tests
{
	internal class MongoDBStorageUtils
	{
		public static async Task SetUpAsync()
		{
			var storage = MongoDBStorageFactory.DefaultInstance;
			await storage.Revisions.Database.Client.DropDatabaseAsync(
				storage.Revisions.Database.DatabaseNamespace.DatabaseName);

			await storage.SetUp();			
		}
	}
}
