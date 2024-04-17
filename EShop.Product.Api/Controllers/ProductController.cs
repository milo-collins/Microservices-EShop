using EShop.Infrastructure.Command.Product;
using EShop.Product.DataProvider.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Product.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _service { get; }

        // Inject product service to save or fetch data
        public ProductController(IProductService service) 
        {
            _service = service;
        }

        // Get Action method
        public async Task<IActionResult> Get(string productId)
        {
            // Returns service get to client
            var product = await _service.GetProduct(productId);
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateProduct product)
        {
            var addedProduct = await _service.AddProduct(new CreateProduct()
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice
            });

            return Ok(addedProduct);
        }

    }
}
