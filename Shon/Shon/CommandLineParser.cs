using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shon
{
    public class CommandLineParser
    {
        private List<string> _values;
        private List<string> _commands;
        private Dictionary<string, string> _namedValues;
        private List<string> _valuesId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        public CommandLineParser()
        {
            _values = new List<string>();
            _namedValues = new Dictionary<string, string>();
            _commands = new List<string>();
            _valuesId = new List<string>();
        }

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
    }
}
