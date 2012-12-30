using System.ComponentModel;
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

            _processInstaller.Account = ServiceAccount.LocalService;

            _serviceInstaller.ServiceName = "toto";

            Installers.Add(_processInstaller);
            Installers.Add(_serviceInstaller);
            InitializeComponent();
        }

    }
}
