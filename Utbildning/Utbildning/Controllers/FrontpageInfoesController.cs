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
    public class FrontpageInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FrontpageInfoes
        public ActionResult Index()
        {
            return View(db.FrontpageInfoes.ToList());
        }

        // GET: FrontpageInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrontpageInfo frontpageInfo = db.FrontpageInfoes.Find(id);
            if (frontpageInfo == null)
            {
                return HttpNotFound();
            }
            return View(frontpageInfo);
        }

        // GET: FrontpageInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FrontpageInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Bold,Text")] FrontpageInfo frontpageInfo)
        {
            if (ModelState.IsValid)
            {
                db.FrontpageInfoes.Add(frontpageInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(frontpageInfo);
        }

        // GET: FrontpageInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrontpageInfo frontpageInfo = db.FrontpageInfoes.Find(id);
            if (frontpageInfo == null)
            {
                return HttpNotFound();
            }
            return View(frontpageInfo);
        }

        // POST: FrontpageInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Bold,Text")] FrontpageInfo frontpageInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(frontpageInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(frontpageInfo);
        }

        // GET: FrontpageInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrontpageInfo frontpageInfo = db.FrontpageInfoes.Find(id);
            if (frontpageInfo == null)
            {
                return HttpNotFound();
            }
            return View(frontpageInfo);
        }

        // POST: FrontpageInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FrontpageInfo frontpageInfo = db.FrontpageInfoes.Find(id);
            db.FrontpageInfoes.Remove(frontpageInfo);
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
