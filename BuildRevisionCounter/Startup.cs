using System.Web.Http.Filters;
using BuildRevisionCounter.Data;
using BuildRevisionCounter.Security;
using Microsoft.Owin;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.FilterBindingSyntax;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using BuildRevisionCounter;
using System.Web.Http;
using System.Net.Http.Formatting;

[assembly: OwinStartup(typeof(Startup))]

namespace BuildRevisionCounter
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
            kernel.Bind<DbStorage>().ToMethod(c => new DbStorage()).InSingletonScope();
			kernel.BindHttpFilter<BasicAuthenticationFilter>(FilterScope.Controller).WhenControllerHas<BasicAuthenticationAttribute>();
		}
	}
}
