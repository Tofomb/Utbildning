using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utbildning.Models;

namespace Utbildning.Areas.Admin.Controllers
{
    public class HemController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HemController()
        {
        }

        public HemController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
                return Redirect("~/Admin");
            }
            return View(model);
        }
    }
}