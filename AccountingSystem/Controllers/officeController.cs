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
using AccountingSystem.Reports;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class officeController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /office/
        public ActionResult Index()
        {
            return View(db.offices.ToList());
        }
        // GET: /office/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            office office = db.offices.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            return View(office);
        }
        
        // GET: /office/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /office/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "officeId,mantralaye,bibhag,karyalaye,address,phone,url_web,email,POBoxNo,officeCode")] office office)
        {
            if (ModelState.IsValid)
            {
                office objOffice = new office();
                objOffice.mantralaye = TrimText(office.mantralaye);
                objOffice.bibhag = TrimText(office.bibhag);
                objOffice.karyalaye = TrimText(office.karyalaye);
                objOffice.address = TrimText(office.address);
                objOffice.phone = TrimText(office.phone);
                objOffice.url_web = office.url_web;
                objOffice.email = office.email;
                objOffice.POBoxNo = TrimText(office.POBoxNo);
                objOffice.officeCode = TrimText(office.officeCode);
                db.offices.Add(office);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(office);
        }

        // GET: /office/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            office office = db.offices.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            return View(office);
        }

        // POST: /office/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="officeId,mantralaye,bibhag,karyalaye,address,phone,url_web,email,POBoxNo,officeCode")] office office)
        {
            if (office == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                office objOffice = db.offices.Find(office.officeId);
                objOffice.mantralaye = TrimText(office.mantralaye);
                objOffice.bibhag = TrimText(office.bibhag);
                objOffice.karyalaye = TrimText(office.karyalaye);
                objOffice.address = TrimText(office.address);
                objOffice.phone = TrimText(office.phone);
                objOffice.url_web = office.url_web;
                objOffice.email = office.email;
                objOffice.POBoxNo = TrimText(office.POBoxNo);
                objOffice.officeCode = TrimText(office.officeCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(office);
        }

        // GET: /office/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            office office = db.offices.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    TempData["Message"] = "Record Deleted Successfully";
                    return RedirectToAction("Index", "Office");
                }
                catch {
                    TempData["ErrorMessage"] = "No Records Deleted";
                    return RedirectToAction("Index", "Office");
                }
            }
        }

        // POST: /office/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            office office = db.offices.Find(id);
            db.offices.Remove(office);
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

        public ActionResult CreateCat()
        {
            return View();
        }

    }
}
