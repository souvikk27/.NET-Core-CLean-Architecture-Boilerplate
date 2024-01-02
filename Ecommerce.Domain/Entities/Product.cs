using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public Guid CategoryId { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public decimal Price { get; set; }
        public Guid DiscountId { get; set; }
        public Discount Discount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; }
    }
}