using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace SClient
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
            connector.Connect(endPoint, () => { return new ServerSession(); });

            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    /* //입장 문의
                     socket.Connect(endPoint);
                     Console.WriteLine($"Conneted To {socket.RemoteEndPoint.ToString()}");


                   *//*  //받는다
                     byte[] receive_buffer = new byte[1024];
                     int recvByte = socket.Receive(receive_buffer);
                     string recvData = Encoding.UTF8.GetString(receive_buffer, 0, recvByte);
                     Console.WriteLine($"[From Server] {recvData}");*//*

                     socket.Shutdown(SocketShutdown.Both);
                     socket.Close();*/
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(1000);
            }
        }
    }
}
