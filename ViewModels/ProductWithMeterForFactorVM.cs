using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Models;

namespace KaraYadak.ViewModels
{
    public class ProductWithMeterForFactorVM:Product
    {
        public double Count { get; set; }
    }
}
