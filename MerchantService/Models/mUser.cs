using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MerchantService.Models
{
    public class mUser:mStaff
    {
        public string UserID { get; set; }
        public string Passwd { get; set; }
        public Nullable<int> UserLevel { get; set; }
        public Nullable<int> AdminLogin { get; set; }

        public static string EncryptPassword(string str)
        {
            var passwdStr = new StringBuilder();
            SHA512Managed sha = new SHA512Managed();
            byte[] data = sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(str));

            foreach (var d in data)
            {
                passwdStr.Append(d.ToString("x2"));
            }

            return passwdStr.ToString();
        }

        public bool UserLogin(mUser user)
        {
            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();

                SqlCommand cmd = new SqlCommand("sp_u_login", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", user.UserID);
                cmd.Parameters.AddWithValue("@Passwd", EncryptPassword(user.Passwd));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool AdminUserLogin()
        {
            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();

                SqlCommand cmd = new SqlCommand("sp_a_login", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Passwd", EncryptPassword(Passwd));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CheckPasswd()
        {
            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();

                SqlCommand cmd = new SqlCommand("sp_u_login", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Passwd", EncryptPassword(Passwd));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ChangePassword(string NewPasswd)
        {
            bool Is_Success = false;

            try
            {
                using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
                {
                    consql.Open();
                    SqlCommand cmd = new SqlCommand("sp_u_change_passwd", consql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UID", UserID);
                    cmd.Parameters.AddWithValue("@Passwd",EncryptPassword(NewPasswd));
                    cmd.ExecuteNonQuery();

                    consql.Close();
                    Is_Success = true;
                }
            }
            catch
            {
                Is_Success = false;
            }

            return Is_Success;
        }

        public bool check_user_right(string username,int requiredRight)
        {
            bool isSuccess = false;

            using (Models.MerchantService db = new MerchantService())
            {
                t_Users u = db.t_Users.Where(x => x.UserID == username).SingleOrDefault();

                mUser mU = new mUser();
                mU.UserID = u.UserID;
                mU.UserLevel = u.UserLevel;

                if(requiredRight==1)
                {
                    if(mU.UserLevel==1 || mU.UserLevel==3)
                    {
                        isSuccess = true;
                    }
                }
                if(requiredRight==2)
                {
                    if(mU.UserLevel==2||mU.UserLevel==3)
                    {
                        isSuccess = true;
                    }
                }
            }

                return isSuccess;
        }

    }
    
}