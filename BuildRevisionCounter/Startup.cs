using System;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;
using BuildRevisionCounter;
using BuildRevisionCounter.Interfaces;
using BuildRevisionCounter.Security;
using Microsoft.Owin;
using MongoDB.Driver;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.FilterBindingSyntax;
using Ninject.Web.WebApi.OwinHost;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace BuildRevisionCounter
{
	public class MyAssemblyNameRetriever : AssemblyNameRetriever
	{

	}

	public class Startup
	{
		static Startup()
		{
			AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
		}

		private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		{
			var name = new AssemblyName(args.Name);
			//if (name.Name == "System.Web.Http")
			//{
			//	name.Version = new Version(5, 2, 3, 0);
			//	return Assembly.Load(name);
			//}
			if (name.Name == "System.Web.Http.Owin")
			{
				name.Version = new Version(5, 2, 3, 0);
				return Assembly.Load(name);
			}
			if (name.Name == "Microsoft.Owin")
			{
				name.Version = new Version(3, 0, 1, 0);
				return Assembly.Load(name);
			}
			return null;
		}

		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();

			config.Formatters.Clear();
			config.Formatters.Add(new JsonMediaTypeFormatter()); 

			config.MapHttpAttributeRoutes();
			config.EnsureInitialized();

			app.UseNinjectMiddleware(CreateKernel);
			app.UseNinjectWebApi(config);
		}

		/// <summary>
		/// Создает ядро Ninject.
		/// </summary>
		/// <returns>Созданное ядро Ninject.</returns>
		private static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();
			try
			{
				RegisterServices(kernel);
				return kernel;
			}
			catch
			{
				kernel.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Загрузка модулей Ninject и регистрация сервисов.
		/// </summary>
		/// <param name="kernel">Ядро Ninject.</param>
		private static void RegisterServices(IKernel kernel)
		{
			kernel.Bind<IMongoDBStorage>().ToMethod(c => GetMongoDbStorage()).InSingletonScope();
			kernel.BindHttpFilter<BasicAuthenticationFilter>(FilterScope.Controller).WhenControllerHas<BasicAuthenticationAttribute>();
		}

		private static MongoDBStorage GetMongoDbStorage(string connectionStringName = "MongoDBStorage")
		{
			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			var mongoUrl = MongoUrl.Create(connectionString);
			var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
			return new MongoDBStorage(database);
		}
	}
}
