using System;
using System.Threading.Tasks;
using System.Configuration;
using BuildRevisionCounter.Interfaces;

namespace BuildRevisionCounter.Data
{
	public static class DbProviderUtil
	{
		public static IDataProvider GetDataProvider(string connectionStringName = "MongoDBStorage")
		{
			const string typeName = "BuildRevisionCounter.Data.MongoDBStorage";
			var type = Type.GetType(typeName);
			if (type == null)
				throw new ApplicationException("на найден класс для IDataProvider");
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return (IDataProvider)Activator.CreateInstance(type, connectionString);
		}
		
		public static async Task<long> Bumping(this IDataProvider dataProvider, string revisionName)
		{
			// попробуем обновить документ
			var result = await dataProvider.FindOneAndUpdateRevisionModelAsync(revisionName);
			if (result != null)
				return result.CurrentNumber;

			// если не получилось, значит документ еще не был создан
			// создадим его с начальным значением 0
			try
			{
				await dataProvider.RevisionInsertAsync(revisionName);
				return 0;
			}
			catch (DuplicateKeyException)
			{
			}

			// если при вставке произошла ошибка значит мы не успели и запись там уже есть
			// и теперь попытка обновления должна пройти без ошибок
			result = await dataProvider.FindOneAndUpdateRevisionModelAsync(revisionName);

			return result.CurrentNumber;
		}

		public static async Task SetUpAsync(this IDatabaseTestProvider dataProvider)
		{
			await dataProvider.DropDatabaseAsync();
			await dataProvider.SetUp();
		}
	}
}