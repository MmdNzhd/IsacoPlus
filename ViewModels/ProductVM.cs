using KaraYadak.Models;
using System.Collections.Generic;
using System.Drawing;

namespace KaraYadak.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public double Discount { get; set; }
        public string ImageUrl { get; set; }
        public ProductStatus Status { get; set; }
        public string Tags { get; set; }
        public string setProductsValue { get; set; }
        public string   QrCode { get; set; }
    }
}
