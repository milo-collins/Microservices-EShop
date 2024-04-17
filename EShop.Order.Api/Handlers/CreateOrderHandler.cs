using EShop.Order.DataProvider.Services;
    using EShop.Infrastructure.Command.Order;
using System.Threading.Tasks;
using MassTransit;

namespace EShop.Order.Api.Handlers
{
    using EShop.Infrastructure.Order;

    // Queue name = create-order-handler
    public class CreateOrderHandler : IConsumer<CreateOrder>
    {
        private IOrderService _service;
        public CreateOrderHandler(IOrderService service)
        {
            _service = service;
        }

        public async Task Consume(ConsumeContext<CreateOrder> context)
        {
            var order = new Order
            {
                Items = context.Message.Items,
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId
            };

            await _service.CreateOrder(order);
        }
    }
}
