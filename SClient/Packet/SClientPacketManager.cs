using SClient;
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
  
        _onRecv.Add((ushort)PacketID.SS_Result, MakePacket<SS_Result>);
        _handler.Add((ushort)PacketID.SS_Result, PacketHandler.SS_ResultHandler);
  
        _onRecv.Add((ushort)PacketID.SS_LoginFailed, MakePacket<SS_LoginFailed>);
        _handler.Add((ushort)PacketID.SS_LoginFailed, PacketHandler.SS_LoginFailedHandler);
  
        _onRecv.Add((ushort)PacketID.SS_LoginResult, MakePacket<SS_LoginResult>);
        _handler.Add((ushort)PacketID.SS_LoginResult, PacketHandler.SS_LoginResultHandler);
  
        _onRecv.Add((ushort)PacketID.SS_ScreenRequest, MakePacket<SS_ScreenRequest>);
        _handler.Add((ushort)PacketID.SS_ScreenRequest, PacketHandler.SS_ScreenRequestHandler);
  
        _onRecv.Add((ushort)PacketID.SS_QResult, MakePacket<SS_QResult>);
        _handler.Add((ushort)PacketID.SS_QResult, PacketHandler.SS_QResultHandler);
  
        _onRecv.Add((ushort)PacketID.SS_AtdRequest, MakePacket<SS_AtdRequest>);
        _handler.Add((ushort)PacketID.SS_AtdRequest, PacketHandler.SS_AtdRequestHandler);
  
        _onRecv.Add((ushort)PacketID.SS_QuizOX, MakePacket<SS_QuizOX>);
        _handler.Add((ushort)PacketID.SS_QuizOX, PacketHandler.SS_QuizOXHandler);
  
        _onRecv.Add((ushort)PacketID.SS_Quiz, MakePacket<SS_Quiz>);
        _handler.Add((ushort)PacketID.SS_Quiz, PacketHandler.SS_QuizHandler);
  
        _onRecv.Add((ushort)PacketID.SS_ImgSendFaild, MakePacket<SS_ImgSendFaild>);
        _handler.Add((ushort)PacketID.SS_ImgSendFaild, PacketHandler.SS_ImgSendFaildHandler);

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