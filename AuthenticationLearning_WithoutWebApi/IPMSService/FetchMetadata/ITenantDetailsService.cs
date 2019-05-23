using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPMSService
{
    public interface ITenantDetailsService
    {
        string FetchTenantId();
    }
}
