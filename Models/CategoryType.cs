using System;
using System.Collections;
using System.Collections.Generic;

namespace KaraYadak.Models
{
    public class CategoryType
    {
        public CategoryType()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Category> Categories { get; set; }
        public Image Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}