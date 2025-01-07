using System;

namespace TheAmCo.Products.Data.Products
{
    public static class ProductsInitialiser
    {
        public static async Task SeedTestData(ProductsContext context)
        {
            if (context.Products.Any())
            {
                // db seems to be seeded
                return;
            }

            // Seed database with test data
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product Placeholder 1", Description = "Sample description 1", Price = 10.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X", Source = "Seeded Data" },
                new Product { Id = 2, Name = "Product Placeholder 2", Description = "Sample description 2", Price = 15.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y", Source = "Seeded Data" }
            };
            products.ForEach(p => context.Add(p));
            await context.SaveChangesAsync();
        }
    }
    
    
}
