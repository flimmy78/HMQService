using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HMQService.Server
{
    public class HMQManager
    {
        private Thread m_HMQManagerThread = null;


        public HMQManager()
        {

        }

        ~HMQManager()
        {
            StopWork();
        }

        public void StartWork()
        {
            m_HMQManagerThread =
                    new Thread(new ThreadStart(HMQManagerThreadProc));

            m_HMQManagerThread.Start();
        }

        public void StopWork()
        {
            m_HMQManagerThread.Join(1000);

            if (m_HMQManagerThread.IsAlive)
            {
                m_HMQManagerThread.Abort();
            }
            m_HMQManagerThread = null;
        }

        private void HMQManagerThreadProc()
        {

        }
    }
}
