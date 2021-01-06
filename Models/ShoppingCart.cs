using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Phone { get; set; }
        public string MapAddress { get; set; }
        public string RequestCode { get; set; }
        public RequestStatus Status { get; set; }
        public PaymentType PaymentType { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public string Price { get; set; }
        public DateTime Date { get; set; }
        public string DiscountPercent { get; set; }
        public string SendPrice { get; set; }
        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
