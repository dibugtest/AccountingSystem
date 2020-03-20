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
    public class ByehoraController : Controller
    {
        private AccountContext db = new AccountContext();

        // GET: /Byehora/
        public ActionResult Index()
        {
            var byehoras = db.byehoras.Include(b => b.khaSiNa);
            return View(byehoras.ToList());
        }

        // GET: /Byehora/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            byehora byehora = db.byehoras.Find(id);
            if (byehora == null)
            {
                return HttpNotFound();
            }
            return View(byehora);
        }

        // GET: /Byehora/Create
        public ActionResult Create()
        {
            List<byehora> byehoras = new List<byehora>{new byehora { byehoraId=0}};
            return View(byehoras);
        }

        // POST: /Byehora/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<byehora> byehoras)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var item in byehoras)
                    {
                        if(item.hisabNo==null)
                        {
                            byehora objByehora = new byehora();
                            objByehora.byehoraName=TrimText(item.byehoraName);
                            objByehora.hisabNo=TrimText(item.hisabNo);
                            objByehora.remarks=TrimText(item.remarks);
                            objByehora.khaSiNaId=null;
                            db.byehoras.Add(objByehora);
                            db.SaveChanges();
                        }
                        else
                        {
                            string hisabNo =TrimText(item.hisabNo);
                            var objKhaSiNa = db.khaSiNas.FirstOrDefault(m=>m.khaSiNo==hisabNo);
                            if(objKhaSiNa==null)
                            {
                                byehora objByehora = new byehora();
                                objByehora.byehoraName=TrimText(item.byehoraName);
                                objByehora.hisabNo =TrimText(item.hisabNo);
                                objByehora.remarks =TrimText(item.remarks);
                                objByehora.khaSiNaId = null;
                                db.byehoras.Add(objByehora);
                                db.SaveChanges();
                            }
                            else
                            {
                                byehora objByehora = new byehora();
                                objByehora.byehoraName = TrimText(item.byehoraName);
                                objByehora.hisabNo = TrimText(item.hisabNo);
                                objByehora.remarks = TrimText(item.remarks);
                                objByehora.khaSiNaId = objKhaSiNa.khaSiNaId;
                                db.byehoras.Add(objByehora);
                                db.SaveChanges();
                            }
                        }
                        
                    }
                }
                ViewBag.Message = "Record Inserted Successfully";
                return RedirectToAction("index", "Byehora");
            }
            catch
            {
                ViewBag.ErrorMessage = "Error! Please Try Again";
                return View(byehoras);
            }
        }

        // GET: /Byehora/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            byehora byehora = db.byehoras.Find(id);
            if (byehora == null)
            {
                return HttpNotFound();
            }
            ViewBag.khaSiNaId = new SelectList(db.khaSiNas, "khaSiNaId", "khaSirsak", byehora.khaSiNaId);
            return View(byehora);
        }

        // POST: /Byehora/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "byehoraId,byehoraName,hisabNo,khaSiNaId,remarks")] byehora byehora)
        {
            if (ModelState.IsValid)
            {
                var objByehora = db.byehoras.Find(byehora.byehoraId);
                objByehora.byehoraName = TrimText(byehora.byehoraName);
                objByehora.hisabNo = TrimText(byehora.hisabNo);
                objByehora.remarks = TrimText(byehora.remarks);
                objByehora.khaSiNaId = byehora.khaSiNaId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.khaSiNaId = new SelectList(db.khaSiNas, "khaSiNaId", "khaSirsak", byehora.khaSiNaId);
            return View(byehora);
        }

        // GET: /Byehora/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            byehora byehora = db.byehoras.Find(id);
            if (byehora == null)
            {
                return HttpNotFound();
            }
            return View(byehora);
        }

        // POST: /Byehora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            byehora byehora = db.byehoras.Find(id);
            db.byehoras.Remove(byehora);
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
            var myobj = db.byehoras.SingleOrDefault(p => p.byehoraId == id);
            return Json(myobj);
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
