using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; } 

        [Required(ErrorMessage = "SKU is required!")]
        [MaxLength(20, ErrorMessage = "Maximum length for the SKU is 20 characters.")]
        public string? SKU { get; set; }

        [Required(ErrorMessage = "Product Name is required!")]
        [MaxLength(100, ErrorMessage = "Maximum length for the Name is 100 characters.")]
        public string? Name { get; set; }

        [MaxLength(20, ErrorMessage = "Maximum length for the UPC is 20 characters.")]
        public string? UPC { get; set; }

        [MaxLength(200, ErrorMessage = "Maximum length for the Title is 200 characters.")]
        public string? Title { get; set; }

        [MaxLength(100, ErrorMessage = "Maximum length for the Category is 100 characters.")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Price is required!")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required!")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Listing status is required!")]
        public bool ListingStatus { get; set; }

        [Required(ErrorMessage = "AddedOn date is required!")]
        public DateTime AddedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}