using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
   
    class Program
    {
        static Listener listener = new Listener();
        //public static ClassRoom Room = new ClassRoom();
        public static SessionManager sessionManager = new SessionManager();
        public static JobTimer jobTimer = new JobTimer();
        public static PacketManager packetManager = new PacketManager();

        /*  static void FlushRoom()
          {
              Room.Push(() => Room.Flush());
              jobTimer.Push(FlushRoom, 50);
          }*/
       

        static void Main(string[] args)
        {
            Console.WriteLine("프로그램 시작");
            //String host = Dns.GetHostName();
            //IPHostEntry ipHost = Dns.GetHostEntry(host);
            //IPAddress ipAddr = ipHost.AddressList[0];
            //Console.WriteLine(ipAddr);
            //IPAddress ip = IPAddress.Parse("49.247.149.125");
            //Console.WriteLine(ip);
            IPAddress ip2 = IPAddress.Parse("0.0.0.0");
            Console.WriteLine(ip2);
            IPEndPoint endPoint = new IPEndPoint(ip2, 7777);
            listener.Init(endPoint, () => { return sessionManager.Generate(); });
            Console.WriteLine($"Listening... port : 7777");                   

            //FlushRoom();
            //jobTimer.Push(FlushRoom);
            //int roomTick = 0;


            while (true)
            {
                //int now = System.Environment.TickCount;
                //if (roomTick < now)
                //{
                //    Room.Push(() => Room.Flush());
                //    roomTick = now + 250;
                //}
                //jobTimer.Flush();

            }

        }
    }
}
