using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DummyClient3;

public enum PacketID
{
    CP_Login = 1,
	CP_ScreenRequest = 2,
	CP_QuizOX = 3,
	CP_Quiz = 4,
	CP_QResult = 5,
	CP_Atd = 6,
	CP_StudentList = 7,
	CP_EndOfClass = 8,
	CS_Login = 9,
	CS_Quiz = 10,
	CS_QuizOX = 11,
	CS_ScreenResult = 12,
	CS_QustionText = 13,
	CS_QustionImg = 14,
	CS_Qustion = 15,
	CS_AtdCheck = 16,
	SP_Result = 17,
	SP_LoginFailed = 18,
	SP_LoginResult = 19,
	SP_StudentInfo = 20,
	SP_ScreenResult = 21,
	SP_QustionText = 22,
	SP_QustionImg = 23,
	SP_Qustion = 24,
	SP_QuizResult = 25,
	SP_QuizOXResult = 26,
	SP_AddStudent = 27,
	SP_LeaveStudent = 28,
	SP_AddAtd = 29,
	SP_EndClass = 30,
	SS_Result = 31,
	SS_LoginFailed = 32,
	SS_Logout = 33,
	SS_LoginResult = 34,
	SS_EnterRoom = 35,
	SS_ScreenRequest = 36,
	SS_QResult = 37,
	SS_AtdRequest = 38,
	SS_QuizOX = 39,
	SS_Quiz = 40,
	SS_ImgSendFaild = 41,
	SS_EndOfClass = 42,
	SS_QustionFaild = 43,
	
}

interface IPacket
{
	ushort Protocol { get;  }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class CP_Login : IPacket
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

public class CP_ScreenRequest : IPacket
{
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

