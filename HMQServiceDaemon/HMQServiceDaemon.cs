using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Net;
using System.Configuration.Install;
using System.Reflection;
using BekUtils.Util;
using log4net;
using System.Threading;
using System.Windows.Forms;

namespace HMQService
{
    class HMQServiceDaemon : ServiceBase
    {
        private System.ComponentModel.Container components = null;
        private Thread daemonThread = null;

        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public HMQServiceDaemon()
        {
            //初始化 log4net 配置信息
            log4net.Config.XmlConfigurator.Configure();

            //设置服务运行路径
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            //Log.GetLogger().InfoFormat("HMQServiceDaemon Constructor");



            InitializeComponent();
        }

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main(string[] args)
        {
                System.ServiceProcess.ServiceBase[] ServicesToRun;

                ServicesToRun = new System.ServiceProcess.ServiceBase[] { new HMQServiceDaemon() };
                System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }

        /// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            this.ServiceName = "HMQ Daemon Service";
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether
        ///    or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            //logger.InfoFormat("HMQServiceDaemon Dispose");

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //logger.InfoFormat("HMQServiceDaemon OnStart");

            StartService();

            base.OnStart(args);
        }

        /// <summary>
        /// OnStop(): Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            //logger.InfoFormat("HMQService OnStop");

            StopService();

            base.OnStop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            //logger.InfoFormat("HMQServiceDaemon OnPause");

            base.OnPause();
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            //logger.InfoFormat("HMQServiceDaemon OnContinue");

            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            //logger.InfoFormat("HMQServiceDaemon OnShutdown");

            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            //logger.InfoFormat("HMQServiceDaemon OnCustomCommand");

            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcast Status
        /// (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            //logger.InfoFormat("HMQServiceDaemon OnPowerEvent");

            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event
        ///   from a Terminal Server session.
        ///   Useful if you need to determine
        ///   when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription">The Session Change
        /// Event that occured.</param>
        protected override void OnSessionChange(
                  SessionChangeDescription changeDescription)
        {
            //logger.InfoFormat("HMQServiceDaemon OnSessionChange");

            base.OnSessionChange(changeDescription);
        }

        private void StartService()
        {
            daemonThread = new Thread(DaemonThreadProc);
            daemonThread.Start();
        }

        private void StopService()
        {
            try
            {
                daemonThread.Join(1000);
                if (daemonThread.IsAlive)
                {
                    daemonThread.Abort();
                }
            }
            catch(Exception e)
            {
                //logger.ErrorFormat("catch an error : {0}", e.Message);
            }
        }

        private void DaemonThreadProc()
        {
            //logger.InfoFormat("DaemonThreadProc begin");

            try
            {
                BekUtils.Util.Log.GetLogger().InfoFormat("222");
            }
            catch (Exception e)
            {
                //MessageBox.Show("error : {0}", e.Message);
            }

            while (true)
            {
                try
                {
                    bool bFind = false;
                    ServiceController[] services = ServiceController.GetServices();
                    foreach(ServiceController service in services)
                    {
                        if ("HMQService" == service.ServiceName)
                        {
                            bFind = true;

                            if (ServiceControllerStatus.Stopped == service.Status)
                            {
                                //logger.InfoFormat("HMQService 当前处于停止状态");

                                service.Start();
                                for (int i = 0; i < 5; i ++)
                                {
                                    if (ServiceControllerStatus.Stopped == service.Status)
                                    {
                                        Thread.Sleep(1000);
                                        service.Refresh();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (ServiceControllerStatus.Stopped == service.Status)
                                {
                                    //logger.ErrorFormat("HMQService 启动失败");
                                }
                                else
                                {
                                    //logger.InfoFormat("HMQService 启动成功");
                                }
                            }

                            break;
                        }
                    }

                    if (!bFind)
                    {
                        //logger.ErrorFormat("找不到 HMQService 服务");
                    }

                }
                catch(Exception e)
                {
                    //logger.ErrorFormat("catch an error : {0}", e.Message);
                }

                int sleepTime = 60 * 1000;
                Thread.Sleep(sleepTime);
            }
        }


    }
}