using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Utbildning.Classes;
using Utbildning.Models;

namespace Utbildning.Areas.Kursledare.Controllers
{
    public class ProfilController : Controller
    {
        // GET: Kursledare/Profil
        [NoCache]
        [Authorize(Roles = "Kursledare")]
        public ActionResult Index()
        {
            string Id = User.Identity.GetUserId();
            ViewBag.FullName = User.GetFullName();
            using (var db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Find(Id);
                return View(user);
            }
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Namn()
        {
            ViewBag.FullName = User.GetFullName();
            return View();
        }

        [Authorize(Roles = "Kursledare")]
        [HttpPost]
        public ActionResult Namn(ChangeFullNameViewModel model)
        {
            User.SetFullName(model.FullName);
            return Redirect("~/Kursledare/Profil");
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Bild()
        {
            return View();
        }

        [Authorize(Roles = "Kursledare")]
        [HttpPost]
        public ActionResult Bild(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string imageName = User.Identity.GetUserId() + Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/profile"), imageName);

                var image = new WebImage(file.InputStream);

                int width = image.Width;
                int height = image.Height;

                int Diff = (width - height) / 2;
                if (width > height)
                    image.Crop(0, Diff, 0, Diff);
                else if (height > width)
                    image.Crop(-Diff, 0, -Diff, 0);

                if (width > height)
                {
                    var leftrightcrop = (width - height) / 2;
                    var topbottomcrop = (height - width) / 2;
                    image.Crop(0, leftrightcrop, 0, leftrightcrop);
                }

                image.Save(path);

                //file.SaveAs(path);
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Users.Find(User.Identity.GetUserId()).ProfilePicture = Path.GetExtension(file.FileName);
                    db.SaveChanges();
                }
            }

            return Redirect("~/Kursledare/Profil");
        }
    }

    public class NoCache : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
}