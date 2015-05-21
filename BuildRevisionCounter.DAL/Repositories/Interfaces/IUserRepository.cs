using System.Threading.Tasks;
using BuildRevisionCounter.Model.BuildRevisionStorage;

namespace BuildRevisionCounter.DAL.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<UserModel>
    {
        /// <summary>
        /// Создаёт первого пользователя
        /// </summary>
        /// <returns></returns>
        Task CreateOneAsync();

        /// <summary>
        /// Создаёт пользователя
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="password">Пароль</param>
        /// <param name="roles">Роли</param>
        /// <returns></returns>
        Task CreateUser(string name, string password, string[] roles);

        /// <summary>
        /// Проверяет наличие пользователей, в случае отсутствия создаёт админа
        /// </summary>
        /// <returns></returns>
        Task EnsureAdminUser();

        /// <summary>
        /// Ищет пользователя по имени
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <returns></returns>
        Task<UserModel> FindUser(string name);
    }
}
