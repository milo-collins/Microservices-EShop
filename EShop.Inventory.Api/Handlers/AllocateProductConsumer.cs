using EShop.Infrastructure.Command.Inventory;
using EShop.Inventory.DataProvider.Services;
using MassTransit;

namespace EShop.Inventory.Api.Handlers
{
    public class AllocateProductConsumer : IConsumer<AllocateProduct>
    {
        private IInventoryService _inventoryService;
        public AllocateProductConsumer(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        public async Task Consume(ConsumeContext<AllocateProduct> context)
        {
            await _inventoryService.AddStock(context.Message);
            await Task.CompletedTask;
        }
    }
}
