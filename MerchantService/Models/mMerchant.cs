using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MerchantService.Models
{
    public class mMerchant
    {
        public string MPU_Merchant_ID { get; set; }
        public string Merchant_Name { get; set; }
        public string Merchant_Address { get; set; }
        public string Merchant_Email { get; set; }
        public string Merchant_Phone { get; set; }
        public string Settlement_Acc { get; set; }
        public string SettAccountName { get; set; }
        public Nullable<double> MPURate { get; set; }
        public Nullable<double> JCBRate { get; set; }
        public Nullable<double> UPIRate { get; set; }
        public Nullable<double> OMPURate { get; set; }
        public Nullable<double> OJCBRate { get; set; }
        public Nullable<double> OUPIRate { get; set; }
        public Nullable<int> BusinessCategoryID { get; set; }
        public Nullable<int> MStatusID { get; set; }
        public string MStatus { get; set; }
        public string BusinessCategory { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public void UpdateMerchant(mMerchant merchant)
        {
            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();

                SqlCommand cmd = new SqlCommand("sp_m_update_merchant", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MerchantID", merchant.MPU_Merchant_ID);
                cmd.Parameters.AddWithValue("@Merchant_Name", merchant.Merchant_Name);
                cmd.Parameters.AddWithValue("@Merchant_Address", merchant.Merchant_Address);
                cmd.Parameters.AddWithValue("@Merchant_Email", merchant.Merchant_Email);
                cmd.Parameters.AddWithValue("@Merchant_Phone", merchant.Merchant_Phone);
                cmd.Parameters.AddWithValue("@Settlement_Acc", merchant.Settlement_Acc);
                cmd.Parameters.AddWithValue("@SettAccName", merchant.SettAccountName);
                cmd.Parameters.AddWithValue("@OMPURate", merchant.OMPURate);
                cmd.Parameters.AddWithValue("@OJCBRate", merchant.OJCBRate);
                cmd.Parameters.AddWithValue("@OUPIRate", merchant.OUPIRate);
                cmd.Parameters.AddWithValue("@MPURate", merchant.MPURate);
                cmd.Parameters.AddWithValue("@JCBRate", merchant.JCBRate);
                cmd.Parameters.AddWithValue("@UPIRate", merchant.UPIRate);
                cmd.Parameters.AddWithValue("@BusinessCategoryID", merchant.BusinessCategoryID);

                cmd.ExecuteNonQuery();
            }
        }

        public Double get_mdr_rate_mpunotonus(mMerchant merchant)
        {
            Double MDR = 0;

            using (Models.MerchantService db = new MerchantService())
            {
                t_Merchant_Info merchant_info = new t_Merchant_Info();
                merchant_info = db.t_Merchant_Info.Where(x => x.MPU_Merchant_ID == merchant.MPU_Merchant_ID).SingleOrDefault();
                if (merchant_info != null)
                {
                    MDR = Convert.ToDouble(merchant_info.MPURate);
                }
                else
                {
                    MDR = -123;
                }
            }

            return MDR;
        }

        public Double get_mdr_rate_mpuonus(mMerchant merchant)
        {
            Double MDR = 0;

            using (Models.MerchantService db = new MerchantService())
            {
                t_Merchant_Info merchant_info = new t_Merchant_Info();
                merchant_info = db.t_Merchant_Info.Where(x => x.MPU_Merchant_ID == merchant.MPU_Merchant_ID).SingleOrDefault();
                if (merchant_info != null)
                {
                    MDR = Convert.ToDouble(merchant_info.OMPURate);
                }
                else
                {
                    MDR = -123;
                }
            }

            return MDR;
        }

        public Double get_mdr_rate_jcbnotonus(mMerchant merchant)
        {
            Double MDR = 0;

            using (Models.MerchantService db = new MerchantService())
            {
                t_Merchant_Info merchant_info = new t_Merchant_Info();
                merchant_info = db.t_Merchant_Info.Where(x => x.MPU_Merchant_ID == merchant.MPU_Merchant_ID).SingleOrDefault();
                if (merchant_info != null)
                {
                    MDR = Convert.ToDouble(merchant_info.JCBRate);
                }
                else
                {
                    MDR = -123;
                }
            }

            return MDR;
        }

        public Double get_mdr_rate_jcbonus(mMerchant merchant)
        {
            Double MDR = 0;

            using (Models.MerchantService db = new MerchantService())
            {
                t_Merchant_Info merchant_info = new t_Merchant_Info();
                merchant_info = db.t_Merchant_Info.Where(x => x.MPU_Merchant_ID == merchant.MPU_Merchant_ID).SingleOrDefault();
                if (merchant_info != null)
                {
                    MDR = Convert.ToDouble(merchant_info.OJCBRate);
                }
                else
                {
                    MDR = -123;
                }
            }

            return MDR;
        }

        public Double get_mdr_rate_upinotonus(mMerchant merchant)
        {
            Double MDR = 0;

            using (Models.MerchantService db = new MerchantService())
            {
                t_Merchant_Info merchant_info = new t_Merchant_Info();
                merchant_info = db.t_Merchant_Info.Where(x => x.MPU_Merchant_ID == merchant.MPU_Merchant_ID).SingleOrDefault();
                if (merchant_info != null)
                {
                    MDR = Convert.ToDouble(merchant_info.UPIRate);
                }
                else
                {
                    MDR = -123;
                }
            }

            return MDR;
        }

        public Double get_mdr_rate_upionus(mMerchant merchant)
        {
            Double MDR = 0;

            using (Models.MerchantService db = new MerchantService())
            {
                t_Merchant_Info merchant_info = new t_Merchant_Info();
                merchant_info = db.t_Merchant_Info.Where(x => x.MPU_Merchant_ID == merchant.MPU_Merchant_ID).SingleOrDefault();
                if (merchant_info != null)
                {
                    MDR = Convert.ToDouble(merchant_info.OUPIRate);
                }
                else
                {
                    MDR = -123;
                }
            }

            return MDR;
        }

        public void TerminateMerchant(string userid)
        {
            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();
                SqlCommand cmd = new SqlCommand("sp_m_terminate", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@mid", MPU_Merchant_ID);
                cmd.Parameters.AddWithValue("@uid", userid);
                cmd.ExecuteNonQuery();
                consql.Close();
            }
        }
    }
}