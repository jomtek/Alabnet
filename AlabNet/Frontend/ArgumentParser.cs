using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlabNet.Frontend
{
    class ArgumentParserException : Exception
    {
        public new string Message;

        public ArgumentParserException(string message)
        {
            this.Message = message;
        }
    }

    class ArgumentParser
    {
        public static ConnectionInfo Parse(string[] args)
        {
            string target = "localhost";
            string clientNickname = "unknown";
            bool listen = false;
            string password = "";
            int port = 80;

            if (args.Length > 5)
            {
                throw new ArgumentParserException("Too much arguments specified");
            }
            else if (args.Length < 1)
            {
                throw new ArgumentParserException("No argument specified");
            }

            if (args[0] == "listen")
            {
                listen = true;
            } else if (args[0] == "connect")
            {
                listen = false;
            } else
            {
                throw new ArgumentParserException("Invalid argument specified (listen/connect)");
            }

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];

                if (listen)
                {
                    switch (i)
                    {
                        case 1:
                            try
                            {
                                port = Convert.ToInt32(arg);
                            }
                            catch (Exception)
                            {
                                throw new ArgumentParserException($"Invalid port specified '{port}'");
                            }
                            break;
                        case 2:
                            password = arg;
                            break;
                    }
                } else
                {
                    switch (i)
                    {
                        case 1:
                            target = arg;
                            break;
                        case 2:
                            try
                            {
                                port = Convert.ToInt32(arg);
                            }
                            catch (Exception)
                            {
                                throw new ArgumentParserException("Invalid port specified");
                            }
                            break;
                        case 3:
                            password = arg;
                            break;
                        case 4:
                            if (!new Regex("^[a-zA-Z0-9]+$").IsMatch(arg))
                                throw new ArgumentParserException("Invalid nickname");
                            clientNickname = arg;
                            break;
                    }
                }
            }

            return new ConnectionInfo(target, clientNickname, listen, port, password);
        } 
    }
}
