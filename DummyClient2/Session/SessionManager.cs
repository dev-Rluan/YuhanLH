using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient2
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
        public void AtdStart(string a, string b)
        {
            CP_Atd pkt = new CP_Atd();
            pkt.classTime = Convert.ToInt32(a);
            pkt.week = Convert.ToInt32(b);
            _sessions.Send(pkt.Write());
            Console.WriteLine("출석 시작");
        }
        public void AtdListRequest()
        {
            CP_AtdListRequest pkt = new CP_AtdListRequest();
            _sessions.Send(pkt.Write());
        }
        public void StudentListRequest()
        {
            lock (_lock)
            {
                CP_StudentList pkt = new CP_StudentList();
                _sessions.Send(pkt.Write());
            }
        }
        public void Quiz(string quiz, string studentID)
        {
            lock (_lock)
            {
                Console.WriteLine("퀴즈 보내기");
                CP_Quiz quiz_packet = new CP_Quiz();
                quiz_packet.quiz = quiz;
                CP_Quiz.Student s = new CP_Quiz.Student();
                s.studentId = studentID;
                quiz_packet.students.Add(s);
                _sessions.Send(quiz_packet.Write());
            }
        }
        public void QuizOX(string quiz, string studentID)
        {
            lock (_lock)
            {
                Console.WriteLine("퀴즈 보내기");
                CP_QuizOX quiz_packet = new CP_QuizOX();
                quiz_packet.quiz = quiz;
                CP_QuizOX.Student s = new CP_QuizOX.Student();
                s.studentId = studentID;
                quiz_packet.students.Add(s);
                _sessions.Send(quiz_packet.Write());
            }
        }

        public void ScreenRequest(string studentID)
        {
            lock (_lock)
            {
                CP_ScreenRequest sc_request_pkt = new CP_ScreenRequest();
                CP_ScreenRequest.Student s = new CP_ScreenRequest.Student();
                s.studentId = studentID;
                sc_request_pkt.students.Add(s);
                _sessions.Send(sc_request_pkt.Write());
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

