using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Common;
using EShop.Infrastructure.Event.Product;

namespace EShop.Product.DataProvider.Repositories
{
    public interface IProductRepository 
        //: IIdempotent
    {
        Task<ProductCreated> GetProduct(string productId);
        Task<ProductCreated> AddProduct(CreateProduct product);
    }
}
