using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ADebugger
{
    public class ADebugClientWindow : EditorWindow
    {
        [MenuItem("WorldGame/lua远程调试器", false, 101)]
        public static void Init()
        {
            ADebugClientWindow window = GetWindow<ADebugClientWindow>("lua远程调试器");
            window._ip = EditorPrefs.GetString("a_debuger_ip", null) ?? "127.0.0.1";
            window._toConsole = true;
        }

        private AClient _client;
        private string _ip = "127.0.0.1";
        private int _port = 10080;
        private bool _toConsole;
        private string _luacode = "";
        private string _luafile = "LuaScript/Src/editor/aDebug.lua";
        private string _recvLines = "";

        void OnGUI()
        {
            _ip = EditorGUILayout.TextField("ip", _ip);
            _port = EditorGUILayout.IntField("port", _port);

            _toConsole = EditorGUILayout.Toggle("recv to console", _toConsole);


            if (_client != null)
            {
                if (_toConsole)
                {
                    _client.OnRecv = RecvToConsole;
                }
                else
                {
                    _client.OnRecv = RecvToThisWindow;
                }

                if (GUILayout.Button("reconnect"))
                {
                    _client.Close();
                    _client = new AClient(_ip, _port);
                }


                if (_client.IsConnected())
                {
                    EditorGUILayout.LabelField("connected");

                    _luacode = EditorGUILayout.TextField("lua", _luacode);
                    if (GUILayout.Button("dolua"))
                    {
                        _recvLines = "...";
                        _client.Send("dolua " + _luacode);
                        Repaint();
                    }

                    _luafile = EditorGUILayout.TextField("luafile", _luafile);

                    if (GUILayout.Button("doluafile"))
                    {
                        _recvLines = "...";
                        string base64 = Convert.ToBase64String(File.ReadAllBytes(_luafile));
                        _client.Send("dobase64lua " + base64);
                        Repaint();
                    }

                    if (_recvLines.Length > 2000)
                    {
                        _recvLines = _recvLines.Substring(0, 2000);
                    }

                    GUILayout.TextArea(_recvLines);

                    if (GUILayout.Button("quit"))
                    {
                        _recvLines = "...";
                        _client.Send("quit");
                        _client = null;
                        Repaint();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("connecting");
                }
            }
            else
            {
                if (GUILayout.Button("connect"))
                {
                    EditorPrefs.SetString("a_debuger_ip", _ip);
                    _client = new AClient(_ip, _port);
                }
            }
        }

        private void RecvToConsole(string lines)
        {
            var len_msg = Regex.Match(lines, @"(\d)+ ([.\s\S\n]+)");
            var len = len_msg.Groups[1].Value;
            var msg = len_msg.Groups[2].Value;
            var logType_logStr = Regex.Match(msg, @"\[(\w+)\]([.\s\S\n]+)");

            if (logType_logStr.Groups.Count == 3)
            {
                string logType = logType_logStr.Groups[1].Value;
                string debugStr = $"远程：{logType_logStr.Groups[2].Value}";
                switch (logType)
                {
                    case "Log":
                        Debug.Log(debugStr);
                        break;
                    case "Warning":
                        Debug.LogWarning(debugStr);
                        break;
                    case "Error":
                        Debug.LogError(debugStr);
                        break;
                    case "Exception":
                        Debug.LogError(debugStr);
                        break;
                }
            }
            else
            {
                Debug.Log($"远程：{msg}");
            }
        }

        private void RecvToThisWindow(string lines)
        {
            _recvLines += lines;
            Repaint();
        }


        void Update()
        {
            if (_client != null)
            {
                _client.Process();
            }
        }
    }
}