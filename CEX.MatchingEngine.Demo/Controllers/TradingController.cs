using CEX.MatchingEngine.Core.Interfaces;
using CEX.MatchingEngine.Data.Repositories;
using CEX.MatchingEngine.Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradingController : ControllerBase
    {
        private readonly ILogger<TradingController> _logger;
        private readonly OrderRepository _orderRepository;
        private readonly TradeRepository _tradeRepository;
        private readonly IOrderBook _orderBook;
        public TradingController(
            ILogger<TradingController> logger,
            TradeRepository tradeRepository,
            OrderRepository orderRepository,
            IOrderBook orderBook)
        {
            _logger = logger;
            _tradeRepository = tradeRepository;
            _orderRepository = orderRepository;
            _orderBook = orderBook;

        }
        [HttpGet("get-trades")]
        public async Task<IActionResult> GetTrades()
        {
            var data = await _tradeRepository.GetAllAsync();

            return Ok(data);
        }
        [HttpGet("get-orders")]
        public async Task<IActionResult> GetOrders()
        {
            var data = await _orderRepository.GetAllAsync();
            return Ok(data);
        }
        [HttpPost("add-order")]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel order)
        {
            if (order == null)
            {
                return BadRequest("Order cannot be null.");
            }
            order.OrderId = Guid.NewGuid(); // Generate a new OrderId for the order
            var eOrder = order.ToOrder();
            await _orderRepository.CreateAsync(eOrder);
            _orderBook.AddInCommingOrder(eOrder);

            return Ok($"Order with ID {order.OrderId} placed successfully.");
        }
    }
}
