using System;

namespace KaraYadak.Models
{
    public class WarehouseProductQuantity
    {
        public WarehouseProductQuantity()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public int Id { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public double Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}