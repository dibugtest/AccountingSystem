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
    public class VendorController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /Vendor/
        public ActionResult Index()
        {
            return View(db.vendors.ToList());
        }

        // GET: /Vendor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vendor vendor = db.vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // GET: /Vendor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Vendor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="vendorId,name,address,TPIN_PAN,karDataNo,email,altEmail,phone,mobilePhone,url,contactPerson,bankName,bankBranch,bankAccNo")] vendor vendor)
        {
            if (ModelState.IsValid)
            {
                vendor objVendor = new vendor();
                objVendor.name = TrimText(vendor.name);
                objVendor.address = TrimText(vendor.address);
                objVendor.TPIN_PAN = TrimText(vendor.TPIN_PAN);
                objVendor.karDataNo = TrimText(vendor.karDataNo);
                objVendor.email = vendor.email;
                objVendor.altEmail = vendor.altEmail;
                objVendor.phone = TrimText(vendor.phone);
                objVendor.mobilePhone = TrimText(vendor.mobilePhone);
                objVendor.url = TrimText(vendor.url);
                objVendor.contactPerson = TrimText(vendor.contactPerson);
                objVendor.bankName = TrimText(vendor.bankName);
                objVendor.bankBranch = TrimText(vendor.bankBranch);
                objVendor.bankAccNo = TrimText(vendor.bankAccNo);
                db.vendors.Add(objVendor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vendor);
        }

        // GET: /Vendor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vendor vendor = db.vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // POST: /Vendor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="vendorId,name,address,TPIN_PAN,karDataNo,email,altEmail,phone,mobilePhone,url,contactPerson,bankName,bankBranch,bankAccNo")] vendor vendor)
        {
            if (ModelState.IsValid)
            {
                vendor objVendor = db.vendors.Find(vendor.vendorId);
                objVendor.name = TrimText(vendor.name);
                objVendor.address = TrimText(vendor.address);
                objVendor.TPIN_PAN = TrimText(vendor.TPIN_PAN);
                objVendor.karDataNo = TrimText(vendor.karDataNo);
                objVendor.email = vendor.email;
                objVendor.altEmail = vendor.altEmail;
                objVendor.phone = TrimText(vendor.phone);
                objVendor.mobilePhone = TrimText(vendor.mobilePhone);
                objVendor.url = TrimText(vendor.url);
                objVendor.contactPerson = TrimText(vendor.contactPerson);
                objVendor.bankName = TrimText(vendor.bankName);
                objVendor.bankBranch = TrimText(vendor.bankBranch);
                objVendor.bankAccNo = TrimText(vendor.bankAccNo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vendor);
        }

        // GET: /Vendor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vendor vendor = db.vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // POST: /Vendor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            vendor vendor = db.vendors.Find(id);
            db.vendors.Remove(vendor);
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
