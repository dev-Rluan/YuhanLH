using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;


namespace Server
{
   
    class Program
    {
        static Listener listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        //static void OnAcceptHandler(Socket client_socket)
        //{
        //    try
        //    {
        //        GameSession session = new GameSession();
        //        session.Start(client_socket);


        //        byte[] send_buffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
        //        session.Send(send_buffer);


        //        Thread.Sleep(1000);

        //        session.Disconnect();

        //        ////받는다
        //        //byte[] receive_buffer = new byte[1024];
        //        //int reveByte = client_socket.Receive(receive_buffer);
        //        //String receData = Encoding.UTF8.GetString(receive_buffer, 0, reveByte);
        //        //Console.WriteLine($"[fromClient] {receData}");

        //        ////보낸다
        //        //byte[] send_buffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
        //        //client_socket.Send(send_buffer);

        //        ////쫓아 낸다.
        //        //client_socket.Shutdown(SocketShutdown.Both);
        //        //client_socket.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
        static void Main(string[] args)
        {
            

            String host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);


            listener.Init(endPoint, () => { return SessionManager.Instance.Generate();  });
            Console.WriteLine("Listening...");

            //FlushRoom();
            JobTimer.Instance.Push(FlushRoom);
            //int roomTick = 0;
            //
            //
            //
            //
            //


            while (true)
            {
                //int now = System.Environment.TickCount;
                //if (roomTick < now)
                //{
                //    Room.Push(() => Room.Flush());
                //    roomTick = now + 250;
                //}
                JobTimer.Instance.Flush();

            }

        }
    }
}
