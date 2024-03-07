using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductSearch.Entities;
using System.Collections.Generic;

namespace ProductSearch.Database
{
    public class ProductDbContext: DbContext
    {
        private readonly IConfiguration _configuration;
        public ProductDbContext(DbContextOptions<ProductDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
    }
}
