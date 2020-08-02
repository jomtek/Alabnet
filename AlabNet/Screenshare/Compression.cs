using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlabNet.Screenshare
{
    class Compression
    {
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; j++)
                if (encoders[j].MimeType == mimeType) return encoders[j];
            return null;
        }

        public static byte[] ResizeConvertBMP(Bitmap bitmap, int quality)
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            var stream = new MemoryStream();
            var resizedBitmap = new Bitmap(bitmap, new Size(1600, 1050));
            resizedBitmap.Save(stream, GetEncoderInfo("image/jpeg"), encoderParameters);

            byte[] buffer = stream.GetBuffer();

            bitmap.Dispose();
            stream.Close();

            return buffer;
        }

        public static byte[] DeflateCompress(byte[] data)
        {
            using (var outputS = new MemoryStream())
            {
                using (var deflateS = new DeflateStream(outputS, CompressionMode.Compress))
                {
                    deflateS.Write(data, 0, data.Length);
                }

                return outputS.ToArray();
            }
        }

        public static byte[] DeflateDecompress(byte[] data)
        {
            using (var inputS = new MemoryStream(data))
            using (var outputS = new MemoryStream())
            {
                using (var deflateS = new DeflateStream(inputS, CompressionMode.Decompress))
                {
                    deflateS.CopyTo(outputS);
                }

                return outputS.ToArray();
            }
        }
    }
}
