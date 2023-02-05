using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Windows.Forms;

namespace AlgoDemo
{
    // netsh http add urlacl url=http://+:8090/ user=%USERDOMAIN%\%USERNAME%
    internal static class Program
    {
        private static readonly AutoResetEvent _wh = new AutoResetEvent(false);
        private static void ThreadProc(Object stateInfo)
        {
            Uri baseAddress = new Uri("http://127.0.0.1:8090/algodemo");
            using (ServiceHost host = new ServiceHost(typeof(AlgoDemoService), baseAddress))
            {
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                _wh.WaitOne();

                // Close the ServiceHost.
                host.Close();
            }
        }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            ThreadPool.QueueUserWorkItem(ThreadProc);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            _wh.Set();
        }
    }
}
