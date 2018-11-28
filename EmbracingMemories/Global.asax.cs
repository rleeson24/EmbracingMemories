using EmbracingMemories.Utilities;
using Stripe;
using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EmbracingMemories
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			StripeConfiguration.SetApiKey( ConfigurationSettings.AppSettings[ "StripeCode" ] );
			GlobalConfiguration.Configure( WebApiConfig.Register );
			FilterConfig.RegisterGlobalFilters( GlobalFilters.Filters );
			RouteConfig.RegisterRoutes( RouteTable.Routes );
			BundleConfig.RegisterBundles( BundleTable.Bundles );
		}

		protected void Application_Error( Object sender, EventArgs e )
		{
			var raisedException = Server.GetLastError();

			var message = new EmailService.EmailMessage();
			message.Recipients = "rleeson_2000@yahoo.com";
			message.Subject = "Exception Occurred in EmbracingMemories";
			message.Body = String.Format( "Unhandled exception processing: {0}",
										raisedException );

			EmailService.Send( "Error", message );

		}
	}
}
