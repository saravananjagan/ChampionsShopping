using PMSModel.Order;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSIDAL.Order
{
    public interface ICartDetailsDAL
    {
        bool CUDCartValue(CartData cartData, string QuerySelector);
        DataSet FetchCartDetails(string UserId, string ProductPricingId, string QuerySelector);
    }
}
