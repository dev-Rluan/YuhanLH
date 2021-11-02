using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SessionManager
    {
       

        int _sessionId = 0;
        ClassRoom classRoom;
        /// <summary>
        /// 전체 유저 리스트
        /// </summary>
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        /// <summary>
        /// 로그인 한 전체 유저 리스트
        /// </summary>
        Dictionary<string, ClientSession> _loginSessions = new Dictionary<string, ClientSession>();
        Dictionary<string, ClassRoom> _classRoom = new Dictionary<string, ClassRoom>();

        object _lock = new object();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId;
                //큐에 저장해서 날려도됨
                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Connected : {sessionId}");

                return session;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="id">로그인 한 아이디</param>
        /// <param name="Flag">0 = 교수, 1 = 학생</param>
        public void Login(ClientSession session, string id, int Flag)
        {
            _loginSessions.Add(id, session);
            if(Flag == 0)
            {
                if (_classRoom[id] == null)
                {
                    ClassRoom cr = new ClassRoom();
                    cr.ProfessorID = id;
                    cr.Enter(session);
                    _classRoom.Add(id, cr);
                }
            }
            else
            {

            }
        }
        

        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }
    }
}
