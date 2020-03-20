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
    public class BhuktaniAdeshController : Controller
    {
        private AccountContext db = new AccountContext();
        private MonthToIndex MI = new MonthToIndex();
        // GET: /BhuktaniAdesh/
        public ActionResult Index()
        {
            //var bhuktaniadeshs = db.bhuktaniAdeshs.Where(m => m.bhuktaniType == "भुक्तानी आदेश तलबी").Include(b => b.baUSiNa);
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo", 2);
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            List<bhuktaniAdesh> objTalabiBhuktani = new List<bhuktaniAdesh>();
            return View(objTalabiBhuktani);
            //return View(bhuktaniadeshs.ToList());
        }

        [HttpPost]
        public ActionResult Index(int fyId, int mn)
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var objBhuktaniAdesh = db.bhuktaniAdeshs.Where(m => m.bhuktaniType == "भुक्तानी आदेश तलबी" && m.fyId == fyId && m.monthIndex == mn).ToList();
            if (objBhuktaniAdesh.Count == 0)
            {
                List<bhuktaniAdesh> objTalabiBhuktani = new List<bhuktaniAdesh>();
                ViewBag.ErrorMessage = "No Record Found";
                return View(objTalabiBhuktani);
            }
            else
            {
                return View(objBhuktaniAdesh);
            }
        }

        // GET: /BhuktaniAdesh/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bhuktaniAdesh bhuktaniadesh = db.bhuktaniAdeshs.Find(id);
            if (bhuktaniadesh == null)
            {
                return HttpNotFound();
            }
            return View(bhuktaniadesh);
        }

        public ActionResult CreateTalabi()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var uniqJornal = (from d in db.jornalEntries
                              where d.fyId == objFiscalYear.fyId && d.monthIndex == 0 && d.hisabNo == "२११११" && d.deCre == "debit"
                              group d by new { d.jornalNo }
                                  into mygroup
                                  select mygroup.FirstOrDefault()).ToList();
            var objJornal = (from d in uniqJornal
                             select new { label = d.jornalNo + "(" + d.nepDateStr + ")", value = d.jornalNo }).ToList();
            ViewBag.jornalNo = new SelectList(objJornal, "value", "label");
            return View();
        }


        [HttpGet]
        public ActionResult Create(int fyId, int mn,String jornalNo)
        {
            string selYear = getYear(fyId, mn);
            List<bhuktaniAdesh> objBhuktani = new List<bhuktaniAdesh>();
            var objJornal = db.jornalEntries.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn&&m.deCre=="debit"&& m.hisabNo=="२११११");
            if (objJornal == null)
            {
                TempData["ErrorMessage"] ="गोश्वारा भौचर(खर्च तलबी) not found";
                return RedirectToAction("CreateTalabi"); 
            }
            var objMonthYear = db.yearMonths.FirstOrDefault(m => (m.fyId == fyId) && (m.month == ((month)mn).ToString()));
            //var objTalabi = new List<talabiBharpai>();
            //if(objMonthYear!=null)
            //{
            // objTalabi = db.talabiBharpais.Where(m => m.yearMonthId == objMonthYear.yearMonthId).ToList();
            //}
           
            // if (objTalabi.Count == 0)
            //{
            //    TempData["ErrorMessage"] = "तलबी भरपाई not found for selected fiscalyear and month";
            //    return RedirectToAction("CreateTalabi");
            //}
            //else
            {
                var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
                int baUSiNaId = objBaUSiNa.baUSiNaId;
                var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh"&& m.baUSiNaId==baUSiNaId);
                Session["BhuktaniAdeshNo"] = StringToUnicode(setNum.number.ToString());
               // var objTalabiBharpai = db.talabiBharpais.Where(m => m.yearMonthId == objMonthYear.yearMonthId).Include(m => m.yearMonth).ToList();
                var objJornalList = db.jornalEntries.Where(m => m.fyId == fyId && m.monthIndex == mn && m.jornalNo == jornalNo&& m.jornalType=="खर्च"&&m.bibaran!="एकल खाता कोष").ToList(); 
                objBhuktani = getTalabiBhuktaniList(objJornalList);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                return View(objBhuktani);
            }
          
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<bhuktaniAdesh> bhuktaniadesh, FormCollection col)
        {
            int mn = Convert.ToInt32(bhuktaniadesh[0].monthIndex);
            int fyId = Convert.ToInt32(col["fyId"]);
            DateTime enDate = Convert.ToDateTime(col["enDate"]);
            string nepDateStr = col["nepDate"].Replace('-', '/');
            string year = getYear(fyId, mn);
            var objTalabiBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश तलबी") && (m.fyId == fyId) && (m.monthIndex == mn)).ToList();
            var objJornal = db.jornalEntries.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn&&m.deCre=="debit"&& m.hisabNo=="२११११"&& m.jornalType=="खर्च");
            string jornalNo = "";
            if (objJornal != null)
            {
                jornalNo = objJornal.jornalNo;
            }
            if (ModelState.IsValid)
            {
                if (objTalabiBhuktani.Count > 0)
                {
                    foreach (var item in objTalabiBhuktani)
                    {
                        db.bhuktaniAdeshs.Remove(item);
                    }
                    db.SaveChanges();
                }

                foreach (var item in bhuktaniadesh)
                {
                    if (item.rakam != 0)
                    {
                        bhuktaniAdesh objBhuktnai = new bhuktaniAdesh();
                        objBhuktnai.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                        objBhuktnai.bibaran = TrimText(item.bibaran);
                        objBhuktnai.khaSiNo = TrimText(item.khaSiNo);
                        objBhuktnai.rakam = item.rakam;
                        objBhuktnai.pauneKoNaam = TrimText(item.pauneKoNaam);
                        objBhuktnai.pauneKoCode = TrimText(item.pauneKoCode);
                        objBhuktnai.reamrks = TrimText(item.reamrks);
                        objBhuktnai.nepDate = DateTime.Now;
                        objBhuktnai.enDate = enDate;
                        objBhuktnai.nepDateStr = nepDateStr;
                        objBhuktnai.year = year;
                        objBhuktnai.month = item.month;
                        objBhuktnai.monthIndex = item.monthIndex;
                        objBhuktnai.baUSiNaId = 2;
                        objBhuktnai.bhuktaniType = "भुक्तानी आदेश तलबी";
                        objBhuktnai.fyId = fyId;
                        objBhuktnai.jornalKharchaNo = jornalNo;
                        objBhuktnai.jornalNikasiNo = "०";
                        db.bhuktaniAdeshs.Add(objBhuktnai);
                    }
                }
                db.SaveChanges();

                var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
                int baUSiNaId = objBaUSiNa.baUSiNaId;
                var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh" && m.baUSiNaId == baUSiNaId);
                setNum.faramName = setNum.faramName;
                setNum.number = setNum.number + 1;
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            TempData["ErrorMessage"] = "Record Not Inserted.";
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            return View(bhuktaniadesh);
        }


        [HttpGet]
        public ActionResult Create2(int fyId, int mn, string jornalNo)
        {
            string selYear = getYear(fyId, mn);
            List<bhuktaniAdesh> objBhuktani = new List<bhuktaniAdesh>();
            var objJornal = db.jornalEntries.Where(m => m.jornalNo == jornalNo && m.fyId == fyId && m.monthIndex == mn && m.bibaran != "एकल खाता कोष प्रणाली").ToList();
            var objMonthYear = db.yearMonths.FirstOrDefault(m => (m.fyId == fyId) && (m.month == ((month)mn).ToString()));
             var objTalabi = new List<talabiBharpai>();

            if (objJornal.ToList().Count == 0)
            {
                TempData["ErrorMessage"] = "खर्च गोश्वारा भौचर not found for selected fiscal year, month and jornal No.";
                return RedirectToAction("CreateTalabi");
            }
            else
            {
                var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
                int baUSiNaId = objBaUSiNa.baUSiNaId;
                var objBhuktaniAdesh = db.bhuktaniAdeshs.OrderByDescending(m => m.bhuktaniAdeshId).FirstOrDefault();
                int bhuktaniNo =Convert.ToInt32(UnicodeToString(objBhuktaniAdesh.bhuktaniAdeshNo));
                var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh" && m.baUSiNaId == baUSiNaId);
                Session["BhuktaniAdeshNo"] = StringToUnicode(setNum.number.ToString());
                if (bhuktaniNo == setNum.number)
                {
                    Session["BhuktaniAdeshNo"] = StringToUnicode((setNum.number+1).ToString());
                }
                //  var objTalabiBharpai = db.talabiBharpais.Where(m => m.yearMonthId == objMonthYear.yearMonthId).Include(m => m.yearMonth).ToList();
                objBhuktani = getTalabiBhuktaniList(objJornal);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                return View(objBhuktani);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(List<bhuktaniAdesh> bhuktaniadesh, FormCollection col)
        {
            int mn = Convert.ToInt32(bhuktaniadesh[0].monthIndex);
            int fyId = Convert.ToInt32(col["fyId"]);
            DateTime enDate = Convert.ToDateTime(col["enDate"]);
            string nepDateStr = col["nepDate"].Replace('-', '/');
            string year = getYear(fyId, mn);
            var objTalabiBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश तलबी") && (m.fyId == fyId) && (m.monthIndex == mn)).ToList();
            var objJornal = db.jornalEntries.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn && m.deCre == "debit" && m.hisabNo == "२११११" && m.jornalType == "खर्च");
            string jornalNo = "";
            if (objJornal != null)
            {
                jornalNo = objJornal.jornalNo;
            }
            if (ModelState.IsValid)
            {
                //if (objTalabiBhuktani.Count > 0)
                //{
                //    foreach (var item in objTalabiBhuktani)
                //    {
                //        db.bhuktaniAdeshs.Remove(item);
                //    }
                //    db.SaveChanges();
                //}

                foreach (var item in bhuktaniadesh)
                {
                    if (item.rakam != 0)
                    {
                        bhuktaniAdesh objBhuktnai = new bhuktaniAdesh();
                        objBhuktnai.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                        objBhuktnai.bibaran = TrimText(item.bibaran);
                        objBhuktnai.khaSiNo = TrimText(item.khaSiNo);
                        objBhuktnai.rakam = item.rakam;
                        objBhuktnai.pauneKoNaam = TrimText(item.pauneKoNaam);
                        objBhuktnai.pauneKoCode = TrimText(item.pauneKoCode);
                        objBhuktnai.reamrks = TrimText(item.reamrks);
                        objBhuktnai.nepDate = DateTime.Now;
                        objBhuktnai.enDate = enDate;
                        objBhuktnai.nepDateStr = nepDateStr;
                        objBhuktnai.year = year;
                        objBhuktnai.month = item.month;
                        objBhuktnai.monthIndex = item.monthIndex;
                        objBhuktnai.baUSiNaId = 2;
                        objBhuktnai.bhuktaniType = "भुक्तानी आदेश तलबी";
                        objBhuktnai.fyId = fyId;
                        objBhuktnai.jornalKharchaNo = item.jornalKharchaNo;
                        objBhuktnai.jornalNikasiNo = "०";
                        db.bhuktaniAdeshs.Add(objBhuktnai);
                    }
                }
                db.SaveChanges();

                var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
                int baUSiNaId = objBaUSiNa.baUSiNaId;
                var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh" && m.baUSiNaId == baUSiNaId);
                setNum.faramName = setNum.faramName;
                setNum.number = setNum.number + 1;
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            TempData["ErrorMessage"] = "Record Not Inserted.";
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            return View(bhuktaniadesh);
        }





        // GET: /BhuktaniAdesh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bhuktaniAdesh bhuktaniadesh = db.bhuktaniAdeshs.Find(id);
            if (bhuktaniadesh == null)
            {
                return HttpNotFound();
            }
            List<bhuktaniAdesh> objBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश तलबी") && (m.bhuktaniAdeshNo == bhuktaniadesh.bhuktaniAdeshNo)&&(m.fyId==bhuktaniadesh.fyId)&&(m.monthIndex==bhuktaniadesh.monthIndex)).ToList();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(objBhuktani);
        }

        // POST: /BhuktaniAdesh/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<bhuktaniAdesh> bhuktaniadesh, FormCollection col)
        {
            int mn = Convert.ToInt32(col["month"]);
            int fyId = Convert.ToInt32(col["fyId"]);

            if (ModelState.IsValid)
            {
                foreach (var item in bhuktaniadesh)
                {
                    bhuktaniAdesh objBhuktnai = db.bhuktaniAdeshs.Find(item.bhuktaniAdeshId);
                    objBhuktnai.bibaran = TrimText(item.bibaran);
                    objBhuktnai.khaSiNo = TrimText(item.khaSiNo);
                    objBhuktnai.rakam = item.rakam;
                    objBhuktnai.pauneKoNaam = TrimText(item.pauneKoNaam);
                    objBhuktnai.pauneKoCode = TrimText(item.pauneKoCode);
                    objBhuktnai.reamrks = TrimText(item.reamrks);
                    objBhuktnai.nepDate = Convert.ToDateTime(col["nepDate"]);
                    objBhuktnai.year = TrimText(item.year);
                    objBhuktnai.month = ((month)mn).ToString();
                    objBhuktnai.monthIndex = mn;
                    objBhuktnai.baUSiNaId = 2;
                    objBhuktnai.bhuktaniType = "भुक्तानी आदेश तलबी";
                    objBhuktnai.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                    objBhuktnai.jornalKharchaNo = item.jornalKharchaNo;
                    objBhuktnai.jornalNikasiNo = item.jornalNikasiNo;
                    objBhuktnai.fyId = fyId;
                    objBhuktnai.jornalKharchaNo = item.jornalKharchaNo;
                    objBhuktnai.jornalNikasiNo = item.jornalNikasiNo;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            return View(bhuktaniadesh);
        }

        // GET: /BhuktaniAdesh/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bhuktaniAdesh bhuktaniadesh = db.bhuktaniAdeshs.Find(id);
            List<bhuktaniAdesh> talabiBhuktaniAdesh = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश तलबी") && (m.bhuktaniAdeshNo == bhuktaniadesh.bhuktaniAdeshNo) && (m.fyId == bhuktaniadesh.fyId) && (m.monthIndex == bhuktaniadesh.monthIndex)).ToList();
            if (bhuktaniadesh == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    foreach (var item in talabiBhuktaniAdesh)
                    {
                        db.bhuktaniAdeshs.Remove(item);
                    }
                    db.SaveChanges();
                    TempData["Message"] = "Record Deleted Successfully.";
                    return RedirectToAction("Index", "BhuktaniAdesh");
                }
                catch
                {
                    TempData["ErrorMessage"] = "No Record Deleted.";
                    return RedirectToAction("Index", "BhuktaniAdesh");
                }
            }
        }

        // POST: /BhuktaniAdesh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bhuktaniAdesh bhuktaniadesh = db.bhuktaniAdeshs.Find(id);
            db.bhuktaniAdeshs.Remove(bhuktaniadesh);
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


        public ActionResult TalabiBhuktani()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var uniqBhuktani = (from d in db.bhuktaniAdeshs
                                where d.fyId == objFiscalYear.fyId && d.monthIndex == 0 && ( d.bhuktaniType == "भुक्तानी आदेश तलबी")
                                group d by new { d.bhuktaniAdeshNo }
                                    into mygroup
                                    select mygroup.FirstOrDefault()).ToList();
            var objBhuktanis = (from d in uniqBhuktani
                                select new { label = d.bhuktaniAdeshNo + "(" + d.nepDateStr + ")", value = d.bhuktaniAdeshNo }).ToList();
            ViewBag.bhuktaniAdeshNo = new SelectList(objBhuktanis, "label", "value");
            return View();
        }

        public ActionResult TalabiBhuktaniAdesh()
        {
            return RedirectToAction("TalabiBhuktani");
        }
        [HttpPost]
        public ActionResult TalabiBhuktaniAdesh(int fyId, int month, string bhuktaniAdeshNo)
        {
            List<bhuktaniAdeshTalabi> objBhuktaniTalabi = new List<bhuktaniAdeshTalabi>();
            //string selYear = StringToUnicode(TrimText(year));
            string selYear = getYear(fyId, month);
            decimal total;
            var objFy = db.fisYears.FirstOrDefault(m => m.status == true);
            var objBhuktani = db.bhuktaniAdeshs.Where(m =>(m.bhuktaniAdeshNo==bhuktaniAdeshNo) &&(m.fyId == fyId) && (m.monthIndex == month) && (m.bhuktaniType == "भुक्तानी आदेश तलबी")).ToList();
            if (objBhuktani.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record for selected Bhuktani Adesh No.";
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                return RedirectToAction("talabiBhuktani");
            }
            foreach (var item in objBhuktani)
            {
                bhuktaniAdeshTalabi objTalabi = new bhuktaniAdeshTalabi();
                objTalabi.bhuktaniAdeshNo = item.bhuktaniAdeshNo;
                objTalabi.bibaran = item.bibaran;
                objTalabi.khaSiNo = item.khaSiNo;
                objTalabi.month = item.month;
                objTalabi.nepDate = item.nepDate;
                objTalabi.pauneKoCode = item.pauneKoCode;
                objTalabi.pauneKoNaam = item.pauneKoNaam;
                objTalabi.rakam = item.rakam;
                objTalabi.rakamInWords = NumToWords(item.rakam);
                objTalabi.reamrks = item.reamrks;
                objTalabi.year = selYear;
                objTalabi.jornalNo = item.jornalKharchaNo;
                objTalabi.nepDateStr = item.nepDateStr;
                objBhuktaniTalabi.Add(objTalabi);
            }
            total = objBhuktani.Sum(m => m.rakam);
            ViewBag.totalInWords = NumToWords(total);
            ViewBag.nepFy = objFy.nepFy;
            return View(objBhuktaniTalabi);
        }


        public ActionResult Bhuktani()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        public ActionResult BhuktaniAdeshBibaran()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var uniqBhuktani = (from d in db.bhuktaniAdeshs
                                where d.fyId == objFiscalYear.fyId && d.monthIndex == 0 && (d.bhuktaniType == "भुक्तानी आदेश" || d.bhuktaniType == "भुक्तानी आदेश तलबी")
                                group d by new { d.bhuktaniAdeshNo }
                                    into mygroup
                                    select mygroup.FirstOrDefault()).ToList();
            var objBhuktanis = (from d in uniqBhuktani
                                select new { label = d.bhuktaniAdeshNo + "(" + d.nepDateStr + ")", value = d.bhuktaniAdeshNo }).ToList();
            ViewBag.bhuktaniAdeshNo = new SelectList(objBhuktanis, "label", "value");
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo");
           
            return View();
        }
        [HttpPost]
        public ActionResult BhuktaniAdeshBibaran(int fyId, int month,int baUSiNaId,string bhuktaniAdeshNo)
        {
            List<bhuktaniAdeshTalabi> objBhuktaniTalabi = new List<bhuktaniAdeshTalabi>();
            string selYear = getYear(fyId, month);
            decimal total;
            var objFy = db.fisYears.FirstOrDefault(m => m.status == true);
            var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.fyId == fyId) && (m.monthIndex == month) && (m.bhuktaniAdeshNo == bhuktaniAdeshNo) && (m.bhuktaniType == "भुक्तानी आदेश" ) && (m.baUSiNaId == baUSiNaId)).ToList();
            if (objBhuktani.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record For Fiscalyear and month";
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                return RedirectToAction("Bhuktani");
            }
            foreach (var item in objBhuktani)
            {
                bhuktaniAdeshTalabi objAdesh = new bhuktaniAdeshTalabi();
                objAdesh.bhuktaniAdeshNo = item.bhuktaniAdeshNo;
                objAdesh.bibaran = item.bibaran;
                objAdesh.khaSiNo = item.khaSiNo;
                objAdesh.month = item.month;
                objAdesh.nepDate = item.nepDate;
                objAdesh.pauneKoCode = item.pauneKoCode;
                objAdesh.pauneKoNaam = item.pauneKoNaam;
                objAdesh.rakam = item.rakam;
                objAdesh.rakamInWords = NumToWords(item.rakam);
                objAdesh.reamrks = item.reamrks;
                objAdesh.year = selYear;
                objAdesh.jornalNo = item.jornalKharchaNo;
                objAdesh.panNo = item.panNo;
                objAdesh.BhuPraNo = item.BhuPraNo;
                objAdesh.nepDateStr = item.nepDateStr;
                objBhuktaniTalabi.Add(objAdesh);
            }
            string jornalKharchaNo = objBhuktani[0].jornalKharchaNo;
            var objAyakar = db.ayaKars.Where(m =>m.fyId==fyId&& m.monthIndex == month && m.baUSiNaId == baUSiNaId &&( m.aayaKar>0 || m.paKar>0 ||m.dharauti>0)&& m.jornalKharchaNo==jornalKharchaNo).ToList();
            if (objAyakar.Where(m => m.aayaKar > 0).ToList().Count > 0)
            {
                bhuktaniAdeshTalabi objAdesh = new bhuktaniAdeshTalabi();
                objAdesh.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
                objAdesh.bibaran = "आयकर";
                objAdesh.khaSiNo = "";
                objAdesh.month = objBhuktani[0].month;
                objAdesh.nepDate = objBhuktani[0].nepDate;
                objAdesh.pauneKoCode ="";
                objAdesh.pauneKoNaam = "";
                objAdesh.rakam = objAyakar.Sum(m=>m.aayaKar);
                objAdesh.rakamInWords = NumToWords(objAyakar.Sum(m=>m.aayaKar));
                objAdesh.reamrks ="";
                objAdesh.year = selYear;
                objAdesh.jornalNo ="";
                objAdesh.panNo = "";
                objAdesh.BhuPraNo = "";
                objAdesh.nepDateStr = objBhuktani[0].nepDateStr;
                objBhuktaniTalabi.Add(objAdesh);
            }
            if(objAyakar.Where(m => m.paKar > 0).ToList().Count > 0)
            {
                bhuktaniAdeshTalabi objAdesh = new bhuktaniAdeshTalabi();
                objAdesh.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
                objAdesh.bibaran = "पारिश्रमिक कर";
                objAdesh.khaSiNo = "";
                objAdesh.month = objBhuktani[0].month;
                objAdesh.nepDate = objBhuktani[0].nepDate;
                objAdesh.pauneKoCode = "";
                objAdesh.pauneKoNaam = "";
                objAdesh.rakam = objAyakar.Sum(m => m.paKar);
                objAdesh.rakamInWords = NumToWords(objAyakar.Sum(m => m.paKar));
                objAdesh.reamrks = "";
                objAdesh.year = selYear;
                objAdesh.jornalNo = "";
                objAdesh.panNo = "";
                objAdesh.BhuPraNo = "";
                objAdesh.nepDateStr = objBhuktani[0].nepDateStr;
                objBhuktaniTalabi.Add(objAdesh); 
            }
            if (objAyakar.Where(m => m.dharauti > 0).ToList().Count > 0)
            {
                bhuktaniAdeshTalabi objAdesh = new bhuktaniAdeshTalabi();
                objAdesh.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
                objAdesh.bibaran = "धरौटी";
                objAdesh.khaSiNo = "";
                objAdesh.month = objBhuktani[0].month;
                objAdesh.nepDate = objBhuktani[0].nepDate;
                objAdesh.pauneKoCode = "";
                objAdesh.pauneKoNaam = "";
                objAdesh.rakam = objAyakar.Sum(m => m.dharauti);
                objAdesh.rakamInWords = NumToWords(objAyakar.Sum(m => m.dharauti));
                objAdesh.reamrks = "";
                objAdesh.year = selYear;
                objAdesh.jornalNo = "";
                objAdesh.panNo = "";
                objAdesh.nepDateStr = objBhuktani[0].nepDateStr;
                objAdesh.BhuPraNo = "";
                objBhuktaniTalabi.Add(objAdesh);
            }
            total = objBhuktaniTalabi.Sum(m => m.rakam);
            ViewBag.totalInWords = NumToWords(total);
            ViewBag.nepFy = objFy.nepFy;

            var chaluShirshak = db.baUSiNas.Find(baUSiNaId);
            ViewBag.chaluKharcha = "व.उ.शि.नं. " + chaluShirshak.baUSiNo ;
            ViewBag.shirshak = chaluShirshak.baUSirsak;
            return View(objBhuktaniTalabi);
        }

        public List<bhuktaniAdesh> getTalabiBhuktaniList(List<jornalEntries> objJornal)
        {
            var objTalabi = db.officers.Where(m => m.jobType == "करार" && m.status == "कार्यरत");
            var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
            int baUSiNaId = objBaUSiNa.baUSiNaId;
            var objBhuktan = db.bhuktaniAdeshs.OrderByDescending(m => m.bhuktaniAdeshId).FirstOrDefault();
            int bhuktaniNo = Convert.ToInt32(UnicodeToString(objBhuktan.bhuktaniAdeshNo));
            var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh" && m.baUSiNaId == baUSiNaId);
            int setNumber = setNum.number;
            if (bhuktaniNo == setNum.number)
            {
                setNumber = setNumber + 1;
            }

            var objKharcha = objJornal.ToList();
            //var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh");

            List<bhuktaniAdesh> objBhuktaniAdesh = new List<bhuktaniAdesh>();

            foreach (var item in objKharcha)
            {
                bhuktaniAdesh objBhuktnai = new bhuktaniAdesh();
                objBhuktnai.bhuktaniAdeshNo = StringToUnicode((setNum.number).ToString());
                objBhuktnai.bibaran = item.bibaran;
                objBhuktnai.pauneKoNaam = "";
                objBhuktnai.pauneKoCode = "";
                objBhuktnai.reamrks = "Ac/Payee";
                objBhuktnai.nepDate = DateTime.Now;
                objBhuktnai.year = item.year;
                objBhuktnai.month = item.month;
                objBhuktnai.monthIndex = item.monthIndex;
                objBhuktnai.baUSiNaId = 2;
                objBhuktnai.bhuktaniType = "भुक्तानी आदेश तलबी";
                objBhuktnai.khaSiNo = item.hisabNo;
                objBhuktnai.pauneKoNaam = "";
                objBhuktnai.pauneKoCode = "";
                objBhuktnai.jornalKharchaNo = item.jornalNo;
                if (item.deCre == "debit")
                {
                    objBhuktnai.rakam = item.debit;
                    if (item.bibaran.Contains("तलब"))
                    {
                        objBhuktnai.rakam = item.debit - objJornal.Where(m => m.deCre == "credit").Sum(m => m.credit) + objTalabi.Sum(m => m.saSukar);
                    }
                }
                else
                {

                    objBhuktnai.rakam = item.credit;
                    if (item.bibaran.Contains("सामाजिक सुरक्षा"))
                    {
                        objBhuktnai.rakam = item.credit - objTalabi.Sum(m => m.saSukar);
                    }
                }
                objBhuktaniAdesh.Add(objBhuktnai);
            }



            {
                bhuktaniAdesh objBhuktnai = new bhuktaniAdesh();
                objBhuktnai.bhuktaniAdeshNo = StringToUnicode((setNum.number).ToString());
                objBhuktnai.bibaran = "सामाजिक सुरक्षा कर कट्टी";
                objBhuktnai.pauneKoNaam = "";
                objBhuktnai.pauneKoCode = "";
                objBhuktnai.reamrks = "Ac/Payee";
                objBhuktnai.nepDate = DateTime.Now;
                objBhuktnai.year = objJornal[0].year;
                objBhuktnai.month = objJornal[0].month;
                objBhuktnai.monthIndex = objJornal[0].monthIndex;
                objBhuktnai.baUSiNaId = 2;
                objBhuktnai.bhuktaniType = "भुक्तानी आदेश तलबी";
                objBhuktnai.khaSiNo = objJornal[0].hisabNo;
                objBhuktnai.pauneKoNaam = "";
                objBhuktnai.pauneKoCode = "";
                objBhuktnai.jornalKharchaNo = objJornal[0].jornalNo;
                objBhuktnai.rakam = objTalabi.Sum(m => m.saSukar);
                objBhuktaniAdesh.Add(objBhuktnai);
            }




            //List<string> bhuktaniItems = new List<string> { "तलब", "क.स.कोष", "ना.ल.कोष", "बीमा", "पारिश्रमिक कर कट्टी", "सा.सु.कर कट्टी", "महंगी भत्ता", "अन्य भत्ता", "अन्य सेवा शुल्क", "सा.सु.कर कट्टी", "पोशाक" };

            //foreach (var item in objBhuktaniAdesh)
            //{
            //    switch (item.bibaran)
            //    {
            //        case ("तलब"):
            //            {
            //                item.khaSiNo = "२११११";
            //                item.pauneKoNaam = "कृ.बि.बैंक कर्पोरेट बैं. का. बै.अ. रामशाहपथ हि.नं. १२०५१०३०४१००००५२४";
            //                item.pauneKoCode = "२";
            //                break;
            //            }
            //        case ("क.स.कोष कट्टी"):
            //            {
            //                objBhuktnai.khaSiNo = "२११११";
            //                objBhuktnai.pauneKoNaam = "क.सं. कोष १२०६००१-००१-०००५२४";
            //                objBhuktnai.pauneKoCode = "३";
            //                objBhuktnai.rakam = objTalabi.Sum(m => m.kaSaKoKatti);
            //                break;
            //            }
            //        case ("ना.ल.कोष(तलब)"):
            //            {
            //                objBhuktnai.khaSiNo = "२११११";
            //                objBhuktnai.pauneKoNaam = "ना.ल. कोष १२०६००२-००१-०००५२४";
            //                objBhuktnai.pauneKoCode = "४";
            //                objBhuktnai.rakam = objTalabi.Sum(m => m.naLaKos);
            //                break;
            //            }
            //        case ("बीमा(तलब)"):
            //            {
            //                objBhuktnai.khaSiNo = "२११११";
            //                objBhuktnai.pauneKoNaam = "बीमा कोष १२०६००२-००१-००१५२४";
            //                objBhuktnai.pauneKoCode = "५";
            //                objBhuktnai.rakam = 2*objTalabi.Sum(m => m.bima);
            //                break;
            //            }
            //        case ("पारिश्रमिक कर कट्टी(तलब)"):
            //            {
            //                objBhuktnai.khaSiNo = "२११११";
            //                objBhuktnai.pauneKoNaam = "राजश्व खाता नं. ११११२";
            //                objBhuktnai.pauneKoCode = "७";
            //                objBhuktnai.rakam = objTalabi.Sum(m => m.pariKar);
            //                break;
            //            }
            //        case ("सा.सु.कर कट्टी(तलब)"):
            //            {
            //                objBhuktnai.khaSiNo = "२११११";
            //                objBhuktnai.pauneKoNaam = "राजश्व खाता नं. ११२११";
            //                objBhuktnai.pauneKoCode = "६";
            //                objBhuktnai.rakam = objTalabi.Where(m => m.officer.jobType == "स्थाई").Sum(m => m.saSuKar);// - objTalabi.Where(m => m.officer.jobType == "करार").Sum(m => m.saSuKar);
            //                break;
            //            }
            //        case ("महंगी भत्ता"):
            //            {
            //                objBhuktnai.khaSiNo = "२१११३";
            //                objBhuktnai.pauneKoNaam = "कृ.बि.बैंक कर्पोरेट बैं. का. बै.अ. रामशाहपथ हि.नं. १२०५१०३०४१००००५२४";
            //                objBhuktnai.pauneKoCode = "२";
            //                objBhuktnai.rakam = objTalabi.Sum(m => m.mahangiBhatta);
            //                break;
            //            }
            //        case ("अन्य भत्ता"):
            //            {
            //                objBhuktnai.khaSiNo = "२१११९";
            //                objBhuktnai.pauneKoNaam = "कृ.बि.बैंक कर्पोरेट बैं. का. बै.अ. रामशाहपथ हि.नं. १२०५१०३०४१००००५२४";
            //                objBhuktnai.pauneKoCode = "२";
            //                objBhuktnai.rakam = objTalabi.Sum(m => m.jokhimBhatta);
            //                break;
            //            }
            //        case ("अन्य सेवा शुल्क"):
            //            {
            //                objBhuktnai.khaSiNo = "२२४१२";
            //                objBhuktnai.pauneKoNaam = "कृ.बि.बैंक कर्पोरेट बैं. का. बै.अ. रामशाहपथ हि.नं. १२०५१०३०४१००००५२४";
            //                objBhuktnai.pauneKoCode = "२";
            //                objBhuktnai.rakam = objTalabi.Where(m => m.officer.jobType == "करार").Sum(m => m.suruBimaTotal) - objTalabi.Where(m => m.officer.jobType == "करार").Sum(m => m.saSuKar);// decimal.Round(Convert.ToDecimal(0.01) * objTalabi.Where(m => m.officer.jobType == "करार").Sum(m => m.suruBimaTotal), 2, MidpointRounding.AwayFromZero); 
            //                break;
            //            }
            //        case ("सा.सु.कर कट्टी"):
            //            {
            //                objBhuktnai.khaSiNo = "२११११";
            //                objBhuktnai.pauneKoNaam = "राजश्व खाता नं. ११२११";
            //                objBhuktnai.pauneKoCode = "६";
            //                objBhuktnai.rakam = objTalabi.Where(m => m.officer.jobType == "करार").Sum(m => m.saSuKar);//decimal.Round(Convert.ToDecimal(0.01) * objTalabi.Where(m => m.officer.jobType == "करार").Sum(m => m.suruBimaTotal), 2, MidpointRounding.AwayFromZero);
            //                break;
            //            }
            //        case ("पोशाक"):
            //            {
            //                objBhuktnai.khaSiNo = "२११२१";
            //                objBhuktnai.pauneKoNaam = "";
            //                objBhuktnai.pauneKoCode = "";
            //                objBhuktnai.rakam = 0;
            //                break;
            //            }
            //        default:
            //            {
            //                objBhuktnai.khaSiNo = "२११११";
            //                objBhuktnai.rakam = 0;
            //                break;
            //            }
            //    }
            //    objBhuktaniAdesh.Add(objBhuktnai);
            //}

            return objBhuktaniAdesh;
        }

        [HttpPost]
        public JsonResult getBhuktaniNo(string fyId, string monthIndex,string baUSiNaId)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                int baUSiNoId = Convert.ToInt32(baUSiNaId);
                var uniqBhuktani = (from d in db.bhuktaniAdeshs
                                    where d.fyId == fisId && d.monthIndex == mn && (d.bhuktaniType == "भुक्तानी आदेश" ) && d.baUSiNaId == baUSiNoId
                                    group d by new { d.bhuktaniAdeshNo }
                                        into mygroup
                                        select mygroup.FirstOrDefault()).ToList();
                var objBhuktanis = (from d in uniqBhuktani
                                    select new { label = d.bhuktaniAdeshNo + "(" + d.nepDateStr + ")", value = d.bhuktaniAdeshNo }).ToList();
                return Json(objBhuktanis);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult getBhuktaniNoTalabi(string fyId, string monthIndex)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                var uniqBhuktani = (from d in db.bhuktaniAdeshs
                                    where d.fyId == fisId && d.monthIndex == mn && ( d.bhuktaniType == "भुक्तानी आदेश तलबी") && d.baUSiNaId ==2
                                    group d by new { d.bhuktaniAdeshNo }
                                        into mygroup
                                        select mygroup.FirstOrDefault()).ToList();
                var objBhuktanis = (from d in uniqBhuktani
                                    select new { label = d.bhuktaniAdeshNo + "(" + d.nepDateStr + ")", value = d.bhuktaniAdeshNo }).ToList();
                return Json(objBhuktanis);
            }
            catch
            {
                return Json(null);
            }
        }
        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            string prefixTrim = prefix.Trim();
            var bhuktaniPaune = db.bhuktaniPaunes.ToList();
            var objBhuktaniPaune = (from p in bhuktaniPaune
                                    where p.pauneKoNaam.Contains(prefixTrim)
                                    select new { label = p.pauneKoNaam, value = p.bhuktaniPauneId }).ToList();
            return Json(objBhuktaniPaune);

        }

        private string[] nepaliNum = { "सुन्य", "एक", "दुई", "तीन", "चार", "पाँच", "छ", "सात", "आठ", "नौ", "दस", "एघार", "बाह्र", "तेह्र", "चौध", "पन्ध्र", "सोह्र", "सत्र", "अठाह्र", "उन्नाइस", "बीस", "एकाइस", "बाइस", "तेइस", "चौबीस", "पचीस", "छब्बीस", "सत्ताइस", "अठ्ठाइस", "उनन्तीस", "तीस", "एकतीस", "बतीस", "तेतीस", "चौतीस", "पैतीस", "छतीस", "सरतीस", "अरतीस", "उननचालीस", "चालीस", "एकचालीस", "बयालिस", "तीरचालीस", "चौवालिस", "पैंतालिस", "छयालिस", "सरचालीस", "अरचालीस", "उननचास", "पचास", "एकाउन्न", "बाउन्न", "त्रिपन्न", "चौवन्न", "पच्पन्न", "छपन्न", "सन्ताउन्न", "अन्ठाउँन्न", "उनान्न्साठी ", "साठी", "एकसाठी", "बासाठी", "तीरसाठी", "चौंसाठी", "पैसाठी", "छैसठी", "सत्सठ्ठी", "अर्सठ्ठी", "उनन्सत्तरी", "सतरी", "एकहत्तर", "बहत्तर", "त्रिहत्तर", "चौहत्तर", "पचहत्तर", "छहत्तर", "सत्हत्तर", "अठ्हत्तर", "उनास्सी", "अस्सी", "एकासी", "बयासी", "त्रीयासी", "चौरासी", "पचासी", "छयासी", "सतासी", "अठासी", "उनान्नब्बे", "नब्बे", "एकान्नब्बे", "बयान्नब्बे", "त्रियान्नब्बे", "चौरान्नब्बे", "पंचान्नब्बे", "छयान्नब्बे", "सन्तान्‍नब्बे", "अन्ठान्नब्बे", "उनान्सय" };
         public string NumToWords(decimal nums)
        {
            var num = Convert.ToString(nums);
            string num1 = num.Split('.')[0];
            string num2 = num.Split('.')[1].Substring(0, 2);
            string str = "";
            int charCount = num1.Length;
            if (num1.Length > 10 || num1.Length == 0)
            {
                str = "Error Converting Code";
                return str;
            }

            if (num1.Length != 10)
            {
                for (int i = 0; i <= 10 - charCount - 1; i++)
                {
                    num1 = "0" + num1;
                }
            }

            int[] place = new int[6];
            place[0] = Convert.ToInt32(num1.Substring(1, 1));
            place[1] = Convert.ToInt32(num1.Substring(1, 2));
            place[2] = Convert.ToInt32(num1.Substring(3, 2));
            place[3] = Convert.ToInt32(num1.Substring(5, 2));
            place[4] = Convert.ToInt32(num1.Substring(7, 1));
            place[5] = Convert.ToInt32(num1.Substring(8, 2));
            for (int i = 0; i <= 6 - 1; i++)
            {
                if (place[i] != 0)
                {
                    switch (i)
                    {
                        case (0):
                            {
                                str += nepaliNum[place[i]] + " " + "अरब ";
                                break;
                            }
                        case (1):
                            {
                                str += nepaliNum[place[i]] + " " + "करोड ";
                                break;
                            }
                        case (2):
                            {
                                str += nepaliNum[place[i]] + " " + "लाख ";
                                break;
                            }
                        case (3):
                            {
                                str += nepaliNum[place[i]] + " " + "हजार ";
                                break;
                            }
                        case (4):
                            {
                                str += nepaliNum[place[i]] + " " + "सय ";
                                break;
                            }
                        case (5):
                            {
                                str += nepaliNum[place[i]] + " ";
                                break;
                            }
                        default:
                            {
                                str += " सुन्य ";
                                break;
                            }
                    }
                }
            }
            if (Convert.ToInt32(num2) != 0)
            {
                str += "रुपैयाँ र" + " " + nepaliNum[Convert.ToInt32(num2)] + " पैसा ";
            }
            else
            {
                str += "रुपैयाँ ";
            }

            return str;
        }

        [HttpPost]
        public JsonResult autofill(string value)
        {
            int id = int.Parse(value);
            var myobj = (from b in db.bhuktaniPaunes
                         where b.bhuktaniPauneId == id
                         select new
                         {
                             bhuktaniPauneId = b.bhuktaniPauneId,
                             pauneKoNaam = b.pauneKoNaam,
                             pauneKoCode = b.code,
                             reamrks = "AC/Payee"
                         }).SingleOrDefault();
            return Json(myobj);
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

        [HttpPost]
        public JsonResult checkBhuktaniAdeshNo(string bhuktaniAdeshNo, string fyId)
        {
            try
            {
                string res;
                int fisId = Convert.ToInt32(fyId);
               var objBhuktaniAdesh = db.bhuktaniAdeshs.Where(m => m.bhuktaniAdeshNo == bhuktaniAdeshNo&&m.fyId==fisId).ToList();
                 
                if (objBhuktaniAdesh.Count > 0 && bhuktaniAdeshNo!="")
                {
                    res="NotNull";
                }
                else
                {
                    res="Null";
              
              }
              return Json(res);
            }
            catch
            {
                return Json("NotNull");
            }
        }


        [HttpPost]
        public JsonResult getJornalNo(string fyId, string monthIndex)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                var uniqJornal = (from d in db.jornalEntries
                                  where d.fyId == fisId && d.monthIndex == mn && d.hisabNo == "२११११" && d.deCre == "debit"
                                  group d by new { d.jornalNo }
                                      into mygroup
                                      select mygroup.FirstOrDefault()).ToList();
                var objJornal = (from d in uniqJornal
                                 select new { label = d.jornalNo + "(" + d.nepDateStr + ")", value = d.jornalNo }).ToList();
                return Json(objJornal);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult CheckYearMonthJornalNo(string fyId, string monthIndex, string jornalNo)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);

                var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.fyId == fisId) && (m.monthIndex == mn) && (m.bhuktaniType == "भुक्तानी आदेश तलबी") && m.jornalKharchaNo == jornalNo).ToList();
                if (objBhuktani.Count > 0)
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
        public JsonResult CheckYearMonth(string fyId, string monthIndex)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.fyId == fisId) && (m.monthIndex == mn) && (m.bhuktaniType == "भुक्तानी आदेश तलबी")).ToList();
                if (objBhuktani.Count > 0)
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
    }
}
