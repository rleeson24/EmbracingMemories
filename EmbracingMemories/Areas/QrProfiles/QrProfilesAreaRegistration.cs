using System.Web.Mvc;

namespace EmbracingMemories.Areas.QRCodes
{
    public class QrProfileAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QrProfiles";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Qr_List",
                "api/QrProfiles",
                new { controller = "QrProfiles", action = "GetQrProfiles" }
            );
            context.MapRoute(
                "Qr_View",
                "api/QrProfiles/{id}",
                new { controller = "QrProfiles", action = "GetQrProfile", Id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Qr_Update",
                "api/QrProfiles/{id}",
                new { controller = "QrProfiles", action = "PutQrProfile" }
            );
            context.MapRoute(
                 "Qr_Create",
                 "api/QrProfiles/Create",
                 new { controller = "QrProfiles", action = "PostQrProfile", id = UrlParameter.Optional }
             );
            //context.MapRoute(
            //     "Qr_StartRegistration",
            //     "Profile/StartRegistration",
            //     new { controller = "QrProfiles", action = "StartRegistration", id = UrlParameter.Optional }
            // );
            context.MapRoute(
                "QRCodes_default",
                "api/QrProfiles/{action}/{id}",
                new { controller = "QrProfiles", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "ProfileViews",
                "ProfileViews/{action}",
                new { controller = "ProfileViews", action = "Index" },
                new[] { "EmbracingMemories.Areas.QrProfiles.Controllers" }
            );
        }
    }
}