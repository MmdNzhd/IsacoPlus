using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.ViewModels
{
    public class FilterVM
    {
        public string SubCategory { get; set; }
        public List<string> Categories { get; set; }
        public int CatId { get; set; }
    }
    public class FilteringVM
    {
        public int CatId { get; set; }
        public string SubCategory { get; set; }
        public string Categories { get; set; }
    }
}
