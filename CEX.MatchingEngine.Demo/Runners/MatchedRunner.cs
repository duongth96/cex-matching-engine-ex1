
using CEX.MatchingEngine.Core.Models;
using Microsoft.OpenApi.Any;

namespace CEX.MatchingEngine.Demo.Runners
{
    public class MatchedRunner : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;

        public MatchedRunner(IServiceScopeFactory factory)
        {
            _factory = factory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _factory.CreateScope();
                var _marketPublisher = scope.ServiceProvider.GetRequiredService<Core.Interfaces.IMarketPublisher>();
                var _tradeRepository = scope.ServiceProvider.GetRequiredService<Data.Repositories.TradeRepository>();
                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<MatchedRunner>>();
                var trades = _marketPublisher.GetMatchedTrades();

                List<Trade> tradesToProcess;
                lock (trades)
                {
                    tradesToProcess = trades.ToList(); // Create a copy of the trades list
                }

                foreach (var trade in tradesToProcess)
                {
                    _logger.LogInformation($"Trade {trade.Id}, Buyer: {trade.BuyOrderId}, Seller: {trade.SellOrderId}, MatchedQty: {trade.Quantity}, Price: {trade.Price}");
                    if (await _tradeRepository.GetByIdAsync(trade.Id) == null)
                    {
                        await _tradeRepository.CreateAsync(trade);
                        _logger.LogInformation($"Trade {trade.Id} saved to repository.");
                    }
                    else
                    {
                        _logger.LogWarning($"Trade {trade.Id} already exists in repository.");
                    }
                }

                _marketPublisher.Clear();
                await Task.Delay(1, stoppingToken);
            }
        }
    }
}
