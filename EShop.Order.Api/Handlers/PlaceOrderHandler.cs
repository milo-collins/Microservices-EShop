using MassTransit;
using System.Threading.Tasks;
namespace EShop.Order.Api.Handlers
{
    using EShop.Infrastructure.Activities.RoutingActivities.AllocateProductActivity;
    using EShop.Infrastructure.Activities.RoutingActivities.UpdateCardActivity;
    using EShop.Infrastructure.Activities.RoutingActivities.UpdateOrderActivity;
    using EShop.Infrastructure.Activities.RoutingActivities.WalletActivity;
    using EShop.Infrastructure.Cart;
    using EShop.Infrastructure.Command.Inventory;
    using EShop.Infrastructure.Event.Order;
    using EShop.Infrastructure.Order;
    using MassTransit.Courier.Contracts;

    public class PlaceOrderHandler : IConsumer<Order>, IConsumer<RoutingSlipCompleted>, IConsumer<RoutingSlipFaulted>
    {
        private IEndpointNameFormatter _endpointNameFormatter;
        public PlaceOrderHandler(IEndpointNameFormatter endpointNameFormatter)
        {
            _endpointNameFormatter = endpointNameFormatter;
        }
        public async Task Consume(ConsumeContext<Order> context)
        {
            try
            {
                var slip = CreateRoutingSlip(context);
                // Executes itenerary
                await context.Execute(slip);
            }
            catch (Exception ex) {}
        }

        public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            try
            {
                var message = context.Message;

                var requestId = context.GetVariable<Guid>(nameof(ConsumeContext.RequestId));
                var responseAddress = context.GetVariable<Uri>(nameof(ConsumeContext.ResponseAddress));
                var order = context.GetVariable<Order>("PlacedOrder");

                if (requestId.HasValue && responseAddress != null)
                {
                    var endpoint = await context.GetSendEndpoint(responseAddress);
                    await endpoint.Send(new OrderPlaced
                    {
                        OrderId = order.OrderId,
                        RequestId = requestId.Value.ToString(),
                        Message = "Order Placed."
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            try
            {
                var message = context.Message;

                var requestId = context.GetVariable<Guid>(nameof(ConsumeContext.RequestId));
                var responseAddress = context.GetVariable<Uri>(nameof(ConsumeContext.ResponseAddress));
                var order = context.GetVariable<Order>("PlacedOrder");

                if (requestId.HasValue && responseAddress != null)
                {
                    var endpoint = await context.GetSendEndpoint(responseAddress);
                    await endpoint.Send(new OrderPlaced
                    {
                        OrderId = order.OrderId,
                        RequestId = requestId.Value.ToString(),
                        Message = "Order Placement Failed."
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private RoutingSlip CreateRoutingSlip(ConsumeContext<Order> context) 
        {
            Order order = context.Message;
            var routingSlipBuilder = new RoutingSlipBuilder(new Guid(order.OrderId));

            routingSlipBuilder.AddVariable("RequestId", context.RequestId);
            routingSlipBuilder.AddVariable("ResponseAddress", context.ResponseAddress);
            routingSlipBuilder.AddVariable("PlaceOrder", order);

            // ITENERARY (name, uri, object to pass)
            // Wallet Activity
            // Args ( Activity, expected argument )
            var walletActivityQueueName = _endpointNameFormatter.ExecuteActivity<WalletActivity, TransactMoney>();
            routingSlipBuilder.AddActivity("PROCESS_WALLET", new Uri($"queue:{walletActivityQueueName}"), new { order.UserId, order.Amount });

            // Allocate Product Activity
            var allocateProductActivityQueueName = _endpointNameFormatter.ExecuteActivity<AllocateProductActivity, AllocateProduct>();
            routingSlipBuilder.AddActivity("ALLOCATE_PRODUCT", new Uri($"queue:{allocateProductActivityQueueName}"), new { });

            // Update order details
            var updateOrderActivityQueueName = _endpointNameFormatter.ExecuteActivity<UpdateOrderActivity, Order>();
            routingSlipBuilder.AddActivity("UPDATE_ORDER", new Uri($"queue:{updateOrderActivityQueueName}"), order);

            // Update cart
            var updateCartActivityQueueName = _endpointNameFormatter.ExecuteActivity<UpdateCardActivity, Cart>();
            routingSlipBuilder.AddActivity("UPDATE_CART", new Uri($"queue:{updateCartActivityQueueName}"), new { order.UserId });

            return routingSlipBuilder.Build();
        }
    }
}
