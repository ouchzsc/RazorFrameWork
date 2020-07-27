using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Aio
{
    // 连接状态
    public enum ConnectState
    {
        Unconnected,        // 未连接
        Connecting,         // 连接中
        Connected,          // 已连接
    }

    //协议处理模式
    public enum NetMode
    {
        ProcessAll,
        ProcessOnly,
        ProcessPause
    }

    public sealed class Link
    {
        public delegate void NetExceptionHandler(string detail, string reason);
        public delegate void RecvProtocolHandler(int type, OctetsStream data);
        public delegate void ProtocolExceedHandler(int processedSize, int exceedSize);
        public delegate void ConnectedHandler();

        public IPAddress HostAddress { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public int ReceiveBufferSize { get; private set; }
        public int SendBufferSize { get; private set; }
        public int OutputBufferSize { get; private set; }

        public ConnectedHandler OnConnected;
        public NetExceptionHandler OnNetException;
        public RecvProtocolHandler OnRecvProtocol;

        private Socket _socket;
        public ConnectState ConnectState { get; private set; }
        private readonly ConcurrentQueue<IoAction> _connActionQueue = new ConcurrentQueue<IoAction>();

        private readonly Stopwatch _frameWatcher = new Stopwatch();
        private readonly List<ProtocolStruct> _protocolStrs = new List<ProtocolStruct>();


        private NetMode _processMode = NetMode.ProcessAll;
        private int _processOnlyProtocolType;

        //private const int InputSize = 4096;
        //private const int ReserveInputBufSize = 8192;
        //private const int ReserveOutputBufSize = 1024;


        private const int InputSize = 8192; //每次从系统读64k        
        private const int ReserveInputBufSize = 16384; //每次保留64k
        private const int ReserveOutputBufSize = 8192; //每次保留64k

        private readonly Octets _inputBuf = new Octets(ReserveInputBufSize);
        private readonly Octets _outputBuf = new Octets(ReserveOutputBufSize);
        private readonly byte[] _input = new byte[InputSize];
        private ISecurity _inputSecurity = NullSecurity.Instance;
        private ISecurity _outputSecurity = NullSecurity.Instance;

        private readonly OctetsStream _sendos = new OctetsStream();
        private readonly Octets _recvo = new Octets();
        private readonly OctetsStream _recvos = new OctetsStream();

        public Link(IPAddress host, int port, int receiveBufferSize, int sendBufferSize, int outputBufferSize) 
            : this(host.ToString(), port, receiveBufferSize, sendBufferSize, outputBufferSize)
        {
            HostAddress = host;
        }

        public Link(string host, int port, int receiveBufferSize, int sendBufferSize, int outputBufferSize)
        {
            Host = host;
            Port = port;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            OutputBufferSize = outputBufferSize;

            ConnectState = ConnectState.Unconnected;
        }

        public byte[] OutputSecurity
        {
            set { _outputSecurity = new Arc4Security { Parameter = new Octets(value) }; }
        }

        public byte[] InputSecurity
        {
            set { _inputSecurity = new DecompressArc4Security { Parameter = new Octets(value) }; }
        }

        public void Connect()
        {
            Close();
            try
            {

                IPAddress address = HostAddress;
                if (address == null)
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(Host);
                    if (addresses.Length == 0)
                    {
                        throw new Exception("can not get ipaddress by host, host is " + Host);
                    }
                    address = addresses[0];
                }

                Socket socket = _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendBufferSize = SendBufferSize,
                    ReceiveBufferSize = ReceiveBufferSize,
                    NoDelay = true
                };

                //socket.BeginConnect 在IOS下应该是有BUG的，有时候连接无回调
                //复现方式：先连一个无法连接的IP，然后再链接一个正常的IP，有概率连不上
                //socket.BeginSend在测试期间也有这个问题，BeginSend测试出的问题是每次都没有回调
                //现在改成：自己起线程
                new Thread(() =>
                {
                    try
                    {
                        socket.Connect(address, Port);
                        _connActionQueue.Enqueue(() => ConnectedCallback(socket));
                    }
                    catch (Exception e)
                    {
                        _connActionQueue.Enqueue(() => ConnectExceptionCallback(socket, e));
                    }
                }).Start();

                ConnectState = ConnectState.Connecting;
            }
            catch (Exception e)
            {
                ConnectExceptionCallback(_socket, e);
            }
        }

        private void ConnectedCallback(Socket socket)
        {
            if (socket == _socket)
            {
                ConnectState = ConnectState.Connected;
                if (OnConnected != null)
                {
                    OnConnected();
                }
            }
        }

        private void ConnectExceptionCallback(Socket socket, Exception e)
        {
            if (socket == _socket)
            {
                Close("ConnectFailed", e);
            }
            else
            {
                //对于主动关闭的连接，sock.EndConnect(ar); 会抛出 The object was used after being disposed.  此时 _socket = null
                socket.Close();
            }
        }

        public void Close()
        {
            ConnectState = ConnectState.Unconnected;
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }

            _protocolStrs.Clear();

            _inputBuf.Clear();
            _outputBuf.Clear();
            _inputSecurity = NullSecurity.Instance;
            _outputSecurity = NullSecurity.Instance;
        }

        private void Close(string detail, Exception e)
        {
            Close();
            if (OnNetException != null)
            {
                string reason = detail;
                detail = e == null ? (string.IsNullOrEmpty(detail) ? "unknow" : detail) : e.Message;
                OnNetException(detail, reason);
            }
        }

        public void Process(long maxMilliseconds = 20)
        {
            if (ConnectState == ConnectState.Connecting)
            {
                IoAction connAction;
                while (_connActionQueue.TryDequeue(out connAction))
                {
                    connAction();
                }
            }

            if (ConnectState != ConnectState.Connected)
            {
                return;
            }

            try
            {
                ProcessOutput();
                ProcessInput();
                ProcessProtocol(maxMilliseconds);
            }
            catch (Exception e)
            {
                Close("ExceptionOnUpdate", e);
            }
        }

        public void LateProcess()
        {
            if (ConnectState != ConnectState.Connected)
            {
                return;
            }

            try
            {
                ProcessOutput(); //数据尽早发出去
            }
            catch (Exception e)
            {
                Close("ExceptionOnLateUpdate", e);
            }
        }

        public void SetProcessAll()
        {
            _processMode = NetMode.ProcessAll;
        }

        public void SetProcessOnly(int ptype)
        {
            _processOnlyProtocolType = ptype;
            _processMode = NetMode.ProcessOnly;
        }

        public void SetProcessPause()
        {
            _processMode = NetMode.ProcessPause;
        }
        private void ProcessProtocol(long maxMilliseconds)
        {
            if (_processMode == NetMode.ProcessPause)
            {
                return;
            }

            _frameWatcher.Reset();
            _frameWatcher.Start();
            int processed = 0;
            while (_frameWatcher.ElapsedMilliseconds < maxMilliseconds)
            {
                ProtocolStruct? p = FetchOne();
                if (p == null)
                {
                    break;
                }
                if (OnRecvProtocol != null)
                {
                    processed++;
                    OnRecvProtocol(p.Value.Type, OctetsStream.Wrap(Octets.Wrap(p.Value.Data)));
                }
            }
        }

        private ProtocolStruct? FetchOne()
        {
            switch (_processMode)
            {
                case NetMode.ProcessPause:
                    return null;

                case NetMode.ProcessAll:
                    if (_protocolStrs.Count > 0)
                    {
                        ProtocolStruct p = _protocolStrs[0];
                        _protocolStrs.RemoveAt(0);
                        return p;
                    }
                    else
                    {
                        return null;
                    }
                case NetMode.ProcessOnly:
                    for (var i = 0; i < _protocolStrs.Count; i++)
                    {
                        ProtocolStruct p = _protocolStrs[i];
                        if (p.Type == _processOnlyProtocolType)
                        {
                            _protocolStrs.RemoveAt(i);
                            return p;
                        }
                    }
                    return null;
            }
            return null;
        }

        //0: ok, 1: NetUnconnected, 2: OutputBufferExceed
        public int SendProtocol(int type, OctetsStream octetsStream) 
        {
            if (ConnectState != ConnectState.Connected)
                return 1;

            //此时连接并没有断开，为了保证协议不丢失，在ProcessOutput中再断开连接（如果在此处就断开连接，可能会导致逻辑不一致吧？？？）
            if (_outputBuf.Count >= OutputBufferSize)
                return 2;

            try
            {
                _sendos.MarshalSize(type).MarshalOctets(octetsStream.Data);
                _outputBuf.Append(_outputSecurity.Update(_sendos.Data));
            }
            finally
            {
                _sendos.Reset();
                //_sendos.ResetWithSize(ReserveOutputBufSize);

            }

            return 0;
        }


        private void ProcessInput()
        {
            if (!_socket.Poll(0, SelectMode.SelectRead))
            {
                return;
            }

            int received = _socket.Receive(_input, _input.Length, SocketFlags.None);
            if (received <= 0)
            {
                throw new Exception("Socket.Receive received = " + received);
            }
            _inputBuf.Append(_inputSecurity.Update(_recvo.WrapBytes(_input, received)));
            var os = _recvos.WrapOctets(_inputBuf);
            
            while (os.Remaining > 0)
            {
                int tranpos = os.Begin();
                try
                {
                    int type = os.UnmarshalSize();
                    int size = os.UnmarshalSize();
                    if (size > os.Remaining)
                    {
                        os.Rollback(tranpos);
                        break; // not enough
                    }
                    _protocolStrs.Add(new ProtocolStruct(type, os.UnmarshalFixedSizeBytes(size)));

                    //因为回调里，可能Close了Link，则os底层的_inputBuf也已经clear了。
                    if (ConnectState != ConnectState.Connected)
                    {
                        return;
                    }

                }
                catch (MarshalException)
                {
                    os.Rollback(tranpos);
                    break;
                }
            }

            if (os.Position != 0)
            {
                _inputBuf.EraseAndCompact(os.Position, ReserveInputBufSize);
            }
        }

        private void ProcessOutput()
        {
            if (_outputBuf.Count == 0)
            {
                return;
            }

            if (_outputBuf.Count > OutputBufferSize)
            {
                // 如果输入流大小大于定义的大小，断开连接
                throw new Exception("Exceed OutputBufferSize. curr = " + _outputBuf.Count + ", max = " +
                                    OutputBufferSize);
            }

            // 清空输出流
            int sendByteOffset = 0;
            int lastSend = 0;
            while (_outputBuf.Count > sendByteOffset)
            {
                //不可写，等待下次Update继续判断
                if (!_socket.Poll(0, SelectMode.SelectWrite))
                {
                    break;
                }

                lastSend = _socket.Send(_outputBuf.ByteArray, sendByteOffset, _outputBuf.Count - sendByteOffset,
                    SocketFlags.None);

                if (lastSend <= 0)
                {
                    break;
                }

                sendByteOffset += lastSend;
            }

            if (lastSend < 0)
            {
                throw new Exception("Socket.Send return = " + lastSend);
            }

            if (sendByteOffset > 0)
            {
                _outputBuf.EraseAndCompact(sendByteOffset, ReserveOutputBufSize);
            }
        }
    }
}