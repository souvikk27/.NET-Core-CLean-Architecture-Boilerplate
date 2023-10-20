using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category name is required")] 
        public string Name { get; set; }

        [Required]
        public DateTime AddedOn { get; set; }

        public DateTime? ModifiedOn { get; set;}

        public DateTime? DeletedOn { get; set; }

    }
}
