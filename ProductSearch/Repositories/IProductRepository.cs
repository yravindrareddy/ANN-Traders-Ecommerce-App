using ProductSearch.Entities;

namespace ProductSearch.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> SearchProductsByCategory(int CategoryId, string searchText);
        Task<IEnumerable<Product>> SearchProducts(string searchText);
    }
}
