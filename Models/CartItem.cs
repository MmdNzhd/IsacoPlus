using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public DateTime Date { get; set; }
        public string QR { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

    }
}
