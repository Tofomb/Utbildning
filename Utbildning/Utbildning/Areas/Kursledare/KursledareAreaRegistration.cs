using System.Web.Mvc;

namespace Utbildning.Areas.Kursledare
{
    public class KursledareAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Kursledare";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Kursledare_default",
                "Kursledare/{controller}/{action}/{param1}/{param2}/{param3}",
                new { controller = "Hem", action = "Index", param1 = UrlParameter.Optional, param2 = UrlParameter.Optional, param3 = UrlParameter.Optional }
            );
        }
    }
}