

namespace TheAmCo.Products.Services.DodgeyDealers;

public class DodgyDealersServiceFake : IDodgyDealersService
{
    private readonly TheAmCo.Products.Data.Products.Product[] _products=
    {
        new TheAmCo.Products.Data.Products.Product { Id = 1, Name = "DODGEYDEALERS EXPENSIVE", Description = "Sample description 1", Price = 110.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X" },
        new TheAmCo.Products.Data.Products.Product { Id = 8, Name = "DODGEYDEALERS Placeholder 1", Description = "Sample description 1", Price = 10.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X" },
        new TheAmCo.Products.Data.Products.Product { Id = 9, Name = "DODGEYDEALERS Placeholder 2", Description = "Sample description 2", Price = 15.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y" },
        new TheAmCo.Products.Data.Products.Product { Id = 2, Name = "DODGEYDEALERS CHEAP", Description = "Sample description 2", Price = 1m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y" }
    };
    
    public Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}