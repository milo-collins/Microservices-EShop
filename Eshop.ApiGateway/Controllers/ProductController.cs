using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Event.Product;
using EShop.Infrastructure.Query.Product;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Wrap;

namespace Eshop.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IBusControl _bus;
        private IRequestClient<GetProductById> _requestClient;

        #region Policies
        private static int MAX_RETRY_COUNT = 1;
        private readonly AsyncFallbackPolicy<IActionResult> _fallbackPolicy;
        // Jittering retry strategy
        private static AsyncRetryPolicy<IActionResult> _retryPolicy = Policy<IActionResult>.Handle<Exception>().WaitAndRetryAsync(MAX_RETRY_COUNT,
                retryCount => TimeSpan.FromSeconds(Math.Pow(3, retryCount) / 3));
        // If 50% of requests in the span of 30 seconds with 2 consecutive failures, break for 1 minute.
        private static AsyncCircuitBreakerPolicy<IActionResult> _circuitBreakerPolicy = Policy<IActionResult>.Handle<Exception>()
            .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(30), 2, TimeSpan.FromMinutes(1));
        // Wraps multiple policies together
        private static AsyncPolicyWrap<IActionResult> _wrapPolicy = Policy.WrapAsync(_circuitBreakerPolicy, _retryPolicy);
        // ( max execution slots, max queue slots, action on request rejection
        private static AsyncBulkheadPolicy _bulkhead = Policy.BulkheadAsync(1, 2, (ctx) =>
        {
            throw new Exception("All slots are filled");
        });
        #endregion

        public ProductController(IBusControl bus, IRequestClient<GetProductById> request)
        {
            _bus = bus;
            _requestClient = request;
            _fallbackPolicy = Policy<IActionResult>.Handle<Exception>().FallbackAsync(Content("Jinkies! It seems we are experiencing technical difficulties. Please try again later"));
        }


        [HttpGet]
        public async Task<IActionResult> Get(string productId)
        {
            var emptyExecutionSlots = _bulkhead.BulkheadAvailableCount;
            var emptyQueueSlots = _bulkhead.QueueAvailableCount;

            //var circuitState = _circuitBreakerPolicy.CircuitState;
            //return await _bulkhead.ExecuteAsync(async () =>
            return await _fallbackPolicy.WrapAsync(_wrapPolicy).ExecuteAsync(async () =>
            {
                var prdct = new GetProductById() { ProductId = productId };
                var product = await _requestClient.GetResponse<ProductCreated>(prdct);
                return Accepted(product);
            });
        }

        // Validates Obj at controller in net 6 
        // Dto allows you to send missing information thus the missing ProductId from your form passes validation as a Dto
        // Authorize asserts only logged in users can add products
        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Add([FromForm] CreateProductDto product)
        {
            // Defines where to send the message.
            var uri = new Uri("rabbitmq://localhost/create_product");
            var endPoint = await _bus.GetSendEndpoint(uri);
            await endPoint.Send( new CreateProduct(){
                ProductDescription = product.ProductDescription, 
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice
            });

            return Accepted("Product Created");
        }
    }
}
