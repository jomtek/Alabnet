using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlabNet.Screenshare.ServerSide
{
    class TcpHeaderManage
    {
        public static byte[] ToDigitArray(int n)
        {
            string nStr = n.ToString().PadLeft(9, '0');
            return nStr.Select(o => Convert.ToByte(o)).ToArray();
        }

        public static byte[] InsertBytes(ref byte[] source, byte[] insertion)
        {
            var sourceList = new List<byte>(source);
            foreach (byte b in insertion.Reverse()) sourceList.Insert(0, b);
            return sourceList.ToArray();
        }

        public static int FindBufferSize(NetworkStream stream, ConnectionInfo info)
        {
            int bufferSize = 0;
            byte[] sizeBuffer = new byte[9];
            stream.Read(sizeBuffer, 0, sizeBuffer.Length);
            Int32.TryParse(Encoding.ASCII.GetString(sizeBuffer), out bufferSize);

            return bufferSize;
        }
    }
}
