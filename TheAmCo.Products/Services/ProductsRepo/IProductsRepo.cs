using System.Collections.Generic;
using System.Threading.Tasks;
using TheAmCo.Products.Data.Products;

namespace ThAmCo.Products.Services.ProductsRepo
{
    public interface IProductsRepo
    {
        Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync();
    }
}
