using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KaraYadak.Models
{
    public class Product
    {
        public Product()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public int Discount { get; set; }
        public bool SpecialSale { get; set; }
        public string Tags { get; set; }
        public string SetProducts { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Image> Images { get; set; }
        public string Description { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public int CategoryIdLvl1 { get; set; }
        public int CategoryIdLvl2 { get; set; }
        public int CategoryIdLvl3 { get; set; }
        public int ProductCategoryType { get; set; }
        public ProductUnit Unit { get; set; }
        public int MinEntity { get; set; }
        public int MaxEntity { get; set; }
        public float? Rate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string QR { get; set; }
    }
}