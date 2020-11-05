using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MerchantService.Models;

namespace MerchantService.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult Index()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 2))
            {
                return RedirectToAction("Denied", "Portal", null);
            }
            return View();
        }

        
        
        public PartialViewResult TRX_PartialView(string trxdate,string TrxType)
        {
            
            List<mTransaction> trx_list = new List<mTransaction>();
            ViewBag.TrxType = TrxType;
            ViewBag.TrxDate = trxdate;
            if (HttpContext.Session["UID"] == null)
            {
                TempData["ErrorMessage"] = "Session Error";
                TempData.Keep();
                return PartialView(trx_list);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 2))
            {
                TempData["ErrorMessage"] = "Access Denied";
                TempData.Keep();
                return PartialView(trx_list);
            }

            try
            {
                DateTime trxDate = Convert.ToDateTime(trxdate);
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    List<t_Transactions> ts = new List<t_Transactions>();
                    ts = db.t_Transactions.Where(x => x.TrxDate == trxDate && x.RecordType== TrxType).ToList();
                    trx_list = ts.Select(x => new mTransaction { TrxID = x.TrxID, TrxDate = x.TrxDate, MPU_Merchant_ID = x.MPU_Merchant_ID,CardNo=x.CardNo,CardType=x.CardType,TRXMDRRate=x.TRXMDRRate,TrxAmount=x.TrxAmount,SettAmount=x.SettAmount,ProcessingCode=x.ProcessingCode,TerminalID=x.TerminalID,RecordType=x.RecordType }).ToList();
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData.Keep();
            }
            return PartialView(trx_list);
        }

        public ActionResult Delete(string[] trxid)
        {
            var data =new {status="",message="" };
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 2))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            if (trxid==null)
            {
                data = new { status = "ok", message = "Invalid TRXID" };
            }

            try
            {
                ViewBag.TRXId = trxid;
                string uid = Session["UID"].ToString();
                for(int i=0;i<trxid.Length;i++)
                {
                    string tx = trxid[i];
                    mTransaction t = new mTransaction();
                    t.TrxID = int.Parse(tx);
                    t.Delete(uid);
                }
                
                var message= String.Format("No of Transactions : {0} had been deleted...",trxid.Length);
                data = new { status = "ok", message = message };
            }
            catch(Exception ex)
            {
                data = new { status = "ok", message = "Transaction List is empty or\n"+ex.Message };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
            
        }
    }
}