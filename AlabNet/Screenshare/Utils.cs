using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AlabNet.Screenshare.ServerSide;

namespace AlabNet.Screenshare
{
    class Utils
    {
        public static byte[] ReadAndDecryptStream(ref NetworkStream stream, ConnectionInfo info)
        {
            int bufferSize = TcpHeaderManage.FindBufferSize(stream, info);
            Thread.Sleep(20);
            byte[] buffer = new byte[bufferSize];
            stream.Read(buffer, 0, buffer.Length);
            return buffer; //Cipher.Decrypt(buffer, info.Password);
        }

        public static void Send(ref byte[] rawBytes, ref NetworkStream stream, ConnectionInfo info)
        {
            byte[] insertion = TcpHeaderManage.ToDigitArray(rawBytes.Length);
            byte[] bytes = TcpHeaderManage.InsertBytes(ref rawBytes, insertion);//Cipher.Encrypt(rawBytes, info.Password);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Send(string text, ref NetworkStream stream, ConnectionInfo info)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            Send(ref bytes, ref stream, info);
        }

        public static int CountNonZeroBytes(ref byte[] buffer)
        {
            int nonZeroBytes = 0;
            foreach (byte b in buffer)
                if (b != 0)  nonZeroBytes++;
            return nonZeroBytes;
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static byte[] SaltBuffer(byte[] buffer)
        {
            return buffer.Concat(GlobalInfo.recognitionSalt).ToArray();
        }

        public static byte[] GetSalt(byte[] buffer)
        {
            return buffer.Skip(buffer.Count() - GlobalInfo.saltLength).ToArray();
        }

        public static bool CheckSalt(byte[] buffer)
        {
            byte[] salt = GetSalt(buffer);
            return salt.SequenceEqual(GlobalInfo.recognitionSalt);
        }

        public static byte[] RemoveSalt(byte[] buffer)
        {
            return buffer.Take(buffer.Length - GlobalInfo.saltLength).ToArray();
        }
    }
}
