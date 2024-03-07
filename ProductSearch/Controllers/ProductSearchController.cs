using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductSearch.Entities;
using ProductSearch.Models;
using ProductSearch.Repositories;

namespace ProductSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSearchController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;

        public ProductSearchController(IProductRepository productRepository, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _configuration = configuration;
        }

        [HttpGet]        
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        public async Task<IActionResult> SearchProducts([FromQuery]int? categoryId, [FromQuery]string searchText)
        {
            IEnumerable<Product> searchResults;
            if(categoryId == null)
            {
                searchResults =  await _productRepository.SearchProducts(searchText);                
            } else
            {
                searchResults = await _productRepository.SearchProductsByCategory(categoryId.Value,searchText);                
            }
            return Ok(searchResults.Select(p => Map(p)));
        }

        private ProductDto Map(Product product)
        {
            return new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                ImageUrl = $"{_configuration["BlobStorageUrl"]}{product.ImageFileName}"
            };
        }
    }
}
