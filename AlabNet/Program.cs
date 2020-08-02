using AlabNet.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlabNet
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Waitroom.BeginConnectionOrListening(ArgumentParser.Parse(args));
            } catch (ArgumentParserException ex)
            {
                Console.WriteLine($"error: {ex.Message}");
            }
        }
    }
}