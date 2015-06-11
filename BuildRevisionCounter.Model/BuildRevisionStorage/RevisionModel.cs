using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildRevisionCounter.Model.BuildRevisionStorage
{
	public class RevisionModel
	{
		[BsonId]
		public string Id;

		[BsonElement("created"), BsonDateTimeOptions(Kind=DateTimeKind.Utc)]
		public DateTime Created;

		[BsonElement("updated"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime Updated;

		[BsonElement("nextNumber")]
		public long NextNumber;

	}
}