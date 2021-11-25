using DummyClient2;
using System;
using System.Drawing;

class PacketHandler
{

    public static void SP_ResultHandler(PacketSession session, IPacket packet)
    {
        SP_Result result = packet as SP_Result;
        ServerSession serverSession = session as ServerSession;       
       
    }
    
    public static void SP_LoginFailedHandler(PacketSession session, IPacket packet)
    {
        SP_LoginFailed pkt = packet as SP_LoginFailed;

        Console.WriteLine(pkt.result + "로 인한 실패");
    }     
    public static void SP_LoginResultHandler(PacketSession session, IPacket packet)
    {
        SP_LoginResult pkt = packet as SP_LoginResult;
        Console.WriteLine("로그인 성공");
        Console.WriteLine("전체 수업 정보");
        foreach(SP_LoginResult.Lecture lec in pkt.lectures)
        {
            Console.WriteLine( $"{lec.lecture_code} {lec.credit} {lec.lecture_name} {lec.professor_id} {lec.strat_time} {lec.end_time}");

        }
        foreach(SP_LoginResult.Student student in pkt.students)
        {
            Console.WriteLine($"{student.lectureCode} {student.studentId} {student.studentName}");
        }


    }
    public static void SP_StudentInfoHandler(PacketSession session, IPacket packet)
    {
        SP_StudentInfo pkt = packet as SP_StudentInfo;
     
    }

    public static void SP_ScreenResultHandler(PacketSession session, IPacket packet)
    {
        SP_ScreenResult sp_screenPacket = packet as SP_ScreenResult;
        ServerSession serverSession = session as ServerSession;
       
    
    }
    public static void SP_QustionTextHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QustionImgHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QustionHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QuizResultHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_EndClassHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QuizOXResultHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_AddStudentHandler(PacketSession session, IPacket packet)
    {
        SP_AddStudent pkt = packet as SP_AddStudent;
        Console.WriteLine($"{pkt.studentId} : 접속" );
    }
    public static void SP_LeaveStudentHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_AddAtdHandler(PacketSession session, IPacket packet)
        {

        }
    

}