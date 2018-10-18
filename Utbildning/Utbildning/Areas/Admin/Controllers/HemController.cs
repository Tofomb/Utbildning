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
        public ActionResult Index(string r)
        {
            if (User.IsInRole("Admin") && User.IsInRole("Kursledare"))
            {
                if (r == "admin")
                    return View();
                else if (r == "kl")
                    return Redirect("~/Kursledare");
                else
                    return View("AdminEllerKl");
            }
            if (User.IsInRole("Admin"))
                return View();

            else if (User.IsInRole("Kursledare"))
                return Redirect("~/Kursledare");
            else return RedirectToAction("Index", "Kurser");
        }
    }
}