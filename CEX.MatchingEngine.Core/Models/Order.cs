using CEX.MatchingEngine.Core.enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CEX.MatchingEngine.Core.Models
{
    public class Order
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// true if buy/bid, false if sale/ask
        /// </summary>
        public bool IsBuy { get; set; }
        public decimal OpenVolume { get; set; }
        public decimal RemainingVolume { get; set; } = 0;
        public decimal FilledVolume { get; set; } = 0;
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Undefined;

        /// <summary>
        /// Loại lệnh (LIMIT - Giới hạn / MARKET - Thị trường / STOP - Dừng, v.v.).
        /// </summary>
        public string OrderType { get; set; } = "LIMIT"; // Consider using enum if available
    }
}
