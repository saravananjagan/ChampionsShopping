using IPMSService.FetchMetadata;
using PMSService.FetchMetadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSProxy.FetchMetadata
{
    public static class ModuleDisplayDetailsProxy
    {
        public static DataSet FetchModuleDisplayDetails(string TenantId, string UserId)
        {
            IModuleDisplayDetailsService moduleDisplayDetailsService = new ModuleDisplayDetailsService();
            return moduleDisplayDetailsService.FetchModuleDisplayDetails(TenantId, UserId);
        }
    }
}
