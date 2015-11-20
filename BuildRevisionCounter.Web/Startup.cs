using System.Web.Http.Filters;
using Microsoft.Owin;
using MongoDB.Driver;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.FilterBindingSyntax;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using System.Web.Http;
using System.Net.Http.Formatting;
using BuildRevisionCounter.Web.Security;
using BuildRevisionCounter.MongoDB;
using BuildRevisionCounter.Web.Filters;

[assembly: OwinStartup(typeof(BuildRevisionCounter.Web.Startup))]

namespace BuildRevisionCounter.Web
{
	public class Startup
	{
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
			kernel.Bind<IUserRepository>().To<MongoUserRepository>().InSingletonScope();
			kernel.Bind<IRevisionRepository>().To<MongoRevisionRepository>().InSingletonScope();
			kernel.Bind<IMongoDatabase>().ToMethod(c=>MongoHelper.GetMongoDb()).InSingletonScope();
			kernel.BindHttpFilter<BasicAuthenticationFilter>(FilterScope.Controller).WhenControllerHas<BasicAuthenticationAttribute>();
			kernel.BindHttpFilter<KnownExceptionFilterAttribute>(FilterScope.Controller);
			kernel.BindHttpFilter<RewriteResponseCodeFilterAttribute>(FilterScope.Controller);
		}		
	}
}
