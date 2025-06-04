using CEX.MatchingEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Core
{
    public class Trades : List<Trade>
    {
        public void AddTradeByOrder(Order order, int quantity)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }
    }
}
