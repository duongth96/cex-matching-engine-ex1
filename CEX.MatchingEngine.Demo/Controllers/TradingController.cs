using CEX.MatchingEngine.Core.Interfaces;
using CEX.MatchingEngine.Data.Repositories;
using CEX.MatchingEngine.Data.Services;
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
        private readonly OrderCommingCache _orderCommingCache;
        private readonly OrderBookTraceService _orderBookTraceService;
        public TradingController(
            ILogger<TradingController> logger,
            TradeRepository tradeRepository,
            OrderRepository orderRepository,
            OrderCommingCache orderCommingCache,
            OrderBookTraceService orderBookTraceService)
        {
            _logger = logger;
            _tradeRepository = tradeRepository;
            _orderRepository = orderRepository;
            _orderCommingCache = orderCommingCache;
            _orderBookTraceService = orderBookTraceService;

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
        [HttpGet("init")]
        public async Task<IActionResult> Init()
        {
            return Ok("Initialization complete.");
        }
        [HttpGet("order-book-trace")]
        public async Task<IActionResult> GetOrderBookTrace()
        {
            var orderBook = await _orderBookTraceService.GetData();
            if (orderBook == null)
            {
                return NotFound("Order book not found.");
            }
            return Ok(orderBook);
        }


        [HttpPost("add-order")]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel order)
        {
            if (order == null)
            {
                return BadRequest("Order cannot be null.");
            }
            var eOrder = order.ToOrder();
            await _orderRepository.CreateAsync(eOrder);
            await _orderCommingCache.Add(eOrder.Id);

            return Ok($"Order with ID {order.OrderId} placed successfully.");
        }

        
    }
}
