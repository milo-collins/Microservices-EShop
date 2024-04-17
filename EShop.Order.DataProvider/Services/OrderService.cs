using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Order.DataProvider.Services
{
    using EShop.Infrastructure.Order;
    using EShop.Order.DataProvider.Repositories;

    public class OrderService : IOrderService
    {
        private IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<bool> CreateOrder(Order order)
        {
            return await _orderRepository.CreateOrder(order);
        }

        public async Task<List<Order>> GetAllOrders(string UserId)
        {
            return await _orderRepository.GetAllOrders(UserId);
        }

        public async Task<Order> GetOrder(string OrderId)
        {
            return await _orderRepository.GetOrder(OrderId);
        }
    }
}
