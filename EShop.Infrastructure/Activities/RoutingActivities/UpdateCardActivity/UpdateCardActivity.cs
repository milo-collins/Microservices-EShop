using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Activities.RoutingActivities.UpdateCardActivity
{
    // No compensation method
    public class UpdateCardActivity : IExecuteActivity<Cart.Cart>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<Cart.Cart> context)
        {
            try
            {
                var endpoint = await context.GetSendEndpoint(new Uri("rabbitmq://localhost/remove_cart"));
                await endpoint.Send(context.Arguments);

                return context.Completed();
            }
            catch(Exception ex)
            {
                throw new Exception("Error whilst updating card");
            }
        }
    }
}
