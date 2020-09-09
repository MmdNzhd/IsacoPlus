using System;
using System.ComponentModel.DataAnnotations;

namespace KaraYadak.Models
{
    public class Image
    {
        public Image()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
