using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.ViewModels
{
    public class DashboardVM
    {
        public double SellerCounts { get; set; }
        public double OrderCounts { get; set; }
        public double ProductCounts { get; set; }
        public double CustomerCounts { get; set; }
        public List<ProductForIndexVM> TopSellProducts { get; set; }
        public List<ShoppingCart> LastOrders { get; set; }
    }
}
