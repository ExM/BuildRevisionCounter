using System;
using System.Threading.Tasks;
using System.Configuration;
using BuildRevisionCounter.Interfaces;

namespace BuildRevisionCounter.Data
{
	public static class DbProviderUtil
	{

		public static string GetAdminName(this IDatabaseTestProvider dataProvider)
		{
			return "admin";
		}

		private const string AdminPassword = "admin";

		private static readonly string[] AdminRoles = { "admin", "buildserver", "editor" };

		public static IRevisionDataProvider GetRevisionDataProvider(string connectionStringName = "MongoDBStorage")
		{
			const string typeName = "BuildRevisionCounter.Data.MongoDBStorage";
			var type = Type.GetType(typeName);
			if (type == null)
				throw new ApplicationException("на найден класс для IDataProvider");
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return (IRevisionDataProvider)Activator.CreateInstance(type, connectionString);
		}

		public static IUserDataProvider GetUserDataProvider(string connectionStringName = "MongoDBStorage")
		{
			const string typeName = "BuildRevisionCounter.Data.MongoDBStorage";
			var type = Type.GetType(typeName);
			if (type == null)
				throw new ApplicationException("на найден класс для IDataProvider");
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return (IUserDataProvider)Activator.CreateInstance(type, connectionString);
		}
		
		public static async Task SetUpAsync(this IDatabaseTestProvider dataProvider)
		{
			await dataProvider.DropDatabaseAsync();
			await dataProvider.SetUp();
		}

		public static async Task EnsureAdminUser(this IDatabaseTestProvider dataProvider)
		{
			if (await dataProvider.FindUser(dataProvider.GetAdminName()) == null)
			{
				await dataProvider.CreateUser(dataProvider.GetAdminName(), AdminPassword, AdminRoles);
			}
		}
	}
}