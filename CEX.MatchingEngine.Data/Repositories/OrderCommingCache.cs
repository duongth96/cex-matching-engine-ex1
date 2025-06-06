using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Data.Repositories
{
    public class OrderCommingCache
    {
        public const string CACHE_KEY = "OrderCommingCache";

        private readonly IMemoryCache _memoryCache;
        
        public OrderCommingCache(
            IMemoryCache memoryCache) 
        { 
            _memoryCache = memoryCache;
        }
        public Task Add(Guid orderId)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(orderId, out var cachedData);
                if (exists)
                {
                    _memoryCache.Set(CACHE_KEY, $"{orderId},{cachedData}", TimeSpan.FromMinutes(5));
                }
                else
                {
                    _memoryCache.Set(CACHE_KEY, orderId, TimeSpan.FromMinutes(5));
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new InvalidOperationException("Failed to add order to cache.", ex);
            }
        }
        public Task Remove(Guid orderId)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(CACHE_KEY, out var cachedData);
                if (exists)
                {
                    var orders = cachedData.ToString().Split(',').ToList();
                    orders.Remove(orderId.ToString());
                    _memoryCache.Set(CACHE_KEY, string.Join(",", orders), TimeSpan.FromMinutes(5));
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new InvalidOperationException("Failed to remove order from cache.", ex);
            }
        }
        public Task<IEnumerable<Guid>> GetAll()
        {
            try
            {
                var exists = _memoryCache.TryGetValue(CACHE_KEY, out var cachedData);
                if (exists)
                {
                    var orders = cachedData.ToString().Split(',').Select(Guid.Parse).ToList();
                    return Task.FromResult<IEnumerable<Guid>>(orders);
                }
                return Task.FromResult<IEnumerable<Guid>>(new List<Guid>());
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new InvalidOperationException("Failed to retrieve orders from cache.", ex);
            }
        }
    }
}
