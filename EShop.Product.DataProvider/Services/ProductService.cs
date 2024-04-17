using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Event.Product;
using EShop.Product.DataProvider.Repositories;

namespace EShop.Product.DataProvider.Services
{
    // Implements IProduct Service
    public class ProductService : IProductService
    {
        private IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductCreated> AddProduct(CreateProduct product)
        {
            // Add product to the repository (ID is added by MongoDb
            return await _repository.AddProduct(product);
        }

        public async Task<ProductCreated> GetProduct(string productId)
        {
            var product = await _repository.GetProduct(productId);
            return product;
        }

        //public async Task AddMessage(string ConsumerName, Guid? MessageId)
        //{
        //    await _repository.AddMessage(ConsumerName, MessageId);
        //}
        //
        //public async Task<bool> IsNewMessage(Guid? MessageId)
        //{
        //    return await _repository.IsNewMessage(MessageId);
        //}
    }
}
