using EShop.Infrastructure.Command.Inventory;
using EShop.Inventory.DataProvider.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Inventory.DataProvider.Services
{
    public class InventoryService : IInventoryService
    {
        private IInventoryRepository _inventoryRepository;
        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }
        public async Task<bool> AddStock(AllocateProduct stock)
        {
            return await _inventoryRepository.AddStock(stock);
        }

        public async Task<bool> ReleaseStock(ReleaseProduct stock)
        {
            return await _inventoryRepository.ReleaseStock(stock);
        }
    }
}
