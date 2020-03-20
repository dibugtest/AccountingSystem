using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;
using AccountingSystem.DAL;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class BhuktaniPauneController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /BhuktaniPaune/
        public ActionResult Index()
        {
            return View(db.bhuktaniPaunes.ToList());
        }

        // GET: /BhuktaniPaune/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bhuktaniPaune bhuktanipaune = db.bhuktaniPaunes.Find(id);
            if (bhuktanipaune == null)
            {
                return HttpNotFound();
            }
            return View(bhuktanipaune);
        }

        // GET: /BhuktaniPaune/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /BhuktaniPaune/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="bhuktaniPauneId,pauneKoNaam,code,remarks")] bhuktaniPaune bhuktanipaune)
        {
            if (ModelState.IsValid)
            {
                db.bhuktaniPaunes.Add(bhuktanipaune);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bhuktanipaune);
        }

        // GET: /BhuktaniPaune/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bhuktaniPaune bhuktanipaune = db.bhuktaniPaunes.Find(id);
            if (bhuktanipaune == null)
            {
                return HttpNotFound();
            }
            return View(bhuktanipaune);
        }

        // POST: /BhuktaniPaune/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="bhuktaniPauneId,pauneKoNaam,address,code,phone,email,remarks,pauneType")] bhuktaniPaune bhuktanipaune)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bhuktanipaune).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bhuktanipaune);
        }

        // GET: /BhuktaniPaune/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bhuktaniPaune bhuktanipaune = db.bhuktaniPaunes.Find(id);
            if (bhuktanipaune == null)
            {
                return HttpNotFound();
            }
            return View(bhuktanipaune);
        }

        // POST: /BhuktaniPaune/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bhuktaniPaune bhuktanipaune = db.bhuktaniPaunes.Find(id);
            db.bhuktaniPaunes.Remove(bhuktanipaune);
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
