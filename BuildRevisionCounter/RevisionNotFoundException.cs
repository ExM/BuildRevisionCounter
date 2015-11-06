using System;
using System.Runtime.Serialization;

namespace BuildRevisionCounter
{
	/// <summary>
	/// Исключение, генерируется в случае если ревизия не была найдена.
	/// </summary>
	public class RevisionNotFoundException : Exception
	{
		public RevisionNotFoundException() : base()
		{
		}

		public RevisionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RevisionNotFoundException(string message) : base(message)
		{
		}

		public RevisionNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

	}
}
