using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Shon
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static void Main(string[] args)
        {
            bool runService = true;
            CommandLineParser parser = new CommandLineParser();
            parser.Parse(args);

            if (parser.IsActive("install"))
            {
                ShonInstaller.InstallService();
                runService = false;
            }
            if (parser.IsActive("uninstall"))
            {
                ShonInstaller.UninstallService();
                runService = false;
            }
            if (runService)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new Service() 
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
