using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildRevisionCounter.Core.DomainObjects
{
	public class Revision
	{
		[BsonId] 
		public string Id;

		[BsonElement("created"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)] 
		public DateTime Created;

		[BsonElement("updated"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)] 
		public DateTime Updated;

		[BsonElement("nextNumber")] 
		public long NextNumber;
	}
}