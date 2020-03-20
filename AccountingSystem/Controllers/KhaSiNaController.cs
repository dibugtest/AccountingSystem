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
    public class KhaSiNaController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /KhaSiNa/
        public ActionResult Index()
        {
            var khasinas = db.khaSiNas.Include(k => k.baUSiNa);
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSirsak");
            return View(khasinas.ToList());
        }

        [HttpPost]
        public ActionResult Index(FormCollection col)
        {
            try
            {
                int baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
                ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSirsak", baUSiNaId);
                var khasinas = db.khaSiNas.Include(k => k.baUSiNa).Where(m => m.baUSiNaId == baUSiNaId);
                return View(khasinas.ToList());
            }
            catch
            {
                ViewBag.ErrorMessage = "Please Select BaUSiNa to filter Records.";
                ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSirsak");
                return View(db.khaSiNas.Include(k => k.baUSiNa));
            }

        }


        // GET: /KhaSiNa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            khaSiNa khasina = db.khaSiNas.Find(id);
            if (khasina == null)
            {
                return HttpNotFound();
            }
            return View(khasina);
        }

        // GET: /KhaSiNa/Create
        public ActionResult Create()
        {
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo");
            return View();
        }

        // POST: /KhaSiNa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="khaSiNaId,khaSirsak,khaSiNo,khaRakam,remarks,baUSiNaId")] khaSiNa khasina)
        {
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", khasina.baUSiNaId);
            if (ModelState.IsValid)
            {
                khaSiNa objKhaSiNa = new khaSiNa();
                objKhaSiNa.khaSirsak = TrimText(khasina.khaSirsak);
                objKhaSiNa.khaSiNo = TrimText(khasina.khaSiNo);
                objKhaSiNa.khaRakam = khasina.khaRakam;
                objKhaSiNa.remarks = khasina.remarks;
                objKhaSiNa.baUSiNaId = khasina.baUSiNaId;
                db.khaSiNas.Add(objKhaSiNa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khasina);
        }

        // GET: /KhaSiNa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            khaSiNa khasina = db.khaSiNas.Find(id);
            if (khasina == null)
            {
                return HttpNotFound();
            }
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSirsak", khasina.baUSiNaId);
            return View(khasina);
        }

        // POST: /KhaSiNa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "khaSiNaId,khaSirsak,khaSiNo,khaRakam,remarks,baUSiNaId")] khaSiNa khasina)
        {
            if (ModelState.IsValid)
            {
                khaSiNa objKhaSiNa = db.khaSiNas.Find(khasina.khaSiNaId);
                objKhaSiNa.khaSirsak = TrimText(khasina.khaSirsak);
                objKhaSiNa.khaSiNo = TrimText(khasina.khaSiNo);
                objKhaSiNa.khaRakam = khasina.khaRakam;
                objKhaSiNa.remarks = khasina.remarks;
                objKhaSiNa.baUSiNaId = khasina.baUSiNaId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSirsak", khasina.baUSiNaId);
            return View(khasina);
        }

        // GET: /KhaSiNa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            khaSiNa khasina = db.khaSiNas.Find(id);
            if (khasina == null)
            {
                return HttpNotFound();
            }
            return View(khasina);
        }

        // POST: /KhaSiNa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            khaSiNa khasina = db.khaSiNas.Find(id);
            db.khaSiNas.Remove(khasina);
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
