using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ParseDLL;

namespace k190324_Q2
{
    public partial class Service1 : ServiceBase
    {
        int interval = 10;
        public Service1()
        {
            InitializeComponent();
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            parsing();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = interval * 60 * 1000; // 10 minutes
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            
        }

        public void parsing()
        {
            while (true)
            {
                HtmlFileManipulation.ParseData();
                Thread.Sleep(interval * 60 * 1000);     //task will be executed every 10 minutes
            }
        }
    }
}
