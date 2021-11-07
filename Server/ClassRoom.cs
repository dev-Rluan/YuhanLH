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
        // 수업 이름
        public string LectureCode { get; set; }
        public string ProfessorID { get; set; }
        public Lecture _lecture { get; set; }
        /// <summary>
        /// 전체 클라이언트 정보
        /// </summary>
        List<ClientSession> _sessions = new List<ClientSession>();
        
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        
        /// <summary>
        /// 순차적으로 실행
        /// </summary>
        /// <param name="job"></param>
        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

      /*  public void Flush()
        {
            // N ^ 2
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }*/

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
        
        public void Quiz_OX(CP_QuizOX packet)
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
        public void Quiz(CP_Quiz packet)
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
            ArraySegment<byte> segment = sp_screenPacket.Write();
            _pendingList.Add(segment);

            // 현재 수업의 교수를 찾아서 이미지 보내기
            foreach (ClientSession s in _sessions)
            {
                if(_lecture.professor_id == null)
                {
                    return;
                }
                if (s.ID == _lecture.professor_id)
                    s.Send(segment);
            }               
            Console.WriteLine("이미지전송");  

        } 

        public void CreateRoom(Lecture lecture)
        {
            _lecture = lecture;
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
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
