using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{

    class Program
    {
        static void Main(string[] args)
         {
            //DNS 설정
            String host = Dns.GetHostName();          
            IPHostEntry ipHost = Dns.GetHostEntry(host);            
            IPAddress ipAddr = ipHost.AddressList[0];            
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);           
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate();  }, 100);

            while (true)
            {               
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(250);
            }

        }
    }
}