using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Models;

namespace KaraYadak.ViewModels
{
    public class CartItemVM
    {
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string QRCode { get; set; }
    }
    public class CartItemForSalersVM
    {
        public int ProductId { get; set; }
        public double Meter { get; set; }
        public double Price { get; set; }
        public string QRCode { get; set; }
        public string SalerName { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
    }
}
