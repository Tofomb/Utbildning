using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utbildning.Classes;
using Utbildning.Models;

namespace Utbildning.Areas.Kursledare.Controllers
{
    public class HemController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Kursledare/Hem
        [Authorize(Roles = "Kursledare")]
        public ActionResult Index()
        {
            if (User.GetFullName() == User.Identity.Name)
            {
                ViewBag.NameReset = true;
            }
            ViewBag.Name = User.GetFullName();
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Kursledare")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChangeFullNameViewModel model)
        {
            User.SetFullName(model.FullName);
            return View();
        }
    }
}