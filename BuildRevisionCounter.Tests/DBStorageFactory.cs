using System;
using System.Configuration;
using BuildRevisionCounter.Data;
using BuildRevisionCounter.Interfaces;

namespace BuildRevisionCounter.Tests
{
	public static class DBStorageFactory
	{
		private static readonly Lazy<IUserDatabaseTestProvider> _defaultInstance =
			new Lazy<IUserDatabaseTestProvider>(() => FromConfigurationConnectionString());

		public static IUserDatabaseTestProvider DefaultInstance { get { return _defaultInstance.Value; } }


		public static IUserDatabaseTestProvider GetInstance<T>(string connectionStringName = "MongoDBStorage") where T : IUserDatabaseTestProvider
		{
			return GetDatabaseTestProvider<T>(connectionStringName);
		}

		public static IUserDatabaseTestProvider FromConfigurationConnectionString(string connectionStringName = "MongoDBStorage")
		{
			return GetDatabaseTestProvider(connectionStringName);
		}

		private static IUserDatabaseTestProvider GetDatabaseTestProvider(string connectionStringName)
		{
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			connectionString = DBUtil.SetDatabaseNameRandom(typeof(MongoDBUserStorage), connectionString);
			return new MongoDBUserStorage(connectionString);
		}

		private static IUserDatabaseTestProvider GetDatabaseTestProvider<T>(string connectionStringName) where T : IUserDatabaseTestProvider
		{
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			connectionString = DBUtil.SetDatabaseNameRandom(typeof (T), connectionString);
			return (IUserDatabaseTestProvider)Activator.CreateInstance(typeof(T), connectionString);
		}
	}
}