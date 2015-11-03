using System;
using System.Runtime.Serialization;

namespace BuildRevisionCounter
{
	/// <summary>
	/// Исключение при записи дубля в репозиторий.
	/// </summary>
	public class DuplicateKeyException: Exception
	{
		public DuplicateKeyException() : base()
		{			
		}

		public DuplicateKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public DuplicateKeyException(string message) : base(message)
		{			
		}

		
		public DuplicateKeyException(string message, Exception innerException) : base(message, innerException)
		{			
		}		
	}
}
