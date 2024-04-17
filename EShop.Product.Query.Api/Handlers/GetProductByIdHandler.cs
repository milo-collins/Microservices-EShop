using EShop.Infrastructure.Event.Product;
using EShop.Infrastructure.Query.Product;
using EShop.Product.DataProvider.Services;
using MassTransit;

namespace EShop.Product.Query.Api.Handlers
{
    [Obsolete("Now using Ocelot -> HTTP")]
    public class GetProductByIdHandler : IConsumer<GetProductById>
    {
        private IProductService _service;
        private static int EXCEPTION_COUNT = 0;
        public GetProductByIdHandler(IProductService service)
        {
            _service = service;
        }

        // Any request of type GetProductBy Id is handled / consumed in this method
        public async Task Consume(ConsumeContext<GetProductById> context)
        {
            //await Task.Delay(30000);
            if(EXCEPTION_COUNT < 4) // Just a showcase of the retry policy
            {
                EXCEPTION_COUNT++;
                throw new Exception();
            }
            // Context contains the message we send in the request
            var product = await _service.GetProduct(context.Message.ProductId);
            await context.RespondAsync<ProductCreated>(product);
        }
    }
}