        Array.Copy(BitConverter.GetBytes((ushort)this.students.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Student student in students)
		    student.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class CP_QuizOX : IPacket
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

public class CP_Quiz : IPacket
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

public class CP_QResult : IPacket
{
    public string studentId;
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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
		ushort resultLen = (ushort)Encoding.Unicode.GetBytes(this.result, 0, this.result.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(resultLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += resultLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class CP_Atd : IPacket
{
    public int classTime;
	public int week;    
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
        
        this.classTime = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.week = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
       

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

        Array.Copy(BitConverter.GetBytes(classTime), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(week), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class CP_StudentList : IPacket
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

public class CP_EndOfClass : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CP_EndOfClass; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CP_EndOfClass), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class CS_Login : IPacket
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

public class CS_Quiz : IPacket
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

public class CS_QuizOX : IPacket
{
    public bool result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.CS_QuizOX; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CS_QuizOX), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class CS_ScreenResult : IPacket
{
    public string studentId;
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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
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

public class CS_QustionText : IPacket
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

public class CS_QustionImg : IPacket
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

public class CS_Qustion : IPacket
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

public class CS_AtdCheck : IPacket
{
    public int classTime;
	public int week;    
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
        
        this.classTime = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.week = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
       

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

        Array.Copy(BitConverter.GetBytes(classTime), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(week), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_Result : IPacket
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

public class SP_LoginFailed : IPacket
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

public class SP_LoginResult : IPacket
{
    public string name;
	public class Student
	{
	   public string studentId;
		public string studentName;
		public string lectureCode;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
			count += studentIdLen;
			ushort studentNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.studentName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentNameLen);
			count += studentNameLen;
			ushort lectureCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.lectureCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, lectureCodeLen);
			count += lectureCodeLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentIdLen;
			ushort studentNameLen = (ushort)Encoding.Unicode.GetBytes(this.studentName, 0, this.studentName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(studentNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += studentNameLen;
			ushort lectureCodeLen = (ushort)Encoding.Unicode.GetBytes(this.lectureCode, 0, this.lectureCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(lectureCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += lectureCodeLen;
	        return success;
	    }
	
	    
	
	}
	public List<Student> students = new List<Student>();
	
	public class Lecture
	{
	   public string lecture_code;
		public string professor_id;
		public string lecture_name;
		public int credit;
		public string weekday;
		public string strat_time;
		public string end_time;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort lecture_codeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.lecture_code = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, lecture_codeLen);
			count += lecture_codeLen;
			ushort professor_idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.professor_id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, professor_idLen);
			count += professor_idLen;
			ushort lecture_nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.lecture_name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, lecture_nameLen);
			count += lecture_nameLen;
			this.credit = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort weekdayLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.weekday = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, weekdayLen);
			count += weekdayLen;
			ushort strat_timeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.strat_time = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, strat_timeLen);
			count += strat_timeLen;
			ushort end_timeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.end_time = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, end_timeLen);
			count += end_timeLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort lecture_codeLen = (ushort)Encoding.Unicode.GetBytes(this.lecture_code, 0, this.lecture_code.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(lecture_codeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += lecture_codeLen;
			ushort professor_idLen = (ushort)Encoding.Unicode.GetBytes(this.professor_id, 0, this.professor_id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(professor_idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += professor_idLen;
			ushort lecture_nameLen = (ushort)Encoding.Unicode.GetBytes(this.lecture_name, 0, this.lecture_name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(lecture_nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += lecture_nameLen;
			Array.Copy(BitConverter.GetBytes(credit), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort weekdayLen = (ushort)Encoding.Unicode.GetBytes(this.weekday, 0, this.weekday.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(weekdayLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += weekdayLen;
			ushort strat_timeLen = (ushort)Encoding.Unicode.GetBytes(this.strat_time, 0, this.strat_time.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(strat_timeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += strat_timeLen;
			ushort end_timeLen = (ushort)Encoding.Unicode.GetBytes(this.end_time, 0, this.end_time.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(end_timeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += end_timeLen;
	        return success;
	    }
	
	    
	
	}
	public List<Lecture> lectures = new List<Lecture>();
	    
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
        
        ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
		count += nameLen;
		this.students.Clear();
		ushort studentLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < studentLen; i++)
		{
		    Student student = new Student();
		    student.Read(segment, ref count);
		    students.Add(student);
		}
		this.lectures.Clear();
		ushort lectureLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < lectureLen; i++)
		{
		    Lecture lecture = new Lecture();
		    lecture.Read(segment, ref count);
		    lectures.Add(lecture);
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

        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nameLen;
		Array.Copy(BitConverter.GetBytes((ushort)this.students.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Student student in students)
		    student.Write(segment, ref count);
		Array.Copy(BitConverter.GetBytes((ushort)this.lectures.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Lecture lecture in lectures)
		    lecture.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_StudentInfo : IPacket
{
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

public class SP_ScreenResult : IPacket
{
    public string studentId;
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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
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

public class SP_QustionText : IPacket
{
    public string studentId;
	public string qustion;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_QustionText; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_QustionText), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
		ushort qustionLen = (ushort)Encoding.Unicode.GetBytes(this.qustion, 0, this.qustion.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(qustionLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += qustionLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_QustionImg : IPacket
{
    public string studentId;
	public byte[] img;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_QustionImg; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_QustionImg), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
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

public class SP_Qustion : IPacket
{
    public string studentId;
	public string qustion;
	public byte[] img;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_Qustion; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_Qustion), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
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

public class SP_QuizResult : IPacket
{
    public string studentId;
	public string result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_QuizResult; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_QuizResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
		ushort resultLen = (ushort)Encoding.Unicode.GetBytes(this.result, 0, this.result.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(resultLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += resultLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_QuizOXResult : IPacket
{
    public string studentId;
	public bool result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_QuizOXResult; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_QuizOXResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
		Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_AddStudent : IPacket
{
    public string studentId;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_AddStudent; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_AddStudent), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_LeaveStudent : IPacket
{
    public string studentId;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_LeaveStudent; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_LeaveStudent), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_AddAtd : IPacket
{
    public string studentId;
	public int classTime;
	public int attr;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_AddAtd; } }

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
        
        ushort studentIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIdLen);
		count += studentIdLen;
		this.classTime = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.attr = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_AddAtd), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        ushort studentIdLen = (ushort)Encoding.Unicode.GetBytes(this.studentId, 0, this.studentId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIdLen;
		Array.Copy(BitConverter.GetBytes(classTime), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(attr), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SP_EndClass : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SP_EndClass; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SP_EndClass), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_Result : IPacket
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

public class SS_LoginFailed : IPacket
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

public class SS_Logout : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_Logout; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_Logout), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_LoginResult : IPacket
{
    public int result;
	public string studentID;
	public string name;
	public class Lecture
	{
	   public string lecture_code;
		public string professor_id;
		public string lecture_name;
		public int credit;
		public string weekday;
		public string strat_time;
		public string end_time;
	
	    // 데이터 읽어오는 부분
	    public void Read(ArraySegment<byte> segment, ref int count)
	    {
	       ushort lecture_codeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.lecture_code = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, lecture_codeLen);
			count += lecture_codeLen;
			ushort professor_idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.professor_id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, professor_idLen);
			count += professor_idLen;
			ushort lecture_nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.lecture_name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, lecture_nameLen);
			count += lecture_nameLen;
			this.credit = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort weekdayLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.weekday = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, weekdayLen);
			count += weekdayLen;
			ushort strat_timeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.strat_time = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, strat_timeLen);
			count += strat_timeLen;
			ushort end_timeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.end_time = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, end_timeLen);
			count += end_timeLen;
	    }
	
	    // 데이터 쓰는 부분
	    public bool Write(ArraySegment<byte> segment, ref int count)
	    {
	        bool success = true;
	        ushort lecture_codeLen = (ushort)Encoding.Unicode.GetBytes(this.lecture_code, 0, this.lecture_code.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(lecture_codeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += lecture_codeLen;
			ushort professor_idLen = (ushort)Encoding.Unicode.GetBytes(this.professor_id, 0, this.professor_id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(professor_idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += professor_idLen;
			ushort lecture_nameLen = (ushort)Encoding.Unicode.GetBytes(this.lecture_name, 0, this.lecture_name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(lecture_nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += lecture_nameLen;
			Array.Copy(BitConverter.GetBytes(credit), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort weekdayLen = (ushort)Encoding.Unicode.GetBytes(this.weekday, 0, this.weekday.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(weekdayLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += weekdayLen;
			ushort strat_timeLen = (ushort)Encoding.Unicode.GetBytes(this.strat_time, 0, this.strat_time.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(strat_timeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += strat_timeLen;
			ushort end_timeLen = (ushort)Encoding.Unicode.GetBytes(this.end_time, 0, this.end_time.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(end_timeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += end_timeLen;
	        return success;
	    }
	
	    
	
	}
	public List<Lecture> lectures = new List<Lecture>();
	    
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
        
        this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort studentIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.studentID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, studentIDLen);
		count += studentIDLen;
		ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
		count += nameLen;
		this.lectures.Clear();
		ushort lectureLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort); 
		for(int i = 0; i < lectureLen; i++)
		{
		    Lecture lecture = new Lecture();
		    lecture.Read(segment, ref count);
		    lectures.Add(lecture);
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

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort studentIDLen = (ushort)Encoding.Unicode.GetBytes(this.studentID, 0, this.studentID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(studentIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += studentIDLen;
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nameLen;
		Array.Copy(BitConverter.GetBytes((ushort)this.lectures.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Lecture lecture in lectures)
		    lecture.Write(segment, ref count);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_EnterRoom : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_EnterRoom; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_EnterRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_ScreenRequest : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_ScreenRequest; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_ScreenRequest), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_QResult : IPacket
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

public class SS_AtdRequest : IPacket
{
    public int classTime;    
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
        
        this.classTime = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
       

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

        Array.Copy(BitConverter.GetBytes(classTime), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_QuizOX : IPacket
{
    public string quiz;    
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
        
        ushort quizLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.quiz = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, quizLen);
		count += quizLen;
       

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

        ushort quizLen = (ushort)Encoding.Unicode.GetBytes(this.quiz, 0, this.quiz.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(quizLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += quizLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_Quiz : IPacket
{
    public string quiz;    
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
        
        ushort quizLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.quiz = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, quizLen);
		count += quizLen;
       

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

        ushort quizLen = (ushort)Encoding.Unicode.GetBytes(this.quiz, 0, this.quiz.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(quizLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += quizLen;
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_ImgSendFaild : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_ImgSendFaild; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_ImgSendFaild), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_EndOfClass : IPacket
{
        
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_EndOfClass; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_EndOfClass), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

public class SS_QustionFaild : IPacket
{
    public int result;    
    // 프로토콜 구분   
    public ushort Protocol { get { return (ushort)PacketID.SS_QustionFaild; } }

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
        
        this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
       

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.SS_QustionFaild), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }
}

