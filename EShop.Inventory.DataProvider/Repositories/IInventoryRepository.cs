using EShop.Infrastructure.Command.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Inventory.DataProvider.Repositories
{
    public interface IInventoryRepository
    {
        Task<bool> AddStock(AllocateProduct stock);
        Task<bool> ReleaseStock(ReleaseProduct stock);
    }
}
