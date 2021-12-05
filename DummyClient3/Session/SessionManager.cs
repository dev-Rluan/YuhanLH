using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyClient3
{
    public class SessionManager
    {
        ServerSession _sessions;
        public static string Id { get; set; } 
        object _lock = new object();

        public void LoginSend(string id, string pwd)
        {
            lock (_lock)
            {
                Id = id;
                CS_Login loging_packet = new CS_Login();                
                loging_packet.id = id;
                loging_packet.pwd = pwd;
                ArraySegment<byte> segment = loging_packet.Write();
                _sessions.Send(segment);
            }
        }
        public void ask(string ask)
        {
            CS_QustionText pkt = new CS_QustionText();
            pkt.qustion = ask;            
            _sessions.Send(pkt.Write());
           
        }
        public void askAndImg(string ask)
        {
            CS_Qustion pkt = new CS_Qustion();
            //byte[] img = ScreenCopy.Copy();
            byte[] img = { 0 };
            pkt.img = img;
            pkt.qustion = ask;
            _sessions.Send(pkt.Write());
        }
        public void SCShot()
        {
            CS_QustionImg pkt = new CS_QustionImg();
            byte[] img = { 0 };
            pkt.img = img;
            _sessions.Send(pkt.Write());
        }
        public void Quiz(string result)
        {
            CS_Quiz pkt = new CS_Quiz();
            pkt.result = result;
            _sessions.Send(pkt.Write());
        }
        public void QuizOX(bool result)
        {
            CS_QuizOX pkt = new CS_QuizOX();
            pkt.result = result;
            _sessions.Send(pkt.Write());
        }
        public void Atd()
        {
            CS_AtdCheck pkt = new CS_AtdCheck();
            pkt.classTime = 1;
            pkt.week = 1;
            pkt.attr = 2;
            _sessions.Send(pkt.Write());

        }
        public void ImgSend( )
        {
            lock (_lock) 
            {               
                byte[] img = { 0 };           
                CS_ScreenResult Img_packet = new CS_ScreenResult();
                Img_packet.img = img;
                _sessions.Send(Img_packet.Write());
              
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

