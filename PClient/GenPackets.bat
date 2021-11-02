start ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../SClient/Packet"
XCOPY /Y GenPackets.cs "../../PClient/Packet"
XCOPY /Y GenPackets.cs "../../Server/Packet"
XCOPY /Y PClientPacketManager.cs "../../PClient/Packet"
XCOPY /Y SClientPacketManager.cs "../../SClient/Packet"
XCOPY /Y ServerPacketManager.cs "../../Server/Packet"
XCOPY /Y CPPacketHandler.cs "../../PClient/Packet"
XCOPY /Y CSPacketHandler.cs "../../SClient/Packet"
XCOPY /Y SPacketHandler.cs "../../Server/Packet"

pause

