using System.Web.Mvc;

namespace EmbracingMemories.Areas.Contact
{
    public class ContactAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Contact";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //     "Contact_Us",
            //     "api/ContactUs/{form}",
            //     new { controller = "ContactUs", action = "PostContactUs" }
            // );
            //context.MapRoute(
            //    "Contact_default",
            //    "api/ContactUs/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional },
            //    new[] { "EmbracingMemories.Areas.Contact.Controllers" }
            //);
        }
    }
}