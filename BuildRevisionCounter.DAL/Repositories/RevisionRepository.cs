using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BuildRevisionCounter.DAL.Repositories.Interfaces;
using BuildRevisionCounter.Model.BuildRevisionStorage;
using MongoDB.Driver;

namespace BuildRevisionCounter.DAL.Repositories
{
    public class RevisionRepository : BaseRepository<RevisionModel>, IRevisionRepository
    {
        public RevisionRepository()
            : base("revisions") { }

        public async Task<RevisionModel> FindOneAndUpdate(Expression<Func<RevisionModel, bool>> searchPredicate, UpdateDefinition<RevisionModel> update, FindOneAndUpdateOptions<RevisionModel> updateOptions)
        {
            return await DbContext.FindOneAndUpdateAsync<RevisionModel>(
                    searchPredicate,
                    update,
                    updateOptions);
        }

        public async Task DropDatabaseAsync()
        {
            await DbContext.Database.Client.DropDatabaseAsync(
                DbContext.Database.DatabaseNamespace.DatabaseName);
        }
    }
}
