using System.Collections.Generic;

namespace KaraYadak.ViewModels
{
    public class ProductCategoriesWithType
    {
        public string Name { get; set; }
        public List<ProductCategoryWithChilds> Categories { get; set; }
    }
}
