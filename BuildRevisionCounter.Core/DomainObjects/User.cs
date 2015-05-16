﻿using MongoDB.Bson.Serialization.Attributes;

namespace BuildRevisionCounter.Core.DomainObjects
{
	internal class User
	{
		[BsonId]
		public string Name { get; set; }

		[BsonElement("password")]
		public string Password { get; set; }

		[BsonElement("roles")]
		public string[] Roles { get; set; }
	}
}