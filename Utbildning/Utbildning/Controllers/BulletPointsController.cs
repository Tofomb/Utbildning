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
    public class BulletPointsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BulletPoints
        public ActionResult Index()
        {
            var bulletPoints = db.BulletPoints.Include(b => b.Course);
            return View(bulletPoints.ToList());
        }

        // GET: BulletPoints/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BulletPoints bulletPoints = db.BulletPoints.Find(id);
            if (bulletPoints == null)
            {
                return HttpNotFound();
            }
            return View(bulletPoints);
        }

        // GET: BulletPoints/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");
            return View();
        }

        // POST: BulletPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CourseId,Text")] BulletPoints bulletPoints)
        {
            if (ModelState.IsValid)
            {
                db.BulletPoints.Add(bulletPoints);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", bulletPoints.CourseId);
            return View(bulletPoints);
        }

        // GET: BulletPoints/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BulletPoints bulletPoints = db.BulletPoints.Find(id);
            if (bulletPoints == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", bulletPoints.CourseId);
            return View(bulletPoints);
        }

        // POST: BulletPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CourseId,Text")] BulletPoints bulletPoints)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bulletPoints).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", bulletPoints.CourseId);
            return View(bulletPoints);
        }

        // GET: BulletPoints/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BulletPoints bulletPoints = db.BulletPoints.Find(id);
            if (bulletPoints == null)
            {
                return HttpNotFound();
            }
            return View(bulletPoints);
        }

        // POST: BulletPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BulletPoints bulletPoints = db.BulletPoints.Find(id);
            db.BulletPoints.Remove(bulletPoints);
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
