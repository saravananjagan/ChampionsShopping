using IPMSService.Order;
using PMSDAL.Order;
using PMSIDAL.Order;
using PMSModel.Order;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSService.Order
{
    public class CartDetailsService: ICartDetailsService
    {
        public bool CUDCartValue(CartData cartData, string QuerySelector)
        {
            try
            {
                ICartDetailsDAL cartDetailsDAL = new CartDetailsDAL();
                cartDetailsDAL.CUDCartValue(cartData, QuerySelector);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public DataSet FetchCartDetails(string UserId, string ProductPricingId, string QuerySelector)
        {
            DataSet dataSet = new DataSet();
            try
            {
                ICartDetailsDAL cartDetailsDAL = new CartDetailsDAL();
                dataSet=cartDetailsDAL.FetchCartDetails(UserId,ProductPricingId,QuerySelector);                
            }
            catch (Exception e)
            {
                
            }
            return dataSet;
        }
    }
}
