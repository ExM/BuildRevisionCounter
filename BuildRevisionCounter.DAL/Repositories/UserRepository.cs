using System.Threading.Tasks;
using BuildRevisionCounter.DAL.Repositories.Interfaces;
using BuildRevisionCounter.Model.BuildRevisionStorage;
using MongoDB.Driver;

namespace BuildRevisionCounter.DAL.Repositories
{
    public class UserRepository : BaseRepository<UserModel>, IUserRepository
    {
        public static readonly string AdminName = "admin";
        public static readonly string AdminPassword = "admin";
        private static readonly string[] AdminRoles = { Enums.AdminRoles.Admin.ToString().ToLower(), Enums.AdminRoles.Buildserver.ToString().ToLower(), Enums.AdminRoles.Editor.ToString().ToLower() };

        public UserRepository() :
            base("users") { }

        public Task CreateUser(string name, string password, string[] roles)
        {
            return Add(
                    new UserModel
                    {
                        Name = name,
                        Password = password,
                        Roles = roles
                    });
        }

        public async Task CreateOneIndexAsync()
        {
            await DbContext.Indexes.CreateOneAsync(
                Builders<UserModel>.IndexKeys.Ascending(u => u.Name),
                new CreateIndexOptions { Unique = true });
        }

        public async Task EnsureAdminUser()
        {
            if (await Count() == 0)
            {
                await CreateUser(AdminName, AdminPassword, AdminRoles);
            }
        }

        public async Task<UserModel> FindUser(string name)
        {
            return await Get(u => u.Name == name);
        }
    }
}
