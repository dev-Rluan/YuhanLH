using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// 룸에 대한 정보를 가지고있는 클래스
    /// </summary>
    class ClassRoom : IJobQueue
    {
        // 수업 정보
        public Lecture _lecture { get; set; }
        /// <summary>
        /// 전체 클라이언트 정보
        /// </summary>
        List<ClientSession> _sessions = new List<ClientSession>();
        /// <summary>
        /// 교수 클라이언트
        /// </summary>
        ClientSession ProfessorClient;
        /// <summary>
        /// 교수 아이디
        /// </summary>
        public string Host { get; set; }
        JobQueue _jobQueue = new JobQueue();
        
        /// <summary>
        /// 순차적으로 실행
        /// </summary>
        /// <param name="job"></param>
        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }


        /// <summary>
        /// 수업코드 반환
        /// </summary>
        /// <returns></returns>
        public string Get_LectureCode()
        {
            return _lecture.lecture_code;
        }
        /// <summary>
        ///  교수가 처음 방을 만들었을 때
        /// </summary>
        /// <param name="session"></param>
        public void CreateClassRoom(ClientSession session)
        {
            Host = session.ID;
            ProfessorClient = session;
            Console.WriteLine($"방생성 : {Host}");
;        }
        /// <summary>
        /// 전체 접속중인 학생 리스트 보내기
        /// </summary>
        public void ShowStudentList()
        {
            Database db = new Database();
            SP_StudentInfo pkt = new SP_StudentInfo();
            SP_StudentInfo.Student student = new SP_StudentInfo.Student();
            foreach(ClientSession s in _sessions)
            {

                Student _student = db.GetStudent(s.ID);
                student.studentId = _student.StudentId;
                Console.WriteLine(student.studentId);
                pkt.students.Add(student);
            }
            Console.WriteLine($"학생 리스트 {ProfessorClient.ID} 에게 보냄");
            ProfessorClient.Send(pkt.Write());
            Console.WriteLine("2");
        }
        /// <summary>
        /// 교수의 답변 보내기
        /// </summary>
        /// <param name="packet"></param>
        public void QResult(CP_QResult packet)
        {
            SS_QResult pkt = new SS_QResult();
            pkt.result = packet.result;
            ForStudent(packet.studentId, pkt.Write());
        }
        /// <summary>
        /// 학생 한명에게 패킷 보내기
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="buffer"></param>
        public void ForStudent(string studentID, ArraySegment<byte> buffer)
        {
            foreach(ClientSession s in _sessions)
            {
                if(s.ID == studentID)
                {
                    s.Send(buffer);
                }
            }
        }
        /// <summary>
        /// 학생 출석
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void AtdResult(ClientSession session, CS_AtdCheck packet)
        {
            SP_AddAtd pkt = new SP_AddAtd();
            pkt.studentId = session.ID;
            pkt.classTime = packet.classTime;
            pkt.attr = packet.Attr;
            ProfessorClient.Send(pkt.Write());
        }

        /// <summary>
        /// 교수자가 수업를 종료 하였을때 호출 됩니다. 학생 리스트 삭제
        /// </summary>
        public void ClearRoom(ClientSession session)
        {            
            BroadCast_EndClass();
        }

        public List<ClientSession> GetStudentList()
        {
            return _sessions;
        }
        /// <summary>
        /// 학생리스트로 스크린 샷 요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void Img_Request(ClientSession session, CP_ScreenRequest packet)
        {
            SS_ScreenRequest pkt = new SS_ScreenRequest();
            foreach(ClientSession s in _sessions)
            {
                foreach(CP_ScreenRequest.Student s2 in packet.students)
                {
                    if (s.ID == s2.studentId)
                    {
                        s.Send(pkt.Write());
                    }
                }               
            }
        }
        
        /// <summary>
        /// 퀴즈 보내기
        /// </summary>
        /// <param name="packet"></param>
        public void Quiz_Request( CP_Quiz packet)
        {
            SS_Quiz pkt = new SS_Quiz();
            pkt.quiz = packet.quiz;
            foreach (ClientSession s in _sessions)
            {
                foreach (CP_Quiz.Student s2 in packet.students)
                {
                    if (s.ID == s2.studentId)
                    {
                        s.Send(pkt.Write());
                    }
                }
            }
        }

        /// <summary>
        /// OX 퀴즈보내기
        /// </summary>
        /// <param name="packet"></param>
        public void QuizOX_Request(CP_QuizOX packet)
        {
            SS_QuizOX pkt = new SS_QuizOX();
            pkt.quiz = packet.quiz;
            foreach (ClientSession s in _sessions)
            {
                foreach (CP_QuizOX.Student s2 in packet.students)
                {
                    if (s.ID == s2.studentId)
                    {
                        s.Send(pkt.Write());
                    }
                }
            }
        }
        /// <summary>
        /// 퀴즈 답변 보내기
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void Quiz_Result(ClientSession session, CS_Quiz packet)
        {
            SP_QuizResult pkt = new SP_QuizResult();
            pkt.studentId = session.ID;
            pkt.result = packet.result;
            ProfessorClient.Send(pkt.Write());
        }
        /// <summary>
        /// OX 퀴즈 답변 보내기
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void QuizOX_Result(ClientSession session, CS_QuizOX packet)
        {
            SP_QuizOXResult pkt = new SP_QuizOXResult();
            pkt.studentId = session.ID;
            pkt.result = packet.result;
            ProfessorClient.Send(pkt.Write());
        }

        public void QustionText(ClientSession session, CS_QustionText packet)
        {
            SP_QustionText pkt = new SP_QustionText();
            pkt.qustion = packet.qustion;
            pkt.studentId = session.ID;
            ProfessorClient.Send(pkt.Write());            
        }
        public void QustionImg(ClientSession session, CS_QustionImg packet)
        {
            SP_QustionImg pkt = new SP_QustionImg();
            pkt.studentId = session.ID;
            ProfessorClient.Send(pkt.Write());
        }
        public void Qustion(ClientSession session, CS_Qustion packet)
        {
            SP_Qustion pkt = new SP_Qustion();
            pkt.qustion = packet.qustion;
            pkt.img = packet.img;
            pkt.studentId = session.ID;
            ProfessorClient.Send(pkt.Write());
        }


        /// <summary>
        /// 수업종료를 학생들에게 알려주고 룸에서 비워준다.
        /// </summary>
        public void BroadCast_EndClass()
        {
            SS_EndOfClass pkt = new SS_EndOfClass();
            foreach (ClientSession s in _sessions)
            {
                if (s.ID != Host)
                {
                    s.Send(pkt.Write());
                    Leave(s);
                }
            }
        }

        // 이미지 전송 
        /// <summary>
        /// 이미지 전송
        /// </summary>
        /// <param name="session"></param>
        /// <param name="img"></param>
        /// <param name="id"></param>
        public void Img_Send( byte[] img, string id)
        {
            SP_ScreenResult sp_screenPacket = new SP_ScreenResult();
            sp_screenPacket.studentId = id;
            sp_screenPacket.img = img;
            ProfessorClient.Send(sp_screenPacket.Write());         
            Console.WriteLine("이미지전송");  
        } 
   


        public void Enter(ClientSession session)
        {
            Console.WriteLine("학생 수업방 접속");
            _sessions.Add(session);

            SP_AddStudent pkt = new SP_AddStudent();
            pkt.studentId = session.ID;
            ProfessorClient.Send(pkt.Write());
        }

        public void AttRequest(CP_Atd packet)
        {
            SS_AtdRequest pkt = new SS_AtdRequest();
            pkt.classTime = packet.classTime;            
            BroadCast(pkt.Write());
        }
       
        public void BroadCast(ArraySegment<byte> buffer)
        {
            foreach(ClientSession s in _sessions)
            {
                s.Send(buffer);
            }
        }

        public void LeaveRoom(ClientSession session)
        {
            _sessions.Remove(session);
            SP_LeaveStudent pkt = new SP_LeaveStudent();
            pkt.studentId = session.ID;
            ProfessorClient.Send(pkt.Write());
        }

        /// <summary>
        ///  로그아웃
        /// </summary>
        /// <param name="session"></param>
        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
            
        }
        
       
        
    }
}
