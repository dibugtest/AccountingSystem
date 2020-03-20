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
    public class SetNumberController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /SetNumber/
        public ActionResult Index()
        {
            return View(db.setNumbers.ToList());
        }

        // GET: /SetNumber/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            setNumber setnumber = db.setNumbers.Find(id);
            if (setnumber == null)
            {
                return HttpNotFound();
            }
            return View(setnumber);
        }

        // GET: /SetNumber/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /SetNumber/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="setNumberId,faramName,number")] setNumber setnumber)
        {
            if (ModelState.IsValid)
            {
                db.setNumbers.Add(setnumber);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(setnumber);
        }

        // GET: /SetNumber/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            setNumber setnumber = db.setNumbers.Find(id);
            if (setnumber == null)
            {
                return HttpNotFound();
            }
            return View(setnumber);
        }

        // POST: /SetNumber/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="setNumberId,faramName,number")] setNumber setnumber)
        {
            if (ModelState.IsValid)
            {
                db.Entry(setnumber).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(setnumber);
        }

        // GET: /SetNumber/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            setNumber setnumber = db.setNumbers.Find(id);
            if (setnumber == null)
            {
                return HttpNotFound();
            }
            return View(setnumber);
        }

        // POST: /SetNumber/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            setNumber setnumber = db.setNumbers.Find(id);
            db.setNumbers.Remove(setnumber);
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
