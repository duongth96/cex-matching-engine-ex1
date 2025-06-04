using CEX.MatchingEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Core.Interfaces
{
    public interface IOrderBook
    {
        IEnumerable<Order> Bids { get; }
        IEnumerable<Order> Asks { get; }

        /// <summary>
        /// Adds an order to the order book.
        /// </summary>
        /// <param name="order">The order to add.</param>
        void AddOrder(Order order);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        void UpdateOrder(Order order, decimal matchQuantiy);

        /// <summary>
        /// Removes an order from the order book.
        /// </summary>
        /// <param name="orderId">The ID of the order to remove.</param>
        void RemoveOrder(Guid orderId);
        /// <summary>
        /// Gets the current state of the order book.
        /// </summary>
        /// <returns>A list of orders in the order book.</returns>
        List<Order> GetOrders();

        Order GetBestBid();
        Order GetBestAsk();
    }
}
