using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service.Contract.Generators
{
    public class ProductGenerator
    {
        public List<Product> GenerateProducts()
        {
            List<Product> products = new List<Product>()
            {
                new Product
                {
                    Id = Guid.NewGuid(), 
                    SKU = "mug_301_5",
                    Name = "Coffee Mug",
                    UPC = "888414139765", 
                    Title = $"Skating - Figure Skating, Skates - Mugs",
                    Category = "Kitchen",
                    Price = (decimal)11.24, 
                    Quantity = 12,
                    ListingStatus = true,
                    AddedOn = DateTime.UtcNow.AddHours(-1), 
                    ModifiedOn = null
                },
                new Product
                {
                    Id = Guid.NewGuid(), 
                    SKU = "tm_10887_1",
                    Name = "Travel Mug",
                    UPC = "888414139765", 
                    Title = $"Cassie Peters Digital Art - Violin by Angelandspot - Travel Mugs",
                    Category = "Travel",
                    Price = (decimal)24.70, 
                    Quantity = 15,
                    ListingStatus = true,
                    AddedOn = DateTime.UtcNow.AddHours(-1), 
                    ModifiedOn = null
                }
            };
            return products;
        }
    }
}
