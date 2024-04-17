using EShop.Infrastructure.Command.Inventory;
using EShop.Infrastructure.Inventory;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Inventory.DataProvider.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private IMongoDatabase _database;
        private IMongoCollection<StockItem> _collection;
        public InventoryRepository(IMongoDatabase database)
        {
            _database = database;
            _collection = database.GetCollection<StockItem>("stock");
        }
        public async Task<bool> AddStock(AllocateProduct stock)
        {
            List<WriteModel<StockItem>> productUpdateModel = new List<WriteModel<StockItem>>();

            foreach (var item in stock.Items)
            {
                var currentStock = await GetStock(item.ProductId);
                currentStock.Quantity += item.Quantity;

                var filter = Builders<StockItem>.Filter.Eq("productId", item.ProductId);
                var update = Builders<StockItem>.Update.Set("quantity", currentStock.Quantity);

                productUpdateModel.Add(new UpdateOneModel<StockItem>(filter, update)
                {
                    IsUpsert = true
                });
            }

            await _collection.BulkWriteAsync(productUpdateModel);
            return true;
        }

        public async Task<bool> ReleaseStock(ReleaseProduct stock)
        {
            List<WriteModel<StockItem>> productUpdateModel = new List<WriteModel<StockItem>>();

            foreach (var item in stock.Items)
            {
                var currentStock = await GetStock(item.ProductId);
                currentStock.Quantity -= item.Quantity;

                var filter = Builders<StockItem>.Filter.Eq("productId", item.ProductId);
                var update = Builders<StockItem>.Update.Set("quantity", currentStock.Quantity);

                productUpdateModel.Add(new UpdateOneModel<StockItem>(filter, update)
                {
                    IsUpsert = false
                });
            }

            await _collection.BulkWriteAsync(productUpdateModel);
            return true;
        }

        private async Task<StockItem> GetStock(string productId)
        {
            var stock = new StockItem();
            stock = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.ProductId == productId);

            if (stock is null)
                stock = new StockItem();

            return stock;
        }
    }
}
