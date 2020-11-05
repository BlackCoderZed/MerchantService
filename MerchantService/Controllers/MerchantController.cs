using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MerchantService.Models;
using OfficeOpenXml;
using System.Drawing;
using System.Data.SqlClient;

namespace MerchantService.Controllers
{
    public class MerchantController : Controller
    {
        // GET: Merchant
        public ActionResult Index(string keyworksearch)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            List<v_Merchant> merchant_list = new List<v_Merchant>();
            using (MerchantService.Models.MerchantService db = new Models.MerchantService())
            {
                merchant_list = db.v_Merchant.Where(x => x.Merchant_Name.Contains(keyworksearch) || keyworksearch == null).ToList();
            }
            return View(merchant_list);
        }
        
        public ActionResult CreateMerchant()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Merchant", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }
            //Business Category for dropdown
            List<t_BusinessCategory> category_list = new List<t_BusinessCategory>();
            using (Models.MerchantService db = new Models.MerchantService())
            {
                category_list = db.t_BusinessCategory.ToList();
            }
            ViewBag.Category = category_list;

            return View();
        }
        
        public ActionResult AddMerchant(mMerchant model)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            try
            {
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    t_Merchant_Info merchant = new t_Merchant_Info();
                    merchant.MPU_Merchant_ID = model.MPU_Merchant_ID;
                    merchant.Merchant_Name = model.Merchant_Name;
                    merchant.Merchant_Phone = model.Merchant_Phone;
                    merchant.Merchant_Email = model.Merchant_Email;
                    merchant.Merchant_Address = model.Merchant_Address;
                    merchant.OMPURate = model.OMPURate;
                    merchant.OJCBRate = model.OJCBRate;
                    merchant.OUPIRate = model.OUPIRate;
                    merchant.MPURate = model.MPURate;
                    merchant.JCBRate = model.JCBRate;
                    merchant.UPIRate = model.UPIRate;
                    merchant.Settlement_Acc = model.Settlement_Acc;
                    merchant.SettAccountName = model.SettAccountName;
                    merchant.BusinessCategoryID = model.BusinessCategoryID;
                    merchant.MStatusID = 100;
                    merchant.CreationDate = DateTime.Now.Date;
                    db.t_Merchant_Info.Add(merchant);
                    db.SaveChanges();

                    TempData["Message"] ="Saved";
                    TempData.Keep();
                }
            }
            catch(Exception ex)
            {
                TempData["Message"] = ex.InnerException.InnerException.Message;
                TempData.Keep();
            }

            return RedirectToAction("CreateMerchant");
        }

        public ActionResult UpdateMerchantInfo(string id)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            if (id==null)
            {
                return new HttpStatusCodeResult(statusCode: 400);
            }
            //Business Category for dropdown
            List<t_BusinessCategory> category_list = new List<t_BusinessCategory>();
            using (Models.MerchantService db = new Models.MerchantService())
            {
                category_list = db.t_BusinessCategory.ToList();
            }
            ViewBag.Category = category_list;

            //Get Merchant Info
            mMerchant merchantinfo = new mMerchant();
            try
            {
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    t_Merchant_Info merch_info = db.t_Merchant_Info.Where(x => x.MPU_Merchant_ID == id).FirstOrDefault();
                    merchantinfo.MPU_Merchant_ID = merch_info.MPU_Merchant_ID;
                    merchantinfo.Merchant_Name = merch_info.Merchant_Name;
                    merchantinfo.Merchant_Address = merch_info.Merchant_Address;
                    merchantinfo.Merchant_Email = merch_info.Merchant_Email;
                    merchantinfo.Merchant_Phone = merch_info.Merchant_Phone;
                    merchantinfo.OMPURate = merch_info.OMPURate;
                    merchantinfo.OJCBRate = merch_info.OJCBRate;
                    merchantinfo.OUPIRate = merch_info.OUPIRate;
                    merchantinfo.MPURate = merch_info.MPURate;
                    merchantinfo.JCBRate = merch_info.JCBRate;
                    merchantinfo.UPIRate = merch_info.UPIRate;
                    merchantinfo.Settlement_Acc = merch_info.Settlement_Acc;
                    merchantinfo.SettAccountName = merch_info.SettAccountName;
                    merchantinfo.BusinessCategoryID = merch_info.BusinessCategoryID;
                }

                return View(merchantinfo);
            }
            catch(Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return View();
            }
        }

        public ActionResult UpdateMerchant(mMerchant model)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            try
            {
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    //Call Update Method
                    mMerchant merchant = new mMerchant();
                    merchant.UpdateMerchant(model);

                    TempData["Message"] = "Success";
                    TempData.Keep();
                }
            }
            catch(Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData.Keep();
            }

            //Business Category for dropdown
            List<t_BusinessCategory> category_list = new List<t_BusinessCategory>();
            using (Models.MerchantService db = new Models.MerchantService())
            {
                category_list = db.t_BusinessCategory.ToList();
            }
            ViewBag.Category = category_list;

            return RedirectToAction("UpdateMerchantInfo", new { id = model.MPU_Merchant_ID });
        }

        public ActionResult ViewDetails(string merchantid)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            mMerchant merchant = new mMerchant();
            if (merchantid!=null)
            {
                try
                {
                    using (Models.MerchantService db = new Models.MerchantService())
                    {
                        v_Merchant merchant_info = new v_Merchant();
                        merchant_info = db.v_Merchant.Where(x => x.MPU_Merchant_ID == merchantid).SingleOrDefault();
                        merchant.MPU_Merchant_ID = merchant_info.MPU_Merchant_ID;
                        merchant.Merchant_Name = merchant_info.Merchant_Name;
                        merchant.Merchant_Address = merchant_info.Merchant_Address;
                        merchant.Merchant_Email = merchant_info.Merchant_Email;
                        merchant.Merchant_Phone = merchant_info.Merchant_Phone;
                        merchant.OMPURate = merchant_info.OMPURate;
                        merchant.OJCBRate = merchant_info.OJCBRate;
                        merchant.OUPIRate = merchant_info.OUPIRate;
                        merchant.MPURate = merchant_info.MPURate;
                        merchant.JCBRate = merchant_info.JCBRate;
                        merchant.UPIRate = merchant_info.UPIRate;
                        merchant.Settlement_Acc = merchant_info.Settlement_Acc;
                        merchant.SettAccountName = merchant_info.SettAccountName;
                        merchant.BusinessCategoryID = merchant_info.BusinessCategoryID;
                        merchant.BusinessCategory = merchant_info.BusinessCategory;
                    }
                }
                catch(Exception ex)
                {
                    TempData["Message"] = ex.Message;
                    TempData.Keep();
                }
                
            }

            return View(merchant);
        }

        public ActionResult ExportMerchant()
        {
            if(HttpContext.Session["UID"]!=null)
            {
                mUser u = new mUser();
                if (!u.check_user_right(Session["UID"].ToString(), 1))
                {
                    return RedirectToAction("Denied", "Portal", null);
                }
                return ExportExcel();
            }
            else
            {
                return RedirectToAction("Index", "Portal", null);
            }
        }

        internal FileResult ExportExcel()
        {
            if (HttpContext.Session["UID"] != null)
            {
                List<v_Merchant> merchant_list = new List<v_Merchant>();
                using (MerchantService.Models.MerchantService db = new Models.MerchantService())
                {
                    merchant_list = db.v_Merchant.ToList();
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
                Sheet.Cells["A1"].Value = "MID";
                Sheet.Cells["B1"].Value = "Merchant";
                Sheet.Cells["C1"].Value = "Phone";
                Sheet.Cells["D1"].Value = "Email";
                Sheet.Cells["E1"].Value = "Address";
                Sheet.Cells["F1"].Value = "MPU Rate";
                Sheet.Cells["G1"].Value = "UPI Rate";
                Sheet.Cells["H1"].Value = "JCB Rate";
                Sheet.Cells["I1"].Value = "MPU(on us) Rate";
                Sheet.Cells["J1"].Value = "UPI(on us) Rate";
                Sheet.Cells["K1"].Value = "JCB(on us) Rate";
                Sheet.Cells["L1"].Value = "Settlement Account";
                Sheet.Cells["M1"].Value = "Account Name";
                int row = 2;
                foreach (var item in merchant_list)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.MPU_Merchant_ID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Merchant_Name;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Merchant_Phone;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Merchant_Email;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Merchant_Address;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.MPURate;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.UPIRate;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.JCBRate;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.OMPURate;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.OUPIRate;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.OJCBRate;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Settlement_Acc;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.SettAccountName;
                    row++;
                }


                Sheet.Cells["A:AZ"].AutoFitColumns();
                //Response.Clear();
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment: filename=" + "MerchantList.xlsx");
                //Response.BinaryWrite(Ep.GetAsByteArray());
                //Response.End();

                var FileByteArray = Ep.GetAsByteArray();

                return File(FileByteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MerchantList.xlsx");
            }
            else
            {
                return File(new Byte[0], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MerchantList.xlsx");
            }
        }
        public ActionResult Termination()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }
            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }
            return View();
        }
        public ActionResult RetrieveMerchant(string mid)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return Json(new {success=false, msg = "Session Error" }, JsonRequestBehavior.AllowGet);
            }

            mUser u = new mUser();
            if (!u.check_user_right(Session["UID"].ToString(), 1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            return RetrieveMerchantInfo(mid);
        }
        internal JsonResult RetrieveMerchantInfo(string mid)
        {
            v_Merchant m = new v_Merchant();

            using (MerchantService.Models.MerchantService db = new Models.MerchantService())
            {
                m = db.v_Merchant.Where(x => x.MPU_Merchant_ID == mid && x.MStatusID == 100).SingleOrDefault();

            }
            
            return Json(new {success=true,merchant=m }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TerminateMerchant(string mid)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return Json(new { success = true, msg = "Session Error" }, JsonRequestBehavior.AllowGet);
            }

            mUser u = new mUser();
            if(!u.check_user_right(Session["UID"].ToString(),1))
            {
                return RedirectToAction("Denied", "Portal", null);
            }

            return Terminate(mid);
        }
        internal JsonResult Terminate(string mid)
        {
            string msg = null;
            var data = Json(new { });
            try
            {
                mMerchant m = new mMerchant();
                m.MPU_Merchant_ID = mid;
                m.TerminateMerchant(HttpContext.Session["UID"].ToString());
                msg = "Terminated";
                data = Json(new { success = true, msg = msg }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                data = Json(new { success = true, msg = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
            return data;
        }

    }
}