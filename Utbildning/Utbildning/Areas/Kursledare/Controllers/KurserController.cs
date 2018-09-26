using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Utbildning.Models;
using Utbildning.Classes;
using System.Net;

namespace Utbildning.Areas.Kursledare.Controllers
{
    public class KurserController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Kursledare/Kurser
        [Authorize(Roles = "Kursledare")]
        public ActionResult Index(string param1)
        {
            /*
            if (param1.GetId(out int Id))
            {
                ViewBag.test = Id;
            }
            else
            {
                return RedirectToAction("", "Kurser");
            }
            */
            return View(db.Courses.ToList().Where(m => m.Email == User.Identity.Name));
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Kurs(string param1, string param2, string param3)
        {
            if (param1 == "Kurstillfällen") //Kurstillfällen
            {
                int Id = 0;
                if (int.TryParse(param2, out Id))
                {
                    if (param2 == null) { return RedirectToAction("Index", "Kurser"); }
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
                        return View("Kurstillfällen/Kurstillfällen", COList);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            else if (param1 == "Kurstillfälle" && param2 == "Bokningar" && param3 != null)
            {
                if (param3.GetIds(out List<int> Ids))
                {
                    int id = Ids.First();
                    var DbCourseOc = db.CourseOccasions.ToList();
                    var DbCourse = db.Courses.ToList();

                    var COCourses = DbCourseOc.Where(m => m.Id == id).ToList();
                    if (COCourses.Count() < 1)
                    {
                        return RedirectToAction("Kurstillfälle");
                    }
                    int COCourseId = COCourses.First().CourseId;
                    string userEmail = DbCourse.Where(m => m.Id == COCourseId).First().Email;
                    if (User.Identity.Name == userEmail)
                    {
                        ViewBag.CourseOccasionDate = DbCourseOc.Where(m => m.Id == id).First().StartDate;
                        ViewBag.CourseName = DbCourse.Where(m => m.Id == COCourseId).First().Name;

                        var bookings = db.Bookings.Include(b => b.CourseOccasion).Where(m => m.CourseOccasion.Id == id);

                        return View("Kurstillfällen/Bokningar/Bokningar", bookings.ToList());
                    }
                }
                return View("Kurstillfällen/Bokningar/Bokningar");
            }

            else if (param1 == "Kurstillfälle")
            {
                if (int.TryParse(param2, out int Id))
                {
                    CourseOccasion courseOccasion = db.CourseOccasions.Find(Id);
                    if (courseOccasion == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", courseOccasion.CourseId);
                    return View("Kurstillfälle", courseOccasion);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            //if you've come this far something has gone wrong.
            return RedirectToAction("", "Kurser");
        }
    }
}