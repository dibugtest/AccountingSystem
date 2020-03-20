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
using AccountingSystem.ViewModel;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class KharchaKoFantwariController : Controller
    {
        private AccountContext db = new AccountContext();
        //
        // GET: /KharchaKoFantwari/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArthaBudget()
        {
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
            return View();
        }

        [HttpPost]
        public ActionResult ArthaBudget(int fyId, int mn)
        {
            try
            {
                var objArtha = db.arthaBudgets.Where(m => m.fyId == fyId && m.monthIndex == mn).ToList();
                if (objArtha.Count == 0)
                {
                    return View();
                }
                else
                {
                    return View(objArtha);
                }
            }
            catch
            {
                return View();
            }

        }

        public ActionResult Fantwari()
        {
            var objBaUsiNo = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
            if (objBaUsiNo != null)
            {
                ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo", objBaUsiNo.baUSiNaId);
            }
            else
            {
                ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo");
            }
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);

            return View();
        }

        public ActionResult KharchaFantwari()
        {
            return RedirectToAction("Fantwari");

        }

        [HttpPost]
        public ActionResult KharchaFantwari(int baUSiNaId, int mn, int fyId)
        {
            //Check for Input Fields
            if (baUSiNaId == 0 || baUSiNaId == 0 || fyId == 0)
            {
                TempData["ErrorMessage"] = "Please Select BaUSiNa and Year to submit";
                return RedirectToAction("Fantwari");
            }


            var objBaUSiNa = db.baUSiNas.Find(baUSiNaId);
            var objFiscalYear = db.fisYears.Find(fyId);
            var objKhaSiNas = db.khaSiNas.Where(m => m.baUSiNaId == baUSiNaId).ToList();
            string selYear = getYear(fyId, mn);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);

            //Get List of jornalEntries of types "खर्च" and "पेश्की" and "पेश्की फछर्यौट" for upto selected month
            List<jornalEntries> objJornal = db.jornalEntries
                                            .Where(m => (m.jornalType == "खर्च" || m.jornalType == "पेश्की" || m.jornalType == "पेश्की फछर्यौट") &&
                                                (m.fyId == fyId) && (m.monthIndex <= mn)
                                                && (m.baUSiNaId == baUSiNaId)).ToList();
            if (objJornal.Count == 0)//&& objJornalPeskiUpto.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found";
                return RedirectToAction("Fantwari");
            }


            //Unique jornalNo for jornal list
            var uniqJornal = (from d in objJornal
                              group d by new { d.jornalNo }
                                          into mygroup
                              select mygroup.FirstOrDefault()).ToList();


            List<string> uniqJornalNo = (from a in uniqJornal
                                         select a.jornalNo.ToString()).ToList();

            //get list of jornalentries for type "पेश्की फछर्यौट" for upto selected month
            List<jornalEntries> objJornalF = db.jornalEntries
                                          .Where(m => (m.jornalType == "पेश्की फछर्यौट") &&
                                              (m.fyId == fyId) && (m.monthIndex <= mn)
                                              && (m.baUSiNaId == baUSiNaId)).ToList();

            var uniqJornalF = (from d in objJornalF
                               where d.sanketNo != "" && d.sanketNo != null
                               group d by new { d.jornalNo }
                                          into mygroup
                               select mygroup.FirstOrDefault()
                                ).ToList();

            List<string> uniqJornalNoF = (from a in uniqJornalF
                                          select a.sanketNo.ToString()).ToList();
            //All JornalNos 
            List<string> uniqJornalAll = uniqJornalNo.Except(uniqJornalNoF).ToList();

            //Obtain List of KharchaKoFantwari viewModel for all khaSiNas
            List<kharchaKoFantwari> objKharcha = new List<kharchaKoFantwari>();
            foreach (var item in objKhaSiNas)
            {
                objKharcha.Add(new kharchaKoFantwari { khaSiNo = item.khaSiNo, khaSirsak = item.khaSirsak, budgetThisYear = item.khaRakam, nepFy = objFiscalYear.nepFy, baUSiNo = objBaUSiNa.baUSiNo, year = selYear, month = ((month)mn).ToString() });
            }

            List<jornalEntries> objJornalList = new List<jornalEntries>();

            foreach (var item in uniqJornalAll)
            {
                List<jornalEntries> objJor = objJornal.Where(m => m.jornalNo == item).ToList();
                objJornalList = objJornalList.Concat(objJor).ToList();
            }


            foreach (var item in objKhaSiNas)
            {
                var objKharcha2 = objKharcha.FirstOrDefault(m => m.khaSiNo == item.khaSiNo);


                //Check jornalentries for khaSiNas and selected month, Calculate total for KhaSiNa
                if (objJornal.Any(m => m.hisabNo == item.khaSiNo))
                {
                    objKharcha2.kharchaThisMonth = objJornalList.Where(m => m.hisabNo == item.khaSiNo && m.monthIndex == mn && m.deCre == "debit").Sum(m => m.debit);
                    objKharcha2.kharchaTillThisMonth = objJornalList.Where(m => m.hisabNo == item.khaSiNo && m.monthIndex <= mn && m.deCre == "debit").Sum(m => m.debit);
                }

            }

            //Calculation for Peski in report
            ViewBag.FarchinaBaki = 0;
            {
                decimal farchinaBaki = objJornalList.Where(m => m.jornalType == "पेश्की" && m.deCre == "debit").Sum(m => m.debit);// objJornalFarchina.Where(m => m.jornalType == "पेश्की फछर्यौत" && m.deCre == "credit" && m.hisabNo != null).Sum(m => m.credit);
                //List<jornalEntries> ferJor = objJornalList.Where(m => m.jornalType == "पेश्की" && m.deCre == "debit").ToList();
                ViewBag.FarchinaBaki = farchinaBaki;
            }


            //For Artha Budget 
            var objArtha = db.arthaBudgets.Where(m => m.fyId == fyId && m.monthIndex == mn).ToList();
            decimal khaSiNa3, khaSiNa4;
            if (objArtha.Count > 0)
            {
                khaSiNa3 = objArtha.Sum(m => m.khaSiNa3);
                khaSiNa4 = objArtha.Sum(m => m.khaSiNa4);
                ViewBag.IsNullArtha = "NotNull";
            }
            else
            {
                khaSiNa3 = 0;
                khaSiNa4 = 0;
                ViewBag.IsNullArtha = "Null";
            }

            ViewBag.khaSiNa3 = khaSiNa3;
            ViewBag.khaSiNa4 = khaSiNa4;

            return View(objKharcha.ToList());
        }




        [HttpPost]
        public ActionResult KharchaFantwari2(int baUSiNaId, int mn, int fyId)
        {
            //Check for Input Fields
            if (baUSiNaId == 0 || baUSiNaId == 0 || fyId == 0)
            {
                TempData["ErrorMessage"] = "Please Select BaUSiNa and Year to submit";
                return RedirectToAction("Fantwari");
            }


            var objBaUSiNa = db.baUSiNas.Find(baUSiNaId);
            var objFiscalYear = db.fisYears.Find(fyId);
            var objKhaSiNas = db.khaSiNas.Where(m => m.baUSiNaId == baUSiNaId).ToList();
            string selYear = getYear(fyId, mn);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);

            //Get List of jornalEntries of types "खर्च" and "पेश्की" for selected month
            List<jornalEntries> objJornal = db.jornalEntries
                                            .Where(m => (m.jornalType == "खर्च" || m.jornalType == "पेश्की") &&
                                                (m.fyId == fyId) && (m.month == ((month)mn).ToString())
                                                && (m.baUSiNaId == baUSiNaId)).ToList();

            //Check for jornalEntries of Type "पेश्की फेर्चौत" Add them to objJornal of selected month
            if (db.jornalEntries.Any(m => m.jornalType == "पेश्की फछर्यौट" &&
                                (m.fyId == fyId) && (m.month == ((month)mn).ToString())
                                && m.bibaran == "एकल खाता कोष"))
            {
                List<jornalEntries> objJornalFer = db.jornalEntries.Where(m => m.jornalType == "पेश्की फछर्यौट" &&
                                                    (m.fyId == fyId) && (m.month == ((month)mn).ToString())
                                                     && m.bibaran == "एकल खाता कोष").ToList();
                objJornal.Concat(objJornalFer);

            }



            //Check for jornalEntries of Type "पेश्की फेर्चौत", Add them to objJornal of selected month
            var objJornalPeskiUpto = db.jornalEntries.Where(m => m.jornalType == "पेश्की फछर्यौट"
                                        && (m.fyId == fyId) && (m.monthIndex <= mn) && (m.baUSiNaId == baUSiNaId)).ToList();


            //Find Uniquq JornalEntries 
            var uniqJornalPeskiUpto = (from d in objJornalPeskiUpto
                                       group d by new { d.jornalNo }
                                           into mygroup
                                       select mygroup.FirstOrDefault()).ToList();


            if (objJornal.Count == 0 && objJornalPeskiUpto.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found";
                return RedirectToAction("Fantwari");
            }

            //Obtain List of KharchaKoFantwari viewModel for all khaSiNas
            List<kharchaKoFantwari> objKharcha = new List<kharchaKoFantwari>();
            foreach (var item in objKhaSiNas)
            {
                objKharcha.Add(new kharchaKoFantwari { khaSiNo = item.khaSiNo, khaSirsak = item.khaSirsak, budgetThisYear = item.khaRakam, nepFy = objFiscalYear.nepFy, baUSiNo = objBaUSiNa.baUSiNo, year = selYear, month = ((month)mn).ToString() });
            }



            foreach (var item in objKhaSiNas)
            {
                var objKharcha2 = objKharcha.FirstOrDefault(m => m.khaSiNo == item.khaSiNo);

                //Find All Kharcha JornalEntries of type debit for each KhaSiNas
                List<jornalEntries> objJornalKharcha = db.jornalEntries.Where(m => (m.jornalType == "खर्च" || m.jornalType == "पेश्की")
                                                        && (m.fyId == fyId) && (m.month == ((month)mn).ToString()) && (m.baUSiNaId == baUSiNaId)
                                                        && (m.hisabNo == item.khaSiNo) && (m.deCre == "debit")).ToList();

                if (db.jornalEntries.Any(m => m.jornalType == "पेश्की फछर्यौट" &&
                              (m.fyId == fyId) && (m.month == ((month)mn).ToString())
                              && (m.hisabNo == item.khaSiNo) && (m.deCre == "debit")) &&
                              (db.jornalEntries.Any(m => m.jornalType == "पेश्की फछर्यौट" &&
                              (m.fyId == fyId) && (m.month == ((month)mn).ToString()) && m.bibaran == "एकल खाता कोष"
                             )))
                {
                    List<jornalEntries> objJornalFer = db.jornalEntries
                                        .Where(m => m.jornalType == "पेश्की फछर्यौट" &&
                                                (m.fyId == fyId) && (m.month == ((month)mn).ToString())
                                                && (m.hisabNo == item.khaSiNo)
                                                && (m.deCre == "debit")).ToList();

                    //Works Only if jornal Entries have only one KhaSiNa (Assigns a amount greater than amount in corresponding peski
                    // jornal entries)
                    foreach (var itemDe in objJornalFer)
                    {
                        itemDe.debit = db.jornalEntries
                                        .FirstOrDefault(m => m.jornalType == "पेश्की फछर्यौट" &&
                                                (m.fyId == fyId) && (m.month == ((month)mn).ToString())
                                                && (m.jornalNo == itemDe.jornalNo)
                                                && (m.bibaran == "एकल खाता कोष")).credit;

                    }
                    //Adds modified Peski Ferchaut jornal entries to objJornalKharcha
                    objJornalKharcha.Concat(objJornalFer);

                }


                //Check jornalentries for khaSiNas and selected month, Calculate total for KhaSiNa
                if (objJornalKharcha.Count > 0)
                {
                    decimal totalThisMonth = 0;
                    foreach (var item2 in objJornalKharcha)
                    {
                        totalThisMonth = totalThisMonth + item2.debit;
                    }
                    objKharcha2.kharchaThisMonth = totalThisMonth;
                }
                else
                {
                    objKharcha2.kharchaThisMonth = 0;
                }


                //For totalTillThisMonth monthIndex<=mn
                List<jornalEntries> objJornalKharcha2 = (from m in db.jornalEntries
                                                         where (m.jornalType == "खर्च" || m.jornalType == "पेश्की")
                                                             && m.monthIndex <= mn && m.fyId == fyId && m.hisabNo == item.khaSiNo
                                                             && m.baUSiNaId == baUSiNaId && m.deCre == "debit"
                                                         select m).ToList();
                //Check Ferchaut for tillThisMonth monthIndex<=mn
                if (db.jornalEntries.Any(m => m.jornalType == "पेश्की फछर्यौट" &&
                              (m.fyId == fyId) && (m.monthIndex <= mn)
                              && (m.hisabNo == item.khaSiNo) && (m.deCre == "debit")) &&
                              (db.jornalEntries.Any(m => m.jornalType == "पेश्की फछर्यौट" &&
                              (m.fyId == fyId) && (m.monthIndex <= mn)
                               && m.bibaran == "एकल खाता कोष")))
                {
                    List<jornalEntries> objJornalFer = db.jornalEntries.Where(m => m.jornalType == "पेश्की फछर्यौट" &&
                                                        (m.fyId == fyId) && (m.monthIndex <= mn)
                                                        && (m.hisabNo == item.khaSiNo)
                                                        && (m.deCre == "debit")).ToList();
                    foreach (var itemDe in objJornalFer)
                    {
                        itemDe.debit = db.jornalEntries.FirstOrDefault(m => m.jornalType == "पेश्की फछर्यौट" &&
                                                        (m.fyId == fyId) && (m.monthIndex <= mn)
                                                        && (m.jornalNo == itemDe.jornalNo)
                                                        && (m.bibaran == "एकल खाता कोष")).credit;
                    }

                    objJornalKharcha2.Concat(objJornalFer);

                }


                //Assign TotalTillThisMonth for kharchaFantari viewModel Object
                if (objJornalKharcha2.Count > 0)
                {
                    decimal total = 0;
                    foreach (var item3 in objJornalKharcha2)
                    {
                        total = total + item3.debit;
                    }
                    objKharcha2.kharchaTillThisMonth = total;
                }
                else
                {
                    objKharcha2.kharchaTillThisMonth = 0;
                }
            }


            //Calculation for Peski in report
            ViewBag.FarchinaBaki = 0;
            {
                List<jornalEntries> objJornalFarchina = db.jornalEntries.Where(m => (m.jornalType == "पेश्की"
                                                        || m.jornalType == "पेश्की फछर्यौट") && m.monthIndex <= mn && m.fyId == fyId
                                                        && m.baUSiNaId == baUSiNaId).ToList();
                var peskiFarchaut = (from m in objJornalFarchina
                                     where (m.jornalType == "पेश्की फछर्यौट") && m.hisabNo != null && m.deCre == "credit"
                                     select m).ToList();
                decimal farchiyeko = peskiFarchaut.Sum(m => m.credit);
                decimal farchinaBaki = objJornalFarchina.Where(m => m.jornalType == "पेश्की" && m.deCre == "debit").Sum(m => m.debit) - farchiyeko;// objJornalFarchina.Where(m => m.jornalType == "पेश्की फछर्यौत" && m.deCre == "credit" && m.hisabNo != null).Sum(m => m.credit);
                ViewBag.FarchinaBaki = farchinaBaki;
            }


            //For Artha Budget 
            var objArtha = db.arthaBudgets.Where(m => m.fyId == fyId && m.monthIndex == mn).ToList();
            decimal khaSiNa3, khaSiNa4;
            if (objArtha.Count > 0)
            {
                khaSiNa3 = objArtha.Sum(m => m.khaSiNa3);
                khaSiNa4 = objArtha.Sum(m => m.khaSiNa4);
                ViewBag.IsNullArtha = "NotNull";
            }
            else
            {
                khaSiNa3 = 0;
                khaSiNa4 = 0;
                ViewBag.IsNullArtha = "Null";
            }

            ViewBag.khaSiNa3 = khaSiNa3;
            ViewBag.khaSiNa4 = khaSiNa4;

            return View(objKharcha.ToList());
        }


        public ActionResult Nagadi()
        {
            var objBaUsiNo = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
            if (objBaUsiNo != null)
            {
                ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo", objBaUsiNo.baUSiNaId);
            }
            else
            {
                ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo");
            }
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);

            return View();
        }
        public ActionResult BankNagadi()
        {
            return RedirectToAction("Nagadi");

        }
        [HttpPost]
        public ActionResult BankNagadi(int baUSiNaId, int mn, int fyId)
        {
            //Validation for Inputs
            if (baUSiNaId == 0 || fyId == 0)
            {
                TempData["ErrorMessage"] = "Please Select BaUSiNa and Year to submit";
                return RedirectToAction("Nagadi");
            }
            var objBaUSiNa = db.baUSiNas.Find(baUSiNaId);
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status);
            string selYear = getYear(fyId, mn);
            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            ViewBag.shirshak = objBaUSiNa.baUSiNo + " " + objBaUSiNa.baUSirsak;
            List<nagadi> objNagadi = new List<nagadi>();

            //All Unique JornalEntries Nos 
            var uniqJornalNo = (from d in db.jornalEntries
                                where (d.jornalType == "खर्च" || d.jornalType == "निकासा" || d.jornalType == "कट्टी"
                                || d.jornalType == "पेश्की" || d.jornalType == "पेश्की फछर्यौट") && d.fyId == fyId && d.monthIndex == mn
                                && d.baUSiNaId == baUSiNaId
                                orderby d.jornalEntriesId
                                group d by new { d.jornalNo } into mygroup
                                select mygroup.FirstOrDefault()).ToList();

            //All Unique JornalEntries Nos Nikasa
            var NiuniqJornalNo = (from d in db.jornalEntries
                                  where (d.jornalType == "निकासा") && d.fyId == fyId && d.monthIndex == mn
                                  && d.baUSiNaId == baUSiNaId
                                  orderby d.jornalEntriesId
                                  group d by new { d } into mygroup
                                  select mygroup.FirstOrDefault()).ToList();

            List<string> NiuniqJornalNo2 = new List<string>();


            if (uniqJornalNo.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found";
                return RedirectToAction("Nagadi");
            }

            //Loop through all unique jornalNos
            foreach (var item in uniqJornalNo)
            {
                var objJornal = db.jornalEntries.Where(m => (m.fyId == fyId) && (m.monthIndex == mn)
                                && (m.baUSiNaId == baUSiNaId) && (m.jornalNo == item.jornalNo)).ToList();

                //JornalType Kharcha
                if (objJornal.Where(m => m.jornalType == "खर्च").ToList().Count > 0)
                {
                    //Find Nikasi Nos for all jornal kharcha
                    var bhuktaniAdeshNi = db.bhuktaniAdeshs.FirstOrDefault(m => m.fyId == fyId && m.jornalKharchaNo == item.jornalNo);
                    NiuniqJornalNo2.Add(bhuktaniAdeshNi.jornalNikasiNo);


                    var objJornalDebit = from m in objJornal where m.deCre == "debit" select m.debit;
                    var objJornalCredit = from m in objJornal where m.deCre == "credit" && m.bibaran != "एकल खाता कोष प्रणाली" select m.credit;

                    //Jornal type Talabi
                    if (objJornal.Where(m => m.hisabNo == "२११११").ToList().Count > 0)
                    {
                        foreach (var itemJor in objJornal)
                        {
                            if (itemJor.deCre == "debit" && objJornal.First() == itemJor)
                            {
                                objNagadi.Add(new nagadi
                                {
                                    bibaran = "तलब समेत भु",
                                    bajetKharcha = itemJor.debit,
                                    bankCre = objJornalDebit.Sum() - objJornalCredit.Sum(),
                                    bankDe = 0,
                                    baUSiNo = objBaUSiNa.baUSiNo,
                                    bibidhaCre = objJornalCredit.Sum(),
                                    bibidhaDe = 0,
                                    chequeNo = "",
                                    hisabNo = "",
                                    jornalNo = itemJor.jornalNo,
                                    khaSiNo = itemJor.hisabNo,
                                    maujdatBanki = 0,
                                    month = itemJor.month,
                                    monthIndex = itemJor.monthIndex,
                                    nepDate = itemJor.nepDate,
                                    peskiFarkiyeko = 0,
                                    peskiPayeko = 0,
                                    tahabilCre = 0,
                                    tahabilDe = 0,
                                    year = itemJor.year,
                                    fyId = fyId,
                                    nepDateStr = itemJor.nepDateStr,
                                    serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                                });
                            }
                            else if (itemJor.deCre == "debit" && objJornal.First() != itemJor)
                            {
                                objNagadi.Add(new nagadi
                                {
                                    bibaran = "",
                                    bajetKharcha = itemJor.debit,
                                    bankCre = 0,
                                    bankDe = 0,
                                    baUSiNo = objBaUSiNa.baUSiNo,
                                    bibidhaCre = 0,
                                    bibidhaDe = 0,
                                    chequeNo = "",
                                    hisabNo = "",
                                    jornalNo = itemJor.jornalNo,
                                    khaSiNo = itemJor.hisabNo,
                                    maujdatBanki = 0,
                                    month = itemJor.month,
                                    monthIndex = itemJor.monthIndex,
                                    nepDate = itemJor.nepDate,
                                    peskiFarkiyeko = 0,
                                    peskiPayeko = 0,
                                    tahabilCre = 0,
                                    tahabilDe = 0,
                                    year = itemJor.year,
                                    fyId = fyId,
                                    nepDateStr = itemJor.nepDateStr,
                                    serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                                });
                            }
                        }
                    }


                    //Non Talabi JornalEntries Kharcha
                    else
                    {
                        //Jornal With AayaKar
                        if (objJornalCredit.ToList().Count > 0)
                        {
                            foreach (var itemJor in objJornal)
                            {
                                if (itemJor.deCre == "debit" && objJornal.First() == itemJor)
                                {
                                    var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalKharchaNo == itemJor.jornalNo && m.fyId == fyId && m.monthIndex == mn && m.khaSiNo == itemJor.hisabNo);
                                    if (bhuktaniAdesh == null)
                                    {
                                        TempData["ErrorMessage"] = "Bhuktani Adesh not found for journal type " + itemJor.jornalType + " for journal No. " + itemJor.jornalNo;
                                        return RedirectToAction("nagadi");
                                    }
                                    objNagadi.Add(new nagadi
                                    {
                                        bibaran = bhuktaniAdesh.pauneKoNaam,
                                        bajetKharcha = itemJor.debit,
                                        bankCre = objJornalDebit.Sum() - objJornalCredit.Sum(),
                                        bankDe = 0,
                                        baUSiNo = objBaUSiNa.baUSiNo,
                                        bibidhaCre = objJornalCredit.Sum(),
                                        bibidhaDe = 0,
                                        chequeNo = "",
                                        hisabNo = "",
                                        jornalNo = item.jornalNo,
                                        khaSiNo = itemJor.hisabNo,
                                        maujdatBanki = 0,
                                        month = itemJor.month,
                                        monthIndex = itemJor.monthIndex,
                                        nepDate = itemJor.nepDate,
                                        peskiFarkiyeko = 0,
                                        peskiPayeko = 0,
                                        tahabilCre = 0,
                                        tahabilDe = 0,
                                        year = itemJor.year,
                                        fyId = fyId,
                                        nepDateStr = itemJor.nepDateStr,
                                        serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                                    });
                                }
                                else if (itemJor.deCre == "debit" && objJornal.First() != itemJor)
                                {
                                    var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalKharchaNo == itemJor.jornalNo && m.fyId == fyId && m.baUSiNaId == baUSiNaId// && m.monthIndex == mn 
                                    && m.khaSiNo == itemJor.hisabNo);
                                    if (bhuktaniAdesh == null)
                                    {
                                        TempData["ErrorMessage"] = "Bhuktani Adesh not found for journal type " + itemJor.jornalType + " for journal No. " + itemJor.jornalNo;
                                        return RedirectToAction("nagadi");
                                    }
                                    if (bhuktaniAdesh == null)
                                    {
                                        ViewBag.ErrorMessage = "Bhuktani Adesh not found for journal type " + itemJor.jornalType + " for journal No. " + itemJor.jornalNo;
                                    }
                                    objNagadi.Add(new nagadi
                                    {
                                        bibaran = bhuktaniAdesh.pauneKoNaam,
                                        bajetKharcha = itemJor.debit,
                                        bankCre = 0,
                                        bankDe = 0,
                                        baUSiNo = objBaUSiNa.baUSiNo,
                                        bibidhaCre = 0,
                                        bibidhaDe = 0,
                                        chequeNo = "",
                                        hisabNo = "",
                                        jornalNo = itemJor.jornalNo,
                                        khaSiNo = itemJor.hisabNo,
                                        maujdatBanki = 0,
                                        month = itemJor.month,
                                        monthIndex = itemJor.monthIndex,
                                        nepDate = itemJor.nepDate,
                                        peskiFarkiyeko = 0,
                                        peskiPayeko = 0,
                                        tahabilCre = 0,
                                        tahabilDe = 0,
                                        year = itemJor.year,
                                        fyId = fyId,
                                        nepDateStr = itemJor.nepDateStr,
                                        serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                                    });
                                }
                            }
                        }
                        //Jornal Type Without AAyakar
                        else
                        {
                            foreach (var itemJor in objJornal)
                            {
                                var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalKharchaNo == itemJor.jornalNo && m.fyId == fyId// && m.monthIndex == mn
                                && m.khaSiNo == itemJor.hisabNo);
                                //if (bhuktaniAdesh == null)
                                //{
                                //    TempData["ErrorMessage"] = "Bhuktani Adesh not found for journal type " + itemJor.jornalType + " for journal No. " + itemJor.jornalNo;
                                //    return RedirectToAction("nagadi");
                                //}
                                if (itemJor.deCre == "debit" && objJornal.First() == itemJor)
                                {
                                    objNagadi.Add(new nagadi
                                    {
                                        bibaran = bhuktaniAdesh.pauneKoNaam,
                                        bajetKharcha = itemJor.debit,
                                        bankCre = objJornalDebit.Sum() - objJornalCredit.Sum(),
                                        bankDe = 0,
                                        baUSiNo = objBaUSiNa.baUSiNo,
                                        bibidhaCre = objJornalCredit.Sum(),
                                        bibidhaDe = 0,
                                        chequeNo = "",
                                        hisabNo = "",
                                        jornalNo = itemJor.jornalNo,
                                        khaSiNo = itemJor.hisabNo,
                                        maujdatBanki = 0,
                                        month = itemJor.month,
                                        monthIndex = itemJor.monthIndex,
                                        nepDate = itemJor.nepDate,
                                        peskiFarkiyeko = 0,
                                        peskiPayeko = 0,
                                        tahabilCre = 0,
                                        tahabilDe = 0,
                                        year = itemJor.year,
                                        fyId = fyId,
                                        nepDateStr = itemJor.nepDateStr,
                                        serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                                    });
                                }
                                else if (itemJor.deCre == "debit" && objJornal.First() != itemJor)
                                {

                                    objNagadi.Add(new nagadi
                                    {
                                        bibaran = bhuktaniAdesh.pauneKoNaam,
                                        bajetKharcha = itemJor.debit,
                                        bankCre = 0,
                                        bankDe = 0,
                                        baUSiNo = objBaUSiNa.baUSiNo,
                                        bibidhaCre = 0,
                                        bibidhaDe = 0,
                                        chequeNo = "",
                                        hisabNo = "",
                                        jornalNo = itemJor.jornalNo,
                                        khaSiNo = itemJor.hisabNo,
                                        maujdatBanki = 0,
                                        month = itemJor.month,
                                        monthIndex = itemJor.monthIndex,
                                        nepDate = itemJor.nepDate,
                                        peskiFarkiyeko = 0,
                                        peskiPayeko = 0,
                                        tahabilCre = 0,
                                        tahabilDe = 0,
                                        year = itemJor.year,
                                        fyId = fyId,
                                        nepDateStr = itemJor.nepDateStr,
                                        serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                                    });
                                }
                            }
                        }

                    }

                }
                //Jornal Type Nikasa
                else if (objJornal.Where(m => m.jornalType == "निकासा").ToList().Count > 0)
                {
                    var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalNikasiNo == item.jornalNo && m.fyId == fyId && m.baUSiNaId == baUSiNaId// && m.monthIndex == mn
                    );
                    string bhuNo = "";
                    if (bhuktaniAdesh != null)
                    {
                        bhuNo = bhuktaniAdesh.bhuktaniAdeshNo;
                    }

                    objNagadi.Add(new nagadi
                    {
                        bibaran = "भु.आ.नं. " + bhuNo + " बाट",
                        bajetKharcha = 0,
                        bankCre = 0,
                        bankDe = objJornal.Sum(m => m.debit),
                        baUSiNo = objBaUSiNa.baUSiNo,
                        bibidhaCre = objJornal.Sum(m => m.debit),
                        bibidhaDe = 0,
                        chequeNo = "",
                        hisabNo = "",
                        jornalNo = item.jornalNo,
                        khaSiNo = "",
                        maujdatBanki = 0,
                        month = ((month)mn).ToString(),
                        monthIndex = mn,
                        nepDate = item.nepDate,
                        peskiFarkiyeko = 0,
                        peskiPayeko = 0,
                        tahabilCre = 0,
                        tahabilDe = 0,
                        year = selYear,
                        fyId = fyId,
                        nepDateStr = item.nepDateStr,
                        serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                    });
                }
                //Jornal Type Katti
                else if (objJornal.Where(m => m.jornalType == "कट्टी").ToList().Count > 0)
                {
                    //Jornal Type Talabi
                    if (objJornal.Where(m => m.bibaran == "क.स.कोष कट्टी").ToList().Count > 0)
                    {
                        objNagadi.Add(new nagadi
                        {
                            bibaran = "क.स.कोष समेत दाखिला",
                            bajetKharcha = 0,
                            bankCre = objJornal.Sum(m => m.debit),
                            bankDe = 0,
                            baUSiNo = objBaUSiNa.baUSiNo,
                            bibidhaCre = 0,
                            bibidhaDe = objJornal.Sum(m => m.debit),
                            chequeNo = "",
                            hisabNo = "",
                            jornalNo = item.jornalNo,
                            khaSiNo = "",
                            maujdatBanki = 0,
                            month = ((month)mn).ToString(),
                            monthIndex = mn,
                            nepDate = objJornal[0].nepDate,
                            peskiFarkiyeko = 0,
                            peskiPayeko = 0,
                            tahabilCre = 0,
                            tahabilDe = 0,
                            year = selYear,
                            fyId = fyId,
                            nepDateStr = objJornal[0].nepDateStr,
                            serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                        });
                    }
                    //Jornal Type Katti with Aayar(not talabi)
                    else
                    {
                        objNagadi.Add(new nagadi
                        {
                            bibaran = "आयकर दाखिला",
                            bajetKharcha = 0,
                            bankCre = objJornal.Sum(m => m.debit),
                            bankDe = 0,
                            baUSiNo = "",
                            bibidhaCre = 0,
                            bibidhaDe = objJornal.Sum(m => m.debit),
                            chequeNo = "",
                            hisabNo = "",
                            jornalNo = item.jornalNo,
                            khaSiNo = "",
                            maujdatBanki = 0,
                            month = item.month,
                            monthIndex = item.monthIndex,
                            nepDate = item.nepDate,
                            peskiFarkiyeko = 0,
                            peskiPayeko = 0,
                            tahabilCre = 0,
                            tahabilDe = 0,
                            year = objJornal[0].year,
                            fyId = fyId,
                            nepDateStr = objJornal[0].nepDateStr,
                            serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                        });

                    }

                }

                else if (objJornal.Where(m => m.jornalType == "पेश्की").ToList().Count > 0)
                {
                    var objJornalDebit = from m in objJornal where m.deCre == "debit" select m.debit;
                    var objJornalCredit = from m in objJornal where m.deCre == "credit" && m.bibaran != "एकल खाता कोष प्रणाली" select m.credit;


                    foreach (var itemJor in objJornal)
                    {
                        if (itemJor.deCre == "debit" && objJornal.First() == itemJor && objJornalCredit.ToList().Count > 0)
                        {
                            var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalKharchaNo == itemJor.jornalNo && m.fyId == fyId && m.baUSiNaId == baUSiNaId//  && m.monthIndex == mn
                                                && m.khaSiNo == itemJor.hisabNo);
                            if (bhuktaniAdesh == null)
                            {
                                TempData["ErrorMessage"] = "Bhuktani Adesh not found for journal type " + itemJor.jornalType + " for journal No. " + itemJor.jornalNo;
                                return RedirectToAction("nagadi");
                            }
                            objNagadi.Add(new nagadi
                            {
                                bibaran = bhuktaniAdesh.pauneKoNaam,
                                bajetKharcha = itemJor.debit,
                                bankCre = objJornalDebit.Sum() - objJornalCredit.Sum(),
                                bankDe = 0,
                                baUSiNo = objBaUSiNa.baUSiNo,
                                bibidhaCre = objJornalCredit.Sum(),
                                bibidhaDe = 0,
                                chequeNo = "",
                                hisabNo = "",
                                jornalNo = item.jornalNo,
                                khaSiNo = itemJor.hisabNo,
                                maujdatBanki = 0,
                                month = itemJor.month,
                                monthIndex = itemJor.monthIndex,
                                nepDate = itemJor.nepDate,
                                peskiFarkiyeko = 0,
                                peskiPayeko = itemJor.debit,
                                tahabilCre = 0,
                                tahabilDe = 0,
                                year = itemJor.year,
                                fyId = fyId,
                                nepDateStr = itemJor.nepDateStr,
                                serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                            });
                        }
                        else if (itemJor.deCre == "debit" && objJornal.First() == itemJor && objJornalCredit.ToList().Count == 0)
                        {
                            var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalKharchaNo == itemJor.jornalNo && m.fyId == fyId && m.baUSiNaId == baUSiNaId//&& m.monthIndex == mn
                            && m.khaSiNo == itemJor.hisabNo);
                            if (bhuktaniAdesh == null)
                            {
                                TempData["ErrorMessage"] = "Bhuktani Adesh not found for journal type " + itemJor.jornalType + " for journal No. " + itemJor.jornalNo;
                                return RedirectToAction("nagadi");
                            }
                            objNagadi.Add(new nagadi
                            {
                                bibaran = bhuktaniAdesh.pauneKoNaam,
                                bajetKharcha = itemJor.debit,
                                bankCre = objJornalDebit.Sum(),
                                bankDe = 0,
                                baUSiNo = objBaUSiNa.baUSiNo,
                                bibidhaCre = 0,
                                bibidhaDe = 0,
                                chequeNo = "",
                                hisabNo = "",
                                jornalNo = item.jornalNo,
                                khaSiNo = itemJor.hisabNo,
                                maujdatBanki = 0,
                                month = itemJor.month,
                                monthIndex = itemJor.monthIndex,
                                nepDate = itemJor.nepDate,
                                peskiFarkiyeko = 0,
                                peskiPayeko = itemJor.debit,
                                tahabilCre = 0,
                                tahabilDe = 0,
                                year = itemJor.year,
                                fyId = fyId,
                                nepDateStr = itemJor.nepDateStr,
                                serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                            });
                        }
                        else if (itemJor.deCre == "debit" && objJornal.First() != itemJor)
                        {
                            var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalKharchaNo == itemJor.jornalNo && m.fyId == fyId && m.baUSiNaId == baUSiNaId// && m.monthIndex == mn
                            && m.khaSiNo == itemJor.hisabNo);
                            if (bhuktaniAdesh == null)
                            {
                                TempData["ErrorMessage"] = "Bhuktani Adesh not found for journal type " + itemJor.jornalType + " for journal No. " + itemJor.jornalNo;
                                return RedirectToAction("nagadi");
                            }
                            objNagadi.Add(new nagadi
                            {
                                bibaran = bhuktaniAdesh.pauneKoNaam,
                                bajetKharcha = itemJor.debit,
                                bankCre = 0,
                                bankDe = 0,
                                baUSiNo = objBaUSiNa.baUSiNo,
                                bibidhaCre = 0,
                                bibidhaDe = 0,
                                chequeNo = "",
                                hisabNo = "",
                                jornalNo = itemJor.jornalNo,
                                khaSiNo = itemJor.hisabNo,
                                maujdatBanki = 0,
                                month = itemJor.month,
                                monthIndex = itemJor.monthIndex,
                                nepDate = itemJor.nepDate,
                                peskiFarkiyeko = 0,
                                peskiPayeko = itemJor.debit,
                                tahabilCre = 0,
                                tahabilDe = 0,
                                year = itemJor.year,
                                fyId = fyId,
                                nepDateStr = itemJor.nepDateStr,
                                serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                            });
                        }
                    }
                }
                else if (objJornal.Where(m => m.jornalType == "पेश्की फछर्यौट").ToList().Count > 0)
                {
                    var objJornalFerchaut = from m in objJornal where m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली" select m.credit;
                    decimal creditRakam = 0;
                    if (objJornal.Any(m => m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली"))
                    {
                        creditRakam = objJornal.FirstOrDefault(m => m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली").credit;
                    }
                    string peskiJornal = objJornal.FirstOrDefault(m => m.sanketNo != "" || m.sanketNo != null && item.jornalNo == item.jornalNo).sanketNo;
                    decimal ferchautRakam = objJornal.Where(m => m.jornalNo == item.jornalNo).Sum(m => m.debit);// objJornal.FirstOrDefault(m => m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली").credit;
                                                                                                                //if (item.deCre == "debit" && objJornal.First() == item)


                    var objJornalDebit = from m in objJornal where m.deCre == "debit" select m;

                    foreach (var itemJor in objJornalDebit)
                    {

                        if (objJornalDebit.First() == itemJor)
                        {
                            objNagadi.Add(new nagadi
                            {
                                bibaran = "पेश्की फछर्यौट",//itemJor.bibaran,
                                bajetKharcha = creditRakam,
                                bankCre = creditRakam,
                                bankDe = 0,
                                baUSiNo = objBaUSiNa.baUSiNo,
                                bibidhaCre = creditRakam,
                                bibidhaDe = 0,
                                chequeNo = "",
                                hisabNo ="",
                                jornalNo = item.jornalNo,
                                khaSiNo = itemJor.hisabNo,
                                maujdatBanki = 0,
                                month = item.month,
                                monthIndex = item.monthIndex,
                                nepDate = item.nepDate,
                                peskiFarkiyeko = itemJor.debit,
                                peskiPayeko = 0,
                                tahabilCre = 0,
                                tahabilDe = 0,
                                year = item.year,
                                fyId = fyId,
                                nepDateStr = item.nepDateStr,
                                serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                            });
                        }
                        else
                        {
                            objNagadi.Add(new nagadi
                            {
                                bibaran = "पेश्की फछर्यौट",//itemJor.bibaran,
                                bajetKharcha = 0,
                                bankCre = 0,
                                bankDe = 0,
                                baUSiNo = objBaUSiNa.baUSiNo,
                                bibidhaCre = 0,
                                bibidhaDe = 0,
                                chequeNo = "",
                                hisabNo = "",
                                jornalNo = item.jornalNo,
                                khaSiNo = itemJor.hisabNo,
                                maujdatBanki = 0,
                                month = item.month,
                                monthIndex = item.monthIndex,
                                nepDate = item.nepDate,
                                peskiFarkiyeko = itemJor.debit,
                                peskiPayeko = 0,
                                tahabilCre = 0,
                                tahabilDe = 0,
                                year = item.year,
                                fyId = fyId,
                                nepDateStr = item.nepDateStr,
                                serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                            });

                        }

                    }
                }



                    //objNagadi.Add(new nagadi
                    //    {
                    //        bibaran = "पेश्की फछर्यौट",//itemJor.bibaran,
                    //        bajetKharcha = creditRakam,
                    //        bankCre = creditRakam,
                    //        bankDe = 0,
                    //        baUSiNo = objBaUSiNa.baUSiNo,
                    //        bibidhaCre = creditRakam,
                    //        bibidhaDe = 0,
                    //        chequeNo = "",
                    //        hisabNo = "",
                    //        jornalNo = item.jornalNo,
                    //        khaSiNo = "",
                    //        maujdatBanki = 0,
                    //        month = item.month,
                    //        monthIndex = item.monthIndex,
                    //        nepDate = item.nepDate,
                    //        peskiFarkiyeko = ferchautRakam,
                    //        peskiPayeko = 0,
                    //        tahabilCre = 0,
                    //        tahabilDe = 0,
                    //        year = item.year,
                    //        fyId = fyId,
                    //        nepDateStr = item.nepDateStr,
                    //        serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                    //    });
                    //}






                    //   if (itemJor.deCre == "debit" && objJornal.First() != itemJor)
                    //        {
                    //            var bhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.jornalKharchaNo == itemJor.jornalNo && m.fyId == fyId && m.monthIndex == mn && m.khaSiNo == itemJor.hisabNo);
                    //            objNagadi.Add(new nagadi
                    //            {
                    //                bibaran = itemJor.bibaran,
                    //                bajetKharcha = creditRakam,
                    //                bankCre = creditRakam,
                    //                bankDe = 0,
                    //                baUSiNo = objBaUSiNa.baUSiNo,
                    //                bibidhaCre = creditRakam,
                    //                bibidhaDe = 0,
                    //                chequeNo = "",
                    //                hisabNo = "",
                    //                jornalNo = item.jornalNo,
                    //                khaSiNo = itemJor.hisabNo,
                    //                maujdatBanki = 0,
                    //                month = itemJor.month,
                    //                monthIndex = itemJor.monthIndex,
                    //                nepDate = itemJor.nepDate,
                    //                peskiFarkiyeko = 0,
                    //                peskiPayeko = 0,
                    //                tahabilCre = 0,
                    //                tahabilDe = 0,
                    //                year = itemJor.year,
                    //                fyId = fyId,
                    //                nepDateStr = itemJor.nepDateStr,
                    //                serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                    //            });
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    jornalEntries objJornalCreditFarchaut = (from m in objJornal where m.deCre == "credit" && m.hisabNo != null && m.bibaran != "एकल खाता कोष प्रणाली" select m).FirstOrDefault();

                    //    objNagadi.Add(new nagadi
                    //    {
                    //        bibaran = "पेश्की फछर्यौट",
                    //        bajetKharcha = 0,
                    //        bankCre = 0,
                    //        bankDe = 0,
                    //        baUSiNo = objBaUSiNa.baUSiNo,
                    //        bibidhaCre = 0,
                    //        bibidhaDe = 0,
                    //        chequeNo = "",
                    //        hisabNo = "",
                    //        jornalNo = item.jornalNo,
                    //        khaSiNo = objJornalCreditFarchaut.hisabNo,
                    //        maujdatBanki = 0,
                    //        month = item.month,
                    //        monthIndex = item.monthIndex,
                    //        nepDate = item.nepDate,
                    //        peskiFarkiyeko = objJornalCreditFarchaut.credit,
                    //        peskiPayeko = 0,
                    //        tahabilCre = 0,
                    //        tahabilDe = 0,
                    //        year = item.year,
                    //        fyId = fyId,
                    //        nepDateStr = item.nepDateStr,
                    //        serialNo = Convert.ToInt32(UnicodeToString(item.jornalNo))
                    //    });
                    //}

                    //  }







                }



                //Calculation for आ.ल्या.
                List<nagadi> prevMonth = new List<nagadi> { new nagadi { nepDate = DateTime.Now } };
                {
                    prevMonth[0].nepDate = DateTime.Now;
                    decimal kharchaTotal, kattiTotal, peski, peskiFarchaut, kharchaFarchaut;
                    try
                    {
                        List<jornalEntries> objJornalFarchaut = (from m in db.jornalEntries
                                                                 where m.fyId == fyId && m.monthIndex < mn && m.baUSiNaId == baUSiNaId
                                                              && m.jornalType == "पेश्की फछर्यौट" && m.deCre == "credit"
                                                              && m.bibaran == "एकल खाता कोष प्रणाली"
                                                                 select m).ToList();
                        kharchaFarchaut = 0;
                        peski = 0;
                        peskiFarchaut = 0;
                        if (objJornalFarchaut.Count > 0)
                        {
                            kharchaFarchaut = objJornalFarchaut.Sum(m => m.credit);
                        }
                        //kharchaFarchaut =(from m in db.jornalEntries where  m.fyId == fyId && m.monthIndex < mn && m.jornalType == "पेश्की फछर्यौट" && m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली" select m.credit).Sum();
                        kharchaTotal = kharchaFarchaut + db.jornalEntries.Where(m => m.fyId == fyId && m.monthIndex < mn
                                          && (m.jornalType == "खर्च" || m.jornalType == "पेश्की") && m.baUSiNaId == baUSiNaId).Sum(m => m.debit);

                        List<jornalEntries> objJornalpeski = (from m in db.jornalEntries
                                                              where m.fyId == fyId && m.monthIndex < mn
                                       && m.jornalType == "पेश्की" && m.baUSiNaId == baUSiNaId
                                                              select m).ToList();
                        if (objJornalpeski.Count > 0)
                        {
                            peski = objJornalpeski.Sum(m => m.debit);
                        }
                        List<jornalEntries> objJornalPeskiFarchaut = (from m in db.jornalEntries
                                                                      where m.fyId == fyId && m.monthIndex < mn && m.baUSiNaId == baUSiNaId
                                                                        && m.jornalType == "पेश्की फछर्यौट" && m.deCre == "credit" && m.hisabNo != null
                                                                      select m).ToList();
                        if (objJornalPeskiFarchaut.Count > 0)
                        {
                            peskiFarchaut = objJornalPeskiFarchaut.Sum(m => m.credit);
                        }
                        kattiTotal = db.jornalEntries.Where(m => m.fyId == fyId && m.monthIndex < mn && m.jornalType == "कट्टी" && m.baUSiNaId == baUSiNaId).Sum(m => m.debit);

                    }
                    catch
                    {
                        peski = 0;
                        peskiFarchaut = 0;
                        kharchaTotal = 0;
                        kattiTotal = 0;

                    }
                    prevMonth[0].bankDe = kharchaTotal;
                    prevMonth[0].bankCre = kharchaTotal;
                    prevMonth[0].bajetKharcha = kharchaTotal;
                    // prevMonth[0].bibidhaCre = kharchaTotal;
                    prevMonth[0].maujdatBanki = 0;
                    prevMonth[0].peskiFarkiyeko = peskiFarchaut;
                    prevMonth[0].peskiPayeko = peski;
                    prevMonth[0].bibidhaDe = kattiTotal;
                    prevMonth[0].bibidhaCre = kattiTotal + kharchaTotal;
                }
                ViewBag.IsNullPrevNagadi = "NotNull";
                if (mn == 0)
                {
                    ViewBag.IsNullPrevNagadi = "Null";
                }
                ViewBag.prevBankNagadi = prevMonth;
                return View(objNagadi.OrderBy(m => m.serialNo).ToList());
            }

            public ActionResult Hisab()
            {
                var objBaUSiNo = db.baUSiNas.FirstOrDefault(m => m.baUSiNo == "३२५०१४३");
                if (objBaUSiNo != null)
                {
                    ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo", objBaUSiNo.baUSiNaId);
                }
                else
                {
                    ViewBag.baUSiNaId = new SelectList(db.baUSiNas.ToList(), "baUSiNaId", "baUSiNo");
                }
                var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", objFiscalYear.fyId);
                return View();
            }

            public ActionResult BajetHisab()
            {
                return RedirectToAction("Hisab");


            }

        [HttpPost]
        public ActionResult BajetHisab(int baUSiNaId, int mn, int fyId)
        {
            //validation of input types
            if (baUSiNaId == 0 || fyId == 0)
            {
                TempData["ErrorMessage"] = "Please Select BaUSiNa and submit.";
                return RedirectToAction("Hisab");
            }
            //Bajet Hisab for BaUSiNaId not found 
            else if (baUSiNaId != 2)
            {
                TempData["ErrorMessage"] = "Bajet Hisab for Selected BaUSiNa not found.";
                return RedirectToAction("Hisab");
            }
            var objBaUSiNa = db.baUSiNas.Find(baUSiNaId);
            List<hisab> objHisab = new List<hisab>();
            var objFiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
            string selYear = getYear(fyId, mn);

            ViewBag.fyId = new SelectList(db.fisYears, "fyId", "nepFy", fyId);
            string monthName = ((month)mn).ToString();

            //Get Unique Nikasa and Kharcha Jornal Nos
            var uniqJornalNikasa = (from d in db.jornalEntries
                                    where d.jornalType == "निकासा" && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                                    group d by new { d.jornalNo }
                                        into mygroup
                                    select mygroup.FirstOrDefault()).ToList();

            var uniqJornalKharcha = (from d in db.jornalEntries
                                     where (d.jornalType == "खर्च" || d.jornalType == "पेश्की" || d.jornalType == "पेश्की फछर्यौट")
                                     && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                                     group d by new { d.jornalNo }
                                         into mygroup
                                     select mygroup.FirstOrDefault()).ToList();


            //All JornalNos
            List<string> uniqJornalKharchaNos = (from d in uniqJornalKharcha
                                               select d.jornalNo).ToList();


            //Peski ferchaut bhayeka haru
            var uniqJornalKharchaFer = (from d in db.jornalEntries
                                     where ( d.jornalType == "पेश्की फछर्यौट" && d.sanketNo!=null && d.sanketNo!="")
                                     && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                                     group d by new { d.sanketNo}
                                        into mygroup
                                     select mygroup.FirstOrDefault()).ToList();

            //unique peski fercaut bhayeka haru
            List<string> uniqJornalFerNos= (from d in uniqJornalKharchaFer
                                               select d.sanketNo).ToList();

            uniqJornalKharchaNos = uniqJornalKharchaNos.Except(uniqJornalFerNos).ToList();

            if (uniqJornalNikasa.Count == 0 || uniqJornalKharcha.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found.";
                return RedirectToAction("Hisab");
            }


            List<jornalEntries> objJornalNikasa = new List<jornalEntries>();
            List<jornalEntries> objJornal = new List<jornalEntries>();

            foreach (var item in uniqJornalNikasa)
            {
                var objNikasa = db.jornalEntries.Where(m => (m.jornalType == "निकासा") && (m.fyId == fyId)
                                && (m.monthIndex == mn) && (m.jornalNo == item.jornalNo) && m.baUSiNaId == baUSiNaId).OrderBy(m => m.deCre)
                                .ToList();

                objJornalNikasa = objJornalNikasa.Concat(objNikasa).ToList();

            }

            foreach (var item in uniqJornalKharchaNos)
            {
                var objKharcha = db.jornalEntries.Where(d => (d.jornalType == "खर्च" || d.jornalType == "पेश्की" || d.jornalType == "पेश्की फछर्यौट")
                                     && d.fyId == fyId && d.monthIndex == mn && d.baUSiNaId == baUSiNaId
                                && (d.monthIndex == mn) && (d.jornalNo == item) ).OrderBy(d => d.deCre)
                                .ToList();

                //uniqJornalKharcha.Where(m=>(m.jornalNo == item)) 
                 //               .OrderByDescending(m => m.deCre).ToList();

                objJornal = objJornal.Concat(objKharcha).ToList();

            }

            if (objJornalNikasa.Count == 0 || objJornal.Count == 0)
            {
                TempData["ErrorMessage"] = "No Record Found";
                return RedirectToAction("Hisab");
            }

            //Get Bhuktnai Adesh No
            var objBhuktani = db.bhuktaniAdeshs.Where(m => (m.baUSiNaId == baUSiNaId) && m.fyId == fyId && m.monthIndex == mn &&
                                (m.bhuktaniType == "भुक्तानी आदेश तलबी" || m.bhuktaniType == "भुक्तानी आदेश"));
            var uniqBhuktaniNo = (from d in objBhuktani
                                  group d by new { d.bhuktaniAdeshNo }
                                      into mygroup
                                  select mygroup.FirstOrDefault()).ToList();


            //Find if BhuktaniAdesh Not found for JornalEntries
            var uniqNotFoundNikasa = uniqBhuktaniNo.Where(m => m.jornalKharchaNo == "०" || m.jornalNikasiNo == "०").ToList();
            if (uniqNotFoundNikasa.Count > 0)
            {
                string AdeshNotFound = "";
                foreach (var item in uniqNotFoundNikasa)
                {
                    if (item != uniqNotFoundNikasa.Last())
                    {
                        AdeshNotFound += item.bhuktaniAdeshNo + ",";
                    }
                    else
                    {
                        AdeshNotFound += item.bhuktaniAdeshNo;
                    }
                }
                TempData["ErrorMessage"] = "Journal nikasa not found for Bhuktani Adesh Nos " + AdeshNotFound;
                return RedirectToAction("Hisab");
            }

            foreach (var item in uniqBhuktaniNo)
            {
                var objBhuktaniAdesh = db.bhuktaniAdeshs.FirstOrDefault(m => m.bhuktaniAdeshNo == item.bhuktaniAdeshNo);
                var objJornalNi = db.jornalEntries.Where(m => m.jornalNo == objBhuktaniAdesh.jornalNikasiNo && m.baUSiNaId == baUSiNaId
                    && m.fyId == fyId && m.monthIndex == mn).ToList();
                var objUniqKharcha = (from d in db.bhuktaniAdeshs
                                      where d.bhuktaniAdeshNo == objBhuktaniAdesh.bhuktaniAdeshNo && d.fyId == fyId && d.monthIndex == mn
                                      && d.baUSiNaId == baUSiNaId && (d.bhuktaniType == "भुक्तानी आदेश तलबी" || d.bhuktaniType == "भुक्तानी आदेश")
                                      group d by new { d.jornalKharchaNo }
                                          into mygroup
                                      select mygroup.FirstOrDefault()).ToList();


                //Talabi Nikasa tarfa
                {
                    hisab hisabs = new hisab();
                    hisabs.nepDate = objJornalNi[0].nepDate;
                    hisabs.baUSiNo = objBaUSiNa.baUSiNo;
                    hisabs.bibaran = "भु. आ. नं." + objBhuktaniAdesh.bhuktaniAdeshNo + " बाट";
                    hisabs.month = ((month)mn).ToString();
                    hisabs.monthIndex = mn;
                    hisabs.year = selYear;
                    hisabs.bajetHisabType = "nikasa";
                    hisabs.jornalNo = objJornalNi[0].jornalNo;
                    hisabs.talab = CalculateJornalItemNikasa(objJornalNi, "२११११");
                    hisabs.mahangiBhatta = CalculateJornalItemNikasa(objJornalNi, "२१११३");
                    hisabs.anyaBhatta = CalculateJornalItemNikasa(objJornalNi, "२१११९");
                    hisabs.posak = CalculateJornalItemNikasa(objJornalNi, "२११२१");
                    hisabs.paniBijuli = CalculateJornalItemNikasa(objJornalNi, "२२१११");
                    hisabs.sancharMahasul = CalculateJornalItemNikasa(objJornalNi, "२२११२");
                    hisabs.endhan = CalculateJornalItemNikasa(objJornalNi, "२२२११");
                    hisabs.sanchalanMarmat = CalculateJornalItemNikasa(objJornalNi, "२२२१२");
                    hisabs.bima = CalculateJornalItemNikasa(objJornalNi, "२२२१३");
                    hisabs.karyalayaSambandhi = CalculateJornalItemNikasa(objJornalNi, "२२३११");
                    hisabs.pustakSamagri = CalculateJornalItemNikasa(objJornalNi, "२२३१३");
                    hisabs.sewaParamarsa = CalculateJornalItemNikasa(objJornalNi, "२२४११");
                    hisabs.anyaSewaSulka = CalculateJornalItemNikasa(objJornalNi, "२२४१२");
                    hisabs.karyakram = CalculateJornalItemNikasa(objJornalNi, "२२५२२");
                    hisabs.bibidhaKaryakram = CalculateJornalItemNikasa(objJornalNi, "२२५२९");
                    hisabs.anugamanMulyankan = CalculateJornalItemNikasa(objJornalNi, "२२६११");
                    hisabs.bhraman = CalculateJornalItemNikasa(objJornalNi, "२२६१२");
                    hisabs.bibidha = CalculateJornalItemNikasa(objJornalNi, "२२७११");
                    hisabs.nepDateStr = objJornalNi[0].nepDateStr;
                    hisabs.fyId = fyId;
                    objHisab.Add(hisabs);
                }
                foreach (var itemKha in objUniqKharcha)
                {
                    //  decimal ferchautRakam = 0;
                    var objJornalKha = objJornal.Where(m => m.jornalNo == itemKha.jornalKharchaNo && m.monthIndex == mn
                                        && m.fyId == fyId && m.baUSiNaId == baUSiNaId).ToList();
                    if (objJornalKha.Any())
                    { 
                    if (objJornalKha.Any(m => m.jornalType == "पेश्की फछर्यौट"))
                    {
                        string pauneKoNaam = "";
                        var objJornalFerchaut = (from d in objJornal
                                                 where d.jornalType == "पेश्की फछर्यौट" && d.baUSiNaId == baUSiNaId && d.deCre == "credit" && d.bibaran == "एकल खाता कोष प्रणाली"
                                                 select d).ToList();
                        string jornalPeskiNo = objJornalKha.FirstOrDefault(m => m.sanketNo != "" || m.sanketNo == "").sanketNo;
                        if (objJornalFerchaut == null)
                        {
                            pauneKoNaam = db.bhuktaniAdeshs.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn
                                             && m.jornalKharchaNo == jornalPeskiNo && m.baUSiNaId == baUSiNaId).pauneKoNaam;
                        }
                        pauneKoNaam = db.bhuktaniAdeshs.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn
                                              && m.jornalKharchaNo == itemKha.jornalKharchaNo && m.baUSiNaId == baUSiNaId).pauneKoNaam;



                        hisab hisabs = new hisab();
                        hisabs.nepDate = objJornalKha[0].nepDate;
                        hisabs.baUSiNo = objBaUSiNa.baUSiNo;
                        hisabs.bibaran = pauneKoNaam + "लाई भु.";
                        hisabs.month = ((month)mn).ToString();
                        hisabs.monthIndex = mn;
                        hisabs.year = selYear;
                        hisabs.bajetHisabType = "kharcha";
                        hisabs.jornalNo = objJornalKha[0].jornalNo;
                        hisabs.talab = CalculateJornalItem(objJornalKha, "२११११");
                        hisabs.mahangiBhatta = CalculateJornalItem(objJornalKha, "२१११३");
                        hisabs.anyaBhatta = CalculateJornalItem(objJornalKha, "२१११९");
                        hisabs.posak = CalculateJornalItem(objJornalKha, "२११२१");
                        hisabs.paniBijuli = CalculateJornalItem(objJornalKha, "२२१११");
                        hisabs.sancharMahasul = CalculateJornalItem(objJornalKha, "२२११२");
                        hisabs.endhan = CalculateJornalItem(objJornalKha, "२२२११");
                        hisabs.sanchalanMarmat = CalculateJornalItem(objJornalKha, "२२२१२");
                        hisabs.bima = CalculateJornalItem(objJornalKha, "२२२१३");
                        hisabs.karyalayaSambandhi = CalculateJornalItem(objJornalKha, "२२३११");
                        hisabs.pustakSamagri = CalculateJornalItem(objJornalKha, "२२३१३");
                        hisabs.sewaParamarsa = CalculateJornalItem(objJornalKha, "२२४११");
                        hisabs.anyaSewaSulka = CalculateJornalItem(objJornalKha, "२२४१२");
                        hisabs.karyakram = CalculateJornalItem(objJornalKha, "२२५२२");
                        hisabs.bibidhaKaryakram = CalculateJornalItem(objJornalKha, "२२५२९");
                        hisabs.anugamanMulyankan = CalculateJornalItem(objJornalKha, "२२६११");
                        hisabs.bhraman = CalculateJornalItem(objJornalKha, "२२६१२");
                        hisabs.bibidha = CalculateJornalItem(objJornalKha, "२२७११");
                        hisabs.nepDateStr = objJornalKha[0].nepDateStr;
                        hisabs.fyId = fyId;
                        objHisab.Add(hisabs);


                    }

                    //    //for JornalEntries peski ferchaut
                    //    var objJornalFerchaut = (from d in objJornalKha
                    //                         where d.jornalType == "पेश्की फछर्यौट" && d.baUSiNaId == baUSiNaId// && d.deCre == "credit" && d.bibaran == "एकल खाता कोष प्रणाली"
                    //                         select d).ToList();
                    //if (objJornalFerchaut.Count > 0)
                    //{
                    //    string pauneKoNaam = db.bhuktaniAdeshs.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn
                    //                           && m.jornalKharchaNo == itemKha.jornalKharchaNo && m.baUSiNaId == baUSiNaId).pauneKoNaam;
                    //    ferchautRakam = objJornalKha.Where(m => m.bibaran == "एकल खाता कोष प्रणाली" && m.deCre == "credit" && m.baUSiNaId == baUSiNaId).Sum(m => m.credit);
                    //    hisab hisabs = new hisab();
                    //    hisabs.nepDate = objJornalKha[0].nepDate;
                    //    hisabs.baUSiNo = objBaUSiNa.baUSiNo;
                    //    hisabs.bibaran = pauneKoNaam + "लाई भु.";
                    //    hisabs.month = ((month)mn).ToString();
                    //    hisabs.monthIndex = mn;
                    //    hisabs.year = selYear;
                    //    hisabs.bajetHisabType = "kharcha";
                    //    hisabs.jornalNo = objJornalKha[0].jornalNo;
                    //    string khaSiNo = objJornalKha.FirstOrDefault(m => m.deCre == "debit" && m.hisabNo != null).hisabNo.Trim();
                    //    switch (khaSiNo)
                    //    {
                    //        case "२११११":
                    //            hisabs.talab = ferchautRakam;
                    //            break;
                    //        case "२१११३":
                    //            hisabs.mahangiBhatta = ferchautRakam;
                    //            break;
                    //        case "२१११९":
                    //            hisabs.anyaBhatta = ferchautRakam;
                    //            break;
                    //        case "२११२१":
                    //            hisabs.posak = ferchautRakam;
                    //            break;
                    //        case "२२१११":
                    //            hisabs.paniBijuli = ferchautRakam;
                    //            break;
                    //        case "२२११२":
                    //            hisabs.sancharMahasul = ferchautRakam;
                    //            break;
                    //        case "२२२११":
                    //            hisabs.endhan = ferchautRakam;
                    //            break;
                    //        case "२२२१२":
                    //            hisabs.sanchalanMarmat = ferchautRakam;
                    //            break;
                    //        case "२२२१३":
                    //            hisabs.bima = ferchautRakam;
                    //            break;
                    //        case "२२३११":
                    //            hisabs.karyalayaSambandhi = ferchautRakam;
                    //            break;
                    //        case "२२३१३":
                    //            hisabs.pustakSamagri = ferchautRakam;
                    //            break;
                    //        case "२२४११":
                    //            hisabs.sewaParamarsa = ferchautRakam;
                    //            break;
                    //        case "२२४१२":
                    //            hisabs.anyaSewaSulka = ferchautRakam;
                    //            break;
                    //        case "२२५२२":
                    //            hisabs.karyakram = ferchautRakam;
                    //            break;
                    //        case "२२५२९":
                    //            hisabs.bibidhaKaryakram = ferchautRakam;
                    //            break;
                    //        case "२२६११":
                    //            hisabs.anugamanMulyankan = ferchautRakam;
                    //            break;
                    //        case "२२६१२":
                    //            hisabs.bhraman = ferchautRakam;
                    //            break;
                    //        case "२२७११":
                    //            hisabs.bibidha = ferchautRakam;
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //    hisabs.nepDateStr = objJornalKha[0].nepDateStr;
                    //    hisabs.fyId = fyId;
                    //    objHisab.Add(hisabs);
                    //}

                    else//if (objJornalKha.FirstOrDefault().jornalType != "पेश्की फछर्यौट")
                    {
                        if (!objJornal.Any(m => m.sanketNo == item.jornalKharchaNo))
                        {
                            string pauneKoNaam = db.bhuktaniAdeshs.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn && m.jornalKharchaNo == itemKha.jornalKharchaNo && m.baUSiNaId == baUSiNaId).pauneKoNaam;
                            {

                                hisab hisabs = new hisab();
                                hisabs.nepDate = objJornalKha[0].nepDate;
                                hisabs.baUSiNo = objBaUSiNa.baUSiNo;
                                if (objJornalKha.FirstOrDefault(m => m.hisabNo == "२११११") != null)
                                {
                                    hisabs.bibaran = "तलब समेत भु.";
                                }
                                else
                                {
                                    hisabs.bibaran = pauneKoNaam + "लाई भु.";
                                }
                                hisabs.month = ((month)mn).ToString();
                                hisabs.monthIndex = mn;
                                hisabs.year = selYear;
                                hisabs.bajetHisabType = "kharcha";
                                hisabs.jornalNo = objJornalKha[0].jornalNo;
                                hisabs.talab = CalculateJornalItem(objJornalKha, "२११११");
                                hisabs.mahangiBhatta = CalculateJornalItem(objJornalKha, "२१११३");
                                hisabs.anyaBhatta = CalculateJornalItem(objJornalKha, "२१११९");
                                hisabs.posak = CalculateJornalItem(objJornalKha, "२११२१");
                                hisabs.paniBijuli = CalculateJornalItem(objJornalKha, "२२१११");
                                hisabs.sancharMahasul = CalculateJornalItem(objJornalKha, "२२११२");
                                hisabs.endhan = CalculateJornalItem(objJornalKha, "२२२११");
                                hisabs.sanchalanMarmat = CalculateJornalItem(objJornalKha, "२२२१२");
                                hisabs.bima = CalculateJornalItem(objJornalKha, "२२२१३");
                                hisabs.karyalayaSambandhi = CalculateJornalItem(objJornalKha, "२२३११");
                                hisabs.pustakSamagri = CalculateJornalItem(objJornalKha, "२२३१३");
                                hisabs.sewaParamarsa = CalculateJornalItem(objJornalKha, "२२४११");
                                hisabs.anyaSewaSulka = CalculateJornalItem(objJornalKha, "२२४१२");
                                hisabs.karyakram = CalculateJornalItem(objJornalKha, "२२५२२");
                                hisabs.bibidhaKaryakram = CalculateJornalItem(objJornalKha, "२२५२९");
                                hisabs.anugamanMulyankan = CalculateJornalItem(objJornalKha, "२२६११");
                                hisabs.bhraman = CalculateJornalItem(objJornalKha, "२२६१२");
                                hisabs.bibidha = CalculateJornalItem(objJornalKha, "२२७११");
                                hisabs.nepDateStr = objJornalKha[0].nepDateStr;
                                hisabs.fyId = fyId;
                                objHisab.Add(hisabs);
                            }
                        }
                    }
                }
                }

            }
            var objKhaSiNa = db.khaSiNas.Where(m => m.baUSiNaId == baUSiNaId).ToList();
            //budget tarfa
            {
                hisab hisabs = new hisab();
                hisabs.nepDate = DateTime.Now;
                hisabs.baUSiNo = objBaUSiNa.baUSiNo;
                hisabs.bibaran = "आ.ब." + objFiscalYear.nepFy;
                hisabs.month = ((month)mn).ToString();
                hisabs.monthIndex = mn;
                hisabs.year = selYear;
                hisabs.bajetHisabType = "budget";
                hisabs.jornalNo = "";
                hisabs.fyId = fyId;
                objHisab.Add(hisabs);
            }
            var objHisabBudget = objHisab.FirstOrDefault(m => (m.bibaran == ("आ.ब." + objFiscalYear.nepFy)) && (m.monthIndex == mn) && (m.fyId == fyId) && (m.bajetHisabType == "budget"));

            foreach (var item in objKhaSiNa)
            {
                string khaSiNo = item.khaSiNo.Trim();
                switch (khaSiNo)
                {
                    case "२११११":
                        objHisabBudget.talab = item.khaRakam;
                        break;
                    case "२१११३":
                        objHisabBudget.mahangiBhatta = item.khaRakam;
                        break;
                    case "२१११९":
                        objHisabBudget.anyaBhatta = item.khaRakam;
                        break;
                    case "२११२१":
                        objHisabBudget.posak = item.khaRakam;
                        break;
                    case "२२१११":
                        objHisabBudget.paniBijuli = item.khaRakam;
                        break;
                    case "२२११२":
                        objHisabBudget.sancharMahasul = item.khaRakam;
                        break;
                    case "२२२११":
                        objHisabBudget.endhan = item.khaRakam;
                        break;
                    case "२२२१२":
                        objHisabBudget.sanchalanMarmat = item.khaRakam;
                        break;
                    case "२२२१३":
                        objHisabBudget.bima = item.khaRakam;
                        break;
                    case "२२३११":
                        objHisabBudget.karyalayaSambandhi = item.khaRakam;
                        break;
                    case "२२३१३":
                        objHisabBudget.pustakSamagri = item.khaRakam;
                        break;
                    case "२२४११":
                        objHisabBudget.sewaParamarsa = item.khaRakam;
                        break;
                    case "२२४१२":
                        objHisabBudget.anyaSewaSulka = item.khaRakam;
                        break;
                    case "२२५२२":
                        objHisabBudget.karyakram = item.khaRakam;
                        break;
                    case "२२५२९":
                        objHisabBudget.bibidhaKaryakram = item.khaRakam;
                        break;
                    case "२२६११":
                        objHisabBudget.anugamanMulyankan = item.khaRakam;
                        break;
                    case "२२६१२":
                        objHisabBudget.bhraman = item.khaRakam;
                        break;
                    case "२२७११":
                        objHisabBudget.bibidha = item.khaRakam;
                        break;
                    default:
                        break;
                }
            }


            //Calculation for आ.ल्या.
            var objJornalKharchaPrevMonth = db.jornalEntries.Where(m => (m.baUSiNaId == baUSiNaId) && (m.jornalType == "खर्च" || m.jornalType == "पेश्की") && (m.fyId == fyId) && (m.monthIndex < mn) && (m.deCre == "debit")).ToList();
            var objJornalKharchaFarchaut = db.jornalEntries.Where(m => (m.baUSiNaId == baUSiNaId) && m.jornalType == "पेश्की फछर्यौट" && m.fyId == fyId && m.monthIndex < mn && m.deCre == "credit" && m.bibaran == "एकल खाता कोष प्रणाली").ToList();
            foreach (var item in objJornalKharchaFarchaut)
            {
                jornalEntries jornal = db.jornalEntries.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == item.monthIndex && m.jornalNo == item.jornalNo && m.jornalType == "पेश्की फछर्यौट" && m.deCre == "debit" && m.baUSiNaId == baUSiNaId);
                item.hisabNo = jornal.hisabNo;
                item.debit = item.credit;
            }
            objJornalKharchaPrevMonth.Concat(objJornalKharchaFarchaut);
            List<hisab> objHisabPrev = new List<hisab> { new hisab { nepDate = DateTime.Now } };
            if (objJornalKharchaPrevMonth.Count == 0)
            {
                hisab hisabs = new hisab();
                hisabs.nepDate = DateTime.Now;
                hisabs.nepDateStr = "";
                hisabs.baUSiNo = objBaUSiNa.baUSiNo;
                hisabs.bibaran = "";
                hisabs.month = ((month)mn).ToString();
                hisabs.monthIndex = mn;
                hisabs.year = selYear;
                hisabs.bajetHisabType = "kharchaUpto";
                hisabs.jornalNo = "0";
                hisabs.talab = 0;
                hisabs.mahangiBhatta = 0;
                hisabs.anyaBhatta = 0;
                hisabs.posak = 0;
                hisabs.paniBijuli = 0;
                hisabs.sancharMahasul = 0;
                hisabs.endhan = 0;
                hisabs.sanchalanMarmat = 0;
                hisabs.bima = 0;
                hisabs.karyalayaSambandhi = 0;
                hisabs.pustakSamagri = 0;
                hisabs.sewaParamarsa = 0;
                hisabs.anyaSewaSulka = 0;
                hisabs.karyakram = 0;
                hisabs.bibidhaKaryakram = 0;
                hisabs.anugamanMulyankan = 0;
                hisabs.bhraman = 0;
                hisabs.bibidha = 0;
                hisabs.fyId = fyId;
                objHisabPrev[0] = hisabs;
            }
            else
            {
                hisab hisabs = new hisab();
                hisabs.nepDate = DateTime.Now;
                hisabs.nepDateStr = "";
                hisabs.baUSiNo = objBaUSiNa.baUSiNo;
                hisabs.bibaran = "";
                hisabs.month = ((month)mn).ToString();
                hisabs.monthIndex = mn;
                hisabs.year = selYear;
                hisabs.bajetHisabType = "kharchaUpto";
                hisabs.jornalNo = "0";
                hisabs.talab = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२११११").Select(m => m.debit).Sum();
                hisabs.mahangiBhatta = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२१११३").Select(m => m.debit).Sum();
                hisabs.anyaBhatta = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२१११९").Select(m => m.debit).Sum();
                hisabs.posak = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२११२१").Select(m => m.debit).Sum();
                hisabs.paniBijuli = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२१११").Select(m => m.debit).Sum();
                hisabs.sancharMahasul = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२११२").Select(m => m.debit).Sum();
                hisabs.endhan = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२२११").Select(m => m.debit).Sum();
                hisabs.sanchalanMarmat = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२२१२").Select(m => m.debit).Sum();
                hisabs.bima = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२२१३").Select(m => m.debit).Sum();
                hisabs.karyalayaSambandhi = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२३११").Select(m => m.debit).Sum();
                hisabs.pustakSamagri = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२३१३").Select(m => m.debit).Sum();
                hisabs.sewaParamarsa = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२४११").Select(m => m.debit).Sum();
                hisabs.anyaSewaSulka = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२४१२").Select(m => m.debit).Sum();
                hisabs.karyakram = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२५२२").Select(m => m.debit).Sum();
                hisabs.bibidhaKaryakram = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२५२९").Select(m => m.debit).Sum();
                hisabs.anugamanMulyankan = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२६११").Select(m => m.debit).Sum();
                hisabs.bhraman = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२६१२").Select(m => m.debit).Sum();
                hisabs.bibidha = objJornalKharchaPrevMonth.Where(m => m.hisabNo == "२२७११").Select(m => m.debit).Sum();
                objHisabPrev[0] = hisabs;
                hisabs.fyId = fyId;
            }
            ViewBag.prevMonthtotal = objHisabPrev[0].talab + objHisabPrev[0].mahangiBhatta + objHisabPrev[0].anyaBhatta + objHisabPrev[0].posak + objHisabPrev[0].paniBijuli + objHisabPrev[0].sancharMahasul + objHisabPrev[0].endhan + objHisabPrev[0].sanchalanMarmat + objHisabPrev[0].bima + objHisabPrev[0].karyalayaSambandhi + objHisabPrev[0].pustakSamagri + objHisabPrev[0].sewaParamarsa + objHisabPrev[0].anyaSewaSulka + objHisabPrev[0].karyakram + objHisabPrev[0].bibidhaKaryakram + objHisabPrev[0].anugamanMulyankan + objHisabPrev[0].bhraman + objHisabPrev[0].bibidha;

            ViewBag.hisabPrevMonth = objHisabPrev;
            return View(objHisab.ToList());
        }



        public decimal CalculateJornalItem(List<jornalEntries> objJornal, string khaSiNo)
        {
            if (objJornal.FirstOrDefault(m => m.hisabNo == khaSiNo) != null)
            {
                return objJornal.Where(m => m.hisabNo == khaSiNo).Sum(m => m.debit);
            }
            else
            {
                return 0;
            }

        }

        public decimal CalculateJornalItemNikasa(List<jornalEntries> objJornal, string khaSiNo)
        {
            if (objJornal.FirstOrDefault(m => m.hisabNo == khaSiNo) != null)
            {
                return objJornal.Where(m => m.hisabNo == khaSiNo).Sum(m => m.credit);
            }
            else
            {
                return 0;
            }
        }


        [HttpPost]
        public JsonResult checkArtha(string fyId, string monthIndex)
        {
            try
            {
                int fisId = Convert.ToInt32(fyId);
                int mn = Convert.ToInt32(monthIndex);
                var objArtha = db.arthaBudgets.Where(m => m.fyId == fisId && m.monthIndex == mn).ToList();
                if (objArtha.Count > 0)
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
                return Json("Null");
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



        public ActionResult TalabiBharpaiBibaran(int fyId, int mn)
        {
            var objYear = db.yearMonths.FirstOrDefault(m => m.monthIndex == mn && m.fyId == fyId);
            int yearMonthId = 0;
            if (objYear != null)
            {
                yearMonthId = objYear.yearMonthId;
            }

            var objTalabiBharpai =
                                    (from T in db.talabiBharpais
                                     join Y in db.yearMonths on T.yearMonthId equals Y.yearMonthId
                                     join O in db.officers on T.officerId equals O.officerId
                                     where Y.yearMonthId == yearMonthId
                                     select new
                                     {
                                         T.talabiId,
                                         T.suruScaleGrade,
                                         T.kaSaKoThap,
                                         T.suruBimaTotal,
                                         T.talabiBhattaTotal,
                                         T.kaSaKoKatti,
                                         T.sapati,
                                         T.naLaKos,
                                         T.pariKar,
                                         T.saSuKar,
                                         T.kattiTotal,
                                         T.pauneTotal,
                                         T.yearMonthId,
                                         T.officerId
                                    ,
                                         O.fullName,
                                         O.pisNo,
                                         O.darja,
                                         O.bankAccNo,
                                         O.sthaiLekhaNo,
                                         O.bimaPariNo,
                                         O.bimaCode,
                                         O.bimaSheetRollNo,
                                         T.suruScale,
                                         T.gradeRakam,
                                         T.mahangiBhatta,
                                         T.jokhimBhatta,
                                         T.bima,
                                         O.status,
                                         O.jobType,
                                         Y.year,
                                         Y.month
                                     }).ToList();
            if (objTalabiBharpai == null || objYear == null)
            {
                TempData["ErrorMessage"] = "No Record Found";
                return RedirectToAction("IndexTalabiBharpai", "Report");

            }
            var objPosak = db.bhuktaniAdeshs.FirstOrDefault(m => m.fyId == fyId && m.monthIndex == mn && m.khaSiNo == "२११२१");
            decimal posak = 0;
            if (objPosak != null)
            {
                posak = objPosak.rakam;
            }
            List<talabiBharpaiVM> objListTalabi = new List<talabiBharpaiVM>();


            foreach (var item in objTalabiBharpai)
            {
                talabiBharpaiVM objBharpai = new talabiBharpaiVM();
                objBharpai.talabiId = item.talabiId;
                objBharpai.suruScaleGrade = item.suruScaleGrade;
                objBharpai.kaSaKoThap = item.kaSaKoThap;
                objBharpai.suruBimaTotal = item.suruBimaTotal;
                objBharpai.talabiBhattaTotal = item.talabiBhattaTotal;
                objBharpai.kaSaKoKatti = item.kaSaKoKatti;
                objBharpai.sapati = item.sapati;
                objBharpai.naLaKos = item.naLaKos;
                objBharpai.pariKar = item.pariKar;
                objBharpai.saSukar = item.saSuKar;
                objBharpai.kattiTotal = item.kaSaKoKatti + item.naLaKos + 2 * item.bima + item.pariKar + item.saSuKar + item.sapati;
                objBharpai.pauneTotal = item.talabiBhattaTotal - (item.kaSaKoKatti + item.naLaKos + 2 * item.bima + item.pariKar + item.saSuKar + item.sapati);// item.pauneTotal;
                objBharpai.officerId = item.officerId;//,O.fullName,O.pisNo,O.darja,O.bankAccNo,O.sthaiLekhaNo,O.bimaPariNo,O.bimaCode,O.bimaSheetRollNo
                objBharpai.fullName = item.fullName;                   //,O.suruScale,O.gradeDar,O.gradeSankhya,O.mahangiBhatta,O.jokhimBhatta,O.bima,O.status,O.jobType,Y.year,Y.month
                objBharpai.pisNo = item.pisNo;
                objBharpai.darja = item.darja;
                objBharpai.bankAccNo = item.bankAccNo;
                objBharpai.sthaiLekhaNo = item.sthaiLekhaNo;
                objBharpai.bimaPariNo = item.bimaPariNo;
                objBharpai.bimaCode = item.bimaCode;
                objBharpai.bimaSheetRollNo = item.bimaSheetRollNo;
                objBharpai.suruScale = item.suruScale;
                objBharpai.gradeDar = item.gradeRakam;
                objBharpai.mahangiBhatta = item.mahangiBhatta;
                objBharpai.jokhimBhatta = item.jokhimBhatta;
                objBharpai.bima = item.bima;
                objBharpai.status = item.status;
                objBharpai.jobType = item.jobType;
                objBharpai.year = item.year;
                objBharpai.month = item.month;
                objBharpai.yearMonthId = item.yearMonthId;
                objListTalabi.Add(objBharpai);
            }
            ViewBag.posak = posak;
            return View(objListTalabi);
        }






    }
}