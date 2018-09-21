using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utbildning.Models;

namespace Utbildning.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            string Name = User.Identity.Name.Split('@').First();
            Name = Regex.Replace(Name, "[^A-Za-zå-öÅ-Ö]", " ");
            Name = Name.Trim();

            char[] NameChar = Name.ToCharArray();

            for (int i = 1; i < NameChar.Length; i++)
            {
                if (NameChar[i - 1] == ' ')
                {
                    NameChar[i] = (char)(NameChar[i] - 32);
                }
            }

            NameChar[0] = char.ToUpper(Name[0]);
            ViewBag.Name = new string(NameChar);

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult NyKursledare()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> NyKursledare(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: Add confirmation email

                string pw = PassGen();

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };                
                var result = await UserManager.CreateAsync(user, pw);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Kursledare");
                    return RedirectToAction("Admin", "Admin", new { password = pw });
                }
            }
            return View(model);
        }

        private string PassGen()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[8];
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Kursledare()
        {
            string Name = User.Identity.Name.Split('@').First();
            Name = Regex.Replace(Name, "[^A-Za-zå-öÅ-Ö]", " ");
            Name = Name.Trim();

            char[] NameChar = Name.ToCharArray();

            for (int i = 1; i < NameChar.Length; i++)
            {
                if (NameChar[i - 1] == ' ')
                {
                    NameChar[i] = (char)(NameChar[i] - 32);
                }
            }

            NameChar[0] = char.ToUpper(Name[0]);
            ViewBag.Name = new string(NameChar);

            return View();
        }
    }
}