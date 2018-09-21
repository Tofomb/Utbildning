using Microsoft.AspNet.Identity;
using System.Data.Entity;
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
        private ApplicationDbContext db = new ApplicationDbContext();
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

            return new string(stringChars);
        }

        // GET: Admin/Startsida
        public ActionResult Startsida()
        {
            if (db.FrontpageInfoes.Count() > 0)
            {
                FrontpageInfo Info = db.FrontpageInfoes.ToList().Last();
                ViewBag.Title = Info.Title;
                ViewBag.Bold = Info.Bold;
                ViewBag.Text = Info.Text;
            }            
            return View();
        }

        // POST: FrontpageInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Startsida([Bind(Include = "Id,Title,Bold,Text")] FrontpageInfo frontpageInfo)
        {
            if (ModelState.IsValid)
            {
                db.FrontpageInfoes.Add(frontpageInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(frontpageInfo);
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Kursledare()
        {
            string PP = db.Users.ToList().Where(x => x.Id == User.Identity.GetUserId()).First().ProfilePicture;

            ViewBag.PP = PP;

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
        [Authorize(Roles = "Kursledare")]
        public ActionResult Kursvy()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return View(db.Courses.ToList().Where(m=>m.Email == User.Identity.Name));
            
        }


        // GET: CourseOccasions
        [Authorize(Roles = "Kursledare")]
        public ActionResult Kurstillfälle(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var courseOccasions = db.CourseOccasions.Include(c =>c.Course);
            return View(courseOccasions.ToList().Where(m => m.Course.Email == User.Identity.Name && m.Course.Id == id));
        }
    }
}