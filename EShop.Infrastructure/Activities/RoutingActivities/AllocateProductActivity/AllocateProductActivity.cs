using EShop.Infrastructure.Command.Inventory;
using MassTransit;
using Newtonsoft.Json;

namespace EShop.Infrastructure.Activities.RoutingActivities.AllocateProductActivity
{
    public class AllocateProductActivity : IActivity<AllocateProduct, OrderLog>
    {
        public async Task<CompensationResult> Compensate(CompensateContext<OrderLog> context)
        {
            try
            {
                var endpoint = await context.GetSendEndpoint(new Uri("rabbitmq://localhost/allocate_product"));
                var allocatedProduct = JsonConvert.DeserializeObject<AllocateProduct>(context.Message.Variables["PlacedOrder"].ToString());

                await endpoint.Send(allocatedProduct);

                // Was compensated
                return context.Compensated();
            }
            catch (Exception)
            {
                // Failed to compensate
                return context.Failed();
            }
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateProduct> context)
        {
            try
            {
                var endpoint = await context.GetSendEndpoint(new Uri("rabbitmq://localhost/release_product"));
                var order = JsonConvert.DeserializeObject<ReleaseProduct>(context.Message.Variables["PlacedOrder"].ToString());

                await endpoint.Send(order);

                return context.CompletedWithVariables<ReleaseProduct>(order, new { });
            }
            catch(Exception ex)
            {
                // Activity Faulted
                return context.Faulted();
            }
        }
    }

    public class OrderLog
    {
        public Order.Order Order { get; set; }
        public string Message { get; set; }
    }
}

