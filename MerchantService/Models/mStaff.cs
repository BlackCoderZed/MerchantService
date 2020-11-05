using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MerchantService.Models
{
    public class mStaff
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public string Department { get; set; }
        public Nullable<int> OfficeID { get; set; }
        public string Office { get; set; }

        public bool UpdateStaffInfo()
        {
            bool isSuccess = false;

            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();

                SqlCommand cmd = new SqlCommand("sp_s_update_staff", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StaffID", StaffID);
                cmd.Parameters.AddWithValue("@StaffName", StaffName);
                cmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                cmd.Parameters.AddWithValue("@OfficeID", OfficeID);

                cmd.ExecuteNonQuery();
                consql.Close();

                isSuccess = true;
            }
            return isSuccess;
        }
    }
}