using PMSModel.Pricing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSIDAL.Pricing
{
    public interface IPricingDetailsDAL
    {
        bool InsertBulkPricingDetails(string ImportValues);
        DataSet FetchPricingDetails();
        DataSet FetchPricingDetails(string UserId);
        bool CUDPricingDetails(PricingData pricingData, string QuerySelector);
        DataSet FetchUserPricingDetails(string UserId);
        bool InsertBulkPricingPhotoDetails(string ImportValues);
		PricingData GetPricingDetailsByProductId(string productId);
        DataSet FetchCumulativeCartDetails(string UserId);
    }
}
