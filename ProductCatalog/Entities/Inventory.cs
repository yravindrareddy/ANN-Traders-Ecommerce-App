using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }
}
