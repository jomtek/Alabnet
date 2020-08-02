using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlabNet
{
    class ConnectionInfo
    {
        public string Target { get; set; }
        public string ClientNickname { get; }
        public bool Listen;
        public int Port { get; }
        public string Password;

        public ConnectionInfo(string target, string clientNickname, bool listen, int port, string password)
        {
            this.Target = target;
            this.ClientNickname = clientNickname;
            this.Listen = listen;
            this.Port = port;
            this.Password = password;
        }

        public void ResolveTarget()
        {
            IPAddress address = Dns.GetHostAddresses(this.Target)[0];
            this.Target = address.ToString();
        }
    }
}
