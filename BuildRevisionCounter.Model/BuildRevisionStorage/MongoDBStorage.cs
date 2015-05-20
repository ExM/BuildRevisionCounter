using System.Threading.Tasks;
using BuildRevisionCounter.Model.Interfaces;
using MongoDB.Driver;

namespace BuildRevisionCounter.Model.BuildRevisionStorage
{
    public class MongoDBStorage //: IMongoDBStorage
    {
        public IMongoDatabase Database { get; private set; }
        //public IMongoCollection<RevisionModel> Revisions { get; private set; }
        //public IMongoCollection<UserModel> Users { get; private set; }

        public MongoDBStorage(IMongoDatabase database) {
            Database = database;
            //Revisions = database.GetCollection<RevisionModel>("revisions");
            //Users = database.GetCollection<UserModel>("users");
        }

        //public async Task SetUp()
        //{

        //    await Users.Indexes.CreateOneAsync(
        //        Builders<UserModel>.IndexKeys.Ascending(u => u.Name),
        //        new CreateIndexOptions { Unique = true });

        //    //await EnsureAdminUser();
        //}
    }
}