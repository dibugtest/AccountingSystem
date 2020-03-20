using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;
using AccountingSystem.ViewModel;
using AccountingSystem.DAL;
using System.Text.RegularExpressions;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class JornalEntriesController : Controller
    {
        private AccountContext db = new AccountContext();

        private MonthToIndex MI = new MonthToIndex();
        // GET: /JornalEntries/
        public ActionResult Index()
        {
            List<jornalEntries> objJornal = new List<jornalEntries>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo");
            ViewBag.type = new List<SelectListItem>(){new SelectListItem { Value = "खर्च", Text = "खर्च" },
                new SelectListItem { Value = "निकासा", Text = " निकासा" }, 
                new SelectListItem { Value = "कट्टी", Text = "कट्टी" }};
            //var mnList = Enum.GetValues(typeof(month));
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
        public ActionResult Index(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
            int mn = Convert.ToInt32(col["month"]);
            int baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
            //ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo",baUSiNaId);
            //string year = TrimText(StringToUnicode(col["year"]));
            string year = getYear(fyId, mn);
            string jornalType = col["Type"];
            //  ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);


            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", baUSiNaId);
            List<SelectListItem> type = new List<SelectListItem>(){new SelectListItem { Value = "खर्च", Text = "खर्च" },
                new SelectListItem { Value = "निकासा", Text = " निकासा" }, 
                new SelectListItem { Value = "कट्टी", Text = "कट्टी" }
            };
            ViewBag.type = new SelectList(type, "value", "text", jornalType); List<SelectListItem> monthList = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(month)))
            {
                int id = (int)item;
                monthList.Add(new SelectListItem { Value = id.ToString(), Text = ((month)id).ToString() });
            }

            ViewBag.month = new SelectList(monthList, "value", "text", mn);

            var objJornal = db.jornalEntries.Where(m => (m.year == year) && (m.month == ((month)mn).ToString()) && (m.jornalType == jornalType) && (m.baUSiNaId == baUSiNaId)).ToList();
            if (objJornal.Count == 0)
            {
                List<jornalEntries> objJornals = new List<jornalEntries>();
                ViewBag.ErrorMessage = "No Record Found";
                return View(objJornals);
            }
            return View(objJornal.ToList());
        }



        public ActionResult IndexPeski()
        {
            List<jornalEntries> objJornal = new List<jornalEntries>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo");
            ViewBag.type = new List<SelectListItem>(){
                new SelectListItem { Value = "पेश्की", Text = "पेश्की"},
            new SelectListItem { Value = "पेश्की फछर्यौट", Text = "पेश्की फछर्यौट" }};
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
        public ActionResult IndexPeski(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
            int mn = Convert.ToInt32(col["month"]);
            int baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
            string year = getYear(fyId, mn);
            string jornalType = col["Type"];
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            ViewBag.baUSiNaId = new SelectList(db.baUSiNas, "baUSiNaId", "baUSiNo", baUSiNaId);
            List<SelectListItem> type = new List<SelectListItem>(){ 
                new SelectListItem { Value = "पेश्की", Text = "पेश्की"},
            new SelectListItem { Value = "पेश्की फछर्यौट", Text = "पेश्की फछर्यौट" }
            };
            ViewBag.type = new SelectList(type, "value", "text", jornalType); List<SelectListItem> monthList = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(month)))
            {
                int id = (int)item;
                monthList.Add(new SelectListItem { Value = id.ToString(), Text = ((month)id).ToString() });
            }

            ViewBag.month = new SelectList(monthList, "value", "text", mn);

            var objJornal = db.jornalEntries.Where(m => (m.year == year) && (m.month == ((month)mn).ToString()) && (m.jornalType == jornalType) && (m.baUSiNaId == baUSiNaId)).ToList();
            if (objJornal.Count == 0)
            {
                List<jornalEntries> objJornals = new List<jornalEntries>();
                ViewBag.ErrorMessage = "No Record Found";
                return View(objJornals);
            }
            return View(objJornal.ToList());
        }


        // GET: /JornalEntries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            jornalEntries jornalentries = db.jornalEntries.Find(id);
            if (jornalentries == null)
            {
                return HttpNotFound();
            }
            return View(jornalentries);
        }

        // GET: /JornalEntries/Create
        public ActionResult CreateKharcha()
        {
            //get jornalEntries Number
            // var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "JornalEntries");
            // Session["jornalEntriesNo"] = StringToUnicode(setNum.number.ToString());


            // var setNum1 = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh");
            //  Session["BhuktaniAdeshNo"] = StringToUnicode(setNum1.number.ToString());


            // ViewBag.khaSiNaId = new SelectList(db.khaSiNas, "khaSiNaId", "KhaSirsak");
            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo");
            List<jornalEntries> jornalEntries = new List<jornalEntries> { new jornalEntries { jornalEntriesId = 0, baUSiNaId = 1, fyId = 1 } };

            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);

            return View(jornalEntries);
        }

        // POST: /JornalEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateKharcha(List<jornalEntries> jornalentries, FormCollection col)
        {
            int baUSiNaId;
            try
            {
                string jornalType = col["jornalType"];
                baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
                int mn = Convert.ToInt32(col["month"]);
                int fyId = Convert.ToInt32(col["fyId"]);
                string jornalNo = TrimText(col["jornalNo"]);
                // DateTime date = Convert.ToDateTime(col["nepDate"]);
                string nepDateStr = col["nepDate"].Replace('-', '/');
                DateTime enDate = Convert.ToDateTime(col["enDate"]);
                string note = col["note"];
                string year = getYear(fyId, mn);
                decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"].ToString());
                // string year = StringToUnicode(date.Year.ToString());


                if (ModelState.IsValid)
                {
                    foreach (var item in jornalentries)
                    {
                        //Add Jornal
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = "खर्च";
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit, 2);
                        objJornal.credit = Math.Round(item.credit, 2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = TrimText(item.sanketNo);
                        objJornal.year = year;
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = chequeRakam;
                        db.jornalEntries.Add(objJornal);
                    }
                    db.SaveChanges();

                    //update JornalEntries Number
                    var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "JornalEntries" && m.baUSiNaId == baUSiNaId);
                    setNum.faramName = setNum.faramName;
                    setNum.number = setNum.number + 1;
                    db.SaveChanges();

                    if (col["jornalTypeTalabi"] == "")
                    {
                        int rowNo = Convert.ToInt32(col["RowCount"]);
                        for (int i = 0; i <= rowNo; i++)
                        {
                            {
                                bhuktaniAdesh objBhuktani = new bhuktaniAdesh();
                                objBhuktani.baUSiNaId = baUSiNaId;
                                objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                                objBhuktani.jornalKharchaNo = TrimText(jornalNo);
                                objBhuktani.rakam = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                                objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                                objBhuktani.nepDate = DateTime.Now;
                                objBhuktani.nepDateStr = nepDateStr;
                                objBhuktani.enDate = enDate;
                                objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                                objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                                objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                                objBhuktani.jornalNikasiNo = "०";
                                objBhuktani.year = year;
                                objBhuktani.month = ((month)mn).ToString();
                                objBhuktani.monthIndex = MI.change(nepDateStr);
                                objBhuktani.bhuktaniType = "भुक्तानी आदेश";
                                objBhuktani.fyId = fyId;
                                objBhuktani.panNo = TrimText(col[i + "panNo"]);
                                objBhuktani.BhuPraNo = TrimText(col[i + "bhuPraNo"]);
                                db.bhuktaniAdeshs.Add(objBhuktani);
                            }
                            {
                                ayaKar objAyaKar = new ayaKar();
                                objAyaKar.firmName = TrimText(col[i + "pauneKoNaam"]);
                                objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                                objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                                objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                                objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                                objAyaKar.bakiPaune = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objAyaKar.fyId = fyId;
                                objAyaKar.monthIndex = MI.change(nepDateStr);
                                objAyaKar.jornalKharchaNo = jornalNo;
                                objAyaKar.jornalKattiNo = "०";
                                objAyaKar.hisabNo = TrimText(col[i + "khaSiNo"]);
                                objAyaKar.baUSiNaId = baUSiNaId;
                                db.ayaKars.Add(objAyaKar);
                            }
                        }

                        db.SaveChanges();
                        int bhuktaniAdeshNo;
                        try
                        {
                            bhuktaniAdeshNo = Convert.ToInt32(UnicodeToString(col["bhuktaniAdeshNo"]));
                        }
                        catch
                        {
                            bhuktaniAdeshNo = 0;
                        }
                        //update bhuktaniadeshno Number
                        var setNum1 = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh" && m.baUSiNaId == baUSiNaId);
                        setNum1.faramName = setNum1.faramName;
                        if (bhuktaniAdeshNo == 0)
                        {
                            setNum1.number = setNum1.number;
                        }
                        else
                        {
                            setNum1.number = bhuktaniAdeshNo;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        var objTalabiBhuktani = db.bhuktaniAdeshs.Where(m => m.fyId == fyId && m.monthIndex == mn && m.bhuktaniType == "भुक्तानी आदेश तलबी").ToList();
                        if (objTalabiBhuktani.Count > 0)
                        {
                            foreach (var item in objTalabiBhuktani)
                            {
                                item.jornalKharchaNo = jornalNo;
                            }
                        }
                        db.SaveChanges();

                    }



                    return RedirectToAction("Index");
                }
                else
                {
                    var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
                    ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", baUSiNaId);
                    ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                    return View(jornalentries);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex;
                var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
                ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo");
                var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
                return View(jornalentries);
            }
        }

        public ActionResult CreateKatti()
        {
            //get jornalEntries Number
            var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "Jornalentries");
            // Session["jornalEntriesNo"] = StringToUnicode(setNum.number.ToString());

            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo");

            List<jornalEntries> jornalEntries = new List<jornalEntries> { new jornalEntries { jornalEntriesId = 0, baUSiNaId = 1, fyId = 1 } };
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(jornalEntries);
        }

        // POST: /JornalEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateKatti(List<jornalEntries> jornalentries, FormCollection col)
        {
            int baUSiNaId;
            try
            {
                int fyId = Convert.ToInt32(col["fyId"]);
                baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
                int mn = Convert.ToInt32(col["month"]);
                string jornalNo = col["jornalNo"];
                //DateTime date = Convert.ToDateTime(col["nepDate"]);
                string note = TrimText(col["note"]);
                DateTime enDate = Convert.ToDateTime(col["enDate"]);
                string nepDateStr = col["nepDate"].Replace('-', '/');
                string year = getYear(fyId, mn);
                decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"]);
                // string year = StringToUnicode(date.Year.ToString());
                if (ModelState.IsValid)
                {
                    foreach (var item in jornalentries)
                    {
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = "कट्टी";
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit, 2);
                        objJornal.credit = Math.Round(item.credit, 2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = TrimText(item.sanketNo);
                        objJornal.year = year;
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = chequeRakam;
                        db.jornalEntries.Add(objJornal);
                    }
                    //update JornalEntries Number
                    var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "JornalEntries" && m.baUSiNaId == baUSiNaId);
                    setNum.faramName = setNum.faramName;
                    setNum.number = setNum.number + 1;
                    db.SaveChanges();

                    if (col["jornalTypeTalabi"] == "")
                    {
                        int rowNo = Convert.ToInt32(col["RowCount"]);
                        for (int i = 0; i <= rowNo; i++)
                        {

                            ayaKar objAyaKar = new ayaKar();
                            objAyaKar.firmName = TrimText(col[i + "firmName"]);
                            objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                            objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                            objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                            objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                            objAyaKar.jornalKattiNo = jornalNo;
                            objAyaKar.jornalKharchaNo = "०";
                            objAyaKar.fyId = fyId;
                            objAyaKar.monthIndex = MI.change(nepDateStr);
                            objAyaKar.baUSiNaId = baUSiNaId;
                            db.ayaKars.Add(objAyaKar);
                        }
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
                ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", baUSiNaId);
                return View(jornalentries);
            }
            catch
            {
                var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
                ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo");
                var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
                return View(jornalentries);
            }
        }





        public ActionResult CreatePeski()
        {
            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo");
            List<jornalEntries> jornalEntries = new List<jornalEntries> { new jornalEntries { jornalEntriesId = 0, baUSiNaId = 1, fyId = 1, enDate = DateTime.Now } };

            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);

            return View(jornalEntries);
        }

        // POST: /JornalEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePeski(List<jornalEntries> jornalentries, FormCollection col)
        {
            int baUSiNaId;
            try
            {
                string jornalType = col["jornalTypeTalabi"];
                string peskiType = col["peskiType"];
                baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
                int mn = Convert.ToInt32(col["month"]);
                int fyId = Convert.ToInt32(col["fyId"]);
                string jornalNo = TrimText(col["jornalNo"]);
                // DateTime date = Convert.ToDateTime(col["nepDate"]);
                string nepDateStr = col["nepDate"].Replace('-', '/');
                DateTime enDate = Convert.ToDateTime(col["enDate"]);
                string note = col["note"];
                string year = getYear(fyId, mn);
                decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"].ToString());
                // string year = StringToUnicode(date.Year.ToString());

                if (ModelState.IsValid)
                {
                    foreach (var item in jornalentries)
                    {
                        //Add Jornal
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = jornalType;
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
                        objJornal.note = note;
                        objJornal.deCre = item.deCre;
                        objJornal.debit = Math.Round(item.debit, 2);
                        objJornal.credit = Math.Round(item.credit, 2);
                        objJornal.bibaran = TrimText(item.bibaran);
                        objJornal.hisabNo = TrimText(item.hisabNo);
                        objJornal.khaPaNo = TrimText(item.khaPaNo);
                        objJornal.sanketNo = TrimText(item.sanketNo);
                        objJornal.year = year;
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.monthIndex = MI.change(nepDateStr);
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = chequeRakam;
                        db.jornalEntries.Add(objJornal);
                    }
                    db.SaveChanges();

                    //update JornalEntries Number
                    var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "JornalEntries" && m.baUSiNaId == baUSiNaId);
                    setNum.faramName = setNum.faramName;
                    setNum.number = setNum.number + 1;
                    db.SaveChanges();

                    if ((col["jornalTypeTalabi"] == "पेश्की फछर्यौट" && peskiType == "पेस्की भन्दा बढी खर्च भएमा") || col["jornalTypeTalabi"] == "पेश्की")
                    {
                        int rowNo = Convert.ToInt32(col["RowCount"]);
                        for (int i = 0; i <= rowNo; i++)
                        {
                            {
                                bhuktaniAdesh objBhuktani = new bhuktaniAdesh();
                                objBhuktani.baUSiNaId = baUSiNaId;
                                objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                                objBhuktani.jornalKharchaNo = TrimText(jornalNo);
                                objBhuktani.rakam = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                                objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                                objBhuktani.nepDate = DateTime.Now;
                                objBhuktani.nepDateStr = nepDateStr;
                                objBhuktani.enDate = enDate;
                                objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                                objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                                objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                                objBhuktani.jornalNikasiNo = "०";
                                objBhuktani.year = year;
                                objBhuktani.month = ((month)mn).ToString();
                                objBhuktani.monthIndex = MI.change(nepDateStr);
                                objBhuktani.bhuktaniType = "भुक्तानी आदेश";
                                objBhuktani.fyId = fyId;
                                objBhuktani.panNo = TrimText(col[i + "panNo"]);
                                objBhuktani.BhuPraNo = TrimText(col[i + "bhuPraNo"]);
                                db.bhuktaniAdeshs.Add(objBhuktani);
                            }
                            {
                                ayaKar objAyaKar = new ayaKar();
                                objAyaKar.firmName = TrimText(col[i + "pauneKoNaam"]);
                                objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                                objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                                objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                                objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                                objAyaKar.bakiPaune = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objAyaKar.fyId = fyId;
                                objAyaKar.monthIndex = MI.change(nepDateStr);
                                objAyaKar.jornalKharchaNo = jornalNo;
                                objAyaKar.jornalKattiNo = "०";
                                objAyaKar.hisabNo = TrimText(col[i + "khaSiNo"]);
                                objAyaKar.baUSiNaId = baUSiNaId;
                                db.ayaKars.Add(objAyaKar);
                            }
                        }

                        db.SaveChanges();
                        int bhuktaniAdeshNo;
                        try
                        {
                            bhuktaniAdeshNo = Convert.ToInt32(UnicodeToString(col["bhuktaniAdeshNo"]));
                        }
                        catch
                        {
                            bhuktaniAdeshNo = 0;
                        }

                        //update bhuktaniadeshno Number
                        var setNum1 = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh" && m.baUSiNaId == baUSiNaId);
                        setNum1.faramName = setNum1.faramName;
                        if (bhuktaniAdeshNo == 0)
                        {
                            setNum1.number = setNum1.number;
                        }
                        else
                        {
                            setNum1.number = bhuktaniAdeshNo;
                        }
                        db.SaveChanges();
                    }
                   

                    return RedirectToAction("IndexPeski");
                }
                else
                {
                    var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
                    ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", baUSiNaId);
                    ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
                    return View(jornalentries);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex;
                var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
                ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo");
                var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
                return View(jornalentries);
            }
        }


        public ActionResult nikasa()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            var uniqBhuktani = (from d in db.bhuktaniAdeshs
                                where d.fyId == objFiscalYear.fyId && d.monthIndex == 0 && d.bhuktaniType != "भुक्तानी आदेश धरौटी"
                                group d by new { d.bhuktaniAdeshNo }
                                    into mygroup
                                    select mygroup.FirstOrDefault()).ToList();
            var objBhuktanis = (from d in uniqBhuktani
                                select new { label = d.bhuktaniAdeshNo + "(" + d.nepDateStr + ")", value = d.bhuktaniAdeshNo }).ToList();
            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo");
            ViewBag.bhuktaniAdeshNo = new SelectList(objBhuktanis, "label", "value");
            return View();
        }
        public ActionResult createNikasa()
        {
            return RedirectToAction("nikasa", "JornalEntries");
        }
        [HttpPost]
        public ActionResult createNikasa(int fyId, int mn, int baUSiNaId, string bhuktaniAdeshNo)
        {
            var objBhuktani = new List<bhuktaniAdesh>();
            objBhuktani = (from d in db.bhuktaniAdeshs
                           where d.bhuktaniType == "भुक्तानी आदेश" && d.fyId == fyId  && d.bhuktaniAdeshNo == bhuktaniAdeshNo && d.baUSiNaId == baUSiNaId //&& d.monthIndex == mn
                           group d by new { d.jornalKharchaNo }
                               into mygroup
                               select mygroup.FirstOrDefault()).ToList();


            var objBhuktaniTalabi = (from d in db.bhuktaniAdeshs
                                     where d.bhuktaniType == "भुक्तानी आदेश तलबी" && d.fyId == fyId  && d.bhuktaniAdeshNo == bhuktaniAdeshNo// && d.monthIndex == mn
                                     group d by new { d.jornalKharchaNo }
                                         into mygroup
                                         select mygroup.FirstOrDefault()).ToList();
            if (objBhuktani.Count == 0 && objBhuktaniTalabi.Count > 0)
            {

                objBhuktani = objBhuktaniTalabi;
            }

            ViewBag.bhuktaniAdeshNo = bhuktaniAdeshNo;
            List<jornalEntries> objJornalNikasa = new List<jornalEntries>();
            List<jornalEntries> objJornalKharcha = new List<jornalEntries>();
            List<jornalEntries> objJornalFarchaut = new List<jornalEntries>();

            foreach (var item in objBhuktani)
            {
                var objJornalEntries = db.jornalEntries.Where(m => (m.fyId == fyId)  && (m.jornalNo == item.jornalKharchaNo) && (m.baUSiNaId == baUSiNaId) //&& (m.monthIndex == mn)
                ).ToList();
                if (objJornalEntries.FirstOrDefault().jornalType == "पेश्की"||objJornalEntries.FirstOrDefault().jornalType == "खर्च")
                {
                    objJornalKharcha = objJornalKharcha.Concat(objJornalEntries).ToList();
                }
                if (objJornalEntries.FirstOrDefault().jornalType == "पेश्की फछर्यौट")
                {
                    decimal farchautRakam = db.jornalEntries.Where(m => m.jornalNo == item.jornalKharchaNo && m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली").FirstOrDefault().credit;
                    string hisabNo = db.jornalEntries.FirstOrDefault(m => m.jornalNo == item.jornalKharchaNo  && m.fyId == fyId && m.deCre == "debit"// && m.monthIndex == mn
                    ).hisabNo;
                    objJornalKharcha.Add(new jornalEntries
                    {
                        credit=0,
                        debit=farchautRakam,
                        hisabNo=hisabNo,
                        jornalNo=item.jornalKharchaNo,
                        deCre="debit",
                        nepDate = DateTime.Now,
                        enDate = DateTime.Now,
                        monthIndex= mn
                    });
                    objJornalKharcha.Add(new jornalEntries
                    {
                        credit = farchautRakam,
                        debit = 0,
                        hisabNo = "",
                        jornalNo = item.jornalKharchaNo,
                        deCre = "credit",
                        nepDate = DateTime.Now,
                        enDate = DateTime.Now,
                        monthIndex = mn
                    });
                }
            }
            if (objJornalKharcha.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found";
                return RedirectToAction("nikasa");
            }

            var objSetNum = db.setNumbers.FirstOrDefault(m => m.faramName == "JornalEntries" && m.baUSiNaId == baUSiNaId);
            var objBaUSiNo = db.baUSiNas.FirstOrDefault(m => m.baUSiNaId == baUSiNaId);
            string jornalNo;
            if (objSetNum != null)
            {
                jornalNo = StringToUnicode(objSetNum.number.ToString());
            }
            else
            {

                jornalNo = "";
            }
            var uniqKhaSiNo = (from d in objJornalKharcha
                               where d.deCre == "debit"
                               group d by new { d.hisabNo }
                                   into mygroup
                                   select mygroup.FirstOrDefault()).ToList();
            {
                jornalEntries objJornal = new jornalEntries();
                objJornal.jornalNo = jornalNo;
                objJornal.jornalType = "निकासा";
                objJornal.jornalEntriesId = 0;
                objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
                objJornal.fyId = fyId;
                objJornal.monthIndex = mn;
                objJornal.nepDateStr = "";
                objJornal.nepDate = DateTime.Now;
                objJornal.enDate = DateTime.Now;
                objJornal.sanketNo = "";
                objJornal.deCre = "debit";
                objJornal.bibaran = "एकल खाता कोष प्रणाली";
                objJornal.khaPaNo = "";
                objJornal.hisabNo = "";
                objJornal.debit = objJornalKharcha.Sum(m => m.credit);
                objJornal.credit = 0;
                objJornal.note = "यसमा संलग्न लेखा |";
                objJornal.month = ((month)mn).ToString();
                objJornal.year = getYear(fyId, mn);
                objJornalNikasa.Add(objJornal);
            }

            foreach (var item in uniqKhaSiNo)
            {
                jornalEntries objJornal = new jornalEntries();
                objJornal.jornalNo = jornalNo;
                objJornal.jornalType = "निकासा";
                objJornal.jornalEntriesId = 0;
                objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
                objJornal.fyId = fyId;
                objJornal.monthIndex = mn;
                objJornal.nepDateStr = "";
                objJornal.nepDate = DateTime.Now;
                objJornal.enDate = DateTime.Now;
                objJornal.sanketNo = "";
                objJornal.deCre = "credit";
                objJornal.bibaran = "रकम निकासा";
                objJornal.khaPaNo = "";
                objJornal.hisabNo = item.hisabNo;
                objJornal.debit = 0;
                objJornal.credit = objJornalKharcha.Where(m => m.hisabNo == item.hisabNo).Sum(m => m.debit);
                objJornal.note = "यसमा संलग्न लेखा |";
                objJornal.month = ((month)mn).ToString();
                objJornal.year = getYear(fyId, mn);
                objJornalNikasa.Add(objJornal);
            }
            var decre = new List<SelectListItem>{ new SelectListItem { Text = "--डे/क्रे--",Value="" },
                         new SelectListItem { Text = "डेबिट", Value = "debit" },
                         new SelectListItem { Text = "क्रेडिट", Value = "credit" } };


            int i = 0;
            foreach (var item in objJornalNikasa)
            {
                var selected = decre.Where(x => x.Value == item.deCre).First();
                selected.Selected = true;

                ViewData.Add("[" + i + "].deCre", decre);
                i++;
            }
            ViewBag.bhuktaniAdeshNo = bhuktaniAdeshNo;
            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", objJornalNikasa[0].baUSiNaId);

            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objJornalNikasa[0].fyId);
            return View(objJornalNikasa);
        }


        //[HttpPost]
        //public ActionResult createNikasa2(int fyId, int mn, int baUSiNaId, string bhuktaniAdeshNo)
        //{
        //    var objBhuktani = new List<bhuktaniAdesh>();
        //    objBhuktani = (from d in db.bhuktaniAdeshs
        //                   where d.bhuktaniType == "भुक्तानी आदेश" && d.fyId == fyId && d.monthIndex == mn && d.bhuktaniAdeshNo == bhuktaniAdeshNo && d.baUSiNaId == baUSiNaId
        //                   group d by new { d.jornalKharchaNo }
        //                       into mygroup
        //                       select mygroup.FirstOrDefault()).ToList();


        //    var objBhuktaniTalabi = (from d in db.bhuktaniAdeshs
        //                             where d.bhuktaniType == "भुक्तानी आदेश तलबी" && d.fyId == fyId && d.monthIndex == mn && d.bhuktaniAdeshNo == bhuktaniAdeshNo
        //                             group d by new { d.jornalKharchaNo }
        //                                 into mygroup
        //                                 select mygroup.FirstOrDefault()).ToList();
        //    if (objBhuktani.Count == 0 && objBhuktaniTalabi.Count > 0)
        //    {
        //        objBhuktani = objBhuktaniTalabi;
        //    }

        //    ViewBag.bhuktaniAdeshNo = bhuktaniAdeshNo;
        //    List<jornalEntries> objJornalNikasa = new List<jornalEntries>();
        //    List<jornalEntries> objJornalKharcha = new List<jornalEntries>();
        //    List<jornalEntries> objJornalPeski = new List<jornalEntries>();
        //    foreach (var item in objBhuktani)
        //    {
        //        var objJornalEntries = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex == mn) && (m.jornalType == "खर्च") && (m.jornalNo == item.jornalKharchaNo) && (m.baUSiNaId == baUSiNaId)).ToList();
        //        objJornalKharcha = objJornalKharcha.Concat(objJornalEntries).ToList();

        //        var objJornalEntries1 = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex == mn) && (m.jornalType == "पेश्की" || m.jornalType == "पेश्की फछर्यौट") && (m.jornalNo == item.jornalKharchaNo) && (m.baUSiNaId == baUSiNaId)).ToList();
        //        objJornalPeski = objJornalKharcha.Concat(objJornalEntries1).ToList();
        //    }
        //    if (objJornalKharcha.Count == 0 && objJornalPeski.Count == 0)
        //    {
        //        TempData["ErrorMessage"] = "No Record Found";
        //        return RedirectToAction("nikasa");
        //    }

        //    var objSetNum = db.setNumbers.FirstOrDefault(m => m.faramName == "JornalEntries" && m.baUSiNaId == baUSiNaId);
        //    var objBaUSiNo = db.baUSiNas.FirstOrDefault(m => m.baUSiNaId == baUSiNaId);
        //    string jornalNo;
        //    if (objSetNum != null)
        //    {
        //        jornalNo = StringToUnicode(objSetNum.number.ToString());
        //    }
        //    else
        //    {
        //        jornalNo = "";
        //    }

        //    if (objJornalKharcha.Count > 0 && objJornalPeski.Count == 0)
        //    {
        //        var uniqKhaSiNo = (from d in objJornalKharcha
        //                           where d.deCre == "debit"
        //                           group d by new { d.hisabNo }
        //                               into mygroup
        //                               select mygroup.FirstOrDefault()).ToList();

        //        {
        //            jornalEntries objJornal = new jornalEntries();
        //            objJornal.jornalNo = jornalNo;
        //            objJornal.jornalType = "निकासा";
        //            objJornal.jornalEntriesId = 0;
        //            objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
        //            objJornal.fyId = fyId;
        //            objJornal.monthIndex = mn;
        //            objJornal.nepDateStr = "";
        //            objJornal.nepDate = DateTime.Now;
        //            objJornal.enDate = DateTime.Now;
        //            objJornal.sanketNo = "";
        //            objJornal.deCre = "debit";
        //            objJornal.bibaran = "एकल खाता कोष प्रणाली";
        //            objJornal.khaPaNo = "";
        //            objJornal.hisabNo = "";
        //            objJornal.debit = objJornalKharcha.Sum(m => m.credit);
        //            objJornal.credit = 0;
        //            objJornal.note = "यसमा संलग्न लेखा |";
        //            objJornal.month = ((month)mn).ToString();
        //            objJornal.year = getYear(fyId, mn);
        //            objJornalNikasa.Add(objJornal);
        //        }

        //        foreach (var item in uniqKhaSiNo)
        //        {
        //            jornalEntries objJornal = new jornalEntries();
        //            objJornal.jornalNo = jornalNo;
        //            objJornal.jornalType = "निकासा";
        //            objJornal.jornalEntriesId = 0;
        //            objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
        //            objJornal.fyId = fyId;
        //            objJornal.monthIndex = mn;
        //            objJornal.nepDateStr = "";
        //            objJornal.nepDate = DateTime.Now;
        //            objJornal.enDate = DateTime.Now;
        //            objJornal.sanketNo = "";
        //            objJornal.deCre = "credit";
        //            objJornal.bibaran = "रकम निकासा";
        //            objJornal.khaPaNo = "";
        //            objJornal.hisabNo = item.hisabNo;
        //            objJornal.debit = 0;
        //            objJornal.credit = objJornalKharcha.Where(m => m.hisabNo == item.hisabNo).Sum(m => m.debit);
        //            objJornal.note = "यसमा संलग्न लेखा |";
        //            objJornal.month = ((month)mn).ToString();
        //            objJornal.year = getYear(fyId, mn);
        //            objJornalNikasa.Add(objJornal);
        //        }
        //    }
        //    else if (objJornalPeski.Count > 0)
        //    {
        //        decimal totalAmount = 0;
        //        var uniqKhaSiNo = (from d in objJornalPeski
        //                           where d.deCre == "debit" && d.jornalType == "पेश्की"
        //                           group d by new { d.hisabNo }
        //                               into mygroup
        //                               select mygroup.FirstOrDefault()).ToList();

        //        var uniqJornalFarchaut = (from d in objJornalPeski
        //                                  where d.jornalType == "पेश्की फछर्यौट"
        //                                  group d by new { d.jornalNo }
        //                                      into mygroup
        //                                      select mygroup.FirstOrDefault()).ToList();
        //        foreach (var item in uniqKhaSiNo)
        //        {
        //            List<jornalEntries> lstPeski = objJornalPeski.Where(m => m.hisabNo == item.hisabNo).ToList();
        //            jornalEntries objJornal = new jornalEntries();
        //            objJornal.jornalNo = jornalNo;
        //            objJornal.jornalType = "निकासा";
        //            objJornal.jornalEntriesId = 0;
        //            objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
        //            objJornal.fyId = fyId;
        //            objJornal.monthIndex = mn;
        //            objJornal.nepDateStr = "";
        //            objJornal.nepDate = DateTime.Now;
        //            objJornal.enDate = DateTime.Now;
        //            objJornal.sanketNo = "";
        //            objJornal.deCre = "credit";
        //            objJornal.bibaran = "रकम निकासा";
        //            objJornal.khaPaNo = "";
        //            objJornal.hisabNo = item.hisabNo;
        //            objJornal.debit = 0;
        //            objJornal.credit = lstPeski.Sum(m => m.debit);
        //            objJornal.note = "यसमा संलग्न लेखा |";
        //            objJornal.month = ((month)mn).ToString();
        //            objJornal.year = getYear(fyId, mn);
        //            objJornalNikasa.Add(objJornal);
        //            totalAmount = totalAmount + objJornal.credit;
        //        }

        //        foreach (var item in uniqJornalFarchaut)
        //        {
        //            //List<string> bibarans = db.jornalEntries.Where(m => m.jornalNo == item.jornalNo && m.monthIndex == mn && m.fyId == m.fyId && m.jornalType == "पेश्की फछर्यौट").Select(m => m.bibaran).ToList();
        //            List<jornalEntries> lstPeski = objJornalPeski.Where(m => m.jornalNo == item.jornalNo && m.jornalType == "पेश्की फछर्यौट").ToList();
        //            var uniqKhaSiNoFa = (from d in lstPeski
        //                                 where d.deCre == "debit"
        //                                 group d by new { d.hisabNo }
        //                                     into mygroup
        //                                     select mygroup.FirstOrDefault()).ToList();
        //            if (uniqKhaSiNoFa.Count == 1)
        //            {
        //                jornalEntries objJornal = new jornalEntries();
        //                objJornal.jornalNo = jornalNo;
        //                objJornal.jornalType = "निकासा";
        //                objJornal.jornalEntriesId = 0;
        //                objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
        //                objJornal.fyId = fyId;
        //                objJornal.monthIndex = mn;
        //                objJornal.nepDateStr = "";
        //                objJornal.nepDate = DateTime.Now;
        //                objJornal.enDate = DateTime.Now;
        //                objJornal.sanketNo = "";
        //                objJornal.deCre = "credit";
        //                objJornal.bibaran = "रकम निकासा";
        //                objJornal.khaPaNo = "";
        //                objJornal.hisabNo = lstPeski.FirstOrDefault(m => m.deCre == "debit").hisabNo;
        //                objJornal.debit = 0;
        //                objJornal.credit = lstPeski.FirstOrDefault(m => m.bibaran == "एकल खाता कोष प्रणाली").credit;
        //                objJornal.note = "यसमा संलग्न लेखा |";
        //                objJornal.month = ((month)mn).ToString();
        //                objJornal.year = getYear(fyId, mn);
        //                objJornalNikasa.Add(objJornal);
        //                totalAmount = totalAmount + objJornal.credit;
        //            }
        //            else  if (uniqKhaSiNoFa.Count > 1)
        //            {
        //                foreach (var item2 in uniqKhaSiNoFa)
        //                {
        //                    jornalEntries objJornal = new jornalEntries();
        //                    objJornal.jornalNo = jornalNo;
        //                    objJornal.jornalType = "निकासा";
        //                    objJornal.jornalEntriesId = 0;
        //                    objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
        //                    objJornal.fyId = fyId;
        //                    objJornal.monthIndex = mn;
        //                    objJornal.nepDateStr = "";
        //                    objJornal.nepDate = DateTime.Now;
        //                    objJornal.enDate = DateTime.Now;
        //                    objJornal.sanketNo = "";
        //                    objJornal.deCre = "credit";
        //                    objJornal.bibaran = "रकम निकासा";
        //                    objJornal.khaPaNo = "";
        //                    objJornal.hisabNo = lstPeski.FirstOrDefault(m => m.deCre == "debit" && m.hisabNo == item2.hisabNo).hisabNo;
        //                    objJornal.debit = 0;
        //                    objJornal.credit = lstPeski.FirstOrDefault(m => m.deCre == "debit" && m.hisabNo == item2.hisabNo).debit - lstPeski.FirstOrDefault(m => m.deCre == "credit" && m.hisabNo == item2.hisabNo).credit;
        //                    objJornal.note = "यसमा संलग्न लेखा |";
        //                    objJornal.month = ((month)mn).ToString();
        //                    objJornal.year = getYear(fyId, mn);
        //                    objJornalNikasa.Add(objJornal);
        //                    totalAmount = totalAmount + objJornal.credit;
        //                }
        //            }


        //        }


        //        {


        //            jornalEntries objJornal = new jornalEntries();
        //            objJornal.jornalNo = jornalNo;
        //            objJornal.jornalType = "निकासा";
        //            objJornal.jornalEntriesId = 0;
        //            objJornal.baUSiNaId = objBaUSiNo.baUSiNaId;
        //            objJornal.fyId = fyId;
        //            objJornal.monthIndex = mn;
        //            objJornal.nepDateStr = "";
        //            objJornal.nepDate = DateTime.Now;
        //            objJornal.enDate = DateTime.Now;
        //            objJornal.sanketNo = "";
        //            objJornal.deCre = "debit";
        //            objJornal.bibaran = "एकल खाता कोष प्रणाली";
        //            objJornal.khaPaNo = "";
        //            objJornal.hisabNo = "";
        //            objJornal.debit = totalAmount;
        //            objJornal.credit = 0;
        //            objJornal.note = "यसमा संलग्न लेखा |";
        //            objJornal.month = ((month)mn).ToString();
        //            objJornal.year = getYear(fyId, mn);
        //            objJornalNikasa.Insert(0, objJornal);
        //        }

        //    }
        //    var decre = new List<SelectListItem>{ new SelectListItem { Text = "--डे/क्रे--",Value="" },
        //                 new SelectListItem { Text = "डेबिट", Value = "debit" },
        //                 new SelectListItem { Text = "क्रेडिट", Value = "credit" } };


        //    int i = 0;
        //    foreach (var item in objJornalNikasa)
        //    {
        //        var selected = decre.Where(x => x.Value == item.deCre).First();
        //        selected.Selected = true;

        //        ViewData.Add("[" + i + "].deCre", decre);
        //        i++;
        //    }
        //    ViewBag.bhuktaniAdeshNo = bhuktaniAdeshNo;
        //    var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
        //    ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", objJornalNikasa[0].baUSiNaId);

        //    ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objJornalNikasa[0].fyId);
        //    return View(objJornalNikasa);
        //}

        public ActionResult CreateNikasas()
        {
            return RedirectToAction("nikasa", "JornalEntries");
        }

        [HttpPost]
        public ActionResult CreateNikasas(List<jornalEntries> objJornalEntries, FormCollection col)
        {
            // DateTime nepDate = Convert.ToDateTime(col["nepDate"]);
            DateTime enDate = Convert.ToDateTime(col["enDate"]);
            string nepDateStr = col["nepDate"].Replace('-', '/');
            string note = col["note"];
            string jornalNo = col["jornalEntriesNo"];
            int baUSiNaId = Convert.ToInt32(objJornalEntries[0].baUSiNaId);
            decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"]);
            if (ModelState.IsValid)
            {
                int fyId = objJornalEntries[0].fyId;
                int mn = objJornalEntries[0].monthIndex;
                foreach (var item in objJornalEntries)
                {
                    jornalEntries objJornal = new jornalEntries();
                    objJornal.jornalNo = jornalNo;
                    objJornal.jornalType = "निकासा";
                    objJornal.baUSiNaId = item.baUSiNaId;
                    objJornal.fyId = item.fyId;
                    objJornal.monthIndex = item.monthIndex;
                    objJornal.nepDate = DateTime.Now;
                    objJornal.sanketNo = item.sanketNo;
                    objJornal.deCre = item.deCre;
                    objJornal.bibaran = item.bibaran;
                    objJornal.khaPaNo = item.khaPaNo;
                    objJornal.hisabNo = item.hisabNo;
                    objJornal.debit = Math.Round(item.debit, 2);
                    objJornal.credit = Math.Round(item.credit, 2);
                    objJornal.note = note;
                    objJornal.month = item.month;
                    objJornal.year = item.year;
                    objJornal.nepDateStr = nepDateStr;
                    objJornal.enDate = enDate;
                    objJornal.chequeRakam = chequeRakam;
                    db.jornalEntries.Add(objJornal);
                }

                string bhuktaniAdeshNo = col["bhuktaniAdeshNo"];
                // mn = MI.change(nepDate);
                var objBhuktani = db.bhuktaniAdeshs.Where(m => m.fyId == fyId  && m.bhuktaniAdeshNo == bhuktaniAdeshNo && m.bhuktaniType != "भुक्तानी आदेश धरौटी" && m.baUSiNaId == baUSiNaId// && m.monthIndex == mn
                ).ToList();
                foreach (var item in objBhuktani)
                {
                    item.jornalNikasiNo = jornalNo;
                }
                try
                {
                    var setNum = db.setNumbers.FirstOrDefault(m => m.faramName == "JornalEntries" && m.baUSiNaId == baUSiNaId);
                    setNum.number = setNum.number + 1;

                    
                    var setNum2 = db.setNumbers.FirstOrDefault(m => m.faramName == "BhuktaniAdesh" && m.baUSiNaId == baUSiNaId);
                    if (objJornalEntries.Where(m => m.hisabNo == "२११११").ToList().Count > 0)
                    {
                        setNum2.number = setNum2.number;
                    }
                    else
                    {
                        setNum2.number = setNum2.number+1;
                    }
                    

                    db.SaveChanges();
                    return RedirectToAction("Index", "JornalEntries");
                }
                catch
                {
                    ViewBag.ErrorMessage = "No Records Inserted";
                    return RedirectToAction("nikasa", "JornalEntries");
                    // return View(objJornalEntries);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "No Records Inserted";
                return RedirectToAction("nikasa", "JornalEntries");
            }
        }


        // GET: /JornalEntries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            jornalEntries jornalentries = db.jornalEntries.Find(id);
            List<jornalEntries> objJornalEntries = db.jornalEntries.Where(m => (m.jornalNo == jornalentries.jornalNo) && (m.jornalType == jornalentries.jornalType) && m.baUSiNaId == jornalentries.baUSiNaId && m.monthIndex == jornalentries.monthIndex && m.fyId == jornalentries.fyId).ToList();
            int i = 0;
            var decre = new List<SelectListItem>{ new SelectListItem { Text = "--डे/क्रे--",Value="" },
                         new SelectListItem { Text = "डेबिट", Value = "debit" },
                         new SelectListItem { Text = "क्रेडिट", Value = "credit" } };



            foreach (var item in objJornalEntries)
            {
                var selected = decre.Where(x => x.Value == item.deCre).First();
                selected.Selected = true;

                ViewData.Add("[" + i + "].deCre", decre);
                //ViewBag.decre=new" SelectList(decre,"Value","Text",item.deCre);
                i++;
            }
            if (objJornalEntries == null)
            {
                return HttpNotFound();
            }

            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", jornalentries.baUSiNaId);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objJornalEntries[0].fyId);
            ViewBag.IsNullBhuktanis = "null";
            ViewBag.BhuktnaiBibaran = null;
            ViewBag.RowCount = 0;
            var objJornalTalabi = (db.jornalEntries.Where(m => (m.jornalType == "खर्च") && (m.jornalNo == jornalentries.jornalNo) && (m.fyId == jornalentries.fyId) && (m.monthIndex == jornalentries.monthIndex)).ToList()).Where(m => m.hisabNo == "२११११").ToList();
            if (jornalentries.jornalType == "खर्च" && objJornalTalabi.Count == 0)
            {
                var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश") && (m.jornalKharchaNo == jornalentries.jornalNo) && (m.fyId == jornalentries.fyId) && (m.monthIndex == jornalentries.monthIndex) && m.baUSiNaId == jornalentries.baUSiNaId).ToList();
                var objAyaKar = db.ayaKars.Where(m => m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex && m.jornalKharchaNo == jornalentries.jornalNo).ToList();
                var objBhuktaniKharcha = new List<bhuktaniBibaranKharcha>();
                foreach (var item in objBhuktani)
                {
                    var objAyaBibaran = objAyaKar.FirstOrDefault(m => m.firmName == item.pauneKoNaam && m.hisabNo == item.khaSiNo && m.baUSiNaId == jornalentries.baUSiNaId);
                    objBhuktaniKharcha.Add(new bhuktaniBibaranKharcha
                    {
                        baUSiNaId = item.baUSiNaId,
                        bhuktaniAdeshId = item.bhuktaniAdeshId,
                        bhuktaniAdeshNo = item.bhuktaniAdeshNo,
                        jornalKharchaNo = item.jornalKharchaNo,
                        rakam = item.rakam,
                        pauneKoNaam = item.pauneKoNaam,
                        pauneKoCode = item.pauneKoCode,
                        nepDate = item.nepDate,
                        reamrks = item.reamrks,
                        bibaran = item.bibaran,
                        khaSiNo = item.khaSiNo,
                        jornalNikasiNo = item.jornalNikasiNo,
                        year = item.year,
                        month = item.year,
                        monthIndex = item.monthIndex,
                        bhuktaniType = item.bhuktaniType,
                        fyId = item.fyId,
                        panNo = item.panNo,
                        BhuPraNo = item.BhuPraNo,
                        jammaRakam = objAyaBibaran.jammaRakam,
                        ayaKar = objAyaBibaran.aayaKar,
                        paKar = objAyaBibaran.paKar,
                        dharauti = objAyaBibaran.dharauti,
                        bakiPaune = objAyaBibaran.bakiPaune,
                        ayaKarId = objAyaBibaran.ayaKarId
                    });
                }
                ViewBag.BhuktaniBibaran = objBhuktaniKharcha;
                ViewBag.RowCount = objBhuktani.Count;
                ViewBag.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
                ViewBag.IsNullBhuktanis = "notNull";
            }
            else if (jornalentries.jornalType == "कट्टी")
            {
                var objAyakar = db.ayaKars.Where(m => m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex && m.jornalKattiNo == jornalentries.jornalNo && m.baUSiNaId == jornalentries.baUSiNaId).ToList();
                if (objAyakar.Count > 0)
                {
                    ViewBag.AyaKarBibaran = objAyakar;
                    ViewBag.RowCount = objAyakar.Count;
                    ViewBag.IsNullBhuktanis = "notNull";
                }
            }
            objJornalEntries[0].monthIndex = (int)Enum.Parse(typeof(month), objJornalEntries[0].month);
            return View(objJornalEntries);
        }

        // POST: /JornalEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<jornalEntries> jornalentries, FormCollection col)
        {
            int baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            int mn = Convert.ToInt32(col["month"]);
            DateTime enDate = Convert.ToDateTime(col["enDate"]);
            string nepDateStr = col["nepDate"].Replace('-', '/');
            string note = TrimText(col["note"]);
            string year = getYear(fyId, mn);
            decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"]);
            //  string jornalType =col["jornalType"].ToString();
            // string year = StringToUnicode(date.Year.ToString());
            if (ModelState.IsValid)
            {
                foreach (var item in jornalentries)
                {
                    if (item.jornalEntriesId != 0)
                    {
                        jornalEntries objJornal = db.jornalEntries.Find(item.jornalEntriesId);
                        objJornal.jornalNo = TrimText(objJornal.jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = objJornal.jornalType;
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
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
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = chequeRakam;
                    }
                    else
                    {
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(jornalentries[0].jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = jornalentries[0].jornalType;
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
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
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.chequeRakam = chequeRakam;
                        db.jornalEntries.Add(objJornal);
                    }
                }
                //db.Entry(jornalentries).State = EntityState.Modified;
                db.SaveChanges();



                if (jornalentries[0].jornalType == "खर्च")
                {
                    int rowNo = Convert.ToInt32(col["RowCount"]);
                    for (int i = 0; i < rowNo; i++)
                    {
                        {
                            if (col[i + "bhuktaniAdeshId"] != "0")
                            {
                                //Edit Bhuktani Adesh
                                int bhuktaniId = Convert.ToInt32(col[i + "bhuktaniAdeshId"]);
                                bhuktaniAdesh objBhuktani = db.bhuktaniAdeshs.Find(bhuktaniId);
                                objBhuktani.baUSiNaId = baUSiNaId;
                                objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                                objBhuktani.jornalKharchaNo = jornalentries[0].jornalNo;
                                objBhuktani.rakam = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                                objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                                objBhuktani.nepDate = DateTime.Now;
                                objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                                objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                                objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                                objBhuktani.jornalNikasiNo = objBhuktani.jornalNikasiNo;
                                objBhuktani.year = year;
                                objBhuktani.month = ((month)mn).ToString();
                                objBhuktani.monthIndex = MI.change(nepDateStr);
                                objBhuktani.bhuktaniType = "भुक्तानी आदेश";
                                objBhuktani.fyId = fyId;
                                objBhuktani.panNo = TrimText(col[i + "panNo"]);
                                objBhuktani.BhuPraNo = TrimText(col[i + "bhuPraNo"]);
                                objBhuktani.nepDateStr = nepDateStr;
                                objBhuktani.enDate = enDate;

                                int ayaKarId = Convert.ToInt32(col[i + "ayaKarId"]);
                                ayaKar objAyaKar = db.ayaKars.Find(ayaKarId);
                                objAyaKar.firmName = TrimText(col[i + "pauneKoNaam"]);
                                objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                                objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                                objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                                objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                                objAyaKar.bakiPaune = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objAyaKar.fyId = fyId;
                                objAyaKar.monthIndex = MI.change(nepDateStr);
                                objAyaKar.jornalKharchaNo = jornalentries[0].jornalNo;
                                objAyaKar.jornalKattiNo = "०";
                                objAyaKar.hisabNo = TrimText(col[i + "khaSiNo"]);

                            }
                            else
                            {
                                {
                                    bhuktaniAdesh objBhuktani = new bhuktaniAdesh();
                                    objBhuktani.baUSiNaId = baUSiNaId;
                                    objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                                    objBhuktani.jornalKharchaNo = jornalentries[0].jornalNo;
                                    objBhuktani.rakam = Convert.ToDecimal(col[i + "bakiPaune"]);
                                    objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                                    objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                                    objBhuktani.nepDate = DateTime.Now;
                                    objBhuktani.nepDateStr = nepDateStr;
                                    objBhuktani.enDate = enDate;
                                    objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                                    objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                                    objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                                    objBhuktani.jornalNikasiNo = "०";
                                    objBhuktani.year = year;
                                    objBhuktani.month = ((month)mn).ToString();
                                    objBhuktani.monthIndex = MI.change(nepDateStr);
                                    objBhuktani.bhuktaniType = "भुक्तानी आदेश";
                                    objBhuktani.fyId = fyId;
                                    objBhuktani.panNo = TrimText(col[i + "panNo"]);
                                    objBhuktani.BhuPraNo = TrimText(col[i + "bhuPraNo"]);
                                    db.bhuktaniAdeshs.Add(objBhuktani);
                                }
                                {
                                    ayaKar objAyaKar = new ayaKar();
                                    objAyaKar.firmName = TrimText(col[i + "pauneKoNaam"]);
                                    objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                                    objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                                    objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                                    objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                                    objAyaKar.bakiPaune = Convert.ToDecimal(col[i + "bakiPaune"]);
                                    objAyaKar.fyId = fyId;
                                    objAyaKar.monthIndex = MI.change(nepDateStr);
                                    objAyaKar.jornalKharchaNo = jornalentries[0].jornalNo;
                                    objAyaKar.jornalKattiNo = "०";
                                    objAyaKar.hisabNo = TrimText(col[i + "khaSiNo"]);
                                    objAyaKar.baUSiNaId = baUSiNaId;
                                    db.ayaKars.Add(objAyaKar);
                                }
                            }
                        }

                    }
                    db.SaveChanges();
                }
                else if (jornalentries[0].jornalType == "कट्टी")
                {
                    int rowNo = Convert.ToInt32(col["rowCount2"]);
                    for (int i = 0; i < rowNo; i++)
                    {
                        if (col[i + "ayaKarId2"] != "0")
                        {
                            int ayaKarId = Convert.ToInt32(col[i + "ayaKarId2"]);
                            ayaKar objAyaKar = db.ayaKars.Find(ayaKarId);
                            objAyaKar.firmName = TrimText(col[i + "firmName"]);
                            objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                            objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                            objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                            objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                            objAyaKar.bakiPaune = Convert.ToDecimal(col[i + "bakiPaune"]);
                            objAyaKar.fyId = fyId;
                            objAyaKar.monthIndex = MI.change(nepDateStr);
                            objAyaKar.jornalKharchaNo = "०";
                            objAyaKar.jornalKattiNo = jornalentries[0].jornalNo;
                            objAyaKar.baUSiNaId = baUSiNaId;
                            objAyaKar.hisabNo = "";
                        }
                        else
                        {
                            ayaKar objAyaKar = new ayaKar();
                            objAyaKar.firmName = TrimText(col[i + "firmName"]);
                            objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                            objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                            objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                            objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                            objAyaKar.jornalKattiNo = jornalentries[0].jornalNo;
                            objAyaKar.jornalKharchaNo = "०";
                            objAyaKar.fyId = fyId;
                            objAyaKar.monthIndex = MI.change(nepDateStr);
                            objAyaKar.baUSiNaId = baUSiNaId;
                            db.ayaKars.Add(objAyaKar);
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", jornalentries[0].baUSiNaId);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", jornalentries[0].fyId);


            ViewBag.BhuktnaiBibaran = null;
            ViewBag.RowCount = 0;
            if (jornalentries[0].jornalType == "खर्च")
            {
                var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश") && (m.jornalKharchaNo == jornalentries[0].jornalNo) && (m.fyId == jornalentries[0].fyId) && (m.monthIndex == jornalentries[0].monthIndex) && m.baUSiNaId == baUSiNaId).ToList();
                ViewBag.BhuktaniBibaran = objBhuktani;
                ViewBag.RowCount = objBhuktani.Count;
                ViewBag.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
            }
            else if (jornalentries[0].jornalType == "कट्टी")
            {
                var objAyakar = db.ayaKars.Where(m => m.fyId == jornalentries[0].fyId && m.monthIndex == jornalentries[0].monthIndex && m.jornalKattiNo == jornalentries[0].jornalNo && m.baUSiNaId == baUSiNaId).ToList();
                if (objAyakar.Count > 0)
                {
                    ViewBag.AyaKarBibaran = objAyakar;
                    ViewBag.RowCount = objAyakar.Count;
                    ViewBag.IsNullBhuktanis = "notNull";
                }
            }

            return View(jornalentries);
        }



        public ActionResult EditPeski(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            jornalEntries jornalentries = db.jornalEntries.Find(id);
            List<jornalEntries> objJornalEntries = db.jornalEntries.Where(m => (m.jornalNo == jornalentries.jornalNo) && (m.jornalType == jornalentries.jornalType) && m.baUSiNaId == jornalentries.baUSiNaId && m.monthIndex == jornalentries.monthIndex && m.fyId == jornalentries.fyId).ToList();
            int i = 0;
            var decre = new List<SelectListItem>{ new SelectListItem { Text = "--डे/क्रे--",Value="" },
                         new SelectListItem { Text = "डेबिट", Value = "debit" },
                         new SelectListItem { Text = "क्रेडिट", Value = "credit" } };

            foreach (var item in objJornalEntries)
            {
                var selected = decre.Where(x => x.Value == item.deCre).First();
                selected.Selected = true;

                ViewData.Add("[" + i + "].deCre", decre);
                //ViewBag.decre=new" SelectList(decre,"Value","Text",item.deCre);
                i++;
            }
            if (objJornalEntries == null)
            {
                return HttpNotFound();
            }

            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", jornalentries.baUSiNaId);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objJornalEntries[0].fyId);
            ViewBag.IsNullBhuktanis = "null";
            ViewBag.BhuktnaiBibaran = null;
            ViewBag.RowCount = 0;
            List<jornalEntries> objJornal = new List<jornalEntries>();
            if (objJornalEntries.FirstOrDefault().jornalType == "पेश्की फछर्यौट")
            {
               objJornal = objJornalEntries.Where(m => m.jornalType == "पेश्की फछर्यौट" && m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली").ToList();
            }
            if (objJornalEntries.FirstOrDefault().jornalType=="पेश्की" || objJornal.Count>0)
            {
                var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश") && (m.jornalKharchaNo == jornalentries.jornalNo) && (m.fyId == jornalentries.fyId) && (m.monthIndex == jornalentries.monthIndex) && m.baUSiNaId == jornalentries.baUSiNaId).ToList();
                var objAyaKar = db.ayaKars.Where(m => m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex && m.jornalKharchaNo == jornalentries.jornalNo).ToList();
                var objBhuktaniKharcha = new List<bhuktaniBibaranKharcha>();
                foreach (var item in objBhuktani)
                {
                    var objAyaBibaran = objAyaKar.FirstOrDefault(m => m.firmName == item.pauneKoNaam && m.hisabNo == item.khaSiNo && m.baUSiNaId == jornalentries.baUSiNaId);
                    objBhuktaniKharcha.Add(new bhuktaniBibaranKharcha
                    {
                        baUSiNaId = item.baUSiNaId,
                        bhuktaniAdeshId = item.bhuktaniAdeshId,
                        bhuktaniAdeshNo = item.bhuktaniAdeshNo,
                        jornalKharchaNo = item.jornalKharchaNo,
                        rakam = item.rakam,
                        pauneKoNaam = item.pauneKoNaam,
                        pauneKoCode = item.pauneKoCode,
                        nepDate = item.nepDate,
                        reamrks = item.reamrks,
                        bibaran = item.bibaran,
                        khaSiNo = item.khaSiNo,
                        jornalNikasiNo = item.jornalNikasiNo,
                        year = item.year,
                        month = item.year,
                        monthIndex = item.monthIndex,
                        bhuktaniType = item.bhuktaniType,
                        fyId = item.fyId,
                        panNo = item.panNo,
                        BhuPraNo = item.BhuPraNo,
                        jammaRakam = objAyaBibaran.jammaRakam,
                        ayaKar = objAyaBibaran.aayaKar,
                        paKar = objAyaBibaran.paKar,
                        dharauti = objAyaBibaran.dharauti,
                        bakiPaune = objAyaBibaran.bakiPaune,
                        ayaKarId = objAyaBibaran.ayaKarId
                    });
                }
                ViewBag.BhuktaniBibaran = objBhuktaniKharcha;
                ViewBag.RowCount = objBhuktani.Count;
                ViewBag.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
                ViewBag.IsNullBhuktanis = "notNull";
            }
           
            objJornalEntries[0].monthIndex = (int)Enum.Parse(typeof(month), objJornalEntries[0].month);
            return View(objJornalEntries);
        }

        // POST: /JornalEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPeski(List<jornalEntries> jornalentries, FormCollection col)
        {
            int baUSiNaId = Convert.ToInt32(col["baUSiNaId"]);
            int fyId = Convert.ToInt32(col["fyId"]);
            int mn = Convert.ToInt32(col["month"]);
            DateTime enDate = Convert.ToDateTime(col["enDate"]);
            string nepDateStr = col["nepDate"].Replace('-', '/');
            string note = TrimText(col["note"]);
            string year = getYear(fyId, mn);
            decimal chequeRakam = Convert.ToDecimal(col["chequeRakam"]);
              var objJornalFarchaut = jornalentries.Where(m=>m.jornalType=="पेश्की फछर्यौट" && m.bibaran=="एकल खाता कोष प्रणाली" && m.deCre=="credit").ToList();

            //  string jornalType =col["jornalType"].ToString();
            // string year = StringToUnicode(date.Year.ToString());
            if (ModelState.IsValid)
            {
                foreach (var item in jornalentries)
                {
                    if (item.jornalEntriesId != 0)
                    {
                        jornalEntries objJornal = db.jornalEntries.Find(item.jornalEntriesId);
                        objJornal.jornalNo = TrimText(objJornal.jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = objJornal.jornalType;
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
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
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.fyId = fyId;
                        objJornal.chequeRakam = chequeRakam;
                    }
                    else
                    {
                        jornalEntries objJornal = new jornalEntries();
                        objJornal.jornalNo = TrimText(jornalentries[0].jornalNo);
                        objJornal.baUSiNaId = baUSiNaId;
                        objJornal.jornalType = jornalentries[0].jornalType;
                        objJornal.month = ((month)mn).ToString();
                        objJornal.nepDate = DateTime.Now;
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
                        objJornal.nepDateStr = nepDateStr;
                        objJornal.enDate = enDate;
                        objJornal.chequeRakam = chequeRakam;
                        db.jornalEntries.Add(objJornal);
                    }
                }
                //db.Entry(jornalentries).State = EntityState.Modified;
                db.SaveChanges();
            
              

                if (jornalentries[0].jornalType == "पेश्की" ||   objJornalFarchaut.Count>0)
                {
                    int rowNo = Convert.ToInt32(col["RowCount"]);
                    for (int i = 0; i < rowNo; i++)
                    {
                        {
                            if (col[i + "bhuktaniAdeshId"] != "0")
                            {
                                //Edit Bhuktani Adesh
                                int bhuktaniId = Convert.ToInt32(col[i + "bhuktaniAdeshId"]);
                                bhuktaniAdesh objBhuktani = db.bhuktaniAdeshs.Find(bhuktaniId);
                                objBhuktani.baUSiNaId = baUSiNaId;
                                objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                                objBhuktani.jornalKharchaNo = jornalentries[0].jornalNo;
                                objBhuktani.rakam = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                                objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                                objBhuktani.nepDate = DateTime.Now;
                                objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                                objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                                objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                                objBhuktani.jornalNikasiNo = objBhuktani.jornalNikasiNo;
                                objBhuktani.year = year;
                                objBhuktani.month = ((month)mn).ToString();
                                objBhuktani.monthIndex = MI.change(nepDateStr);
                                objBhuktani.bhuktaniType = "भुक्तानी आदेश";
                                objBhuktani.fyId = fyId;
                                objBhuktani.panNo = TrimText(col[i + "panNo"]);
                                objBhuktani.BhuPraNo = TrimText(col[i + "bhuPraNo"]);
                                objBhuktani.nepDateStr = nepDateStr;
                                objBhuktani.enDate = enDate;

                                int ayaKarId = Convert.ToInt32(col[i + "ayaKarId"]);
                                ayaKar objAyaKar = db.ayaKars.Find(ayaKarId);
                                objAyaKar.firmName = TrimText(col[i + "pauneKoNaam"]);
                                objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                                objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                                objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                                objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                                objAyaKar.bakiPaune = Convert.ToDecimal(col[i + "bakiPaune"]);
                                objAyaKar.fyId = fyId;
                                objAyaKar.monthIndex = MI.change(nepDateStr);
                                objAyaKar.jornalKharchaNo = jornalentries[0].jornalNo;
                                objAyaKar.jornalKattiNo = "०";
                                objAyaKar.hisabNo = TrimText(col[i + "khaSiNo"]);

                            }
                            else
                            {
                                {
                                    bhuktaniAdesh objBhuktani = new bhuktaniAdesh();
                                    objBhuktani.baUSiNaId = baUSiNaId;
                                    objBhuktani.bhuktaniAdeshNo = TrimText(col["bhuktaniAdeshNo"]);
                                    objBhuktani.jornalKharchaNo = jornalentries[0].jornalNo;
                                    objBhuktani.rakam = Convert.ToDecimal(col[i + "bakiPaune"]);
                                    objBhuktani.pauneKoNaam = TrimText(col[i + "pauneKoNaam"]);
                                    objBhuktani.pauneKoCode = TrimText(col[i + "pauneKoCode"]);
                                    objBhuktani.nepDate = DateTime.Now;
                                    objBhuktani.nepDateStr = nepDateStr;
                                    objBhuktani.enDate = enDate;
                                    objBhuktani.reamrks = TrimText(col[i + "remarks"]);
                                    objBhuktani.bibaran = TrimText(col[i + "khaSirsak"]);
                                    objBhuktani.khaSiNo = TrimText(col[i + "khaSiNo"]);
                                    objBhuktani.jornalNikasiNo = "०";
                                    objBhuktani.year = year;
                                    objBhuktani.month = ((month)mn).ToString();
                                    objBhuktani.monthIndex = MI.change(nepDateStr);
                                    objBhuktani.bhuktaniType = "भुक्तानी आदेश";
                                    objBhuktani.fyId = fyId;
                                    objBhuktani.panNo = TrimText(col[i + "panNo"]);
                                    objBhuktani.BhuPraNo = TrimText(col[i + "bhuPraNo"]);
                                    db.bhuktaniAdeshs.Add(objBhuktani);
                                }
                                {
                                    ayaKar objAyaKar = new ayaKar();
                                    objAyaKar.firmName = TrimText(col[i + "pauneKoNaam"]);
                                    objAyaKar.jammaRakam = Convert.ToDecimal(col[i + "jammaRakam"]);
                                    objAyaKar.aayaKar = Convert.ToDecimal(col[i + "ayaKar"]);
                                    objAyaKar.paKar = Convert.ToDecimal(col[i + "paKar"]);
                                    objAyaKar.dharauti = Convert.ToDecimal(col[i + "dharauti"]);
                                    objAyaKar.bakiPaune = Convert.ToDecimal(col[i + "bakiPaune"]);
                                    objAyaKar.fyId = fyId;
                                    objAyaKar.monthIndex = MI.change(nepDateStr);
                                    objAyaKar.jornalKharchaNo = jornalentries[0].jornalNo;
                                    objAyaKar.jornalKattiNo = "०";
                                    objAyaKar.hisabNo = TrimText(col[i + "khaSiNo"]);
                                    objAyaKar.baUSiNaId = baUSiNaId;
                                    db.ayaKars.Add(objAyaKar);
                                }
                            }
                        }

                    }
                    db.SaveChanges();
                }
                return RedirectToAction("IndexPeski");
            }
            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", jornalentries[0].baUSiNaId);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", jornalentries[0].fyId);


            ViewBag.BhuktnaiBibaran = null;
            ViewBag.RowCount = 0;


            if (jornalentries[0].jornalType == "पेश्की" || objJornalFarchaut.Count>0)
            {
                var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.bhuktaniType == "भुक्तानी आदेश") && (m.jornalKharchaNo == jornalentries[0].jornalNo) && (m.fyId == jornalentries[0].fyId) && (m.monthIndex == jornalentries[0].monthIndex) && m.baUSiNaId == baUSiNaId).ToList();
                ViewBag.BhuktaniBibaran = objBhuktani;
                ViewBag.RowCount = objBhuktani.Count;
                ViewBag.bhuktaniAdeshNo = objBhuktani[0].bhuktaniAdeshNo;
            }
           

            return View(jornalentries);
        }






        public ActionResult IndexJornal()
        {

            var baUSiNa = db.baUSiNas.Where(m => m.baUSirsak != "धरौटी" || m.baUSiNo != "धरौटी").ToList();
            ViewBag.baUSiNaId = new SelectList(baUSiNa, "baUSiNaId", "baUSiNo", 2);
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        [HttpPost]
        public ActionResult IndexJornal(int mn, int fyId, int baUSiNaId)
        {
            var kharchaJornal = (from d in db.jornalEntries
                                 where d.jornalType == "खर्च" && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                                 group d by new { d.jornalNo }
                                     into mygroup
                                     select mygroup.FirstOrDefault()).ToList();
            ViewBag.kharchaJornal = kharchaJornal;
            var nikasaJornal = (from d in db.jornalEntries
                                where d.jornalType == "निकासा" && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                                group d by new { d.jornalNo }
                                    into mygroup
                                    select mygroup.FirstOrDefault()).ToList();
            ViewBag.nikasaJornal = nikasaJornal;
            var kattiJornal = (from d in db.jornalEntries
                               where d.jornalType == "कट्टी" && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                               group d by new { d.jornalNo }
                                   into mygroup
                                   select mygroup.FirstOrDefault()).ToList();
            ViewBag.kattiJornal = kattiJornal;
            var peskiJornal = (from d in db.jornalEntries
                               where d.jornalType == "पेश्की" && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                               group d by new { d.jornalNo }
                                   into mygroup
                                   select mygroup.FirstOrDefault()).ToList();
            ViewBag.peskiJornal = peskiJornal;


            var farchautJornal = (from d in db.jornalEntries
                               where d.jornalType == "पेश्की फछर्यौट" && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                               group d by new { d.jornalNo }
                                   into mygroup
                                   select mygroup.FirstOrDefault()).ToList();
            ViewBag.farchautJornal = farchautJornal;




            return View();

        }

        public ActionResult JornalBibaran(string id)
        {
            int jornalId = Convert.ToInt32(id);
            var jornalentries = db.jornalEntries.Find(jornalId);
            var objJornal = db.jornalEntries.Where(m => m.jornalNo == jornalentries.jornalNo && m.jornalType == jornalentries.jornalType && m.fyId == jornalentries.fyId && m.monthIndex == jornalentries.monthIndex && m.baUSiNaId == jornalentries.baUSiNaId).ToList();
           var objjornalFarchaut = db.jornalEntries.Where(m=>m.jornalNo==jornalentries.jornalNo && m.jornalType=="पेश्की फछर्यौट" && m.deCre=="credit" && m.bibaran=="एकल खाता कोष प्रणाली").ToList();
            if (objJornal.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found for selected month and year.";
                return RedirectToAction("DharautiJornal");
            }
            try
            {
                ViewBag.TotalInWords = NumToWords(objJornal.FirstOrDefault().chequeRakam);
            }
            catch
            {
                ViewBag.TotalInWords = "";
            }
            var objTalabiJornal = objJornal.FirstOrDefault(m => m.hisabNo == "२११११");
            if ((objJornal[0].jornalType == "खर्च" || objJornal[0].jornalType=="पेश्की" ||objjornalFarchaut.Count>0) && objTalabiJornal != null )
            {
                ViewBag.BhuktaniPaune = null;
            }
            else if ((objJornal[0].jornalType == "खर्च" && objTalabiJornal == null) || (objJornal[0].jornalType == "पेश्की")||(objjornalFarchaut.Count>0))
            {
                var objAyaKar = db.ayaKars.Where(m => m.monthIndex == jornalentries.monthIndex && m.fyId == jornalentries.fyId && m.jornalKharchaNo == jornalentries.jornalNo && m.baUSiNaId == jornalentries.baUSiNaId).ToList();
                if (objAyaKar.Count > 0)
                {
                    List<bhuktaniBibaran> objBhuktaniPaune = new List<bhuktaniBibaran>();
                    foreach (var item in objAyaKar)
                    {
                        bhuktaniBibaran objPaune = new bhuktaniBibaran();
                        objPaune.pauneKoNam = item.firmName;
                        objPaune.jammaRakam = item.jammaRakam;
                        objPaune.ayaKar = item.aayaKar;
                        objPaune.paKar = item.paKar;
                        objPaune.dharauti = item.dharauti;
                        objPaune.bakiPaune = item.bakiPaune;
                        objPaune.firmName = "";
                        objBhuktaniPaune.Add(objPaune);
                    }
                    ViewBag.BhuktaniPaune = objBhuktaniPaune;
                }
                else
                {
                    ViewBag.BhuktaniPaune = null;
                }

            }
            else if (objJornal[0].jornalType == "कट्टी")
            {
                var objAyaBibaran = db.ayaKars.Where(m => m.monthIndex == jornalentries.monthIndex && m.fyId == jornalentries.fyId && m.jornalKattiNo == jornalentries.jornalNo && m.baUSiNaId == jornalentries.baUSiNaId).ToList();
                if (objAyaBibaran.Count > 0)
                {
                    List<bhuktaniBibaran> objBhuktaniPaune = new List<bhuktaniBibaran>();
                    foreach (var item in objAyaBibaran)
                    {
                        bhuktaniBibaran objPaune = new bhuktaniBibaran();
                        objPaune.pauneKoNam = "";
                        objPaune.firmName = item.firmName;
                        objPaune.ayaKar = item.aayaKar;
                        objPaune.paKar = item.paKar;
                        objPaune.dharauti = item.dharauti;
                        objPaune.jammaRakam = item.jammaRakam;
                        objBhuktaniPaune.Add(objPaune);
                    }
                    ViewBag.BhuktaniPaune = objBhuktaniPaune;
                }
                else
                {
                    ViewBag.BhuktaniPaune = null;
                }

            }
            else
            {
                ViewBag.BhuktaniPaune = null;
            }
            return View(objJornal);
        }




        // GET: /JornalEntries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            jornalEntries objJornal = db.jornalEntries.Find(id);
            List<jornalEntries> jornalentries = db.jornalEntries.Where(m => m.jornalNo == objJornal.jornalNo && m.jornalType == objJornal.jornalType && m.fyId == objJornal.fyId && m.monthIndex == objJornal.monthIndex && m.baUSiNaId == objJornal.baUSiNaId).ToList();
            if (jornalentries == null)
            {
                return HttpNotFound();
            }
            else
            {
                var objBhuktani = db.bhuktaniAdeshs.Where(m => m.fyId == objJornal.fyId && m.monthIndex == objJornal.monthIndex && m.jornalKharchaNo == objJornal.jornalNo && m.baUSiNaId == objJornal.baUSiNaId).ToList();
                var objAyaKar = db.ayaKars.Where(m => m.fyId == objJornal.fyId && m.monthIndex == objJornal.monthIndex && m.jornalKharchaNo == objJornal.jornalNo && m.baUSiNaId == objJornal.baUSiNaId).ToList();
                try
                {
                    foreach (var item in jornalentries)
                    {
                        db.jornalEntries.Remove(item);
                    }
                    foreach (var item in objBhuktani)
                    {
                        db.bhuktaniAdeshs.Remove(item);
                    }
                    foreach (var item in objAyaKar)
                    {
                        db.ayaKars.Remove(item);
                    }
                    db.SaveChanges();
                    TempData["Message"] = "Record Deleted Successfully.";
                    return RedirectToAction("Index", "JornalEntries");
                }
                catch
                {
                    TempData["ErrorMessage"] = "No Record Deleted.";
                    return RedirectToAction("Index", "JornalEntries");
                }
            }
        }

        // POST: /JornalEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(String jornalNo)
        {
            List<jornalEntries> jornalentries = db.jornalEntries.Where(m => m.jornalNo == jornalNo).ToList();
            foreach (var item in jornalentries)
            {
                db.jornalEntries.Remove(item);
            }
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
            string prefixTrim = prefix.Trim();
            var objVendors = db.vendors.ToList();
            var vendorName = (from p in objVendors
                              where p.name.Contains(prefixTrim)
                              select new { label = p.name + "," + p.TPIN_PAN, value = p.vendorId }).ToList();
            return Json(vendorName);

        }
        [HttpPost]
        public JsonResult autofill2(string value)
        {
            int id = int.Parse(value);
            var myobj = (from d in db.vendors
                         where d.vendorId == id
                         select new
                         {
                             pauneKoNaam = d.name,
                             panNo = d.TPIN_PAN
                         }).SingleOrDefault();
            return Json(myobj);
        }
        [HttpPost]
        public JsonResult AutoComplete3(string prefix)
        {
            string prefixTrim = prefix.Trim();
            var objByehora = db.byehoras.ToList();
            var byehoraName = (from p in objByehora
                               where p.hisabNo.Contains(prefixTrim)
                               select new { label = p.hisabNo + "," + p.byehoraName, value = p.byehoraId }).ToList();
            return Json(byehoraName);

        }
        [HttpPost]
        public JsonResult autofill3(string value)
        {
            int id = int.Parse(value);
            var myobj = (from d in db.byehoras
                         where d.byehoraId == id
                         select new
                         {
                             khaSirsak = d.byehoraName,
                             khaSiNo = d.hisabNo
                         }).SingleOrDefault();
            return Json(myobj);
        }

        [HttpPost]
        public JsonResult AutoCompleteKhaSirsak(string prefix)
        {
            var objBudgets = db.khaSiNas.ToList();
            string trimPrefix = prefix.Trim();
            var objBudget = (from p in objBudgets
                             where p.khaSiNo.Contains(trimPrefix)
                             select new { label = p.khaSiNo + "," + p.khaSirsak, value = p.khaSiNaId }).ToList();
            return Json(objBudget);

        }
        [HttpPost]
        public JsonResult autofillKhaSirsak(string value)
        {
            int id = int.Parse(value);
            var myobj = (from d in db.khaSiNas
                         where d.khaSiNaId == id
                         select new
                         {
                             khaSiNaId = d.khaSiNaId,
                             khaSiNo = d.khaSiNo,
                             khaSirsak = d.khaSirsak,
                         }).SingleOrDefault();
            return Json(myobj);
        }
        [HttpPost]
        public JsonResult getJornalNo(string baUSiNaId)
        {
            int id = int.Parse(baUSiNaId);
            var objJornal = db.setNumbers.FirstOrDefault(m => m.baUSiNaId == id && m.faramName == "JornalEntries");
            var objBhuktaniAdesh = db.setNumbers.FirstOrDefault(m => m.baUSiNaId == id && m.faramName == "BhuktaniAdesh");
            setNumberNM objSetNum = new setNumberNM();
            objSetNum.jornalNo = "";
            objSetNum.bhuktaniAdeshNo = "";
            if (objJornal != null)
            {
                objSetNum.jornalNo = StringToUnicode(objJornal.number.ToString());
            }
            else
            {
                db.setNumbers.Add(new setNumber { faramName = "JornalEntries", number = 1, baUSiNaId = id });
                db.SaveChanges();
                objSetNum.jornalNo = StringToUnicode("1");
            }
            if (objBhuktaniAdesh != null)
            {
                objSetNum.bhuktaniAdeshNo = StringToUnicode(objBhuktaniAdesh.number.ToString());
            }
            else
            {
                db.setNumbers.Add(new setNumber { faramName = "BhuktaniAdesh", number = 1, baUSiNaId = id });
                db.SaveChanges();
                objSetNum.bhuktaniAdeshNo = StringToUnicode("1");
            }
            return Json(objSetNum);
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
        public JsonResult getBhuktaniNo(string fyId, string monthIndex, string baUSiNaId)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                int baUSiNoId = Convert.ToInt32(baUSiNaId);
                var uniqBhuktani = (from d in db.bhuktaniAdeshs
                                    where d.fyId == fisId && d.monthIndex == mn && d.baUSiNaId == baUSiNoId
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
        public JsonResult checkJornalNo(string jornalNo, string jornalType, string fyId, string baUSiNaId)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int baUSiNoId = Convert.ToInt32(baUSiNaId);
                var objJornal = new List<jornalEntries>();
                if (jornalType == "खर्च" || jornalType == "निकासा" || jornalType == "कट्टी")
                {
                    objJornal = db.jornalEntries.Where(m => m.jornalNo == jornalNo && (m.jornalType == "खर्च" || m.jornalType == "निकासा" || m.jornalType == "कट्टी") && m.fyId == fisId && m.baUSiNaId == baUSiNoId).ToList();

                }
                else if (jornalType == "राजश्व")
                {
                    objJornal = db.jornalEntries.Where(m => m.jornalNo == jornalNo && (m.jornalType == "राजश्व आम्दानी" || m.jornalType == "राजश्व दाखिला") && m.fyId == fisId).ToList();

                }
                else if (jornalType == "धरौटी")
                {
                    objJornal = db.jornalEntries.Where(m => m.jornalNo == jornalNo && (m.jornalType == "धरौटी आम्दानी" || m.jornalType == "धरौटी फिर्ता") && m.fyId == fisId).ToList();

                }
                if (objJornal.Count > 0)
                {
                    return Json("NotNull");
                }
                else
                {
                    return Json("Null");
                }
            }
            catch
            {
                return Json("NotNull");
            }
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



    }
}
