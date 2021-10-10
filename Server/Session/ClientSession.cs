using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
	class ClientSession : PacketSession
    {   
        public int SessionId { get; set; }
        public ClassRoom Room { get; set; }
        public SessionManager _sessionManager = Program.sessionManager;
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            //TODO
            Program.Room.Push(() => Program.Room.Enter(this));

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            _sessionManager.Remove(this);
            if(Room != null)
            {
                // 룸이 비었어도 에러가 안나도록 (빈공간 삭제 방지)
                ClassRoom room = Room;

                room.Push(() => room.Leave(this));                
                Room = null;
            }
            Console.WriteLine($"Transferred bytes: {endPoint}");
        }

        //역 직렬화
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

     

        public override void OnSend(int numOfByteser)
        {
            //Console.WriteLine($"Transferred bytes: {numOfByteser}");

        }
    }
}
