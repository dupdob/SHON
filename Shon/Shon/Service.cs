using System.ServiceProcess;

namespace Shon
{
    public partial class Service : ServiceBase
    {
        private Host _host;
        public Service()
        {
            this.ServiceName = "toto";
            _host = new Host();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _host.Start();
        }

        protected override void OnStop()
        {
            _host.Stop();
        }
    }
}
