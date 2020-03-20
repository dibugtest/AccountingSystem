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

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class officerController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /officer/
        public ActionResult Index()
        {
            var officers = db.officers.Include(o => o.office).OrderBy(m => m.serialNo);
            return View(officers.ToList());
        }

        // GET: /officer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            officer officer = db.officers.Find(id);
            if (officer == null)
            {
                return HttpNotFound();
            }
            return View(officer);
        }

        // GET: /officer/Create
        public ActionResult Create(int? serialNo)
        {
            if (serialNo == 0 || serialNo==null)
                return RedirectToAction("index");
            ViewBag.officeId = new SelectList(db.offices, "officeId", "karyalaye");
            ViewBag.serialNo = serialNo;
            return View();
        }

        // POST: /officer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "officerId,fullName,pisNo,jobType,email,officePhone,extenNo,mobileNo,status,officeId,darja,bankAccNo,sthaiLekhaNo,bima,bimaPariNo,bimaCode,bimaSheetRollNo,suruScale,gradeSankhya,mahangiBhatta,jokhimBhatta,gradeDar,bankShakha,jobType,naLaKos,kaSaKos,naLaKosNo,kaSaKosNo,saSuKar,paKar,serialNo,sapati")] officer officer, FormCollection col)
        {
            if (ModelState.IsValid)
            {
                int intstatus = Convert.ToInt32(officer.status);
                string getStatus = ((status)intstatus).ToString();
                officer objOfficer = new officer();
                objOfficer.fullName = TrimText(officer.fullName);
                objOfficer.pisNo = TrimText(officer.pisNo);
                objOfficer.email = officer.email;
                objOfficer.officePhone = TrimText(officer.officePhone);
                objOfficer.officeId = officer.officeId;
                objOfficer.mobileNo = TrimText(officer.mobileNo);
                objOfficer.sapati = officer.sapati;
                objOfficer.jokhimBhatta = officer.jokhimBhatta;
                objOfficer.mahangiBhatta = officer.mahangiBhatta;
                objOfficer.status = getStatus;
                objOfficer.jobType = officer.jobType;
                objOfficer.darja = TrimText(officer.darja);
                objOfficer.bankAccNo = TrimText(officer.bankAccNo);
                objOfficer.sthaiLekhaNo = TrimText(officer.sthaiLekhaNo);
                objOfficer.bima = officer.bima;
                objOfficer.bimaCode = TrimText(officer.bimaCode);
                objOfficer.bimaPariNo = TrimText(officer.bimaPariNo);
                objOfficer.bimaSheetRollNo = TrimText(officer.bimaSheetRollNo);
                objOfficer.suruScale = officer.suruScale;
                objOfficer.gradeSankhya = officer.gradeSankhya;
                objOfficer.gradeDar = officer.gradeDar;
                objOfficer.bankShakha = TrimText(officer.bankShakha);
                objOfficer.extenNo = TrimText(officer.extenNo);
                objOfficer.naLaKos = officer.naLaKos;
                objOfficer.kaSaKos = officer.kaSaKos;
                objOfficer.naLaKosNo = officer.naLaKosNo;
                objOfficer.kaSaKosNo = officer.kaSaKosNo;
                objOfficer.saSukar = officer.saSukar;
                objOfficer.paKar = officer.paKar;
                objOfficer.serialNo = officer.serialNo;

                //TempData["pisNo"] = StringToUnicode(officer.pisNo);
                db.officers.Add(objOfficer);
                db.SaveChanges();

                int officerId = db.officers.OrderByDescending(m => m.officerId).ToList()[0].officerId;

                bool result = serializeOfficer(officer.serialNo, officerId,"Up");
                if (result)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.serialNo = officer.serialNo;
            ViewBag.officeId = new SelectList(db.offices, "officeId", "karyalaya", officer.officeId);
            return View(officer);
        }

        // GET: /officer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            officer officer = db.officers.Find(id);
            var jobsType = new List<SelectListItem>{ new SelectListItem { Text = "--छान्नुहोस्--",Value="" },
                         new SelectListItem { Text = "करार", Value = "करार" },
                         new SelectListItem { Text = "स्थाई", Value = "स्थाई" } };
            ViewBag.jobType = new SelectList(jobsType.ToList(), "Text", "Value", officer.jobType);
            if (officer == null)
            {
                return HttpNotFound();
            }
            ViewBag.officeId = new SelectList(db.offices, "officeId", "karyalaye", officer.officeId);
            return View(officer);
        }

        // POST: /officer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "officerId,fullName,pisNo,email,jobType,officePhone,extenNo,mobileNo,status,officeId,darja,bankAccNo,sthaiLekhaNo,bima,bimaPariNo,bimaCode,bimaSheetRollNo,suruScale,gradeSankhya,mahangiBhatta,jokhimBhatta,gradeDar,bankShakha,jobType,naLaKos,kaSaKos,naLaKosNo,kaSaKosNo,saSuKar,paKar,serialNo,sapati")] officer officer)
        {
            if (ModelState.IsValid)
            {
                string serializeString = "Up";
                var objOfficer = db.officers.Find(officer.officerId);
                if (objOfficer.serialNo > officer.serialNo)
                {
                    serializeString = "Up";
                }
                else if(objOfficer.serialNo < officer.serialNo)
                {
                    serializeString = "Down";
                }
                else if (objOfficer.serialNo == officer.serialNo)
                {
                    serializeString = "Equal";
                }
                int intstatus = Convert.ToInt32(officer.status);
                string getStatus = ((status)intstatus).ToString();

                if (officer.serialNo <= 0)
                {
                    officer.serialNo = 1;
                }
                else if (officer.serialNo > db.officers.ToList().Count)
                {
                    officer.serialNo = db.officers.ToList().Count;
                }

                objOfficer.fullName = TrimText(officer.fullName);
                objOfficer.pisNo = TrimText(officer.pisNo);
                objOfficer.email = officer.email;
                objOfficer.officePhone = TrimText(officer.officePhone);
                objOfficer.officeId = officer.officeId;
                objOfficer.mobileNo = TrimText(officer.mobileNo);
                objOfficer.jokhimBhatta = officer.jokhimBhatta;
                objOfficer.mahangiBhatta = officer.mahangiBhatta;
                objOfficer.status = getStatus;
                objOfficer.jobType = officer.jobType;
                objOfficer.darja = TrimText(officer.darja);
                objOfficer.bankAccNo = TrimText(officer.bankAccNo);
                objOfficer.sthaiLekhaNo = TrimText(officer.sthaiLekhaNo);
                objOfficer.bima = officer.bima;
                objOfficer.bimaCode = TrimText(officer.bimaCode);
                objOfficer.bimaPariNo = TrimText(officer.bimaPariNo);
                objOfficer.bimaSheetRollNo = TrimText(officer.bimaSheetRollNo);
                objOfficer.suruScale = officer.suruScale;
                objOfficer.gradeSankhya = officer.gradeSankhya;
                objOfficer.gradeDar = officer.gradeDar;
                objOfficer.bankShakha = TrimText(officer.bankShakha);
                objOfficer.extenNo = TrimText(officer.extenNo);
                objOfficer.kaSaKos = officer.kaSaKos;
                objOfficer.naLaKos = officer.naLaKos;
                objOfficer.naLaKosNo = officer.naLaKosNo;
                objOfficer.kaSaKosNo = officer.kaSaKosNo;
                objOfficer.saSukar = officer.saSukar;
                objOfficer.paKar = officer.paKar;
                objOfficer.serialNo = officer.serialNo;
                objOfficer.sapati = officer.sapati;
                db.SaveChanges();

                bool result = serializeOfficer(officer.serialNo, officer.officerId,serializeString);

                if (result)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.serialNo = officer.serialNo;
            ViewBag.officeId = new SelectList(db.offices, "officeId", "mantralaye", officer.officeId);
            return View(officer);
        }

        // GET: /officer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            officer officer = db.officers.Find(id);
            if (officer == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    db.officers.Remove(officer);
                    db.SaveChanges();

                    var ListOfficer = db.officers.ToList().OrderBy(o => o.serialNo);
                    int i=1;
                    foreach (var item in ListOfficer)
                    {
                        item.serialNo = i;
                        i++;
                    }
                    db.SaveChanges();

                }
                catch
                {
                    TempData["ErrorMessage"] = "Record not Deleted";
                    return RedirectToAction("Index", "Officer");
                }
            }
            TempData["Message"] = "Record Deleted Successfully";
            return RedirectToAction("Index", "Officer");
        }

        // POST: /officer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            officer officer = db.officers.Find(id);
            db.officers.Remove(officer);
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
                    case "U+0970":
                        str += "4";
                        break;
                    case "U+0971":
                        str += "5";
                        break;
                    case "U+0972":
                        str += "6";
                        break;
                    case "U+0973":
                        str += "7";
                        break;
                    case "U+0974":
                        str += "8";
                        break;
                    case "U+0975":
                        str += "9";
                        break;
                    default:
                        str += c;
                        break;
                }
            }
            return str;
        }

        public bool serializeOfficer(int serialNo, int officerId,string serializeString)
        {
            int officerCount = db.officers.ToList().Count;
            if (serialNo <= 0)
            {
                serialNo = 1;
            }
            if (serialNo > officerCount)
            {
                serialNo = officerCount;
            }
            var firstList=db.officers.ToList();
            var lastList =db.officers.ToList();

            if (serializeString == "Up")
            {
                firstList = db.officers.Where(m => m.serialNo < serialNo).OrderBy(m => m.serialNo).ToList();
                lastList = db.officers.Where(m => m.serialNo >= serialNo && m.officerId != officerId).OrderBy(m => m.serialNo).ToList();
                int i = 1;
                int j = serialNo + 1;
                if (firstList.ToList().Count > 0)
                {
                    foreach (var item in firstList)
                    {
                        item.serialNo = i;
                        i++;
                    }
                }
                if (lastList.ToList().Count > 0)
                {
                    j = i + 1;
                    foreach (var item in lastList)
                    {
                        item.serialNo = j;
                        j++;
                    }
                }
            }
            else if (serializeString == "Down")
            {
                firstList = db.officers.Where(m => m.serialNo <= serialNo && m.officerId != officerId).OrderBy(m => m.serialNo).ToList();
                lastList = db.officers.Where(m => m.serialNo > serialNo ).OrderBy(m => m.serialNo).ToList();
                int i = 1;
                int j = serialNo + 1;
                if (firstList.ToList().Count > 0)
                {
                    foreach (var item in firstList)
                    {
                        item.serialNo = i;
                        i++;
                    }
                }
                if (lastList.ToList().Count > 0)
                {
                    j = i + 1;
                    foreach (var item in lastList)
                    {
                        item.serialNo = j;
                        j++;
                    }
                }

            }
            else if (serializeString == "Equal")
            {
                return true;
            }


            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
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
