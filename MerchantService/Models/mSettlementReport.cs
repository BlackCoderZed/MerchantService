using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MerchantService.Models
{
    public class mSettlementReport
    {
        public Nullable<System.DateTime> TrxDate { get; set; }
        public string MPU_Merchant_ID { get; set; }
        public string Merchant_Name { get; set; }
        public string Settlement_Acc { get; set; }
        public string SettAccountName { get; set; }
        public int TotalTrans { get; set; }
        public Nullable<double> TrxAmount { get; set; }
        public Nullable<double> MDRValue { get; set; }
        public Nullable<double> SettAmount { get; set; }

        public List<mSettlementReport> Retrieve_Settlement_Report(string RecordType,DateTime TrxDate)
        {
            List<mSettlementReport> srl = new List<mSettlementReport>();

            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();

                SqlCommand cmd = new SqlCommand("sp_r_settlement_rp", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TrxDate", TrxDate);
                cmd.Parameters.AddWithValue("@TrxType", RecordType);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                consql.Close();
                if(dt.Rows.Count>0)
                {
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        mSettlementReport sr = new mSettlementReport();
                        sr.TrxDate = Convert.ToDateTime(dt.Rows[i]["TrxDate"].ToString());
                        sr.MPU_Merchant_ID = dt.Rows[i]["MPU_Merchant_ID"].ToString();
                        sr.Merchant_Name= dt.Rows[i]["Merchant_Name"].ToString();
                        sr.Settlement_Acc= dt.Rows[i]["Settlement_Acc"].ToString();
                        sr.SettAccountName= dt.Rows[i]["SettAccountName"].ToString();
                        sr.TotalTrans= int.Parse(dt.Rows[i]["NoOfTrx"].ToString());
                        sr.TrxAmount = Convert.ToDouble(dt.Rows[i]["TotalTrxAmount"].ToString());
                        sr.MDRValue = Convert.ToDouble(dt.Rows[i]["TotalMDRValue"].ToString());
                        sr.SettAmount = Convert.ToDouble(dt.Rows[i]["TotalSettlementAmount"].ToString());

                        srl.Add(sr);
                    }
                }
            }

            return srl;
        }

    }
}