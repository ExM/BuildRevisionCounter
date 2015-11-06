using System;
using System.Configuration;
using MongoDB.Driver;
using BuildRevisionCounter.MongoDB;

namespace BuildRevisionCounter.Tests
{
	public static class MongoDatabaseFactory
	{
		private static readonly Lazy<IMongoDatabase> _defaultInstance =
			new Lazy<IMongoDatabase>(() => MongoHelper.GetMongoDb());

		public static IMongoDatabase DefaultInstance { get { return _defaultInstance.Value; } }	
		
	}
}