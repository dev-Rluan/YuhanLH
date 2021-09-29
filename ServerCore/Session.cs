using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
   
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            
            int processLen = 0;
            int packetCount = 0;
            while (true)
            {
                // 최소한 헤더는 파싱 할 수 있는지 검사
                if(buffer.Count  < HeaderSize)
                    break;

                // 패킷이 완전체로 도작했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                // 여기까지왔으면 패킷 조립 가능

                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset , dataSize ));

                packetCount++;
                // 처리한 데이터 수 
                processLen += dataSize;

                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
                
            }

            if(packetCount > 1)
                Console.WriteLine($"패킷 모아보내기 : {packetCount}");

            return processLen;
        }
        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

     public abstract  class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();        
        List<ArraySegment<byte>>    _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();        
       

        object _lock = new object();
        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisConnected(EndPoint endPoint);

        void Clear()
        {
            lock (_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }


        public void Start(Socket socket)
        {
            _socket = socket;
            
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
         
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegistRecv();
        }
        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if(sendBuffList.Count == 0)
            {
                return;
            }

            //_socket.Send(sendBuffer);
            lock (_lock)
            {
                foreach(ArraySegment<byte> sendBuffer in sendBuffList)
                     _sendQueue.Enqueue(sendBuffer);

                if (_pendingList.Count == 0)
                {
                    RegistSend();
                }
            }
        }
        public void Send(ArraySegment<byte> sendBuffer)
        {
            //_socket.Send(sendBuffer);
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuffer);
                if(_pendingList.Count == 0)
                {
                     RegistSend();
                }
            }  
        }
        public void Disconnect()
        {
           if( Interlocked.Exchange(ref _disconnected, 1) == 1)
            { 
                return;
            }
            OnDisConnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Clear();
        }



        #region 네트워크 통신

        void RegistSend()
        {
            if(_disconnected == 1)
            {
                return;
            }
            
                while (_sendQueue.Count > 0)
                {
                    ArraySegment<byte> buff = _sendQueue.Dequeue();
                    _pendingList.Add(buff);
                }
                _sendArgs.BufferList = _pendingList;
            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                {
                    OnSendCompleted(null, _sendArgs);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"RegisterSend Faild{e}");
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);
                        

                        if(_sendQueue.Count > 0)
                        {
                            RegistSend();
                        }                
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnRecvCompleted Faild {e}");
                    }
                }
                else
                {
                    Disconnect();
                }

            }
        }
        void RegistRecv()
        {
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                {
                    OnRecvCompleted(null, _recvArgs);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"RegisterRecv Faild {e}");
            }

        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // write 커서 이동
                    if (_recvBuffer.OnWrite(args.BytesTransferred) ==  false)
                    {
                        Disconnect();
                        return;
                    }

                    //컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는가를 받는다.
                    int processLen = OnRecv(_recvBuffer.ReadSegment);    
                    if(processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    //Read 커서 이동
                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegistRecv();
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Faild {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}
