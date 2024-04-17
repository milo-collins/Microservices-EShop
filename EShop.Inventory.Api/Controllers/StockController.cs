using EShop.Infrastructure.Command.Inventory;
using EShop.Inventory.DataProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Inventory.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private IInventoryService _inventoryService;
        public StockController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddStocks(AllocateProduct stock)
        {
            var isAdded = await _inventoryService.AddStock(stock);
            return Accepted(isAdded);
        }

        [HttpPost]
        public async Task<IActionResult> ReleaseStocks(ReleaseProduct stock)
        {
            var isReleased = await _inventoryService.ReleaseStock(stock);
            return Accepted(isReleased);
        }
    }
}
