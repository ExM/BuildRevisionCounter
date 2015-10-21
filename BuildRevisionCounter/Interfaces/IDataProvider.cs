using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildRevisionCounter.Model;

namespace BuildRevisionCounter.Interfaces
{
	/// <summary>
	/// Интерфейс для получения данных из БД.
	/// </summary>
	public interface IDataProvider
	{
		Task<IReadOnlyCollection<RevisionModel>> GetAllRevision(Int32 pageSize, Int32 pageNumber);

		Task<long?> CurrentRevision(string revisionName);

		Task RevisionInsertAsync(string revisionName);

		Task<RevisionModel> FindOneAndUpdateRevisionModelAsync(string revisionName);

		Task<UserModel> FindUser(string name);
	}
}