using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Common;
using EShop.Infrastructure.Event.Product;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace EShop.Product.DataProvider.Repositories
{
    // Implements IProduct Repository
    public class ProductRepository : IProductRepository
    {
        private IMongoDatabase _database;

        // Collection maps result to Create Product
        private IMongoCollection<CreateProduct> _collection;
        // Used to read and process messages
        private IMongoCollection<IdempotentConsumer> _messageCollection;

        // Connects mongo db to interface
        public ProductRepository( IMongoDatabase database) 
        {
            _database = database;

            //Accesses product collection to perform insert/fetch operations
            _collection = database.GetCollection<CreateProduct>("product");
            _messageCollection = _database.GetCollection<IdempotentConsumer>("message", null);
        }

        public async Task<ProductCreated> AddProduct(CreateProduct product)
        {
            await _collection.InsertOneAsync(product);
            return new ProductCreated { ProductId = product.ProductId, ProductName = product.ProductName, CreatedAt = DateTime.UtcNow };
        }

        public async Task<ProductCreated> GetProduct(string productId)
        {
            // Returns the first product where product id matches our queried id
            var product = _collection.AsQueryable().Where(x => x.ProductId == productId).FirstOrDefault();
            if(product == null)
            {
                throw new Exception("Product could not be found");
            }
            await Task.CompletedTask;

            //The data to return in the ProductCreated
            return new ProductCreated {  
                ProductId = product.ProductId, 
                ProductName = product.ProductName, 
                CreatedAt = DateTime.Now 
            };
        }

        // Ensures Idempotency
        //public async Task AddMessage(string ConsumerName, Guid? MessageId)
        //{
        //    // Saving message to message log collection
        //
        //    var entry = new IdempotentConsumer()
        //    {
        //        ConsumerName = ConsumerName,
        //        MessageId = MessageId
        //    };
        //
        //    await _messageCollection.InsertOneAsync(entry);
        //}
        //
        //public async Task<bool> IsNewMessage(Guid? MessageId)
        //{
        //    // Check if message exists in message collection
        //
        //    var loggedMessage = await _messageCollection.AsQueryable().Where(message => message.MessageId == MessageId).FirstOrDefaultAsync();
        //
        //    return loggedMessage is null;
        //}
    }
}
