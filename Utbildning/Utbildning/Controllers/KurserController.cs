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
                            
                            // Tester
                            string MailText = DBHandler.GetCourse(DBHandler.GetCourseOccasion(booking.CourseOccasionId)).Name + " " + DBHandler.GetCourseOccasion(booking.CourseOccasionId).StartDate.Format() + "\n Tack för din bokning, " + booking.Firstname + " " + booking.Lastname + "\n Platser:" + booking.Bookings +  "\n Om du har några frågor, hör av dig till kursansvarig: " + DBHandler.GetCourse(DBHandler.GetCourseOccasion(booking.CourseOccasionId)).Email;
                            string MailTextKL = "Ny bokning \n" + DBHandler.GetCourse(DBHandler.GetCourseOccasion(booking.CourseOccasionId)).Name + " " + DBHandler.GetCourseOccasion(booking.CourseOccasionId).StartDate.Format() + "\n" + booking.Firstname + " " + booking.Lastname + "\n Platser:" + booking.Bookings;

                            MailHandler.SendTester("tobias.c.forsberg@gmail.com", booking.Email, "Bokningsbekräftelse",  MailText, "L1llaskuggan");
                           // MailHandler.Send(DBHandler.GetCourse(DBHandler.GetCourseOccasion(booking.CourseOccasionId)).Email, "Bokningsbekräftelse", MailTextKL);
                            Course course = DBHandler.GetCourse(co);
                            return RedirectToAction("Tack", new { kn = course.Name, ad = course.Address, ci = course.City, da = co.StartDate });
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

            ViewBag.CourseOccasionViewBag = from co in courseOccasions
                                            orderby co.StartDate
                                            select co;

            ViewBag.Text = new HtmlString(course.Text.Replace(Environment.NewLine, "<br/>"));
            var courseBulletpoints = (db.BulletPoints.Where(m => m.CourseId == id)).ToList();
            ViewBag.CourseBulletPoints = courseBulletpoints;

            ViewBag.Image = DBHandler.GetProfilePictureByEmail(course.Email);

            return View(course);
        }


         public ActionResult Om()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Om(string UserEmail)
        {

            string UserData = "";
            var bookings = db.Bookings.Where(e => e.Email == UserEmail).ToList();
            
            foreach(var x in bookings){
                
                UserData += "Namn: "  + x.Firstname;
                UserData += " " + x.Lastname;
                UserData += "\n Mail: " + x.Email;
                UserData += "\n Telefonnummer: " + x.PhoneNumber;
                UserData += "\n Fakturaadress: " + x.BillingAddress;
                UserData += "\n Postkod: " + x.PostalCode;
                UserData += "\n Stad: " + x.City;
                UserData += "\n Meddelande: " + x.Message;
                UserData += "\n Rabattkod: " + x.DiscountCode;
                UserData += "\n Bokningsdatum: " + x.BookingDate;
                UserData += "\n Antal bokningar: " + x.Bookings;
                UserData += "\n Kurs: " + DBHandler.GetCourse(DBHandler.GetCourseOccasion(x.CourseOccasionId)).Name;
                UserData += "\n Kurstillfälle: " + DBHandler.GetCourseOccasion(x.CourseOccasionId).StartDate;
                UserData += "\n \n";

            }
            string MailText = "Du efterfrågade en utskrit om all data som Castra Utbildning har sparat kopplat till din mail, om du önskar att ta bort information från vår databas, tryck på länken nedanför. Var medveten om att detta även betyder att bokningen kopplad till din email kommer att avbokas.";
            string SecretEmail = UserEmail;


            // MailHandler.Send(UserEmail, "Användardata Castra-utbildning", UserData + MailText);
            
            //Tester
            MailHandler.SendTester("tobias.c.forsberg@gmail.com",UserEmail,"Användardata Castra", UserData + MailText,"L1llaskuggan");



            return View();
        }


        public ActionResult Tack(string kn, string ad, string ci, string da)
        {
            ViewBag.Namn = "Kurs: " + kn;
            ViewBag.Address = "Address: " + ad;
            ViewBag.City = "Ort: " + ci;
            ViewBag.Date = "Datum" + da;

            return View();
        }
    }
}
