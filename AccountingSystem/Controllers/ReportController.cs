using AccountingSystem.DAL;
using AccountingSystem.Models;
using AccountingSystem.Reports;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private AccountContext db = new AccountContext();

        private DataSetJornalEntries dsJE = new DataSetJornalEntries();

        private DataSetBhuktani dsBhu = new DataSetBhuktani();

        private DataSetArthaBudget dsArtha = new DataSetArthaBudget();

        private DataSetTalabi DSTalabi = new DataSetTalabi();

        public ReportController()
        {
        }

        [HttpPost]
        public JsonResult filBhuktaniNo(string fyId, string mn)
        {
            int num;
            int num1;
            try
            {
                num = Convert.ToInt32(fyId);
                num1 = Convert.ToInt32(mn);
            }
            catch
            {
                num = this.db.fisYears.FirstOrDefault<fiscalYear>((fiscalYear m) => m.status).fyId;
                num1 = 0;
            }
            List<bhuktaniAdesh> list = (
                from d in this.db.bhuktaniAdeshs
                group d by new { bhuktaniAdeshNo = d.bhuktaniAdeshNo } into mygroup
                select mygroup.FirstOrDefault<bhuktaniAdesh>()).ToList<bhuktaniAdesh>();
            List<bhuktaniAdesh> bhuktaniAdeshes = list.Where<bhuktaniAdesh>((bhuktaniAdesh m) =>
            {
                if (m.fyId != num || m.monthIndex != num1)
                {
                    return false;
                }
                return m.bhuktaniType == "भुक्तानी आदेश";
            }).ToList<bhuktaniAdesh>();
            if (bhuktaniAdeshes.Count == 0)
            {
                return base.Json(null);
            }
            return base.Json(bhuktaniAdeshes);
        }

        public JsonResult filJornalNo(string fyId, string mn, string jornalType)
        {
            int num;
            int num1;
            string str = "खर्च";
            try
            {
                num = Convert.ToInt32(fyId);
                num1 = Convert.ToInt32(mn);
                str = jornalType.ToString();
            }
            catch
            {
                num = this.db.fisYears.FirstOrDefault<fiscalYear>((fiscalYear m) => m.status).fyId;
                num1 = 0;
            }
            List<jornalEntries> list = (
                from d in this.db.jornalEntries
                group d by new { jornalNo = d.jornalNo } into mygroup
                select mygroup.FirstOrDefault<jornalEntries>()).ToList<jornalEntries>();
            List<string> strs = list.ToList<jornalEntries>().Where<jornalEntries>((jornalEntries m) =>
            {
                if (m.fyId != num || m.monthIndex != num1)
                {
                    return false;
                }
                return m.jornalType == str;
            }).Select<jornalEntries, string>((jornalEntries m) => m.jornalNo).ToList<string>();
            return base.Json(strs);
        }

        [HttpPost]
        public JsonResult getJornalEntries()
        {
            IQueryable<jornalEntries> variable =
                from d in this.db.jornalEntries
                group d by new { jornalNo = d.jornalNo } into mygroup
                select mygroup.FirstOrDefault<jornalEntries>();
            var list = (
                from j in variable
                select new { jornalNo = j.jornalNo, jornalType = j.jornalType, nepDate = j.nepDate, month = j.month }).ToList();
            return base.Json(list);
        }

        public string getYear(int fyId, int monthIndex)
        {
            DbSet<fiscalYear> fiscalYears = this.db.fisYears;
            object[] objArray = new object[] { fyId };
            fiscalYear _fiscalYear = fiscalYears.Find(objArray);
            string str = this.UnicodeToString(_fiscalYear.nepFy);
            string unicode = null;
            if (monthIndex < 9)
            {
                char[] chrArray = new char[] { '/' };
                int num = int.Parse(str.Split(chrArray)[0]);
                unicode = this.StringToUnicode(num.ToString());
            }
            else
            {
                char[] chrArray1 = new char[] { '/' };
                int num1 = int.Parse(str.Split(chrArray1)[0]) + 1;
                unicode = this.StringToUnicode(num1.ToString());
            }
            return unicode;
        }

        public ActionResult IndexArthaBudget()
        {
            IQueryable<arthaBudget> variable =
                from d in this.db.arthaBudgets
                group d by new { year = d.year } into mygroup
                select mygroup.FirstOrDefault<arthaBudget>();
            IQueryable<arthaBudget> arthaBudgets =
                from d in this.db.arthaBudgets
                orderby d.monthIndex
                group d by new { month = d.month } into mygroup
                select mygroup.FirstOrDefault<arthaBudget>();
            ((dynamic)base.ViewBag).year = new SelectList(variable, "year", "year");
            ((dynamic)base.ViewBag).month = new SelectList(arthaBudgets, "monthIndex", "month");
            return base.View();
        }

        public ActionResult IndexBhuktaniAdesh()
        {
            fiscalYear _fiscalYear = this.db.fisYears.FirstOrDefault<fiscalYear>((fiscalYear m) => m.status);
            ((dynamic)base.ViewBag).fyId = new SelectList(this.db.fisYears, "fyId", "nepFy", (object)_fiscalYear.fyId);
            return base.View();
        }

        [HttpPost]
        public ActionResult IndexBhuktaniAdesh(int fyId, int mn)
        {
            fiscalYear _fiscalYear = this.db.fisYears.FirstOrDefault<fiscalYear>((fiscalYear m) => m.status);
            ((dynamic)base.ViewBag).fyId = new SelectList(this.db.fisYears, "fyId", "nepFy", (object)_fiscalYear.fyId);
            List<bhuktaniAdesh> list = (
                from d in this.db.bhuktaniAdeshs
                where d.fyId == fyId && d.monthIndex == mn && (d.bhuktaniType == "भुक्तानी आदेश")
                group d by new { bhuktaniAdeshNo = d.bhuktaniAdeshNo } into mygroup
                select mygroup.FirstOrDefault<bhuktaniAdesh>()).ToList<bhuktaniAdesh>();
            List<bhuktaniAdesh> bhuktaniAdeshes = (
                from b in list
                select b).ToList<bhuktaniAdesh>();
            ((dynamic)base.ViewBag).bhuktaniAdesh = bhuktaniAdeshes;
            return base.View();
        }

        public ActionResult IndexJornal()
        {
            ((dynamic)base.ViewBag).baUSiNaId = new SelectList(this.db.baUSiNas.ToList<baUSiNa>(), "baUSiNaId", "baUSiNo", (object)2);
            fiscalYear _fiscalYear = this.db.fisYears.FirstOrDefault<fiscalYear>((fiscalYear m) => m.status);
            ((dynamic)base.ViewBag).fyId = new SelectList(this.db.fisYears, "fyId", "nepFy", (object)_fiscalYear.fyId);
            return base.View();
        }

        [HttpPost]
        public ActionResult IndexJornal(int mn, int fyId)
        {
            List<jornalEntries> list = (
                from d in this.db.jornalEntries
                where (d.jornalType == "खर्च") && d.fyId == fyId && d.monthIndex == mn
                group d by new { jornalNo = d.jornalNo } into mygroup
                select mygroup.FirstOrDefault<jornalEntries>()).ToList<jornalEntries>();
            ((dynamic)base.ViewBag).kharchaJornal = list;
            List<jornalEntries> _jornalEntries = (
                from d in this.db.jornalEntries
                where (d.jornalType == "निकासा") && d.fyId == fyId && d.monthIndex == mn
                group d by new { jornalNo = d.jornalNo } into mygroup
                select mygroup.FirstOrDefault<jornalEntries>()).ToList<jornalEntries>();
            ((dynamic)base.ViewBag).nikasaJornal = _jornalEntries;
            List<jornalEntries> list1 = (
                from d in this.db.jornalEntries
                where (d.jornalType == "कट्टी") && d.fyId == fyId && d.monthIndex == mn
                group d by new { jornalNo = d.jornalNo } into mygroup
                select mygroup.FirstOrDefault<jornalEntries>()).ToList<jornalEntries>();
            ((dynamic)base.ViewBag).kattiJornal = list1;
            return base.View();
        }

        public ActionResult IndexTalabiBharpai(string message)
        {
            fiscalYear _fiscalYear = this.db.fisYears.FirstOrDefault<fiscalYear>((fiscalYear m) => m.status);
            ((dynamic)base.ViewBag).fyId = new SelectList(this.db.fisYears, "fyId", "nepFy", (object)_fiscalYear.fyId);
            return base.View();
        }

        public ActionResult ReportArthaBudget(string year, int mn)
        {
            if (year != null)
            {
                if ((
                    from m in this.db.arthaBudgets
                    where (m.year == year) && m.monthIndex == mn
                    select m).ToList<arthaBudget>().Count == 0)
                {
                    base.TempData["ErrorMessage"] = "Record not found for selected year and month.Please select different pair of year,month and submit";
                    return base.RedirectToAction("IndexArthaBudget");
                }
            }
            else if (year == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportViewer reportViewer = new ReportViewer()
            {
                ProcessingMode = ProcessingMode.Local,
                SizeToReportContent = true,
                Width = Unit.Pixel(595),
                Height = Unit.Pixel(842)
            };
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = sqlConnection,
                CommandType = CommandType.StoredProcedure,
                CommandText = "dbo.Select_ArthaBudget"
            };
            sqlCommand.Parameters.AddWithValue("@year", year);
            sqlCommand.Parameters.AddWithValue("@month", mn);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(this.dsArtha, this.dsArtha.arthaBudgets.TableName);
            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportArthaBudget.rdlc");
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSArthaBudget", this.dsArtha.Tables[0]));
            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
            return base.View("Report");
        }

        public ActionResult ReportBhuktaniAdesh(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportViewer reportViewer = new ReportViewer()
            {
                ProcessingMode = ProcessingMode.Local,
                SizeToReportContent = true,
                Width = Unit.Pixel(595),
                Height = Unit.Pixel(842)
            };
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = sqlConnection,
                CommandType = CommandType.StoredProcedure,
                CommandText = "dbo.Select_BhuktaniAdesh"
            };
            sqlCommand.Parameters.AddWithValue("@bhuktaniAdeshNo", id);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(this.dsBhu, this.dsBhu.bhuktaniAdeshes.TableName);
            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBhuktaniAdesh.rdlc");
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBhuktani", this.dsBhu.Tables[0]));
            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
            return base.View("Report");
        }

        //public ActionResult ReportGen(int fyId, int mn, string reportType)
        //{
        //    if (reportType == "TB" && fyId > 0 && mn >= 0)
        //    {
        //        return base.RedirectToAction("TalabiBharpaiBibaran", "KharchaKoFantwari", new { fyId = fyId, mn = mn });
        //    }
        //    else
        //    {
        //        this.getYear(fyId, mn);
        //        if (this.db.yearMonths.FirstOrDefault<yearMonth>((yearMonth m) => m.fyId == fyId && m.monthIndex == mn) == null)
        //        {
        //            base.TempData["ErrorMessage"] = "No Record found";
        //            return base.RedirectToAction("IndexTalabiBharpai", "Report");
        //        }
        //        int num = this.db.yearMonths.FirstOrDefault<yearMonth>((yearMonth m) => m.fyId == fyId && m.monthIndex == mn).yearMonthId;
        //        if ((
        //            from T in this.db.talabiBharpais
        //            join Y in this.db.yearMonths on T.yearMonthId equals Y.yearMonthId
        //            join O in this.db.officers on T.officerId equals O.officerId
        //            where Y.yearMonthId == num
        //            select new { talabiId = T.talabiId }).ToList() == null)
        //        {
        //            base.TempData["ErrorMessage"] = "No Record found";
        //            return base.RedirectToAction("IndexTalabiBharpai", "Report");
        //        }
        //        string str = reportType.ToString();
        //        if (fyId == 0 || reportType == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        ReportViewer reportViewer = new ReportViewer()
        //        {
        //            ProcessingMode = ProcessingMode.Local,
        //            SizeToReportContent = true,
        //            Width = Unit.Pixel(1785),
        //            Height = Unit.Pixel(842)
        //        };
        //        SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
        //        SqlCommand sqlCommand = new SqlCommand()
        //        {
        //            Connection = sqlConnection,
        //            CommandType = CommandType.StoredProcedure,
        //            CommandText = "dbo.Select_TalabiBharpai"
        //        };
        //        sqlCommand.Parameters.AddWithValue("@yearMonthId", num);
        //        if (str == "BA" || str == "TX")
        //        {
        //            sqlCommand.Parameters.AddWithValue("@event", "TH");
        //            sqlCommand.Parameters.AddWithValue("@jobType", "स्थाई");
        //        }
        //        else
        //        {
        //            sqlCommand.Parameters.AddWithValue("@event", "KR");
        //            sqlCommand.Parameters.AddWithValue("@jobType", "स्थाई");
        //        }
        //        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        //        sqlDataAdapter.Fill(this.DSTalabi, this.DSTalabi.talabiBharpais.TableName);

        //        if (str == "BA")
        //        {
        //            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBank.rdlc");
        //            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBank", this.DSTalabi.Tables[0]));
        //            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
        //            return base.View("Report");
        //        }
        //        if (str == "TX")
        //        {
        //            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportTaxOffice.rdlc");
        //            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSTaxOffice", this.DSTalabi.Tables[0]));
        //            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
        //            return base.View("Report");
        //        }
        //        if (str == "NA")
        //        {
        //            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportNaLaKos.rdlc");
        //            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSNaLaKos", this.DSTalabi.Tables[0]));
        //            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
        //            return base.View("Report");
        //        }
        //        if (str == "BI")
        //        {
        //            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBima.rdlc");
        //            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBima", this.DSTalabi.Tables[0]));
        //            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
        //            return base.View("Report");
        //        }
        //        if (str == "BU")
        //        {
        //            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBhuktaniAdesh.rdlc");
        //            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBhuktaniAdesh", this.DSTalabi.Tables[0]));
        //            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
        //            return base.View("Report");
        //        }
        //        if (str != "SK")
        //        {
        //            return base.RedirectToAction("Index");
        //        }
        //        reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportSanchayakos.rdlc");
        //        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSSanchayaKos", this.DSTalabi.Tables[0]));
        //        ((dynamic)base.ViewBag).ReportViewer = reportViewer;
        //        return base.View("Report");
        //    }
        //}

        public ActionResult ReportJornalEntries(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            char[] chrArray = new char[] { ',' };
            string str = id.Split(chrArray)[0];
            char[] chrArray1 = new char[] { ',' };
            string str1 = id.Split(chrArray1)[1];
            ReportViewer reportViewer = new ReportViewer()
            {
                ProcessingMode = ProcessingMode.Local,
                SizeToReportContent = true,
                Width = Unit.Pixel(595),
                Height = Unit.Pixel(842)
            };
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = sqlConnection,
                CommandType = CommandType.StoredProcedure,
                CommandText = "dbo.Select_JornalEntries"
            };
            sqlCommand.Parameters.AddWithValue("@jornalNo", str);
            sqlCommand.Parameters.AddWithValue("@jornalType", str1);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(this.dsJE, this.dsJE.jornalEntries.TableName);
            reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportJornalEntries.rdlc");
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSJornalEntries", this.dsJE.Tables[0]));
            ((dynamic)base.ViewBag).ReportViewer = reportViewer;
            return base.View("Report");
        }

        public ActionResult SelectYearMonth()
        {
            return base.View();
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




        public ActionResult ReportGen(int fyId, int mn, string reportType)
        {
            if (reportType == "TB" && fyId > 0 && mn >= 0)
            {
                return base.RedirectToAction("TalabiBharpaiBibaran", "KharchaKoFantwari", new { fyId = fyId, mn = mn });
            }
            else
            {
                this.getYear(fyId, mn);
                if (this.db.yearMonths.FirstOrDefault<yearMonth>((yearMonth m) => m.fyId == fyId && m.monthIndex == mn) == null)
                {
                    base.TempData["ErrorMessage"] = "No Record found";
                    return base.RedirectToAction("IndexTalabiBharpai", "Report");
                }
                int num = this.db.yearMonths.FirstOrDefault<yearMonth>((yearMonth m) => m.fyId == fyId && m.monthIndex == mn).yearMonthId;
                if ((
                    from T in this.db.talabiBharpais
                    join Y in this.db.yearMonths on T.yearMonthId equals Y.yearMonthId
                    join O in this.db.officers on T.officerId equals O.officerId
                    where Y.yearMonthId == num
                    select new { talabiId = T.talabiId }).ToList() == null)
                {
                    base.TempData["ErrorMessage"] = "No Record found";
                    return base.RedirectToAction("IndexTalabiBharpai", "Report");
                }
                string str = reportType.ToString();
                if (fyId == 0 || reportType == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ReportViewer reportViewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Pixel(1785),
                    Height = Unit.Pixel(842)
                };
                SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
                SqlCommand sqlCommand = new SqlCommand()
                {
                    Connection = sqlConnection,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "dbo.Select_TalabiBharpai"
                };
                sqlCommand.Parameters.AddWithValue("@yearMonthId", num);
                if (str == "BA" || str == "TX")
                {
                    sqlCommand.Parameters.AddWithValue("@event", "T");
                    sqlCommand.Parameters.AddWithValue("@jobType", "स्थाई");
                }
                else if (str == "SP")
                {
                    sqlCommand.Parameters.AddWithValue("@event", "SP");
                    sqlCommand.Parameters.AddWithValue("@jobType", "स्थाई");
                }
                else
                {
                    sqlCommand.Parameters.AddWithValue("@event", "K");
                    sqlCommand.Parameters.AddWithValue("@jobType", "स्थाई");
                }
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(this.DSTalabi, this.DSTalabi.talabiBharpais.TableName);

                if (str == "BA")
                {
                    reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBank.rdlc");
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBank", this.DSTalabi.Tables[0]));
                    ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                    return base.View("Report");
                }
                if (str == "TX")
                {
                    reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportTaxOffice.rdlc");
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSTaxOffice", this.DSTalabi.Tables[0]));
                    ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                    return base.View("Report");
                }
                if (str == "NA")
                {
                    reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportNaLaKos.rdlc");
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSNaLaKos", this.DSTalabi.Tables[0]));
                    ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                    return base.View("Report");
                }
                if (str == "BI")
                {
                    reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBima.rdlc");
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBima", this.DSTalabi.Tables[0]));
                    ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                    return base.View("Report");
                }
                if (str == "SP")
                {
                  
                    reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportSapati.rdlc");
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSSapati", this.DSTalabi.Tables[0]));
                    ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                    return base.View("Report");
                }

                if (str == "BU")
                {
                    reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBhuktaniAdesh.rdlc");
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBhuktaniAdesh", this.DSTalabi.Tables[0]));
                    ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                    return base.View("Report");
                }
                if (str == "BU")
                {
                    reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportBhuktaniAdesh.rdlc");
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSBhuktaniAdesh", this.DSTalabi.Tables[0]));
                    ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                    return base.View("Report");
                }

                if (str != "SK")
                {
                    return base.RedirectToAction("Index");
                }
                reportViewer.LocalReport.ReportPath = string.Concat(base.Request.MapPath(base.Request.ApplicationPath), "\\Reports\\ReportSanchayakos.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSSanchayaKos", this.DSTalabi.Tables[0]));
                ((dynamic)base.ViewBag).ReportViewer = reportViewer;
                return base.View("Report");
            }
        }







    }
}