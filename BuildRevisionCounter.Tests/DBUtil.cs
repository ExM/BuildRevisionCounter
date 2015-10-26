using System;
using System.Threading.Tasks;
using BuildRevisionCounter.Data;
using BuildRevisionCounter.Interfaces;
using MongoDB.Driver;

namespace BuildRevisionCounter.Tests
{
	internal static class DBUtil
	{
		internal static Task DropDatabaseAsync(this IUserDatabaseTestProvider dataProvider)
		{
			switch (GetDatabaseKind(dataProvider.GetType()))
			{
				case DatabaseKind.MongoDB:
					return MangoDBDropDatabaseAsync(dataProvider.ConnectionString);
				default:
					throw new NotImplementedException();
			}
		}

		internal static string SetDatabaseNameRandom(Type storageType, string connectionString)
		{
			switch (GetDatabaseKind(storageType))
			{
				case DatabaseKind.MongoDB:
					return MangoDBSetDatabaseNameRandom(connectionString);
				default:
					throw new NotImplementedException();
			}
		}

		private static string MangoDBSetDatabaseNameRandom(string connectionString)
		{
			var urlBuilder = new MongoUrlBuilder(connectionString)
								 {
									 DatabaseName = "brCounterTest_" + Guid.NewGuid().ToString("N")
								 };
			return urlBuilder.ToString();
		}

		private static Task MangoDBDropDatabaseAsync(string connectionString)
		{
			var mongoUrl = MongoUrl.Create(connectionString);
			return new MongoClient(mongoUrl).DropDatabaseAsync(mongoUrl.DatabaseName);
		}


		private enum DatabaseKind
		{
			Unknow,
			MongoDB
		}

		private static DatabaseKind GetDatabaseKind(Type storageType)
		{
			if (storageType == typeof(MongoDBUserStorage))
				return DatabaseKind.MongoDB;
			return DatabaseKind.Unknow;
		}
	}
}
