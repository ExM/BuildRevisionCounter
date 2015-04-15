using System;
using System.Configuration;
using MongoDB.Driver;

namespace BuildRevisionCounter
{
	public static class MongoDBStorageFactory
	{
		private static readonly Lazy<MongoDBStorage> _defaultInstance =
			new Lazy<MongoDBStorage>(() => FromConfigurationConnectionString());

		public static MongoDBStorage DefaultInstance { get { return _defaultInstance.Value; } }

		public static MongoDBStorage FromConnectionString(string connectionString)
		{
			var mongoUrl = MongoUrl.Create(connectionString);
			var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
			return new MongoDBStorage(database);
		}

		public static MongoDBStorage FromConfigurationConnectionString(string connectionStringName = "MongoDBStorage")
		{
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return FromConnectionString(connectionString);
		}
	}
}