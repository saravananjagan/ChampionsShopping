using PMSIDAL.FetchMetadata;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSDAL.FetchMetadata
{
    public class TenantDetailsDAL : ITenantDetailsDAL
    {
        private string Connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public string FetchTenantId()
        {            
            StringBuilder FetchQuery = new StringBuilder();
            FetchQuery.Append("SELECT TenantId FROM Pms.TenantDetails");
            string TenantId = null;
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                SqlCommand command = new SqlCommand(FetchQuery.ToString(), connection);
                connection.Open();
                SqlDataReader reader=null;
                try
                {
                    reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        TenantId = reader["TenantId"].ToString();
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return TenantId;
        }
    }
}
