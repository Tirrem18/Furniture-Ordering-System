

namespace TheAmCo.Products.Services.DodgeyDealers;

public class DodgyDealersServiceFake : IDodgyDealersService
{
    private readonly ProductDto[] _products=
    {
        new ProductDto { ID = 1, Name = "DODGEYDEALERS Placeholder 1", Description = "Sample description 1", Price = 10.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X" },
        new ProductDto { ID = 2, Name = "DODGEYDEALERS Placeholder 2", Description = "Sample description 2", Price = 15.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y" }
    };
    
    public Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}