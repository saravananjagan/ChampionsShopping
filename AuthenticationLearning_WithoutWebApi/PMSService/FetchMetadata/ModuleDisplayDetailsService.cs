using IPMSService.FetchMetadata;
using PMSDAL.FetchMetadata;
using PMSIDAL.FetchMetadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSService.FetchMetadata
{
    public class ModuleDisplayDetailsService : IModuleDisplayDetailsService
    {
        public DataSet FetchModuleDisplayDetails(string TenantId, string UserId)
        {
            DataSet ModuleDetails = new DataSet();
            try
            {
                IModuleDisplayDetailsDAL moduleDisplayDetailsDAL = new ModuleDisplayDetailsDAL();
                ModuleDetails=moduleDisplayDetailsDAL.FetchModuleDisplayDetails(TenantId,UserId);
            }
            catch (Exception e)
            {

            }
            return ModuleDetails;
        }
    }
}
