using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace k190324_Q1
{
    [RunInstaller(true)]
    public partial class Service1 : ServiceBase
    {
        int interval = 5;
        public Thread thread = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ThreadStart start = new ThreadStart(downloading);
                thread = new Thread(start);
                thread.Start();
            }catch (Exception){
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                if ((thread != null) & thread.IsAlive)
                    thread.Abort();   
            }catch (Exception){
                throw;
            }
        }

        public void downloading()
        {
            while (true)
            {
                using (Process downloadFile = new Process())
                {
                    string path = ConfigurationSettings.AppSettings["path"];
                    downloadFile.StartInfo.UseShellExecute = false;
                    downloadFile.StartInfo.FileName = path;
                    downloadFile.StartInfo.CreateNoWindow = true;
                    downloadFile.Start();
                }
                Thread.Sleep(interval * 60 * 1000);     //task will be executed every 5 minutes
            }
        }
    }
}
