using BuildRevisionCounter.MongoDB;
using BuildRevisionCounter.MongoDB.Model;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BuildRevisionCounter.Tests
{
	internal class MongoDBStorageUtils
	{
		public static async Task SetUpAsync()
		{
			var db = MongoDatabaseFactory.DefaultInstance;
			await db.Client.DropDatabaseAsync(db.DatabaseNamespace.DatabaseName);			
		}		
	}
}
