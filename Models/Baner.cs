using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class Baner
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
