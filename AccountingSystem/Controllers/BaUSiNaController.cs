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
    public class BaUSiNaController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /BaUSiNa/
        public ActionResult Index()
        {
            return View(db.baUSiNas.ToList());
        }

        // GET: /BaUSiNa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            baUSiNa bausina = db.baUSiNas.Find(id);
            if (bausina == null)
            {
                return HttpNotFound();
            }
            return View(bausina);
        }

        // GET: /BaUSiNa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /BaUSiNa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "baUSiNaId,baUSirsak,baUSiNo,remarks")] baUSiNa bausina)
        {
            if (ModelState.IsValid)
            {
                baUSiNa objBaUSiNa = new baUSiNa();
                 string baUSirsak = bausina.baUSirsak.Trim();
                  string baUSiNo = bausina.baUSiNo.Trim();
                if ((bausina.baUSiNo != null) && (bausina.baUSirsak != null))
                {
                    objBaUSiNa.baUSirsak = bausina.baUSirsak.Trim();
                    objBaUSiNa.baUSiNo = bausina.baUSiNo.Trim();
                    objBaUSiNa.remarks = bausina.remarks;
                }
                db.baUSiNas.Add(objBaUSiNa);
                db.SaveChanges();
                
                int baUSiNaId = db.baUSiNas.FirstOrDefault(m=>m.baUSiNo==baUSiNo && m.baUSirsak==baUSirsak).baUSiNaId;
                if (db.setNumbers.FirstOrDefault(m => m.baUSiNaId == baUSiNaId && m.faramName=="JornalEntries") == null)
                {
                    db.setNumbers.Add(new setNumber { faramName = "JornalEntries", number = 1, baUSiNaId = baUSiNaId });
                }
                if (db.setNumbers.FirstOrDefault(m => m.baUSiNaId == baUSiNaId && m.faramName == "BhuktaniAdesh") == null)
                {
                    db.setNumbers.Add(new setNumber { faramName = "BhuktaniAdesh", number = 1, baUSiNaId = baUSiNaId });
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bausina);
        }

        // GET: /BaUSiNa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            baUSiNa bausina = db.baUSiNas.Find(id);
            if (bausina == null)
            {
                return HttpNotFound();
            }
            return View(bausina);
        }

        // POST: /BaUSiNa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "baUSiNaId,baUSirsak,baUSiNo,remarks")] baUSiNa bausina)
        {
            if (ModelState.IsValid)
            {
                var objBaUSiNa = db.baUSiNas.Find(bausina.baUSiNaId);
                if ((bausina.baUSiNo != null) && (bausina.baUSirsak != null))
                {
                    objBaUSiNa.baUSirsak = bausina.baUSirsak.Trim();
                    objBaUSiNa.baUSiNo = bausina.baUSiNo.Trim();
                    objBaUSiNa.remarks = bausina.remarks;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bausina);
        }

        // GET: /BaUSiNa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            baUSiNa bausina = db.baUSiNas.Find(id);
            if (bausina == null)
            {
                return HttpNotFound();
            }
            return View(bausina);
        }

        // POST: /BaUSiNa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            baUSiNa bausina = db.baUSiNas.Find(id);
            db.baUSiNas.Remove(bausina);
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
