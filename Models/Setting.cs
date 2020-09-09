using System;
using System.ComponentModel.DataAnnotations;

namespace KaraYadak.Models
{
    public class Setting
    {
        public Setting()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
