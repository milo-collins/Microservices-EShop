using EShop.Infrastructure.Command.Inventory;
using EShop.Inventory.DataProvider.Services;
using MassTransit;

namespace EShop.Inventory.Api.Handlers
{
    public class ReleaseProductConsumer : IConsumer<ReleaseProduct>
    {
        private IInventoryService _inventoryService;
        public ReleaseProductConsumer(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        public async Task Consume(ConsumeContext<ReleaseProduct> context)
        {
            await _inventoryService.ReleaseStock(context.Message);
            await Task.CompletedTask;
        }
    }
}
