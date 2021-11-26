using System;
using System.Net;

namespace DummyClient3
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
                        string s2 = Console.ReadLine();
                        string s3 = Console.ReadLine();
                        sessionManager.LoginSend(s2, s3);
                        break;
                    case "ask":
                        string s4 = Console.ReadLine();
                        sessionManager.ask(s4);
                        break;
                    case "screenshot":
                        sessionManager.SCShot();
                        break;
                    case "qustion":
                        string s5 = Console.ReadLine();
                        sessionManager.askAndImg(s5);
                        break;
                    case "atd":
                        sessionManager.Atd();
                        break;
                    case "Quiz":
                        string s6 = Console.ReadLine();
                        sessionManager.Quiz(s6);
                        break;
                    case "QuizOX":
                        string s7 = Console.ReadLine();
                        if(s7 == "O")
                        {
                            sessionManager.QuizOX(true);
                        }
                        else
                        {
                            sessionManager.QuizOX(false);
                        }

                        
                        break;

                }
            }
        }
    }
}
