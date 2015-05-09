using System.Threading.Tasks;
using BuildRevisionCounter.Contract;

namespace BuildRevisionCounter.Core.Repositories
{
	public interface IRevisionRepository
	{
		Contract.Revision GetRevisionById(string revisionId);

		Contract.Revision IncrementRevision(string revisionId);
	}
}
