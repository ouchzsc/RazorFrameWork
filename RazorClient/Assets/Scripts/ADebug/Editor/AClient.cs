using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Aio;

namespace ADebugger
{
    public class AClient
    {
        public delegate void RecvHandler(string line);

        public RecvHandler OnRecv;

        private const int InputSize = 4096;
        private const int ReserveInputBufSize = 8192;
        private const int ReserveOutputBufSize = 1024;

        private readonly Octets _inputBuf = new Octets(ReserveInputBufSize);
        private readonly Octets _outputBuf = new Octets(ReserveOutputBufSize);
        private readonly byte[] _input = new byte[InputSize];
        private readonly ConcurrentQueue<IoAction> _actions = new ConcurrentQueue<IoAction>();

        private readonly Socket _socket;

        public AClient(string ip, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), ConnectCallback, null);
        }

        public bool IsConnected()
        {
            return _socket.Connected;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Push(ConnectedInMainThread);
        }

        private void ConnectedInMainThread()
        {
            BeginReceive();
        }

        internal void Start()
        {
            BeginReceive();
        }


        internal void Close()
        {
            _socket.Close();
        }

        private void Push(IoAction action)
        {
            _actions.Enqueue(action);
        }

        private void BeginReceive()
        {
            try
            {
                _socket.BeginReceive(_input, 0, InputSize, SocketFlags.None, BeginReceiveCallback, _socket);
            }
            catch (Exception)
            {
                Close();
            }
        }

        private void BeginReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int received = _socket.EndReceive(ar);
                Push(() => { ReceivedSomethingInMainThread(received); });
            }
            catch (Exception)
            {
                Push(Close);
            }
        }

        private void ReceivedSomethingInMainThread(int received)
        {
            _inputBuf.Append(_input, 0, received);

            while (true)
            {
                int fullMsgLength = 0;
                for (var i = 0; i < _inputBuf.Count; i++)
                {
                    if (_inputBuf.GetByte(i) == ' ')
                    {
                        int msgLengthPrefixLen = i + 1;
                        fullMsgLength = msgLengthPrefixLen + int.Parse(Encoding.UTF8.GetString(_inputBuf.ByteArray, 0, i));
                        break;
                    }
                }

                if (fullMsgLength > 0 && _inputBuf.Count >= fullMsgLength)
                {
                    string msg = Encoding.UTF8.GetString(_inputBuf.ByteArray, 0, fullMsgLength);

                    if (OnRecv != null)
                    {
                        OnRecv(msg);
                    }

                    _inputBuf.EraseAndCompact(fullMsgLength, ReserveInputBufSize);
                }
                else
                {
                    break;
                }
            }

            BeginReceive();
        }


        private void BeginSend()
        {
            try
            {
                _socket.BeginSend(_outputBuf.ByteArray, 0, _outputBuf.Count, SocketFlags.None, BeginSendCallback,
                    _socket);
            }
            catch (Exception)
            {
                Close();
            }
        }

        private void BeginSendCallback(IAsyncResult ar)
        {
            try
            {
                int sent = _socket.EndSend(ar);
                Push(() => { SentSomethingInMainThread(sent); });
            }
            catch (Exception)
            {
                Push(Close);
            }
        }

        private void SentSomethingInMainThread(int sent)
        {
            _outputBuf.EraseAndCompact(sent, ReserveOutputBufSize);
            if (_outputBuf.Count > 0)
                BeginSend();
        }

        public int Send(string inf)
        {
            if (!inf.EndsWith("\n"))
            {
                inf = inf + "\n";
            }

            byte[] bytes = Encoding.UTF8.GetBytes(inf);
            var emptyBeforeAdd = (_outputBuf.Count == 0);
            _outputBuf.Append(bytes);

            if (emptyBeforeAdd)
                BeginSend();
            return 0;
        }

        public void Process()
        {
            while (true)
            {
                IoAction ioAction;
                if (_actions.TryDequeue(out ioAction))
                {
                    ioAction();
                }
                else
                {
                    break;
                }
            }
        }
    }
}