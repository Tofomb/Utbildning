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
using System.Net;

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
            return View(db.Courses.ToList().Where(m => m.Email == User.Identity.Name));
        }


        // GET: CourseOccasions
        [Authorize(Roles = "Kursledare")]
        public ActionResult Kurstillfälle(int? Id)
        {
            if (Id == null) { return RedirectToAction("Kursvy", "Admin"); }
            List<Course> courses = db.Courses.ToList();
            ViewBag.CourseId = Id;
            ViewBag.CourseName = courses.Where(m => m.Id == Id).First().Name;
            if (courses.Where(x => x.Id == Id).First().Email == User.Identity.Name)
            {
                var courseOccasions = db.CourseOccasions.Include(c => c.Course);
                return View(courseOccasions.ToList().Where(m => m.Course.Email == User.Identity.Name && m.Course.Id == Id));
            }
            return RedirectToAction("Kursvy", "Admin");
        }

        // GET: Admin/SkapaKurs
        [Authorize(Roles = "Kursledare")]
        public ActionResult SkapaKurs()
        {
            return View();
        }

        // POST:  Admin/SkapaKurs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SkapaKurs([Bind(Include = "Id,Name,Length,Host,Email,Subtitle,Bold,Text,Image,Address,City,Price")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.Email = User.Identity.Name;
                course.Host = db.Users.ToList().Where(m => m.Email == User.Identity.Name).First().FullName;
                db.Courses.Add(course);

                db.SaveChanges();
                return RedirectToAction("Kursvy");
            }

            return View(course);
        }

        // GET: Admin/RedigeraKurs/5
        [Authorize(Roles = "Kursledare")]
        public ActionResult RedigeraKurs(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Admin/RedigeraKurs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RedigeraKurs([Bind(Include = "Id,Name,Length,Host,Email,Subtitle,Bold,Text,Image,Address,City,Price")] Course course)
        {
            if (ModelState.IsValid)
            {

                course.Email = User.Identity.Name;
                course.Host = db.Users.ToList().Where(m => m.Email == User.Identity.Name).First().FullName;

                db.Entry(course).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("kursvy");
            }
            return View(course);
        }

        // GET: Admin/SkapaKurstillfälle
        [Authorize(Roles = "Kursledare")]
        public ActionResult SkapaKurstillfälle(int? id)
        {
            ViewBag.SpecificCourseId = id;
            ViewBag.CourseName = db.Courses.ToList().Where(x => x.Id == id).First().Name;
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");
            return View();
        }

        // POST: Admin/SkapaKurstillfälle
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SkapaKurstillfälle([Bind(Include = "Id,CourseId,StartDate,AltHost,AltAddress,AltMail,AltProfilePicture,MinPeople,MaxPeople")] CourseOccasion courseOccasion, int id)
        {
            if (ModelState.IsValid)
            {

                courseOccasion.CourseId = id;
                db.CourseOccasions.Add(courseOccasion);
                db.SaveChanges();
                return RedirectToAction("Kurstillfälle");

            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", courseOccasion.CourseId);
            return View(courseOccasion);
        }



        // GET: Bookings
        [Authorize(Roles = "Kursledare")]
        public ActionResult Deltagare(int? id)
        {
            if (id.HasValue)
            {
                var DbCourseOc = db.CourseOccasions.ToList();
                var DbCourse = db.Courses.ToList();

                var COCourses = DbCourseOc.Where(m => m.Id == id).ToList();
                if (COCourses.Count() < 1)
                {
                    return RedirectToAction("Kurstillfälle");
                }
                int COCourseId =  COCourses.First().CourseId;
                string userEmail = DbCourse.Where(m => m.Id == COCourseId).First().Email;
                if (User.Identity.Name == userEmail)
                {
                    ViewBag.CourseOccasionDate = DbCourseOc.Where(m => m.Id == id).First().StartDate;
                    ViewBag.CourseName = DbCourse.Where(m => m.Id == COCourseId).First().Name;

                    var bookings = db.Bookings.Include(b => b.CourseOccasion).Where(m => m.CourseOccasion.Id == id);

                    return View(bookings.ToList());
                }
            }
            return RedirectToAction("Kurstillfälle");
        }


    }
}
