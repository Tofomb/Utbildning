using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utbildning.Classes;
using Utbildning.Models;

namespace Utbildning.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KursledareController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public KursledareController()
        {
        }

        public KursledareController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Admin/
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string result)
        {
            ViewBag.Result = result;

            var Users = db.Users.ToList();

            IdentityRole KLRole = db.Roles.ToList().Where(x => x.Name == "Kursledare").First();

            var Kursledare = from u in Users
                             where u.Roles.Any(x => x.RoleId == KLRole.Id)
                             orderby u.UserName ascending
                             select u;

            ViewBag.Users = Kursledare;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Ny()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Ny(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string pw = UserHandler.GeneratePasswordString();
                string MailText = $"Ett konto har skapats åt dig på Castra Utbldning!<br/><a href='{URLHandler.GetBaseUrl(Request.Url)}'>Klicka här för att komma till inloggningssidan!</a>.\n Ditt nuvarande lösenord är: {pw}. Logga in och gå till Profil-sidan för att byta det.";

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.Email };
                var result = await UserManager.CreateAsync(user, pw);
                if (result.Succeeded)
                {
                    // MailHandler.SendTester("", user.Email, "Nytt Konto På Castra Utbildning", MailText, "");
                    // TODO: Add confirmation email, un-comment 'Send' after Castra mail has been implemented
                    MailHandler.Send(user.Email, "Nytt Konto På Castra Utbildning", MailText);

                    await UserManager.AddToRoleAsync(user.Id, "Kursledare");
                    return RedirectToAction("", new { result = "success" });
                }
            }
            return View(model);
        }
        public ActionResult Radera(string Id)
        {
            if (Id == null)
                return Redirect("~/Admin/Kursledare");

            var Users = db.Users.ToList();
            IdentityRole KLRole = db.Roles.ToList().Where(x => x.Name == "Kursledare").First();
            ApplicationUser user = (from u in Users
                                    where u.Id == Id.ToString() && u.Roles.Any(x => x.RoleId == KLRole.Id)
                                    select u).First();
            if (user != null)
                return View(user);
            return Redirect("~/Admin/Kursledare");
        }

        [HttpPost]
        public ActionResult Radera([Bind(Include = "Id")] ApplicationUser userId)
        {
            if (userId == null)
                return Redirect("~/Admin/Kursledare");

            ApplicationUser user = db.Users.Where(x => x.Id == userId.Id).First();

            List<Course> courses = db.Courses.Where(x => x.Email == user.Email).ToList();

            foreach( Course c in courses)
            {
                db.Courses.Remove(c);
            }
            db.Users.Remove(user);
            db.SaveChanges();

            return Redirect("~/Admin/Kursledare");
        }
    }
}