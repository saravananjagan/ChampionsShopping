using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSIDAL.FetchMetadata
{
    public interface ITenantDetailsDAL
    {
        string FetchTenantId();
    }
}
