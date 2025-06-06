using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CEX.MatchingEngine.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Trade
    {
        public Trade(decimal price, decimal quantity, Guid buyOrderId, Guid sellOrderId, string symbol)
        {
            Id = Guid.NewGuid();
            Price = price;
            Quantity = quantity;
            BuyOrderId = buyOrderId;
            SellOrderId = sellOrderId;
            Timestamp = DateTime.UtcNow;
            Symbol = symbol;
        }

        [BsonId] 
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public Guid BuyOrderId { get; set; }
        public Guid SellOrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Symbol { get; set; } = string.Empty;
    }
}
