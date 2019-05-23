using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSModel.Order
{
    public class CartData
    {
        public string UserCartId { get; set; }
        public string ProductPricingId { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }        
    }
}
