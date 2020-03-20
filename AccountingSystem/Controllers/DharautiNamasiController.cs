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
    public class DharautiNamasiController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /DharautiNamasi/
        public ActionResult Index()
        {
            var dharautinaamnamasis = db.dharautiNaamNamasis.Include(d => d.vendor);
            return View(dharautinaamnamasis.ToList());
        }

        // GET: /DharautiNamasi/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dharautiNaamNamasi dharautinaamnamasi = db.dharautiNaamNamasis.Find(id);
            if (dharautinaamnamasi == null)
            {
                return HttpNotFound();
            }
            return View(dharautinaamnamasi);
        }

        // GET: /DharautiNamasi/Create
        public ActionResult Create()
        {
            ViewBag.vendorId = new SelectList(db.vendors, "vendorId", "name");
            var objFy = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.nepFy = objFy.nepFy;
            return View();
        }

        // POST: /DharautiNamasi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(dharautiNaamNamasi dharautinaamnamasi)
        {
            string month = ((month)dharautinaamnamasi.monthIndex).ToString();
            if (ModelState.IsValid)
            {
                dharautinaamnamasi.month = month;
                db.dharautiNaamNamasis.Add(dharautinaamnamasi); 
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.vendorId = new SelectList(db.vendors, "vendorId", "name", dharautinaamnamasi.vendorId);
            var objFy = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.nepFy = objFy.nepFy;
            return View(dharautinaamnamasi);
        }

        // GET: /DharautiNamasi/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dharautiNaamNamasi dharautinaamnamasi = db.dharautiNaamNamasis.Find(id);
            if (dharautinaamnamasi == null)
            {
                return HttpNotFound();
            }
            ViewBag.vendorId = new SelectList(db.vendors, "vendorId", "name", dharautinaamnamasi.vendorId);
            return View(dharautinaamnamasi);
        }

        // POST: /DharautiNamasi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="dnId,vendorId,bibaran,rakam,subDate,nepFy,uptoDate,monthIndex")] dharautiNaamNamasi dharautinaamnamasi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dharautinaamnamasi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.vendorId = new SelectList(db.vendors, "vendorId", "name", dharautinaamnamasi.vendorId);
            return View(dharautinaamnamasi);
        }

        // GET: /DharautiNamasi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dharautiNaamNamasi dharautinaamnamasi = db.dharautiNaamNamasis.Find(id);
            if (dharautinaamnamasi == null)
            {
                return HttpNotFound();
            }
            return View(dharautinaamnamasi);
        }

        // POST: /DharautiNamasi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dharautiNaamNamasi dharautinaamnamasi = db.dharautiNaamNamasis.Find(id);
            db.dharautiNaamNamasis.Remove(dharautinaamnamasi);
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
