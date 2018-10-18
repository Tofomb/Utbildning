using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Utbildning.Classes;
using Utbildning.Models;

namespace Utbildning.Areas.Kursledare.Controllers
{
    public class ProfilController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ProfilController()
        {
        }

        public ProfilController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


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

                ViewBag.Image = User.Identity.GetUserId() + user.ProfilePicture;
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
        public ActionResult Bild(string w)
        {
            if (w == "wff")
            {
                ViewBag.Warning = "Fel filformat. Endast .jpg och .png stöds.";
            }
            return View();
        }

        [Authorize(Roles = "Kursledare")]
        [HttpPost]
        public ActionResult Bild(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {

                    ApplicationUser user;
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        user = db.Users.Find(User.Identity.GetUserId());
                    }
                    char unique;
                    if (user.ProfilePicture == null)
                        unique = '0';
                    else if (user.ProfilePicture[0] == '1')
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
                else
                {
                    return Redirect("~/Kursledare/Profil/Bild?w=wff");
                }
            }

            return Redirect("~/Kursledare/Profil");
        }

        public ActionResult Lösenord()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Lösenord(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return Redirect("~/Kursledare/Profil");
            }
            return View(model);
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