using MetaWork.WorkTime.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AutoSendMail
{
    [RunInstaller(true)]
    public partial class Service1 : ServiceBase
    {
        private Timer timer = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            this.timer.Interval = 60000; // 60 seconds
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Tick);
            this.timer.Enabled = true;
        }
        private void timer_Tick(object sender, ElapsedEventArgs e)
        {

            CongViecModel model = new CongViecModel();
            model.AuToSendMailShipInActive();
            model.AuToSendMailDoneTask();
        }

        protected override void OnStop()
        {
            this.timer.Enabled = false;
        }
    }
}
