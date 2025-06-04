
using CEX.MatchingEngine.Core.enums;
using CEX.MatchingEngine.Core.Interfaces;

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
                var _orderBook = scope.ServiceProvider.GetRequiredService<IOrderBook>();
                var _matchingEngine = scope.ServiceProvider.GetRequiredService<IMatchingEngine>();
                var incommingOrders = _orderBook.GetInCommingOrders();
                if (incommingOrders != null && incommingOrders.Any())
                {
                    foreach (var order in incommingOrders)
                    {
                        if (order.Status == OrderStatus.Prepared)
                        {
                            _matchingEngine.Processing(order);
                            _orderBook.RemoveInCommingOrder(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
