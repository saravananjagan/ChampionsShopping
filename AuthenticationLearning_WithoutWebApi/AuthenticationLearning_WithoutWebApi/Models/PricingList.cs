using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthenticationLearning_WithoutWebApi.Models
{
    public class PricingList
    {
        public string ProductPricingId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public float BuyPrice { get; set; }
        public float SellPrice { get; set; }
        public float Profit { get; set; }
        public float MRP { get; set; }
    }
}