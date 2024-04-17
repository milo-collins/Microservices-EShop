using EShop.Infrastructure.Cart;
using MassTransit;

namespace EShop.Cart.Api.Handlers
{
    using EShop.Cart.DataProvider.Services;
    using EShop.Infrastructure.Cart;
    using System.Threading.Tasks;

    public class AddCartItemConsumer : IConsumer<Cart>
    {
        private ICartService _service;
        public AddCartItemConsumer(ICartService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<Cart> context)
        {
            await _service.AddCart(context.Message);
            await Task.CompletedTask;
        }
    }
}
