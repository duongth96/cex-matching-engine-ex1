using CEX.MatchingEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Core.Interfaces
{
    public interface IMatchingEngine
    {
        /// <summary>
        /// Get the order book for the matching engine.
        /// </summary>
        /// <returns></returns>
        Task<bool> Processing(Order order);

        Trades Trades { get; }
    }
}
