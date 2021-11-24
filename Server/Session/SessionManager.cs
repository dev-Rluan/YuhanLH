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
        /// 교수가 아직 접속 안했을때 대기하는 리스트
        /// </summary>        
        List<ClientSession> _waitingList = new List<ClientSession>();       
        /// <summary>
        /// 서버에 접속한 전체 유저 딕셔너리
        /// </summary>
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        /// <summary>
        /// 로그인 한 전체 유저 딕셔너리
        /// </summary>
        Dictionary<string, ClientSession> _loginSessions = new Dictionary<string, ClientSession>();
        /// <summary>
        /// 수업정보를 가지고있는 딕셔너리
        /// </summary>
        Dictionary<string, ClassRoom> _classRoom = new Dictionary<string, ClassRoom>();      
        

        object _lock = new object();
        /// <summary>
        /// 서버가 접속하면 접속한 유저를 관리하기위해 세션 딕셔너리에 넣어준다.
        /// </summary>
        /// <returns></returns>
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
        /// 교수의 로그인 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        public void P_Login(ClientSession session, string id, string pwd)
        {
            int _return = 1;
            lock (_lock)
            {
                _return = (ushort)db.LoginReturn(id, pwd, 0);
                // 리턴 값 확인( 0 = 성공, 1 = 비밀번호 불일치, 2 = 아이디 존재하지않음, 3 = 다른 곳에서 로그인된 유저가 있습니다.)
                if (_return == 0)
                {
                    // 이미 로그인 된 유저가 있으면
                    if (_loginSessions.TryGetValue(id, out ClientSession s))
                    {
                        SP_LoginFailed lgFail_packet = new SP_LoginFailed();
                        lgFail_packet.result = 3;
                        //  패킷 전송
                        session.Send(lgFail_packet.Write());                        
                        return;
                    }
                    else
                    {                       
                        _loginSessions.Add(id, session);                        
                        if(_classRoom.TryGetValue(session.ID,out ClassRoom r))
                        {
                            _classRoom.Remove(session.ID);
                            SelectPullClass(session);
                        }
                        session.ID = id;
                        session.Host = id;
                        // 새로운 방 생성
                        ClassRoom room = new ClassRoom();
                        room.CreateClassRoom(session);
                        _classRoom.Add(session.ID, room);
                        // 교수 로그인 성공 패킷 전체 수업정보, 
                        SP_LoginResult pkt = new SP_LoginResult();
                        // 교수의 모든 수업 리스트 가져오기                          
                        Professor professor = db.GetProfessor(id);
                        List<Lecture> _lecture = db.GetLectureExistProfessor(professor.ProfessorId);
                        // 리스트 객체에 넣기위한 빈 Lecture객체
                        SP_LoginResult.Lecture sp_lc = new SP_LoginResult.Lecture();
                        // result에 있는 수업번호로 수업정보 가져와서 리스트 객체에 넣어줌

                        foreach (Lecture lc in _lecture)
                        {                            
                            List<Student> studentList = db.GetStudentsExistLecture(lc.lecture_code);
                            foreach(Student studentInfo in studentList)
                            {
                                SP_LoginResult.Student stu = new SP_LoginResult.Student();
                                stu.studentId = studentInfo.Id;
                                stu.studentName = studentInfo.Name;
                                stu.lectureCode = lc.lecture_code;
                                pkt.students.Add(stu);
                            }
                            sp_lc.lecture_code = lc.lecture_code;
                            sp_lc.professor_id = lc.professor_id;
                            sp_lc.lecture_name = lc.lecture_name;
                            sp_lc.credit = lc.credit;
                            sp_lc.weekday = lc.week_day;
                            sp_lc.strat_time = lc.start_time;
                            sp_lc.end_time = lc.end_time;
                            pkt.lectures.Add(sp_lc);
                        }                        
                        SelectPushClass();      
                        // 패킷 돌려주기
                        session.Send(pkt.Write());
                    }
                }
                // 비밀번호 불일치
                else if(_return == 1)
                {
                    SP_LoginFailed lgFail_packet = new SP_LoginFailed();
                    lgFail_packet.result = 1;
                    session.Send(lgFail_packet.Write());
                }
                // 아이디 없음
                else if(_return == 2)
                {
                    SP_LoginFailed lgFail_packet = new SP_LoginFailed();
                    lgFail_packet.result = 2;
                    session.Send(lgFail_packet.Write());
                }
            }
        }
        /// <summary>
        /// 학생의 로그인 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        public void S_Login(ClientSession session, string id, string pwd)
        {
            int _return = 1;
            lock (_lock)
            {
                _return = (ushort)db.LoginReturn(id, pwd, 1);
                // 리턴 값 확인( 0 = 성공, 1 = 비밀번호 불일치, 2 = 아이디 존재하지않음, 3 = 다른 곳에서 로그인된 유저가 있습니다.)
                if (_return == 0)
                {
                    // 이미 로그인 된 유저가 있으면
                    if (_loginSessions.TryGetValue(id, out ClientSession s))
                    {
                        SS_LoginFailed lgFail_packet = new SS_LoginFailed();
                        lgFail_packet.result = 3;
                        //  패킷 전송
                        session.Send(lgFail_packet.Write());
                        return;
                    }
                    else
                    {
                        Console.WriteLine("아이디 있음");
                        Student student = db.GetStudent(id);
                        Schedule schedule = db.GetScheduleExistTime(DateTime.Now.ToString("HHmm"), student.StudentId);
                        if(schedule == null)
                        {
                            Console.WriteLine("현재시간에 해당하는 수업 없음");
                            _waitingList.Add(session);
                        }
                        else
                        {
                            Console.WriteLine("현재 시간에 해당하는 수업 있음");
                            Lecture lecture = db.GetLecture(schedule.LectureCode);
                            Console.WriteLine("lecture 검색 성공");
                            session.ID = id;
                            session.Host = lecture.professor_id;
                            _loginSessions.Add(id, session);
                            // 현재 시간에 해당하는 교수가 접속해서 방을 만들었으면 (수정 요구
                            if (_classRoom.TryGetValue(lecture.professor_id, out ClassRoom room))
                            {
                                // 방에 넣어주고
                                room.Enter(session);
                            }
                            else
                            {
                                // 없으면 대기 리스트에 넣어준다.
                                _waitingList.Add(session);
                            }
                        }                      
                       

                        SS_LoginResult pkt = new SS_LoginResult();
                        List<IInformation> lectures = db.GetScheduleList(student.StudentId);
                        foreach(Lecture l in lectures)
                        {
                            SS_LoginResult.Lecture lectureResult = new SS_LoginResult.Lecture();
                            lectureResult.credit = l.credit;
                            lectureResult.end_time = l.end_time;
                            lectureResult.lecture_code = l.lecture_code;
                            lectureResult.lecture_name = l.lecture_name;
                            lectureResult.professor_id = l.professor_id;
                            lectureResult.weekday = l.week_day;
                            lectureResult.strat_time = l.start_time;
                            pkt.lectures.Add(lectureResult);
                        }
                        session.Send(pkt.Write());
                    }
                    Console.WriteLine("로그인 성공 패킷 전송 완료");
                }
                else
                {
                    Console.WriteLine("아이디없음");
                    SS_LoginFailed lgFail_packet = new SS_LoginFailed();
                    lgFail_packet.result = 2;
                    //  패킷 전송
                    session.Send(lgFail_packet.Write());
                    return;
                }
            }
        }
        /// <summary>
        /// 교수의 로그아웃 요청
        /// </summary>
        /// <param name="session"></param>
        public void P_Logout(ClientSession session)
        {
            lock (_lock)
            {
                LeaveRoom(session);
            }
        }
        /// <summary>
        /// 접속한 학생들 불러오기
        /// </summary>
        /// <param name="session"></param>
        public void StudnetList(ClientSession session)
        {
            if(_classRoom.TryGetValue(session.ID, out ClassRoom room))
            {
                room.Push(() => room.ShowStudentList());
            }
        }

        /// <summary>
        /// 학생의 로그아웃 요청
        /// </summary>
        /// <param name="session"></param>
        public void S_Logout(ClientSession session)
        {
            lock (_lock)
            {
                if(_loginSessions.TryGetValue(session.ID , out ClientSession s))
                {
                    _loginSessions.Remove(session.ID);
                    // 로그아웃 패킷 보낼 곳
                }
            }
        }
        /// <summary>
        /// 교수의 화면 캡쳐 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void ScreenRequest(ClientSession session, CP_ScreenRequest packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(session.ID, out ClassRoom room))
                {
                    room.Push(() => room.Img_Request(session, packet));
                }
            }            
        }        
        public void ScreenResult(ClientSession session, CS_ScreenResult packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(GetProfessorId(session), out ClassRoom room))
                {
                    room.Push(() => room.Img_Send(packet.img, packet.studentId));
                }
            }          
        }
        /// <summary>
        /// 방찾아서 퀴즈 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QuizRequest(ClientSession session, CP_Quiz packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(session.ID, out ClassRoom room))
                {
                    room.Push(() => room.Quiz_Request(packet));
                }
            }
        }
        /// <summary>
        /// 방 찾아서 OX 퀴즈 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QuizOXRequest(ClientSession session, CP_QuizOX packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(session.ID, out ClassRoom room))
                {
                    room.Push(() => room.QuizOX_Request(packet));
                }
            }
        }
        /// <summary>
        /// 학생의 질문에 대한 교수의 답변이 돌아왔을때 호출
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QResult(ClientSession session, CP_QResult packet)
        {
            lock (_lock)
            {
                if(_classRoom.TryGetValue(session.ID, out ClassRoom room))
                {
                    room.Push(() => room.QResult(packet));
                }
            }
        }


        /// <summary>
        /// 수업이 종료했음을 알리는 패킷 수정필요
        /// </summary>
        /// <param name="session"></param> 
        public void EndOfClass(ClientSession session)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(session.ID, out ClassRoom room))
                {
                    SelectPullClass(session);
                    room.Push(() => room.ClearRoom(session));
                    SelectPushClass();
                }
            }           
        }
        /// <summary>
        /// 교수의 출석 체크 요청
        /// </summary>
        /// <param name="session"></param>
        public void AtdClass(ClientSession session, CP_Atd packet)
        {
            lock (_lock)
            {
                if(packet.classTime == 1)
                {
                    Lecture lecture = db.GetLectureExistProfessorTime(session.ID, DateTime.Now.ToString("HHmm"));
                    List<Student> students = db.GetStudentsExistLecture(lecture.lecture_code);
                    foreach (Student s in students)
                    {
                        db.PR_Attendance(s.Id, lecture.lecture_code, packet.week);
                    }                    
                }

                if(_classRoom.TryGetValue(session.ID, out ClassRoom room))
                {
                    room.Push(() => room.AttRequest(packet));
                }               
            }
        }

        /// <summary>
        /// 학생의 퀴즈 답 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QuizResult(ClientSession session, CS_Quiz packet)
        {
            lock (_lock)
            {
                if(_classRoom.TryGetValue(GetProfessorId(session), out ClassRoom room))
                {
                    room.Push(() => room.Quiz_Result(session, packet));
                }                
            }
        }
        /// <summary>
        /// 학생의 OX 퀴즈 답 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QuizOXResult(ClientSession session, CS_QuizOX packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(GetProfessorId(session), out ClassRoom room))
                {
                    room.Push(() => room.QuizOX_Result(session, packet));
                }
            }
        }
        /// <summary>
        /// 학생의 질문(Text)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QustionTextRequest(ClientSession session, CS_QustionText packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(GetProfessorId(session), out ClassRoom room))
                {
                    room.Push(() => room.QustionText(session, packet));
                }
            } 
        }
        /// <summary>
        /// 학생의 질문(Img)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QustionImgRequest(ClientSession session, CS_QustionImg packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(GetProfessorId(session), out ClassRoom room))
                {
                    room.Push(() => room.QustionImg(session, packet));
                }
            }            
        }
        /// <summary>
        /// 학생의 질문
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QustionRequest(ClientSession session, CS_Qustion packet)
        {
            lock (_lock)
            {
                if (_classRoom.TryGetValue(GetProfessorId(session), out ClassRoom room))
                {
                    room.Push(() => room.Qustion(session, packet));
                }
            }           
        }
        /// <summary>
        /// 학생의 출석 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void AtdResult(ClientSession session, CS_AtdCheck packet)
        {
            lock (_lock)
            {
                db.ST_Attendance(session.ID, GetLectureCode(session), packet.week, packet.classTime, packet.Attr);
                if(_classRoom.TryGetValue(session.Host, out ClassRoom room))
                {
                    room.Push(() => room.AtdResult(session, packet));
                }
            }
        }


       
        /*   public void Login(ClientSession session, string id,string pwd, int Flag)
           {
               lock(_lock){            
               ushort _return = 1;
                   if (Flag == 0)
                   {   // 로그인 시도
                       _return = (ushort)db.LoginReturn(id, pwd, Flag);
                       // 리턴 값 확인( 0 = 성공, 1 = 비밀번호 불일치, 2 = 아이디 존재하지않음, 3 = 다른 곳에서 로그인하였습니다.)
                       if (_return == 0)
                       {
                           // 로그인 된 세션 목록에 넣어준다
                           ClientSession s = null;
                           if (_loginSessions.TryGetValue(id, out s))
                           {
                               ClientSession oldSession = s;
                               _loginSessions[id] = session;
                               // 외부로그인으로 연결해제 했다는걸 알려주는 패킷 
                               SP_LoginFailed lgFail_packet = new SP_LoginFailed();
                               lgFail_packet.result = 3;                    
                               //  패킷 전송
                               oldSession.Send(lgFail_packet.Write());
                               // 접속되어있는 수업방에서 세션 삭제
                               _classRoom[oldSession.Room._lecture.lecture_code].Leave(oldSession);
                               return;
                           }
                           else
                           {
                               _loginSessions.Add(id, session);
                           }

                           // 현재 시간에 맞는 수업 정보
                           Lecture InTimeLecture = db.GetLectureExistProfessorTime(id, DateTime.Now.ToString("HHmm"));
                           string lecture_code = InTimeLecture.lecture_code;
                           // 현재 들어가야하는 방이 생성되어 있지않으면 방을 생성한다.
                           if (_classRoom[lecture_code] == null)
                           {
                               // 수업 방 새로 만들기
                               ClassRoom cr = new ClassRoom();
                               // 현재 유저 방에 추가
                               cr.Enter(session);
                               if (InTimeLecture != null)
                               {
                                   cr.CreateRoom(InTimeLecture);
                                   _classRoom.Add(lecture_code, cr);
                               }
                           }
                           else
                           {
                               _classRoom[lecture_code].Enter(session);
                           }

                           // 정상적으로 로그인 되었음을 알려주는 패킷 생성
                           SP_LoginResult pkt = new SP_LoginResult();
                           // 교수의 모든 수업 리스트 가져오기                                                   
                           List<Lecture> _lecture = db.GetLectureExistProfessor(id);
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
                               // 학생에게 패킷 보내기
                               oldSession.Send(lgFail_packet.Write());
                               // 이전의 계정은 들어가있는 수업방에서 삭제
                               oldSession.Room.Leave(oldSession);
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
                           if (sc == null)
                           {
                               // 현재 열려있는 수업방이 없다 
                               pkt.result = 1;
                           }
                           else
                           {
                               //session.LectureCode = sc.LectureCode;
                               //if (_classRoom[session.LectureCode] == null)
                               //{
                               //    // 룸 새로 만들기
                               //    ClassRoom cr = new ClassRoom();
                               //    // 상세 수업 정보
                               //    lc = db.GetLecture(sc.LectureCode);
                               //    // 현재 유저 수업 방에 추가
                               //    cr.Enter(session);
                               //    if (lc != null)
                               //    {
                               //        // 수업정보 수업방 에 넣어줌
                               //        cr.CreateRoom(lc);
                               //        // 만든 수업방을 lectureCode를 키값으로 전체 방 딕셔너리에 넣어준다
                               //        _classRoom.Add(session.LectureCode, cr);
                               //    }
                               //}
                               //else
                               //{
                               //    _classRoom[session.LectureCode].Enter(session);
                               //}
                               //// 방 열려있음
                               //pkt.result = 0;
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
           }        */


     
        /* public void Logout(ClientSession session)
         {
             lock (_lock)
             {
                 _classRoom[session.Room._lecture.lecture_code].Leave(session);
                 _loginSessions.Remove(session.ID);
             }
         }*/


        /// <summary>
        /// 현재 시간에 맞는 과목 번호
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public string GetLectureCode(ClientSession session)
        {
            Schedule schedule = db.GetScheduleExistTime(DateTime.Now.ToString("HHmm"), session.ID);
            return schedule.LectureCode;
        }
       /// <summary>
       /// 현재 시간에 맞는 교수 아이디 가져오기
       /// </summary>
       /// <param name="session"></param>
       /// <returns></returns>
        public string GetProfessorId(ClientSession session)
        {
            Lecture lecture = db.GetLecture(GetLectureCode(session));
            return lecture.professor_id;
        }
        /// <summary>
        /// 현재 대기큐에 있는 학생들을 검사해서 시간에 맞는 교수에게 보내준다.
        /// </summary>
        public void SelectPushClass()
        {
            for (int i = _waitingList.Count - 1; i >= 0; i--)
            {
                Schedule schedule = db.GetScheduleExistTime(DateTime.Now.ToString("HHmm"), _waitingList[i].ID);
                Lecture lecture = db.GetLecture(schedule.LectureCode);
                if (_classRoom.TryGetValue(lecture.professor_id, out ClassRoom room))
                {
                    _waitingList[i].Host = lecture.professor_id;
                    room.Push(() => room.Enter(_waitingList[i]));
                    _waitingList.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 방에서 학생 내보내기
        /// </summary>
        /// <param name="session"></param>
        public void SelectPullClass(ClientSession session)
        {
            if(_classRoom.TryGetValue(session.ID, out ClassRoom room))
            {
                _waitingList.AddRange(room.GetStudentList());
                foreach(ClientSession s in _waitingList)
                {
                    s.Host = null;
                }
            }
        }

        /// <summary>
        /// 객체를 찾는 함수
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);
                return session;
            }
        }
        public void LeaveRoom(ClientSession session)
        {
            // 본인의 아이디로된 방이 있으면 방안의 학생들 대기큐로 넘기고 룸 삭제
            if (_classRoom.TryGetValue(session.ID, out ClassRoom room))
            {
                _waitingList.AddRange(room.GetStudentList());

                foreach (ClientSession s in _waitingList)
                {
                    s.Host = null;
                }                
                room.Push(()=>room.ClearRoom(session));
                _classRoom.Remove(session.ID);
            }
            else
            {
                Console.WriteLine("1");
                if (_classRoom.TryGetValue(session.Host, out ClassRoom room2))
                {
                    room2.Push(() => room.LeaveRoom(session));
                    Console.WriteLine("2");
                }
                Console.WriteLine("3");
            }
        }

        /// <summary>
        /// 연결이 해제 되었을 때 호출되는 함수
        /// </summary>
        /// <param name="session"></param>
        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                Console.WriteLine("-1");
                if (session.ID != null)
                {
                    LeaveRoom(session);
                    Console.WriteLine("-2");
                    if (_loginSessions.TryGetValue(session.ID, out ClientSession s))
                    {
                        _loginSessions.Remove(session.ID);
                        Console.WriteLine("-10");
                    }
                    Console.WriteLine("-3");
                }
                
                Console.WriteLine("-4");
                _sessions.Remove(session.SessionId);
                Console.WriteLine("-5");
            }
        }

    }
}
