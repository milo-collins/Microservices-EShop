using EShop.Cart.DataProvider.Services;

namespace EShop.Cart.Api.Handlers
{
    using EShop.Infrastructure.Cart;
    using MassTransit;
    using System.Threading.Tasks;

    public class RemoveCartItemConsumer : IConsumer<Cart>
    {
        private ICartService _service;
        public RemoveCartItemConsumer(ICartService service)
        {
            _service = service;
        }

        public async Task Consume(ConsumeContext<Cart> context)
        {
            await _service.RemoveCart(context.Message.UserId);
            await Task.CompletedTask;
        }
    }
}
