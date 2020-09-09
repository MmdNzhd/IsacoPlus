using System;
using System.ComponentModel.DataAnnotations;

namespace KaraYadak.Models
{
    public class Warehouse
    {
        public Warehouse()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
