using IPMSService;
using PMSDAL.FetchMetadata;
using PMSIDAL.FetchMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSService.FetchMetadata
{
    public class TenantDetailsService : ITenantDetailsService
    {
        public string FetchTenantId()
        {
            string tenantid = null;
            try
            {
                ITenantDetailsDAL tenantDetailsDAL = new TenantDetailsDAL();
                tenantid= tenantDetailsDAL.FetchTenantId();
            }
            catch(Exception e)
            {

            }
            return tenantid;
        }
    }
}
