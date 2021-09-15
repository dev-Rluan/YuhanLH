using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }
    class PlayerInfoReq : Packet
    {
        public long playerId;

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;

            long playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {

            ushort count = 0;

            ArraySegment<byte> s = SendBufferHelper.Open(4096);
            bool success = true;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), this.size);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;


            //byte[] send_buffer = new byte[4096];
            //byte[] size = BitConverter.GetBytes(packet.size);
            //byte[] packetId = BitConverter.GetBytes(packet.packetId);
            //byte[] playerId = BitConverter.GetBytes(packet.playerId);


            //Array.Copy(size, 0, s.Array, s.Offset + count, 2);
            //count += 2;
            //Array.Copy(packetId, 0, s.Array, s.Offset  + count,  2 + 2);
            //count += 2;
            //Array.Copy(playerId, 0, s.Array, s.Offset + count, 8);
            //count += 2;

            if (success == false)
                return null;

            return SendBufferHelper.Close(count);

        }
    }
    /*class PlayerInfoOk : Packet 
    {
        public int hp;
        public int attack;
    }*/
    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
        playerType = 3
    }

    class ServerSession : Session
    {

        /* static unsafe void ToBytes(byte[] array,int offset, ulong value)
         {
             fixed (byte* ptr = &array[offset]) 
                 *(ulong*)ptr = value;
         }*/

        public override void OnConnected(EndPoint endPoint)
        {
            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = (ushort)PacketID.PlayerInfoReq };


        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Transferred bytes: {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            String recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[fromServer] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfByteser)
        {
            Console.WriteLine($"Transferred bytes: {numOfByteser}");

        }
    }
}
