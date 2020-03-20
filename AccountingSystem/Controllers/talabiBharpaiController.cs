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
using System.Globalization;
using AccountingSystem.ViewModel;
using AccountingSystem.Reports;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using AccountingSystem.Controllers;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class talabiBharpaiController : Controller
    {
        private AccountContext db = new AccountContext();

        //public ActionResult Index()
        //{
        //    List<talabiBharpai> talabis = new List<talabiBharpai>();
        //    var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
        //    ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
        //    return View(talabis);
        //}
        //[HttpPost]
        //public ActionResult Index(FormCollection col)
        //{
        //    int fyId = Convert.ToInt32(col["fyId"]);
        //    int mn = Convert.ToInt32(col["month"]);
        //    string year = getYear(fyId, mn);
        //    ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
        //    var objYearMonth = db.yearMonths.FirstOrDefault(m => (m.fyId == fyId) && (m.monthIndex == mn));
        //    if (objYearMonth == null)
        //    {
        //        ViewBag.ErrorMessage = "No Record Found";
        //        List<talabiBharpai> talabis = new List<talabiBharpai>();
        //        return View(talabis);
        //    }
        //    List<talabiBharpai> talabiBharpais = db.talabiBharpais.Include(m => m.officer).Include(m => m.yearMonth).OrderBy(m=>m.officerId).ToList();
        //    int yearMonthId = db.yearMonths.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn).yearMonthId;
        //    var filTalaBhiBharpais = talabiBharpais.Where(m => m.yearMonthId == yearMonthId);
        //    return View(filTalaBhiBharpais.ToList());
        //}
        //public ActionResult Create()
        //{
        //    List<talabiBharpai> talabi = new List<talabiBharpai>();
        //    decimal grade, suruScaleGrade, kaSaKoThap, suruBimaTotal, talabiBhattaTotal, kaSaKoKatti, naLaKos, pariKar, kattiTotal, pauneTotal, saSuKar;
        //    if (db.officers.ToList().Count > 0)
        //    {

        //        var listOfficer = db.officers.Where(m=>m.status=="कार्यरत").OrderBy(m => m.serialNo).ToList();

        //        foreach (officer offr in listOfficer)
        //        {
        //            grade = offr.gradeSankhya * offr.gradeDar;
        //            suruScaleGrade = offr.suruScale+ grade;
        //            kaSaKoThap = 0;
        //            if (offr.jobType == "स्थाई")
        //            {
        //                kaSaKoThap = suruScaleGrade * offr.kaSaKos / 100;
        //            }
        //            suruBimaTotal = suruScaleGrade + offr.bima +kaSaKoThap;
        //            talabiBhattaTotal = suruBimaTotal + offr.mahangiBhatta + offr.jokhimBhatta;
        //            kaSaKoKatti = 2 *  kaSaKoThap ;
        //            naLaKos =offr.naLaKos*(suruScaleGrade)/100; 
        //            saSuKar = offr.saSukar;
        //            pariKar = offr.paKar;
        //           // pariKar = CalculatePariKar(talabiBhattaTotal);
        //            kattiTotal =2*kaSaKoThap + pariKar +naLaKos + 2 * offr.bima + saSuKar;
        //            pauneTotal = talabiBhattaTotal - kattiTotal;
        //            talabi.Add(new talabiBharpai { talabiId = 0, officer = offr,pariKar=pariKar, officerId = offr.officerId, suruScaleGrade = suruScaleGrade, kaSaKoThap = kaSaKoThap, suruBimaTotal = suruBimaTotal, talabiBhattaTotal = talabiBhattaTotal, kaSaKoKatti = kaSaKoKatti, sapati = 0, naLaKos = naLaKos, saSuKar = saSuKar, kattiTotal = kattiTotal, pauneTotal = pauneTotal, yearMonthId = 1 ,fyId=1});
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.ErrorMessage = "No Record found";
        //        return RedirectToAction("Create", "officer");
        //    }
        //    var objFiscalYear = db.fisYears.FirstOrDefault(m=>m.status==true);
        //    ViewBag.fyId = new SelectList(db.fisYears,"fyId","nepFy",objFiscalYear.fyId);
        //    return View(talabi);

        //}
        //[HttpPost]
        //public ActionResult Create(List<talabiBharpai> talabi, FormCollection col)
        //{
        //    decimal grade, suruScaleGrade, kaSaKoThap, suruBimaTotal, talabiBhattaTotal, kaSaKoKatti, naLaKos, pariKar, kattiTotal, pauneTotal, saSuKar;
        //    if (ModelState.IsValid)
        //    {
        //        int mn = Convert.ToInt32(col["month"]);
        //        int fyId = Convert.ToInt32(col["fyId"]);
        //        string nepYear = getYear(fyId, mn);
        //        var objYearMonth = new yearMonth();
        //        if (db.yearMonths==null)
        //        {
        //            db.yearMonths.Add(new yearMonth {fyId=fyId,year=nepYear, month = ((month)mn).ToString(),date=col["nepDate"],monthIndex=mn });
        //            db.SaveChanges();
        //            objYearMonth = db.yearMonths.FirstOrDefault(m => (m.month == ((month)mn).ToString()) && (m.fyId == fyId)&&(m.date==col["nepDate"]));
        //        }
        //        else
        //        {
        //            var objym = db.yearMonths.FirstOrDefault(m => (m.month == ((month)mn).ToString()) && (m.fyId == fyId)&&(m.monthIndex==mn));
        //            if (objym == null)
        //            {
        //                db.yearMonths.Add(new yearMonth { fyId=fyId, year = nepYear, month = ((month)mn).ToString(), date = col["nepDate"] ,monthIndex=mn});
        //                db.SaveChanges();
        //            }
                       
        //            else
        //            {
        //                var query = db.yearMonths.FirstOrDefault(m=>(m.month == ((month)mn).ToString()) && (m.fyId == fyId) && (m.monthIndex == mn));
                        
        //                    query.year = nepYear;
        //                    query.month = ((month)mn).ToString();
        //                    query.monthIndex = mn;
        //                    query.date = col["nepDate"];
        //                db.SaveChanges();
        //            }
        //            objYearMonth = db.yearMonths.ToList().First(m => (m.month == ((month)mn).ToString()) && (m.fyId == fyId) && (m.date == col["nepDate"])&&(m.monthIndex==mn));
                 
        //        }
        //        var talabis = db.talabiBharpais.Where(m => (m.yearMonthId == objYearMonth.yearMonthId) && (m.fyId==fyId)).ToList();
        //        foreach (talabiBharpai item in talabis)
        //        {
        //            db.talabiBharpais.Remove(item);
        //            db.SaveChanges();
        //        }
        //        int j = 0;
        //        int yearMonthId = objYearMonth.yearMonthId;
        //        foreach (talabiBharpai tb in talabi)
        //        {
        //            var objOfficer = db.officers.Find(tb.officerId);
        //            grade = objOfficer.gradeSankhya * objOfficer.gradeDar;
        //            suruScaleGrade = objOfficer.suruScale + grade;
        //            kaSaKoThap = tb.kaSaKoThap;
        //            suruBimaTotal = suruScaleGrade + objOfficer.bima + tb.kaSaKoThap;
        //            talabiBhattaTotal = suruBimaTotal + objOfficer.mahangiBhatta + objOfficer.jokhimBhatta;
        //            kaSaKoKatti = 2 *kaSaKoThap;
        //            naLaKos =  tb.naLaKos;
        //            saSuKar = objOfficer.saSukar;
        //            pariKar = tb.pariKar;
        //            kattiTotal = kaSaKoKatti + pariKar +  tb.naLaKos + 2 * objOfficer.bima + saSuKar+tb.sapati;
        //            pauneTotal = talabiBhattaTotal - kattiTotal;

        //            talabiBharpai objTalabi = new talabiBharpai();
        //              objTalabi.officerId = tb.officerId;
        //              objTalabi.suruScaleGrade = suruScaleGrade;
        //              objTalabi.kaSaKoThap = kaSaKoThap;
        //              objTalabi.suruBimaTotal = suruBimaTotal;
        //              objTalabi.talabiBhattaTotal = talabiBhattaTotal; 
        //              objTalabi.kaSaKoKatti = kaSaKoKatti;
        //              objTalabi.sapati = tb.sapati;
        //              objTalabi.naLaKos = naLaKos;
        //              objTalabi.pariKar = tb.pariKar;
        //              objTalabi.saSuKar = saSuKar;
        //              objTalabi.kattiTotal = kattiTotal;
        //              objTalabi.pauneTotal = pauneTotal;
        //              objTalabi.yearMonthId = yearMonthId;
        //              objTalabi.fyId = fyId;
        //              db.talabiBharpais.Add(objTalabi);

        //            //db.talabiBharpais.Add(new talabiBharpai { 
        //            //    officerId = tb.officerId,
        //            //    suruScaleGrade = tb.suruScaleGrade,
        //            //    kaSaKoThap = tb.kaSaKoThap,
        //            //    suruBimaTotal = tb.suruBimaTotal,
        //            //    talabiBhattaTotal = tb.talabiBhattaTotal,
        //            //    kaSaKoKatti = tb.kaSaKoKatti,
        //            //    sapati = tb.sapati,
        //            //    naLaKos = tb.naLaKos,
        //            //    pariKar = CalculatePariKar(tb.talabiBhattaTotal),
        //            //    saSuKar = tb.saSuKar,
        //            //    kattiTotal = (tb.kattiTotal + tb.sapati+tb.naLaKos),
        //            //    pauneTotal = (tb.pauneTotal - tb.sapati-tb.naLaKos),
        //            //    yearMonthId = yearMonthId ,
        //            //    fyId=fyId
        //            //});
        //            j++;
        //        }
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
        //    ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
        //    return View(talabi);
        //}
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            talabiBharpai objTalabi = db.talabiBharpais.Find(id);
            List<talabiBharpai> talabibharpais = db.talabiBharpais.Where(m => m.yearMonthId == objTalabi.yearMonthId).ToList();
            if (talabibharpais == null)
            {
                return HttpNotFound();
            }
            else
            {
                var yearMonth = db.yearMonths.Find(objTalabi.yearMonthId);
                
                 try
                {
                    foreach (var item in talabibharpais)
                    {
                        db.talabiBharpais.Remove(item);
                    }
                    db.yearMonths.Remove(yearMonth);
                    db.SaveChanges();
                    TempData["Message"] = "Record Deleted Successfully.";
                    return RedirectToAction("Index", "talabiBharpai");
                }
                catch
                {
                    TempData["ErrorMessage"] = "No Record Deleted.";
                    return RedirectToAction("Index", "talabiBharpai");
                }
            }
        }

        public ActionResult TalabiBharpaiBibaran(string type,int fyId, int monthIndex)
        {
            return View();
        }
        [HttpPost]
        public ActionResult TalabiBharpaiBibaran()
        {
            return View();
        }


        public decimal CalculatePariKar(decimal talabiBhattaTotal)
        {
            decimal total = talabiBhattaTotal * 13 + 7500;
            if (total > 350000)
            {
                decimal pariKar = ((total - 350000) * 15) / 1200;
                return pariKar;
            }
            else
            {
                return 0;
            }
        }

        [HttpPost]
        public JsonResult getYearMonth()
        {
            var yearMonth = db.yearMonths.ToList();
            return Json(yearMonth);
        }





        //string[] nepaliNum = new string[] { "सुन्य", "एक", "दुई", "तीन", "चार", "पाँच", "छ", "सात", "आठ", "नौ", "दस", "एघार", "बाह्र", "तेह्र", "चौध", "पन्ध्र", "सोह्र", "सत्र", "अठाह्र", "उन्नाइस", "बीस", "एकाइस", "बाइस", "तेइस", "चौबीस", "पचीस", "छब्बीस", "सत्ताइस", "अठ्ठाइस", "उनन्तीस", "तीस", "एकतीस", "बतीस", "तेतीस", "चौतीस", "पैतीस", "छतीस", "सरतीस", "अरतीस", "उननचालीस", "चालीस", "एकचालीस", "बयालिस", "तीरचालीस", "चौवालिस", "पैंतालिस", "छयालिस", "सरचालीस", "अरचालीस", "उननचास", "पचास", "एकाउन्न", "बाउन्न", "त्रिपन्न", "चौवन्न", "पच्पन्न", "छपन्न", "सन्ताउन्न", "अन्ठाउँन्न", "उनान्न्साठी ", "साठी", "एकसाठी", "बासाठी", "तीरसाठी", "चौंसाठी", "पैसाठी", "छैसठी", "सत्सठ्ठी", "अर्सठ्ठी", "उनन्सत्तरी", "सतरी", "एकहत्तर", "बहत्तर", "त्रिहत्तर", "चौहत्तर", "पचहत्तर", "छहत्तर", "सत्हत्तर", "अठ्हत्तर", "उनास्सी", "अस्सी", "एकासी", "बयासी", "त्रीयासी", "चौरासी", "पचासी", "छयासी", "सतासी", "अठासी", "उनान्नब्बे", "नब्बे", "एकान्नब्बे", "बयान्नब्बे", "त्रियान्नब्बे", "चौरान्नब्बे", "पंचान्नब्बे", "छयान्नब्बे", "सन्तान्‍नब्बे", "अन्ठान्नब्बे", "उनान्सय" };
 
        //public string NumToWords(string num)
        //{
        //   string num1 = num.Split('.')[0];
        //   string num2 = num.Split('.')[1].Substring(0, 2);
        //    string str="";
        //    int charCount = num1.Length;
        //    if (num1.Length > 10 || num1.Length==0)
        //    {
        //        str = "Enter number less than 1 अर्ब and greater than 0";
        //        return str;
        //    }
        //    if (num1.Length != 10)
        //    {
        //        for (int i = 0; i <10-charCount; i++)
        //        {
        //            num1 = "0"+num1; 
        //        }
        //    }
        //    int[] place = new int[6];
        //    place[0] = Convert.ToInt32(num1.Substring(1,1));
        //    place[1] = Convert.ToInt32(num1.Substring(1,2));
        //    place[2] = Convert.ToInt32(num1.Substring(3,2));
        //    place[3] = Convert.ToInt32(num1.Substring(5,2));
        //    place[4]= Convert.ToInt32(num1.Substring(7,1));
        //    place[5] = Convert.ToInt32(num1.Substring(8,2));

        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (place[i] != 0)
        //        {
        //            switch (i)
        //            { 
        //                case 0:
        //                    str+=nepaliNum[place[i]]+" "+ "अरब ";
        //                    break;
        //                case 1:
        //                    str += nepaliNum[place[i]] + " " + "करोड ";
        //                    break;
        //                case 2:
        //                    str += nepaliNum[place[i]] + " " + "लाख ";
        //                    break;
        //                case 3:
        //                    str += nepaliNum[place[i]] + " " + "हजार ";
        //                    break;
        //                case 4:
        //                    str += nepaliNum[place[i]] + " " + "सय ";
        //                    break;
        //                case 5:
        //                    str += nepaliNum[place[i]] ;
        //                    break;
        //                default:
        //                    str +=" सुन्य ";
        //                    break;
        //            }
        //        }
        //    }
        //    if (Convert.ToInt32(num2) != 0)
        //    {
        //        str +=" "+ nepaliNum[Convert.ToInt32(num2)] + " पैसा";
        //    }
        //    str += " " + "मात्र |";
        //    return str;
        //}

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
                string year = getYear(fisId, mn);
                var objYearMonth = db.yearMonths.FirstOrDefault(m => m.year == year && m.monthIndex == mn);
               // var objBhuktani = db.talabiBharpais.Where(m => (m.fyId == fisId) && (m.monthIndex == mn) && (m.bhuktaniType == "भुक्तानी आदेश तलबी")).ToList();
                if (objYearMonth !=null)
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






        public ActionResult Create()
        {
            List<VMTalabiBharpai> talabi = new List<VMTalabiBharpai>();
            decimal grade, suruScaleGrade, kaSaKoThap, suruBimaTotal, talabiBhattaTotal, kaSaKoKatti, naLaKos, pariKar, kattiTotal, pauneTotal, saSuKar,sapatiRakam;
            if (db.officers.ToList().Count > 0)
            {
                var listOfficer = db.officers.Where(m => m.status == "कार्यरत").OrderBy(m => m.serialNo).ToList();
                foreach (officer offr in listOfficer)
                {
                    grade =decimal.Round(offr.gradeSankhya * offr.gradeDar,2,MidpointRounding.AwayFromZero);
                    suruScaleGrade = offr.suruScale + grade;
                    kaSaKoThap = 0;
                    if (offr.jobType == "स्थाई")
                    {
                        kaSaKoThap =decimal.Round(suruScaleGrade * offr.kaSaKos / 100,2,MidpointRounding.AwayFromZero);
                    }
                    suruBimaTotal = suruScaleGrade + offr.bima + kaSaKoThap;
                    talabiBhattaTotal = suruBimaTotal + offr.mahangiBhatta + offr.jokhimBhatta;
                    kaSaKoKatti = 2 * kaSaKoThap;
                    naLaKos =decimal.Round(offr.naLaKos * (suruScaleGrade) / 100,2,MidpointRounding.AwayFromZero);
                    saSuKar = offr.saSukar;
                    pariKar = offr.paKar;
                    sapatiRakam = offr.sapati;
                    // pariKar = CalculatePariKar(talabiBhattaTotal);
                    kattiTotal = 2 * kaSaKoThap + pariKar + naLaKos + 2 * offr.bima + saSuKar;
                    pauneTotal = talabiBhattaTotal - kattiTotal;// pariKar = pariKar,
                    talabi.Add(new VMTalabiBharpai { talabiId = 0,fullName=offr.fullName,darja=offr.darja, officerId = offr.officerId, suruScale = offr.suruScale, gradeRakam = grade, suruScaleGrade = suruScaleGrade, kaSaKoThap = kaSaKoThap, bima = offr.bima, suruBimaTotal = suruBimaTotal, mahangiBhatta = offr.mahangiBhatta, jokhimBhatta = offr.jokhimBhatta, talabiBhattaTotal = talabiBhattaTotal, kaSaKoKatti = kaSaKoKatti, sapati = sapatiRakam, naLaKos = naLaKos,pariKar=pariKar, saSuKar = saSuKar, kattiTotal = kattiTotal, pauneTotal = pauneTotal, yearMonthId = 1, fyId = 1 });
                }
            }
            else
            {
                ViewBag.ErrorMessage = "No Record found";
                return RedirectToAction("Create", "officer");
            }
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(talabi);

        }
        [HttpPost]
        public ActionResult Create(List<VMTalabiBharpai> talabi, FormCollection col)
        {
            decimal grade, suruScaleGrade, kaSaKoThap, suruBimaTotal, talabiBhattaTotal, kaSaKoKatti, naLaKos, pariKar, kattiTotal, pauneTotal, saSuKar;
            if (ModelState.IsValid)
            {
                int mn = Convert.ToInt32(col["month"]);
                int fyId = Convert.ToInt32(col["fyId"]);
                string nepYear = getYear(fyId, mn);
                var objYearMonth = new yearMonth();
                if (db.yearMonths == null)
                {
                    db.yearMonths.Add(new yearMonth { fyId = fyId, year = nepYear, month = ((month)mn).ToString(), date = col["nepDate"], monthIndex = mn });
                    db.SaveChanges();
                    objYearMonth = db.yearMonths.FirstOrDefault(m => (m.month == ((month)mn).ToString()) && (m.fyId == fyId) && (m.date == col["nepDate"]));
                }
                else
                {
                    var objym = db.yearMonths.FirstOrDefault(m => (m.month == ((month)mn).ToString()) && (m.fyId == fyId) && (m.monthIndex == mn));
                    if (objym == null)
                    {
                        db.yearMonths.Add(new yearMonth { fyId = fyId, year = nepYear, month = ((month)mn).ToString(), date = col["nepDate"], monthIndex = mn });
                        db.SaveChanges();
                    }

                    else
                    {
                        var query = db.yearMonths.FirstOrDefault(m => (m.month == ((month)mn).ToString()) && (m.fyId == fyId) && (m.monthIndex == mn));

                        query.year = nepYear;
                        query.month = ((month)mn).ToString();
                        query.monthIndex = mn;
                        query.date = col["nepDate"];
                        db.SaveChanges();
                    }
                    objYearMonth = db.yearMonths.ToList().First(m => (m.month == ((month)mn).ToString()) && (m.fyId == fyId) && (m.date == col["nepDate"]) && (m.monthIndex == mn));

                }
                var talabis = db.talabiBharpais.Where(m => (m.yearMonthId == objYearMonth.yearMonthId) && (m.fyId == fyId)).ToList();
                foreach (talabiBharpai item in talabis)
                {
                    db.talabiBharpais.Remove(item);
                    db.SaveChanges();
                }
                int j = 0;
                int yearMonthId = objYearMonth.yearMonthId;
                foreach (VMTalabiBharpai tb in talabi)
                {
                    var objOfficer = db.officers.Find(tb.officerId);
                    grade =tb.gradeRakam;// objOfficer.gradeSankhya * objOfficer.gradeDar;
                    suruScaleGrade = tb.suruScale + tb.gradeRakam;// objOfficer.suruScale + grade;
                    kaSaKoThap = tb.kaSaKoThap;
                    suruBimaTotal = suruScaleGrade + tb.bima + tb.kaSaKoThap;// suruScaleGrade + objOfficer.bima + tb.kaSaKoThap;
                    talabiBhattaTotal = suruBimaTotal + tb.mahangiBhatta + tb.jokhimBhatta;// suruBimaTotal + objOfficer.mahangiBhatta + objOfficer.jokhimBhatta;
                    kaSaKoKatti = 2 * kaSaKoThap;
                    naLaKos = tb.naLaKos;
                    saSuKar = tb.saSuKar;// objOfficer.saSukar;
                    pariKar = tb.pariKar;
                    kattiTotal = kaSaKoKatti + pariKar + tb.naLaKos + 2 * tb.bima + saSuKar + tb.sapati; //kaSaKoKatti + pariKar + tb.naLaKos + 2 * objOfficer.bima + saSuKar + tb.sapati;
                    pauneTotal = talabiBhattaTotal - kattiTotal;

                    talabiBharpai objTalabi = new talabiBharpai();
                    objTalabi.officerId = tb.officerId;
                    objTalabi.suruScaleGrade = suruScaleGrade;
                    objTalabi.kaSaKoThap = kaSaKoThap;
                    objTalabi.suruBimaTotal = suruBimaTotal;
                    objTalabi.talabiBhattaTotal = talabiBhattaTotal;
                    objTalabi.kaSaKoKatti = kaSaKoKatti;
                    objTalabi.sapati = tb.sapati;
                    objTalabi.naLaKos = naLaKos;
                    objTalabi.pariKar = tb.pariKar;
                    objTalabi.saSuKar = saSuKar;
                    objTalabi.kattiTotal = kattiTotal;
                    objTalabi.pauneTotal = pauneTotal;
                    objTalabi.yearMonthId = yearMonthId;
                    objTalabi.fyId = fyId;
                    objTalabi.suruScale = tb.suruScale;//Modified 
                    objTalabi.gradeRakam = tb.gradeRakam;
                    objTalabi.bima = tb.bima;
                    objTalabi.mahangiBhatta = tb.mahangiBhatta;
                    objTalabi.jokhimBhatta = tb.jokhimBhatta;
                    db.talabiBharpais.Add(objTalabi);

                    //db.talabiBharpais.Add(new talabiBharpai { 
                    //    officerId = tb.officerId,
                    //    suruScaleGrade = tb.suruScaleGrade,
                    //    kaSaKoThap = tb.kaSaKoThap,
                    //    suruBimaTotal = tb.suruBimaTotal,
                    //    talabiBhattaTotal = tb.talabiBhattaTotal,
                    //    kaSaKoKatti = tb.kaSaKoKatti,
                    //    sapati = tb.sapati,
                    //    naLaKos = tb.naLaKos,
                    //    pariKar = CalculatePariKar(tb.talabiBhattaTotal),
                    //    saSuKar = tb.saSuKar,
                    //    kattiTotal = (tb.kattiTotal + tb.sapati+tb.naLaKos),
                    //    pauneTotal = (tb.pauneTotal - tb.sapati-tb.naLaKos),
                    //    yearMonthId = yearMonthId ,
                    //    fyId=fyId
                    //});
                    j++;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(talabi);
        }




        public ActionResult Index()
        {
            List<VMTalabiBharpai> talabis = new List<VMTalabiBharpai>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View(talabis);
        }
        [HttpPost]
        public ActionResult Index(FormCollection col)
        {
            int fyId = Convert.ToInt32(col["fyId"]);
            int mn = Convert.ToInt32(col["month"]);
            string year = getYear(fyId, mn);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            var objYearMonth = db.yearMonths.FirstOrDefault(m => (m.fyId == fyId) && (m.monthIndex == mn));
            if (objYearMonth == null)
            {
                ViewBag.ErrorMessage = "No Record Found";
                List<VMTalabiBharpai> talabis = new List<VMTalabiBharpai>();
                return View(talabis);
            }
            int yearMonthId = db.yearMonths.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn).yearMonthId;
            //List<VMTalabiBharpai>
            var talabiBharpais = (from t in db.talabiBharpais
                                  join o in db.officers on t.officerId equals o.officerId
                                  join y in db.yearMonths on t.yearMonthId equals y.yearMonthId
                                  where t.yearMonthId == yearMonthId
                                  select new
                                  {
                                      t.talabiId,
                                      o.fullName,
                                      o.darja,
                                      t.suruScale,
                                      t.gradeRakam,
                                      t.suruScaleGrade,
                                      t.kaSaKoThap,
                                      t.bima,
                                      t.suruBimaTotal,
                                      t.mahangiBhatta,
                                      t.jokhimBhatta,
                                      t.talabiBhattaTotal,
                                      t.kaSaKoKatti,
                                      t.sapati,
                                      t.naLaKos,
                                      t.pariKar,
                                      t.saSuKar,
                                      t.kattiTotal,
                                      t.pauneTotal,
                                      t.yearMonthId,
                                      t.fyId
                                  }).ToList();
            List<VMTalabiBharpai> filTalabi = new List<VMTalabiBharpai>();

            foreach (var item in talabiBharpais)
            {
                VMTalabiBharpai objTalabi = new VMTalabiBharpai();
                filTalabi.Add(new VMTalabiBharpai
                {
                    talabiId = item.talabiId,
                    fullName=item.fullName,
                    darja=item.darja,
                    suruScale=item.suruScale,
                    gradeRakam=item.gradeRakam,
                    suruScaleGrade=item.suruScaleGrade,
                    kaSaKoThap=item.kaSaKoThap,
                    bima=item.bima,
                    suruBimaTotal=item.suruBimaTotal,
                    mahangiBhatta=item.mahangiBhatta,
                    jokhimBhatta=item.jokhimBhatta,
                    talabiBhattaTotal=item.talabiBhattaTotal,
                    kaSaKoKatti=item.kaSaKoKatti,
                    sapati=item.sapati,
                    naLaKos=item.naLaKos,
                    pariKar=item.pariKar,
                    saSuKar=item.saSuKar,
                    kattiTotal =item.kattiTotal,
                    pauneTotal=item.pauneTotal,
                    yearMonthId=item.yearMonthId,
                    fyId=item.fyId
                });
                                      
            }
            return View(filTalabi);
        }






        
    }
}