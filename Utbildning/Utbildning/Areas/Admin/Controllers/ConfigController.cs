using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utbildning.Models;

namespace Utbildning.Areas.Admin.Controllers
{
    public class ConfigController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Config
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.SiteConfigurations.ToList());
        }

       
        [Authorize(Roles = "Admin")]
        public ActionResult Redigera(int id)
        {
            var sc = db.SiteConfigurations.Where(m => m.Id == id).First();
            return View(sc);
        }

        [HttpPost]
        public ActionResult Redigera([Bind(Include = "Id,Value")]SiteConfiguration sc)
        {

            db = new ApplicationDbContext();
            sc.Property = db.SiteConfigurations.Where(m => m.Id == sc.Id).First().Property;
            db = new ApplicationDbContext();
            db.Entry(sc).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect("~/admin/config/");
        }
    }
}