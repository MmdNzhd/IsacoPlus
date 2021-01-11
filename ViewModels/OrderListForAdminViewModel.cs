using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class OrderListForAdminViewModel
    {
        public int OrderId { get; set; }
        public string Date { get; set; }
        public string phoneNumber { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal PriceWithDiscount { get; set; }
        public decimal PriceWithTax { get; set; }
        public string OrderType { get; set; }
        public string CompanyName { get; set; }
        public string Issuccess { get; set; }
        public string PostType { get; set; }
        public string OrderLevel { get; set; }
        public int ShopingId { get; set; }
        public string PostTrackingNumber { get; set; }
    }
}
