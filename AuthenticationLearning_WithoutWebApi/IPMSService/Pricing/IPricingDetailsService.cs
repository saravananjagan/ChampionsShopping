﻿using PMSModel.Pricing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPMSService.Pricing
{
    public interface IPricingDetailsService
    {
        bool InsertBulkPricingDetails(string ImportValues);
        DataSet FetchPricingDetails();
        bool CUDPricingDetails(PricingData pricingData, string QuerySelector);
        bool CUDPricingPhotoDetails(PricingPhotoData pricingPhotoData, string QuerySelector);
        DataSet FetchUserPricingDetails(string UserId);
        bool InsertBulkPricingPhotoDetails(string ImportValues);
		PricingData GetPricingDetailsByProductId(string productId);
        DataSet FetchPricingDetails(string UserId);
        DataSet FetchCumulativeCartDetails(string UserId);

    }
}
