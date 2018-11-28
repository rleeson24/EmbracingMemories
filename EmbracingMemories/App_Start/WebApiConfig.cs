using EmbracingMemories.Utilities;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;

namespace EmbracingMemories
{
	public static class WebApiConfig
	{
		public static void Register( HttpConfiguration config )
		{
			// Web API configuration and services
			// Configure Web API to use only bearer token authentication.
			config.SuppressDefaultHostAuthentication();
			config.Filters.Add( new HostAuthenticationFilter( OAuthDefaults.AuthenticationType ) );
			config.Services.Add( typeof( IExceptionLogger ), new CustomExceptionLogger() );
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
			// Enable CORS for all origins, all headers, and all methods,
			// You can customize this to match your requirements
			var cors = new EnableCorsAttribute( "*", "*", "*" );
			config.EnableCors( cors );
			// Web API routes
			config.MapHttpAttributeRoutes();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "text/html" ) );
			//config.Routes.MapHttpRoute(
			//    name: "DefaultApi",
			//    routeTemplate: "api/{controller}/{id}",
			//    defaults: new { id = RouteParameter.Optional }
			//);
			config.Routes.MapHttpRoute( "DefaultApiWithId", "api/{controller}/{id}", new { id = RouteParameter.Optional } );
			config.Routes.MapHttpRoute( "DefaultApiWithAction", "api/{controller}/{action}" );
			config.Routes.MapHttpRoute( "DefaultApiGet", "api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint( HttpMethod.Get ) } );
			config.Routes.MapHttpRoute( "DefaultApiPost", "api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint( HttpMethod.Post ) } );
			config.Routes.MapHttpRoute( "DefaultApiPut", "api/{controller}", new { action = "Put" }, new { httpMethod = new HttpMethodConstraint( HttpMethod.Put ) } );
			config.Routes.MapHttpRoute( "DefaultApiDelete", "api/{controller}", new { action = "Delete" }, new { httpMethod = new HttpMethodConstraint( HttpMethod.Delete ) } );
			config.Routes.MapHttpRoute(
			  name: "GenerateResetToken",
			  routeTemplate: "api/Account/GenerateResetToken/",
			  defaults: new { controller = "Account", action = "GenerateResetToken" }
			);
		}

		public class CustomExceptionLogger : ExceptionLogger
		{
			public override Task LogAsync( ExceptionLoggerContext context, CancellationToken cancellationToken )
			{
				return base.LogAsync( context, cancellationToken );
			}

			public override void Log( ExceptionLoggerContext context )
			{
				var message = new EmailService.EmailMessage();
				message.Recipients = "rleeson_2000@yahoo.com";
				//message.Recipients = "support@embracingthememories.com";
				message.Subject = "Exception Occurred in EmbracingMemories";
				message.Body = String.Format( "Unhandled exception processing {0} for {1}: {2}",
											context.Request.Method,
											context.Request.RequestUri,
											context.Exception );

				EmailService.Send( "Error", message );
				base.Log( context );
			}
		}
	}
}
