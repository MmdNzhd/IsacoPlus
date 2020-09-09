using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KaraYadak.Models
{
    public class Factor
    {
        public Factor()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserName { get; set; }
        public Warehouse Warehouse { get; set; }
        public ICollection<FactorItem> FactorItems { get; set; }
        public string Code { get; set; }
        public double CargoPrice { get; set; }
        public string SellerFactorNumber { get; set; }
        public ReciptType ReciptType { get; set; }

        // سررسید
        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


}