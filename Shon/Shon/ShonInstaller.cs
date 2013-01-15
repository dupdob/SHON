using System.ComponentModel;
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

            _processInstaller.Account = ServiceAccount.LocalSystem;
            _processInstaller.Username = null;
            _processInstaller.Password = null;

            _serviceInstaller.ServiceName = "toto";

            Installers.Add(_processInstaller);
            Installers.Add(_serviceInstaller);
            InitializeComponent();
        }

    }
}
