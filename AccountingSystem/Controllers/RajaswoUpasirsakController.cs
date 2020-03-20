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
    public class RajaswoUpasirsakController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /RajaswoUpasirsak/
        public ActionResult Index()
        {
            var rajaswoupasirsaks = db.rajaswoUpasirsaks.Include(r => r.rajaswoSirsak);
            return View(rajaswoupasirsaks.ToList());
        }

        // GET: /RajaswoUpasirsak/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoUpasirsak rajaswoupasirsak = db.rajaswoUpasirsaks.Find(id);
            if (rajaswoupasirsak == null)
            {
                return HttpNotFound();
            }
            return View(rajaswoupasirsak);
        }

        // GET: /RajaswoUpasirsak/Create
        public ActionResult Create()
        {
            ViewBag.rsId = new SelectList(db.rajaswoSirsaks, "rsId", "sirsak");
            return View();
        }

        // POST: /RajaswoUpasirsak/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ruId,upaSirsak,rsId,remarks")] rajaswoUpasirsak rajaswoupasirsak)
        {
            if (ModelState.IsValid)
            {
                rajaswoUpasirsak objUpaSirsak = new rajaswoUpasirsak();
                objUpaSirsak.upaSirsak = TrimText(objUpaSirsak.upaSirsak);
                objUpaSirsak.remarks = TrimText(objUpaSirsak.remarks);
                objUpaSirsak.rsId = rajaswoupasirsak.rsId;
                db.rajaswoUpasirsaks.Add(objUpaSirsak);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.rsId = new SelectList(db.rajaswoSirsaks, "rsId", "sirsak", rajaswoupasirsak.rsId);
            return View(rajaswoupasirsak);
        }

        // GET: /RajaswoUpasirsak/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoUpasirsak rajaswoupasirsak = db.rajaswoUpasirsaks.Find(id);
            if (rajaswoupasirsak == null)
            {
                return HttpNotFound();
            }
            ViewBag.rsId = new SelectList(db.rajaswoSirsaks, "rsId", "sirsak", rajaswoupasirsak.rsId);
            return View(rajaswoupasirsak);
        }

        // POST: /RajaswoUpasirsak/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ruId,upaSirsak,rsId,remarks")] rajaswoUpasirsak rajaswoupasirsak)
        {
            if (ModelState.IsValid)
            {
                rajaswoUpasirsak objUpaSirsak = db.rajaswoUpasirsaks.Find(rajaswoupasirsak.ruId);
                objUpaSirsak.upaSirsak = TrimText(objUpaSirsak.upaSirsak);
                objUpaSirsak.remarks = TrimText(objUpaSirsak.remarks);
                objUpaSirsak.rsId = rajaswoupasirsak.rsId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.rsId = new SelectList(db.rajaswoSirsaks, "rsId", "sirsak", rajaswoupasirsak.rsId);
            return View(rajaswoupasirsak);
        }

        // GET: /RajaswoUpasirsak/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoUpasirsak rajaswoupasirsak = db.rajaswoUpasirsaks.Find(id);
            if (rajaswoupasirsak == null)
            {
                return HttpNotFound();
            }
            return View(rajaswoupasirsak);
        }

        // POST: /RajaswoUpasirsak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            rajaswoUpasirsak rajaswoupasirsak = db.rajaswoUpasirsaks.Find(id);
            db.rajaswoUpasirsaks.Remove(rajaswoupasirsak);
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
        public string TrimText(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            else
            {
                return str.Trim();
            }
        }
    }
}
