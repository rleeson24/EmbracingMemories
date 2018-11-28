using EmbracingMemories.Areas.Contact.Models;
using EmbracingMemories.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmbracingMemories.Areas.Contact.Controllers
{
	[RoutePrefix( "api/ContactUs" )]
	public class ContactUsController : ApiController
	{
		public ContactUsController()
		{

		}


		[HttpPost]
		[AllowAnonymous]
		public async Task<IHttpActionResult> PostContactUs( ContactForm form )
		{
			if ( !ModelState.IsValid )
			{
				return BadRequest( ModelState );
			}
			try
			{
				await EmailService.SendAsync( "Embracing Memories - " + form.EnquiryType.ToLower(), String.Format( @"
                                    <p>From: {0}<{1}></p>
                                    <p>Phone: {2}</p>
									<br />
                                    <p>{3}</p>
									<br />
                                    {4}"
						, form.Name
						, form.EmailAddress
						, form.Phone
						, form.Message
						, EmailService.Signature ), form.EmailAddress, "Generic" );
				return Ok();
			}
			catch ( InvalidDataException ex )
			{
				return BadRequest( ex.Message );
			}
		}
	}
}
