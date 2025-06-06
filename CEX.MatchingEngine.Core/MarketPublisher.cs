using CEX.MatchingEngine.Core.Interfaces;
using CEX.MatchingEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Core
{
    public class MarketPublisher : IMarketPublisher
    {
        private readonly Trades _matchedTrades = new Trades();
        public void AddMatchedTrade(Trade trade)
        {
            if (trade == null) throw new ArgumentNullException(nameof(trade));
            _matchedTrades.Add(trade);
        }


        public void Clear()
        {
            _matchedTrades.Clear();
        }

        public IEnumerable<Trade> GetMatchedTrades()
        {
            return _matchedTrades.AsEnumerable();
        }
    }
}
