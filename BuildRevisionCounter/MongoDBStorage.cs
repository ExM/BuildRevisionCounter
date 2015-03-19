using BuildRevisionCounter.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Wrappers;

namespace BuildRevisionCounter
{
	public class MongoDBStorage
	{
		public readonly MongoCollection<RevisionModel> Revisions;
		public readonly MongoCollection<UserModel> Users;

		public MongoDBStorage()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["MongoDBStorage"].ConnectionString;
			var database = MongoDatabase.Create(connectionString);

			Revisions = database.GetCollection<RevisionModel>("revisions");
			Users = database.GetCollection<UserModel>("users");

			CreateAdmin();
		}

		private void CreateAdmin()
		{
			if (!Users.AsQueryable().Any())
			{
				Users.Insert(new UserModel
				{
					Name = "admin",
					Password = "admin",
					Roles = new[] {"admin", "buildserver", "editor"}
				});
			}
		}
	}
}