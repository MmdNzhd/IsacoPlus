using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.ViewModels
{

      public class CategoriesVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Picture { get; set; }
    }
    public class ProductForIndexVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Picture { get; set; }
        public double Off { get; set; }
        public double Price { get; set; }
        public float Rate { get; set; }
        public DateTime CreatingDate { get; set; }
        public string Code { get; set; }
        public string Brand { get; set; }
    }
    
    public class ListForIndexVM
    {
        public List<ProductForIndexVM> First { get; set; }
        public List<ProductForIndexVM> Second { get; set; }
        public List<ProductForIndexVM> Third { get; set; }
        public List<ProductForIndexVM> Fourth { get; set; }
    }

    public class ProductsDetail : ProductForIndexVM
    {
        public string Color { get; set; }
        public string Describe { get; set; }
    }
    public class DesignerForIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }
}
