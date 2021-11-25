using DummyClient2;
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
  
        _onRecv.Add((ushort)PacketID.SP_Result, MakePacket<SP_Result>);
        _handler.Add((ushort)PacketID.SP_Result, PacketHandler.SP_ResultHandler);
  
        _onRecv.Add((ushort)PacketID.SP_LoginFailed, MakePacket<SP_LoginFailed>);
        _handler.Add((ushort)PacketID.SP_LoginFailed, PacketHandler.SP_LoginFailedHandler);
  
        _onRecv.Add((ushort)PacketID.SP_LoginResult, MakePacket<SP_LoginResult>);
        _handler.Add((ushort)PacketID.SP_LoginResult, PacketHandler.SP_LoginResultHandler);
  
        _onRecv.Add((ushort)PacketID.SP_StudentInfo, MakePacket<SP_StudentInfo>);
        _handler.Add((ushort)PacketID.SP_StudentInfo, PacketHandler.SP_StudentInfoHandler);
  
        _onRecv.Add((ushort)PacketID.SP_ScreenResult, MakePacket<SP_ScreenResult>);
        _handler.Add((ushort)PacketID.SP_ScreenResult, PacketHandler.SP_ScreenResultHandler);
  
        _onRecv.Add((ushort)PacketID.SP_QustionText, MakePacket<SP_QustionText>);
        _handler.Add((ushort)PacketID.SP_QustionText, PacketHandler.SP_QustionTextHandler);
  
        _onRecv.Add((ushort)PacketID.SP_QustionImg, MakePacket<SP_QustionImg>);
        _handler.Add((ushort)PacketID.SP_QustionImg, PacketHandler.SP_QustionImgHandler);
  
        _onRecv.Add((ushort)PacketID.SP_Qustion, MakePacket<SP_Qustion>);
        _handler.Add((ushort)PacketID.SP_Qustion, PacketHandler.SP_QustionHandler);
  
        _onRecv.Add((ushort)PacketID.SP_QuizResult, MakePacket<SP_QuizResult>);
        _handler.Add((ushort)PacketID.SP_QuizResult, PacketHandler.SP_QuizResultHandler);
  
        _onRecv.Add((ushort)PacketID.SP_QuizOXResult, MakePacket<SP_QuizOXResult>);
        _handler.Add((ushort)PacketID.SP_QuizOXResult, PacketHandler.SP_QuizOXResultHandler);
  
        _onRecv.Add((ushort)PacketID.SP_AddStudent, MakePacket<SP_AddStudent>);
        _handler.Add((ushort)PacketID.SP_AddStudent, PacketHandler.SP_AddStudentHandler);
  
        _onRecv.Add((ushort)PacketID.SP_LeaveStudent, MakePacket<SP_LeaveStudent>);
        _handler.Add((ushort)PacketID.SP_LeaveStudent, PacketHandler.SP_LeaveStudentHandler);
  
        _onRecv.Add((ushort)PacketID.SP_AddAtd, MakePacket<SP_AddAtd>);
        _handler.Add((ushort)PacketID.SP_AddAtd, PacketHandler.SP_AddAtdHandler);
  
        _onRecv.Add((ushort)PacketID.SP_EndClass, MakePacket<SP_EndClass>);
        _handler.Add((ushort)PacketID.SP_EndClass, PacketHandler.SP_EndClassHandler);

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