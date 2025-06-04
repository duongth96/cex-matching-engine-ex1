using CEX.MatchingEngine.Core.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Data.Repositories
{
    public class TradeRepository
    {
        private readonly IMongoCollection<Trade> _trades;

        public TradeRepository(MatchingEngineDbContext context)
        {
            _trades = context.Trades;
        }

        public async Task<Trade?> GetByIdAsync(Guid tradeId)
        {
            return await _trades.Find(t => t.Id == tradeId).FirstOrDefaultAsync();
        }

        public async Task<List<Trade>> GetAllAsync()
        {
            return await _trades.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(Trade trade)
        {
            await _trades.InsertOneAsync(trade);
        }

        public async Task UpdateAsync(Trade trade)
        {
            await _trades.ReplaceOneAsync(t => t.Id == trade.Id, trade);
        }

        public async Task DeleteAsync(Guid tradeId)
        {
            await _trades.DeleteOneAsync(t => t.Id == tradeId);
        }
    }
}