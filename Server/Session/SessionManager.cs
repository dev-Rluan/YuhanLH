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
        ClassRoom cs = new ClassRoom();
        /// <summary>
        /// 교수가 아직 접속 안했을때 대기하는 큐
        /// </summary>        
        Queue<string> waitingQueue = new Queue<string>();
        public void test()
        {
            
        }
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
        public void Login(ClientSession session, string id,string pwd, int Flag)
        {
            ushort _return = 1;
            Database db = Program.db;
            
            if(Flag == 0)
            {   // 로그인 시도
                _return = (ushort)db.LoginReturn(id, pwd, Flag);
                // 리턴 값 확인( 0 = 성공, 1 = 비밀번호 불일치, 2 = 아이디 존재하지않음)
                if (_return == 0)
                {
                    // 로그인 된 세션 목록에 넣어준다
                    _loginSessions.Add(id, session);
                    // 정상적으로 로그인 되었음을 알려주는 패킷 생성
                    SP_LoginResult pkt = new SP_LoginResult();
                    // 교수의 모든 수업 리스트 가져오기
                    List<IInformation> result = db.GetScheduleList(id);
                    // 패킷에 정의된 리스트 형식대로 객체 생성
                    List<SS_LoginResult.Lecture> lecureList = null;
                    // 리스트 객체에 넣기위한 빈 Lecture객체
                    SS_LoginResult.Lecture ss_lc = new SS_LoginResult.Lecture();
                    // 데이터베이스에서 수업 정보를 가져오기 위한 객체
                    Lecture lc;
                    // result에 있는 수업번호로 수업정보 가져와서 리스트 객체어 넣어줌
                    foreach (Schedule schdule in result)
                    {
                        lc = db.GetLecture(schdule.LectureCode);
                        ss_lc.lecture_code = lc.lecture_code;
                        ss_lc.professor_id = lc.professor_id;
                        ss_lc.lecture_name = lc.lecture_name;
                        ss_lc.credit = lc.credit;
                        ss_lc.weekday = lc.week_day;
                        ss_lc.strat_time = lc.start_time;
                        ss_lc.end_time = lc.end_time;
                        lecureList.Add(ss_lc);
                    }


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
                    SP_LoginFailed loginFailed = new SP_LoginFailed();
                    loginFailed.result = _return;
                }
               
            }
            else
            {
                // 로그인 시도
                _return = (ushort)db.LoginReturn(id, pwd, Flag);
                // 리턴 값 확인( 0 = 성공, 1 = 비밀번호 불일치, 2 = 아이디 존재하지않음)
                if (_return == 0)
                {
                    // 로그인 된 세션 목록에 넣어준다
                    _loginSessions.Add(id, session);  
                    // 정상적으로 로그인 되었음을 알려주는 패킷 생성
                    SS_LoginResult pkt = new SS_LoginResult();
                    // 학생의 모든 수업 리스트 가져오기
                    List<IInformation> result = db.GetScheduleList(id);
                    // 패킷에 정의된 리스트 형식대로 객체 생성
                    List<SS_LoginResult.Lecture> lecureList = null;
                    // 리스트 객체에 넣기위한 빈 Lecture객체
                    SS_LoginResult.Lecture ss_lc = new SS_LoginResult.Lecture();
                    // 데이터베이스에서 수업 정보를 가져오기 위한 객체
                    Lecture lc;
                    // result에 있는 수업번호로 수업정보 가져와서 리스트 객체어 넣어줌
                    foreach (Schedule schdule in result)
                    {
                        lc = db.GetLecture(schdule.LectureCode);
                        ss_lc.lecture_code = lc.lecture_code;
                        ss_lc.professor_id = lc.professor_id;
                        ss_lc.lecture_name = lc.lecture_name;
                        ss_lc.credit = lc.credit;
                        ss_lc.weekday = lc.week_day;
                        ss_lc.strat_time = lc.start_time;
                        ss_lc.end_time = lc.end_time;
                        lecureList.Add(ss_lc);
                    }
                    // 생성한 패킷에 수업정보 담기
                    pkt.lectures = lecureList;
                    // 다시 전송
                    session.Send(pkt.Write());

                }
                else
                {
                    // 로그인 실패 패킷 생성
                    SS_LoginFailed pkt = new SS_LoginFailed();
                    // 왜 실패했는지 리턴값 저장후 전송
                    pkt.result = _return;
                    //
                    session.Send(pkt.Write());
                }


                // Host 찾아서 세션 호스트에 넣기 

                Schedule sc = db.GetScheduleAboutTime(DateTime.Now.ToString("HHmm"), id);                
                

                if(_classRoom[session.Host] == null)
                {
                    
                }

              
                


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
