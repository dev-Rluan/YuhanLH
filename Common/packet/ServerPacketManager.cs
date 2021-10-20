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
  
        _onRecv.Add((ushort)PacketID.CS_Login, MakePacket<CS_Login>);
        _handler.Add((ushort)PacketID.CS_Login, PacketHandler.CS_LoginHandler);
  
        _onRecv.Add((ushort)PacketID.CP_Chat, MakePacket<CP_Chat>);
        _handler.Add((ushort)PacketID.CP_Chat, PacketHandler.CP_ChatHandler);
  
        _onRecv.Add((ushort)PacketID.CS_Chat, MakePacket<CS_Chat>);
        _handler.Add((ushort)PacketID.CS_Chat, PacketHandler.CS_ChatHandler);
  
        _onRecv.Add((ushort)PacketID.CP_ScreenRequest, MakePacket<CP_ScreenRequest>);
        _handler.Add((ushort)PacketID.CP_ScreenRequest, PacketHandler.CP_ScreenRequestHandler);
  
        _onRecv.Add((ushort)PacketID.CS_ScreenResult, MakePacket<CS_ScreenResult>);
        _handler.Add((ushort)PacketID.CS_ScreenResult, PacketHandler.CS_ScreenResultHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        int count = 0;

        int size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
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