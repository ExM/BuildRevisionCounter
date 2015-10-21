using System;
using System.Configuration;
using BuildRevisionCounter.Data;
using BuildRevisionCounter.Interfaces;

namespace BuildRevisionCounter.Tests
{
	public static class DBStorageFactory
	{
		private static readonly Lazy<IDatabaseTestProvider> _defaultInstance =
			new Lazy<IDatabaseTestProvider>(() => FromConfigurationConnectionString());

		public static IDatabaseTestProvider DefaultInstance { get { return _defaultInstance.Value; } }


		public static IDatabaseTestProvider GetInstance<T>(string connectionStringName = "MongoDBStorage") where T : IDatabaseTestProvider
		{
			return GetDatabaseTestProvider<T>(connectionStringName);
		}

		public static IDatabaseTestProvider FromConfigurationConnectionString(string connectionStringName = "MongoDBStorage")
        {
			return GetDatabaseTestProvider(connectionStringName);
		}

		private static IDatabaseTestProvider GetDatabaseTestProvider(string connectionStringName)
		{
			const string typeName = "BuildRevisionCounter.Data.MongoDBStorage,BuildRevisionCounter";
			var type = Type.GetType(typeName);
			if (type == null)
				throw new ApplicationException("на найден класс для IDataProvider");
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return (IDatabaseTestProvider)Activator.CreateInstance(type, connectionString);
		}

		private static IDatabaseTestProvider GetDatabaseTestProvider<T>(string connectionStringName) where T : IDatabaseTestProvider
		{
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return (IDatabaseTestProvider)Activator.CreateInstance(typeof(T), connectionString);
		}
	}
}