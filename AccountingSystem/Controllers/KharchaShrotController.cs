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
    public class KharchaShrotController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /KharchaShrot/
        public ActionResult Index()
        {
            return View(db.khaShrots.ToList());
        }

        // GET: /KharchaShrot/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            khaShrot khashrot = db.khaShrots.Find(id);
            if (khashrot == null)
            {
                return HttpNotFound();
            }
            return View(khashrot);
        }

        // GET: /KharchaShrot/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /KharchaShrot/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="sourceId,sourceName,remarks")] khaShrot khashrot)
        {
            if (ModelState.IsValid)
            {
                khaShrot objKhaShrot = new khaShrot();
                objKhaShrot.sourceName = TrimText(khashrot.sourceName);
                objKhaShrot.remarks = TrimText(khashrot.remarks);
                db.khaShrots.Add(objKhaShrot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(khashrot);
        }

        // GET: /KharchaShrot/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            khaShrot khashrot = db.khaShrots.Find(id);
            if (khashrot == null)
            {
                return HttpNotFound();
            }
            return View(khashrot);
        }

        // POST: /KharchaShrot/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="sourceId,sourceName,remarks")] khaShrot khashrot)
        {
            if (ModelState.IsValid)
            {
                khaShrot objKhaShrot = new khaShrot();
                objKhaShrot.sourceName = TrimText(khashrot.sourceName);
                objKhaShrot.remarks = TrimText(khashrot.remarks);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khashrot);
        }

        // GET: /KharchaShrot/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            khaShrot khashrot = db.khaShrots.Find(id);
            if (khashrot == null)
            {
                return HttpNotFound();
            }
            return View(khashrot);
        }

        // POST: /KharchaShrot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            khaShrot khashrot = db.khaShrots.Find(id);
            db.khaShrots.Remove(khashrot);
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
