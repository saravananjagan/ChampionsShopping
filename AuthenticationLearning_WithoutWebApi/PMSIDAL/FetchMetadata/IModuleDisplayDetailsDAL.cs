using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSIDAL.FetchMetadata
{
    public interface IModuleDisplayDetailsDAL
    {
        DataSet FetchModuleDisplayDetails(string TenantId, string UserId);
    }
}
