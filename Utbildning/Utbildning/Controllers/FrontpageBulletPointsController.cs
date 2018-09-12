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
    public class FrontpageBulletPointsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FrontpageBulletPoints
        public ActionResult Index()
        {
            return View(db.FrontpageBulletPoints.ToList());
        }

        // GET: FrontpageBulletPoints/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrontpageBulletPoints frontpageBulletPoints = db.FrontpageBulletPoints.Find(id);
            if (frontpageBulletPoints == null)
            {
                return HttpNotFound();
            }
            return View(frontpageBulletPoints);
        }

        // GET: FrontpageBulletPoints/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FrontpageBulletPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text")] FrontpageBulletPoints frontpageBulletPoints)
        {
            if (ModelState.IsValid)
            {
                db.FrontpageBulletPoints.Add(frontpageBulletPoints);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(frontpageBulletPoints);
        }

        // GET: FrontpageBulletPoints/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrontpageBulletPoints frontpageBulletPoints = db.FrontpageBulletPoints.Find(id);
            if (frontpageBulletPoints == null)
            {
                return HttpNotFound();
            }
            return View(frontpageBulletPoints);
        }

        // POST: FrontpageBulletPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text")] FrontpageBulletPoints frontpageBulletPoints)
        {
            if (ModelState.IsValid)
            {
                db.Entry(frontpageBulletPoints).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(frontpageBulletPoints);
        }

        // GET: FrontpageBulletPoints/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrontpageBulletPoints frontpageBulletPoints = db.FrontpageBulletPoints.Find(id);
            if (frontpageBulletPoints == null)
            {
                return HttpNotFound();
            }
            return View(frontpageBulletPoints);
        }

        // POST: FrontpageBulletPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FrontpageBulletPoints frontpageBulletPoints = db.FrontpageBulletPoints.Find(id);
            db.FrontpageBulletPoints.Remove(frontpageBulletPoints);
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
