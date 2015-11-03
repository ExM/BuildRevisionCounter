using BuildRevisionCounter.MongoDB.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildRevisionCounter.MongoDB
{
	public static class MongoHelper
	{
		public static IMongoDatabase GetMongoDb(string connectionStringName = "MongoDBStorage")
		{ 
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			var mongoUrl = MongoUrl.Create(connectionString);
			return new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
		}
	}
}
