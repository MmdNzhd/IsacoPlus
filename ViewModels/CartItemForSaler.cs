using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Models;

namespace KaraYadak.ViewModels
{
    public class CartItemForSaler:CartItem
    {
        public double discount { get; set; }
        public string Descrip { get; set; }
    }
}
