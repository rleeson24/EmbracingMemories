using EmbracingMemories.Controllers;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EmbracingMemories.Utilities
{
	public static class EmailService
	{
		public static String Signature
		{
			get
			{
				return @"<p>
                           <img src=""http://embracingthememories.com/Content/Images/EmailLogo.png"" height=""100px"" />
                         </p>";
			}
		}

		public static string RenderEmailViewToString( string viewName, object model )
		{
			var sw = new StringWriter();
			var httpContext = new HttpContextWrapper( HttpContext.Current );

			// point to an empty controller
			var routeData = new RouteData();
			routeData.Values.Add( "controller", "Email" );

			var controllerContext = new ControllerContext( new RequestContext( httpContext, routeData ), new EmailController() );

			var view = ViewEngines.Engines.FindView( controllerContext, viewName, "EmailLayout" ).View;

			view.Render( new ViewContext( controllerContext, view, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw ), sw );

			return sw.ToString();
		}



		public static void Send( String viewName, EmailMessage message )
		{
			var smtpClient = new SmtpClient( "smtpout.secureserver.net", 3535 );
			smtpClient.EnableSsl = false;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential( "support@embracingthememories.com", "j5lzAaeNz4XWnNEYptLU" );
			//var smtpClient = new SmtpClient("smtp.gmail.com", 587);
			//smtpClient.EnableSsl = true;
			//smtpClient.UseDefaultCredentials = false;
			//smtpClient.Credentials = new NetworkCredential("rleeson24@gmail.com", "eqfhaxeijsimjfsa");
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			var mailMessage = new MailMessage( new MailAddress( "support@embracingthememories.com" ), new MailAddress( message.Recipients, message.DisplayName ) );
			mailMessage.Subject = message.Subject;
			mailMessage.Body = RenderEmailViewToString( viewName, message.Body );
			mailMessage.IsBodyHtml = true;
			smtpClient.SendCompleted += ( s, e ) =>
			{
				smtpClient.Dispose();
				mailMessage.Dispose();
			};
			smtpClient.Send( mailMessage );
		}

		public static async Task SendAsync( EmailMessage message )
		{
			await sendViaSmtp( message );
		}

		public static async Task SendAsync( String subject, String body, String emailAddress, String viewName )
		{
			var message = new EmailMessage();
			message.DisplayName = emailAddress;
			message.Recipients = emailAddress;
			message.Subject = subject;
			message.Body = RenderEmailViewToString( viewName, body );
			await SendAsync( message );
		}

		// Use NuGet to install SendGrid (Basic C# client lib) 
		private static async Task sendViaSmtp( EmailMessage message )
		{
			var smtpClient = new SmtpClient( "smtpout.secureserver.net", 3535 );
			smtpClient.EnableSsl = false;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential( "support@embracingthememories.com", "j5lzAaeNz4XWnNEYptLU" );
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			var mailMessage = new MailMessage();
			mailMessage.From = new MailAddress( message.Sender );
			mailMessage.To.Add( "support@embracingthememories.com," + message.Recipients );
			mailMessage.Subject = message.Subject;
			mailMessage.Body = message.Body;
			mailMessage.IsBodyHtml = true;
			if ( message.Attachment != null )
			{
				var ms = new MemoryStream( message.Attachment.Content, 0, message.Attachment.Content.Length );
				var contentType = new System.Net.Mime.ContentType();
				contentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
				contentType.Name = message.Attachment.Name;
				var imageAttachment = new System.Net.Mail.Attachment( ms, contentType );
				mailMessage.Attachments.Add( imageAttachment );
			}
			smtpClient.SendCompleted += ( s, e ) =>
			{
				smtpClient.Dispose();
				mailMessage.Dispose();
			};
			await smtpClient.SendMailAsync( mailMessage );
		}

		public class EmailMessage
		{
			public EmailMessage()
			{
				Sender = "support@embracingthememories.com";
			}

			public String Sender { get; set; }
			/// <summary>
			/// Recipient email addresses separated by commas
			/// </summary>
			public String Recipients { get; set; }
			public String Subject { get; set; }
			public String Body { get; set; }
			public EmailService.Attachment Attachment { get; set; }
			public string DisplayName { get; internal set; }
		}

		public class Attachment
		{
			public Byte[] Content { get; set; }
			public String Name { get; set; }
		}
	}
}