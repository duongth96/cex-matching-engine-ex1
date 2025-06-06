using CEX.MatchingEngine.Core.enums;
using CEX.MatchingEngine.Core.Interfaces;
using CEX.MatchingEngine.Core.Models;

namespace CEX.MatchingEngine.Core;
public class OrderBook : IOrderBook
{
    private IList<Order> _orders = new List<Order>();
    public IEnumerable<Order> Bids => _orders.Where(o => o.IsBuy && o.OpenVolume > 0);

    public IEnumerable<Order> Asks => _orders.Where(o => !o.IsBuy && o.OpenVolume > 0);

    public void AddOrder(Order order)
    {
        if(order == null)
        {
            throw new ArgumentNullException(nameof(order), "Order cannot be null.");
        }
        _orders.Add(order);
    }

    public Order GetBestAsk()
    {
        return _orders
            .Where(o => !o.IsBuy && o.OpenVolume > 0)
            .OrderBy(o => o.Price)
            .FirstOrDefault();
    }

    public Order GetBestBid()
    {
        return _orders
            .Where(o => o.IsBuy && o.OpenVolume > 0)
            .OrderByDescending(o => o.Price)
            .FirstOrDefault();
    }

    public List<Order> GetOrders()
    {
        return _orders.ToList();
    }

    public void RemoveOrder(Guid orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            _orders.Remove(order);
        }
        else
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found in the order book.");
        }
    }

    public void UpdateOrder(Order order, decimal matchQuantity)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        var existingOrder = _orders.FirstOrDefault(o => o.Id == order.Id);
        if (existingOrder == null)
            throw new KeyNotFoundException($"Order with ID {order.Id} not found in the order book.");

        // Reduce open and remaining volume, increase filled volume
        if (matchQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(matchQuantity), "Match quantity must be positive.");

        if (matchQuantity > existingOrder.OpenVolume)
            throw new InvalidOperationException("Match quantity exceeds open volume.");

        existingOrder.OpenVolume -= matchQuantity;
        existingOrder.RemainingVolume -= matchQuantity;
        existingOrder.FilledVolume += matchQuantity;

        // Update order status if needed
        if (existingOrder.OpenVolume == 0)
        {
            existingOrder.Status = OrderStatus.Filled;
        }
        else if (existingOrder.FilledVolume > 0)
        {
            existingOrder.Status = OrderStatus.Matched;
        }
    }
}