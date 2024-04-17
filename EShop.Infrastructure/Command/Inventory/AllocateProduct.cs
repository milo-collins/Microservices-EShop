using EShop.Infrastructure.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Command.Inventory
{
    public class AllocateProduct
    {
        public List<StockItem> Items { get; set; }
    }
}
