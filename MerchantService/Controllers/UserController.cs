using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MerchantService.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace MerchantService.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(string keywordsearch)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }
            List<mUser> users = new List<mUser>();
            using (Models.MerchantService db = new Models.MerchantService())
            {
                List<v_UserInfo> userinfos = db.v_UserInfo.Where(x => x.StaffName.Contains(keywordsearch) || keywordsearch == null).ToList();
                users = userinfos.Select(x => new mUser { StaffID = x.StaffID, StaffName = x.StaffName, DepartmentID = x.DepartmentID, Department = x.Department, OfficeID = x.OfficeID, Office = x.Office, UserID = x.UserID, UserLevel = x.UserLevel, AdminLogin = x.AdminLogin }).ToList();
            }

            return View(users);
        }

        public ActionResult Create()
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            mUser User = new mUser();
            
            return View(User);
        }

        [HttpPost]
        public ActionResult Create(mUser User)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            try
            {
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    t_Users user = new t_Users();
                    user.StaffID = User.StaffID;
                    user.UserID = User.UserID;
                    user.Passwd = mUser.EncryptPassword(User.Passwd);
                    user.UserLevel = User.UserLevel;
                    user.AdminLogin = User.AdminLogin;

                    db.t_Users.Add(user);
                    db.SaveChanges();

                    TempData["StatusMessage"] = "Success";
                    TempData.Keep();
                }
            }
            catch(Exception ex)
            {
                TempData["StatusMessage"] = ex.Message;
                TempData.Keep();
            }
            return View("Create");
        }

        public bool Check(string staffid,string uid)
        {
            bool isPass = false;
            using (Models.MerchantService db = new Models.MerchantService())
            {
                t_Users u = new t_Users();
                u = db.t_Users.Where(x => x.UserID == uid).SingleOrDefault();

                if(u!=null)
                {
                    isPass = false;
                }
                else
                {
                    isPass = true;
                }
            }
                return isPass;
        }

        public ActionResult Reset(string uid)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            mUser User = new mUser();

            try
            {
                using (Models.MerchantService db = new Models.MerchantService())
                {
                    t_Users u = new t_Users();
                    u = db.t_Users.Where(x => x.UserID == uid).SingleOrDefault();
                    User.UserID = u.UserID;
                    //User.Passwd = u.Passwd;
                }
            }
            catch(Exception ex)
            {
                TempData["StatusMessage"] = ex.Message;
                TempData.Keep();
            }
            return View(User);
        }

        [HttpPost]
        public ActionResult Reset(mUser user)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            try
            {
                using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
                {
                    consql.Open();
                    SqlCommand cmd = new SqlCommand("sp_u_reset_passwd", consql);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);
                    cmd.Parameters.AddWithValue("@Passwd", mUser.EncryptPassword(user.Passwd));
                    cmd.ExecuteNonQuery();

                    TempData["StatusMessage"] = "Success";
                    TempData.Keep();
                }
            }
            catch(Exception ex)
            {
                TempData["StatusMessage"] = ex.Message;
                TempData.Keep();
            }
                return View();
        }

        public ActionResult Edit(string uid)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            mUser user = new mUser();

            using (Models.MerchantService db = new Models.MerchantService())
            {
                t_Users u = new t_Users();
                u = db.t_Users.Where(x => x.UserID == uid).SingleOrDefault();

                user.UserID = u.UserID;
                user.AdminLogin = u.AdminLogin;
                user.UserLevel = u.UserLevel;
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(mUser user)
        {
            if (HttpContext.Session["AID"] == null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            try
            {
                using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
                {
                    consql.Open();
                    SqlCommand cmd = new SqlCommand("sp_u_update_user_right", consql);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);
                    cmd.Parameters.AddWithValue("@AdminLogin", user.AdminLogin);
                    cmd.Parameters.AddWithValue("@EnabledUser", user.UserLevel);
                    cmd.ExecuteNonQuery();

                    TempData["StatusMessage"] = "Success";
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = ex.Message;
                TempData.Keep();
            }

            return View();
        }
    }  
}