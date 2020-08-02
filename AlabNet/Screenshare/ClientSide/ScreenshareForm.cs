using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlabNet.Screenshare.ClientSide
{
    class ScreenshareForm : Form
    {
        private PictureBox ScreensharePB;
        private ConnectionInfo info;
        private NetworkStream stream;
        private TcpClient client;

        public ScreenshareForm(ConnectionInfo info, ref NetworkStream stream, ref TcpClient client)
        {
            this.info = info;
            this.stream = stream;
            this.client = client;

            Size = new Size(900, 505);
            ShowIcon = false;
            FormBorderStyle = FormBorderStyle.Sizable;
            KeyPreview = true;

            FormClosing += new FormClosingEventHandler((object sender, FormClosingEventArgs e) => {
                Console.WriteLine("Session aborted.");
                Environment.Exit(0);
            });

            InitPictureBox();
            Text = $"Alabnet ; '{info.ClientNickname}' -> {info.Target}:{info.Port}";

            Thread listeningThread = new Thread(ListenFrames);
            listeningThread.Start();
        }

        private void InitPictureBox()
        {
            ScreensharePB = new PictureBox();
            ScreensharePB.Dock = DockStyle.Fill;
            ScreensharePB.BackColor = Color.Black;
            ScreensharePB.SizeMode = PictureBoxSizeMode.StretchImage;
            ScreensharePB.MouseDoubleClick += new MouseEventHandler(ScreensharePB_DoubleClick);
            Controls.Add(ScreensharePB);
        }

        private void ScreensharePB_DoubleClick(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            if (TopMost)
                Text += " <.>";
            else
                Text = Text.Remove(Text.Length - 4);
        }

        private void UpdateFrame(Image newFrame)
        {
            ScreensharePB.Image = newFrame;
            ScreensharePB.Refresh();
        }

        public Image ByteToImage(byte[] bytes)
        {
            using (MemoryStream memstr = new MemoryStream(bytes))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }

        // <3
        // https://stackoverflow.com/users/377618/codecat
        private Image ImageFromStream(NetworkStream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            while (true)
            {
                int ctBytes = reader.ReadInt32();

                byte[] buffer = Compression.DeflateDecompress(reader.ReadBytes(ctBytes));
                byte[] decryptedBuffer = buffer; //Cipher.Decrypt(buffer, info.Password);

                MemoryStream ms = new MemoryStream(decryptedBuffer);
                return Image.FromStream(ms);
            }
        }

        public void ListenFrames()
        {
            while (true)
            {
                if (this.stream.DataAvailable)
                {
                    Image frame = ImageFromStream(this.stream);
                    UpdateFrame(frame);
                }
            }
        }
    }
}
