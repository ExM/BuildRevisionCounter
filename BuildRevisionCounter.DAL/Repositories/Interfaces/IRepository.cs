using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BuildRevisionCounter.DAL.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IRepository<T> where T : class, new()
    {
        /// <summary>
        /// Получает сущность по условию
        /// </summary>
        /// <param name="predicate">Предикат поиска</param>
        /// <returns></returns>
        Task<T> Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Получает сущность по условию
        /// </summary>
        /// <param name="predicate">Предикат поиска</param>
        /// <returns></returns>
        Task<T> Get(FilterDefinition<T> predicate);

        /// <summary>
        /// Добавляет новую сущность в репозиторий
        /// </summary>
        /// <param name="entity">Новая сущность</param>
        /// <returns></returns>
        Task Add(T entity);

        /// <summary>
        /// Обновляет сущность в репозитории
        /// </summary>
        /// <param name="filter">Условия поиска обновляемого объекта</param>
        /// <param name="update">Поля для обновления</param>
        Task Update(FilterDefinition<T> filter, UpdateDefinition<T> update);

        /// <summary>
        /// Количество элементов сущности по условию
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<long> Count(Expression<Func<T, bool>> predicate);
    }
}
