using PMSIDAL.FetchMetadata;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSDAL.FetchMetadata
{
    public class ModuleDisplayDetailsDAL : IModuleDisplayDetailsDAL
    {
        private string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public DataSet FetchModuleDisplayDetails(string TenantId, string UserId)
        {
            DataSet ModuleDisplayDetails = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("dbo.USP_FetchModuleDisplayDetails",connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@TenantId",SqlDbType.UniqueIdentifier).Value=Guid.Parse(TenantId);
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(UserId);
                    connection.Open();
                    sqlCommand.CommandTimeout = 120;
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                    adapter.Fill(ModuleDisplayDetails);
                    connection.Close();
                    adapter.Dispose();
                }
                    return ModuleDisplayDetails;
            }
            finally
            {
                ModuleDisplayDetails = null;
            }
        }
    }
}
