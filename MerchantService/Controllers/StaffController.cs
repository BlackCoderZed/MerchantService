using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MerchantService.Models;

namespace MerchantService.Controllers
{
    public class StaffController : Controller
    {
        // GET: Staff
        public ActionResult Index(string keywordsearch)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            List<mStaff> StaffList = new List<mStaff>();

            using (MerchantService.Models.MerchantService db = new Models.MerchantService())
            {
                List<v_Staff> Staffs = db.v_Staff.Where(x => x.StaffName.Contains(keywordsearch) || keywordsearch == null).ToList();
                StaffList = Staffs.Select(x => new mStaff { StaffID = x.StaffID,StaffName=x.StaffName,DepartmentID=x.DepartmentID,Department=x.Department,OfficeID=x.OfficeID,Office=x.Office }).ToList();
            }

            return View(StaffList);
        }

        public ActionResult AddStaff()
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            using (MerchantService.Models.MerchantService db = new Models.MerchantService())
            {
                List<t_Department> departments = new List<t_Department>();
                departments = db.t_Department.ToList();

                List<t_Office> Offices = new List<t_Office>();
                Offices = db.t_Office.ToList();

                ViewBag.DepartmentList = departments;
                ViewBag.OfficeList = Offices;
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddStaff(mStaff Staff)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            using (MerchantService.Models.MerchantService db = new Models.MerchantService())
            {
                //Insert To Table
                try
                {
                    t_Staff Tstaff = new t_Staff();
                    Tstaff.StaffName = Staff.StaffName;
                    Tstaff.DepartmentID = Staff.DepartmentID;
                    Tstaff.OfficeID = Staff.OfficeID;

                    db.t_Staff.Add(Tstaff);
                    db.SaveChanges();
                    TempData["StatusMessage"] = "Success";
                    TempData.Keep();
                }
                catch(Exception ex)
                {
                    TempData["StatusMessage"] = ex.Message;
                    TempData.Keep();
                }

                //Load data for dropdown list
                List<t_Department> departments = new List<t_Department>();
                departments = db.t_Department.ToList();

                List<t_Office> Offices = new List<t_Office>();
                Offices = db.t_Office.ToList();

                ViewBag.DepartmentList = departments;
                ViewBag.OfficeList = Offices;
            }
            return View();
        }

        public ActionResult Update(string staffid)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            mStaff Mstaff = new mStaff();
            if(staffid==null)
            {
                return new HttpStatusCodeResult(statusCode: 400);
            }

            int staff_id = int.Parse(staffid);
            try
            {
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    //Load data for dropdown list
                    List<t_Department> departments = new List<t_Department>();
                    departments = db.t_Department.ToList();

                    List<t_Office> Offices = new List<t_Office>();
                    Offices = db.t_Office.ToList();

                    ViewBag.DepartmentList = departments;
                    ViewBag.OfficeList = Offices;


                    t_Staff staff = db.t_Staff.Where(x => x.StaffID == staff_id).SingleOrDefault();
                    Mstaff.StaffID = staff.StaffID;
                    Mstaff.StaffName = staff.StaffName;
                    Mstaff.DepartmentID = staff.DepartmentID;
                    Mstaff.OfficeID = staff.OfficeID;
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData.Keep();
            }
            return View(Mstaff);
        }

        [HttpPost]
        public ActionResult Update(mStaff Staff)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            mStaff Mstaff = new mStaff();
            try
            {
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    //Load data for dropdown list
                    List<t_Department> departments = new List<t_Department>();
                    departments = db.t_Department.ToList();

                    List<t_Office> Offices = new List<t_Office>();
                    Offices = db.t_Office.ToList();

                    ViewBag.DepartmentList = departments;
                    ViewBag.OfficeList = Offices;

                    //update info call procedure
                    bool is_success=Staff.UpdateStaffInfo();
                    if(is_success)
                    {
                        TempData["StatusMessage"] = "Success";
                    }
                    else
                    {
                        TempData["StatusMessage"] = "Error";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData.Keep();
            }

            return View();
        }

        public ActionResult ViewDetails(string staffid)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            if (staffid==null)
            {
                return new HttpStatusCodeResult(400);
            }

            mStaff s = new mStaff();
            try
            {
                int id = int.Parse(staffid);
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    v_Staff staff = db.v_Staff.Where(x => x.StaffID == id).SingleOrDefault();
                    s.StaffID = staff.StaffID;
                    s.StaffName = staff.StaffName;
                    s.Department = staff.Department;
                    s.Office = staff.Office;
                }
            }
            catch(Exception ex)
            {
                TempData["StatusMessage"] = ex.Message;
            }
            return View(s);
        }

        public ActionResult ChooseStaff()
        {
            if (HttpContext.Session["AID"] == null)
            {
                return new HttpStatusCodeResult(400);
            }
            List<mStaff> s = new List<mStaff>();
            using (Models.MerchantService db = new Models.MerchantService())
            {
                List<v_Staff> staffs = new List<v_Staff>();
                staffs = db.v_Staff.ToList();

                s = staffs.Select(x => new mStaff { StaffID = x.StaffID, StaffName = x.StaffName, Department = x.Department, Office = x.Office }).ToList();
            }
            return View(s);
        }
    }
}