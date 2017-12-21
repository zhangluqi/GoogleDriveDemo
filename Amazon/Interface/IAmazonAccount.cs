using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Interface
{
    /// <summary>
    /// Account related part of API
    /// </summary>
    public interface IAmazonAccount
    {
        /// <summary>
        /// Request for API endpoints. Cached in memory for 3 days.
        /// </summary>
        /// <returns>Endpoint info</returns>
        Task<Endpoint> GetEndpoint();

        /// <summary>
        /// Request for drive quota info.
        /// </summary>
        /// <returns>Quota info</returns>
        Task<Quota> GetQuota();

        /// <summary>
        /// Request for drive usage info.
        /// </summary>
        /// <returns>Usage info</returns>
        Task<Usage> GetUsage();
    }
}
