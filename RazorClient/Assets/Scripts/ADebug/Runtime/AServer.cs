using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Aio;

namespace ADebugger
{
    public delegate void IoAction();

    public sealed class AServerSession
    {
        private const int InputSize = 4096;
        private const int ReserveInputBufSize = 8192;
        private const int ReserveOutputBufSize = 1024;

        private readonly Octets _inputBuf = new Octets(ReserveInputBufSize);
        private readonly Octets _outputBuf = new Octets(ReserveOutputBufSize);
        private readonly byte[] _input = new byte[InputSize];

        private readonly AServer _server;
        private readonly Socket _socket;
        private readonly int _id;

        public AServerSession(AServer server, Socket client, int id)
        {
            _server = server;
            _socket = client;
            _id = id;
        }

        internal void Start()
        {
            BeginReceive();
        }

        public int Id()
        {
            return _id;
        }

        private void Close()
        {
            _server.CloseSession(this);
        }

        internal void CloseThis()
        {
            _socket.Close();
        }

        private void Push(IoAction action)
        {
            _server.Push(action);
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
            bool found = false;
            int i;
            for (i = 0; i < received; i++)
            {
                byte c = _input[i];
                if (c == '\n')
                {
                    found = true;
                    break;
                }
            }

            int byteCount = _inputBuf.Count + i;
            _inputBuf.Append(_input, 0, received);

            if (found)
            {
                string line = Encoding.UTF8.GetString(_inputBuf.ByteArray, 0, byteCount);
                _server.ClientRecvLine(this, line.Trim());

                _inputBuf.EraseAndCompact(byteCount, ReserveInputBufSize);
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

        public int Send(string info)
        {
            string inf = info.Replace("\n", "\r\n");
            if (!inf.EndsWith("\r\n"))
            {
                inf = inf + "\r\n";
            }

            byte[] bytes = Encoding.UTF8.GetBytes(inf);

            var emptyBeforeAdd = (_outputBuf.Count == 0);
            string bytesLengthPrefix = string.Format("{0} ", bytes.Length);
            _outputBuf.Append(Encoding.UTF8.GetBytes(bytesLengthPrefix));
            _outputBuf.Append(bytes);

            if (emptyBeforeAdd)
                BeginSend();
            return 0;
        }
    }


    public sealed class AServer
    {
        public delegate void ClientRecvLineHandler(AServerSession serverSession, string line);

        public delegate void ClientAddRemoveHandler(AServerSession serverSession, bool isAdd);

        public ClientAddRemoveHandler OnClientAddRemove;
        public ClientRecvLineHandler OnClientRecvLine;

        private readonly ConcurrentQueue<IoAction> _actions = new ConcurrentQueue<IoAction>();
        private readonly HashSet<AServerSession> _clients = new HashSet<AServerSession>();

        private Socket _socket;
        private readonly int _port;
        private int _serialId;


        public AServer(int port)
        {
            _port = port;
        }


        public int GetSessionCount()
        {
            return _clients.Count;
        }


        public void CloseServer()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }

            foreach (var ac in _clients)
            {
                ac.CloseThis();
            }

            _clients.Clear();
            _actions.Clear();
        }


        internal void Push(IoAction action)
        {
            _actions.Enqueue(action);
        }

        public void CloseSession(AServerSession ac)
        {
            ac.CloseThis();
            _clients.Remove(ac);
            if (OnClientAddRemove != null)
            {
                OnClientAddRemove(ac, false);
            }
        }

        public void StartServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, _port));
            _socket.Listen(10);
            _socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket client = _socket.EndAccept(ar);
            _actions.Enqueue(() => { AcceptedInMainThread(client); });
            _socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptedInMainThread(Socket client)
        {
            _serialId++;
            AServerSession ac = new AServerSession(this, client, _serialId);
            _clients.Add(ac);
            ac.Start();
            if (OnClientAddRemove != null)
            {
                OnClientAddRemove(ac, true);
            }
        }

        public void Broadcast(string info)
        {
            foreach (var ac in _clients)
            {
                ac.Send(info);
            }
        }

        internal void ClientRecvLine(AServerSession ac, string line)
        {
            if (OnClientRecvLine != null)
            {
                OnClientRecvLine(ac, line);
            }
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