using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    class SessionManager
    {

        Database db = new Database();

        int _sessionId = 0;
        /// <summary>
        /// 교수가 아직 접속 안했을때 대기하는 큐
        /// </summary>        
        Queue<string> waitingQueue = new Queue<string>();       
        /// <summary>
        /// 서버에 접속한 전체 유저 리스트
        /// </summary>
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        /// <summary>
        /// 로그인 한 전체 유저 리스트
        /// </summary>
        Dictionary<string, ClientSession> _loginSessions = new Dictionary<string, ClientSession>();
        /// <summary>
        /// 수업정보를 가지고있는 리스트
        /// </summary>
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
            
            if(Flag == 0)
            {   // 로그인 시도
                _return = (ushort)db.LoginReturn(id, pwd, Flag);
                // 리턴 값 확인( 0 = 성공, 1 = 비밀번호 불일치, 2 = 아이디 존재하지않음, 3 = 다른 곳에서 로그인하였습니다.)
                if (_return == 0)
                {
                    // 로그인 된 세션 목록에 넣어준다
                    if (_loginSessions[id] != null)
                    {
                        ClientSession oldSession = _loginSessions[id];
                        _loginSessions[id] = session;
                        // 외부로그인으로 연결해제 했다는걸 알려주는 패킷 
                        SP_LoginFailed lgFail_packet = new SP_LoginFailed();
                        lgFail_packet.result = 3;
                        oldSession.Send(lgFail_packet.Write());
                        return;
                    }
                    else
                    {
                        _loginSessions.Add(id, session);
                    }

                    // 정상적으로 로그인 되었음을 알려주는 패킷 생성
                    SP_LoginResult pkt = new SP_LoginResult();
                    // 교수의 모든 수업 리스트 가져오기                                                   ( 수정필요
                    List<Lecture> _lecture = db.GetLectureExistProfessor(id);
                    // 패킷에 정의된 리스트 형식대로 객체 생성
                    List<SP_LoginResult.Lecture> lecureList = null;
                    // 리스트 객체에 넣기위한 빈 Lecture객체
                    SP_LoginResult.Lecture sp_lc = new SP_LoginResult.Lecture();

                    // result에 있는 수업번호로 수업정보 가져와서 리스트 객체에 넣어줌
                    foreach (Lecture lc in _lecture)
                    {
                        sp_lc.lecture_code = lc.lecture_code;
                        sp_lc.professor_id = lc.professor_id;
                        sp_lc.lecture_name = lc.lecture_name;
                        sp_lc.credit = lc.credit;
                        sp_lc.weekday = lc.week_day;
                        sp_lc.strat_time = lc.start_time;
                        sp_lc.end_time = lc.end_time;
                        pkt.lectures.Add(sp_lc);
                    }
                    
                    // 현재 시간에 맞는 수업 정보
                    Lecture InTimeLecture = db.GetLectureExistProfessorTime(id, DateTime.Now.ToString("HHmm"));
                    string lecture_code = InTimeLecture.lecture_code;
                    // 현재 들어가야하는 방이 생성되어 있지않으면 방을 생성한다.
                    if (_classRoom[lecture_code] == null)
                    {
                        // 룸 새로 만들기
                        ClassRoom cr = new ClassRoom();
                        // 현재 유저 방에 추가
                        cr.Enter(session);
                        if(InTimeLecture != null)
                        {
                            cr.CreateRoom(InTimeLecture);
                            _classRoom.Add(lecture_code, cr);
                        }
                    }
                    else
                    {
                        _classRoom[lecture_code].Enter(session);
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
                    if (_loginSessions[id] != null)
                    {
                        ClientSession oldSession = _loginSessions[id];
                        _loginSessions[id] = session;
                        // 외부로그인으로 연결해제 했다는걸 알려주는 패킷 
                        SS_LoginFailed lgFail_packet = new SS_LoginFailed();
                        lgFail_packet.result = 3;
                        oldSession.Send(lgFail_packet.Write());
                        return;
                    }
                    else
                    {
                        _loginSessions.Add(id, session);
                    }
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
                    // 현재 시간의 수업
                    Schedule sc = db.GetScheduleExistTime(DateTime.Now.ToString("HHmm"), id);                   
                    if(sc == null)
                    {
                        // 현재 열려있는 수업방이 없다 
                        pkt.result = 1;
                    }
                    else
                    {
                        session.LectureCode = sc.LectureCode;
                        if (_classRoom[session.LectureCode] == null)
                        {
                            // 룸 새로 만들기
                            ClassRoom cr = new ClassRoom();
                            // 상세 수업 정보
                            lc = db.GetLecture(sc.LectureCode);
                            // 현재 유저 수업 방에 추가
                            cr.Enter(session);
                            if (lc != null)
                            {
                                // 수업정보 수업방 에 넣어줌
                                cr.CreateRoom(lc);
                                // 만든 수업방을 lectureCode를 키값으로 전체 방 딕셔너리에 넣어준다
                                _classRoom.Add(session.LectureCode, cr);
                            }
                        }
                        else
                        {
                            _classRoom[session.LectureCode].Enter(session);
                        }
                        // 방 열려있음
                        pkt.result = 0;
                    }
                    
                    // 다시 전송
                    session.Send(pkt.Write());

                }
                else
                {
                    // 로그인 실패 패킷 생성
                    SS_LoginFailed pkt = new SS_LoginFailed();
                    // 왜 실패했는지 리턴값 저장후 전송
                    pkt.result = _return;
                   
                    session.Send(pkt.Write());
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
