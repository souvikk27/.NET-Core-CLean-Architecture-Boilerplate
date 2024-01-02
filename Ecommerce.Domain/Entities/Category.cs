using System.ComponentModel.DataAnnotations;


namespace Ecommerce.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}
        public DateTime? DeletedAt { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
