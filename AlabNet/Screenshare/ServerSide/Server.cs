using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlabNet.Screenshare.ServerSide
{
    class Server
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Keys vKeys);

        private readonly ConnectionInfo info;
        public TcpListener Listener;
        public static bool active = false;

        public Server(ConnectionInfo info)
        {
            this.info = info;
        }

        public void Init()
        {
            if (Listener == null)
            {
                Listener = new TcpListener(IPAddress.Any, this.info.Port);
                Listener.Start();
            }

            Thread abortSessionThread = new Thread(AbortSessionLoop);
            abortSessionThread.Start();
        }

        private void AbortSessionLoop()
        {
            while (true)
            {
                var ctrl = GetAsyncKeyState(Keys.ControlKey) & 0x8000;
                var key = GetAsyncKeyState(Keys.Space) & 0x8000;
                if (ctrl != 0 && key != 0 && Server.active)
                {
                    Console.WriteLine("Current session aborted");
                    Environment.Exit(0);
                }
            }
        }

        public void ListenIdentifications()
        {
            while (true)
            {
                TcpClient client = Listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                string clientNickname = "";

                try
                {
                    clientNickname = IdentifyClient(ref stream);
                } catch (ArgumentException)
                {
                    Utils.Send("Connection refused", ref stream, info);
                    continue;
                }

                Console.Write($"'{clientNickname}' is asking to connect (y/n): ");

                if (Console.ReadLine().ToLower() != "y")
                {
                    Utils.Send("Connection refused", ref stream, info);
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Utils.ClearCurrentConsoleLine();
                    continue;
                }

                Utils.Send("Hello from Server", ref stream, info);

                Console.WriteLine();
                Console.WriteLine($@"/\ You're now screensharing with '{clientNickname}'");
                Console.WriteLine("Hint - press CTRL + SPACE at any moment to end the screenshare\n");

                Server.active = true;
                var transmitter = new VideoTransmitter(stream, info);
                transmitter.Start();

                //break;
            }
        }

        private string IdentifyClient(ref NetworkStream stream)
        {
            byte[] data = Utils.ReadAndDecryptStream(ref stream, this.info);

            string requestText =
                Encoding.ASCII.GetString(data, 0, Utils.CountNonZeroBytes(ref data));

            if (requestText.StartsWith("Hello from "))
            {
                return requestText.Split(' ')[2];
            } else
            {
                throw new ArgumentException("Invalid stream data received");
            }
        }
    }
}
