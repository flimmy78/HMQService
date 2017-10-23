using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMQService.Common;

namespace HMQService.Decode
{
    public class CarManager
    {
        private int m_userId;
        private int m_kch; //考车号
        byte[] m_deChannel; //考车对应的解码通道号

        public CarManager()
        {
            m_deChannel = new byte[4];
        }

        ~CarManager()
        {
          
        }

        public void InitCar(int kch, int userid, Byte[] deChan)
        {
            m_userId = userid;
            m_kch = kch;

            m_deChannel[0] = deChan[0];
            m_deChannel[1] = deChan[1];
            m_deChannel[2] = deChan[2];
            m_deChannel[3] = deChan[3];

            Log.GetLogger().InfoFormat("初始化考车{0}, 解码通道={1}, {2}, {3}, {4}", kch, m_deChannel[0], m_deChannel[1], m_deChannel[2],
                m_deChannel[3]);
        }

    }
}
