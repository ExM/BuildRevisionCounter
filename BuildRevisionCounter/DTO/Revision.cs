using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildRevisionCounter.DTO
{
	/// <summary>
	/// DTO ревизий, неизменяемый.
	/// </summary>
	public class Revision
	{
		/// <summary>
		/// Идентификатор резвизии.
		/// </summary>
		public string Id { get; private set; }
		/// <summary>
		/// Дата создания ревизии.
		/// </summary>
		public DateTime Created { get; private set; }

		/// <summary>
		/// Дата изменения ревизии.
		/// </summary>
		public DateTime Updated { get; private set; }

		/// <summary>
		/// Номер ревизии.
		/// </summary>
		public long CurrentNumber { get; private set; }

		/// <summary>
		/// Создает новый экземпляр.
		/// </summary>
		/// <param name="id">Идентификатор ревизии.</param>
		/// <param name="created">Дата создания ревизии.</param>
		/// <param name="updated">Дата изменения ревизии.</param>
		/// <param name="currNumber">Текущий номер ревизии.</param>
		public Revision(string id, DateTime created, DateTime updated, long currNumber)
		{
			Id = id;
			Created = created;
			Updated = updated;
			CurrentNumber = currNumber;
		}
	}
}