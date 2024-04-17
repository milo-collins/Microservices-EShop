using EShop.Infrastructure.Command.Product;
using EShop.Product.DataProvider.Services;
using MassTransit;
using MongoDB.Driver;

namespace EShop.Product.Api.Handlers
{
    // Handles create product command, Handles/consumes msgs for CreateProduct only
    public class CreateProductHandler : IConsumer<CreateProduct>
    {
        private IProductService _service;
        private IMongoClient _client;
        // Initialises with product service
        public CreateProductHandler(IProductService productService, IMongoClient client) 
        {
            _service = productService;
            _client = client;
        }
        // To maintain idempotency we use database transaction with IMongoClinet
        // Idempotent consumer pattern
        public async Task Consume(ConsumeContext<CreateProduct> context)
        {
            try
            {

                {
                    //Saves product to db
                    // Product data from context.Message
                    await _service.AddProduct(context.Message);

                    await Task.CompletedTask;

                    throw new System.Exception();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong when creating your product...");
            }
        }
        //public async Task Consume(ConsumeContext<CreateProduct> context)
        //{
        //    using (var session = await _client.StartSessionAsync())
        //    {
        //        try
        //        {
        //            session.StartTransaction();
        //            var isNew = await _service.IsNewMessage(context.MessageId);
        //
        //            if(isNew)
        //            {
        //                //Saves product to db
        //                // Product data from context.Message
        //                await _service.AddProduct(context.Message);
        //                await _service.AddMessage(nameof(CreateProductHandler), context.MessageId);
        //
        //                await Task.CompletedTask;
        //                
        //                await session.CommitTransactionAsync();
        //                throw new System.Exception();
        //            }
        //
        //        }
        //        catch (Exception ex)
        //        {
        //            await session.AbortTransactionAsync();
        //        }
        //    }
        //}
    }
}
