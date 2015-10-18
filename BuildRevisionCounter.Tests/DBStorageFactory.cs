using System;
using BuildRevisionCounter.Data;

namespace BuildRevisionCounter.Tests
{
	public static class DBStorageFactory
	{
		private static readonly Lazy<DbStorage> _defaultInstance =
            new Lazy<DbStorage>(() => FromConfigurationConnectionString());

        public static DbStorage DefaultInstance { get { return _defaultInstance.Value; } }

        public static DbStorage FromConnectionString(string connectionStringName)
		{
            return new DbStorage(connectionStringName);
		}

        public static DbStorage FromConfigurationConnectionString(string connectionStringName = "MongoDBStorage")
        {
            return FromConnectionString(connectionStringName);
		}
	}
}