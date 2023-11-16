using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Service.Contract.Generators
{
    public class CategoryGenerator
    {
        public List<Category> GenerateCategories()
        {
            List<Category> categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Home",
                    AddedOn = DateTime.Now,
                    ModifiedOn = null,
                    DeletedOn = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Kitchen",
                    AddedOn = DateTime.Now,
                    ModifiedOn = null,
                    DeletedOn = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Furniture",
                    AddedOn = DateTime.Now,
                    ModifiedOn = null,
                    DeletedOn = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Fabric",
                    AddedOn = DateTime.Now,
                    ModifiedOn = null,
                    DeletedOn = null
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Office Products",
                    AddedOn = DateTime.Now,
                    ModifiedOn = null,
                    DeletedOn = null
                }
            };
            return categories;
        }
    }
}