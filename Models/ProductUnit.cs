using System;

namespace KaraYadak.Models
{
    public class ProductUnit
    {
        public ProductUnit()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public double ParentRatio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
