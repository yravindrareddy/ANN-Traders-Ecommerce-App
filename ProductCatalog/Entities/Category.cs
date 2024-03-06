using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
