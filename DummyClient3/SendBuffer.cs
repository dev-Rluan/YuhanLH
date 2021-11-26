using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyClient3
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; }) ;
        public static int ChunkSize { get; set; } = 65535 * 1000;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if(CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
             
            if(CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }

    }
    public class SendBuffer
    {

        byte[] _buffer ;
        int _usedSize = 0 ;

        //사용이 가능한 남은 버퍼 크기
        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

       public ArraySegment<byte> Open(int recvSIze)
        {
            if(recvSIze > FreeSize)
            {
                return null;
            }
            //버퍼에서 받은 데이터 크기만큼 버퍼에서 끌고온다
            return new ArraySegment<byte>(_buffer, _usedSize, recvSIze);
        }
        public ArraySegment <byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }
    }
}
