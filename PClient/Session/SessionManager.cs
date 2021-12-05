using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PClient
{
    public class SessionManager
    {
        object _lock = new object();
        ServerSession _sessions = new ServerSession();
        public void LoginSend()
        {
            lock (_lock)
            {
                CP_Login loging_packet = new CP_Login();

                loging_packet.id = "test";
                loging_packet.pwd = "test1234";
                ArraySegment<byte> segment = loging_packet.Write();
                _sessions.Send(segment);                
            }

        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                this._sessions = session;
                return session;
            }
        }

        public void CloseForm()
        {
            _sessions.Disconnect();
        }
    }
}

