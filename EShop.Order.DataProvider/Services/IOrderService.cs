using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Order.DataProvider.Services
{
    using EShop.Infrastructure.Order;
    public interface IOrderService
    {
        public Task<Order> GetOrder(string OrderId);
        public Task<List<Order>> GetAllOrders(string UserId);
        public Task<bool> CreateOrder(Order order);
    }
}
