using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient2
{
    public class RecvBuffer
    {
        // [] [] [] [] [] [] [] [] [] [] [] 
        ArraySegment<byte> _buffer;
        //버퍼의 커서 위치
        int _readPos;
        int _writePos;


        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);

        }

        //유효 범위 얼마나의 데이터가 쌓여있는지 
        public int DataSize { get { return _writePos - _readPos; } }
        //버퍼의 남은 공간(_buffer.Count(버퍼의 전체 크기))
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        //현재까지 받은 데이터의 유효범위가 어디까지인지 
        public ArraySegment<byte> ReadSegment 
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        // 리시브를 할때 어디서부터 어디까지가 유효범위인지 나타내 준다.
        public ArraySegment<byte> WriteSegment
        {  
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                //남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;

            }
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if(numOfBytes > DataSize)
            {
                return false;
            }
            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if(numOfBytes > FreeSize)
            {
                return false;
            }
            _writePos += numOfBytes;
            return true;
        }
    }


}
