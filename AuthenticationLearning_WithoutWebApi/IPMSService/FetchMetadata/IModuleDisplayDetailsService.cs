using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPMSService.FetchMetadata
{
    public interface IModuleDisplayDetailsService
    {
        DataSet FetchModuleDisplayDetails(string TenantId, string UserId);
    }
}
