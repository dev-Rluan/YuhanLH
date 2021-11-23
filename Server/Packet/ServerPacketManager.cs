using Server;
using System;
using System.Collections.Generic;

public class PacketManager
{
    
    public PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {
  
        _onRecv.Add((ushort)PacketID.CP_Login, MakePacket<CP_Login>);
        _handler.Add((ushort)PacketID.CP_Login, PacketHandler.CP_LoginHandler);
  
        _onRecv.Add((ushort)PacketID.CP_ScreenRequest, MakePacket<CP_ScreenRequest>);
        _handler.Add((ushort)PacketID.CP_ScreenRequest, PacketHandler.CP_ScreenRequestHandler);
  
        _onRecv.Add((ushort)PacketID.CP_QuizOX, MakePacket<CP_QuizOX>);
        _handler.Add((ushort)PacketID.CP_QuizOX, PacketHandler.CP_QuizOXHandler);
  
        _onRecv.Add((ushort)PacketID.CP_Quiz, MakePacket<CP_Quiz>);
        _handler.Add((ushort)PacketID.CP_Quiz, PacketHandler.CP_QuizHandler);
  
        _onRecv.Add((ushort)PacketID.CP_QResult, MakePacket<CP_QResult>);
        _handler.Add((ushort)PacketID.CP_QResult, PacketHandler.CP_QResultHandler);
  
        _onRecv.Add((ushort)PacketID.CP_Atd, MakePacket<CP_Atd>);
        _handler.Add((ushort)PacketID.CP_Atd, PacketHandler.CP_AtdHandler);
  
        _onRecv.Add((ushort)PacketID.CP_StudentList, MakePacket<CP_StudentList>);
        _handler.Add((ushort)PacketID.CP_StudentList, PacketHandler.CP_StudentListHandler);
  
        _onRecv.Add((ushort)PacketID.CP_EndOfClass, MakePacket<CP_EndOfClass>);
        _handler.Add((ushort)PacketID.CP_EndOfClass, PacketHandler.CP_EndOfClassHandler);
  
        _onRecv.Add((ushort)PacketID.CS_Login, MakePacket<CS_Login>);
        _handler.Add((ushort)PacketID.CS_Login, PacketHandler.CS_LoginHandler);
  
        _onRecv.Add((ushort)PacketID.CS_Quiz, MakePacket<CS_Quiz>);
        _handler.Add((ushort)PacketID.CS_Quiz, PacketHandler.CS_QuizHandler);
  
        _onRecv.Add((ushort)PacketID.CS_ScreenResult, MakePacket<CS_ScreenResult>);
        _handler.Add((ushort)PacketID.CS_ScreenResult, PacketHandler.CS_ScreenResultHandler);
  
        _onRecv.Add((ushort)PacketID.CS_QustionText, MakePacket<CS_QustionText>);
        _handler.Add((ushort)PacketID.CS_QustionText, PacketHandler.CS_QustionTextHandler);
  
        _onRecv.Add((ushort)PacketID.CS_QustionImg, MakePacket<CS_QustionImg>);
        _handler.Add((ushort)PacketID.CS_QustionImg, PacketHandler.CS_QustionImgHandler);
  
        _onRecv.Add((ushort)PacketID.CS_Qustion, MakePacket<CS_Qustion>);
        _handler.Add((ushort)PacketID.CS_Qustion, PacketHandler.CS_QustionHandler);
  
        _onRecv.Add((ushort)PacketID.CS_AtdCheck, MakePacket<CS_AtdCheck>);
        _handler.Add((ushort)PacketID.CS_AtdCheck, PacketHandler.CS_AtdCheckHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        int count = 0;

        int size = BitConverter.ToInt32(buffer.Array, buffer.Offset);
        count += sizeof(int);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;


        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }
}