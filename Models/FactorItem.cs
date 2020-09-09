using System;

namespace KaraYadak.Models
{
    public class FactorItem
    {
        public FactorItem()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public int Id { get; set; }
        public Product Product { get; set; }
        public ProductUnit Unit { get; set; }
        public ProductUnit BaseUnit { get; set; }
        public double Quantity { get; set; }
        public int Discount { get; set; }

        //  مالیات
        public int Tax { get; set; }

        // عوارض
        public int Charges { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}