using System.Threading.Tasks;

namespace BuildRevisionCounter.Core.Repositories
{
	public interface IRevisionRepository
	{
		Task<Protocol.Revision> GetRevisionByIdAsync(string revisionId);

		Task<Protocol.Revision> IncrementRevisionAsync(string revisionId);

		Task AddRevisionAsync(Protocol.Revision revision);

		Task ClearRevisionCollectionAsync();
	}
}
