using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PClient
{
	
	public class ServerSession : PacketSession
    {

        /* static unsafe void ToBytes(byte[] array,int offset, ulong value)
         {
             fixed (byte* ptr = &array[offset]) 
                 *(ulong*)ptr = value;
         }*/

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");         

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Transferred bytes: {endPoint}");
        }

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
