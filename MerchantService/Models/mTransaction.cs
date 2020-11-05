using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MerchantService.Models
{
    public class mTransaction
    {
        public int TrxID { get; set; }
        public string MPU_Merchant_ID { get; set; }
        public string Merchant_Name { get; set; }
        public string Merchant_Address { get; set; }
        public string Merchant_Email { get; set; }
        public string Merchant_Phone { get; set; }
        public string Settlement_Acc { get; set; }
        public Nullable<int> BusinessCategoryID { get; set; }
        public string BusinessCategory { get; set; }
        public string RecordType { get; set; }
        public string CardNo { get; set; }
        public string CardType { get; set; }
        public string ProcessingCode { get; set; }
        public Nullable<double> TrxAmount { get; set; }
        public Nullable<double> SettAmount { get; set; }
        public Nullable<System.DateTime> TrxDate { get; set; }
        public Nullable<double> MDRValue { get; set; }
        public Nullable<double> SettConversationRate { get; set; }
        public Nullable<double> TRXMDRRate { get; set; }
        public string CurrencyCode { get; set; }
        public string SettCurrencyCode { get; set; }
        public string TerminalID { get; set; }
        public string TRXRemark { get; set; }

        public bool InsertTran()
        {
            bool is_success = false;
            using (Models.MerchantService db = new MerchantService())
            {
                t_Transactions tran = new t_Transactions();

                tran.CardNo = CardNo;
                tran.CardType = CardType;
                tran.ProcessingCode = ProcessingCode;
                tran.TrxAmount = TrxAmount;
                tran.SettAmount = SettAmount;
                tran.MPU_Merchant_ID = MPU_Merchant_ID;
                tran.TrxDate = TrxDate;
                tran.MDRValue = MDRValue;
                tran.SettConversationRate = SettConversationRate;
                tran.CurrencyCode = CurrencyCode;
                tran.SettCurrencyCode = SettCurrencyCode;
                tran.TerminalID = TerminalID;
                tran.TRXRemark = TRXRemark;
                tran.RecordType = RecordType;
                tran.TRXMDRRate = TRXMDRRate;

                db.t_Transactions.Add(tran);
                db.SaveChanges();
                is_success = true;
            }

            return is_success;
        }

        //To use Report->TransactionQuery as stored procedure
        public List<mTransaction> transaction_query(string searchkey, DateTime startdate, DateTime enddate)
        {
            List<mTransaction> Trans = new List<mTransaction>();
            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();
                SqlCommand cmd = new SqlCommand("sp_r_tranquery", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@searchkey", searchkey);
                cmd.Parameters.AddWithValue("@startdate", startdate);
                cmd.Parameters.AddWithValue("@enddate", enddate);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if(dt.Rows.Count>0)
                {
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        mTransaction tran = new mTransaction();
                        tran.MPU_Merchant_ID = dt.Rows[i]["MPU_Merchant_ID"].ToString();
                        tran.Merchant_Name = dt.Rows[i]["Merchant_Name"].ToString();
                        tran.TrxDate =Convert.ToDateTime(dt.Rows[i]["TrxDate"].ToString());
                        string cno= dt.Rows[i]["CardNo"].ToString();
                        tran.CardNo = cno.Substring(0, 6) + "xxxxxx" + cno.Substring(cno.Length - 4, 4);
                        tran.CardType = dt.Rows[i]["CardType"].ToString();
                        tran.TrxAmount=Convert.ToDouble(dt.Rows[i]["TrxAmount"].ToString());
                        tran.MDRValue = Convert.ToDouble(dt.Rows[i]["MDRValue"].ToString());
                        tran.SettAmount=Convert.ToDouble(dt.Rows[i]["SettAmount"].ToString());
                        if(!String.IsNullOrEmpty(dt.Rows[i]["TRXMDRRate"].ToString()))
                        {
                            tran.TRXMDRRate = Convert.ToDouble(dt.Rows[i]["TRXMDRRate"].ToString());
                        }
                        

                        Trans.Add(tran);
                    }
                }
            }
            return Trans;
        }

        //stored procedure is use for this function to delete and store in RecycleBin
        public bool Delete(string uid)
        {
            bool is_success = false;

            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings["MerchantServiceDB"].ConnectionString))
            {
                consql.Open();
                SqlCommand cmd = new SqlCommand("sp_t_delete", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TrxID", TrxID);
                cmd.Parameters.AddWithValue("@UID", uid);
                cmd.ExecuteNonQuery();
                consql.Close();
                is_success = true;
            }
            

            return is_success;
        }

        public List<mTransaction> SettlementReport()
        {
            List<mTransaction> ts = new List<mTransaction>();
            using (SqlConnection consql = new SqlConnection(ConfigurationManager.ConnectionStrings[""].ConnectionString))
            {
                consql.Open();
                SqlCommand cmd = new SqlCommand("", consql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("", TrxDate);
                cmd.Parameters.AddWithValue("", RecordType);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                if(dt.Rows.Count>0)
                {
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        mTransaction m = new mTransaction();
                        
                    }
                }
            }
            return ts;
        }
    }

    public class MPU:mTransaction
    {
        public List<mTransaction> read_file_inc(string filename)
        {
            List<mTransaction> transaction_list = new List<mTransaction>();

            return transaction_list;
        }
    }
}