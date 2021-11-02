﻿using System;
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
        // 호스트 교수자
        public string ProfessorID { get; set; }

        /// <summary>
        /// 전체 클라이언트 정보
        /// </summary>
        List<ClientSession> _sessions = new List<ClientSession>();           
       
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        /// <summary>
        /// 교수가 아직 접속 안했을때 대기하는 큐
        /// </summary>        
        Queue<string> waitingQueue = new Queue<string>();
        /// <summary>
        /// 수업 하나의 인원큐
        /// </summary>
        Queue<string> hostQueue = null;
        /// <summary>
        /// 수업의 정보들을 교수자의 이름에 따라서 관리하기 편하도록 분류
        /// </summary>
        Dictionary<string, Queue<string>> _classRoom = new Dictionary<string, Queue<string>>();
        
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
                //if(s.Host == host)
                s.Send(segment);
            }
               

            Console.WriteLine("이미지전송");            

        } 

        public void Enter(ClientSession session)
        {
          
            _sessions.Add(session);
            session.Room = this;
        }
        public void P_Enter(ClientSession session, CP_Login packet)
        {

            _sessions.Add(session);
            hostQueue = new Queue<string>();
            hostQueue.Enqueue(packet.id);
            _classRoom.Add(packet.id, hostQueue);
            hostQueue = null;
            foreach(string id in waitingQueue)
            {
                string s_host;
                // 학생의 호스트 확인 
                //if( s_host == packet.id)
                //{
                //waitingQueue.Dequeue();
                //Queue<string> action = null;
                //if (_classRoom.TryGetValue(host, out action))
                //    action.Enqueue(packet.id);
                // _classRoom[host] = action;
                //}
            }
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
