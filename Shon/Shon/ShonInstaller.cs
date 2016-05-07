// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShonInstaller.cs" company="Home">C.Dupuydauby
// </copyright>
// <summary>
//   Defines the ShonInstaller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;

namespace Shon
{
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    /// <summary>
    /// The <see cref="ShonInstaller"/> installer.
    /// </summary>
    [RunInstaller(true)]
    public partial class ShonInstaller : Installer
    {
        /// <summary>
        /// <see cref="ServiceProcessInstaller"/> instance used for the service setup.
        /// </summary>
        private readonly ServiceProcessInstaller processInstaller;

        /// <summary>
        /// <see cref="ServiceInstaller"/> instance used for the service setup.
        /// </summary>
        private readonly ServiceInstaller serviceInstaller;

        private static string serviceName;

        public static string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShonInstaller"/> class.
        /// </summary>
        public ShonInstaller()
        {
            this.processInstaller = new ServiceProcessInstaller();
            this.serviceInstaller = new ServiceInstaller();
            this.Installers.Add(this.processInstaller);
            this.Installers.Add(this.serviceInstaller);
            this.InitializeComponent();
        }

        /// <summary>
        /// Installs the service
        /// </summary>
        /// <param name="serviceName">Name of the service to be installed</param>
        public static void InstallService(string serviceName)
        {
            using (var installer = GetInstaller())
            {
 
                ShonInstaller.ServiceName = serviceName;
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

        /// <summary>
        /// Uninstall the service
        /// </summary>
        /// <param name="serviceName">Name of the service to be uninstalled</param>
        public static void UninstallService(string serviceName)
        {
            using (var installer = GetInstaller())
            {
                ShonInstaller.ServiceName = serviceName;
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
        /// The initialization step.
        /// </summary>
        /// <param name="savedState">
        /// The saved state.
        /// </param>
        public void Init(IDictionary savedState)
        {
            this.processInstaller.Account = ServiceAccount.LocalSystem;
            this.processInstaller.Username = null;
            this.processInstaller.Password = null;

            this.serviceInstaller.ServiceName = ServiceName;
        }

        /// <summary>
        /// The on before install.
        /// </summary>
        /// <param name="savedState">
        /// The saved state.
        /// </param>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            this.Init(savedState);
            base.OnBeforeInstall(savedState);
        }

        /// <summary>
        /// The on before uninstall.
        /// </summary>
        /// <param name="savedState">
        /// The saved state.
        /// </param>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            this.Init(savedState);
            base.OnBeforeUninstall(savedState);
        }

        /// <summary>
        /// Gets the service installer for this assembly.
        /// </summary>
        /// <returns>
        /// The <see cref="AssemblyInstaller"/> instance, null if none found.
        /// </returns>
        private static AssemblyInstaller GetInstaller()
        {
            var installer = new AssemblyInstaller(typeof(Service).Assembly, null) { UseNewContext = true };
            return installer;
        }
    }
}