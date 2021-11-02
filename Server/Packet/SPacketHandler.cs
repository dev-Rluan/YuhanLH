using Server;
using System;
class PacketHandler
{

    public static void CP_LoginHandler(PacketSession session, IPacket packet)
    {
        CP_Login pc_loginPacket = packet as CP_Login;
        ClientSession clientSession = session as ClientSession;

        //if (clientSession.Room == null)
        //    return;
        //ClassRoom room = clientSession.Room;
        //room.Push(
        //    () => room.Broadcast(clientSession, pc_loginPacket.id));
        SP_Result r_packet = new SP_Result();
        switch (pc_loginPacket.id, pc_loginPacket.pwd)
        {
            case ("Ptest", "1234"):
                r_packet.result = true;
                Console.WriteLine($"{pc_loginPacket.id} 로그인요청 Success");
                break;
            case ("Stest", "1234"):
                r_packet.result = true;
                Console.WriteLine($"{pc_loginPacket.id} 로그인요청 Success");
                break;
            default:
                Console.WriteLine($"{pc_loginPacket.id} 로그인요청 Faild");
                break;
        }

        
        ArraySegment<byte> segment = r_packet.Write();
        clientSession.Send(segment);

        ClassRoom room = new ClassRoom();
        room.P_Enter(clientSession, pc_loginPacket);

    }


    public static void CP_ChatHandler(PacketSession session, IPacket packet)
    {
        
    }

    public static void CP_ScreenRequestHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CP_QuizOXHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CP_QuizHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CP_QResultHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CP_AtdHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CP_StudentListHandler(PacketSession session, IPacket packet)
    {

    }

    public static void CS_LoginHandler(PacketSession session, IPacket packet)
    {
        CS_Login sc_loginPacket = packet as CS_Login;
        ClientSession clientSession = session as ClientSession;
        SS_Result r_packet = new SS_Result();
        if (sc_loginPacket.id == "test" || sc_loginPacket.pwd == "1234")
        {
            r_packet.result = true;
            Console.WriteLine($"{sc_loginPacket.id} 로그인요청 Success");
        }
        else
        {
            r_packet.result = false;
            Console.WriteLine($"{sc_loginPacket.id} 로그인요청 Faild");
        }
        ArraySegment<byte> segment = r_packet.Write();
        clientSession.Send(segment);
    }

    public static void CS_QuizHandler(PacketSession session, IPacket packet)
    {

    }

    public static void CS_ScreenResultHandler(PacketSession session, IPacket packet)
    {
        CS_ScreenResult Screen_packet = packet as CS_ScreenResult;
        ClientSession clientSession = session as ClientSession;
        ClassRoom room = clientSession.Room;
        room.Push(() => room.Img_Broadcast(clientSession, Screen_packet.img, Screen_packet.studentID));


    }

    public static void CS_QustionTextHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CS_QustionImgHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CS_QustionHandler(PacketSession session, IPacket packet)
    {

    }
     
    public static void CS_AtdCheckHandler(PacketSession session, IPacket packet)
    {

    }

    

}