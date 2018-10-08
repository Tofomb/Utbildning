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
                ApplicationUser user;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    user = db.Users.Find(User.Identity.GetUserId());
                }
                char unique;
                if (user.ProfilePicture[0] == '1')
                    unique = '0';
                else
                    unique = '1';

                string imageName = User.Identity.GetUserId() + unique + Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/profile"), imageName);

                var image = new WebImage(file.InputStream);

                int width = image.Width;
                int height = image.Height;

                int Diff = (width - height) / 2;
                if (width > height)
                    image.Crop(0, Diff, 0, Diff);
                else if (height > width)
                    image.Crop(-Diff, 0, -Diff, 0);

                if (image.Width > 250)
                    image.Resize(250, 250);

                image.Save(path);
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Users.Find(User.Identity.GetUserId()).ProfilePicture = unique + Path.GetExtension(file.FileName);
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