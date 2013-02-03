using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shon
{
    /// <summary>
    /// Command line analyzing class
    /// </summary>
     public class CommandLineParser
    {
        private List<string> _values;
        private List<string> _commands;
        private Dictionary<string, string> _namedValues;
        private List<string> _valuesId;

    /// <summary>
    /// Default contructor
    /// </summary>
        public CommandLineParser()
        {
            _values = new List<string>();
            _namedValues = new Dictionary<string, string>();
            _commands = new List<string>();
            _valuesId = new List<string>();
        }
        /// <summary>
        /// Parse the command line arguments
        /// </summary>
        /// <param name="commandLine">commandline, expressed as an aray of string.</param>
        public void Parse(string[] commandLine)
        {
            for (int i = 0; i < commandLine.Length; i++)
            {
                string command = commandLine[i];
                if (command.StartsWith("/") || command.StartsWith("-"))
                {
                    command = command.Substring(1).ToLower();
                    if (_valuesId.Contains(command))
                    {
                        i++;
                        if (i < commandLine.Length)
                        {
                            _namedValues.Add(command, commandLine[i]);
                        }
                    }
                    else
                    {
                        _commands.Add(command);
                    }
                }
                else
                {
                    // value
                    _values.Add(command);
                }
            }
        }

        /// <summary>
        /// Get the number 
        /// </summary>
        public int ParametersCount
        {
            get
            {
                return _values.Count + _namedValues.Count+_commands.Count;
            }
        }
        
        /// <summary>
        /// Register a parameter requestion a value
        /// </summary>
        /// <param name="param">parameter name</param>
        public void RegisterParamValue(string param)
        {
            param = param.ToLower();
            if (_valuesId.Contains(param))
            {
                throw new InvalidOperationException("Cannot register the same option twice");
            }
            _valuesId.Add(param); ;
        }

        /// <summary>
        /// Gets the named option value
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns>Parameter value</returns>
        public string GetNamedOption(string param)
        {
            return _namedValues[param];
        }

        /// <summary>
        /// Gets the anonymous values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IList<string> Values
        {
            get
            {
                return _values;
            }
        }

        /// <summary>
        /// Determines whether the specified command is active.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        ///   <c>true</c> if the specified command is active; otherwise, <c>false</c>.
        /// </returns>
        public bool IsActive(string command)
        {
            return _commands.Contains(command);
        }
    }
}
