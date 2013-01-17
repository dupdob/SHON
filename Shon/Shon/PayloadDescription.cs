using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.IO;
using log4net;
using System.Xml.Serialization;

namespace Shon
{
    /// <summary>
    /// Stores the description of the payload
    /// </summary>
    [Serializable]
    public class PayloadDescription
    {
        #region attributes
        private string _binaryFolder;
        private string _configFile;
        private string _assembly;
        private static ILog logger = LogManager.GetLogger("Shon.Configuration");
        #endregion 

        #region Properties
        /// <summary>
        /// Assembly name
        /// </summary>
        public string AssemblyFullName
        {
            get
            {
                if (_assembly == null)
                {
                    return null;
                }
                if (!Uri.IsWellFormedUriString(_assembly, UriKind.Absolute) && !Path.IsPathRooted(_assembly))
                {
                    return Path.Combine(BinaryFolder, _assembly);
                }
                else
                {
                    return _assembly;
                }
            }

            set
            {
                this._assembly = value;
            }
        }

        public string AssemblyFileName
        {
            get
            {
                string assembly = AssemblyFullName;
                int lastSlash = assembly.LastIndexOf('/');
                if (lastSlash == -1)
                {
                    return assembly;
                }
                else
                {
                    return assembly.Substring(lastSlash + 1);
                }
            }
        }
        /// <summary>
        /// Parameter used to start guest
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// Class name hosting the service
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Binary folder 
        /// </summary>
        public string BinaryFolder
        {
            get
            {
                // folder has not been defined
                if (string.IsNullOrEmpty(_binaryFolder))
                {
                    string result;
                    // analyse assembly name forat
                    if (Uri.IsWellFormedUriString(_assembly, UriKind.Absolute))
                    {
                        // this is an URI, not a filename
                        Uri uri = new Uri(_assembly);
                        result=uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                    }
                    else
                    {
                        // build one using other informations
                        result = Path.GetDirectoryName(_assembly);
                        if (string.IsNullOrEmpty(result))
                        {
                            result = AppDomain.CurrentDomain.BaseDirectory;
                            if (Path.GetPathRoot(result) != result)
                            {
                                if (result[result.Length - 1] == Path.DirectorySeparatorChar)
                                {
                                    result = result.Substring(0, result.Length - 1);
                                }
                            }
                            logger.InfoFormat("BinaryFolder undefined, no path in assembly name,  using Shon base: ", result);
                        }
                        else
                        {
                            logger.InfoFormat("BinaryFolder undefined, parsing assembly name instead: ", result);
                        }
                    }

                    _binaryFolder = result;
                }
                return _binaryFolder;
            }
            set
            {
                _binaryFolder = value;
            }
        }
        /// <summary>
        /// Configuration File
        /// </summary>
        public string ConfigurationFile
        {
            get
            {
                if (this._configFile == null)
                {
                    return AssemblyFullName+".config";
                }
                else
                {
                    return this._configFile;
                }
            }
            set
            {
                this._configFile = value;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Load a XML configuration file
        /// </summary>
        /// <param name="fileName">name of file to load</param>
        public static PayloadDescription Load(string fileName)
        {
            XmlSerializer serializer=new XmlSerializer(typeof(PayloadDescription));
            PayloadDescription result;
            using (StreamReader reader = new StreamReader(fileName))
            {
                result = (PayloadDescription)serializer.Deserialize(reader);
            }
            return result;
        }
        /// <summary>
        /// Saves a description as a XML file
        /// </summary>
        /// <param name="description">description to save</param>
        /// <param name="fileName">name of file</param>
        public static void Save(PayloadDescription description, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PayloadDescription));
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, description);
            }
        }
        #endregion

    }
}
