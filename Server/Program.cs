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
        public static Database db = new Database();
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
            for (int i = 7771; i < 7775; i++)
            {
                IPEndPoint endPoint = new IPEndPoint(ipAddr, i);
                listener.Init(endPoint, () => { return sessionManager.Generate(); });
                Console.WriteLine($"Listening... port : {i}");
            }
            DBStart();




            //FlushRoom();
            //jobTimer.Push(FlushRoom);
            //int roomTick = 0;
            void DBStart()
            {
                IInformation information;

                information = db.GetProfessor("test");

                information.Print();

                //db.insert("stu", "'test0', 'test0', 'badman', '201507000'");
                //db.update("stu", "id = 'test5'", "stu_no = '201507000'");
                //db.delete("stu", "id = 'test5'");
            }

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
