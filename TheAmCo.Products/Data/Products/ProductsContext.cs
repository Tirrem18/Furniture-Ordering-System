using System;
using Microsoft.EntityFrameworkCore;

namespace ThAmCo.Products.Data.Products
{
    public class ProductsContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        public ProductsContext(DbContextOptions<ProductsContext> options)
            : base(options)
        {
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