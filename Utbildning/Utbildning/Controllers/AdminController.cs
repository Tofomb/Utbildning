using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Utbildning.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "Admin, Kursledare")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Admin");
            }
            if (User.IsInRole("Kursledare"))
            {
                return RedirectToAction("Kursledare");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Kursledare()
        {
            return View();
        }
    }
}