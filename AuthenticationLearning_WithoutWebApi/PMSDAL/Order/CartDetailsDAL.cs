using PMSIDAL.Order;
using PMSModel.Order;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSDAL.Order
{
    public class CartDetailsDAL: ICartDetailsDAL
    {
        private string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public bool CUDCartValue(CartData cartData,string QuerySelector)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("dbo.USP_CUDCartDetails", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@QuerySelector", SqlDbType.VarChar).Value = QuerySelector;
                sqlCommand.Parameters.Add("@UserId", SqlDbType.VarChar).Value = cartData.UserId;
                if (!string.IsNullOrEmpty(cartData.ProductPricingId))
                {
                    sqlCommand.Parameters.Add("@ProductPricingId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(cartData.ProductPricingId);
                }
                if (!string.IsNullOrEmpty(cartData.Quantity.ToString()))
                {
                    sqlCommand.Parameters.Add("@Quantity", SqlDbType.BigInt).Value = cartData.Quantity;
                }
                connection.Open();
                sqlCommand.CommandTimeout = 120;
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            return true;
        }

        public DataSet FetchCartDetails(string UserId, string ProductPricingId, string QuerySelector)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("dbo.USP_FetchCartDetails", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@QuerySelector", SqlDbType.VarChar).Value = QuerySelector;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(UserId);
                    if (!string.IsNullOrEmpty(ProductPricingId))
                    {
                        sqlCommand.Parameters.Add("@ProductPricingId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(ProductPricingId);
                    }
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
