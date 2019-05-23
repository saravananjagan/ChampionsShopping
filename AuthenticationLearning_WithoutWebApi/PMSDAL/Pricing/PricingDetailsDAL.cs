using PMSIDAL.Pricing;
using PMSModel.Pricing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSDAL.Pricing
{
    public class PricingDetailsDAL: IPricingDetailsDAL
    {
        private string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public bool InsertBulkPricingDetails(string ImportValues)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("dbo.USP_ImportPricingDetails", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@ImportValues", SqlDbType.VarChar).Value = ImportValues;
                connection.Open();
                sqlCommand.CommandTimeout = 120;
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            return true;
        }

        public DataSet FetchPricingDetails()
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlConnection connection=new SqlConnection(ConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("dbo.USP_FetchProductPricingDetails", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
                    dataAdapter.Fill(dataSet);
                    connection.Close();
                    dataAdapter.Dispose();
                }
                return dataSet;
            }
            finally
            {
                dataSet = null;
            }
        }

        public bool CUDPricingDetails(PricingData pricingData,string QuerySelector)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("dbo.USP_CUDPricingDetails", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@QuerySelector", SqlDbType.VarChar).Value = QuerySelector;
                sqlCommand.Parameters.Add("@ProductPricingId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(pricingData.ProductPricingId);
                if (!string.IsNullOrEmpty(pricingData.ProductId))
                {
                    sqlCommand.Parameters.Add("@ProductId", SqlDbType.VarChar).Value = pricingData.ProductId;
                }
                if (!string.IsNullOrEmpty(pricingData.ProductName))
                {
                    sqlCommand.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = pricingData.ProductName;
                }
                if (!string.IsNullOrEmpty(pricingData.BuyPrice.ToString()))
                {
                    sqlCommand.Parameters.Add("@BuyPrice", SqlDbType.Float).Value = pricingData.BuyPrice;
                }
                if (!string.IsNullOrEmpty(pricingData.SellPrice.ToString()))
                {
                    sqlCommand.Parameters.Add("@SellPrice", SqlDbType.Float).Value = pricingData.SellPrice;
                }
                if (!string.IsNullOrEmpty(pricingData.Profit.ToString()))
                {
                    sqlCommand.Parameters.Add("@Profit", SqlDbType.Float).Value = pricingData.Profit;
                }
                if (!string.IsNullOrEmpty(pricingData.MRP.ToString()))
                {
                    sqlCommand.Parameters.Add("@MRP", SqlDbType.Float).Value = pricingData.MRP;
                }
                sqlCommand.Parameters.Add("@Status", SqlDbType.Bit).Value = 1;
                connection.Open();
                sqlCommand.CommandTimeout = 120;
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            return true;
        }

        public DataSet FetchUserPricingDetails(string UserId)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("dbo.USP_FetchProductPricingDetails_UserCart", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(UserId);
                    connection.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
                    dataAdapter.Fill(dataSet);
                    connection.Close();
                    dataAdapter.Dispose();
                }
                return dataSet;
            }
            finally
            {
                dataSet = null;
            }
        }
    }
}
