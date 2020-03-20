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
    public class FiscalYearController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /FiscalYear/
        public ActionResult Index()
        {
            return View(db.fisYears.ToList());
        }

        // GET: /FiscalYear/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fiscalYear fiscalyear = db.fisYears.Find(id);
            if (fiscalyear == null)
            {
                return HttpNotFound();
            }
            return View(fiscalyear);
        }

        public ActionResult SetFiscalYear()
        {
           // ViewBag.fySelected = db.fisYears.FirstOrDefault(m=>m.status==true).fyId;
           // ViewBag.fisYear = db.fisYears.ToList();
            return View(db.fisYears.ToList());
        }

        // GET: /FiscalYear/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /FiscalYear/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="fyId,nepFy,enFy,status,activatedDate,activatedBy")] fiscalYear fiscalyear)
        {
            if (fiscalyear.status == true && ModelState.IsValid) 
            {
                var objActiveFy = db.fisYears.FirstOrDefault(m=>m.status==true);
                objActiveFy.status =false;
                objActiveFy.nepFy = objActiveFy.nepFy;
                objActiveFy.enFy = objActiveFy.enFy;
            }
            
            if (ModelState.IsValid )
            {
                fiscalYear objFy = new fiscalYear();
                objFy.nepFy = TrimText(fiscalyear.nepFy);
                objFy.enFy = TrimText(fiscalyear.enFy);
                objFy.status = fiscalyear.status;
                objFy.activatedDate = fiscalyear.activatedDate;
                objFy.activatedBy = fiscalyear.activatedBy;
                db.fisYears.Add(objFy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fiscalyear);
        }

        public ActionResult CreateFisYear()
        {
            return View();
        }

        // POST: /FiscalYear/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFisYear(FormCollection col,fiscalYear fiscalyear)
        {
            try
            {
                int nepFy1 = fiscalyear.nepStartYear;
                int nepFy2 = fiscalyear.nepEndYear;
                int engFy1 = fiscalyear.enStartYear;
                int engFy2 = fiscalyear.enEndYear;
                bool status = fiscalyear.status;

                if (nepFy1 == 0 || nepFy2 == 0 || engFy1 == 0 || engFy2 == 0)
                {
                    ViewBag.ErrorMessage = "Error No record inserted";
                    return View();
                }
                else
                {
                    string nepFy = StringToUnicode(nepFy1.ToString()) + "/" + StringToUnicode(nepFy2.ToString().Substring(1,3));
                    string enFy = StringToUnicode(engFy1.ToString()) + "/" + StringToUnicode(engFy2.ToString().Substring(1,3));
                    var objFYear = db.fisYears.FirstOrDefault(m => m.nepFy == nepFy && m.enFy == enFy);
                    if (objFYear != null)
                    {
                        ViewBag.ErrorMessage = "Record already contains a fisyear" + objFYear.nepFy;
                        return View();
                    }
                    if (status == true)
                    {
                        var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                        objFiscalYear.status = false;
                        db.SaveChanges();
                        fiscalYear  objFisYear = new fiscalYear();
                        objFisYear.nepStartYear = fiscalyear.nepStartYear;
                        objFisYear.nepEndYear = fiscalyear.nepEndYear;
                        objFisYear.enStartYear = fiscalyear.enStartYear;
                        objFisYear.enEndYear= fiscalyear.enEndYear;
                        objFisYear.nepFy = nepFy;
                        objFisYear.enFy = enFy;
                        objFisYear.status = status;
                        db.fisYears.Add(objFisYear);
                        db.SaveChanges();
                        TempData["Message"] = "Record Inserted Successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        db.fisYears.Add(new fiscalYear {nepStartYear=fiscalyear.nepStartYear,nepEndYear=fiscalyear.nepEndYear,enStartYear=fiscalyear.enStartYear,enEndYear=fiscalyear.enEndYear, nepFy = nepFy, enFy = enFy, status = status, activatedBy = fiscalyear.activatedBy, activatedDate = fiscalyear.activatedDate });
                        db.SaveChanges();
                        TempData["Message"] = "Record Inserted Successfully";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch
            {
                ViewBag.ErrorMessage = "Record Not Inserted";
                return View();
            }
         
        }



        // GET: /FiscalYear/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fiscalYear fiscalyear = db.fisYears.Find(id);
            if (fiscalyear == null)
            {
                return HttpNotFound();
            }
            return View(fiscalyear);
        }

        // POST: /FiscalYear/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(fiscalYear fiscalyear)
        {
            var objActiveFy = db.fisYears.FirstOrDefault(m => m.status == true);
            if (fiscalyear.status == true && ModelState.IsValid &&(fiscalyear.fyId!=objActiveFy.fyId))
            {
                objActiveFy.status = false;
                objActiveFy.nepFy = objActiveFy.nepFy;
                objActiveFy.enFy = objActiveFy.enFy;

                var objfiscalYear = db.fisYears.Find(fiscalyear.fyId);
                objfiscalYear.nepFy = TrimText(fiscalyear.nepFy);
                objfiscalYear.enFy = TrimText(fiscalyear.enFy);
                objfiscalYear.status = true;
                objfiscalYear.activatedDate = fiscalyear.activatedDate;
                objfiscalYear.activatedBy = fiscalyear.activatedBy;

            }
            else if (ModelState.IsValid && (fiscalyear.fyId==objActiveFy.fyId))
            {
                fiscalYear objFy = db.fisYears.Find(fiscalyear.fyId);
                objFy.nepFy = TrimText(fiscalyear.nepFy);
                objFy.enFy = TrimText(fiscalyear.enFy);
                objFy.status = true;
                objFy.activatedDate = fiscalyear.activatedDate;
                objFy.activatedBy = fiscalyear.activatedBy;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public ActionResult EditFisYear(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fiscalYear fiscalyear = db.fisYears.Find(id);
            if (fiscalyear == null)
            {
                return HttpNotFound();
            }
            return View(fiscalyear);
        }

        // POST: /FiscalYear/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFisYear([Bind(Include = "nepStartYear,nepEndYear,enStartYear,enEndYear, fyId,nepFy,enFy,status,activatedDate,activatedBy")] fiscalYear fiscalyear)
        {
            var objActiveFy = db.fisYears.FirstOrDefault(m => m.status == true);
            string nepFy = StringToUnicode(fiscalyear.nepStartYear.ToString()) + "/" + StringToUnicode(fiscalyear.nepEndYear.ToString().Substring(1, 3));
            string enFy = StringToUnicode(fiscalyear.enStartYear.ToString()) + "/" + StringToUnicode(fiscalyear.enEndYear.ToString().Substring(1, 3));
            if (ModelState.IsValid)
            {
                if (fiscalyear.status == true)
                {
                    objActiveFy.status = false;
                    var objfiscalYear = db.fisYears.Find(fiscalyear.fyId);
                    objfiscalYear.nepFy = nepFy;
                    objfiscalYear.enFy = enFy;
                    objfiscalYear.nepStartYear = fiscalyear.nepStartYear;
                    objfiscalYear.nepEndYear = fiscalyear.nepEndYear;
                    objfiscalYear.enStartYear = fiscalyear.enStartYear;
                    objfiscalYear.enEndYear = fiscalyear.enEndYear;
                    objfiscalYear.status = fiscalyear.status;
                    objfiscalYear.activatedDate = fiscalyear.activatedDate;
                    objfiscalYear.activatedBy = fiscalyear.activatedBy;

                }
                else 
                {
                    fiscalYear objFy = db.fisYears.Find(fiscalyear.fyId);
                    objFy.nepFy = nepFy;
                    objFy.enFy = enFy;
                    objFy.nepStartYear = fiscalyear.nepStartYear;
                    objFy.nepEndYear = fiscalyear.nepEndYear;
                    objFy.enStartYear = fiscalyear.enStartYear;
                    objFy.enEndYear = fiscalyear.enEndYear;
                    objFy.status = objFy.status;
                    objFy.activatedDate = fiscalyear.activatedDate;
                    objFy.activatedBy = fiscalyear.activatedBy;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // GET: /FiscalYear/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fiscalYear fiscalyear = db.fisYears.Find(id);
            if (fiscalyear == null)
            {
                return HttpNotFound();
            }
            return View(fiscalyear);
        }

        // POST: /FiscalYear/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            fiscalYear fiscalyear = db.fisYears.Find(id);
            db.fisYears.Remove(fiscalyear);
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
      
        [HttpPost]
        public JsonResult getFiscalYear()
        {
            var objFy = db.fisYears.FirstOrDefault(m=>m.status==true);
            if (objFy != null)
            {
                return Json("true");
            }
            else {
                return Json("false");
            }
        }

        [HttpPost]
        public JsonResult checkFiscalYear(string nepFy1,string nepFy2,string enFy1,string enFy2)
        {
            try
            {
                int nep1 = Convert.ToInt32(nepFy1);
                int nep2 = Convert.ToInt32(nepFy2);
                int en1 = Convert.ToInt32(enFy1);
                int en2 = Convert.ToInt32(enFy2);
                if(db.fisYears.FirstOrDefault(m=>m.nepStartYear==nep1&&m.enStartYear==en1)!=null)
                //int nep1 = Convert.ToInt32(nepFy1);
                //int nep2 = Convert.ToInt32(nepFy2);
                //string nepFy = StringToUnicode(nepFy1) + "/" + StringToUnicode(nepFy2);
               // if (db.fisYears.FirstOrDefault(m => m.nepFy == nepFy) != null)
                {
                    return Json("true");
                }
                else
                {
                    return Json("false");
                }

            }
            catch
            {
                return Json("false");
            }
            }


        [HttpPost]
        public JsonResult changeStatus(string fyId)
        {
            try
            {
                var activeFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                activeFiscalYear.status = false;
                int id = Convert.ToInt32(fyId);
                var objFisYear = db.fisYears.Find(id);
                objFisYear.nepFy = objFisYear.nepFy;
                objFisYear.enFy = objFisYear.enFy;
                objFisYear.status = true;
                foreach (var item in db.setNumbers.ToList())
                {
                    item.number = 1;
                }
                db.SaveChanges();
                return Json(true);

            }
        catch
            {
                return Json(false);
        }
        
        }


        public string StringToUnicode(string data)
        {
            string uni = "";
            foreach (char c in data)
            {
                var charCode = "U+" + ((int)c).ToString("X4");
                switch (charCode)
                {
                    case "U+0030":
                        uni += "०";
                        break;
                    case "U+0031":
                        uni += "१";
                        break;
                    case "U+0032":
                        uni += "२";
                        break;
                    case "U+0033":
                        uni += "३";
                        break;
                    case "U+0034":
                        uni += "४";
                        break;
                    case "U+0035":
                        uni += "५";
                        break;
                    case "U+0036":
                        uni += "६";
                        break;
                    case "U+0037":
                        uni += "७";
                        break;
                    case "U+0038":
                        uni += "८";
                        break;
                    case "U+0039":
                        uni += "९";
                        break;
                    default:
                        uni += c;
                        break;
                }
            }
            return uni;
        }

    }
}
