start ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../DummyClient/Packet"
XCOPY /Y GenPackets.cs "../../Server/Packet"
XCOPY /Y GenPackets.cs "E:\Project\Unity\MMOServer\Client\Assets\Script\Packet"
XCOPY /Y ClientPacketManager.cs "E:\Project\Unity\MMOServer\Client\Assets\Script\Packet"
XCOPY /Y ClientPacketManager.cs "../../DummyClient/Packet"
XCOPY /Y ServerPacketManager.cs "../../Server/Packet"
