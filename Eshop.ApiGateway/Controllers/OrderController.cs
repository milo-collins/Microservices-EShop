using EShop.Infrastructure.Event.Order;
using EShop.Infrastructure.Order;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.ApiGateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IRequestClient<Order> _requestClient;
        public OrderController(IRequestClient<Order> requestClient)
        {
            _requestClient= requestClient;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            try
            {
                var result = await _requestClient.GetResponse<OrderPlaced>(order);
                return Accepted(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
