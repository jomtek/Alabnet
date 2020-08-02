using AlabNet.Frontend;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlabNet.Screenshare.ServerSide
{
    class VideoTransmitter
    {
        private NetworkStream stream;
        private readonly ConnectionInfo info;

        public VideoTransmitter(NetworkStream stream, ConnectionInfo info)
        {
            this.stream = stream;
            this.info = info;
        }

        private Bitmap TakeScreenshot()
        {
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
               Screen.PrimaryScreen.Bounds.Height,
               PixelFormat.Format32bppArgb);

            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            return bmpScreenshot;
        }

        public void Start()
        {
            BinaryWriter writer = new BinaryWriter(this.stream);
            while (true)
            {
                try
                {
                    Bitmap screenshot = TakeScreenshot();

                    byte[] buffer = Compression.DeflateCompress(Compression.ResizeConvertBMP(screenshot, 80));
                    byte[] protectedBuffer = buffer;//Cipher.Encrypt(buffer, info.Password);

                    writer.Write(buffer.Length);
                    writer.Write(buffer);

                    Thread.Sleep(50);
                } catch (IOException)
                {
                    break;
                }
            }
            
            Console.WriteLine("Current session aborted.");
            Server.active = false;
            Thread.Sleep(2500);
            Waitroom.BeginConnectionOrListening(info);
        }
    }
}
