using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClassRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();       
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        Dictionary<string, Queue<string>> _room = new Dictionary<string, Queue<string>>();


        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
        public void Flush()
        {
            // N ^ 2
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Img_Broadcast(ClientSession session, byte[] img, string id)
        {
            SP_ScreenResult sp_screenPacket = new SP_ScreenResult();
            sp_screenPacket.id = id;
            sp_screenPacket.img = img;
            ArraySegment<byte> segment = sp_screenPacket.Write();
            _pendingList.Add(segment);

            foreach (ClientSession s in _sessions)
                s.Send(segment);

            Console.WriteLine("이미지전송");
            

        } 

        public void Enter(ClientSession session)
        {
          
            _sessions.Add(session);

            session.Room = this;
            
            
        }
        public void Leave(ClientSession session)
        {
           
            _sessions.Remove(session);
            
        }

        
    }
}
