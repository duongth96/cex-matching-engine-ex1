using CEX.MatchingEngine.Core.Models;

namespace CEX.MatchingEngine.Demo.Models
{
    public class OrderModel
    {
        public Guid OrderId { get; set; } = Guid.Empty;
        public Guid UserId { get; set; } = Guid.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Side { get; set; } = "buy"; // "buy" or "sell"
        public string OrderType { get; set; } = "LIMIT"; // "LIMIT", "MARKET", etc.

        public Order ToOrder()
        {
            return new Order
            {
                Id = OrderId,
                UserId = UserId,
                Symbol = Symbol,
                Price = Price,
                OpenVolume = Quantity,
                RemainingVolume = Quantity,
                IsBuy = Side.Equals("buy", StringComparison.OrdinalIgnoreCase),
                OrderType = OrderType
            };
        }
    }
}
