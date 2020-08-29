using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NeteaseLogin
{
    [Serializable]
    public class GameDescription
    {
        public string NickName { get; set; }

        public uint Version { get; set; }

        public string Guid { get; set; }

        public List<ulong> ComponetList { get; set; }

        public ulong StartServerTime { get; set; }

        public GameDescription(uint version, string guid, string nickName, ulong startTime, List<ulong> componentList)
        {
            this.Version = version;
            this.Guid = guid;
            this.NickName = nickName;
            this.StartServerTime = startTime;
            this.ComponetList = componentList;
        }
    }

    public class SimplePack
    {
        public static byte[] Pack(params object[] value)
        {
            if (value == null)
                return (byte[])null;
            byte[] numArray1 = new byte[0];
            foreach (object obj in value)
            {
                byte[] numArray2 = new byte[0];
                Type type = obj.GetType();
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Object:
                        if (type == typeof(byte[]))
                        {
                            numArray2 = (byte[])obj;
                            break;
                        }
                        if (type == typeof(List<uint>))
                        {
                            List<byte> byteList = new List<byte>();
                            byte[] bytes = BitConverter.GetBytes((ushort)(((List<uint>)obj).Count * 4));
                            byteList.AddRange((IEnumerable<byte>)numArray2);
                            byteList.AddRange((IEnumerable<byte>)bytes);
                            foreach (uint num in obj as List<uint>)
                                byteList.AddRange((IEnumerable<byte>)BitConverter.GetBytes(num));
                            numArray2 = byteList.ToArray();
                            break;
                        }
                        if (type == typeof(List<ulong>))
                        {
                            List<byte> byteList = new List<byte>();
                            byte[] bytes = BitConverter.GetBytes((ushort)(((List<ulong>)obj).Count * 8));
                            byteList.AddRange((IEnumerable<byte>)numArray2);
                            byteList.AddRange((IEnumerable<byte>)bytes);
                            foreach (ulong num in obj as List<ulong>)
                                byteList.AddRange((IEnumerable<byte>)BitConverter.GetBytes(num));
                            numArray2 = byteList.ToArray();
                            break;
                        }
                        if (type == typeof(GameDescription))
                        {
                            numArray2 = SimplePack.Pack((object)JsonConvert.SerializeObject(obj));
                            break;
                        }
                        break;
                    case TypeCode.Boolean:
                        numArray2 = BitConverter.GetBytes((bool)obj);
                        break;
                    case TypeCode.Byte:
                        numArray2 = new byte[1] { (byte)obj };
                        break;
                    case TypeCode.Int16:
                        numArray2 = BitConverter.GetBytes((short)obj);
                        break;
                    case TypeCode.UInt16:
                        numArray2 = BitConverter.GetBytes((ushort)obj);
                        break;
                    case TypeCode.Int32:
                        numArray2 = BitConverter.GetBytes((int)obj);
                        break;
                    case TypeCode.UInt32:
                        numArray2 = BitConverter.GetBytes((uint)obj);
                        break;
                    case TypeCode.Double:
                        numArray2 = BitConverter.GetBytes((double)obj);
                        break;
                    case TypeCode.String:
                        byte[] bytes1 = Encoding.UTF8.GetBytes((string)obj);
                        numArray2 = SimplePack.Pack((object)(ushort)bytes1.Length, (object)bytes1);
                        break;
                    default:
                        //Logger.Default.Debug("type:" + (object)type, new object[0]);
                        //Logger.Default.Debug("input type error", new object[0]);
                        break;
                }
                numArray1 = ((IEnumerable<byte>)numArray1).Concat<byte>((IEnumerable<byte>)numArray2).ToArray<byte>();
            }
            return numArray1;
        }
    }

    public class ThreadManager
    {
        public Task Create(Action action)
        {
            Task task = Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            //int num = 327680;
            return task;
        }


        public void BeginInvoke(Action action, AsyncCallback callback)
        {
            action.BeginInvoke(callback, (object)null);
        }

        public void Invoke(Action action)
        {
            action();
        }
    }
    public class TimeHelper
    {
        public static long GetUNIXTimeStamp()
        {
            return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long GetUNIXMilliseconds()
        {
            return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static long GetUNIXTimeStamp(DateTime dateTime)
        {
            DateTime localTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(dateTime - localTime).TotalMilliseconds;
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)unixTimeStamp).ToLocalTime();
        }

        public static string FormatTime(long second, int level = 0)
        {
            string[] strArray = TimeSpan.FromSeconds((double)second).ToString("d\\:h\\:m\\:s").Split(':');
            return string.Empty + strArray[0] + "天" + strArray[1] + "小时" + strArray[2] + "分钟";
        }
    }
    public class AESHelper
    {
        public static byte[] AESEncrypt128(byte[] data, byte[] keyBytes, byte[] ivBytes)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            int num1 = 1;
            rijndaelManaged.Mode = (CipherMode)num1;
            int num2 = 2;
            rijndaelManaged.Padding = (PaddingMode)num2;
            int num3 = 128;
            rijndaelManaged.KeySize = num3;
            int num4 = 128;
            rijndaelManaged.BlockSize = num4;
            byte[] numArray1 = keyBytes;
            rijndaelManaged.Key = numArray1;
            byte[] numArray2 = ivBytes;
            rijndaelManaged.IV = numArray2;
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] GetIv(int n)
        {
            char[] chArray = new char[60]
            {
        'a',
        'b',
        'd',
        'c',
        'e',
        'f',
        'g',
        'h',
        'i',
        'j',
        'k',
        'l',
        'm',
        'n',
        'p',
        'r',
        'q',
        's',
        't',
        'u',
        'v',
        'w',
        'z',
        'y',
        'x',
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'Q',
        'P',
        'R',
        'T',
        'S',
        'V',
        'U',
        'W',
        'X',
        'Y',
        'Z'
            };
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random(DateTime.Now.Millisecond);
            for (int index = 0; index < n; ++index)
                stringBuilder.Append(chArray[random.Next(0, chArray.Length)].ToString());
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        public static byte[] AESDecrypt128(byte[] data, byte[] keyBytes, byte[] ivBytes)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            int num1 = 1;
            rijndaelManaged.Mode = (CipherMode)num1;
            int num2 = 2;
            rijndaelManaged.Padding = (PaddingMode)num2;
            int num3 = 128;
            rijndaelManaged.KeySize = num3;
            int num4 = 128;
            rijndaelManaged.BlockSize = num4;
            byte[] numArray1 = keyBytes;
            rijndaelManaged.Key = numArray1;
            byte[] numArray2 = ivBytes;
            rijndaelManaged.IV = numArray2;
            return rijndaelManaged.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] AESEncrypt128Ex(byte[] data, byte[] keyBytes, byte[] ivBytes)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            int num1 = 1;
            rijndaelManaged.Mode = (CipherMode)num1;
            int num2 = 3;
            rijndaelManaged.Padding = (PaddingMode)num2;
            int num3 = 128;
            rijndaelManaged.KeySize = num3;
            int num4 = 128;
            rijndaelManaged.BlockSize = num4;
            byte[] numArray1 = keyBytes;
            rijndaelManaged.Key = numArray1;
            byte[] numArray2 = ivBytes;
            rijndaelManaged.IV = numArray2;
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }
    }

    [Serializable]
    public class ComponentEntity
    {
        public ulong iid;
        public uint version;
        public string key;
        public string name;
        public string md5;

        public ComponentEntity()
        {
            this.md5 = string.Empty;
            this.key = string.Empty;
            this.name = string.Empty;
        }

        public string toString()
        {
            return "iid: '" + (object)this.iid + "', version: '" + (object)this.version + "', md5: '" + this.md5 + "', key: '" + this.key + "'', name: '" + this.name + "'";
        }
    }
    public enum ConnectState
    {
        INIT,
        CONNECT_NO_LOGIN,
        LOGIN,
        LOSTCONECTION,
        EXIT,
    }
    public class SocketCallback
    {
        private Dictionary<ushort, Action<byte[]>> m_receiveCallBacks;
        private Dictionary<string, ushort> m_cmdToSid;
        public Dictionary<ConnectState, Action<string>> StateCallbacks;

        public Func<byte[], bool> SendGameFunc { get; set; }

        public Action<string> ConnectCompleteSocketCallback { get; set; }

        public Action<byte, uint> LoginCompleteSocketCallback { get; set; }

        public Action<string> LostConnectSocketCallback { get; set; }

        public SocketCallback()
        {
            this.m_receiveCallBacks = new Dictionary<ushort, Action<byte[]>>();
            this.m_cmdToSid = new Dictionary<string, ushort>();
        }

        public void RegisterCmdSid(string cmd, ushort sid)
        {
            this.m_cmdToSid[cmd] = sid;
        }

        public void RegisterReceiveCallBack(ushort sid, Action<byte[]> callback)
        {
            this.m_receiveCallBacks[sid] = callback;
        }

        public bool HasCommand(string cmd)
        {
            return this.m_cmdToSid.ContainsKey(cmd);
        }

        public bool HasRecvSid(ushort sid)
        {
            return this.m_receiveCallBacks.ContainsKey(sid);
        }

        public ushort GetSid(string cmd)
        {
            if (this.m_cmdToSid.ContainsKey(cmd))
                return this.m_cmdToSid[cmd];
            //Logger.Default.Debug(" The {0:x4} do not register!", new object[1]
            //    {
            //(object) cmd
            //    });
            return 0;
        }

        public void CallBack(ushort sid, byte[] paramlist)
        {
            if (!this.m_receiveCallBacks.ContainsKey(sid)) { }
            //Logger.Default.Debug(" The {0:x4} do not register!", new object[1]
            //      {
            //(object) sid
            //      });
            else
                this.m_receiveCallBacks[sid](paramlist);
        }
    }

    public class Tool
    {
        public static byte[] a(params object[] ixu)
        {
            byte[] buffer2;
            if (ixu == null)
            {
                buffer2 = null;
            }
            else
            {
                byte[] first = new byte[0];
                object[] objArray = ixu;
                int index = 0;
                while (true)
                {
                    if (index >= objArray.Length)
                    {
                        buffer2 = first;
                        break;
                    }
                    object obj2 = objArray[index];
                    byte[] collection = new byte[0];
                    Type type = obj2.GetType();
                    TypeCode typeCode = Type.GetTypeCode(type);
                    switch (typeCode)
                    {
                        case TypeCode.Object:
                            if (type == Type.GetTypeFromHandle(typeof(byte[]).TypeHandle))
                            {
                                collection = (byte[])obj2;
                            }
                            else if (type == Type.GetTypeFromHandle(typeof(List<uint>).TypeHandle))
                            {
                                List<byte> list = new List<byte>();
                                list.AddRange(collection);
                                list.AddRange(BitConverter.GetBytes((ushort)(((List<uint>)obj2).Count * 4)));
                                foreach (uint num2 in obj2 as List<uint>)
                                {
                                    list.AddRange(BitConverter.GetBytes(num2));
                                }
                                collection = list.ToArray();
                            }
                            else if (!(type == Type.GetTypeFromHandle(typeof(List<ulong>).TypeHandle)))
                            {
                                if (type == Type.GetTypeFromHandle(typeof(GameDescription).TypeHandle))
                                {
                                    string str = JsonConvert.SerializeObject(obj2);
                                    object[] objArray2 = new object[] { str };
                                    collection = a(objArray2);
                                }
                            }
                            else
                            {
                                List<byte> list2 = new List<byte>();
                                list2.AddRange(collection);
                                list2.AddRange(BitConverter.GetBytes((ushort)(((List<ulong>)obj2).Count * 8)));
                                foreach (ulong num3 in obj2 as List<ulong>)
                                {
                                    list2.AddRange(BitConverter.GetBytes(num3));
                                }
                                collection = list2.ToArray();
                            }
                            break;

                        case TypeCode.DBNull:
                        case TypeCode.Char:
                        case TypeCode.SByte:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                            break;

                        case TypeCode.Boolean:
                            collection = BitConverter.GetBytes((bool)obj2);
                            break;

                        case TypeCode.Byte:
                            collection = new byte[] { (byte)obj2 };
                            break;

                        case TypeCode.Int16:
                            collection = BitConverter.GetBytes((short)obj2);
                            break;

                        case TypeCode.UInt16:
                            collection = BitConverter.GetBytes((ushort)obj2);
                            break;

                        case TypeCode.Int32:
                            collection = BitConverter.GetBytes((int)obj2);
                            break;

                        case TypeCode.UInt32:
                            collection = BitConverter.GetBytes((uint)obj2);
                            break;

                        case TypeCode.Double:
                            collection = BitConverter.GetBytes((double)obj2);
                            break;

                        default:
                            if (typeCode == TypeCode.String)
                            {
                                collection = Encoding.UTF8.GetBytes((string)obj2);
                                object[] objArray1 = new object[] { (ushort)collection.Length, collection };
                                collection = a(objArray1);
                            }
                            break;
                    }
                    first = first.Concat<byte>(collection).ToArray<byte>();
                    index++;
                }
            }
            return buffer2;
        }
    }
    public class OtherEnterWorldMsg
    {
        public short Id;

        public ushort Len;

        public string Name;

        public ushort Len1;

        public string Uuid;

        public OtherEnterWorldMsg()
        {
            Id = 0;
            Len = 0;
            Len1 = 0;
            Name = null;
            Uuid = null;
        }
    }
    public class RPC
    {
        private bool NeedChaCha = false;
        private ChaChaX send_ccx;
        private ChaChaX rec_ccx;

        private int m_launchIdx = -1;
        private SocketCallback m_socketCallbackFuc = new SocketCallback();
        private readonly string m_launcherIp = "127.0.0.1";
        private bool m_isNormalExit = false;
        public Dictionary<string, Action> CloseActions = new Dictionary<string, Action>();
        public Dictionary<string, Action> ReadyActions = new Dictionary<string, Action>();
        //private int clientCount = 0;
        public int FingerPrint;
        private bool m_isLaunchIdxReady;
        private TcpListener m_mcControlListener;

        private string serverIP;
        public int serverPort;
        public string RoleName;

        ThreadManager tm = new ThreadManager();

        public TcpClient Client { get; private set; }

        public BinaryReader Reader { get; private set; }

        public BinaryWriter Writer { get; private set; }

        public int LauncherControlPort { get; protected set; }

        public SocketCallback SocketCallbackFuc
        {
            get
            {
                return this.m_socketCallbackFuc;
            }
            protected set
            {
                if (value == null)
                    PrintInfo("The Callback is null!");
                else
                    this.m_socketCallbackFuc = value;
            }
        }

        public RPC(TcpClient tcpClient)
        {
            this.Client = tcpClient;
            NetworkStream stream = this.Client.GetStream();
            this.Reader = new BinaryReader((Stream)stream);
            this.Writer = new BinaryWriter((Stream)stream);
        }

        public RPC(int port, string serverIP, int serverPort, string RoleName, int fingerprint = 0)
        {
            //Messenger.Default.Register<GameMessage>((object)this, (object)"AUTH_COPONENT_LIST", new Action<GameMessage>(this.OnAuthComponents));
            PrintInfo(string.Format("监听端口[{0}] 等待JavaMod连入", (object)port));
            this.LauncherControlPort = port;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.RoleName = RoleName;
            //if ((uint)Singleton<McLanManager>.Instance.LauncherControlPort > 0U)
            //{
            //    this.LauncherControlPort = Singleton<McLanManager>.Instance.LauncherControlPort;
            //    Singleton<LanGameManager>.Instance.PrintInfo(string.Format("LauncherControlPort 覆盖为 {0}", (object)Singleton<McLanManager>.Instance.LauncherControlPort));
            //}
            this.FingerPrint = fingerprint;
            this.StartControlConnection();
        }

        private void StartControlConnection()
        {
            try
            {
                this.m_mcControlListener = new TcpListener(IPAddress.Parse(this.m_launcherIp), this.LauncherControlPort);
                this.m_mcControlListener.Start();
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
                this.CloseControlConnection();
                return;
            }
            tm.Create(new Action(this.ListenControlConnect));
            PrintInfo(string.Format("开始监听游戏控制连接[{0}]...", (object)this.LauncherControlPort));


            this.SocketCallbackFuc.RegisterReceiveCallBack(512, HandShake);//0x20
            this.SocketCallbackFuc.RegisterReceiveCallBack(0, PrepareCha);//0x0 初始化，准备加密密钥

            this.SocketCallbackFuc.RegisterReceiveCallBack(0x205, AuthServer);//AuthServer


        }
        private void PrepareCha(byte[] iyb)
        {
            CC a = new CC();
            new SimpleUnpack(iyb).Unpack(ref a);
            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(a.c);
            byte[] array2 = new byte[32];
            Array.Clear(array2, 0, array2.Length);
            Array.Copy(bytes, 16, array2, 0, 16);
            Array.Copy(bytes, 0, array2, 16, 16);
            this.rec_ccx = new ChaCha(array2);
            this.send_ccx = new ChaCha(bytes);
            this.NeedChaCha = true;
        }


        private void ListenControlConnect()
        {
            try
            {

                while (true)
                {
                    TcpClient tcpClient = this.m_mcControlListener.AcceptTcpClient();
                    PrintInfo(string.Format("[{0}]进入", (object)tcpClient.Client.RemoteEndPoint));
                    if (tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0] != "127.0.0.1")
                    {
                        tcpClient.Close();
                    }
                    else
                    {
                        //this.clientCount = this.clientCount + 1;
                        NetworkStream stream = tcpClient.GetStream();
                        //if (3 != this.clientCount)
                        //{
                        this.Client = tcpClient;
                        this.Reader = new BinaryReader((Stream)stream);
                        this.Writer = new BinaryWriter((Stream)stream);
                        tm.Create(delegate
                        {
                            OnRecvControlData();
                        });
                        //    if (this.clientCount == 1)
                        //        this.SendModInfos();
                        //}
                        //else
                        //    break;
                    }
                }
                //BinaryReader reader = new BinaryReader((Stream)stream);
                //BinaryWriter writer = new BinaryWriter((Stream)stream);
                //tm.Create((Action)(() => this.OnAuthenticationRequest(reader, writer)));
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
                this.CloseControlConnection();
            }
        }

        private void OnRecvControlData()
        {
            //bool flag = (bool)o;
            PrintInfo("开始接收游戏控制信息...");
            while (!this.m_isNormalExit)
            {
                int num;
                byte[] numArray;
                try
                {
                    num = (int)this.Reader.ReadUInt16();
                    numArray = this.Reader.ReadBytes(num);
                }
                catch (Exception ex)
                {
                    PrintInfo(ex.Message);
                    if (!this.m_isNormalExit)
                        this.CloseGameCleaning();
                    return;
                }
                PrintInfo(string.Format("[control] From MC：{0}", (object)Others.ByteToString(numArray, 0, num)));
                this.HandleMcControlMessage(numArray);
            }
        }

        private void HandleMcControlMessage(byte[] message) //ok
        {
            if (this.NeedChaCha)
            {
                this.rec_ccx.Process(message);
            }

            ushort uint16 = BitConverter.ToUInt16(message, 0);
            byte[] array = ((IEnumerable<byte>)message).Skip<byte>(2).Take<byte>(message.Length - 2).ToArray<byte>();
            PrintInfo(string.Format("cmd命令：{0:x4}", (object)uint16));
            if (!this.m_isLaunchIdxReady && (int)uint16 == 261)
            {
                this.m_launchIdx = (int)BitConverter.ToInt16(message, 2);
                PrintInfo(string.Format("LaunchIdx = {0}", (object)this.m_launchIdx));
                this.m_isLaunchIdxReady = true;
                this.ExecuteReadyActions();
            }
            try
            {
                this.SocketCallbackFuc.CallBack(uint16, array);
            }
            catch (Exception arg)
            {
                PrintInfo("游戏Launcher报文通信出错" + arg);
            }
        }
        public void AuthServer(byte[] re)
        {
            SendControlData(SimplePack.Pack((object)(ushort)1031, (object)this.serverIP, (object)(int)this.serverPort, (object)this.RoleName, (object)false));
        }
        //public byte[] HandleSkinMessage(byte[] content)//GameState.gameid, username, profile.getId().toString() Skin
        //{
        //    OtherEnterWorldMsg otherEnterWorldMsg = new OtherEnterWorldMsg();
        //    new SimpleUnpack(content).Unpack(ref otherEnterWorldMsg);
        //    //skinMode  DEFAULT=0 SLIM=1
            
            
        //    return SimplePack.Pack((ushort)520, otherEnterWorldMsg.Name, System.Windows.Forms.Application.ExecutablePath + "\\cache\\skin\\", System.Windows.Forms.Application.ExecutablePath + "\\cache\\skin\\", 0);
        //}
        public void SendControlData(byte[] message)
        {
            if (this.NeedChaCha)
            {
                this.send_ccx.Process(message);
            }

            byte[] array = ((IEnumerable<byte>)BitConverter.GetBytes((ushort)message.Length)).Concat<byte>((IEnumerable<byte>)message).ToArray<byte>();
            PrintInfo(string.Format("[control] To MC：{0}", (object)Others.ByteToString(array, 0, Math.Min(64, array.Length))));
            try
            {
                this.Writer.Write(array);
                this.Writer.Flush();
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
                PrintInfo("向MC发送控制信息失败");
            }
        }

        public void CloseControlConnection()
        {
            try
            {
                TcpListener mcControlListener = this.m_mcControlListener;
                if (mcControlListener != null)
                    mcControlListener.Stop();
                this.m_mcControlListener = (TcpListener)null;
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
        }

        public void RegisterReadyAction(string name, Action action)
        {
            this.ReadyActions.Add(name, action);
            PrintInfo(string.Format("就绪处理[{0}]注册成功:{1}", (object)name, (object)action.Method));
        }

        public void RegisterCloseAction(string name, Action action)
        {
            this.CloseActions.Add(name, action);
            PrintInfo(string.Format("关闭处理[{0}]注册成功:{1}", (object)name, (object)action.Method));
        }

        private void ClearActions()
        {
            this.ReadyActions.Clear();
            this.CloseActions.Clear();
        }

        public void CloseGameCleaning()
        {
            PrintInfo("RPC CloseGameCleaning");
            try
            {
                this.ExecuteCloseActions();
                this.ClearActions();
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
            this.m_isLaunchIdxReady = false;
            this.m_launchIdx = -1;
        }

        private void ExecuteCloseActions()
        {
            foreach (KeyValuePair<string, Action> closeAction in this.CloseActions)
            {
                PrintInfo(string.Format("执行关闭处理函数: {0}", (object)closeAction.Key));
                closeAction.Value();
            }
        }

        private void ExecuteReadyActions()
        {
            foreach (KeyValuePair<string, Action> readyAction in this.ReadyActions)
            {
                PrintInfo(string.Format("执行就绪处理函数: {0}", (object)readyAction.Key));
                readyAction.Value();
            }
        }

        private void PrintInfo(string v)
        {
            Call.Log += "[RPC]" + v + "\r\n";
            Console.WriteLine(v);
        }

        private void Close()
        {
            BinaryReader reader = this.Reader;
            if (reader != null)
                reader.Close();
            BinaryWriter writer = this.Writer;
            if (writer != null)
                writer.Close();
            TcpClient client = this.Client;
            if (client != null)
                client.Close();
            this.CloseControlConnection();
        }

        public void NormalExit()
        {
            this.m_isNormalExit = true;
            this.Close();
        }

        public string RpcToString()
        {
            string str1 = "=============================\n";
            TcpClient client = this.Client;
            string str2 = client != null ? client.ToString() : (string)null;
            string str3 = str1 + str2;
            BinaryReader reader = this.Reader;
            string str4 = reader != null ? reader.ToString() : (string)null;
            string str5 = str3 + str4;
            BinaryWriter writer = this.Writer;
            string str6 = writer != null ? writer.ToString() : (string)null;
            return str5 + str6 + (object)this.m_launchIdx + "=============================\n";
        }
        private void HandShake(byte[] iyl)
        {
            byte[] iyg = Tool.a((ushort)512, "i'am wpflauncher");
            SendControlData(iyg);
        }

        //private void OnAuthComponents(GameMessage msg)
        //{
        //    List<ComponentEntity> data = msg.Data as List<ComponentEntity>;
        //    if (data == null)
        //        return;
        //    this.m_comps = data;
        //}
    }
}
