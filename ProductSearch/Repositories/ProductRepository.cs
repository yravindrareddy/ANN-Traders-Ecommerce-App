using Microsoft.EntityFrameworkCore;
using ProductSearch.Database;
using ProductSearch.Entities;

namespace ProductSearch.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _productDbContext;

        public ProductRepository(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }
        public async Task<IEnumerable<Product>> SearchProducts(string searchText)
        {
            return await _productDbContext.Products.Include(p => p.Category).Where(p => p.Name.ToLower().Contains(searchText.ToLower()) ||
            p.Description.ToLower().Contains(searchText.ToLower())).ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsByCategory(int CategoryId, string searchText)
        {
            return await _productDbContext.Products.Include(p => p.Category).Where(p => p.CategoryId == CategoryId && (p.Name.ToLower().Contains(searchText.ToLower()) ||
            p.Description.ToLower().Contains(searchText.ToLower()))).ToListAsync();
        }
    }
}
