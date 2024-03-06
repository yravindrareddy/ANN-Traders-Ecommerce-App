namespace ProductCatalog.Models
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int AvailableStock { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public IFormFile Image { get; set; }
    }
}
