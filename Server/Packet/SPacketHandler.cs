using Server;
using System;
class PacketHandler
{
    public static void CP_LoginHandler(PacketSession session, IPacket packet)
    {
        CP_Login pc_loginPacket = packet as CP_Login;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.P_Login(clientSession, pc_loginPacket.id, pc_loginPacket.pwd);

    }

    public static void CP_ScreenRequestHandler(PacketSession session, IPacket packet)
    {
        CP_ScreenRequest pkt = packet as CP_ScreenRequest;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.ScreenRequest(clientSession, pkt);

    }
    public static void CP_EndOfClassHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.EndOfClass(clientSession);
    }

    public static void CP_QuizOXHandler(PacketSession session, IPacket packet)
    {
        CP_QuizOX pkt = packet as CP_QuizOX;
        ClientSession clientSession = session as ClientSession;
        Program.sessionManager.QuizOXRequest(clientSession, pkt);
    }
     
    public static void CP_QuizHandler(PacketSession session, IPacket packet)
    {
        CP_Quiz pkt = packet as CP_Quiz;
        ClientSession clientSession = session as ClientSession;
        Program.sessionManager.QuizRequest(clientSession, pkt);
    }
     
    public static void CP_QResultHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        CP_QResult pkt = packet as CP_QResult;

        Program.sessionManager.QResult(clientSession, pkt);
    }
     
    public static void CP_AtdHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        CP_Atd pkt = packet as CP_Atd;

        Program.sessionManager.AtdClass(clientSession, pkt);
    }
     
    public static void CP_StudentListHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.StudnetList(clientSession);

    }

    public static void CS_LoginHandler(PacketSession session, IPacket packet)
    {
        CS_Login sc_loginPacket = packet as CS_Login;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.S_Login(clientSession, sc_loginPacket.id, sc_loginPacket.pwd);
        //sm.Login(clientSession, sc_loginPacket.id, sc_loginPacket.pwd, 0);
       
    }

    public static void CS_QuizHandler(PacketSession session, IPacket packet)
    {
        CS_Quiz pkt = packet as CS_Quiz;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.QuizResult(clientSession, pkt);
    }
    public static void CS_QuizOXHandler(PacketSession session, IPacket packet)
    {
        CS_QuizOX pkt = packet as CS_QuizOX;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.QuizOXResult(clientSession, pkt);
    }

    public static void CS_ScreenResultHandler(PacketSession session, IPacket packet)
    {
        CS_ScreenResult Screen_packet = packet as CS_ScreenResult;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.ScreenResult(clientSession, Screen_packet);
    }

    public static void CS_QustionTextHandler(PacketSession session, IPacket packet)
    {
        CS_QustionText pkt = packet as CS_QustionText;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.QustionTextRequest(clientSession, pkt);
    }
     
    public static void CS_QustionImgHandler(PacketSession session, IPacket packet)
    {
        CS_QustionImg pkt = packet as CS_QustionImg;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.QustionImgRequest(clientSession, pkt);
    }
     
    public static void CS_QustionHandler(PacketSession session, IPacket packet)
    {
        CS_Qustion pkt = packet as CS_Qustion;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.QustionRequest(clientSession, pkt);
    }
     
    public static void CS_AtdCheckHandler(PacketSession session, IPacket packet)
    {
        CS_AtdCheck pkt = packet as CS_AtdCheck;
        ClientSession clientSession = session as ClientSession;

        Program.sessionManager.AtdResult(clientSession, pkt);
    }
   


}