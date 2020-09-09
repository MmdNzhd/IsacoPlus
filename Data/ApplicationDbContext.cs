using System;
using System.Collections.Generic;
using System.Text;
using KaraYadak.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KaraYadak.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Favorite>Favorites  { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Baner> Baners { get; set; }
        public DbSet<Access> Access { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductUnit> ProductUnits { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductCategoryType> ProductCategoryTypes { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseProductQuantity> WarehouseProductQuantities { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<FactorItem> FactorItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<VerificationCode> VerificationCode { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<ContactUsMessage> contactUsMessages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }
        public DbSet<QRCode> QRCodes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<QRCode>()
                .HasIndex(u => u.Code)
                .IsUnique();
        }
    }

}
