using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace KaraYadak.Models
{
    public class Category
    {
        public Category()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Parent { get; set; }
        public ICollection<Category> Childs { get; set; }
        public CategoryType CategoryType { get; set; }
        public Image Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
