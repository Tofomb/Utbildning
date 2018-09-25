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
                "Kursledare/{controller}/{kurs}/{action}/{id}",
                new { controller = "Hem", action = "Index", kurs = UrlParameter.Optional, id = UrlParameter.Optional }
            );
        }
    }
}