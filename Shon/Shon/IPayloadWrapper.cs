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
        /// Gets a flag indicating if the guest has crashed, e.g. raised an unhandled exception
        /// </summary>
        bool HasCrashed { get; }
        /// <summary>
        /// Initialize the wrapper
        /// </summary>
        /// <param name="payloadDesc">payload description</param>
        /// <returns>success of initialization</returns>
        bool Initialize(string assemblyName, string className);
        
        /// <summary>
        /// Start the service
        /// </summary>
        void Start(string parameter);

        /// <summary>
        /// Stops the service
        /// </summary>
        void Stop();
    }
}
