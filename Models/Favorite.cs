using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProductCode { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
