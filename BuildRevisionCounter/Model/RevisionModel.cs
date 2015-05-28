using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildRevisionCounter.Model
{
	public class RevisionModel
	{
		[BsonId]
		public string Id;

		[BsonElement("created"), BsonDateTimeOptions(Kind=DateTimeKind.Utc)]
		public DateTime Created;

		[BsonElement("updated"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime Updated;

		[BsonElement("currentNumber")]
		public long CurrentNumber;

	}
}