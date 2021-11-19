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
        public static ClassRoom Room = new ClassRoom();
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
            String host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
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
