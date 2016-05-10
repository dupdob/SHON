

using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.Principal;
using System.ServiceProcess;

namespace Shon
{
    /// <summary>
    ///     The <see cref="ShonInstaller" /> installer.
    /// </summary>
    [RunInstaller(true)]
    // ReSharper disable once ClassNeverInstantiated.Global
    // class is instantiated through reflection
    public partial class ShonInstaller : Installer
    {
        /// <summary>
        ///     <see cref="ServiceProcessInstaller" /> instance used for the service setup.
        /// </summary>
        private readonly ServiceProcessInstaller processInstaller;

        /// <summary>
        ///     <see cref="ServiceInstaller" /> instance used for the service setup.
        /// </summary>
        private readonly ServiceInstaller serviceInstaller;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShonInstaller" /> class.
        /// </summary>
        public ShonInstaller()
        {
            this.processInstaller = new ServiceProcessInstaller();
            this.serviceInstaller = new ServiceInstaller();
            this.Installers.Add(this.processInstaller);
            this.Installers.Add(this.serviceInstaller);
            this.InitializeComponent();
        }

        private static string ServiceName { get; set; }

        /// <summary>
        ///     Installs the service
        /// </summary>
        /// <param name="serviceName">Name of the service to be installed</param>
        public static void InstallService(string serviceName)
        {
            using (var installer = GetInstaller())
            {
                ServiceName = serviceName;
                IDictionary state = new Hashtable();
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
                        // ignored (report only install errors)
                    }

                    throw;
                }
            }
        }

        public static bool HasServiceInstallationRigths()
        {
            var curIdentity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(curIdentity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        ///     Uninstall the service
        /// </summary>
        /// <param name="serviceName">Name of the service to be uninstalled</param>
        public static void UninstallService(string serviceName)
        {
            using (var installer = GetInstaller())
            {
                ServiceName = serviceName;
                IDictionary state = new Hashtable();
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

        /// <summary>
        ///     The initialization step.
        /// </summary>
        /// <param name="savedState">
        ///     The saved state.
        /// </param>
        private void Init(IDictionary savedState)
        {
            this.processInstaller.Account = ServiceAccount.LocalSystem;
            this.processInstaller.Username = null;
            this.processInstaller.Password = null;

            this.serviceInstaller.ServiceName = ServiceName;
        }

        /// <summary>
        ///     The on before install.
        /// </summary>
        /// <param name="savedState">
        ///     The saved state.
        /// </param>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            this.Init(savedState);
            base.OnBeforeInstall(savedState);
        }

        /// <summary>
        ///     The on before uninstall.
        /// </summary>
        /// <param name="savedState">
        ///     The saved state.
        /// </param>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            this.Init(savedState);
            base.OnBeforeUninstall(savedState);
        }

        /// <summary>
        ///     Gets the service installer for this assembly.
        /// </summary>
        /// <returns>
        ///     The <see cref="AssemblyInstaller" /> instance, null if none found.
        /// </returns>
        private static AssemblyInstaller GetInstaller()
        {
            var installer = new AssemblyInstaller(typeof(Service).Assembly, null) {UseNewContext = true};
            return installer;
        }
    }
}