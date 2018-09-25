using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Utbildning.Models;
using Utbildning.Classes;

namespace Utbildning.Areas.Kursledare.Controllers
{
    public class KurserController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Kursledare/Kurser
        [Authorize(Roles = "Kursledare")]
        public ActionResult Index()
        {
            return View(db.Courses.ToList().Where(m => m.Email == User.Identity.Name));
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Kurstillfällen(int? Id, string kurs)
        {            
            
            if (kurs == "Kurs")
            {
                if (Id == null) { return RedirectToAction("Index", "Kurser"); }
                if (db.Courses.ToList().Where(x => x.Id == Id).First().Email == User.Identity.Name)
                {
                    var courseOccasions = db.CourseOccasions.Include(c => c.Course);
                    var COList = courseOccasions.ToList().Where(m => m.Course.Email == User.Identity.Name && m.Course.Id == Id);
                    ViewBag.CourseName = COList.First().Course.Name;
                    List<int> AvailableBookings = new List<int>();
                    foreach (var item in COList)
                    {
                        AvailableBookings.Add(item.GetAvailableBookings());
                    }
                    return View("Kurstillfällen", COList);
                }
            }
            return RedirectToAction("test", "Kurser");
        }
    }
}