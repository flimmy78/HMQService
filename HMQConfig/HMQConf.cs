using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQConfig
{
    public class HMQConf
    {
        private string ip;
        private int port;
        private string username;
        private string password;
        private Dictionary<int, int> dicTran2Car;

        public HMQConf(string _ip, int _port, string _username, string _password, Dictionary<int, int> _dicTran2Car)
        {
            ip = _ip;
            port = _port;
            username = _username;
            password = _password;
            dicTran2Car = _dicTran2Car;
        }

        public Dictionary<int, int> DicTran2Car
        {
            get { return dicTran2Car; }
        }

        public string Ip
        {
            get { return ip; }
        }

        public int Port
        {
            get { return port; }
        }

        public string Username
        {
            get { return username; }
        }

        public string Password
        {
            get { return password; }
        }

        public bool AddItem(int key, int value)
        {
            if (dicTran2Car.ContainsKey(key))
            {
                return false;
            }

            dicTran2Car.Add(key, value);
            return true;
        }
    }
}
