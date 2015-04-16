using System.Threading.Tasks;
using BuildRevisionCounter.Contract;

namespace BuildRevisionCounter.Core.Repositories
{
    public interface IRevisionRepository
    {
        Task<Contract.Revision> GetRevisionByIdAsync(string revisionId);

        Task<Contract.Revision> IncrementRevisionAsync(string revisionId);
    }
}
