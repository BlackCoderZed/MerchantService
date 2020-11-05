using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MerchantService.Models;

namespace MerchantService.Controllers
{
    public class JCBController : Controller
    {
        // GET: JCB
        public ActionResult Index()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }
            mUser u = new mUser();
            if(!u.check_user_right(Session["UID"].ToString(),2))
            {
                return RedirectToAction("Denied", "Portal", null);
            }
            return View();
        }

        bool Global_Error = false;
        //string ChkType = "inc";

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile, string chkType)
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
            //
            ViewBag.ChkType = chkType;
            //Transaction List to get return value and return to view
            List<mTransaction> Tran_List = new List<mTransaction>();

            if (postedFile != null)
            {
                //Set FileName For View
                ViewBag.FileName = postedFile.FileName;

                //Get Filename , upload to server's Temp dir
                var filename = Path.GetFileName(postedFile.FileName);
                var filepath = Path.Combine(Server.MapPath("~/Temp"), filename);
                if (!Directory.Exists(Server.MapPath("~/Temp")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Temp"));
                }
                postedFile.SaveAs(filepath);


                //determine inc or bcom (old file or new file)
                if (chkType == "inc")
                {
                    //Get return value from Function
                    Tran_List = ReadJCB_INC(filepath);
                }
                else if (chkType == "bcom")
                {

                }
            }

            if (Global_Error == true)
            {
                //Clear Readed data not to pass to view if there is error. Otherwise user can click submit.
                Tran_List = new List<mTransaction>();
            }
            else
            {
                //Insert data to database
                foreach (mTransaction t in Tran_List)
                {
                    bool isSuccess = t.InsertTran();

                    if (!isSuccess)
                    {
                        TempData["TRX_Message"] = "Error on inserting";
                        TempData.Keep();
                        //break;
                    }
                    else
                    {
                        TempData["TRX_Message"] = "Successfully Inserted";
                    }
                }
            }

            return View();
        }

        internal List<mTransaction> ReadJCB_INC(string filepath)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return new List<mTransaction>();
            }

            List<mTransaction> Tran_List = new List<mTransaction>();
            string error_Lines = "Terminated!!!\nError : ";
            try
            {
                if (!string.IsNullOrEmpty(filepath))
                {
                    //Get Trx Date
                    string datestr = Path.GetFileName(filepath);

                    string year = "20" + datestr.Substring(3, 2);
                    string month = datestr.Substring(5, 2);
                    string day = datestr.Substring(7, 2);

                    DateTime TRXDate = DateTime.ParseExact(year + "-" + month + "-" + day, "yyyy-MM-dd", null);

                    //Read File
                    string[] lines = System.IO.File.ReadAllLines(filepath);

                    foreach (string line in lines)
                    {
                        if (line.Substring(0, 3) == "100")
                        {
                            mTransaction tran = new mTransaction();
                            tran.CardNo = line.Substring(3, 19).Replace(" ", "");
                            //To Get Card Type
                            mCardType cardType = new mCardType();
                            cardType.BINCode = tran.CardNo.Substring(0, 4);
                            tran.CardType = cardType.get_card_type();

                            tran.ProcessingCode = line.Substring(22, 6);
                            Double decplace_scr = 1;
                            int dec_place = int.Parse(line.Substring(52, 1));
                            for (int i = 0; i < dec_place; i++)
                            {
                                decplace_scr = decplace_scr * 10;
                            }

                            tran.SettConversationRate = Convert.ToDouble(line.Substring(53, 9)) / decplace_scr;
                            tran.CurrencyCode = line.Substring(62, 3);
                            tran.SettCurrencyCode = line.Substring(65, 3);
                            tran.TerminalID = line.Substring(132, 8);

                            //This is from raw file
                            //tran.TrxID = 0001;
                            tran.TrxAmount = Convert.ToDouble(line.Substring(28, 12)) / 100;
                            tran.MPU_Merchant_ID = line.Substring(140, 15);

                            double rate = 0;

                            if (tran.MPU_Merchant_ID.Substring(0, 3) != "202")
                            {
                                //To get mdr rate from db
                                mMerchant merchant = new mMerchant();
                                merchant.MPU_Merchant_ID = tran.MPU_Merchant_ID;
                                rate = merchant.get_mdr_rate_jcbnotonus(merchant);
                                //rate = getMDRrate(mid, "MPU");

                                if (rate == -123)
                                {
                                    // rate -123 means that cannot get merchant's MDR rate or merchant info does not exist
                                    // so we need to block transaction to upload and show warning
                                    Global_Error = true;
                                    error_Lines += merchant.MPU_Merchant_ID + ",";
                                    TempData["MerchantInfoError"] = error_Lines;
                                    TempData.Keep();
                                }

                                tran.TRXMDRRate = rate;
                                tran.MDRValue = (tran.TrxAmount * rate) / 100;
                                tran.SettAmount = tran.TrxAmount - tran.MDRValue;
                                tran.TrxDate = TRXDate;
                                tran.TRXRemark = Session["UID"].ToString();
                                tran.RecordType = "JCB_Not_ONUS";
                                Tran_List.Add(tran);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                TempData["MerchantInfoError"] = ex.Message;
                TempData.Keep();
            }

            
            return Tran_List;
        }

    }
}