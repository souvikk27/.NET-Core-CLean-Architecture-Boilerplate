using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.DTO
{
    public class CategoryDto
    {
        [Key]
        public Guid Id = Guid.NewGuid();
        public Guid? ParentId { get; set; } 

        [Required(ErrorMessage = "Category name is required")]
        public string CategoryName { get; set; }

        public DateTime CreatedAt = DateTime.UtcNow;
    }
}
