using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildRevisionCounter.Core.Repositories
{
	public interface IRevisionRepository
	{
		Task<Protocol.Revision> GetRevisionByIdAsync(string revisionId);

		Task<Protocol.Revision> IncrementRevisionAsync(string revisionId);

		Task AddRevisionAsync(Protocol.Revision revision);

		Task ClearRevisionCollectionAsync();

		Task<List<Protocol.Revision>> GetAllRevisionAsync(Int32 pageSize = 20, Int32 pageNumber = 1);
	}
}
