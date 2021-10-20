using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PClient;

public enum PacketID
{
    CP_Login = 1,
	CS_Login = 2,
	SS_Result = 3,
	SP_Result = 4,
	CP_Chat = 5,
	CS_Chat = 6,
	CP_ScreenRequest = 7,
	CS_ScreenResult = 8,
	SP_ScreenResult = 9,
	
}

interface IPacket
{
	ushort Protocol { get;  }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


class CP_Login : IPacket
{
    public string id;
	public string pwd;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_Login; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
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
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_Login), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort pwdLen = (ushort)Encoding.Unicode.GetBytes(this.pwd, 0, this.pwd.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(pwdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += pwdLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CS_Login : IPacket
{
    public string id;
	public string pwd;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_Login; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
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
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_Login), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort pwdLen = (ushort)Encoding.Unicode.GetBytes(this.pwd, 0, this.pwd.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(pwdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += pwdLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SS_Result : IPacket
{
    public bool result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_Result; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
        count += sizeof(ushort);
        
        this.result = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
       

    }

    public  ArraySegment<byte> Write()
    {
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_Result), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SP_Result : IPacket
{
    public bool result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_Result; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
        count += sizeof(ushort);
        
        this.result = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
       

    }

    public  ArraySegment<byte> Write()
    {
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_Result), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CP_Chat : IPacket
{
    public string id;
	public string chat;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_Chat; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
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
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CS_Chat : IPacket
{
    public string id;
	public string chat;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_Chat; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
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
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CP_ScreenRequest : IPacket
{
    public string id;
	public string student;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_ScreenRequest; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		ushort studentLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.student = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentLen);
		count += studentLen;
       

    }

    public  ArraySegment<byte> Write()
    {
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_ScreenRequest), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort studentLen = (ushort)Encoding.Unicode.GetBytes(this.student, 0, this.student.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CS_ScreenResult : IPacket
{
    public string id;
	public byte[] img;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_ScreenResult; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		int imgLen = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ArraySegment<byte> imgArray;
		imgArray = segment.Slice(segment.Offset + count, imgLen);
		this.img = imgArray.ToArray();      
		count += imgLen;
		
       

    }

    public  ArraySegment<byte> Write()
    {
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_ScreenResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		int imgLen = (int)this.img.Length;
		 Array.Copy(BitConverter.GetBytes(imgLen), 0, segment.Array, segment.Offset + count, sizeof(int));
		 Array.Copy(this.img, 0, segment.Array, segment.Offset + count + sizeof(int), imgLen);
		 count += sizeof(int);
		 count += imgLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SP_ScreenResult : IPacket
{
    public string id;
	public byte[] img;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_ScreenResult; } }

    public  void Read(ArraySegment<byte> segment)
    {
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
        count += sizeof(ushort);
        
        ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		int imgLen = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ArraySegment<byte> imgArray;
		imgArray = segment.Slice(segment.Offset + count, imgLen);
		this.img = imgArray.ToArray();      
		count += imgLen;
		
       

    }

    public  ArraySegment<byte> Write()
    {
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_ScreenResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		int imgLen = (int)this.img.Length;
		 Array.Copy(BitConverter.GetBytes(imgLen), 0, segment.Array, segment.Offset + count, sizeof(int));
		 Array.Copy(this.img, 0, segment.Array, segment.Offset + count + sizeof(int), imgLen);
		 count += sizeof(int);
		 count += imgLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

