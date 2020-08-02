using AlabNet.Screenshare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AlabNet.Screenshare.ClientSide;
using AlabNet.Screenshare.ServerSide;
using System.Runtime.CompilerServices;

namespace AlabNet.Frontend
{
    class Waitroom
    {
        private static Server server;
        public static void BeginConnectionOrListening(ConnectionInfo info)
        {
            if (info.Listen)
            {
                if (server == null)
                    server = new Server(info);

                server.Init();

                Console.Clear();
                Console.WriteLine($"Waiting for connections (port: {info.Port}) - - -");

                server.ListenIdentifications();
            }
            else
            {
                try
                {
                    info.ResolveTarget();
                }
                catch (SocketException)
                {
                    Console.WriteLine("error: Host resolving failed");
                    return;
                }
                
                Console.Write($"Connecting to {info.Target}:{info.Port} as '" + info.ClientNickname + "' ... ");

                var client = new Client(info);
                client.Init();
            }
        }
    }
}
