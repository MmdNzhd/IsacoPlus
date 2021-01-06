using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class MoneyBackViewModel
    {
        public string ShebaNumber { get; set; }
        public double Price { get; set; }
        public int PlanId { get; set; }
        public string CompanyId { get; set; }
        public bool IsImmediately { get; set; }
        public string TrackingCode { get; set; }
        public int FactorId { get; set; }
    }
}
