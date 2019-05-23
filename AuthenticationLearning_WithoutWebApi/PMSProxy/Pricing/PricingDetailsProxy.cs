using IPMSService.Pricing;
using PMSModel.Pricing;
using PMSService.Pricing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSProxy.Pricing
{
    public static class PricingDetailsProxy
    {
        public static bool InsertBulkPricingDetails(string ImportValues)
        {
            IPricingDetailsService pricingDetailsService = new PricingDetailsService();
            return pricingDetailsService.InsertBulkPricingDetails(ImportValues);
        }
        public static DataSet FetchPricingDetails()
        {
            IPricingDetailsService pricingDetailsService = new PricingDetailsService();
            return pricingDetailsService.FetchPricingDetails();
        }
        public static bool CUDPricingDetails(PricingData pricingData, string QuerySelector)
        {
            IPricingDetailsService pricingDetailsService = new PricingDetailsService();
            return pricingDetailsService.CUDPricingDetails(pricingData,QuerySelector);
        }
        public static DataSet FetchUserPricingDetails(string UserId)
        {
            IPricingDetailsService pricingDetailsService = new PricingDetailsService();
            return pricingDetailsService.FetchUserPricingDetails(UserId);
        }
    }
}
