using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Utbildning.Models;
using Utbildning.Classes;
using System.Net;
using Microsoft.AspNet.Identity;

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
                    if (new Course().GetComparison(course, out string[] Comp))
                    {
                        db.Logs.Add(new Log() { User = User.Identity.Name, Table = "Courses", Action = "Add", Before = "[NULL]", After = Comp[1], Time = DateTime.Now });
                    }

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
                        ViewBag.CourseId = Id;
                        var courseOccasions = db.CourseOccasions.Include(c => c.Course);
                        var COList = courseOccasions.ToList().Where(m => m.Course.Email == User.Identity.Name && m.Course.Id == Id);

                        return View("Kurstillfällen/Kurstillfällen", COList);
                    }
                }
                else
                {
                    return Redirect("~/Kursledare/Kurser");
                }
            }

            // Kursöversikt / Course overview
            else if (param1.HasIds())
            {
                param1.GetIds(out List<int> ids);
                int id = ids.First();
                Course course = db.Courses.Find(id);
                if (course == null)
                {
                    return Redirect("~/Kursledare/Kurser"); ;
                }
                ViewBag.CId = id;
                ViewBag.CourseName = course.Name;

                List<BulletPoints> bulletpoints = db.BulletPoints.Where(x => x.CourseId == id).ToList();
                ViewBag.BPs = bulletpoints;
                if (User.ValidUser(course))
                {
                    return View(course);
                }
                return Redirect("~/Kursledare/Kurser");
            }

            // Redigera Kurs / Edit Course
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
                    ViewBag.CourseId = course.Id;
                    return View("Redigera", course);
                }
                return Redirect("~/Kursledare/Kurser");
            }

            // Radera Kurs / Delete Course
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

            // Se / Skapa Punktlista / See / Create Bulletpoints
            else if (param1 == "punktlista" && param2.HasIds())
            {
                param2.GetIds(out List<int> Ids);
                int Id = Ids.First();
                if (User.ValidUser(DBHandler.GetCourse(Id)))
                {
                    List<BulletPoints> bulletpoints = db.BulletPoints.Where(x => x.CourseId == Id).ToList();

                    ViewBag.BPs = bulletpoints;
                    ViewBag.CourseId = Id;

                    return View("Punktlista/Punktlista");
                }
                return Redirect("~/Kursledare/Kurser");
            }
            else if (param1 == "punktlista" && param2 == "radera" && param3.HasIds())
            {

                param3.GetIds(out List<int> Ids);
                int Id = Ids.First();
                BulletPoints bp = db.BulletPoints.Find(Id);
                if (User.ValidUser(DBHandler.GetCourse(bp.CourseId)))
                {
                    db.BulletPoints.Remove(bp);
                    db.SaveChanges();
                    return Redirect("~/Kursledare/Kurser/Kurs/Punktlista/" + bp.CourseId);
                }
                return Redirect("~/Kursledare/Kurser");
            }



            
            else if (param1 == "kurstillfälle" && param2.HasIds()) //Kursledare/Kurser/Kurs/Kurstillfälle/1
            {
                param2.GetIds(out List<int> Ids);
                int Id = Ids.First();
                CourseOccasion courseOccasion = db.CourseOccasions.Find(Id);
                if (courseOccasion == null)
                {
                    return Redirect("~/Kursledare/Kurser");
                }

                ViewBag.CourseId = courseOccasion.CourseId;
                ViewBag.CODate = courseOccasion.StartDate.Format();
                ViewBag.CourseName = DBHandler.GetCourse(courseOccasion).Name;
                ViewBag.COId = Id;

                if (User.ValidUser(courseOccasion))
                {
                    return View("Kurstillfällen/Kurstillfälle", courseOccasion);
                }
                return Redirect("~/Kursledare/Kurser");
            }


            // Skapa Kurstillfälle / Create Courseoccasion
            else if (param1 == "kurstillfällen" && param2 == "skapa" && param3.HasIds()) //Kursledare/Kurser/Kurs/Kurstillfällen/Skapa/{kurs-id}
            {
                param3.GetIds(out List<int> Ids);
                int Id = Ids.First();
                ViewBag.SpecificCourseId = Id;
                ViewBag.CourseName = db.Courses.ToList().Where(x => x.Id == Id).First().Name;
                ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");
                if (User.ValidUser(db.Courses.Where(m => m.Id == Id).First()))
                {
                    return View("Kurstillfällen/Skapa");
                }
                return Redirect("~/Kursledare/Kurser");
            }

            else if (param1 == "kurstillfälle" && param2 == "radera" && param3.HasIds())
            {
                //TODO: Skicka mail till alla deltagare som fått sin kurs borttagen?
                param3.GetIds(out List<int> Ids);
                int Id = Ids.First();
                CourseOccasion co = DBHandler.GetCourseOccasion(Id);
                Course course = co.GetCourse();
                ViewBag.Course = course;
                ViewBag.CODate = co.StartDate.Format();
                ViewBag.COId = co.Id;

                if (User.ValidUser(course))
                {
                    return View("Kurstillfällen/Radera", co);
                }
                return Redirect("~/Kursledare/Kurser");
            }

            else if (param1 == "kurstillfälle" && param2 == "redigera" && param3.HasIds())
            {
                param3.GetIds(out List<int> Ids);
                int Id = Ids.First();
                CourseOccasion co = DBHandler.GetCourseOccasion(Id);


                if (User.ValidUser(co))
                {
                    ViewBag.COId = co.Id;
                    ViewBag.CourseId = co.GetCourse().Id;
                    ViewBag.CourseName = co.GetCourse().Name;
                    ViewBag.CODate = co.StartDate.Format();
                    return View("Kurstillfällen/Redigera", co);
                }
                return Redirect("~/Kursledare/Kurser");
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
                        ViewBag.CourseOccasionDate = DbCourseOc.Where(m => m.Id == id).First().StartDate.Format();
                        ViewBag.CourseName = DbCourse.Where(m => m.Id == COCourseId).First().Name;
                        ViewBag.COId = DbCourseOc.Where(m => m.Id == id).First().Id;
                        var bookings = db.Bookings.Include(b => b.CourseOccasion).Where(m => m.CourseOccasion.Id == id);


                        return View("Kurstillfällen/Bokningar/Bokningar", bookings.ToList());
                    }
                }
                return View("Kurstillfällen/Bokningar/Bokningar");
            }

            else if (param1 == "kurstillfälle" && param2 == "bokningar" && param3 == "skapa" && param4.HasIds())
            {

                if (param4.GetIds(out List<int> Ids))
                {
                    int Id = Ids.First();
                    CourseOccasion co = DBHandler.GetCourseOccasion(Id);
                    Course course = DBHandler.GetCourse(co.CourseId);
                    ViewBag.COId = Id;
                    ViewBag.CourseName = course.Name;
                    ViewBag.CODate = co.StartDate;
                    if (User.ValidUser(co))
                    {
                        return View("Kurstillfällen/Bokningar/Skapa");
                    }
                    return Redirect("~/Kursledare/Kurser");
                }
            }

            else if (param1 == "kurstillfälle" && param2 == "bokning" && param3 == "radera" && param4.HasIds())
            {
                if (param4.GetIds(out List<int> Ids))
                {
                    int Id = Ids.First();
                    Booking booking = db.Bookings.Where(m => m.Id == Id).First();
                    CourseOccasion co = DBHandler.GetCourseOccasion(booking);
                    Course course = DBHandler.GetCourse(co.CourseId);
                    ViewBag.BookingId = Id;
                    ViewBag.CourseName = course.Name;
                    ViewBag.CODate = co.StartDate.Format();
                    if (User.ValidUser(booking))
                    {
                        return View("Kurstillfällen/Bokningar/Radera", booking);
                    }
                    return Redirect("~/Kursledare/Kurser");
                }
            }
            else if (param1 == "kurstillfälle" && param2 == "bokning" && param3 == "redigera" && param4.HasIds())
            {
                param4.GetIds(out List<int> Ids);
                int Id = Ids.First();
                Booking booking = db.Bookings.Find(Id);

                if (User.ValidUser(booking))
                {
                    ViewBag.CourseOccasionId = new SelectList(db.CourseOccasions, "Id", "StartDate", booking.CourseOccasionId);
                    ViewBag.COId = DBHandler.GetCourseOccasion(booking).Id;
                    ViewBag.CourseName = DBHandler.GetCourse(DBHandler.GetCourseOccasion(Id)).Name;
                    ViewBag.CODate = DBHandler.GetCourseOccasion(Id).StartDate.Format();
                    return View("Kurstillfällen/Bokningar/Redigera", booking);
                }
                return Redirect("~/Kursledare/Kurser");
            }

            else if (param1 == "kurstillfälle" && param2 == "bokning" && param3.HasIds())
            {
                if (param3.GetIds(out List<int> Ids))
                {
                    int Id = Ids.First();
                    Booking booking = db.Bookings.Find(Id);
                    ViewBag.COstartDate = DBHandler.GetCourseOccasion(booking).StartDate.Format();
                    ViewBag.CourseName = DBHandler.GetCourse(DBHandler.GetCourseOccasion(booking)).Name;
                    if (booking == null)
                    {
                        return HttpNotFound();
                    }
                    if (User.ValidUser(booking))
                    {
                        return View("Kurstillfällen/Bokningar/bokning", booking);
                    }
                    return Redirect("~/Kursledare/Kurser");
                }               
            }
            else if (param1 == "kurstillfälle" && param2 == "faktura" && param3.HasIds())
            {
                if (param3.GetIds(out List<int>Ids))
                {
                    int Id = Ids.First();
                    CourseOccasion co = db.CourseOccasions.Find(Id);
                    List<Booking> bookings = db.Bookings.ToList().Where(x => x.CourseOccasionId == co.Id).ToList();
                    return View("Kurstillfällen/Faktura", bookings);
                }
            }

            //if you've come this far something has gone wrong.
            return Redirect("~/Kursledare/Kurser");
        }
        [HttpPost]
        public ActionResult Kurs([Bind(Include = "Id,CourseId,StartDate,AltHost,AltAddress,AltMail,AltProfilePicture,MinPeople,MaxPeople")] CourseOccasion courseOccasion, [Bind(Include = "Id,Name,Length,Host,Email,Subtitle,Bold,Text,Image,Address,City,Price")] Course course, [Bind(Include = "Id,Firstname,Lastname,Email,CourseOccasionId,PhoneNumber,Company,BillingAddress,PostalCode,City,Bookings,Message,DiscountCode,BookingDate")] Booking booking, [Bind(Include = "Id,CourseId,Text")] BulletPoints bulletPoints, string param1, string param2, string param3)
        {
            int Id = 0;
            List<int> Ids;
            string[] Comp;
            switch (param1)
            {
                case "NyttKT":
                    if (int.TryParse(param2, out Id))
                    {
                        courseOccasion.CourseId = Id;
                        if (User.ValidUser(courseOccasion))
                        {
                            db.CourseOccasions.Add(courseOccasion);
                            if (new CourseOccasion().GetComparison(courseOccasion, out Comp))
                            {
                                db.Logs.Add(new Log() { User = User.Identity.Name, Table = "CourseOccasions", Action = "Add", Before = "[NULL]", After = Comp[1], Time = DateTime.Now });
                            }
                            db.SaveChanges();
                        }
                    }
                    return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfällen/" + Id);

                case "RaderaKT":
                    if (int.TryParse(param2, out Id))
                    {
                        CourseOccasion co = db.CourseOccasions.Find(Id);
                        db.CourseOccasions.Remove(co);
                        if (co.GetComparison(new CourseOccasion(), out Comp))
                        {
                            db.Logs.Add(new Log() { User = User.Identity.Name, Table = "CourseOccasions", Action = "Delete", Before = Comp[0], After = "[DELETED]", Time = DateTime.Now });
                        }
                        db.SaveChanges();
                        return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfällen/" + co.CourseId);
                    }
                    return Redirect("~/Kursledre/Kurser");

                case "RedigeraKT":
                    if (int.TryParse(param2, out Id))
                    {
                        if (int.TryParse(param3, out int CourseId))
                        {
                            course = db.Courses.Where(x => x.Id == CourseId).First();
                            if (User.ValidUser(course))
                            {
                                courseOccasion.CourseId = CourseId;

                                db = new ApplicationDbContext();
                                if (db.CourseOccasions.Find(courseOccasion.Id).GetComparison(courseOccasion, out Comp))
                                {
                                    db.Logs.Add(new Log() { User = User.Identity.Name, Table = "CourseOccasions", Action = "Update", Before = Comp[0], After = Comp[1], Time = DateTime.Now });
                                    db.SaveChanges();
                                }

                                db = new ApplicationDbContext();

                                db.Entry(courseOccasion).State = EntityState.Modified;

                                db.SaveChanges();
                                return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfällen/" + courseOccasion.CourseId);
                            }
                        }
                    }
                    return Redirect("~/Kursledare/Kurser");
                case "RedigeraKurs":
                    course.Email = User.Identity.Name;
                    course.Host = User.GetFullName();
                    
                    db = new ApplicationDbContext();
                    if (db.Courses.Find(course.Id).GetComparison(course, out Comp))
                    {
                        db.Logs.Add(new Log() { User = User.Identity.Name, Table = "Courses", Action = "Update", Before = Comp[0], After = Comp[1], Time = DateTime.Now });
                        db.SaveChanges();
                    }
                    db = new ApplicationDbContext();

                    db.Entry(course).State = EntityState.Modified;

                    db.SaveChanges();
                    return Redirect("~/Kursledare/Kurser");


                case "RaderaKurs":
                    if (param2.GetIds(out Ids))
                    {
                        Id = Ids.First();
                        Course CourseToBeDeleted = db.Courses.Find(Id);

                        db.Courses.Remove(CourseToBeDeleted);
                        if (CourseToBeDeleted.GetComparison(new Course(), out Comp))
                        {
                            db.Logs.Add(new Log() { User = User.Identity.Name, Table = "Courses", Action = "Delete", Before = Comp[0], After = "[DELETED]", Time = DateTime.Now });
                        }
                        db.SaveChanges();
                    }

                    return Redirect("~/Kursledare/Kurser");

                case "SkapaBP":
                    if (param2.GetIds(out Ids))
                    {
                        Id = Ids.First();
                        if (User.ValidUser(DBHandler.GetCourse(Id)))
                        {
                            bulletPoints.CourseId = Id;
                            db.BulletPoints.Add(bulletPoints);
                            db.SaveChanges();
                            return Redirect("~/Kursledare/Kurser/Kurs/Punktlista/" + param2);
                        }
                    }

                    return Redirect("~/Kursledare/Kurser");


                case "NyBokning":

                    if (param2.GetIds(out Ids))
                    {
                        booking.CourseOccasionId = Ids.First();

                        booking.BookingDate = DateTime.Now;
                        db.Bookings.Add(booking);
                        db.SaveChanges();
                        return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfälle/Bokningar/" + Ids.First());
                    }
                    return Redirect("");

                case "RaderaBokning":
                    if (param2.GetIds(out Ids))
                    {

                        int BookingId = Ids.First();
                        Booking bo = db.Bookings.Where(m => m.Id == BookingId).First();
                        CourseOccasion co = DBHandler.GetCourseOccasion(bo);

                        db.Bookings.Remove(bo);

                        if (db.Bookings.Find(booking.Id).GetComparison(booking, out Comp))
                        {
                            db.Logs.Add(new Log() { User = User.Identity.Name, Table = "Bookings", Action = "Delete", Before = Comp[0], After = "[DELETED]", Time = DateTime.Now });
                            db.SaveChanges();
                        }

                        db.SaveChanges();

                        return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfälle/Bokningar/" + co.Id);
                    }
                    return View("");

                case "RedigeraBokning":
                    db = new ApplicationDbContext();
                    if (db.Bookings.Find(booking.Id).GetComparison(booking, out Comp))
                    {
                        db.Logs.Add(new Log() { User = User.Identity.Name, Table = "Bookings", Action = "Update", Before = Comp[0], After = Comp[1], Time = DateTime.Now });
                        db.SaveChanges();
                    }
                    db = new ApplicationDbContext();
                    db.Entry(booking).State = EntityState.Modified;
                    db.SaveChanges();
                    return Redirect("~/Kursledare/Kurser/Kurs/Kurstillfälle/Bokningar/" + booking.CourseOccasionId);

                default: //If you reached this something went wrong
                    return Redirect("~/Kursledare/Kurs/Kurser");
            }
        }
    }
}