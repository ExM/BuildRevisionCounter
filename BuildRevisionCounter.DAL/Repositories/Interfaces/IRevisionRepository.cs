using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BuildRevisionCounter.Model.BuildRevisionStorage;
using MongoDB.Driver;

namespace BuildRevisionCounter.DAL.Repositories.Interfaces
{
    public interface IRevisionRepository : IRepository<RevisionModel>
    {
        /// <summary>
        /// Очищает базу данных
        /// </summary>
        /// <returns></returns>
        Task DropDatabaseAsync();

        /// <summary>
        /// Находит нужный элемент и обновляет его
        /// </summary>
        /// <param name="searchPredicate">Предикат поиска нужного элемента</param>
        /// <param name="update">Действие для обновления</param>
        /// <param name="updateOptions">Опции обновления</param>
        /// <returns></returns>
        Task<RevisionModel> FindOneAndUpdate(Expression<Func<RevisionModel, bool>> searchPredicate, UpdateDefinition<RevisionModel> update, FindOneAndUpdateOptions<RevisionModel> updateOptions);
    }
}
