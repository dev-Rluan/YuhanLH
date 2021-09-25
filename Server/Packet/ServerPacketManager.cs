using Server;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance
    {
        get{ return _instance; }
    }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {
  
        _onRecv.Add((ushort)PacketID.PC_Login, MakePacket<PC_Login>);
        _handler.Add((ushort)PacketID.PC_Login, PacketHandler.PC_LoginHandler);
  
        _onRecv.Add((ushort)PacketID.SC_Login, MakePacket<SC_Login>);
        _handler.Add((ushort)PacketID.SC_Login, PacketHandler.SC_LoginHandler);
  
        _onRecv.Add((ushort)PacketID.PC_Chat, MakePacket<PC_Chat>);
        _handler.Add((ushort)PacketID.PC_Chat, PacketHandler.PC_ChatHandler);
  
        _onRecv.Add((ushort)PacketID.SC_Chat, MakePacket<SC_Chat>);
        _handler.Add((ushort)PacketID.SC_Chat, PacketHandler.SC_ChatHandler);
  
        _onRecv.Add((ushort)PacketID.PC_ScreenRequest, MakePacket<PC_ScreenRequest>);
        _handler.Add((ushort)PacketID.PC_ScreenRequest, PacketHandler.PC_ScreenRequestHandler);
  
        _onRecv.Add((ushort)PacketID.SC_ScreenResult, MakePacket<SC_ScreenResult>);
        _handler.Add((ushort)PacketID.SC_ScreenResult, PacketHandler.SC_ScreenResultHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
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