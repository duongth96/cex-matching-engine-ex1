using CEX.MatchingEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Core.Interfaces
{
    public interface IMarketPublisher
    {

        IEnumerable<Trade> GetMatchedTrades();
        /// <summary>
        /// Adds a matched order to the cache.
        /// </summary>
        /// <param name="trade">The ID of the matched order.</param>
        void AddMatchedTrade(Trade trade);

        /// <summary>
        /// Clears all matched orders from the cache.
        /// </summary>
        void Clear();
    }
}
