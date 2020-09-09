using System;
using System.Collections;

namespace KaraYadak.Models
{
    public class ProductCategory
    {
        public ProductCategory()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Parent = 0;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Parent { get; set; }
        public int ProductCategoryType { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
