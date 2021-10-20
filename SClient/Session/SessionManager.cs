using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SClient
{
    public class SessionManager
    {
        

        ServerSession _sessions;
        object _lock = new object();

        public void LoginSend()
        {
            lock (_lock)
            {
                CS_Login loging_packet = new CS_Login();
                
                    loging_packet.id = "test";
                    loging_packet.pwd = "1234";
                    ArraySegment<byte> segment = loging_packet.Write();
                    _sessions.Send(segment);
                
             
            }

        }
        public void ImgSend(byte[] img )
        {
            lock (_lock)
            {
                CS_ScreenResult Img_packet = new CS_ScreenResult();

                Img_packet.id = "test";

                Img_packet.img = img;
                ArraySegment<byte> segment = Img_packet.Write();
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
    }
}

