using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Entities
{
    public class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public ShoppingSession Session { get; set; }
        public Guid ProductId { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
    }
}
