using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace TheAmCo.Products.Data.Products
{
    public class ProductsContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        public ProductsContext(DbContextOptions<ProductsContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment == "Development")
        {
            // Use SQLite for development
            var dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dbPath = System.IO.Path.Combine(dbPath, "products.db");
            Console.WriteLine($"Using SQLite database at: {dbPath}");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
        else
        {
            // Use SQL Server for production
            var connectionString = "Server=tcp:thamco.database.windows.net,1433;Initial Catalog=ThAmCo.Products;Persist Security Info=False;User ID=thamcoboss@thamco;Password=Password1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            optionsBuilder.UseSqlServer(connectionString);
        }

        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
    }
}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(p =>
            {
                p.Property(c => c.Name).IsRequired();
            });

        }
    }
}
