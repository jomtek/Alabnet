using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlabNet
{
    class GlobalInfo
    {
        public static byte[] recognitionSalt = new byte[4] { 2, 0, 2, 0 };
        public static int saltLength = 4;
    }
}
