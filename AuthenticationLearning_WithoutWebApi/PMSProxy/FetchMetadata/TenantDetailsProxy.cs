using IPMSService;
using PMSService.FetchMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSProxy.FetchMetadata
{
    public static class TenantDetailsProxy
    {
        public static string FetchTenantId()
        {
            ITenantDetailsService tenantDetailsService = new TenantDetailsService();
            return tenantDetailsService.FetchTenantId();
        }
    }
}
