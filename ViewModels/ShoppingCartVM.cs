using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.ViewModels
{
    public class ShoppingCartVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Phone { get; set; }
        public string MapAddress { get; set; }
        public string RequestCode { get; set; }
        public string Status { get; set; }
        public string PaymentType { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public PaymentType Payment { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public string Price { get; set; }
        public string Date { get; set; }
        public string DiscountPercent { get; set; }
    }
}
