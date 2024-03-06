using ProductCatalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace ProductCatalog.Database
{
    public class ProductDbContext : DbContext
    {
        private readonly IConfiguration _configuration;        

        public ProductDbContext(DbContextOptions<ProductDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
           modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Nike Running Shoes",
                Description = "Comfortable running shoes",
                AvailableStock = 50,
                Price = 99.99m,
                CategoryId = 1,
                ImageFileName = "NikeShoes.jpg"
            },
            new Product
            {
                Id = 2,
                Name = "Adidas Soccer Ball",
                Description = "High-quality soccer ball",
                AvailableStock = 100,
                Price = 24.99m,
                CategoryId = 2,
                ImageFileName = "AdidasBall.jpg"
            },
            new Product
            {
                Id = 3,
                Name = "Under Armour T-Shirt",
                Description = "Moisture-wicking workout shirt",
                AvailableStock = 75,
                Price = 29.99m,
                CategoryId = 1,
                ImageFileName = "UnderArmourShirt.jpg"
            });

            modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Apparel",
                Description = "Clothing and accessories"
            },
            new Category
            {
                Id = 2,
                Name = "Sports",
                Description = "Sporting goods"
            });

            modelBuilder.Entity<Inventory>().HasData(
                new Inventory()
                {
                    Id=1,
                    ProductId = 1,
                    Quantity = 50
                },
                new Inventory()
                {
                    Id = 2,
                    ProductId = 2,
                    Quantity = 100
                },
                new Inventory()
                {
                    Id = 3,
                    ProductId = 3,
                    Quantity = 75
                });
        }
    }
}
