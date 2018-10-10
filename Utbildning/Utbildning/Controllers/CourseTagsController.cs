using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Utbildning.Models;

namespace Utbildning.Controllers
{
    public class CourseTagsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CourseTags
        public ActionResult Index()
        {
            var courseTags = db.CourseTags.Include(c => c.Course);
            return View(courseTags.ToList());
        }

        // GET: CourseTags/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTags courseTags = db.CourseTags.Find(id);
            if (courseTags == null)
            {
                return HttpNotFound();
            }
            return View(courseTags);
        }

        // GET: CourseTags/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");
            return View();
        }

        // POST: CourseTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CourseId,Text")] CourseTags courseTags)
        {
            if (ModelState.IsValid)
            {
                db.CourseTags.Add(courseTags);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", courseTags.CourseId);
            return View(courseTags);
        }

        // GET: CourseTags/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTags courseTags = db.CourseTags.Find(id);
            if (courseTags == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", courseTags.CourseId);
            return View(courseTags);
        }

        // POST: CourseTags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CourseId,Text")] CourseTags courseTags)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseTags).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", courseTags.CourseId);
            return View(courseTags);
        }

        // GET: CourseTags/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTags courseTags = db.CourseTags.Find(id);
            if (courseTags == null)
            {
                return HttpNotFound();
            }
            return View(courseTags);
        }

        // POST: CourseTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseTags courseTags = db.CourseTags.Find(id);
            db.CourseTags.Remove(courseTags);
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
