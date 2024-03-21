using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Database;
using ProductCatalog.Entities;
using ProductCatalog.Models;
using ProductCatalog.Repositories;
using ServiceBus;

namespace ProductCatalog.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _dbContext;        
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        public ProductController(ProductDbContext dbContext, IProductRepository productRepository, IMapper mapper, IConfiguration configuration, IMessageBus messageBus)
        {
            _dbContext = dbContext;            
            _productRepository = productRepository;
            _mapper = mapper;
            _configuration = configuration;
            _messageBus = messageBus;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        public async Task<IActionResult> GetAllProducts()
        {
            var productsResponse = await _productRepository.GetAllProducts();
            var products = productsResponse.Select(p => Map(p));
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(Map(product));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
            {
                return BadRequest();
            }

            var imageName = GetUniqueImageName(createProductDto.Image);

            var product = new Product()
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                AvailableStock = createProductDto.AvailableStock,
                CategoryId = createProductDto.CategoryId,
                ImageFileName = imageName
            };

            await _productRepository.AddProduct(product);
            await UploadImage(createProductDto.Image, imageName);

            //await _messageBus.PublishMessage(_configuration["ServiceBus:AZURE_SERVCIE_BUS_CONN"], product, _configuration["ServiceBus:QueueName"]);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto updateProductDto)
        {
            if (id != updateProductDto.Id)
            {
                return BadRequest();
            }
            var existingProduct = await _productRepository.GetProductById(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            var imageName = updateProductDto.Image == null ?
                existingProduct.ImageFileName :
                GetUniqueImageName(updateProductDto.Image);

            var updateProduct = new Product()
            {
                Id = updateProductDto.Id,
                Name = updateProductDto.Name,
                Description = updateProductDto.Description,
                Price = updateProductDto.Price,
                AvailableStock = updateProductDto.AvailableStock,
                CategoryId = updateProductDto.CategoryId,
                ImageFileName = imageName
            };                       

            await _productRepository.UpdateProduct(updateProduct);
            if (updateProductDto.Image != null)
            {
                await UploadImage(updateProductDto.Image, imageName);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _productRepository.GetProductById(id);

            if (existingProduct == null)
            {
                return NotFound();
            }
            _productRepository.DeleteProduct(existingProduct);
            return NoContent();
        }

        [HttpGet("Categories")]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
        public async Task<IActionResult> GetCategories()
        {
            var categoriesResponse = await _productRepository.GetCategories();
            var categories = categoriesResponse.Select(c => new CategoryDto()
            {
                Id = c.Id,
                Name = c.Name
            });
            return Ok(categories);
        }

        private async Task UploadImage(IFormFile formFile, string imageName)
        {
            string connectionString = _configuration.GetConnectionString("AZURE_STORAGE_CONNECTIONSTRING");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");
                    
            BlobClient blobClient = containerClient.GetBlobClient(imageName);

            using (var stream = formFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
                blobClient.SetHttpHeaders(new BlobHttpHeaders
                {
                    ContentType = "image/jpeg" // Adjust content type as needed (e.g., image/png, image/gif)
                });

                // Set content disposition to inline
                blobClient.SetHttpHeaders(new BlobHttpHeaders
                {
                    ContentDisposition = "inline"
                });
            }            
        }

        private string GetUniqueImageName(IFormFile formFile)
        {
            var fileExtension = Path.GetExtension(formFile.FileName);
            var guid = Guid.NewGuid();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(formFile.FileName);
            var uniqueImageName = $"{fileNameWithoutExtension}_{guid}{fileExtension}";
            return uniqueImageName;
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
                AvailableStock = product.AvailableStock,
                ImageUrl = $"{_configuration["BlobStorageUrl"]}{product.ImageFileName}"
            };
        }
    }
}
