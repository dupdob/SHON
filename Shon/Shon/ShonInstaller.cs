using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;

namespace Shon
{
    [RunInstaller(true)]
    public partial class ShonInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller _processInstaller;
        private ServiceInstaller _serviceInstaller;

        public ShonInstaller()
        {
            _processInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();
            Installers.Add(_processInstaller);
            Installers.Add(_serviceInstaller);
            InitializeComponent();
        }

        public void Init(IDictionary savedState)
        {
            _processInstaller.Account = ServiceAccount.LocalSystem;
            _processInstaller.Username = null;
            _processInstaller.Password = null;

            _serviceInstaller.ServiceName = (string) savedState["ServiceName"];

        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            Init(savedState);
            base.OnBeforeInstall(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            Init(savedState);
            base.OnBeforeUninstall(savedState);
        }

        /// <summary>
        /// Installs the service
        /// </summary>
        public static void InstallService(string serviceName)
        {
                 using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    state["ServiceName"] = serviceName;
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch
                        {
                        }
                        throw;
                    }
                }
        }

        private static AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(
                typeof(Service).Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        /// <summary>
        /// Uninstall the service
        /// </summary>
        public static void UninstallService(string serviceName)
        {
                 using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    state["ServiceName"] = serviceName;
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
      }

    }
}
