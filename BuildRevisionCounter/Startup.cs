using System;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
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

			app.UseWebApi(config);
		}
	}
}
