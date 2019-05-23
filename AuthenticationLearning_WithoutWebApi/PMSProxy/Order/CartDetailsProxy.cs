using IPMSService.Order;
using PMSModel.Order;
using PMSService.Order;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSProxy.Order
{
    public static class CartDetailsProxy
    {
        public static bool CUDCartValue(CartData cartData, string QuerySelector)
        {
            ICartDetailsService cartDetailsService = new CartDetailsService();
            return cartDetailsService.CUDCartValue(cartData, QuerySelector);
        }

        public static DataSet FetchCartDetails(string UserId, string ProductPricingId, string QuerySelector)
        {
            ICartDetailsService cartDetailsService = new CartDetailsService();
            return cartDetailsService.FetchCartDetails(UserId,ProductPricingId,QuerySelector);
        }
    }
}
