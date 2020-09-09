using KaraYadak.Models;
using System.Collections.Generic;

namespace KaraYadak.ViewModels
{
    public class ProductCategoryWithChilds
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductCategory> Childs { get; set; }
    }
}
