using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PClient;

public enum PacketID
{
    CP_Login = 1,
	CP_ScreenRequest = 2,
	CP_QuizOX = 3,
	CP_Quiz = 4,
	CP_QResult = 5,
	CP_Atd = 6,
	CP_StudentList = 7,
	CS_Login = 8,
	CS_Quiz = 9,
	CS_ScreenResult = 10,
	CS_QustionText = 11,
	CS_QustionImg = 12,
	CS_Qustion = 13,
	CS_AtdCheck = 14,
	SP_Result = 15,
	SP_LoginFailed = 16,
	SP_LoginResult = 17,
	SP_StudentInfo = 18,
	SP_ScreenResult = 19,
	SS_Result = 20,
	SS_LoginFailed = 21,
	SS_LoginResult = 22,
	SS_QResult = 23,
	SS_AtdRequest = 24,
	SS_QuizOX = 25,
	SS_Quiz = 26,
	
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

class CP_ScreenRequest : IPacket
{
    public string id;
	public class Student
	{
	   public string studentID;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort studentIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIDLen);
			count += studentIDLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort studentIDLen = (ushort)Encoding.Unicode.GetBytes(this.studentID, 0, this.studentID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentIDLen;
	        return success;
	    }
	
	    
	
	}
	public List<Student> students = new List<Student>();
	    
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
		this.students.Clear();
		ushort studentLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < studentLen; i++)
		{
		    Student student = new Student();
		    student.Read(segment, ref count);
		    students.Add(student);
		}
       

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
		Array.Copy(BitConverter.GetBytes((ushort)this.students.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Student student in students)
		    student.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CP_QuizOX : IPacket
{
    public string quiz;
	public class Student
	{
	   public string studentId;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
			count += studentIdLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentIdLen;
	        return success;
	    }
	
	    
	
	}
	public List<Student> students = new List<Student>();
	    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_QuizOX; } }

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
        
        ushort quizLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.quiz = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, quizLen);
		count += quizLen;
		this.students.Clear();
		ushort studentLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < studentLen; i++)
		{
		    Student student = new Student();
		    student.Read(segment, ref count);
		    students.Add(student);
		}
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_QuizOX), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort quizLen = (ushort)Encoding.Unicode.GetBytes(this.quiz, 0, this.quiz.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(quizLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += quizLen;
		Array.Copy(BitConverter.GetBytes((ushort)this.students.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Student student in students)
		    student.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CP_Quiz : IPacket
{
    public string quiz;
	public class Student
	{
	   public string studentId;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
			count += studentIdLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentIdLen;
	        return success;
	    }
	
	    
	
	}
	public List<Student> students = new List<Student>();
	    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_Quiz; } }

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
        
        ushort quizLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.quiz = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, quizLen);
		count += quizLen;
		this.students.Clear();
		ushort studentLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < studentLen; i++)
		{
		    Student student = new Student();
		    student.Read(segment, ref count);
		    students.Add(student);
		}
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_Quiz), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort quizLen = (ushort)Encoding.Unicode.GetBytes(this.quiz, 0, this.quiz.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(quizLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += quizLen;
		Array.Copy(BitConverter.GetBytes((ushort)this.students.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Student student in students)
		    student.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CP_QResult : IPacket
{
    public string studentID;
	public string result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_QResult; } }

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
        
        ushort studentIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIDLen);
		count += studentIDLen;
		ushort resultLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.result = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, resultLen);
		count += resultLen;
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_QResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIDLen = (ushort)Encoding.Unicode.GetBytes(this.studentID, 0, this.studentID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIDLen;
		ushort resultLen = (ushort)Encoding.Unicode.GetBytes(this.result, 0, this.result.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(resultLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += resultLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CP_Atd : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_Atd; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_Atd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CP_StudentList : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_StudentList; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_StudentList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
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

class CS_Quiz : IPacket
{
    public string result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_Quiz; } }

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
        
        ushort resultLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.result = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, resultLen);
		count += resultLen;
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_Quiz), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort resultLen = (ushort)Encoding.Unicode.GetBytes(this.result, 0, this.result.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(resultLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += resultLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CS_ScreenResult : IPacket
{
    public string studentID;
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
        
        ushort studentIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIDLen);
		count += studentIDLen;
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

        ushort studentIDLen = (ushort)Encoding.Unicode.GetBytes(this.studentID, 0, this.studentID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIDLen;
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

class CS_QustionText : IPacket
{
    public string qustion;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_QustionText; } }

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
        
        ushort qustionLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.qustion = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, qustionLen);
		count += qustionLen;
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_QustionText), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort qustionLen = (ushort)Encoding.Unicode.GetBytes(this.qustion, 0, this.qustion.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(qustionLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += qustionLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class CS_QustionImg : IPacket
{
    public byte[] img;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_QustionImg; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_QustionImg), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

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

class CS_Qustion : IPacket
{
    public string qustion;
	public byte[] img;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_Qustion; } }

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
        
        ushort qustionLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.qustion = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, qustionLen);
		count += qustionLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_Qustion), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort qustionLen = (ushort)Encoding.Unicode.GetBytes(this.qustion, 0, this.qustion.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(qustionLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += qustionLen;
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

class CS_AtdCheck : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_AtdCheck; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_AtdCheck), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
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

class SP_LoginFailed : IPacket
{
    public ushort result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_LoginFailed; } }

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
        
        this.result = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_LoginFailed), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SP_LoginResult : IPacket
{
    public class Student
	{
	   public string studentID;
		public string studentName;
		public ushort state;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort studentIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIDLen);
			count += studentIDLen;
			ushort studentNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentNameLen);
			count += studentNameLen;
			this.state = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort studentIDLen = (ushort)Encoding.Unicode.GetBytes(this.studentID, 0, this.studentID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentIDLen;
			ushort studentNameLen = (ushort)Encoding.Unicode.GetBytes(this.studentName, 0, this.studentName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentNameLen;
			Array.Copy(BitConverter.GetBytes(state), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
	        return success;
	    }
	
	    
	
	}
	public List<Student> students = new List<Student>();
	
	public class Schedule
	{
	   public string date;
		public string subject;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort dateLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.date = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dateLen);
			count += dateLen;
			ushort subjectLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.subject = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, subjectLen);
			count += subjectLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort dateLen = (ushort)Encoding.Unicode.GetBytes(this.date, 0, this.date.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dateLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dateLen;
			ushort subjectLen = (ushort)Encoding.Unicode.GetBytes(this.subject, 0, this.subject.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(subjectLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += subjectLen;
	        return success;
	    }
	
	    
	
	}
	public List<Schedule> schedules = new List<Schedule>();
	    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_LoginResult; } }

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
        
        this.students.Clear();
		ushort studentLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < studentLen; i++)
		{
		    Student student = new Student();
		    student.Read(segment, ref count);
		    students.Add(student);
		}
		this.schedules.Clear();
		ushort scheduleLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < scheduleLen; i++)
		{
		    Schedule schedule = new Schedule();
		    schedule.Read(segment, ref count);
		    schedules.Add(schedule);
		}
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_LoginResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes((ushort)this.students.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Student student in students)
		    student.Write(segment, ref count);
		Array.Copy(BitConverter.GetBytes((ushort)this.schedules.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Schedule schedule in schedules)
		    schedule.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SP_StudentInfo : IPacket
{
    public class Student
	{
	   public string studentID;
		public ushort state;
		public byte[] img;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort studentIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIDLen);
			count += studentIDLen;
			this.state = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			int imgLen = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ArraySegment<byte> imgArray;
			imgArray = segment.Slice(segment.Offset + count, imgLen);
			this.img = imgArray.ToArray();      
			count += imgLen;
			
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort studentIDLen = (ushort)Encoding.Unicode.GetBytes(this.studentID, 0, this.studentID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentIDLen;
			Array.Copy(BitConverter.GetBytes(state), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			int imgLen = (int)this.img.Length;
			 Array.Copy(BitConverter.GetBytes(imgLen), 0, segment.Array, segment.Offset + count, sizeof(int));
			 Array.Copy(this.img, 0, segment.Array, segment.Offset + count + sizeof(int), imgLen);
			 count += sizeof(int);
			 count += imgLen;
	        return success;
	    }
	
	    
	
	}
	public List<Student> students = new List<Student>();
	    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_StudentInfo; } }

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
        
        this.students.Clear();
		ushort studentLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < studentLen; i++)
		{
		    Student student = new Student();
		    student.Read(segment, ref count);
		    students.Add(student);
		}
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_StudentInfo), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes((ushort)this.students.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Student student in students)
		    student.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SP_ScreenResult : IPacket
{
    public string studentID;
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
        
        ushort studentIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIDLen);
		count += studentIDLen;
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

        ushort studentIDLen = (ushort)Encoding.Unicode.GetBytes(this.studentID, 0, this.studentID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIDLen;
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

class SS_LoginFailed : IPacket
{
    public ushort result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_LoginFailed; } }

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
        
        this.result = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_LoginFailed), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SS_LoginResult : IPacket
{
    public class Schedule
	{
	   public string date;
		public string subject;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort dateLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.date = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dateLen);
			count += dateLen;
			ushort subjectLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.subject = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, subjectLen);
			count += subjectLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort dateLen = (ushort)Encoding.Unicode.GetBytes(this.date, 0, this.date.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dateLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dateLen;
			ushort subjectLen = (ushort)Encoding.Unicode.GetBytes(this.subject, 0, this.subject.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(subjectLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += subjectLen;
	        return success;
	    }
	
	    
	
	}
	public List<Schedule> schedules = new List<Schedule>();
	    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_LoginResult; } }

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
        
        this.schedules.Clear();
		ushort scheduleLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < scheduleLen; i++)
		{
		    Schedule schedule = new Schedule();
		    schedule.Read(segment, ref count);
		    schedules.Add(schedule);
		}
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_LoginResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes((ushort)this.schedules.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Schedule schedule in schedules)
		    schedule.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SS_QResult : IPacket
{
    public string result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_QResult; } }

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
        
        ushort resultLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.result = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, resultLen);
		count += resultLen;
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_QResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort resultLen = (ushort)Encoding.Unicode.GetBytes(this.result, 0, this.result.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(resultLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += resultLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SS_AtdRequest : IPacket
{
    public ushort result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_AtdRequest; } }

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
        
        this.result = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_AtdRequest), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SS_QuizOX : IPacket
{
    public ushort quiz;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_QuizOX; } }

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
        
        this.quiz = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_QuizOX), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(quiz), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

class SS_Quiz : IPacket
{
    public ushort quiz;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_Quiz; } }

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
        
        this.quiz = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_Quiz), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(quiz), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

