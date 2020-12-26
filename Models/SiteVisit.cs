using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class SiteVisit
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public DateTime Date { get; set; }
    }
}
