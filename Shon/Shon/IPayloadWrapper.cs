using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shon
{
    /// <summary>
    /// Implements payload controller for the service
    /// </summary>
    internal interface iPayloadWrapper: IDisposable
    {
        /// <summary>
        /// Initialize the wrapper
        /// </summary>
        /// <param name="payloadDesc">payload description</param>
        /// <returns>success of initialization</returns>
        bool Initialize(string assemblyName, string className);
        
        /// <summary>
        /// Start the service
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the service
        /// </summary>
        void Stop();
    }
}
