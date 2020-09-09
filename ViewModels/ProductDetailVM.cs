using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Models;

namespace KaraYadak.ViewModels
{
    public class ProductDetailVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Images { get; set; }
    }
    public class CarCategoriesForProduct
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public List<string> Subcategories { get; set; }
    }
    public class ProductDetailSVM
    {
        public ProductDetailVM Product { get; set; }
        public List<ProductForIndexVM> OtherProduct { get; set; }
        public Dictionary<string, List<string>> CarCategoriesForProduct { get; set; }
    }
}
