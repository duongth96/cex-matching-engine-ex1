using CEX.MatchingEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEX.MatchingEngine.Core.Interfaces
{
    public interface IRiskChecker
    {
        /// <summary>
        /// Checks if the order meets the risk criteria.
        /// </summary>
        /// <param name="order">The order to check.</param>
        /// <returns>True if the order is valid, otherwise false.</returns>
        Task<bool> IsValidOrder(Order order);
        /// <summary>
        /// Checks if the user has sufficient balance for the order.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="order">The order to check.</param>
        /// <returns>True if the user has sufficient balance, otherwise false.</returns>
        Task<bool> HasSufficientBalance(Guid userId, Order order);
    }
}
