using CEX.MatchingEngine.Core.enums;

namespace CEX.MatchingEngine.Core.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// true if buy/bid, false if sale/ask
        /// </summary>
        public bool IsBuy { get; set; }
        public decimal OpenVolume { get; set; }
        public decimal RemainingVolume { get; set; }
        public decimal FilledVolume { get; set; }
        public decimal Price { get; set; }
        /// <summary>
        /// trigger price for stop order.
        /// </summary>
        public decimal StopPrice { get; set; }
        public DateTime? CancelOn { get; set; }
        public string? SefMatchAction { get; set; } 
        public decimal OrderAmount { get; set; }
        public string? FeeId { get; set; }
        public decimal Fee { get; set; }
        public long Sequnce { get; set; }
        public decimal Cost { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Undefined;

        /// <summary>
        /// Loại lệnh (LIMIT - Giới hạn / MARKET - Thị trường / STOP - Dừng, v.v.).
        /// </summary>
        public string OrderType { get; set; } = "LIMIT"; // Consider using enum if available
    }
}
