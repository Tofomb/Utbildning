using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Utbildning.Classes;
using Utbildning.Models;

namespace Utbildning.Controllers
{
    public class KurserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        public ActionResult Index()
        {
            List<CourseOccasion> cos = db.CourseOccasions.ToList().Where(co => co.StartDate > DateTime.Now).ToList();
            List<Course> courses = new List<Course>();
            ViewBag.CO = cos;


            foreach (CourseOccasion courseoccasion in cos)
            {
                courses.Add(DBHandler.GetCourse(courseoccasion));
            }

            //  var x = from unicCourses in courses
            var x = courses.GroupBy(c => c.Id).Select(y => y.First());

            return View(x.ToList());

            // return View(db.Courses.ToList());
        }



        // GET: Kurser/Boka
        public ActionResult Boka(int? id, string w)
        {
            if (w == "nea")
            {
                ViewBag.Warning = "För många deltagare valda, försök igen eller kontakta kursansvarig.";
            }
            CourseOccasion co = db.CourseOccasions.ToListAsync().Result.Where(z => z.Id == id).First();
            Course course = db.Courses.ToListAsync().Result.Where(x => x.Id == DBHandler.GetCourse(co).Id).First();

            ViewBag.Available = DBHandler.GetAvailableBookings(co);

            List<int> numblist = new List<int>();


            for (int n = 1; n <= ViewBag.Available; n++)
            {
                numblist.Add(n);
            }
            SelectList sl = new SelectList(numblist);

            ViewBag.Numblist = numblist;
            ViewBag.CourseTitle = course.Name;
            ViewBag.CourseSubtitle = course.Subtitle;
            //ViewBag.CourseOccasionId = new SelectList(db.CourseOccasions.Where(x => x.CourseId == course.Id && x.StartDate > DateTime.Now), "Id", "StartDate");
            ViewBag.COId = co.Id;
            ViewBag.CODate = co.StartDate;
            return View();
        }

        // POST: Kurser/Boka
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Boka([Bind(Include = "Id,Firstname,Lastname,Email,CourseOccasionId,PhoneNumber,Company,BillingAddress,PostalCode,City,Bookings,Message,DiscountCode,BookingDate")] Booking booking, int? Id)
        {
            if (ModelState.IsValid)
            {
                if (booking.Bookings > 0)
                {

                    if (Id != null)
                    {
                        booking.CourseOccasionId = (int)Id;

                        var co = DBHandler.GetCourseOccasion(booking);
                        if (co.EnoughAvailable(booking.Bookings))
                        {
                            booking.BookingDate = DateTime.Now;

                            db.Bookings.Add(booking);
                            db.SaveChanges();
                            db = new ApplicationDbContext();
                            db.BookingDatas.Add(db.Bookings.Find(booking.Id).GetBookingData());
                            db.SaveChanges();
                            return RedirectToAction("Index");

                        }
                        else
                        {
                            ViewBag.CourseOccasionId = new SelectList(db.CourseOccasions, "Id", "StartDate", booking.CourseOccasionId);
                            return Redirect("~/kurser/boka?id=" + booking.GetCourseOccasion().GetCourse().Id + "&w=nea");
                        }
                    }
                }
                else
                    return View();                
            }
            ViewBag.CourseOccasionId = new SelectList(db.CourseOccasions, "Id", "StartDate", booking.CourseOccasionId);
            return Redirect("~/kurser/boka?id=" + booking.GetCourseOccasion().GetCourse().Id);



        }


        //




        // GET: Courses/LäsMer/5        
        public ActionResult LäsMer(int? id)
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
            var courseOccasions = (db.CourseOccasions.Where(m => m.CourseId == id && m.StartDate > DateTime.Now)).ToList();

            ViewBag.CourseOccasionViewBag = courseOccasions;

            var courseBulletpoints = (db.BulletPoints.Where(m => m.CourseId == id)).ToList();
            ViewBag.CourseBulletPoints = courseBulletpoints;

            ViewBag.Image = DBHandler.GetProfilePictureByEmail(course.Email);

            return View(course);
        }




        //

        // GET: Courses/Details/5        
        public ActionResult Details(int? id)
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



        // GET: Courses/Create
        [ActionName("hhh")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Length,Host,Email,Subtitle,Bold,Text,Image,Address,City,Price")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
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









        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Length,Host,Email,Subtitle,Bold,Text,Image,Address,City,Price")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
