using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} using 이름
        // {1} 패킷 핸들러
        public static string handlerFormat =
@"using {0};
using System;
class PacketHandler
{{
    {1}
    

}}";
        // {0} 패킷 이름
        public static string handlerMemberFormat =
@"     
        public static void {0}Handler(PacketSession session, IPacket packet)
        {{

        }}";
        
    // {0} 패킷 등록
    // {1} using 이름
    public static string managerFormat =
@"using {1};
using System;
using System.Collections.Generic;

public class PacketManager
{{
    

    public PacketManager()
    {{
        Register();
    }}

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {{
{0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        int count = 0;

        int size = BitConverter.ToInt32(buffer.Array, buffer.Offset);
        count += sizeof(int);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;


        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
    }}

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }}
}}";

        // {0} 패킷 이름
        public static string managerRegisterFormat =
@"  
        _onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);";
        // {0} 패킷 이름/번호 목록
        // {1} 패킷 목록
        // {2} using 
        public static string fillFormat =
@"using System;
using System.Collections.Generic;
using System.Net;
using System.Text;


public enum PacketID
{{
    {0}
}}

interface IPacket
{{
	ushort Protocol {{ get;  }}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}

{1}
";

        public static string packetEnumFormat = 
@"{0} = {1},";

        // {0} 패킷이름
        // {1} 멤버 변수들
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write

        public static string packetFormat =
@"
class {0} : IPacket
{{
    {1}    
    // 프로토콜 구분   
    public ushort Protocol {{ get {{ return (ushort)PacketID.{0}; }} }}

    public  void Read(ArraySegment<byte> segment)
    {{
        // 배열 현재 위치 초기화
        int count = 0;
        // 전체 데이터 사이즈
        BitConverter.ToInt32(segment.Array, segment.Offset + count);
        // 배열 현재 위치 이동
        count += sizeof(int);
        // 배열 현재 위치 이동
        count += sizeof(ushort);
        
        {2}
       

    }}

    public  ArraySegment<byte> Write()
    {{
        // 버퍼 짤라서 이동시킬 크기     
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);   
        // 배열 현재 위치 초기화
        int count = 0;        
        // 전체 데이터 사이즈 (마지막에 합쳐서 넣을것이므로 여기서는 인트 크기만큼만 배열의 현재 위치를 미리 옮겨준다. )
        count += sizeof(int);
        // 프로토콜 지정
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.{0}), 0, segment.Array, segment.Offset + count, sizeof(ushort));
       // 배열 현재 위치 이동
        count += sizeof(ushort);

        {3}
        // 전체 데이터사이즈를 배열 처음부터 인트크기만큼 넣어준다.
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(int));

        return SendBufferHelper.Close(count);


    }}
}}
";
        // {0} 변수의 형식
        // {1} 변수 이름
        public static string memberFormat =
@"public {0} {1};";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버 변수들
        // {3} 멤버 변수 Read
        // {4} 멤버 변수 Write
        public static string memberListFormat =
@"public class {0}
{{
   {2}

    // 데이터 읽어오는 부분
    public void Read(ArraySegment<byte> segment, ref int count)
    {{
       {3}
    }}

    // 데이터 쓰는 부분
    public bool Write(ArraySegment<byte> segment, ref int count)
    {{
        bool success = true;
        {4}
        return success;
    }}

    

}}
public List<{0}> {1}s = new List<{0}>();
";

        // {0} 변수 이름 
        // {1} To~ 변수 형식
        // {2} 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(segment.Array, segment.Offset + count);
count += sizeof({2});";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";

        // {0} 변수이름
        public static string readBytesFormat =
@"int {0}Len = BitConverter.ToInt32(segment.Array, segment.Offset + count);
count += sizeof(int);
ArraySegment<byte> {0}Array;
{0}Array = segment.Slice(segment.Offset + count, {0}Len);
this.{0} = {0}Array.ToArray();      
count += {0}Len;
";

        // {0} 변수 이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string readListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s.Slice(segment.Array, segment.Offset + count));
count += sizeof(ushort); 
for(int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeFormat =
@"Array.Copy(BitConverter.GetBytes({0}), 0, segment.Array, segment.Offset + count, sizeof({1}));
count += sizeof({1});";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";

        // {0} 변수이름 
        public static string writeBytesFormat =
@"int {0}Len = (int)this.{0}.Length;
 Array.Copy(BitConverter.GetBytes({0}Len), 0, segment.Array, segment.Offset + count, sizeof(int));
 Array.Copy(this.img, 0, segment.Array, segment.Offset + count + sizeof(int), {0}Len);
 count += sizeof(int);
 count += {0}Len;";

        // {0} 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
Array.Copy(BitConverter.GetBytes({0}Len), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);
count += {0}Len;";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string writeListFormat =
@"Array.Copy(BitConverter.GetBytes((ushort)this.{1}s.Count), 0, segment.Array, segment.Offset + count, sizeof({ushort}));
count += sizeof(ushort);
foreach({0} {1} in {1}s)
    {1}.Write(segment, ref count);";

    }
}
