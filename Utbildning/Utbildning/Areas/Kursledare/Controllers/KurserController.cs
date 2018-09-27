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
            return View(db.Courses.ToList().Where(m => m.Email == User.Identity.Name));
        }

        // GET: Skapa Kurs
        [Authorize(Roles = "Kursledare")]
        public ActionResult Skapa()
        {
            return View();
        }

        // POST:  Admin/SkapaKurs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Skapa([Bind(Include = "Id,Name,Length,Host,Email,Subtitle,Bold,Text,Image,Address,City,Price")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.Email = User.Identity.Name;
                course.Host = db.Users.ToList().Where(m => m.Email == User.Identity.Name).First().FullName;
                db.Courses.Add(course);

                db.SaveChanges();
                return Redirect("~/Kursledare/Kurser");
            }

            return View(course);
        }

        [Authorize(Roles = "Kursledare")]
        public ActionResult Kurs(string param1, string param2, string param3, string param4)
        {
            param1 = param1 != null ? param1.ToLower() : param1;
            param2 = param2 != null ? param2.ToLower() : param2;
            param3 = param3 != null ? param3.ToLower() : param3;
            param4 = param4 != null ? param4.ToLower() : param4;

            if (param1 == "kurstillfällen" && param2.HasIds()) //Kursledare/Kurser/Kurs/Kurstillfällen/{Id}
            {
                if (param2.GetIds(out List<int> Ids))
                {
                    int Id = Ids.First();
                    if (param2 == null) { return RedirectToAction("Index", "Kurser"); }
                    if (db.Courses.ToList().Where(x => x.Id == Id).First().Email == User.Identity.Name)
                    {
                        ViewBag.CourseName = db.Courses.Find(Id).Name;
                        var courseOccasions = db.CourseOccasions.Include(c => c.Course);
                        var COList = courseOccasions.ToList().Where(m => m.Course.Email == User.Identity.Name && m.Course.Id == Id);

                        return View("Kurstillfällen/Kurstillfällen", COList);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            else if (param1.HasIds())
            {
                param1.GetIds(out List<int> ids);
                int id = ids.First();
                Course course = db.Courses.Find(id);
                if (course == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CId = id;
                return View(course);
            }

            else if (param1 == "redigera" && param2.HasIds())
            {
                param2.GetIds(out List<int> ids);
                int id = ids.First();

                Course course = db.Courses.Find(id);
                if (course.Email == User.Identity.Name)
                {
                    if (course == null)
                    {
                        return HttpNotFound();
                    }
                    return View("Redigera", course);
                }
                return Redirect("~/Kursledare/Kurser");
            }

            else if (param1 == "radera" && param2.HasIds())
            {

                param2.GetIds(out List<int> ids);
                int id = ids.First();
                ViewBag.CId = id;
                Course course = db.Courses.Find(id);

                if (course.Email == User.Identity.Name)
                {
                    if (course == null)
                    {
                        return HttpNotFound();
                    }
                    return View("Radera", course);
                }
                return Redirect("~/Kursledare/Kurser");
            }

            else if (param1 == "punktlista" && param2.HasIds()) //Kursledare/Kurser/Kurs/Kurstillfälle/1
            {
                param2.GetIds(out List<int> Ids);
                int Id = Ids.First();
                BulletPoints bulletpoints = db.BulletPoints.Find(Id);
               /* if (bulletpoints == null)
                {
                    return HttpNotFound();
                }*/
             
                return View("Punktlista/Punktlista", bulletpoints);
            }


            else if (param1 == "Kurstillfälle" && param2.HasIds()) //Kursledare/Kurser/Kurs/Kurstillfälle/1
            {
                param2.GetIds(out List<int> Ids);
                int Id = Ids.First();
                CourseOccasion courseOccasion = db.CourseOccasions.Find(Id);
                if (courseOccasion == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", courseOccasion.CourseId);
                return View("Kurstillfällen/Kurstillfälle", courseOccasion);
            }

            else if (param1 == "kurstillfälle" && param2 == "bokningar" && param3.HasIds()) //Kursledare/Kurser/Kurs/Kurstillfälle/Bokningar/{Id}
            {
                if (param3.GetIds(out List<int> Ids))
                {
                    int id = Ids.First();
                    var DbCourseOc = db.CourseOccasions.ToList();
                    var DbCourse = db.Courses.ToList();

                    var COCourses = DbCourseOc.Where(m => m.Id == id).ToList();
                    if (COCourses.Count() < 1)
                    {
                        return RedirectToAction("", "Kurser");
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

            else if (param1 == "kurstillfällen" && param2 == "skapa" && param3.HasIds()) //Kursledare/Kurser/Kurs/Kurstillfällen/Skapa/{kurs-id}
            {
                param3.GetIds(out List<int> Ids);
                int Id = Ids.First();
                ViewBag.SpecificCourseId = Id;
                ViewBag.CourseName = db.Courses.ToList().Where(x => x.Id == Id).First().Name;
                ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");
                return View("Kurstillfällen/Skapa");
            }

            else if (param1 == "kurstillfälle" && param2 == "radera" && param3.HasIds())
            {
                //TODO: Skicka mail till alla deltagare som fått sin kurs borttagen?
                param3.GetIds(out List<int> Ids);
                int Id = Ids.First();
                CourseOccasion co = DBHandler.GetCourseOccasion(Id);
                Course course = co.GetCourse();
                ViewBag.Course = course;
                ViewBag.COId = co.Id;

                if (User.ValidUser(course))
                    return View("Kurstillfällen/Radera", co);
                return Redirect("~/Kursledare/Kurser");
            }

            else if (param1 == "kurstillfälle" && param2 == "redigera" && param3.HasIds())
            {
                param3.GetIds(out List<int> Ids);
                int Id = Ids.First();
                CourseOccasion co = DBHandler.GetCourseOccasion(Id);                
                //db.CourseOccasions.Find(Id);

                if (User.ValidUser(co))
                {
                    ViewBag.COId = co.Id;
                    ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", co.CourseId);
                    return View("Kurstillfällen/Redigera", co);
                }
                return Redirect("~/Kursledare/Kurser");
            }

            //if you've come this far something has gone wrong.
            return Redirect("~/Kursledare/Kurser");
        }
        [HttpPost]
        public ActionResult Kurs([Bind(Include = "Id,CourseId,StartDate,AltHost,AltAddress,AltMail,AltProfilePicture,MinPeople,MaxPeople")] CourseOccasion courseOccasion, [Bind(Include = "Id,Name,Length,Host,Email,Subtitle,Bold,Text,Image,Address,City,Price")] Course course, [Bind(Include = "Id,Firstname,Lastname,Email,CourseOccasionId,PhoneNumber,Company,BillingAddress,PostalCode,City,Bookings,Message,DiscountCode,BookingDate")] Booking booking, string param1, string param2)
        {
            int Id = 0;
            switch (param1)
            {
                case "NyttKT":
                    if (int.TryParse(param2, out Id))
                    {
                        courseOccasion.CourseId = Id;
                        if (User.ValidUser(courseOccasion))
                        {
                            db.CourseOccasions.Add(courseOccasion);
                            db.SaveChanges();
                        }
                    }
                    return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfällen");

                case "RaderaKT":
                    if (int.TryParse(param2, out Id))
                    {
                        CourseOccasion co = db.CourseOccasions.Find(Id);
                        db.CourseOccasions.Remove(co);
                        db.SaveChanges();
                        return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfällen/" + co.CourseId);
                    }
                    return Redirect("~/Kursledre/Kurser");

                case "RedigeraKT":
                    if (User.ValidUser(courseOccasion))
                    {
                        db.Entry(courseOccasion).State = EntityState.Modified;
                        db.SaveChanges();
                        return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfällen/" + courseOccasion.CourseId);
                    }
                    return Redirect("~/Kursledare/Kurser");

                case "RedigeraKurs":

                    db.Entry(course).State = EntityState.Modified;
                    course.Email = User.Identity.Name;
                    course.Host = User.GetFullName();
                    db.SaveChanges();
                    return Redirect("~/Kursledare/Kurser");


                case "RaderaKurs":

                    Course CourseToBeDeleted = db.Courses.Find(param2);

                    db.Courses.Remove(CourseToBeDeleted);
                    db.SaveChanges();

                    return Redirect("~/Kursledare/Kurser");






                default: //If you reached this something went wrong
                    return Redirect("~/Kursledare/Kurs/Kurser");
            }
        }
    }
}