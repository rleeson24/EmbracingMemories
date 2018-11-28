using System.Web.Optimization;

namespace EmbracingMemories
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles( BundleCollection bundles )
		{
			bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
						"~/Scripts/jquery-{version}.js" ) );

			bundles.Add( new ScriptBundle( "~/bundles/jqueryval" ).Include(
				"~/Scripts/jquery.unobtrusive*",
				"~/Scripts/jquery.validate*" ) );

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
						"~/Scripts/modernizr-*" ) );

			bundles.Add( new ScriptBundle( "~/bundles/bootstrap" ).Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js" ) );

			bundles.Add( new ScriptBundle( "~/bundles/application" ).Include(
					  "~/AngularApp/Scripts/models.js",
					  "~/Scripts/angular-local-storage.min.js",
					  "~/AngularApp/angular-block-ui/angular-block-ui.min.js",
					  "~/AngularApp/angular-growl-notifications/angular-growl-notifications.min.js",
					  "~/AngularApp/Scripts/app.js" ) );
			bundles.Add( new ScriptBundle( "~/bundles/application.test" ).Include(
					  "~/AngularApp/Scripts/models.js",
					  "~/Scripts/angular-local-storage.min.js",
					  "~/AngularApp/angular-block-ui/angular-block-ui.js",
					  "~/AngularApp/angular-growl-notifications/angular-growl-notifications.js",
					  "~/AngularApp/angular-block-ui/config.js",
					  "~/AngularApp/angular-block-ui/interceptor.js",
					  "~/AngularApp/angular-block-ui/service.js",
					  "~/AngularApp/angular-block-ui/block-navigation.js",
					  "~/AngularApp/angular-block-ui/block-ui-container-directive.js",
					  "~/AngularApp/angular-block-ui/block-ui-directive.js",
					  "~/AngularApp/angular-block-ui/utils.js",
					  "~/AngularApp/Scripts/app.js" ) );
			bundles.Add( new StyleBundle( "~/Content/css" ).Include(
					  "~/Content/bootstrap.css",
					  "~/Content/site.css",
					  "~/AngularApp/angular-block-ui/angular-block-ui.min.css" ) );
		}
	}
}
