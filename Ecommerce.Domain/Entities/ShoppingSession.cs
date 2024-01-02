using System.ComponentModel.DataAnnotations;


namespace Ecommerce.Domain.Entities
{
    public class ShoppingSession
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
