using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;


public enum PacketID
{
    PC_Login = 1,
	SC_Login = 2,
	SS_Result = 3,
	PC_Chat = 4,
	sC_Chat = 5,
	PC_ScreenRequest = 6,
	SC_ScreenResult = 7,
	
}

interface IPacket
{
	ushort Protocol { get;  }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


class PC_Login : IPacket
{
    public string id;
	public string pwd;    
        
    public ushort Protocol { get { return (ushort)PacketID.PC_Login; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		ushort pwdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.pwd = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, pwdLen);
		count += pwdLen;
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.PC_Login), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort pwdLen = (ushort)Encoding.Unicode.GetBytes(this.pwd, 0, this.pwd.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(pwdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += pwdLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class SC_Login : IPacket
{
    public string id;
	public string pwd;    
        
    public ushort Protocol { get { return (ushort)PacketID.SC_Login; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		ushort pwdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.pwd = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, pwdLen);
		count += pwdLen;
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SC_Login), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort pwdLen = (ushort)Encoding.Unicode.GetBytes(this.pwd, 0, this.pwd.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(pwdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += pwdLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class SS_Result : IPacket
{
    public bool result;    
        
    public ushort Protocol { get { return (ushort)PacketID.SS_Result; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.result = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_Result), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class PC_Chat : IPacket
{
    public string id;
	public string chat;    
        
    public ushort Protocol { get { return (ushort)PacketID.PC_Chat; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.PC_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class sC_Chat : IPacket
{
    public string id;
	public string chat;    
        
    public ushort Protocol { get { return (ushort)PacketID.sC_Chat; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.sC_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class PC_ScreenRequest : IPacket
{
    public string id;
	public string pwd;    
        
    public ushort Protocol { get { return (ushort)PacketID.PC_ScreenRequest; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		ushort pwdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.pwd = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, pwdLen);
		count += pwdLen;
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.PC_ScreenRequest), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort pwdLen = (ushort)Encoding.Unicode.GetBytes(this.pwd, 0, this.pwd.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(pwdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += pwdLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

class SC_ScreenResult : IPacket
{
    public string id;
	public string pwd;    
        
    public ushort Protocol { get { return (ushort)PacketID.SC_ScreenResult; } }
    public  void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		ushort pwdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.pwd = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, pwdLen);
		count += pwdLen;
       

    }

    public  ArraySegment<byte> Write()
    {
             
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);            
        ushort count = 0;        

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SC_ScreenResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort pwdLen = (ushort)Encoding.Unicode.GetBytes(this.pwd, 0, this.pwd.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(pwdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += pwdLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);


    }
}

