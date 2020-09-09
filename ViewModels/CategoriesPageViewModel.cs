using System.Collections.Generic;
using KaraYadak.Models;

namespace KaraYadak.ViewModels
{
    public class CategoriesPageViewModel
    {
        public List<Category> Categories { get; set; }
        public List<CategoryType> CategoryTypes { get; set; }
    }
}

