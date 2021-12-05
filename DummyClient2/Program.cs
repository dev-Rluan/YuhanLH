using System;
using System.Net;

namespace DummyClient2
{
    class Program
    {
        public static SessionManager sessionManager = new SessionManager();
        public static PacketManager packetManager = new PacketManager();
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("49.247.149.125");
            IPEndPoint endPoint = new IPEndPoint(ip, 7777);
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return sessionManager.Generate(); });

            while (true)
            {
                string s = Console.ReadLine();
                switch (s)
                {
                    case "Login":
                        sessionManager.LoginSend();
                        break;
                    case "Quiz":
                        Console.WriteLine("보낼 퀴즈를 입력해주세요");
                        string s2 = Console.ReadLine();
                        string s3 = Console.ReadLine();
                        sessionManager.Quiz(s2, s3);
                        
                        break;
                    case "QuizOX":
                        break;
                    case "Screen":
                        Console.WriteLine("스크린 요청할 학생의 학번을 입력해 주세요");
                        string s4 = Console.ReadLine();
                        sessionManager.ScreenRequest(s4);
                        break;
                    case "Student":
                        sessionManager.StudentListRequest();
                        break;
                    case "atd":
                        string s5 = Console.ReadLine();
                        string s6 = Console.ReadLine();
                        sessionManager.AtdStart(s5, s6);
                        break;
                    case "atdList":
                        sessionManager.AtdListRequest();
                        break;
                }
            }
        }
    }
}
