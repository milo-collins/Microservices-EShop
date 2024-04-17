using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Common;
using EShop.Infrastructure.Event.Product;

namespace EShop.Product.DataProvider.Services
{
    public interface IProductService 
        //: IIdempotent
    {
        // Returns Product created object event from product ID
        Task<ProductCreated> GetProduct(string productId);

        // Takes Create product and returns product created
        Task<ProductCreated> AddProduct(CreateProduct product);
    }
}
