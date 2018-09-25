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
                //TODO: Add confirmation email

                string pw = UserHandler.GeneratePasswordString();

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.Email };
                var result = await UserManager.CreateAsync(user, pw);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Kursledare");
                    return RedirectToAction("", new { result = "success", password = pw }); //TODO remove pw from url
                } //TODO Send mail to user
            }
            return View(model);
        }
    }
}