
using EmbracingMemories.Utilities;
using System;
using System.Web.Mvc;

namespace EmbracingMemories
{
	public class CustomErrorHandlerAttribute : HandleErrorAttribute
	{
		public override void OnException( ExceptionContext filterContext )
		{
			Exception ex = filterContext.Exception;
			var message = new EmailService.EmailMessage();
			message.Recipients = "rleeson_2000@yahoo.com";
			message.Subject = "Exception Occurred in EmbracingMemories";
			message.Body = String.Format( "Unhandled exception processing: {0}", ex );

			EmailService.Send( "Error", message );

			filterContext.ExceptionHandled = true;
			var model = new HandleErrorInfo( filterContext.Exception, "Controller", "Action" );

			filterContext.Result = new ViewResult()
			{
				ViewName = "Error",
				ViewData = new ViewDataDictionary( model )
			};
		}
	}
}