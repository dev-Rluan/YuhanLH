using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;


public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	S_Test = 3,
	
}

interface IPacket
{
	ushort Protocol { get;  }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


class C_Chat : IPacket
{
    public string chat;    
        
    public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
		count += chatLen;
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class S_Chat : IPacket
{
    public int playerId;
	public string chat;    
        
    public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
		count += chatLen;
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class S_Test : IPacket
{
    public int test;
	public string name;    
        
    public ushort Protocol { get { return (ushort)PacketID.S_Test; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.test = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
		count += nameLen;
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Test), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(test), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nameLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

