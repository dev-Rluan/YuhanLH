using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Listener
    {
        Socket listen_socket;
        //어떤 세션을 만들지
        Func<Session> _sessionFactory;
         
        // 초기화
        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
                        
            listen_socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;

          
            listen_socket.Bind(endPoint);

            listen_socket.Listen(backlog);
            for(int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(onAcceptCompleted);

                RegisterAccept(args);
            }
 
        }
        
        // accept 실행
       void RegisterAccept(SocketAsyncEventArgs args)
        {   
            //이벤트에 연결된 소켓 제거
            args.AcceptSocket = null;

            bool pending = listen_socket.AcceptAsync(args);
            if (pending == false)
            {
                onAcceptCompleted(null, args);
            }
        }

       void onAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                //_onAcceptHandler.Invoke(args.AcceptSocket); 
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            RegisterAccept(args);
        }

        public Socket Accept()
        {
            return listen_socket.Accept();
        }
    }
}
