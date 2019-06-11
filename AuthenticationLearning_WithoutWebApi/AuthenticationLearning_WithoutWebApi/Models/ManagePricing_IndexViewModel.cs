using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AuthenticationLearning_WithoutWebApi.Models
{
    public class ManagePricing_IndexViewModel
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public DataTable PricingDataTable { get; set; }
        public string UserId { get; set; }
        public bool IsChampion { get; set; }
        public Dictionary<string, List<string>> ProductPhotoMappingDic { get; set;   }
        public Dictionary<string, string> PhotoMappingIdDic { get; set; }
        public string TotalItems { get; set; }
        public string TotalBuyValue { get; set; }
    }

    public class ManagePricingPhotoUploadModel
    {
        public HttpPostedFileBase ImageFile { get; set; }
        public string ProductId { get; set; }
        public string ProductPricingId { get; set; }
        public string Ordinal { get; set; }
        public string ProductPhotoMappingId { get; set; }
    }


}