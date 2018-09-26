using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Utbildning.Areas.Admin.Controllers
{
    public class HemController : Controller
    {
        // GET: Admin/Hem
        [Authorize(Roles = "Admin, Kursledare")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
                if (User.IsInRole("Kursledare"))
                    return View("vill du admin eller KL?");
                else
                    return View();
            else if (User.IsInRole("Kursledare"))
                return Redirect("Kursledare");
            else return RedirectToAction("Index", "Home");
        }
    }
}