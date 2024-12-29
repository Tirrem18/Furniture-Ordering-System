using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ThAmCo.Products.Data.Products
{
    public class ProductsContextFactory : IDesignTimeDbContextFactory<ProductsContext>
    {
        public ProductsContext CreateDbContext(string[] args)
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") 
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Create options builder
            var optionsBuilder = new DbContextOptionsBuilder<ProductsContext>();
            var connectionString = configuration.GetConnectionString("ProductsContext");

            // Use SQL Server
            optionsBuilder.UseSqlServer(connectionString);

            return new ProductsContext(optionsBuilder.Options);
        }
    }
}