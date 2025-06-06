namespace CEX.MatchingEngine.Demo.Configurations
{
    public static class AppConfiguration
    {
        public static void AddAppConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<Core.Interfaces.IMarketPublisher, Core.MarketPublisher>();
            services.AddSingleton<Core.Interfaces.IOrderBook, Core.OrderBook>();
            services.AddSingleton<Core.Interfaces.IMarketPublisher, Core.MarketPublisher>();
            services.AddScoped<Core.Interfaces.IMatchingEngine, Core.MatchingEngine>();
            services.AddScoped<Data.Repositories.OrderCommingCache>();
            services.AddScoped<Data.Repositories.OrderRepository>();
            services.AddScoped<Data.Repositories.TradeRepository>();
            services.AddScoped<Data.Services.OrderBookTraceService>();


            services.AddSingleton<Runners.MatchingRunner>();
            services.AddSingleton<Runners.MatchedRunner>();
            services.AddHostedService<Runners.MatchingRunner>();
            services.AddHostedService<Runners.MatchedRunner>();
        }
    }
}
