using CEX.MatchingEngine.Core.enums;
using CEX.MatchingEngine.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Core;

public class MatchingEngine : Interfaces.IMatchingEngine
{
    private readonly Interfaces.IOrderBook _orderBook;
    private readonly Interfaces.IMarketPublisher _matchedCache;
    private readonly Trades trades = new Trades();
    
    public MatchingEngine(
        Interfaces.IOrderBook orderBook,
        Interfaces.IMarketPublisher matchedCache)
    {
        _orderBook = orderBook ?? throw new ArgumentNullException(nameof(orderBook));
        _matchedCache = matchedCache;
    }

    public Trades Trades => trades;

    public async Task<bool> Processing(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var orderBook = _orderBook;
        decimal remainingQuantity = order.OpenVolume;

        // Set initial order status
        order.Status = OrderStatus.Prepared;
        while(remainingQuantity > 0)
        {
            if (order.OrderType == "LIMIT")
            {
                if (order.IsBuy)
                {
                    var bestAsk = orderBook.GetBestAsk();
                    if (bestAsk == null || bestAsk.Price > order.Price)
                        break;

                    decimal matchQuantity = Math.Min(remainingQuantity, bestAsk.RemainingVolume);

                    var trade = new Trade(
                        Math.Min(bestAsk.Price, order.Price),
                        matchQuantity,
                        bestAsk.Id,
                        order.Id,
                        order.Symbol
                    );
                    trades.Add(trade);

                    remainingQuantity -= matchQuantity;
                    orderBook.UpdateOrder(bestAsk, matchQuantity);

                    if (bestAsk.RemainingVolume == 0)
                        orderBook.RemoveOrder(bestAsk.Id);
                }
                else
                {
                    var bestBid = orderBook.GetBestBid();
                    if (bestBid == null || bestBid.Price < order.Price)
                        break;

                    decimal matchQuantity = Math.Min(remainingQuantity, bestBid.RemainingVolume);

                    var trade = new Trade(
                        Math.Min(bestBid.Price, order.Price),
                        matchQuantity,
                        bestBid.Id,
                        order.Id,
                        order.Symbol
                    );
                    trades.Add(trade);

                    remainingQuantity -= matchQuantity;
                    orderBook.UpdateOrder(bestBid, matchQuantity);

                    if (bestBid.RemainingVolume == 0)
                        orderBook.RemoveOrder(bestBid.Id);
                }
            }

            // MARKET ORDER
            if (order.OrderType == "MARKET")
            {
                if (order.IsBuy)
                {
                    var bestAsk = orderBook.GetBestAsk();
                    if (bestAsk == null)
                        break;

                    decimal matchQuantity = Math.Min(remainingQuantity, bestAsk.RemainingVolume);

                    var trade = new Trade(
                        Math.Min(bestAsk.Price, order.Price),
                        matchQuantity,
                        bestAsk.Id,
                        order.Id,
                        order.Symbol
                    );
                    trades.Add(trade);

                    remainingQuantity -= matchQuantity;
                    orderBook.UpdateOrder(bestAsk, matchQuantity);

                    if (bestAsk.RemainingVolume == 0)
                        orderBook.RemoveOrder(bestAsk.Id);
                }
                else
                {
                    var bestBid = orderBook.GetBestBid();
                    if (bestBid == null)
                        break;

                    decimal matchQuantity = Math.Min(remainingQuantity, bestBid.RemainingVolume);

                    var trade = new Trade(
                        Math.Min(bestBid.Price, order.Price),
                        matchQuantity,
                        bestBid.Id,
                        order.Id,
                        order.Symbol
                    );
                    trades.Add(trade);

                    remainingQuantity -= matchQuantity;
                    orderBook.UpdateOrder(bestBid, matchQuantity);

                    if (bestBid.RemainingVolume == 0)
                        orderBook.RemoveOrder(bestBid.Id);
                }
            }
            // Store trades
            foreach (var trade in trades)
                _matchedCache.AddMatchedTrade(trade);
        }

        // LIMIT ORDER
        
        

        // Update order status
        order.FilledVolume = order.OpenVolume - remainingQuantity;
        order.RemainingVolume = remainingQuantity;

        if (remainingQuantity == 0)
            order.Status = OrderStatus.Filled;
        else
            order.Status = OrderStatus.Matched;

        // Add remaining limit order to order book
        if (order.OrderType == "LIMIT" && remainingQuantity > 0)
            orderBook.AddOrder(order);

        // Store trades
        foreach (var trade in trades)
            _matchedCache.AddMatchedTrade(trade);

        // Return true if any trade occurred
        return await Task.FromResult(trades.Count > 0);
    }
}