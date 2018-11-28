using System.Web.Mvc;
using System.Web.Routing;

namespace EmbracingMemories
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			AreaRegistration.RegisterAllAreas();

			routes.MapRoute(
				name: "QrManagement",
				url: "Profiles/{*catch-all}",
				defaults: new { controller = "Home", action = "QrProfile" }
			);
			routes.MapRoute(
				name: "About",
				url: "About/{*catch-all}",
				defaults: new { controller = "Home", action = "About" }
			);
			routes.MapRoute(
				name: "ViewProfile",
				url: "ViewProfile/{id}",
				defaults: new { controller = "Home", action = "ViewProfile" }
			);
			routes.MapRoute(
				name: "BindQrCode",
				url: "BindQrCode/{id}",
				defaults: new { controller = "Home", action = "BindQrCode" }
			);
			routes.MapRoute(
				name: "Archive",
				url: "Archive/{*catch-all}",
				defaults: new { controller = "Home", action = "Archive" }
			);
			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);

			//// Add this code to handle non-existing urls
			//routes.MapRoute(
			//    name: "404-PageNotFound",
			//    // This will handle any non-existing urls
			//    url: "{*url}",
			//    // "Shared" is the name of your error controller, and "Error" is the action/page
			//    // that handles all your custom errors
			//    defaults: new { controller = "Shared", action = "Error" }
			//);
		}
	}
}
