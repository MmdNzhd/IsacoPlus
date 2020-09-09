using KaraYadak.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace KaraYadak.ViewModels
{
    public class ProductWithCategoryVM
    {
        public Product Product { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Categories { get; set; }
        public int SubCategoryId { get; set; }
        public string CategoriyTypes { get; set; }
        public string SubCategories { get; set; }
        public string Images { get; set; }
        public string Url { get; set; }
        public string Codes { get; set; }
        public string Colors { get; set; }
        public string MainImages { get; set; }
        public bool IsFavorite { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<ProductWithCategoryVM> Products { get; set; }
        public int Id { get; set; }
        public string Tags { get; set; }
    }
}
