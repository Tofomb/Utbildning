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
            if(w == "nea")
            {
                ViewBag.Warning = "För många deltagare valda, försök igen eller kontakta kursansvarig.";
            }
            Course course = db.Courses.ToListAsync().Result.Where(x => x.Id == id).First();
            
          //  ViewBag.Available = DBHandler.GetAvailableBookings(db.CourseOccasions.Where(x=>x.Id==id).First());
            
            ViewBag.CourseTitle = course.Name;
            ViewBag.CourseSubtitle = course.Subtitle;
            ViewBag.CourseOccasionId = new SelectList(db.CourseOccasions.Where(x => x.CourseId == course.Id && x.StartDate > DateTime.Now), "Id", "StartDate");
            return View();
        }

        // POST: Kurser/Boka
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Boka([Bind(Include = "Id,Firstname,Lastname,Email,CourseOccasionId,PhoneNumber,Company,BillingAddress,PostalCode,City,Bookings,Message,DiscountCode,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                //add booking date
                var co = DBHandler.GetCourseOccasion(booking);
                if (co.EnoughAvailable(booking.Bookings))
                {              
                booking.BookingDate = now;
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
                }


            }

           
            ViewBag.CourseOccasionId = new SelectList(db.CourseOccasions, "Id", "StartDate", booking.CourseOccasionId);
            return Redirect("~/kurser/boka?id="+booking.GetCourseOccasion().GetCourse().Id + "&w=nea");
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
            var courseOccasions = (db.CourseOccasions.Where(m => m.CourseId == id)).ToList();

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
