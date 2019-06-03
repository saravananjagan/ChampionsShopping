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
        public static DataSet FetchPricingDetails(string UserId)
        {
            IPricingDetailsService pricingDetailsService = new PricingDetailsService();
            return pricingDetailsService.FetchPricingDetails(UserId);
        }
        public static DataSet FetchCumulativeCartDetails(string UserId)
        {
            IPricingDetailsService pricingDetailsService = new PricingDetailsService();
            return pricingDetailsService.FetchCumulativeCartDetails(UserId);
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
        public static bool InsertBulkPricingPhotoDetails(string ImportValues)
        {
            IPricingDetailsService pricingDetailsService = new PricingDetailsService();
            return pricingDetailsService.InsertBulkPricingPhotoDetails(ImportValues);
        }

		/// <summary>
		/// Method to get the Pricing Details by ProductId
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
		public static PricingData GetPricingDetailsByProductId(string productId)
		{
			IPricingDetailsService pricingDetailsService = new PricingDetailsService();
			return pricingDetailsService.GetPricingDetailsByProductId(productId);
		}

	}
}
