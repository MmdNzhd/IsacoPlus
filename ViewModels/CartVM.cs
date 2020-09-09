using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Models;

namespace KaraYadak.ViewModels
{
    public class CartVM
    {
        public List<Product> Products { get; set; }
        public List<int> ProductIds { get; set; }
        public bool IsProfileCompelete { get; set; }
        public List<int> Quantity { get; set; }
        public List<double> Lengths { get; set; }
        public DateTime Date { get; set; }
        public bool IsApp { get; set; }
        public double Price { get; set; }
        public double SendPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    public class SiteCartVM
    {
        public List<ProductWithMeterForFactorVM> Products { get; set; }
        public List<string> ProductIds { get; set; }
        public bool IsProfileCompelete { get; set; }
        public List<int> Quantity { get; set; }
        public List<int> Count { get; set; }
        public DateTime Date { get; set; }
        public bool IsApp { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public double SendPrice { get; set; }
        public double TotalPrice { get; set; }
    }
}
