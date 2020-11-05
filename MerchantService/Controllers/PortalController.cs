using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MerchantService.Models;

namespace MerchantService.Controllers
{
    public class PortalController : Controller
    {
        // GET: Portal
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminLogin()
        {
            return View();
        }
        

        public ActionResult Dashboard()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }
            return View();
        }

        public ActionResult AdminDashboard()
        {
            if(HttpContext.Session["AID"]==null)
            {
                return RedirectToAction("AdminLogin", "Portal", null);
            }

            return View();
        }

        public ActionResult UserLogin(mUser user)
        {
            try
            {
                mUser muser = new mUser();
                bool is_success = muser.UserLogin(user);
                if (is_success)
                {
                    HttpContext.Session["UID"] = user.UserID;
                    //Session["UID"] = user.UserID;
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["Message"] = "Invalid Login";
                    TempData.Keep();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Index");
            }
        }

        public ActionResult LoginToAdmin(mUser user)
        {
            try
            {
                mUser muser = new mUser();
                muser.UserID = user.UserID;
                muser.Passwd = user.Passwd;

                bool is_success = muser.AdminUserLogin();
                if (is_success)
                {
                    HttpContext.Session["AID"] = user.UserID;
                    HttpContext.Session["UID"] = user.UserID;
                    //Session["UID"] = user.UserID;
                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    TempData["Message"] = "Invalid Login";
                    TempData.Keep();
                    return RedirectToAction("AdminLogin");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Logout()
        {
            return RedirectToActionPermanent("Index", "Portal", null);
        }

        public ActionResult AdminLogout()
        {
            return RedirectToActionPermanent("AdminLogin", "Portal", null);
        }

        //Get
        public ActionResult ChangePassword()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldpasswd,string newpasswd,string confirmpasswd)
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            mUser user = new mUser();
            user.UserID = Session["UID"].ToString();
            user.Passwd = oldpasswd;

            if(newpasswd==confirmpasswd)
            {
                bool isRightUser = user.CheckPasswd();

                if (isRightUser)
                {
                    bool isSuccess=user.ChangePassword(newpasswd); 
                    
                    if(isSuccess)
                    {
                        TempData["PasswdChange"] = "Password had been changed.";
                        TempData.Keep();
                    }
                    else
                    {
                        TempData["PasswdError"] = "Something went wrong!!!";
                        TempData.Keep();
                    }  
                }
                else
                {
                    TempData["PasswdError"] = "Invalid Old Password";
                    TempData.Keep();
                }
            }
            else
            {
                TempData["PasswdError"] = "Invalid New Password";
                TempData.Keep();
            }
            
            return View();
        }

        public ActionResult Denied()
        {
            if (HttpContext.Session["UID"] == null)
            {
                return RedirectToAction("Index", "Portal", null);
            }

            return View();
        }
        
    }
}