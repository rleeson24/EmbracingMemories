using System.Web.Mvc;

namespace EmbracingMemories.Areas.Archive
{
    public class ArchiveAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Archive";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Archive_default",
                "api/Archive/{action}/{id}",
                new { controller = "Archive", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}