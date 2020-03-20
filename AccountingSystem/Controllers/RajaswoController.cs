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
    public class RajaswoController : Controller
    {
        private AccountContext db = new AccountContext();
        private MonthToIndex MI = new MonthToIndex();
        //
        // GET: /Rajaswo/
        public ActionResult IndexRajaswoJornal()
        {
            List<jornalEntries> objJornal = new List<jornalEntries>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            ViewBag.type = new List<SelectListItem>(){new SelectListItem { Value = "राजश्व आम्दानी", Text = "आम्दानी" },
                new SelectListItem { Value = "राजश्व दाखिला", Text = "दाखिला" }};
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
        public ActionResult IndexRajaswoJornal(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
            int mn = Convert.ToInt32(col["month"]);
            string year = getYear(fyId, mn);
            string jornalType = col["Type"];
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            List<SelectListItem> type = new List<SelectListItem>(){new SelectListItem  { Value = "राजश्व आम्दानी", Text = "आम्दानी" },
                new SelectListItem { Value = "राजश्व दाखिला", Text = "दाखिला" }};
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
                ViewBag.ErrorMessage = "No Record Found";
                List<jornalEntries> objJornals = new List<jornalEntries>();
                return View(objJornals);
            }
            return View(objJornal.ToList());
        }

        public ActionResult IndexRajaswoAsuli()
        {
            List<rajaswoAsuli> objAsuli = new List<rajaswoAsuli>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(objAsuli);
        }
        [HttpPost]
        public ActionResult IndexRajaswoAsuli(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
           // string year =TrimText( StringToUnicode(col["year"]));
            int mn = Convert.ToInt32(col["month"]);
            string year = getYear(fyId, mn);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            var objAsuli = db.rajaswoAsulis.Where(m => (m.year == year) && (m.month == ((month)mn).ToString())).ToList();
            if (objAsuli.Count == 0)
            {
                ViewBag.ErrorMessage = "No Record Found";
                List<rajaswoAsuli> objAsulis = new List<rajaswoAsuli>();
                return View(objAsulis);
            }
            return View(objAsuli.ToList());
        }

        public ActionResult IndexRajaswoDakhila()
        {
            List<rajaswoDakhila> objDakhila = new List<rajaswoDakhila>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(objDakhila);
        }
        [HttpPost]
        public ActionResult IndexRajaswoDakhila(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
           // string year =TrimText( StringToUnicode(col["year"]));
            int mn = Convert.ToInt32(col["month"]);
            string year = getYear(fyId, mn);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            var objDakhila = db.rajaswoDakhilas.Where(m => (m.year == year) && (m.month == ((month)mn).ToString())).ToList();
            if(objDakhila.Count==0)
            {
                ViewBag.ErrorMessage = "No Record Found";
                List<rajaswoDakhila> objDakhilas = new List<rajaswoDakhila>();
                return View(objDakhilas);
            }
            return View(objDakhila.ToList());
        }



        public ActionResult CreateJornal()
        {
            //get jornalEntries Number
            var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "Rajaswo");
            Session["jornalEntriesNo"] = StringToUnicode(setNum.number.ToString());

            //ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo");
            List<jornalEntries> jornalEntries = new List<jornalEntries> { new jornalEntries { jornalEntriesId = 0, baUSiNaId = null,fyId=1 } };
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(jornalEntries);
        }





        // POST: /JornalEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateJornal(List<jornalEntries> jornalentries, FormCollection col)
        {
            try
            {
                int mn = Convert.ToInt32(col["month"]);
                int fyId = Convert.ToInt32(col["fyId"]);
                string jornalNo = col["jornalNo"];
                string jornalType = col["jornalType"];
                DateTime enDate = Convert.ToDateTime(col["enDate"]);
                string nepDateStr = col["nepDate"].Replace('-', '/');
                string note = col["note"];
                string year = getYear(fyId, mn);
               // string year =TrimText( StringToUnicode(date.Year.ToString()));
                if (ModelState.IsValid && jornalType == "Amdani")
                {
                    foreach (var item in jornalentries)
                    {
                        jornalEntries objJornal = new jornalEntries();
                            objJornal.jornalNo =TrimText(jornalNo);
                            objJornal.baUSiNaId = null;
                            objJornal.jornalType = "राजश्व आम्दानी";
                            objJornal.month = ((month)mn).ToString();
                            objJornal.nepDate = DateTime.Now;
                            objJornal.nepDateStr = nepDateStr;
                            objJornal.enDate = enDate;
                            objJornal.note = note;
                            objJornal.deCre = item.deCre;
                            objJornal.debit = Math.Round(item.debit,2);
                            objJornal.credit = Math.Round(item.credit,2);
                            objJornal.bibaran =TrimText(item.bibaran);
                            objJornal.hisabNo = TrimText( item.hisabNo);//hisab no is used as राजश्व शिर्षक नं.
                            objJornal.khaPaNo =TrimText(item.khaPaNo);
                            objJornal.sanketNo = "";
                            objJornal.year = year;
                            objJornal.monthIndex = MI.change(nepDateStr);
                            objJornal.fyId = fyId;
                            objJornal.chequeRakam = 0;
                         db.jornalEntries.Add(objJornal);
                    
                    }
                    //update JornalEntries Number
                    var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "Rajaswo");
                    setNum.faramName = setNum.faramName;
                    setNum.number = setNum.number + 1;
                    db.SaveChanges();


                    return RedirectToAction("IndexRajaswoJornal");
                }

                else if (ModelState.IsValid && jornalType == "Dakhila")
                {
                    foreach (var item in jornalentries)
                    {
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(jornalNo);
                        objJornal.baUSiNaId = null;
                        objJornal.jornalType = "राजश्व दाखिला";
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate =DateTime.Now;
                        objJornal.nepDateStr = col["nepDate"].Replace('-', '/');
                        objJornal.enDate = enDate;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit,2);
                        objJornal.credit = Math.Round(item.credit,2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);//hisab no is used as राजश्व शिर्षक नं.
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = "";
                        objJornal.year = year;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = 0;
                        db.jornalEntries.Add(objJornal);
                    }
                    //update JornalEntries Number
                    var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "Rajaswo");
                    setNum.faramName = setNum.faramName;
                    setNum.number = setNum.number + 1;
                    db.SaveChanges();


                    return RedirectToAction("IndexRajaswoJornal");
                }
                var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
                return View(jornalentries);
            }
            catch
            {
                var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
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
            List<jornalEntries> objJornalEntries = db.jornalEntries.Where(m => (m.jornalNo == jornalentries.jornalNo) && (m.jornalType == jornalentries.jornalType)&&(m.monthIndex==jornalentries.monthIndex)&&(m.fyId==jornalentries.fyId)).ToList();
            int i = 0;
            var decre = new List<SelectListItem>{ new SelectListItem { Text = "--डे/क्रे--",Value="" },
                         new SelectListItem { Text = "डेबिट", Value = "debit" },
                         new SelectListItem { Text = "क्रेडिट", Value = "credit" } };

            var jornalType = new List<SelectListItem>{ new SelectListItem { Text = "--किसिम--",Value="" },
                         new SelectListItem { Text = "आम्दानी", Value = "राजश्व आम्दानी" },
                         new SelectListItem { Text = "दाखिला", Value = "राजश्व दाखिला" } };

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
            string note = col["note"];
            string year = getYear(fyId, mn);
            //string year =TrimText(StringToUnicode(date.Year.ToString()));
            if (ModelState.IsValid)
            {
                foreach (var item in jornalentries)
                {
                    if (item.jornalEntriesId != 0)
                    {
                        jornalEntries objJornal = db.jornalEntries.Find(item.jornalEntriesId);
                        objJornal.jornalNo = TrimText(col["jornalNo"]);
                        objJornal.baUSiNaId = null;
                        objJornal.jornalType = col["jornalType"];
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit,2);
                        objJornal.credit = Math.Round(item.credit,2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);//hisab no is used as राजश्व शिर्षक नं.
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = "";
                        objJornal.year = year;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = 0;
                    }
                    else
                    {
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(col["jornalNo"]);
                        objJornal.baUSiNaId = null;
                        objJornal.jornalType = col["jornalType"];
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit,2);
                        objJornal.credit = Math.Round(item.credit,2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);//hisab no is used as राजश्व शिर्षक नं.
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = "";
                        objJornal.year = year;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = 0;
                        db.jornalEntries.Add(objJornal);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("IndexRajaswoJornal");
            }
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSirsak", jornalentries[0].baUSiNaId);
            
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy",fyId);
            return View(jornalentries);
        }

        public ActionResult RajaswoJornal()
        {
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo", 2);
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        [HttpPost]
        public ActionResult RajaswoJornal(int mn, int fyId)
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var jornalAmdani = db.jornalEntries.Where(m=>(m.jornalType=="राजश्व आम्दानी")&&(m.fyId==fyId) && (m.monthIndex==mn)).ToList();
            var jornalDakhila = db.jornalEntries.Where(m=>(m.jornalType=="राजश्व दाखिला")&&(m.fyId==fyId) && (m.monthIndex==mn)).ToList();
            if(jornalAmdani.Count==0 &&jornalDakhila.Count==0)
            {
                ViewBag.ErrorMessage = "No Record Found";
                return View();
            }


            var amdaniJornal = (from d in db.jornalEntries
                                 where d.jornalType == "राजश्व आम्दानी" && d.fyId == fyId && d.monthIndex == mn
                                 group d by new { d.jornalNo }
                                     into mygroup
                                     select mygroup.FirstOrDefault()).ToList();
            ViewBag.AmdaniJornal = amdaniJornal;
            var dakhilaJornal = (from d in db.jornalEntries
                                where d.jornalType == "राजश्व दाखिला" && d.fyId == fyId && d.monthIndex == mn
                                group d by new { d.jornalNo }
                                    into mygroup
                                    select mygroup.FirstOrDefault()).ToList();
            ViewBag.DakhilaJornal = dakhilaJornal;
            if (amdaniJornal.Count == 0 && dakhilaJornal.Count == 0)
            {
                ViewBag.ErrorMessage = "No Record Found";
            }
            return View();

        }
       
        public ActionResult RajaswoJornalBibaran(string id)
        {
            int jornalId = Convert.ToInt32(id);
            var jornalentries = db.jornalEntries.Find(jornalId);
            //string selYear = getYear(fyId, mn);
           // string selYear =TrimText(StringToUnicode(year));
           // var objJornal = db.jornalEntries.Where(m => m.year == selYear && m.monthIndex == mn && m.jornalType == type).ToList();

            var objJornal = db.jornalEntries.Where(m =>m.jornalNo==jornalentries.jornalNo && m.jornalType==jornalentries.jornalType&& m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex).ToList();

            if (objJornal.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found for selected month and year.";
                return RedirectToAction("RajaswoJornal");
            }
            ViewBag.AmountInWords = NumToWords(objJornal.Sum(m=>m.debit));
            return View(objJornal);
        }





        public ActionResult CreateAsuli()
        {
            List<rajaswoAsuli> asuli = new List<rajaswoAsuli> { new rajaswoAsuli { raId = 0,nepDate=DateTime.Now,fyId=1} };
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(asuli);
        }




        [HttpPost]
        public ActionResult CreateAsuli(List<rajaswoAsuli> asuli, FormCollection col)
        {
            int mn = Convert.ToInt32(col["month"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            string year = getYear(fyId, mn);
            var objFy = db.fisYears.Find(fyId);
            if (ModelState.IsValid)
            {
                foreach (var item in asuli)
                {
                    DateTime date = Convert.ToDateTime(item.nepDate);
                    rajaswoAsuli objAsuli = new rajaswoAsuli();
                    objAsuli.nepDate = DateTime.Now;
                    objAsuli.rasidNo =TrimText( item.rasidNo);
                    objAsuli.bhujauneKoNaam =TrimText( item.bhujauneKoNaam);
                    objAsuli.kha14151 = item.kha14151;
                    objAsuli.janmaMiti = item.janmaMiti;
                    objAsuli.rePrint = item.rePrint;
                    objAsuli.microFilm = item.microFilm;
                    objAsuli.pratilipi = item.pratilipi;
                    objAsuli.lipyantar = item.lipyantar;
                    objAsuli.digitalImage = item.digitalImage;
                    objAsuli.kha14213 = item.kha14213;
                    objAsuli.kha14227 = item.kha14227;
                    objAsuli.month = ((month)mn).ToString();
                    objAsuli.monthIndex = mn;
                    objAsuli.year = year;
                    objAsuli.fyId = fyId;
                    objAsuli.nepFy = objFy.nepFy;
                    objAsuli.nepDateStr = item.nepDateStr;
                    db.rajaswoAsulis.Add(objAsuli);
                }
                db.SaveChanges();

                return RedirectToAction("indexRajaswoAsuli");
            }
             
             ViewBag.fyId = new SelectList(db.fisYears,"fyId","nepFy",fyId);
	
            return View(asuli);
        }


        public ActionResult EditAsuli(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoAsuli asuli = db.rajaswoAsulis.Find(id);

            List<rajaswoAsuli> objAsuli = db.rajaswoAsulis.Where(m => (m.year == asuli.year) && (m.monthIndex == asuli.monthIndex)).ToList();
           
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objAsuli[0].fyId);
            return View(objAsuli);
        }


        [HttpPost]
        public ActionResult EditAsuli(List<rajaswoAsuli> asuli, FormCollection col)
        {
            int mn = Convert.ToInt32(col["month"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            string year = getYear(fyId, mn);
            var objFy = db.fisYears.Find(fyId);
            if (ModelState.IsValid)
            {
                foreach (var item in asuli)
                {
                    if (item.raId != 0)
                    {
                        DateTime date = Convert.ToDateTime(item.nepDate);
                        //string year = StringToUnicode(date.Year.ToString());
                        rajaswoAsuli objAsuli = new rajaswoAsuli();
                        objAsuli = db.rajaswoAsulis.Find(item.raId);
                        objAsuli.nepDate = DateTime.Now;
                        objAsuli.rasidNo = TrimText(item.rasidNo);
                        objAsuli.bhujauneKoNaam = TrimText(item.bhujauneKoNaam);
                        objAsuli.kha14151 = item.kha14151;
                        objAsuli.janmaMiti = item.janmaMiti;
                        objAsuli.rePrint = item.rePrint;
                        objAsuli.microFilm = item.microFilm;
                        objAsuli.pratilipi = item.pratilipi;
                        objAsuli.lipyantar = item.lipyantar;
                        objAsuli.digitalImage = item.digitalImage;
                        objAsuli.kha14213 = item.kha14213;
                        objAsuli.kha14227 = item.kha14227;
                        objAsuli.month = ((month)mn).ToString();
                        objAsuli.monthIndex = mn;
                        objAsuli.year = year;
                        objAsuli.fyId = fyId;
                        objAsuli.nepFy = objFy.nepFy;
                        objAsuli.nepDateStr = item.nepDateStr;
                    }
                    else
                    {
                       // DateTime date = Convert.ToDateTime(item.nepDate);
                        rajaswoAsuli objAsuli = new rajaswoAsuli();
                        objAsuli.nepDate = DateTime.Now;
                        objAsuli.rasidNo = TrimText(item.rasidNo);
                        objAsuli.bhujauneKoNaam = TrimText(item.bhujauneKoNaam);
                        objAsuli.kha14151 = item.kha14151;
                        objAsuli.janmaMiti = item.janmaMiti;
                        objAsuli.rePrint = item.rePrint;
                        objAsuli.microFilm = item.microFilm;
                        objAsuli.pratilipi = item.pratilipi;
                        objAsuli.lipyantar = item.lipyantar;
                        objAsuli.digitalImage = item.digitalImage;
                        objAsuli.kha14213 = item.kha14213;
                        objAsuli.kha14227 = item.kha14227;
                        objAsuli.month = ((month)mn).ToString();
                        objAsuli.monthIndex = mn;
                        objAsuli.year = year;
                        objAsuli.fyId = fyId;
                        objAsuli.nepFy = objFy.nepFy;
                        objAsuli.nepDateStr = item.nepDateStr;
                        db.rajaswoAsulis.Add(objAsuli);
                    }
                }
                db.SaveChanges();

                return RedirectToAction("indexRajaswoAsuli");
            }
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", asuli[0].fyId);
            return View(asuli);
        }


        public ActionResult CreateDakhila()
        {
            List<rajaswoDakhila> dakhila = new List<rajaswoDakhila> { new rajaswoDakhila { rdId = 0, nepDate = DateTime.Now ,fyId=1} };
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(dakhila);
        }



        [HttpPost]
        public ActionResult CreateDakhila(List<rajaswoDakhila> dakhila, FormCollection col)
        {
            int mn = Convert.ToInt32(col["month"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            DateTime date = Convert.ToDateTime(col["nepDate"]);
            string note = col["note"];
            string year = getYear(fyId, mn);
          //  string year = StringToUnicode(col["year"]);
            var objFy = db.fisYears.Find(fyId);
            if (ModelState.IsValid)
            {
                foreach (var item in dakhila)
                {
                    rajaswoDakhila objDakhila = new rajaswoDakhila();
                    objDakhila.nepDate = DateTime.Now;
                    objDakhila.jornalNo = item.jornalNo;
                    objDakhila.kha14151 = item.kha14151;
                    objDakhila.kha14213 = item.kha14213;
                    objDakhila.kha14223 = item.kha14223;
                    objDakhila.kha14227 = item.kha14227;
                    objDakhila.kha15112 = item.kha15112;
                    objDakhila.year = year;
                    objDakhila.month = ((month)mn).ToString();
                    objDakhila.monthIndex = mn;
                    objDakhila.fyId = fyId;
                    objDakhila.nepFy = objFy.nepFy;
                    objDakhila.nepDateStr = item.nepDateStr;
                    db.rajaswoDakhilas.Add(objDakhila);
                }
                db.SaveChanges();

                return RedirectToAction("indexRajaswoDakhila");
            }
             
            ViewBag.fyId = new SelectList(db.fisYears,"fyId","nepFy",fyId);
            return View(dakhila);
        }



        public ActionResult EditDakhila(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoDakhila dakhila = db.rajaswoDakhilas.Find(id);

            List<rajaswoDakhila> objDakhila = db.rajaswoDakhilas.Where(m => (m.year == dakhila.year) && (m.monthIndex == dakhila.monthIndex)).ToList();
            ViewBag.fyId = new SelectList(db.fisYears,"fyId","nepFy",objDakhila[0].fyId);
            return View(objDakhila);
        }



        [HttpPost]
        public ActionResult EditDakhila(List<rajaswoDakhila> dakhila, FormCollection col)
        {
            int mn = Convert.ToInt32(col["month"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            DateTime date = Convert.ToDateTime(col["nepDate"]);
            string note = col["note"];
            string year = getYear(fyId, mn);
            //string year = StringToUnicode(col["year"]);
            var objFy = db.fisYears.Find(fyId);
            if (ModelState.IsValid)
            {
                foreach (var item in dakhila)
                {
                    if (item.rdId != 0)
                    {
                        rajaswoDakhila objDakhila = new rajaswoDakhila();
                        objDakhila = db.rajaswoDakhilas.Find(item.rdId);
                        objDakhila.nepDate = item.nepDate;
                        objDakhila.jornalNo = item.jornalNo;
                        objDakhila.kha14151 = item.kha14151;
                        objDakhila.kha14213 = item.kha14213;
                        objDakhila.kha14223 = item.kha14223;
                        objDakhila.kha14227 = item.kha14227;
                        objDakhila.kha15112 = item.kha15112;
                        objDakhila.year = year;
                        objDakhila.month = ((month)mn).ToString();
                        objDakhila.monthIndex = mn;
                        objDakhila.fyId = fyId;
                        objDakhila.nepFy = objFy.nepFy;
                        objDakhila.nepDateStr = item.nepDateStr;
                    }
                    else
                    {
                        rajaswoDakhila objDakhila = new rajaswoDakhila();
                        objDakhila.nepDate = DateTime.Now;
                        objDakhila.jornalNo = item.jornalNo;
                        objDakhila.kha14151 = item.kha14151;
                        objDakhila.kha14213 = item.kha14213;
                        objDakhila.kha14223 = item.kha14223;
                        objDakhila.kha14227 = item.kha14227;
                        objDakhila.kha15112 = item.kha15112;
                        objDakhila.year = year;
                        objDakhila.month = ((month)mn).ToString();
                        objDakhila.monthIndex = mn;
                        objDakhila.fyId = fyId;
                        objDakhila.nepFy = objFy.nepFy;
                        objDakhila.nepDateStr = item.nepDateStr;
                        db.rajaswoDakhilas.Add(objDakhila);
                    }
                }
                db.SaveChanges();

                return RedirectToAction("indexRajaswoDakhila");
            }
          
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", dakhila[0].fyId);
            return View(dakhila);
        }








        public ActionResult MasikPratibedan()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        public ActionResult rajaswoPratibedan()
        {
            return RedirectToAction("MasikPratibedan");
        }

        [HttpPost]
        public ActionResult rajaswoPratibedan(int fyId, int mn)
        {
           string selYear = getYear(fyId, mn);
           // string selYear = StringToUnicode(year);
            string month = ((month)mn).ToString();
            var objFy = db.fisYears.FirstOrDefault(m => m.status == true);
            var objRajaswoSirsak = db.rajaswoSirsaks.ToList();
            var objRajaswoDakhila = db.rajaswoDakhilas.Where(m => (m.fyId == fyId) && (m.monthIndex <= mn)).ToList();
            var JornalAmdaniThisMonth = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex == mn) && (m.jornalType == "राजश्व आम्दानी")).ToList();
            var JornalDakhilaThisMonth = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex == mn) && (m.jornalType == "राजश्व दाखिला")).ToList();
            var objJornalAmdani = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex <= mn) && (m.jornalType == "राजश्व आम्दानी")).ToList();
            var objJornalDakhila = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex <= mn) && (m.jornalType == "राजश्व दाखिला")).ToList();
            if (JornalAmdaniThisMonth.Count == 0 || JornalDakhilaThisMonth.Count==0)
            {
                TempData["ErrorMessage"] = "Record Not Found.";
                return RedirectToAction("MasikPratibedan");
            }
            List<rajaswoMasikPratibedan> objPratibedan = new List<rajaswoMasikPratibedan>();
                foreach (var item in objRajaswoSirsak)
                {
                    rajaswoMasikPratibedan objMasik = new rajaswoMasikPratibedan();
                    objMasik.nepFy = objFy.nepFy;
                    objMasik.year = selYear;
                    objMasik.month = month;
                    objMasik.rajaswoSirsak = item.sirsakNo;
                    objMasik.amdaniBargikaran = item.sirsak;
                    objMasik.uptoPrevMonth = objJornalAmdani.Where(m => (m.hisabNo == item.sirsakNo) && (m.monthIndex <mn)).Select(m => m.credit).ToList().Sum();
                    objMasik.totalThisMonth = objJornalAmdani.Where(m => (m.hisabNo == item.sirsakNo) && (m.monthIndex == mn)).Select(m => m.credit).ToList().Sum();
                    objMasik.totalUptoThisMonth = objJornalAmdani.Where(m => (m.hisabNo == item.sirsakNo) && (m.monthIndex <= mn)).Select(m => m.credit).ToList().Sum();
                    objMasik.remarks = "";
                    objMasik.totalBankNagadi = objJornalDakhila.Sum(m => m.credit);

                    if (objJornalAmdani.Where(m => m.monthIndex == mn).Sum(m => m.credit) > objJornalDakhila.Where(m => m.monthIndex == mn).Sum(m => m.credit))
                    {
                        objMasik.nagadMaujdat = objJornalAmdani.Where(m => (m.monthIndex == mn)).Select(m => m.credit).ToList().Sum() - objJornalDakhila.Where(m => (m.monthIndex == mn)).Select(m => m.credit).ToList().Sum();
                    }
                    else if (objJornalAmdani.Where(m => m.monthIndex == mn).Sum(m => m.credit) == objJornalDakhila.Where(m => m.monthIndex == mn).Sum(m => m.credit))
                    {
                        objMasik.nagadMaujdat = 0;
                    }
                    if (objJornalAmdani.Where(m => m.monthIndex == mn).Sum(m => m.credit) < objJornalDakhila.Where(m => m.monthIndex == mn).Sum(m => m.credit))
                    {
                        objMasik.prevNagadiMaujdat = objJornalDakhila.Where(m => (m.monthIndex == mn)).Select(m => m.credit).ToList().Sum() - objJornalAmdani.Where(m =>(m.monthIndex == mn)).Select(m => m.credit).ToList().Sum();
                    }
                    else if (objJornalAmdani.Where(m => m.monthIndex == mn).Sum(m => m.credit) == objJornalDakhila.Where(m => m.monthIndex == mn).Sum(m => m.credit))
                    {
                        objMasik.prevNagadiMaujdat = 0;
                    }
                    objPratibedan.Add(objMasik);
                }
            return View(objPratibedan);
        }



        public ActionResult BankNagadi()
        {
            var uniqJornalEntries = from d in db.jornalEntries
                                    where d.jornalType == "राजश्व आम्दानी"
                                    group d by new { d.jornalNo }
                                        into mygroup
                                        select mygroup.FirstOrDefault();
            ViewBag.nepFy = new SelectList(uniqJornalEntries.ToList(), "nepFy", "nepFy");
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        public ActionResult rajaswoBankNagadi()
        {
            return RedirectToAction("BankNagadi");
        }

        [HttpPost]
        public ActionResult rajaswoBankNagadi(int fyId, int mn)
        {
            var objFy = db.fisYears.FirstOrDefault(m => m.status == true);
            string nepFy = objFy.nepFy;
            string selYear = getYear(fyId, mn);
           // string selYear = StringToUnicode(year);
            int prevMonth = mn - 1;
            string month = ((month)mn).ToString();
            var jornalAmdaniThisMonth = db.jornalEntries.Where(m =>( m.fyId == fyId) && (m.monthIndex == mn) && (m.jornalType == "राजश्व आम्दानी")).ToList();
            var jornalDakhilaThisMonth = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex == mn) && (m.jornalType == "राजश्व दाखिला")).ToList();


            var uniqJornalAmdani = (from d in db.jornalEntries
                                    where d.jornalType == "राजश्व आम्दानी" && d.monthIndex <= mn && d.fyId == fyId
                                    group d by new { d.jornalNo }
                                        into mygroup
                                        select mygroup.FirstOrDefault()).ToList();
            var uniqJornalDakhila = (from d in db.jornalEntries
                                     where d.jornalType == "राजश्व दाखिला" && d.monthIndex <= mn && d.fyId == fyId
                                     group d by new { d.jornalNo }
                                         into mygroup
                                         select mygroup.FirstOrDefault()).ToList();

            List<rajaswoBankNagadi> objBankNagdi = new List<rajaswoBankNagadi>();

            if (jornalAmdaniThisMonth.Count == 0 && jornalDakhilaThisMonth.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found.";
                return RedirectToAction("bankNagadi");
            }

            foreach (var item in uniqJornalAmdani)
            {
                rajaswoBankNagadi objRajaswoBank = new rajaswoBankNagadi();
                var objJornal = db.jornalEntries.Where(m => (m.jornalNo == item.jornalNo) && (m.jornalType == "राजश्व आम्दानी")).ToList();
                objRajaswoBank.nepFy = objFy.nepFy;
                objRajaswoBank.month = item.month;
                objRajaswoBank.monthIndex = item.monthIndex;
                objRajaswoBank.jornalNo = item.jornalNo;
                objRajaswoBank.nepDate = item.nepDate;
                objRajaswoBank.bibaran = item.jornalType;
                objRajaswoBank.nagadiDe = objJornal.Sum(m => m.debit);
                objRajaswoBank.rajswoCre = objJornal.Sum(m => m.debit);
                objBankNagdi.Add(objRajaswoBank);
            }

            foreach (var item in uniqJornalDakhila)
            {
                rajaswoBankNagadi objRajaswoBank = new rajaswoBankNagadi();
                var objJornal = db.jornalEntries.Where(m => m.jornalNo == item.jornalNo && (m.jornalType == "राजश्व दाखिला")).ToList();
                objRajaswoBank.nepFy = objFy.nepFy;
                objRajaswoBank.month = item.month;
                objRajaswoBank.monthIndex = item.monthIndex;
                objRajaswoBank.jornalNo = item.jornalNo;
                objRajaswoBank.nepDate = item.nepDate;
                objRajaswoBank.bibaran = item.jornalType;
                objRajaswoBank.nagadiCre = objJornal.Sum(m => m.debit);
                objRajaswoBank.bankDe = objJornal.Sum(m => m.debit);
                objBankNagdi.Add(objRajaswoBank);
            }

            ViewBag.monthIndex = mn;
            return View(objBankNagdi);
        }





        public ActionResult rajaswoDakhila()
        {
          var objDakhila=  (from d in db.rajaswoDakhilas
             group d by new { d.nepFy }
                 into mygroup
                 select mygroup.FirstOrDefault()).ToList();
            ViewBag.nepFy = new SelectList(objDakhila.ToList(), "nepFy", "nepFy");
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        public ActionResult RajaswoDakhilaBibaran()
        {
            return RedirectToAction("rajaswoDakhila");

        }

        [HttpPost]
        public ActionResult RajaswoDakhilaBibaran(int fyId, int mn)
        {
            if (fyId == 0 && fyId == 0)
            {
                TempData["ErrorMessage"] = "Year not selected.";
                return RedirectToAction("rajaswoDakhila");
             }
            string selYear = getYear(fyId, mn);
            var objFy = db.fisYears.Find(fyId);
           // string selYear = StringToUnicode(year);
            var objDakhilaThisMonth = db.rajaswoDakhilas.Where(m => (m.nepFy == objFy.nepFy) && (m.fyId == fyId) && (m.monthIndex == mn)).ToList();
            var objDakhila = db.rajaswoDakhilas.Where(m => (m.nepFy == objFy.nepFy) && (m.fyId == fyId) && (m.monthIndex <= mn)).ToList();
            if (objDakhilaThisMonth.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found";
               // var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                //ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy",fyId);
                return RedirectToAction("rajaswoDakhila");
            }
            else
            {
                rajaswoDakhila objDakhilaPrev = new rajaswoDakhila();
                List<decimal> dakhilaTotal = new List<decimal>();
                for (int i = 0; i <= 11; i++)
                {
                    if (i <= mn)
                    {
                        decimal total = calculateDakhila(objDakhila, i);
                        dakhilaTotal.Add(total);
                    }
                    else
                    {
                        dakhilaTotal.Add(0);
                    }
                    ViewBag.dakhilaTotal = dakhilaTotal;
                }
                var objRajaswoDakhila = objDakhila.Where(m => m.monthIndex <= mn).ToList();
                ViewBag.monthIndex = mn;
                return View(objRajaswoDakhila);
            }

        }





        public ActionResult rajaswoAsuli()
        {

            var uniqRajaswoAsuli = from d in db.rajaswoAsulis
                                   group d by new { d.nepFy }
                                       into mygroup
                                       select mygroup.FirstOrDefault();
            ViewBag.nepFy = new SelectList(uniqRajaswoAsuli.ToList(), "nepFy", "nepFy");

            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        public ActionResult rajaswoAsuliBibaran()
        {
            return RedirectToAction("rajaswoAsuli");
        }


        [HttpPost]
        public ActionResult rajaswoAsuliBibaran(int mn, int fyId)
        {
            if (fyId == 0)
            {
                TempData["ErrorMessage"] = "fiscal Year not selected.";
                return RedirectToAction("rajaswoAsuli");
            }
            string selYear = getYear(fyId, mn);
            // var selYear = StringToUnicode(year);
            var objAsuliThisMonth = db.rajaswoAsulis.Where(m => (m.monthIndex == mn) && (m.fyId == fyId)).ToList();
            var objAsuli = db.rajaswoAsulis.Where(m =>(m.monthIndex <= mn) && (m.fyId == fyId)).ToList();
            if (objAsuliThisMonth.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found";
                return RedirectToAction("rajaswoAsuli");
            }
            else
            {
                ViewBag.monthIndex = mn;
                return View(objAsuli);
            }
        }


        public ActionResult DeleteJornal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            jornalEntries objJornal = db.jornalEntries.Find(id);
            List<jornalEntries> jornalentries = db.jornalEntries.Where(m => m.jornalNo == objJornal.jornalNo && m.jornalType == objJornal.jornalType && m.fyId == objJornal.fyId && m.monthIndex == objJornal.monthIndex).ToList();
            List<bhuktaniAdesh> bhuktaniAdeshs = db.bhuktaniAdeshs.Where(m => m.jornalKharchaNo == objJornal.jornalNo && m.monthIndex == objJornal.monthIndex && m.fyId == objJornal.fyId && m.bhuktaniType == "भुक्तानी आदेश धरौटी").ToList();
            List<dharautiAmdani> dharautiAmdanis = db.dharautiAmdanis.Where(m => m.jornalNo == objJornal.jornalNo && m.monthIndex == objJornal.monthIndex && m.fyId == objJornal.fyId).ToList();
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
                    if (bhuktaniAdeshs.Count > 0)
                    {
                        foreach (var item in bhuktaniAdeshs)
                        {
                            db.bhuktaniAdeshs.Remove(item);
                        }
                    }
                    if (dharautiAmdanis.Count > 0)
                    {
                        foreach (var item in dharautiAmdanis)
                        {
                            db.dharautiAmdanis.Remove(item);
                        }
                    }
                    db.SaveChanges();
                    TempData["Message"] = "Record Deleted Successfully.";
                    return RedirectToAction("IndexRajaswoJornal", "Rajaswo");
                }
                catch
                {
                    TempData["ErrorMessage"] = "No Record Deleted.";
                    return RedirectToAction("IndexRajaswoJornal", "Rajaswo");
                }
            }
        }

        public ActionResult DeleteAsuli(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoAsuli objAsuli = db.rajaswoAsulis.Find(id);
            List<rajaswoAsuli> asulis = db.rajaswoAsulis.Where(m => m.fyId == objAsuli.fyId && m.monthIndex == objAsuli.monthIndex ).ToList();
            if (asulis == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    foreach (var item in asulis)
                    {
                        db.rajaswoAsulis.Remove(item);
                    }
                    db.SaveChanges();
                    TempData["Message"] = "Record Deleted Successfully.";
                    return RedirectToAction("IndexRajaswoAsuli", "Rajaswo");
                }
                catch
                {
                    TempData["ErrorMessage"] = "No Record Deleted.";
                    return RedirectToAction("IndexRajaswoAsuli", "Rajaswo");
                }
            }
        }


        public ActionResult DeleteDakhila(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rajaswoDakhila objDakhila = db.rajaswoDakhilas.Find(id);
            List<rajaswoDakhila> dakhilas = db.rajaswoDakhilas.Where(m => m.fyId == objDakhila.fyId && m.monthIndex == objDakhila.monthIndex).ToList();
            if (dakhilas == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    foreach (var item in dakhilas)
                    {
                        db.rajaswoDakhilas.Remove(item);
                    }
                    db.SaveChanges();
                    TempData["Message"] = "Record Deleted Successfully.";
                    return RedirectToAction("IndexRajaswoDakhila", "Rajaswo");
                }
                catch
                {
                    TempData["ErrorMessage"] = "No Record Deleted.";
                    return RedirectToAction("IndexRajaswoDakhila", "Rajaswo");
                }
            }
        }


        public decimal calculateDakhila(List<rajaswoDakhila> objDakhila, int mn)
        {
            var objDakhilaUptoThisMonth = objDakhila.Where(m => m.monthIndex == mn).ToList();
            decimal total = 0;
            total = objDakhilaUptoThisMonth.Sum(m => m.kha14151) + objDakhilaUptoThisMonth.Sum(m => m.kha14212) + objDakhilaUptoThisMonth.Sum(m => m.kha14213) + objDakhilaUptoThisMonth.Sum(m => m.kha14223) + objDakhilaUptoThisMonth.Sum(m => m.kha14227) + objDakhilaUptoThisMonth.Sum(m => m.kha15112);
            return total;
        }

        public decimal calculateDakhilaThisMonth(List<rajaswoDakhila> objDakhila, int mn, string sirsakNo)
        {
            var objDakhilaUptoThisMonth = objDakhila.Where(m => m.monthIndex == mn).ToList();
            string rajaswoSirsakNo = sirsakNo;
            decimal total = 0;
            switch (rajaswoSirsakNo)
            {
                case "१४१५१":
                    total = objDakhilaUptoThisMonth.Sum(m => m.kha14151);
                    break;
                case "१४२१३":
                    total = objDakhilaUptoThisMonth.Sum(m => m.kha14213);
                    break;
                case "१४२२३":
                    total = objDakhilaUptoThisMonth.Sum(m => m.kha14223);
                    break;
                case "१४२२७":
                    total = objDakhilaUptoThisMonth.Sum(m => m.kha14227);
                    break;
                case "१४२१२":
                    total = objDakhilaUptoThisMonth.Sum(m => m.kha14212);
                    break;
                case "१५११२":
                    total = objDakhilaUptoThisMonth.Sum(m => m.kha15112);
                    break;
                default:
                    total = 0;
                    break;
            }
            return total;
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
                str += "रुपैयाँ र" + " " + nepaliNum[Convert.ToInt32(num2)] + " पैसा ";
            }
            else
            {
                str += " रुपैयाँ ";
            }

            return str;
        }




        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            string prefixTrim = prefix.Trim();
            var jornalByehoras1 = (from m in db.rajaswoSirsaks
                                   select new
                                   {
                                       sirsak = m.sirsak,
                                       sirsakNo = m.sirsakNo
                                   }).ToList();
            var jornalByhoras2 = (from m in db.rajaswoUpasirsaks.Include(m => m.rajaswoSirsak)
                                  select new
                                  {
                                      sirsak = m.upaSirsak,
                                      sirsakNo = m.rajaswoSirsak.sirsakNo
                                  }).ToList();
            var jornalByehoras = jornalByehoras1.Concat(jornalByhoras2).ToList();

            var byehoras = (from p in jornalByehoras
                            where p.sirsak.Contains(prefixTrim)
                            select new { label = p.sirsak, value = p.sirsakNo }).ToList();

            return Json(byehoras);

        }
        [HttpPost]
        public JsonResult autofill(string value)
        {

            var jornalByehoras1 = (from m in db.rajaswoSirsaks
                                   select new
                                   {
                                       sirsak = m.sirsak,
                                       sirsakNo = m.sirsakNo
                                   }).ToList();
            var jornalByhoras2 = (from m in db.rajaswoUpasirsaks.Include(m => m.rajaswoSirsak)
                                  select new
                                  {
                                      sirsak = m.upaSirsak,
                                      sirsakNo = m.rajaswoSirsak.sirsakNo
                                  }).ToList();
            var jornalByehoras = jornalByehoras1.Concat(jornalByhoras2).ToList();

            var myobj = (from m in jornalByehoras
                         where m.sirsak == value
                         select m
                        ).SingleOrDefault();

            return Json(myobj);
        }

        public JsonResult getYearMonth(int fyId,int mn)
        {
            var objRajaswoAsuli = db.rajaswoAsulis.Where(m => m.fyId == fyId && m.monthIndex == mn).ToList();
            if (objRajaswoAsuli.Count > 0)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
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
        public JsonResult CheckYearMonth(string fyId, string monthIndex)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                var objAsuli = db.rajaswoAsulis.Where(m => (m.fyId == fisId) && (m.monthIndex == mn)).ToList();
                if (objAsuli.Count > 0)
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
        public JsonResult CheckYearMonthDakhila(string fyId, string monthIndex)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                var objDakhila = db.rajaswoDakhilas.Where(m => (m.fyId == fisId) && (m.monthIndex == mn)).ToList();
                if (objDakhila.Count > 0)
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