using System.Web.Mvc;

namespace EmbracingMemories.Areas.Photos
{
    public class PhotoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Photos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Photos_default",
                "api/Photos/{action}/{id}",
                new { controller = "Photos", action = "GetPhoto", id = UrlParameter.Optional },
                new[] { "EmbracingMemories.Areas.Photos.Controllers" }
            );
        }
    }
}