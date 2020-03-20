using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult backup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult backup(string directory)
        {
            string backupDIR = "";
            string drive = "";
            if (System.IO.Directory.Exists("D:\\"))
            {
                drive = "D";
                backupDIR = "D:\\Backup_Database_Account";
            }
            else if (System.IO.Directory.Exists("E:\\") && backupDIR == "")
            {
                drive = "E";
                backupDIR = "E:\\Backup_Database_Account";
            }
            else if (System.IO.Directory.Exists("F:\\") && backupDIR == "")
            {
                drive = "F";
                backupDIR = "F:\\Backup_Database_Account";
            }
            else if (System.IO.Directory.Exists("G:\\") && backupDIR == "")
            {
                drive = "G";
                backupDIR = "G:\\Backup_Database_Account";
            }

            if (!System.IO.Directory.Exists(backupDIR))
            {
                System.IO.Directory.CreateDirectory(backupDIR);
            }
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
                SqlConnection conx = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conx;
                cmd.CommandType = CommandType.StoredProcedure;
                conx.Open();
                cmd = new SqlCommand("backup database AccountingSystem to disk='" + backupDIR + "\\" + DateTime.Now.ToString("ddMMyyyy") + ".Bak'", conx);
                cmd.ExecuteNonQuery();
                conx.Close();
                ViewBag.Message = "Backup Database file created successfully in 'Backup_Database_Account' folder in "+drive+"  drive.";

                //if (SendMail())
                //{
                //    ViewBag.Message = "Backup database successfully";
                //}
                //else
                //{
                //    ViewBag.Message = "Backup database successfully but backup file not send to mail.";
                //}
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error Occured During DB backup process !<br>" + ex.ToString();
                // lblError.Text = "Error Occured During DB backup process !<br>" + ex.ToString();
            }
            return View();
        }


        //public bool SendMail()
        //{
        //    try
        //    {
        //        string from = "dbugtest2016@gmail.com"; //example:- sourabh9303@gmail.com
        //        using (MailMessage mail = new MailMessage(from, "maharjanakancha@gmail.com"))
        //        {
        //            mail.Subject = "BackUp File.";
        //            mail.Body = "Account System BackupFile";
        //            FileInfo bakFile = new FileInfo("D:\\Backup_Database_Account\\" + DateTime.Now.ToString("ddMMyyyy") + ".bak");
        //            if (bakFile != null)
        //            {
        //                var attachment = new Attachment(bakFile.FullName);
        //                mail.Attachments.Add(attachment);
        //            }
        //            mail.IsBodyHtml = false;
        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = "smtp.gmail.com";
        //            smtp.EnableSsl = true;
        //            NetworkCredential networkCredential = new NetworkCredential(from, "my@test#");
        //            smtp.UseDefaultCredentials = true;
        //            smtp.Credentials = networkCredential;
        //            smtp.Port = 587;
        //            smtp.Send(mail);
        //            ViewBag.Message = "Sent";
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
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
    }
}