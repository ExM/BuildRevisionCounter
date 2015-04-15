using System.Configuration;
using BuildRevisionCounter.Model;
using MongoDB.Driver;

namespace BuildRevisionCounter
{
    public class MongoDBStorage
    {
        public static readonly string AdminName = "admin";
        public readonly IMongoCollection<RevisionModel> Revisions;
        public readonly IMongoCollection<UserModel> Users;

        public MongoDBStorage(string connectionString = null)
            : this(GetDatabase(connectionString))
        {
        }

        public MongoDBStorage(IMongoDatabase database)
        {
            Revisions = database.GetCollection<RevisionModel>("revisions");
            Users = database.GetCollection<UserModel>("users");

            CreateAdmin();
        }

        private static IMongoDatabase GetDatabase(string connectionString)
        {
            //TODO: Убрать это отсюда, это не ответственность класса
            connectionString = connectionString
                               ?? ConfigurationManager.ConnectionStrings["MongoDBStorage"].ConnectionString;

            var mongoUrl = MongoUrl.Create(connectionString);
            var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
            return database;
        }

        private void CreateAdmin()
        {
            if (Users.Find(u => u.Name == AdminName).CountAsync().Result == 0)
            {
                Users
                    .InsertOneAsync(
                        new UserModel
                        {
                            Name = AdminName,
                            Password = AdminName,
                            Roles = new[] {AdminName, "buildserver", "editor"}
                        })
                    .Wait();
            }
        }
    }
}