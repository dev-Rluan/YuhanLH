using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {

        static string genPackets;
        static ushort packetId;
        static string packetEnum;

        static string pcPacketHandler;
        static string scPacketHandler;
        static string sPacketHandler;
        


        static string pclientRegister;
        static string sclientRegister;
        static string serverRegister;


        static void Main(string[] args)
        {
            string pdlPath = "../PDL.xml";

            //xml파일을 읽어들이는 세팅값
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                //주석을 읽지않는다.
                IgnoreComments = true,
                //스페이스바를 읽지 않는다.
                IgnoreWhitespace = true
            };

            if (args.Length >= 1)
                pdlPath = args[0];

            //using을 사용하면 유징에서 나올때 알아서 dispose를 호출해준다.
            using (XmlReader r = XmlReader.Create(pdlPath, settings))
            {
                r.MoveToContent();
                while (r.Read())
                {   
                    // Depth - 0부터 시작해서 몇개로 파고드는지
                    // NodeType.Element - 패킷의 정보시작
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePacket(r);
                    // Console.WriteLine(r.Name + " " + r["name"]); 
                }

                string fileText = string.Format(PacketFormat.fillFormat, packetEnum, genPackets);
                File.WriteAllText("GenPackets.cs", fileText);
                string pclientText = string.Format(PacketFormat.managerFormat, pclientRegister, "PClient");
                File.WriteAllText("PClientPacketManager.cs", pclientText);
                string sclientText = string.Format(PacketFormat.managerFormat, sclientRegister, "SClient");
                File.WriteAllText("SClientPacketManager.cs", sclientText);
                string ServerText = string.Format(PacketFormat.managerFormat, serverRegister, "Server");
                File.WriteAllText("ServerPacketManager.cs", ServerText);

                string pcPacketHandlerText = string.Format(PacketFormat.handlerFormat, "PClient", pcPacketHandler);
                File.WriteAllText("PCPacketHandler.cs", pcPacketHandlerText);
                string scPacketHandlerText = string.Format(PacketFormat.handlerFormat, "SClient", scPacketHandler); ;
                File.WriteAllText("SCPacketHandler.cs", scPacketHandlerText);
                string sPacketHandlerText = string.Format(PacketFormat.handlerFormat, "Server", sPacketHandler); ;
                File.WriteAllText("SPacketHandler.cs", sPacketHandlerText);


                


            }

        }

        public static void ParsePacket(XmlReader r) 
        {
            // 어쩌다가 잘못왔을때 리턴시켜줌
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            // r.Name을 소문자로 바꿔서 확인했을때 packet이 아니라면 리턴(안전을 위해서)
            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }
                

            string packetName = r["name"];
            // 비어있는지 체크
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet witout name");
                return;
            }

            Tuple<string, string, string> t = ParseMembers(r);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
            packetEnum += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + Environment.NewLine + "\t";

            



            if (packetName.StartsWith("SP_") || packetName.StartsWith("sp_") || packetName.StartsWith("Sp_") || packetName.StartsWith("sP_"))
            {
                pclientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
                pcPacketHandler += string.Format(PacketFormat.handlerMemberFormat, packetName);
            }                
            if(packetName.StartsWith("SS_") || packetName.StartsWith("ss_") || packetName.StartsWith("Ss_") || packetName.StartsWith("sS_"))
            {
                sclientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
                scPacketHandler += string.Format(PacketFormat.handlerMemberFormat, packetName);
            }                
            if(packetName.StartsWith("PC_") || packetName.StartsWith("pc_") || packetName.StartsWith("Pc_") || packetName.StartsWith("pC_") || 
                packetName.StartsWith("SC_") || packetName.StartsWith("sc_") || packetName.StartsWith("Sc_") || packetName.StartsWith("sC_"))
            {
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
                sPacketHandler += string.Format(PacketFormat.handlerMemberFormat, packetName) + Environment.NewLine;
                
            }

        }

        public static Tuple<string, string, string> ParseMembers(XmlReader r)
        {
            // 패킷 네임 추출 
            string packetName = r["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            
            int depth = r.Depth + 1;
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                // 멤버의 이름이 부정확하면 리턴
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member witout name");
                    return null;
                }

                
                if(string.IsNullOrEmpty(memberCode) == false)
                {
                    // 이미 뭔가 내용이있으면 엔터를 쳐준다.
                    memberCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(readCode) == false)
                {
                    // 이미 뭔가 내용이있으면 엔터를 쳐준다.
                    readCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(writeCode) == false)
                {
                    // 이미 뭔가 내용이있으면 엔터를 쳐준다.
                    writeCode += Environment.NewLine;
                }

                // 소문자로 패킷아이디를 맞춰주기 위해서 ToLower 사용해서 이름 가져오기
                string memberType = r.Name.ToLower();
                switch (memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":                   
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(r);
                        memberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;
                    default:
                        break;
                }
            }
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);

        }

        public static Tuple<string, string, string>ParseList(XmlReader r)
        {
            string listName = r["name"];
            //리스트 네임이 비어있으면 null반환
            if (string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without");
                return null;
            }

            Tuple<string, string, string> t = ParseMembers(r);

            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName),
                t.Item1,
                t.Item2,
                t.Item3);
            
            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);

        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType) 
            {
                case "bool":
                    return "ToBoolean";
                case "short": 
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }

        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }

    }
}
