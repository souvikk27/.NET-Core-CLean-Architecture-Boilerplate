using System.ComponentModel.DataAnnotations;


namespace Ecommerce.Domain.Entities
{
    public class Inventory
    {
        [Key]
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
