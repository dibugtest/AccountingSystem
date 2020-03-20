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
    public class ArthaBudgetController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /ArthaBudget/
        public ActionResult Index()
        {
            List<arthaBudget> objArtha = new List<arthaBudget>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(objArtha);
        }
        [HttpPost]
        public ActionResult Index(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
           // string year = StringToUnicode(col["year"]);
            int mn = Convert.ToInt32(col["month"]);
            string year = getYear(fyId, mn);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            var objArthaBudget = db.arthaBudgets.Where(m => (m.year == year) && (m.monthIndex == mn)).ToList();
            if (objArthaBudget.Count == 0)
            {
                List<arthaBudget> objArtha = new List<arthaBudget>();
                ViewBag.ErrorMessage = "No Record Found";
                return View(objArtha);
            }
            return View(objArthaBudget.ToList());
        }
        // GET: /ArthaBudget/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            arthaBudget arthabudget = db.arthaBudgets.Find(id);
            if (arthabudget == null)
            {
                return HttpNotFound();
            }
            return View(arthabudget);
        }

        // GET: /ArthaBudget/Create
        public ActionResult Create()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears.ToList(), "fyId", "nepFy", objFiscalYear.fyId);
            List<arthaBudget> artha = new List<arthaBudget> { new arthaBudget { arthaBudgetId = 0,fyId=1 } };
            return View(artha);
        }

        // POST: /ArthaBudget/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<arthaBudget> arthabudget, FormCollection col)
        {
            DateTime nepDate;
            int mn = Convert.ToInt32(col["month"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            //var objFiscalYear = db.fisYears.Find(fyId);
            string year = getYear(fyId, mn);
            nepDate = Convert.ToDateTime(col["nepDate"]);
            var objArtha = db.arthaBudgets.FirstOrDefault(m => (m.year == year) && (m.monthIndex == mn));
            if (ModelState.IsValid)
            {
                foreach (var item in arthabudget)
                {
                    arthaBudget artha = new arthaBudget();
                    artha.employeeName = TrimText(item.employeeName);
                    artha.khaSiNa3 = item.khaSiNa3;
                    artha.khaSiNa4 = item.khaSiNa4;
                    artha.nepDate = nepDate;
                    artha.year=year;
                    artha.month = ((month)mn).ToString();
                    artha.monthIndex = mn;
                    artha.fyId = Convert.ToInt32(col["fyId"]);
                    db.arthaBudgets.Add(artha);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "Record Not Inserted.Please Try Again";
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            return View(arthabudget);
        }

        // GET: /ArthaBudget/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            arthaBudget arthabudget = db.arthaBudgets.Find(id);
            if (arthabudget == null)
            {
                return HttpNotFound();
            }
           // var objListArtha = db.arthaBudgets.Where(m => m.fyId == arthabudget.fyId && m.monthIndex == arthabudget.monthIndex).ToList();
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", arthabudget.fyId);
            return View(arthabudget);
           // return View(objListArtha);
        }

        // POST: /ArthaBudget/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(arthaBudget arthabudget)
        {
            if (ModelState.IsValid)
            {
                string year = getYear(arthabudget.fyId, arthabudget.monthIndex);
                var objArtha = db.arthaBudgets.Find(arthabudget.arthaBudgetId);
                objArtha.employeeName =TrimText(arthabudget.employeeName);
                objArtha.khaSiNa3 = arthabudget.khaSiNa3;
                objArtha.khaSiNa4 = arthabudget.khaSiNa4;
                objArtha.monthIndex = arthabudget.monthIndex;
                objArtha.year = year;
                objArtha.month = ((month)arthabudget.monthIndex).ToString();
                objArtha.nepDate = arthabudget.nepDate;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", arthabudget.fyId);
            return View(arthabudget);
        }

        // GET: /ArthaBudget/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            arthaBudget arthabudget = db.arthaBudgets.Find(id);
            if (arthabudget == null)
            {
                return HttpNotFound();
            }
            return View(arthabudget);
        }

        // POST: /ArthaBudget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            arthaBudget arthabudget = db.arthaBudgets.Find(id);
            db.arthaBudgets.Remove(arthabudget);
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



        [HttpPost]
        public JsonResult getData()
        {
            var uniqArthaYear = from d in db.arthaBudgets
                                group d by new { d.year }
                                    into mygroup
                                    select mygroup.FirstOrDefault();

            var uniqArtha = from d in db.arthaBudgets
                            group d by new { d.monthIndex }
                                into mygroup
                                select mygroup.FirstOrDefault();

            return Json(uniqArtha);

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

        public string UnicodeToString(string data)
        {
            string str = "";
            foreach (char c in data)
            {
                var charCode = "U+" + ((int)c).ToString("X4");
                switch (charCode)
                {
                    case "U+0966":
                        str += "0";
                        break;
                    case "U+0967":
                        str += "1";
                        break;
                    case "U+0968":
                        str += "2";
                        break;
                    case "U+0969":
                        str += "3";
                        break;
                    case "U+096A":
                        str += "4";
                        break;
                    case "U+096B":
                        str += "5";
                        break;
                    case "U+096C":
                        str += "6";
                        break;
                    case "U+096D":
                        str += "7";
                        break;
                    case "U+096E":
                        str += "8";
                        break;
                    case "U+096F":
                        str += "9";
                        break;
                    default:
                        str += c;
                        break;
                }
            }
            return str;
        }
        public string getYear(int fyId, int monthIndex)
        {
            var objFiscalYear = db.fisYears.Find(fyId);
            string fisYear = UnicodeToString(objFiscalYear.nepFy);
            string year = null;
            if (monthIndex >= 9)
            {
                year = StringToUnicode((int.Parse(fisYear.Split('/')[0]) + 1).ToString());
            }
            else
            {
                year = StringToUnicode((int.Parse(fisYear.Split('/')[0])).ToString());
            }
            return year;
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
