using ProductCatalog.Database;
using ProductCatalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteProduct(Product product)
        {            
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _dbContext.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Product?> GetProductById(int productId)
        {
            var product = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id == productId);
            return product;
        }

        public async Task UpdateProduct(Product product)
        {
            var existingProduct = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == product.Id);
            if(existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.AvailableStock = product.AvailableStock;
                existingProduct.ImageFileName = product.ImageFileName;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
