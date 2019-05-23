using PMSModel.Order;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPMSService.Order
{
    public interface ICartDetailsService
    {
        bool CUDCartValue(CartData cartData, string QuerySelector);
        DataSet FetchCartDetails(string UserId, string ProductPricingId, string QuerySelector);
    }
}
