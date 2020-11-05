using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MerchantService.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using OfficeOpenXml;
using System.Drawing;

namespace MerchantService.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }
            return View();
        }

        public ActionResult ActiveMerchant()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            return View();
        }

        public PartialViewResult ActiveMerchantResult()
        {
            if (HttpContext.Session["UID"] == null)
            {
                TempData["ErrorMessage"] = "Session Error";
                TempData.Keep();
            }
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult ActiveMerchantResult(DateTime startdate,DateTime enddate)
        {
            if (HttpContext.Session["UID"] == null)
            {
                TempData["ErrorMessage"] = "Session Error";
                TempData.Keep();
                return PartialView();
            }

            using (Models.MerchantService db = new Models.MerchantService())
            {

                List<mMerchant> merchantList = getActiveMerchant(startdate, enddate);

                ViewBag.MerchantVB = merchantList;

                ViewData["startdate"] = startdate.Date;
                ViewData["enddate"] = enddate.Date;
            }
            return PartialView();
        }

        internal List<mMerchant> getActiveMerchant(DateTime startdate, DateTime enddate)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return (new List<mMerchant>());
            }

            List<mMerchant> merchants = new List<mMerchant>();

            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();

                SqlCommand cmd = new SqlCommand("sp_r_active_merchant", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@startdate", startdate);
                cmd.Parameters.AddWithValue("@enddate", enddate);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mMerchant m = new mMerchant();
                    m.MPU_Merchant_ID = dt.Rows[i][0].ToString();
                    m.Merchant_Name = dt.Rows[i][1].ToString();

                    merchants.Add(m);
                }
            }
            return merchants;
        }

        
        public ActionResult PrintAdvise(string merchantid,DateTime startdate,DateTime enddate)
        {
            if (HttpContext.Session["UID"] != null)
            {
                using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
                {
                    consql.Open();

                    SqlCommand cmd = new SqlCommand("sp_rp_load_advise", consql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@merchantid", merchantid);
                    cmd.Parameters.AddWithValue("@startdate", startdate.Date);
                    cmd.Parameters.AddWithValue("@enddate", enddate.Date);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable REPDT = new DataTable();
                    da.Fill(REPDT);

                    consql.Close();

                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/ReportFiles"), "rp_PaymentAdvise.rpt"));

                    rd.SetDataSource(REPDT);

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();


                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    rd.Clone();
                    rd.Dispose();

                    return File(stream, "application/pdf", merchantid + ".pdf");
                }
            }

            else
            {
                return RedirectToAction("Index", "Portal");
            }
        }

        public ActionResult TransactionQuery()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }
            return View();
        }

        //[HttpPost]
        //public ActionResult TransactionQuery(string searchkey,DateTime startdate,DateTime enddate)
        //{
        //    if (HttpContext.Session["UID"] == null)
        //    {
        //        return RedirectToAction("Index", "Portal", null);
        //    }
            

        //    List<mTransaction> Transactions = new List<mTransaction>();

        //    using (Models.MerchantService db = new Models.MerchantService())
        //    {
        //        List<v_Transactions> trans = db.v_Transactions.Where(x => (x.MPU_Merchant_ID.Contains(searchkey) || x.Merchant_Name.Contains(searchkey)) && (x.TrxDate >= startdate && x.TrxDate <= enddate)).ToList();
        //        Transactions = trans.Select(x => new mTransaction { MPU_Merchant_ID = x.MPU_Merchant_ID, Merchant_Name = x.Merchant_Name, CardNo = x.CardNo, CardType = x.CardType, TrxDate = x.TrxDate, TrxAmount = x.TrxAmount, SettAmount = x.SettAmount }).ToList();
        //    }
        //    //mTransaction tran = new mTransaction();
        //    //Transactions = tran.transaction_query(searchkey, startdate, enddate);

        //    return View(Transactions);
        //}

        public PartialViewResult TransactionQueryResult(string searchkey, string startdate, string enddate)
        {
            
            List<mTransaction> Transactions = new List<mTransaction>();

            if (HttpContext.Session["UID"] == null)
            {
                TempData["ErrorMessage"] = "Session Error";
                TempData.Keep();
                return PartialView(Transactions);
            }
            
            try
            {
                DateTime sDate = Convert.ToDateTime(startdate);
                DateTime eDate = Convert.ToDateTime(enddate);
                mTransaction T = new mTransaction();
                Transactions = T.transaction_query(searchkey, sDate, eDate);

                return PartialView(Transactions);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData.Keep();
                return PartialView(Transactions);
            }
           
            //mTransaction tran = new mTransaction();
            //Transactions = tran.transaction_query(searchkey, startdate, enddate);
        }

        public ActionResult TransactionQueryExport(string searchkey, string startdate, string enddate)
        {
            if (HttpContext.Session["UID"] != null)
            {
                return TransactionExportToExcel(searchkey,startdate, enddate);
            }
            else
            {
                return RedirectToAction("Index", "Portal", null);
            }
        }

        internal FileResult TransactionExportToExcel(string searchkey, string startdate, string enddate)
        {
            if (HttpContext.Session["UID"] != null && startdate != null && enddate != null)
            {
                List<mTransaction> srl = new List<mTransaction>();
                mTransaction sr = new mTransaction();

                DateTime sdate = Convert.ToDateTime(startdate);
                DateTime edate = Convert.ToDateTime(enddate);
                srl = sr.transaction_query(searchkey,sdate,edate);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("TransactionQuery");
                Sheet.Cells["A1"].Value = "TrxDate";
                Sheet.Cells["B1"].Value = "MID";
                Sheet.Cells["C1"].Value = "Merchant Name";
                Sheet.Cells["D1"].Value = "Card No";
                Sheet.Cells["E1"].Value = "Card Type";
                Sheet.Cells["F1"].Value = "MDR Rate";
                Sheet.Cells["G1"].Value = "Tran Amount";
                Sheet.Cells["H1"].Value = "MDR Value";
                Sheet.Cells["I1"].Value = "Settlement Amount";
                int row = 2;
                foreach (mTransaction item in srl)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.TrxDate.Value.Date.ToString("MM/dd/yyyy");
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.MPU_Merchant_ID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Merchant_Name;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.CardNo;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.CardType;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.TRXMDRRate;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.TrxAmount;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.MDRValue;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.SettAmount;
                    row++;
                }

                using (ExcelRange rng = Sheet.Cells[string.Format("F{0}:H{1}", 2, row + 1)])
                {
                    rng.Style.Numberformat.Format = "###,###,##0.00";
                }

                Sheet.Cells[string.Format("A{0}:F{1}", row, row)].Merge = true;
                Sheet.Cells[string.Format("A{0}", row)].Value = "Total";
                Sheet.Cells[string.Format("G{0}", row)].Value = srl.Sum(i => i.TrxAmount);
                Sheet.Cells[string.Format("H{0}", row)].Value = srl.Sum(i => i.MDRValue);
                Sheet.Cells[string.Format("I{0}", row)].Value = srl.Sum(i => i.SettAmount);

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();

                var FileByteArray = Ep.GetAsByteArray();

                return File(FileByteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sdate.ToString("MM/dd/yyyy") +"-"+ edate.ToString("MM/dd/yyyy") + "_TransactionQuery.xlsx");
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment: filename=" +trxdate.ToString("MM/dd/yyyy")+chkType+ "_SettlementReport.xlsx");
                //Response.BinaryWrite(Ep.GetAsByteArray());
                //Response.End();
            }
            else
            {
                return File(new byte[0], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        public ActionResult SettlementReport()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            

            return View();
        }

        public PartialViewResult Settlement()
        {
            if (HttpContext.Session["UID"] == null)
            {
                TempData["ErrorMessage"] = "Session Error";
                TempData.Keep();
            }
            return PartialView(new List<mSettlementReport>());
        }
        [HttpPost]
        public PartialViewResult Settlement(DateTime trxdate,string chkType)
        {
            string s = trxdate.Date.ToShortDateString();

            List<mSettlementReport> srl = new List<mSettlementReport>();

            if (HttpContext.Session["UID"] == null)
            {
                TempData["ErrorMessage"] = "Session Error";
                TempData.Keep();
                return PartialView(srl);
            }

            try
            {
                mSettlementReport sr = new mSettlementReport();
                srl = sr.Retrieve_Settlement_Report(chkType, trxdate);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData.Keep();
            }

            return PartialView(srl);
        }

        public ActionResult SettlementReportExportToExcel(DateTime trxdate,string chkType)
        {
            if(HttpContext.Session["UID"]!=null)
            {
                return SettlementExportExcel(trxdate, chkType);
            }
            else
            {
                return RedirectToAction("Index", "Portal", null);
            }
        }

        internal FileResult SettlementExportExcel(DateTime trxdate, string chkType)
        {
            if (HttpContext.Session["UID"] != null && trxdate!=null && chkType!=null)
            {
                List<mSettlementReport> srl = new List<mSettlementReport>();
                mSettlementReport sr = new mSettlementReport();
                srl = sr.Retrieve_Settlement_Report(chkType, trxdate);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("SettlementReport");
                Sheet.Cells["A1"].Value = "TrxDate";
                Sheet.Cells["B1"].Value = "MID";
                Sheet.Cells["C1"].Value = "Account Name";
                Sheet.Cells["D1"].Value = "Merchant Name";
                Sheet.Cells["E1"].Value = "Settlement A/C";
                Sheet.Cells["F1"].Value = "Total Trans";
                Sheet.Cells["G1"].Value = "Tran Amount";
                Sheet.Cells["H1"].Value = "MDR Value";
                Sheet.Cells["I1"].Value = "Settlement Amount";
                int row = 2;
                foreach (var item in srl)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.TrxDate.Value.Date.ToString("MM/dd/yyyy");
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.MPU_Merchant_ID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.SettAccountName;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Merchant_Name;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Settlement_Acc;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.TotalTrans;
                    Sheet.Cells[string.Format("G{0}", row)].Value =  item.TrxAmount;
                    
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.MDRValue;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.SettAmount;
                    row++;
                }

                using (ExcelRange rng = Sheet.Cells[string.Format("G{0}:I{1}", 2, row+1)])
                {
                    rng.Style.Numberformat.Format = "###,###,##0.00";
                }

                Sheet.Cells[string.Format("A{0}:E{1}", row, row)].Merge = true;
                Sheet.Cells[string.Format("A{0}", row)].Value = "Total";
                Sheet.Cells[string.Format("F{0}", row)].Value = srl.Sum(i => i.TotalTrans);
                Sheet.Cells[string.Format("G{0}", row)].Value = srl.Sum(i => i.TrxAmount);
                Sheet.Cells[string.Format("H{0}", row)].Value = srl.Sum(i => i.MDRValue);
                Sheet.Cells[string.Format("I{0}", row)].Value = srl.Sum(i => i.SettAmount);

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();

                var FileByteArray = Ep.GetAsByteArray();

                return File(FileByteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", trxdate.ToString("MM/dd/yyyy") + chkType + "_SettlementReport.xlsx");
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment: filename=" +trxdate.ToString("MM/dd/yyyy")+chkType+ "_SettlementReport.xlsx");
                //Response.BinaryWrite(Ep.GetAsByteArray());
                //Response.End();
            }
            else
            {
                return File(new byte[0], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}