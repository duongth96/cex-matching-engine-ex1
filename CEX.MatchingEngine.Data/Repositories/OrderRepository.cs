using CEX.MatchingEngine.Core.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Data.Repositories
{
    public class OrderRepository
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderRepository(MatchingEngineDbContext context)
        {
            _orders = context.Orders;
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orders.Find(_ => true).ToListAsync();
        }

        public async Task<List<Order>> GetAllBidsAsync()
        {
            // Bids are buy orders (IsBuy == true)
            return await _orders.Find(o => o.IsBuy && o.OpenVolume>0).ToListAsync();
        }

        public async Task<List<Order>> GetAllAsksAsync()
        {
            // Asks are sell orders (IsBuy == false)
            return await _orders.Find(o => !o.IsBuy && o.OpenVolume > 0).ToListAsync();
        }

        public async Task CreateAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
        }

        public async Task UpdateAsync(Order order)
        {
            await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
        }

        public async Task DeleteAsync(Guid orderId)
        {
            await _orders.DeleteOneAsync(o => o.Id == orderId);
        }

    }
}