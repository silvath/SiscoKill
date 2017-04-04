using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiscoKill
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceListenner.WriteEvent += ConsoleWrite;
            WebServiceListenner listenner = new WebServiceListenner();
            foreach (string arg in args) 
            {
                int port;
                if (Int32.TryParse(arg, out port))
                    listenner.Port = port;
                else
                    listenner.Machine = arg;
            }
            listenner.Start();
            while (listenner.IsRunning)
                System.Threading.Thread.Sleep(1000);
        }

        public static void ConsoleWrite(string message)
        {
            Console.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString("hh:mm:ss"), message));
        }
    }
}
