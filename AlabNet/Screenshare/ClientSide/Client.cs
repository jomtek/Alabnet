using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlabNet.Screenshare.ClientSide
{
    class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        private ConnectionInfo info;
        private Form screenshareForm;

        public Client(ConnectionInfo info)
        {
            this.info = info;
        }

        public void Init()
        {
            try
            {
                this.client = new TcpClient(info.Target, info.Port);
            } catch (SocketException)
            {
                Console.Write("unreachable!");
                Environment.Exit(0);
            }

            this.stream = this.client.GetStream();
            Connect();
        }

        [STAThread]
        private void LoadScreenshareForm()
        {
            this.screenshareForm = new ScreenshareForm(info, ref this.stream, ref this.client);
            Application.EnableVisualStyles();
            Application.Run(this.screenshareForm);
        }

        public byte[] WaitForData()
        {
            byte[] receivedData;

            while (true)
            {
                if (this.stream.DataAvailable)
                {
                    receivedData = Utils.ReadAndDecryptStream(ref this.stream, this.info);
                    break;
                }
                Thread.Sleep(200);
            }

            return receivedData;
        }

        private void Connect()
        {
            Utils.Send("Hello from " + this.info.ClientNickname, ref this.stream, info);

            byte[] data = WaitForData();
            string receivedData = Encoding.ASCII.GetString(data);

            if (receivedData == "Hello from Server")
            {
                Console.Write("accepted!\n");
                LoadScreenshareForm();

            } else if (receivedData == "Connection refused")
            {
                Console.Write("refused!\n");
                Environment.Exit(0);
            } else
            {
                Console.Write("failed '" + receivedData + "':/\n");
                Environment.Exit(0);
            }

            //Utils.SendBytes()
        }
    }
}