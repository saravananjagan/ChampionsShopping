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
    }
}