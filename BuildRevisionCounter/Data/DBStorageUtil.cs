using System.Threading.Tasks;
using System.Configuration;
using BuildRevisionCounter.Interfaces;

namespace BuildRevisionCounter.Data
{
	public static class DBStorageUtil
	{

		public static string GetAdminName(this IUserDatabaseTestProvider dataProvider)
		{
			return "admin";
		}

		private const string AdminPassword = "admin";

		private static readonly string[] AdminRoles = { "admin", "buildserver", "editor" };

		public static IRevisionStorage GetRevisionStorage(string connectionStringName = "MongoDBStorage", string connectionString = null)
		{
			connectionString = connectionString ?? ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return new MongoDBRevisionStorage(connectionString);
		}

		public static IUserStorage GetUserStorage(string connectionStringName = "MongoDBStorage")
		{
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			return new MongoDBUserStorage(connectionString);
		}

		public static async Task EnsureAdminUser(this IUserDatabaseTestProvider dataProvider)
		{
			if (await dataProvider.FindUser(dataProvider.GetAdminName()) == null)
			{
				await dataProvider.CreateUser(dataProvider.GetAdminName(), AdminPassword, AdminRoles);
			}
		}
	}
}