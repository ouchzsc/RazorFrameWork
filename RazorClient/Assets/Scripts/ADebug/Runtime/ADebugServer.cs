using System;
using System.Text;
using UnityEngine;

namespace ADebugger
{
    public class ADebugServer : MonoBehaviour
    {
        private AServer _server;

        private string Prefix = "";

        public int ClientCount;
        public int Port = 10080;

        void OnEnable()
        {
            _server = new AServer(Port);
            _server.StartServer();

            _server.OnClientAddRemove = OnClientAddRemove;

            _server.OnClientRecvLine = OnClientRecvLine;

            Application.logMessageReceived += LogCallback;
        }

        void OnDisable()
        {
            if (_server == null)
            {
                return;
            }

            _server.CloseServer();
            _server = null;
            Application.logMessageReceived -= LogCallback;
        }

        void Update()
        {
            if (_server == null)
            {
                ClientCount = 0;
                return;
            }

            _server.Process();
            ClientCount = _server.GetSessionCount();
        }


        private void OnClientAddRemove(AServerSession ac, bool isAdd)
        {
            _server.Broadcast(ac.Id() + (isAdd ? " connected, client count=" : " disconnected, client count=") +
                              _server.GetSessionCount());
        }

        private void OnClientRecvLine(AServerSession ac, string line)
        {
            int cmdendi = line.IndexOf(" ", StringComparison.Ordinal);
            string cmd;
            string param = "";
            if (cmdendi == -1)
            {
                cmd = line.Trim();
            }
            else
            {
                cmd = line.Substring(0, cmdendi);
                param = line.Substring(cmdendi + 1).Trim();
            }

            switch (cmd.ToLower())
            {
                case "quit":
                    ac.Send("bye");
                    _server.CloseSession(ac);
                    break;
                case "luaprefix":
                    ac.Send(Prefix);
                    break;
                case "dolua":
                    DoLua(ac, param);
                    break;

                case "dobase64lua":
                    DoBase64Lua(ac, param);
                    break;

                default:
                    ac.Send("Not implemented " + cmd);
                    break;
            }
        }


        private void DoBase64Lua(AServerSession ac, string param)
        {
            byte[] bytes = Convert.FromBase64String(param);
            string decode = Encoding.UTF8.GetString(bytes);
            ac.Send($"执行Lua文件成功,文件长度:{decode.Length}");
            var luaMgr = csModules.luaManager;
            if (luaMgr != null)
            {
                try
                {
                    luaMgr.LuaEnv.DoString(decode);
                }
                catch (Exception e)
                {
//                    ac.Send(e.ToString());
                    Debug.LogException(e);
                }
            }
        }

        private void DoLua(AServerSession ac, string param)
        {
            ac.Send("DoLua " + param);
            var luaMgr = csModules.luaManager;

            if (luaMgr != null)
            {
                try
                {
                    luaMgr.LuaEnv.DoString(Prefix + "\n" + param);
                }
                catch (Exception e)
                {
                    ac.Send(e.ToString());
                }
            }
        }

        private void LogCallback(string condition, string stackTrace, LogType type)
        {
            _server.Broadcast($"[{type}]{condition}\n{stackTrace}");
        }
    }
}