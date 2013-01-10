using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shon
{
    /// <summary>
    /// Describes various logging level
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Assist in debugging SHON
        /// </summary>
        Debug,
        /// <summary>
        /// Trace
        /// </summary>
        Trace,
        /// <summary>
        /// Information, main events
        /// </summary>
        Info,
        /// <summary>
        /// Warning, indicates a potential issue
        /// </summary>
        Warn,
        /// <summary>
        /// Error, indicates an action is required to fix the problem
        /// </summary>
        Error,
        /// <summary>
        /// Fatal error, causes early termination
        /// </summary>
        Fatal
    }

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

        /// <summary>
        /// Pop one message
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        List<Tuple<LogLevel, string>> PopMessage();
    }
}
