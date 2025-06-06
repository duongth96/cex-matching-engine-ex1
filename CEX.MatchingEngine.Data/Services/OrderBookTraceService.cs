using CEX.MatchingEngine.Core.Interfaces;
using CEX.MatchingEngine.Core.Models;
using CEX.MatchingEngine.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Data.Services
{
    public class OrderBookTraceService
    {
        private readonly IOrderBook _orderBook;
        private readonly TradeRepository _tradeRepository;
        public OrderBookTraceService(
            IOrderBook orderBook,
            TradeRepository tradeRepository) 
        {
            _orderBook = orderBook ?? throw new ArgumentNullException(nameof(orderBook));
            _tradeRepository = tradeRepository ?? throw new ArgumentNullException(nameof(tradeRepository));
        }

        public async Task<IEnumerable<string>> GetData()
        {
            List<Order> orders = _orderBook.GetOrders();
            var orderIds = orders.Select(e => e.Id);
            var trades = await _tradeRepository.GetByOderIds(orderIds); // Assuming this is synchronous for simplicity
            var result = orders.Select(o =>
            {
                decimal matchedVol = 0;
                if (o.IsBuy)
                {
                    matchedVol = trades.Where(t => t.BuyOrderId == o.Id).Sum(t => t.Quantity);
                }
                else
                {
                    matchedVol = trades.Where(t => t.SellOrderId == o.Id).Sum(t => t.Quantity);

                }
                var side = o.IsBuy ? "Buy" : "Sell";
                return $"Order {o.Id} | {o.Symbol} | {side} | Price={o.Price} | OpenVolume={o.OpenVolume} | MatchedVolume={matchedVol}";
            }).ToList();

            return result;
        }
    }
}
