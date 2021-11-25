using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                loging_packet.id = "tjdals0231";
                loging_packet.pwd = "alfl02@!";
                ArraySegment<byte> segment = loging_packet.Write();
                _sessions.Send(segment);
            }
        }

        public void ImgSend( )
        {
            lock (_lock) 
            {
                if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
                {
                    SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
                    {
                        SClientForm.sclientform.textBox1.Text += "스크린샷 찍기";
                    });
                }
                else
                {
                    SClientForm.sclientform.textBox1.Text += "스크린샷 찍기";
                }
                byte[] img = ScreenCopy.Copy();
                SClientForm.sclientform.textBox1.Text = "스크린샷 찍기";
                CS_ScreenResult Img_packet = new CS_ScreenResult();
                Img_packet.studentId = "test";
                Img_packet.img = img;
                _sessions.Send(Img_packet.Write());
                SClientForm.sclientform.textBox1.Text = "스크린샷 보내기";
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

