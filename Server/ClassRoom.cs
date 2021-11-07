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
        /// <summary>
        /// 전체 클라이언트 정보
        /// </summary>
        List<ClientSession> _sessions = new List<ClientSession>();
        Lecture _lecture;
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

        // 이미지 전송 
        /// <summary>
        /// 이미지 전송
        /// </summary>
        /// <param name="session"></param>
        /// <param name="img"></param>
        /// <param name="id"></param>
        public void Img_Broadcast(ClientSession session, byte[] img, string id)
        {
            SP_ScreenResult sp_screenPacket = new SP_ScreenResult();
            sp_screenPacket.studentID = id;
            sp_screenPacket.img = img;
            ArraySegment<byte> segment = sp_screenPacket.Write();
            _pendingList.Add(segment);

            //호스트 검색 host

            
            foreach (ClientSession s in _sessions)
            {
                if(_lecture.professor_id != null)
                {
                    if (s.ID == _lecture.professor_id)
                        s.Send(segment);
                }
                else
                {
                    s
                }
                
            }
               

            Console.WriteLine("이미지전송");            

        } 

        public void CreateRoom(Lecture lecture)
        {
            _lecture = lecture;
            LectureCode = lecture.lecture_code;
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }
        public void P_Enter(ClientSession session)
        {
            //foreach(string id in waitingQueue)
            //{
            //    string s_host;
                // 학생의 호스트 확인 
                //if( s_host == packet.id)
                //{
                //waitingQueue.Dequeue();
                //Queue<string> action = null;
                //if (_classRoom.TryGetValue(host, out action))
                //    action.Enqueue(packet.id);
                // _classRoom[host] = action;
                //}
            //}
            session.Room = this;
        }
        public void S_Enter(ClientSession session, CS_Login packet)
        {
            _sessions.Add(session);
            // 학생의 현재 시간표 sc = ();
            // 전체 정보 저장 scALL
            // 현재 시간표 교수님 host
            //Queue<string> action = null;
            //if (_classRoom.TryGetValue(host, out action))
            //    action.Enqueue(packet.id);
            //else
            //  watingQueue.Enqueue(packet.id);
            // _classRoom[host] = action;
            session.Room = this;
            // session.Host = host
        }
        public void Leave(ClientSession session)
        {
           
            _sessions.Remove(session);
        }

        
    }
}
