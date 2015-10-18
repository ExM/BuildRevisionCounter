using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Model;

namespace BuildRevisionCounter.Data
{
    public class DbStorage
    {
        private readonly IDataProvider _dataProvider;

        public DbStorage(string connectionStringName = "MongoDBStorage")
        {
            _dataProvider = GetDataProvider(connectionStringName);
        }

        #region функционал сервиса

        private static IDataProvider GetDataProvider(string connectionStringName)
        {
            const string typeName = "BuildRevisionCounter.Data.MongoDBStorage";
            var type = Type.GetType(typeName);
            if (type == null)
                throw new ApplicationException("на найден класс для IDataProvider");
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            return (IDataProvider)Activator.CreateInstance(type, connectionString);
        }

        public async Task<IReadOnlyCollection<RevisionModel>> GetAllRevision(Int32 pageSize, Int32 pageNumber)
        {
            return await _dataProvider.GetAllRevision(pageSize, pageNumber);
        }

        public async Task<long?> CurrentRevision(string revisionName)
        {
            return await _dataProvider.CurrentRevision(revisionName);
        }

        public async Task<long> Bumping(string revisionName)
        {
            // попробуем обновить документ
            var result = await FindOneAndUpdateRevisionModelAsync(revisionName);
            if (result != null)
                return result.CurrentNumber;

            // если не получилось, значит документ еще не был создан
            // создадим его с начальным значением 0
            try
            {
                await _dataProvider.RevisionInsertAsync(revisionName);
                return 0;
            }
            catch (DuplicateKeyException)
            {
            }

            // если при вставке произошла ошибка значит мы не успели и запись там уже есть
            // и теперь попытка обновления должна пройти без ошибок
            result = await FindOneAndUpdateRevisionModelAsync(revisionName);

            return result.CurrentNumber;
        }

        /// <summary>
        ///		Увеличивает счетчик в БД
        /// </summary>
        /// <param name="revisionName"></param>
        /// <returns></returns>
        private async Task<RevisionModel> FindOneAndUpdateRevisionModelAsync(string revisionName)
        {
            return await _dataProvider.FindOneAndUpdateRevisionModelAsync(revisionName);
        }

        #endregion

        public async Task SetUpAsync()
        {
            await _dataProvider.DropDatabaseAsync();
            await _dataProvider.SetUp();
        }

        #region операции с пользователями

        public string GetAdminName()
        {
            return _dataProvider.AdminName;
        }

        public async Task<UserModel> FindUser(string name)
        {
            return await _dataProvider.FindUser(name);
        }

        public async Task CreateUser(string name, string password, string[] roles)
        {
            await _dataProvider.CreateUser(name, password, roles);
        }

        public async Task EnsureAdminUser()
        {
            await _dataProvider.EnsureAdminUser();
        }



        #endregion
    }
}