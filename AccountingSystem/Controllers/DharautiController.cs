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
using System.Text.RegularExpressions;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class DharautiController : Controller
    {
        private AccountContext db = new AccountContext();
        private MonthToIndex MI = new MonthToIndex();
        //
        // GET: /Dharauti/
        public ActionResult IndexJornal()
        {
            List<jornalEntries> objJornal = new List<jornalEntries>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            ViewBag.type = new List<SelectListItem>(){new SelectListItem { Value = "धरौटी आम्दानी", Text = "धरौटी आम्दानी" },
                new SelectListItem { Value = "धरौटी फिर्ता", Text = "धरौटी फिर्ता" }};
            List<SelectListItem> monthList = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(month)))
            {
                int id = (int)item;
                monthList.Add(new SelectListItem { Value = id.ToString(), Text = ((month)id).ToString() });
            }

            ViewBag.month = new SelectList(monthList, "value", "text");
            return View(objJornal);
        }
        [HttpPost]
        public ActionResult IndexJornal(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
            int mn = Convert.ToInt32(col["month"]);
            string year = getYear(fyId, mn);
            string jornalType = col["Type"];
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            List<SelectListItem> type = new List<SelectListItem>(){new SelectListItem { Value = "धरौटी आम्दानी", Text = "धरौटी आम्दानी" },
                new SelectListItem { Value = "धरौटी फिर्ता", Text = "धरौटी फिर्ता" }};
            ViewBag.type = new SelectList(type, "value", "text", jornalType);
            List<SelectListItem> monthList = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(month)))
            {
                int id = (int)item;
                monthList.Add(new SelectListItem { Value = id.ToString(), Text = ((month)id).ToString() });
            }

            ViewBag.month = new SelectList(monthList, "value", "text", mn);
            var objJornal = db.jornalEntries.Where(m => (m.year == year) && (m.month == ((month)mn).ToString()) && (m.jornalType == jornalType)).ToList();
            if (objJornal.Count == 0)
            {
                List<jornalEntries> objJornals = new List<jornalEntries>();
                ViewBag.ErrorMessage = "No Record found";
                return View(objJornals);
            }
            return View(objJornal.ToList());
        }

        public ActionResult CreateJornal()
        {
            //get jornalEntries Number
            var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "Dharauti");
            Session["jornalEntriesNo"] = StringToUnicode(setNum.number.ToString());


            var setNum1 = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdeshDharauti");
            Session["BhuktaniAdeshNo"] = StringToUnicode(setNum1.number.ToString());

            var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSirsak == "धरौटी");
            // ViewBag.khaSiNaId = new SelectList(db.khaSiNas, "khaSiNaId", "KhaSirsak");
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", objBaUSiNa.baUSiNaId);
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            List<jornalEntries> jornalEntries = new List<jornalEntries> { new jornalEntries { jornalEntriesId = 0, baUSiNaId = 1, fyId = 1 } };
            return View(jornalEntries);
        }

        // POST: /JornalEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateJornal(List<jornalEntries> jornalentries, FormCollection col)
        {
            int baUSiNaId;
            try
            {
                baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
                int mn = Convert.ToInt32(col["month"]);
                int fyId = Convert.ToInt32(col["fyId"]);
                string jornalNo = col["jornalNo"];
                DateTime enDate = Convert.ToDateTime(col["enDate"]);
                string nepDateStr = col["nepDate"].Replace('-', '/');
                string note = TrimText(col["note"]);
                string type = TrimText(col["Type"]);
                string year = getYear(fyId, mn);
                decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"].ToString());
                //string year = StringToUnicode(date.Year.ToString());
                if (ModelState.IsValid)
                {
                    foreach (var item in jornalentries)
                    {
                        //Add Jornal
                        jornalEntries jornal = new jornalEntries();

                        jornal.jornalNo = TrimText(jornalNo);
                        jornal.baUSiNaId = baUSiNaId;
                        jornal.jornalType = type;
                        jornal.month = ((month)mn).ToString();
                        jornal.nepDate = DateTime.Now;
                        jornal.nepDateStr = nepDateStr;
                        jornal.enDate = enDate;
                        jornal.note = TrimText(note);
                        jornal.deCre = item.deCre;
                        jornal.debit = Math.Round(item.debit, 2);
                        jornal.credit = Math.Round(item.credit, 2);
                        jornal.bibaran = TrimText(item.bibaran);
                        jornal.hisabNo = TrimText(item.hisabNo);
                        jornal.khaPaNo = TrimText(item.khaPaNo);
                        jornal.sanketNo = TrimText(item.sanketNo);
                        jornal.year = year;
                        jornal.monthIndex = MI.change(nepDateStr);
                        jornal.fyId = fyId;
                        jornal.chequeRakam = chequeRakam;
                        db.jornalEntries.Add(jornal);
                    }
                    db.SaveChanges();

                    //update JornalEntries Number
                    var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "Dharauti");
                    setNum.faramName = setNum.faramName;
                    setNum.number = setNum.number + 1;
                    db.SaveChanges();

                    if (type == "धरौटी फिर्ता")
                    {
                        int rowNo = Convert.ToInt32(col["RowCount"]);

                        for (int i = 0; i <= rowNo; i++)
                        {
                            //Add Bhuktani Adesh
                            bhuktaniAdesh objBhuktani = new bhuktaniAdesh();
                            objBhuktani.baUSiNaId = baUSiNaId;
                            objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                            objBhuktani.jornalKharchaNo = TrimText(jornalNo);//jornalKharchaNo as jornalBhuktaniAdeshNo
                            objBhuktani.rakam = Math.Round(Convert.ToDecimal(col[i + "rakam"]), 2);
                            objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                            objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                            objBhuktani.nepDate = DateTime.Now;
                            objBhuktani.nepDateStr = nepDateStr;// col[i + "nepDate"].Replace('-', '/');
                            objBhuktani.enDate = enDate;
                            objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                            objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                            objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                            objBhuktani.jornalNikasiNo = "०";
                            objBhuktani.year = year;
                            objBhuktani.month = ((month)mn).ToString();
                            objBhuktani.bhuktaniType = "भुक्तानी आदेश धरौटी";
                            objBhuktani.monthIndex = MI.change(nepDateStr);
                            objBhuktani.fyId = fyId;
                            objBhuktani.panNo = TrimText(col[i + "panNo"]);
                            db.bhuktaniAdeshs.Add(objBhuktani);

                        }
                        db.SaveChanges();

                        //update JornalEntries Number
                        var setNum1 = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdeshDharauti");
                        setNum1.faramName = setNum1.faramName;
                        setNum1.number = setNum1.number + 1;
                        db.SaveChanges();

                    }
                    else if (type == "धरौटी आम्दानी")
                    {
                        int rowNo = Convert.ToInt32(col["RowCount2"]);

                        for (int i = 0; i <= rowNo; i++)
                        {
                            //Add DharautiAmdani
                            dharautiAmdani objAmdani = new dharautiAmdani();
                            objAmdani.nepDate = col[i + "nepDate"].Replace('-', '/');
                            objAmdani.dharautiRakhne = col[i + "dharautiRakhne"];
                            objAmdani.billNo = col[i + "billNo"];
                            objAmdani.fyId = fyId;
                            objAmdani.billRakamNoVat = Convert.ToDecimal(col[i + "billRakamNoVat"]);
                            objAmdani.VatRakam = Convert.ToDecimal(col[i + "vatRakam"]);
                            objAmdani.dharautiRakam = Convert.ToDecimal(col[i + "dharautiRakam"]);
                            objAmdani.bapat = col[i + "bapat"];
                            objAmdani.jornalNo = jornalNo;
                            objAmdani.monthIndex = MI.change(nepDateStr);
                            db.dharautiAmdanis.Add(objAmdani);
                        }
                        db.SaveChanges();

                    }
                    return RedirectToAction("IndexJornal");
                }
                else
                {
                    ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", baUSiNaId);
                    ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                    return View(jornalentries);
                }
            }

            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex;
                var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSirsak == "धरौटी");
                ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", objBaUSiNa.baUSiNaId);
                int fyId = Convert.ToInt32(col["fyId"]);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                return View(jornalentries);
            }
        }




        public ActionResult EditJornal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            jornalEntries jornalentries = db.jornalEntries.Find(id);
            List<jornalEntries> objJornalEntries = db.jornalEntries.Where(m => (m.jornalNo == jornalentries.jornalNo) && (m.monthIndex == jornalentries.monthIndex) && (m.fyId == jornalentries.fyId) && (m.jornalType == jornalentries.jornalType)).ToList();
            int i = 0;
            var decre = new List<SelectListItem>{ new SelectListItem { Text = "--डे/क्रे--",Value="" },
                         new SelectListItem { Text = "डेबिट", Value = "debit" },
                         new SelectListItem { Text = "क्रेडिट", Value = "credit" } };

            var jornalType = new List<SelectListItem>{ new SelectListItem { Text = "--किसिम--",Value="" },
                         new SelectListItem { Text = "धरौटी आम्दानी", Value = "धरौटी आम्दानी" },
                         new SelectListItem { Text = "धरौटी फिर्ता", Value = "धरौटी फिर्ता" } };

            var selectedType = jornalType.Where(m => m.Value == objJornalEntries[0].jornalType).First();
            selectedType.Selected = true;

            ViewData.Add("jornalType", jornalType);

            foreach (var item in objJornalEntries)
            {
                var selected = decre.Where(x => x.Value == item.deCre).First();
                selected.Selected = true;

                ViewData.Add("[" + i + "].deCre", decre);
                i++;
            }
            if (objJornalEntries == null)
            {
                return HttpNotFound();
            }
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objJornalEntries[0].fyId);
            var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSirsak == "धरौटी");
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", objBaUSiNa.baUSiNaId);

            ViewBag.BhuktnaiBibaran = null;
            ViewBag.RowCount = 0;
            if (jornalentries.jornalType == "धरौटी फिर्ता")
            {
                var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश धरौटी") && (m.jornalKharchaNo == jornalentries.jornalNo) && (m.fyId == jornalentries.fyId) && (m.monthIndex == jornalentries.monthIndex)).ToList();
                ViewBag.BhuktaniBibaran = objBhuktani;
                ViewBag.RowCount = objBhuktani.Count;
                ViewBag.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
            }
            else if (jornalentries.jornalType == "धरौटी आम्दानी")
            {
                var objDharautiAmdani = db.dharautiAmdanis.Where(m => m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex && m.jornalNo == jornalentries.jornalNo).ToList();
                ViewBag.DharautiAmdani = objDharautiAmdani;
                ViewBag.RowCount2 = objDharautiAmdani.Count;

            }

            return View(objJornalEntries);
        }



        // POST: /JornalEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJornal(List<jornalEntries> jornalentries, FormCollection col)
        {
            int mn = Convert.ToInt32(col["month"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            DateTime enDate = Convert.ToDateTime(col["enDate"]);
            string nepDateStr = col["nepDate"].Replace('-', '/');
            string note = TrimText(col["note"]);
            string year = getYear(fyId, mn);
            decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"]);

            int baUSiNaId;
            try
            {
                baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
            }
            catch
            {
                baUSiNaId = 1;
            }
            //string year =TrimText(StringToUnicode(date.Year.ToString()));
            if (ModelState.IsValid)
            {
                foreach (var item in jornalentries)
                {
                    if (item.jornalEntriesId != 0)
                    {
                        jornalEntries objJornal = db.jornalEntries.Find(item.jornalEntriesId);
                        objJornal.jornalNo = TrimText(objJornal.jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = col["jornalType"];
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
                        objJornal.enDate = enDate;
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit, 2);
                        objJornal.credit = Math.Round(item.credit, 2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = TrimText(item.sanketNo);
                        objJornal.year = year;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.chequeRakam = chequeRakam;
                    }
                    else
                    {
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(jornalentries[0].jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = col["jornalType"];
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit, 2);
                        objJornal.credit = Math.Round(item.credit, 2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = TrimText(item.sanketNo);
                        objJornal.year = year;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = chequeRakam;
                        db.jornalEntries.Add(objJornal);
                    }
                }
                db.SaveChanges();

                if (jornalentries[0].jornalType == "धरौटी फिर्ता")
                {
                    int rowNo = Convert.ToInt32(col["RowCount"]);
                    for (int i = 0; i < rowNo; i++)
                    {

                        int bhuktaniAdeshId = Convert.ToInt32(col[i + "bhuktaniAdeshId"]);
                        if (bhuktaniAdeshId != 0)
                        {
                            //Add Bhuktani Adesh
                            bhuktaniAdesh objBhuktani = db.bhuktaniAdeshs.Find(bhuktaniAdeshId);
                            objBhuktani.baUSiNaId = baUSiNaId;
                            objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                            objBhuktani.jornalKharchaNo = jornalentries[0].jornalNo;//jornalKharchaNo as jornalBhuktaniAdeshNo
                            objBhuktani.rakam = Math.Round(Convert.ToDecimal(col[i + "rakam"]));
                            objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                            objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);//paunekoCode as प्यान नं.
                            objBhuktani.nepDate = DateTime.Now;
                            objBhuktani.enDate = enDate;
                            objBhuktani.nepDateStr = nepDateStr;
                            objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                            objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                            objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                            objBhuktani.jornalNikasiNo = "०";
                            objBhuktani.year = year;
                            objBhuktani.month = ((month)mn).ToString();
                            objBhuktani.bhuktaniType = "भुक्तानी आदेश धरौटी";
                            objBhuktani.monthIndex = MI.change(nepDateStr);
                            objBhuktani.panNo = TrimText(col[i + "panNo"]);
                            objBhuktani.fyId = fyId;
                        }
                        else
                        {
                            bhuktaniAdesh objBhuktani = new bhuktaniAdesh();
                            objBhuktani.baUSiNaId = baUSiNaId;
                            objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                            objBhuktani.jornalKharchaNo = jornalentries[0].jornalNo;//jornalKharchaNo as jornalBhuktaniAdeshNo
                            objBhuktani.rakam = Convert.ToDecimal(col[i + "rakam"]);
                            objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                            objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                            objBhuktani.nepDate = DateTime.Now;
                            objBhuktani.enDate = enDate;
                            objBhuktani.nepDateStr = nepDateStr;
                            objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                            objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                            objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                            objBhuktani.jornalNikasiNo = "०";
                            objBhuktani.year = year;
                            objBhuktani.month = ((month)mn).ToString();
                            objBhuktani.bhuktaniType = "भुक्तानी आदेश धरौटी";
                            objBhuktani.monthIndex = MI.change(nepDateStr);
                            objBhuktani.fyId = fyId;
                            objBhuktani.panNo = TrimText(col[i + "panNo"]);
                            db.bhuktaniAdeshs.Add(objBhuktani);
                        }
                    }
                    db.SaveChanges();
                }
                else if (jornalentries[0].jornalType == "धरौटी आम्दानी")
                {
                    int rowNo = Convert.ToInt32(col["RowCount2"]);
                    for (int i = 0; i < rowNo; i++)
                    {
                        int dharautiAmdaniId = Convert.ToInt32(col[i + "dharautiAmdaniId"]);
                        //Add Bhuktani Adesh
                        if (dharautiAmdaniId != 0)
                        {
                            dharautiAmdani objdharauti = db.dharautiAmdanis.Find(dharautiAmdaniId);
                            objdharauti.nepDate = col[i + "nepDate"].Replace('-', '/');
                            objdharauti.dharautiRakhne = col[i + "dharautiRakhne"];
                            objdharauti.billNo = col[i + "billNo"];
                            objdharauti.fyId = fyId;
                            objdharauti.billRakamNoVat = Convert.ToDecimal(col[i + "billRakamNoVat"]);
                            objdharauti.VatRakam = Convert.ToDecimal(col[i + "vatRakam"]);
                            objdharauti.dharautiRakam = Convert.ToDecimal(col[i + "dharautiRakam"]);
                            objdharauti.bapat = col[i + "bapat"];
                            objdharauti.jornalNo = jornalentries[0].jornalNo;
                            objdharauti.monthIndex = MI.change(nepDateStr);
                        }
                        else
                        {
                            dharautiAmdani objdharauti = new dharautiAmdani();
                            objdharauti.nepDate = col[i + "nepDate"].Replace('-', '/');
                            objdharauti.dharautiRakhne = col[i + "dharautiRakhne"];
                            objdharauti.billNo = col[i + "billNo"];
                            objdharauti.fyId = fyId;
                            objdharauti.billRakamNoVat = Convert.ToDecimal(col[i + "billRakamNoVat"]);
                            objdharauti.VatRakam = Convert.ToDecimal(col[i + "vatRakam"]);
                            objdharauti.dharautiRakam = Convert.ToDecimal(col[i + "dharautiRakam"]);
                            objdharauti.bapat = col[i + "bapat"];
                            objdharauti.jornalNo = jornalentries[0].jornalNo;
                            objdharauti.monthIndex = MI.change(nepDateStr);
                            db.dharautiAmdanis.Add(objdharauti);
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("IndexJornal");
            }
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", baUSiNaId);
            // var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", jornalentries[0].fyId);

            return View(jornalentries);
        }




        public ActionResult maskewari()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        public ActionResult DharautiMaskewari()
        {
            return RedirectToAction("maskewari");
        }


        [HttpPost]
        public ActionResult DharautiMaskewari(int fyId, int mn)
        {
            string selYear = getYear(fyId, mn);
            if (fyId == 0)
            {
                TempData["ErrorMessage"] = "Year not selected";
                return RedirectToAction("maskewari");
            }
            dharautiMaskewari objMaskewari = new dharautiMaskewari();
            string month = ((month)mn).ToString();
            // string selYear =TrimText( StringToUnicode(year));
            var objBaUSiNa = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "धरौटी");
            var objFy = db.fisYears.Find(fyId);
            var JonrnalAmdaniThisMonth = db.jornalEntries.Where(m => m.baUSiNaId == objBaUSiNa.baUSiNaId && m.jornalType == "धरौटी आम्दानी" && m.fyId == fyId && m.monthIndex == mn).ToList();
            var jonrnalFirtaThisMonth = db.jornalEntries.Where(m => m.baUSiNaId == objBaUSiNa.baUSiNaId && m.jornalType == "धरौटी फिर्ता" && m.fyId == fyId && m.monthIndex == mn).ToList();
            var objJornalAmdani = db.jornalEntries.Where(m => m.baUSiNaId == objBaUSiNa.baUSiNaId && m.jornalType == "धरौटी आम्दानी" && m.fyId == fyId && m.monthIndex <= mn).ToList();
            var objJornalFirta = db.jornalEntries.Where(m => m.baUSiNaId == objBaUSiNa.baUSiNaId && m.jornalType == "धरौटी फिर्ता" && m.fyId == fyId && m.monthIndex <= mn).ToList();
            if (objJornalAmdani.Count == 0 || objJornalFirta.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record found.";
                return RedirectToAction("maskewari");
            }
            decimal remainDharauti = objJornalAmdani.Where(m => m.monthIndex <= mn).Sum(m => m.debit) - objJornalFirta.Where(m => m.monthIndex <= mn).Sum(m => m.debit);
            objMaskewari.prevMaujdat = objJornalAmdani.Where(m => m.monthIndex < mn).Sum(m => m.debit) - objJornalFirta.Where(m => m.monthIndex < mn).Sum(m => m.debit);
            objMaskewari.currentMaujdat = objJornalAmdani.Where(m => m.monthIndex == mn).Sum(m => m.debit);
            objMaskewari.dharautiFirta = objJornalFirta.Where(m => m.monthIndex == mn).Sum(m => m.debit);
            objMaskewari.shyhaDharauti = 0;
            objMaskewari.bakidharauti = remainDharauti;
            objMaskewari.nagadBaki = 0;
            objMaskewari.neRaBank = remainDharauti;
            objMaskewari.bankBakiMaujdat = remainDharauti;
            objMaskewari.year = selYear;
            objMaskewari.monthIndex = mn;
            objMaskewari.month = ((month)mn).ToString();
            objMaskewari.nepFy = objFy.nepFy;
            return View(objMaskewari);
        }

        public ActionResult bhuktaniAdesh()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            // ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var uniqDharautiBhuktani = from d in db.bhuktaniAdeshs
                                       where d.bhuktaniType == "भुक्तानी आदेश धरौटी" && d.fyId == objFiscalYear.fyId
                                       group d by new { d.bhuktaniAdeshNo }
                                           into mygroup
                                           select mygroup.FirstOrDefault();
            if (uniqDharautiBhuktani.ToList().Count == 0)
            {
                uniqDharautiBhuktani = null;
                ViewBag.bhuktaniAdeshNo = null;
                ViewBag.ErrorMessage = "No Record Found";
            }
            else
            {
                var dharautiBhuktaniList = (from d in uniqDharautiBhuktani
                                            select new
                                            {
                                                label = d.bhuktaniAdeshNo + "(" + d.fiscalYear.nepFy + "," + d.month + ")",
                                                value = d.bhuktaniAdeshNo
                                            }).ToList();
                ViewBag.bhuktaniAdeshNo = new SelectList(dharautiBhuktaniList.ToList(), "label", "value");
            }


            return View();
        }


        public ActionResult DharautiBhuktaniAdesh()
        {
            return RedirectToAction("bhuktaniAdesh");
        }


        [HttpPost]
        public ActionResult DharautiBhuktaniAdesh(string bhuktaniadeshNo)
        {
            if (bhuktaniadeshNo == null)
            {
                TempData["ErrorMessage"] = "BhuktaniAdesh No. not seleted.";
                return RedirectToAction("bhuktaniAdesh");
            }
            var objBhuktani = db.bhuktaniAdeshs.Where(m => m.bhuktaniAdeshNo == bhuktaniadeshNo && m.bhuktaniType == "भुक्तानी आदेश धरौटी").ToList();
            if (objBhuktani.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record found";
                return RedirectToAction("bhuktaniAdesh");
            }
            var objFy = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.nepFy = objFy.nepFy;
            int i = 1;
            foreach (var item in objBhuktani)
            {
                var rakamInWords = NumToWords(item.rakam);
                ViewData.Add(i + "words", rakamInWords);
                i++;
            }
            ViewBag.TotalInWords = NumToWords(objBhuktani.Sum(m => m.rakam));
            return View(objBhuktani);
        }


        public ActionResult DharautiJornal()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }
        [HttpPost]
        public ActionResult DharautiJornal(int fyId, int mn)
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var amdaniJornal = (from d in db.jornalEntries
                                where d.jornalType == "धरौटी आम्दानी" && d.fyId == fyId && d.monthIndex == mn
                                group d by new { d.jornalNo }
                                    into mygroup
                                    select mygroup.FirstOrDefault()).ToList();
            ViewBag.AmdaniJornal = amdaniJornal;
            var firtaJornal = (from d in db.jornalEntries
                               where d.jornalType == "धरौटी फिर्ता" && d.fyId == fyId && d.monthIndex == mn
                               group d by new { d.jornalNo }
                                   into mygroup
                                   select mygroup.FirstOrDefault()).ToList();
            ViewBag.FirtaJornal = firtaJornal;
            if (amdaniJornal.Count == 0 && firtaJornal.Count == 0)
            {
                ViewBag.ErrorMessage = "No Record Found";
            }
            return View();
        }
        public ActionResult DharautiJornalBibaran(string id)
        {
            int jornalId = Convert.ToInt32(id);
            var jornalentries = db.jornalEntries.Find(jornalId);
            var objJornal = db.jornalEntries.Where(m => m.jornalNo == jornalentries.jornalNo && m.jornalType == jornalentries.jornalType && m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex).ToList();

            if (objJornal.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found for selected month and year.";
                return RedirectToAction("DharautiJornal");
            }
            if (objJornal[0].jornalType == "धरौटी फिर्ता")
            {
                string jornalNo = objJornal[0].jornalNo;
                var objBhuktaniAdesh = db.bhuktaniAdeshs.Where(m => (m.jornalKharchaNo == jornalNo) && (m.bhuktaniType == "भुक्तानी आदेश धरौटी"));
                ViewBag.bhuktaniAdesh = objBhuktaniAdesh;
                ViewBag.totalRakam = objBhuktaniAdesh.Sum(m => m.rakam);
            }
            else if (objJornal[0].jornalType == "धरौटी आम्दानी")
            {
                string jornalNo = objJornal[0].jornalNo;
                var objDharautiAmdani = db.dharautiAmdanis.Where(m => m.jornalNo == jornalNo && m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex).ToList();
                ViewBag.DharautiAmdani = objDharautiAmdani;
                ViewBag.totalBillRakamNoVat = objDharautiAmdani.Sum(m => m.billRakamNoVat);
                ViewBag.totalVatRakam = objDharautiAmdani.Sum(m => m.VatRakam);
                ViewBag.totalDharautiRakam = objDharautiAmdani.Sum(m => m.dharautiRakam);
            }
            ViewBag.totalAmount = objJornal.Sum(m => m.debit);
            ViewBag.AmountInWords = NumToWords(objJornal.Sum(m => m.debit));
            return View(objJornal);
        }



        public ActionResult Khata()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        public ActionResult DharautiKhata()
        {
            return RedirectToAction("khata");
        }


        [HttpPost]
        public ActionResult DharautiKhata(int fyId, int mn)
        {
            string selYear = getYear(fyId, mn);
            if (fyId == 0)
            {
                TempData["ErrorMessage"] = "Year not selected";
                return RedirectToAction("khata");
            }
            List<dharautiKhata> lstKhata = new List<dharautiKhata>();
            List<dharautiAmdani> lstDharautiAmdani = db.dharautiAmdanis.Where(m => m.fyId == fyId && m.monthIndex == mn).ToList();
            List<bhuktaniAdesh> lstBhuktaniAdesh = db.bhuktaniAdeshs.Where(m => m.bhuktaniType == "भुक्तानी आदेश धरौटी" && m.fyId == fyId && m.monthIndex == mn).ToList();


            if (lstDharautiAmdani.Count == 0 && lstBhuktaniAdesh.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found for Selected Year and Month.";
                return RedirectToAction("khata");
            }


            foreach (var item in lstDharautiAmdani)
            {
                dharautiKhata objKhata = new dharautiKhata();
                objKhata.nepFy = item.fiscalYear.nepFy;
                objKhata.month = ((month)mn).ToString();
                objKhata.nepDate = StringToUnicode(item.nepDate);
                objKhata.jornalNo = item.jornalNo;
                objKhata.serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo));
                objKhata.bibaran = item.dharautiRakhne + "(" + item.bapat + ")";
                objKhata.debit = 0;
                objKhata.credit = item.dharautiRakam;
                objKhata.baki = 0;
                lstKhata.Add(objKhata);
            }


            foreach (var item in lstBhuktaniAdesh)
            {
                dharautiKhata objKhata = new dharautiKhata();
                objKhata.nepFy = item.fiscalYear.nepFy;
                objKhata.month = ((month)mn).ToString();
                objKhata.nepDate = StringToUnicode(item.nepDateStr);
                objKhata.jornalNo = item.jornalKharchaNo;
                objKhata.serialNo = Convert.ToInt32(UnicodeToString(item.jornalKharchaNo));
                objKhata.bibaran = item.bibaran;
                objKhata.debit = item.rakam;
                objKhata.credit = 0;
                objKhata.baki = 0;
                lstKhata.Add(objKhata);

            }

            int prevFyId = getPreviosFyId(fyId);

            lstDharautiAmdani = db.dharautiAmdanis.Where(m => m.fyId == prevFyId).ToList();
            lstBhuktaniAdesh = db.bhuktaniAdeshs.Where(m => m.bhuktaniType == "धरौटी फिर्ता" && m.fyId == prevFyId).ToList();
            fiscalYear objFiscalYear = db.fisYears.FirstOrDefault(m => m.fyId == prevFyId);


            {
                dharautiKhata objKhata = new dharautiKhata();
                objKhata.nepFy = objFiscalYear.nepFy;
                objKhata.month = ((month)mn).ToString();
                objKhata.nepDate = StringToUnicode(objFiscalYear.nepStartYear.ToString()) + "/" + StringToUnicode((mn + 4).ToString()) + "/०१";

                objKhata.serialNo = 0;
                objKhata.bibaran = "अ.ल्या.";
                objKhata.debit = 0;
                objKhata.credit = 0;
                objKhata.baki = lstDharautiAmdani.Sum(m => m.dharautiRakam) - lstBhuktaniAdesh.Sum(m => m.rakam);
                lstKhata.Add(objKhata);

            }



            return View(lstKhata.OrderBy(m => m.serialNo).ToList());
        }




        public ActionResult DeleteJornal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            jornalEntries objJornal = db.jornalEntries.Find(id);
            List<jornalEntries> jornalentries = db.jornalEntries.Where(m => m.jornalNo == objJornal.jornalNo && m.jornalType == objJornal.jornalType && m.fyId == objJornal.fyId && m.monthIndex == objJornal.monthIndex).ToList();
            if (jornalentries == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    foreach (var item in jornalentries)
                    {
                        db.jornalEntries.Remove(item);
                    }
                    db.SaveChanges();
                    TempData["Message"] = "Record Deleted Successfully.";
                    return RedirectToAction("IndexJornal", "Dharauti");
                }
                catch
                {
                    TempData["ErrorMessage"] = "No Record Deleted.";
                    return RedirectToAction("IndexJornal", "Dharauti");
                }
            }
        }


        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            string prefixTrim = prefix.Trim();
            var jornalByehoras = db.byehoras.ToList();
            var byehoras = (from p in jornalByehoras
                            where p.byehoraName.Contains(prefixTrim)
                            select new { label = p.byehoraName + "," + p.hisabNo, value = p.byehoraId }).ToList();
            return Json(byehoras);

        }
        [HttpPost]
        public JsonResult autofill(string value)
        {
            int id = int.Parse(value);
            var myobj = (from byehoras in db.byehoras
                         where byehoras.byehoraId == id
                         select new
                         {
                             khaSiNaId = byehoras.khaSiNaId,
                             byehoraName = byehoras.byehoraName,
                             hisabNo = byehoras.hisabNo,
                             byehoraId = byehoras.byehoraId
                         }).SingleOrDefault();
            return Json(myobj);
        }

        [HttpPost]
        public JsonResult AutoComplete2(string prefix)
        {
            var prefixTrim = prefix.Trim();
            var jornalByehoras = db.byehoras.ToList();
            var byehoras = (from p in jornalByehoras
                            where p.byehoraName.Contains(prefixTrim)
                            select new { label = p.byehoraName + "," + p.hisabNo, value = p.byehoraId }).ToList();
            return Json(byehoras);

        }
        [HttpPost]
        public JsonResult autofill2(string value)
        {
            int id = int.Parse(value);
            var myobj = (from byehoras in db.byehoras
                         where byehoras.byehoraId == id
                         select new
                         {
                             khaSiNaId = byehoras.khaSiNaId,
                             byehoraName = byehoras.byehoraName,
                             hisabNo = byehoras.hisabNo,
                             byehoraId = byehoras.byehoraId
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
                str += "रुपैयाँ र" + " " + nepaliNum[Convert.ToInt32(num2)] + " पैसा मात्र |";
            }
            else
            {
                str += " रुपैयाँ मात्र |";
            }

            return str;
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

        public int getPreviosFyId(int fyId)
        {
            var objFiscalYear = db.fisYears.Find(fyId);
            string fisYear = UnicodeToString(objFiscalYear.nepFy);
            string year = null;
            year = StringToUnicode((int.Parse(fisYear.Split('/')[0]) - 1) + "/०" + (int.Parse(fisYear.Split('/')[1]) - 1).ToString());
            fiscalYear fyear = db.fisYears.FirstOrDefault(m => m.nepFy == year);
            return fyear.fyId;
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