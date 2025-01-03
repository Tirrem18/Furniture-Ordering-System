

namespace TheAmCo.Products.Services.DodgeyDealers;

public class DodgyDealersServiceFake : IDodgyDealersService
{
    private readonly TheAmCo.Products.Data.Products.Product[] _products=
    {
        new TheAmCo.Products.Data.Products.Product { Id = 8, Name = "DODGEYDEALERS Placeholder 1", Description = "Sample description 1", Price = 10.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X", Source = "DODGEYDEALERS" },
        new TheAmCo.Products.Data.Products.Product { Id = 9, Name = "DODGEYDEALERS Placeholder 2", Description = "Sample description 2", Price = 15.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y", Source = "DODGEYDEALERS" },
        new TheAmCo.Products.Data.Products.Product { Id = 1, Name = "DODGEYDEALERS EXPESNIVE 2", Description = "Sample description 2", Price = 151.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y",Source = "DODGEYDEALERS"},
        new TheAmCo.Products.Data.Products.Product { Id = 2, Name = "DODGEYDEALERS CHEAP 2", Description = "Sample description 2", Price = 1, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y",Source = "DODGEYDEALERS" }
    };
    
    public Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}