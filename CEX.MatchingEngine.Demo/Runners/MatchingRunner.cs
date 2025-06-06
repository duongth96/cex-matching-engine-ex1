
using CEX.MatchingEngine.Core.enums;
using CEX.MatchingEngine.Core.Interfaces;
using CEX.MatchingEngine.Data.Repositories;

namespace CEX.MatchingEngine.Demo.Runners
{
    public class MatchingRunner : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;

        public MatchingRunner(IServiceScopeFactory factory)
        {
            _factory = factory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
                await Task.Delay(1, stoppingToken);
            }
        }
        private async Task DoWork(CancellationToken token)
        {
            try
            {
                using var scope = _factory.CreateScope();
                var _orderRepo = scope.ServiceProvider.GetRequiredService<OrderRepository>();
                var _matchingEngine = scope.ServiceProvider.GetRequiredService<IMatchingEngine>();
                var _orderCommingCache = scope.ServiceProvider.GetRequiredService<OrderCommingCache>();
                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<MatchingRunner>>();
                var incommingOrders = await _orderCommingCache.GetAll();
                if (incommingOrders != null && incommingOrders.Any())
                {
                    foreach (var orderId in incommingOrders)
                    {
                        var order = await _orderRepo.GetByIdAsync(orderId);
                        var side = order.IsBuy ? "Buy" : "Sell";
                        _matchingEngine.Processing(order);
                        await _orderCommingCache.Remove(orderId);
                        _logger.LogInformation("Order {side}, {OrderId} processed successfully.", side, orderId);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
