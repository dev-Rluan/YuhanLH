using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PClient
{
    public class Connector
    {
        // 어떤 세션을 만들지
        Func<Session> _sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sessionFactory = sessionFactory;
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                //유저 토큰은 넘겨줄 값 넣어주는것( send와는 또 다름 ) 여기서는 소켓 객체를 넣어주었다
                args.UserToken = socket;
            

            RegisterConnect(args);
            }

        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            
            Socket socket = args.UserToken as Socket; 
            if(socket == null)
            {
                return; 
            }

            bool pending = socket.ConnectAsync(args);
            if(pending == false)
            {
                OnConnectCompleted(null, args);
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {            
            if(args.SocketError == SocketError.Success)
            {                
                Session session = _sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
                if (PClientForm.pclientform.lbServerConn.InvokeRequired == true)
                {
                    PClientForm.pclientform.lbServerConn.Invoke((MethodInvoker)delegate
                    {
                        PClientForm.pclientform.lbServerConn.Text = "서버연결 : success";
                    });
                }
                else
                {
                    PClientForm.pclientform.lbServerConn.Text = "서버연결 : success";
                }
            }
            else
            {
                if (PClientForm.pclientform.lbServerConn.InvokeRequired == true)
                {
                    PClientForm.pclientform.lbServerConn.Invoke((MethodInvoker)delegate
                    {
                        PClientForm.pclientform.lbServerConn.Text = $"OnConnectCompleted Fail : { args.SocketError }";
                    });
                }
                else
                {
                    PClientForm.pclientform.lbServerConn.Text = $"OnConnectCompleted Fail : { args.SocketError }";
                }
            }
        }
    }
}
