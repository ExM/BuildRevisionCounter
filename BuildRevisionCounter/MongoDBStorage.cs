using BuildRevisionCounter.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BuildRevisionCounter
{
	public class MongoDBStorage
	{
		public readonly MongoCollection<RevisionModel> Revisions;

		public MongoDBStorage()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["MongoDBStorage"].ConnectionString;
			var database = MongoDatabase.Create(connectionString);

			Revisions = database.GetCollection<RevisionModel>("revisions");
		}
	}
}