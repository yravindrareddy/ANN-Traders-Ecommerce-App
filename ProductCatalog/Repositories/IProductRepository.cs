using ProductCatalog.Entities;

namespace ProductCatalog.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(int productId);
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        void DeleteProduct(Product product);        
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<Product>> SearchProductsByCategory(int CategoryId, string searchText);
        Task<IEnumerable<Product>> SearchProducts(string searchText);
    }
}
