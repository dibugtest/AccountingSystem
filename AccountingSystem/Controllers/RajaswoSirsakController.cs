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
    public class RajaswoSirsakController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /RajaswoSirsak/
        public ActionResult Index()
        {
            return View(db.rajaswoSirsaks.ToList());
        }

        // GET: /RajaswoSirsak/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoSirsak rajaswosirsak = db.rajaswoSirsaks.Find(id);
            if (rajaswosirsak == null)
            {
                return HttpNotFound();
            }
            return View(rajaswosirsak);
        }

        // GET: /RajaswoSirsak/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /RajaswoSirsak/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="rsId,sirsak,sirsakNo,remarks")] rajaswoSirsak rajaswosirsak)
        {
            if (ModelState.IsValid)
            {
                rajaswoSirsak objRajaswo = new rajaswoSirsak();
                objRajaswo.sirsak = TrimText(rajaswosirsak.sirsak);
                objRajaswo.sirsakNo = TrimText(rajaswosirsak.sirsakNo);
                objRajaswo.remarks = TrimText(rajaswosirsak.remarks);
                db.rajaswoSirsaks.Add(objRajaswo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rajaswosirsak);
        }

        // GET: /RajaswoSirsak/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoSirsak rajaswosirsak = db.rajaswoSirsaks.Find(id);
            if (rajaswosirsak == null)
            {
                return HttpNotFound();
            }
            return View(rajaswosirsak);
        }

        // POST: /RajaswoSirsak/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="rsId,sirsak,sirsakNo,remarks")] rajaswoSirsak rajaswosirsak)
        {
            if (ModelState.IsValid)
            {
                rajaswoSirsak objRajaswo = db.rajaswoSirsaks.Find(rajaswosirsak.rsId);
                objRajaswo.sirsak = TrimText(rajaswosirsak.sirsak);
                objRajaswo.sirsakNo = TrimText(rajaswosirsak.sirsakNo);
                objRajaswo.remarks = TrimText(rajaswosirsak.remarks);
                db.rajaswoSirsaks.Add(objRajaswo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rajaswosirsak);
        }

        // GET: /RajaswoSirsak/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoSirsak rajaswosirsak = db.rajaswoSirsaks.Find(id);
            if (rajaswosirsak == null)
            {
                return HttpNotFound();
            }
            return View(rajaswosirsak);
        }

        // POST: /RajaswoSirsak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            rajaswoSirsak rajaswosirsak = db.rajaswoSirsaks.Find(id);
            db.rajaswoSirsaks.Remove(rajaswosirsak);
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
