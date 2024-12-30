using System;

namespace ThAmCo.Products.Data.Products
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
                new() { Id = 1, Name = "Seeded Test Data product 1" },
                new() { Id = 2, Name = "Seeded Test Data Test product 2" },
                new() { Id = 3, Name = "Seeded Test Data Test product 3" },
            };
            products.ForEach(p => context.Add(p));
            await context.SaveChangesAsync();
        }
    }
}
