using System;

namespace KaraYadak.Models
{
    public class Menu
    {
        public Menu()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public int Id { get; set; }
        public int AccessId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
