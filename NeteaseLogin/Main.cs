using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseLogin
{
    public interface IClass
    {
        int init();
        int StartRPC(int port, string serverip, int serverport, string rolename);
        void login();
        void auth_heartbeat();
        void auth_login(string userid);
        void auth_start(string ServerID, int GameProcessID, int port);
        void auth_logout();
        void auth_stop();
        string GetSkinUIDFromUUID(string uuid);
        string GetVerificationAccount(string entity_id, string token, string rolename);
        void loop(int delay);
        string gettoken(string url, string data);
        string getlog();
        byte[] HttpEncrypt(string url, string data);
        string Test(string aa);
        string ParseLoginResponse(string data);//hex文本
        string HttpDecrypt(string data);//hex文本
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Class : IClass
    {
        MPaySdkManager p_ms = new MPaySdkManager();
        RPC p_rpc = null;
        AuthHelper p_auth = new AuthHelper();

        public void login()
        {
            p_ms.Init();
            p_ms.Login();
        }
        public void auth_heartbeat()//30s
        {
            p_auth.SendHeartBeatPacket();
        }
        public void auth_login(string userid)
        {
            p_auth.Login(uint.Parse(userid));
        }

        public void auth_start(string ServerID, int GameProcessID,int port)
        {
            p_auth.Auth(Convert.ToUInt64(ServerID), GameProcessID, port);
        }
        public void auth_logout()
        {
            p_auth.Logout();
        }
        public void auth_stop()
        {
            p_auth.Stop();
        }
        public void loop(int delay)//500ms一次
        {
            p_ms.RunLoop(delay/1000);
        }
        public string gettoken(string url, string data)
        {
            return Call.GetToken(url, data);
        }
        public string getlog()
        {
            return p_ms.GetLog();
        }

        public int init()
        {
            return Call.init();
        }

        public int StartRPC(int port, string serverip, int serverport, string rolename)
        {
            p_rpc = new RPC(port, serverip, serverport, rolename);
            if(p_rpc!=null)
            {
                return 1;
            }
            return 0;
        }
        public string GetSkinUIDFromUUID(string uuid)//皮肤UID
        {
            return Others.GetUserIdFromUUID(uuid).ToString();
        }
        public string GetVerificationAccount(string entity_id, string token, string rolename)//333316 mvMwFpkOaKn186KA Hyun
        {
            VerificationAccount v = Others.GetVerificationAccountEx(uint.Parse(entity_id), token, rolename);
            return(v.username + "|" + v.uuid + "|" + v.accessToken);
        }
        public byte[] HttpEncrypt(string url, string data)
        {
            string empty = string.Empty;
            return Call._HttpEncrypt_(url, data, out empty);
        }
        public string HttpDecrypt(string data)//hex文本
        {
            string empty = string.Empty;
            return Call._HttpDecrypt_(Hex2bytes(data), out empty);
        }
        public string ParseLoginResponse(string data)//hex文本
        {
            string empty = string.Empty;
            return Call._ParseLoginResponse_(Hex2bytes(data), out empty);
        }

        public string Test(string aa)
        {
            return "wadwad";
        }

        public static string Byte2hex(byte[] bytes)
        {
            var hex = BitConverter.ToString(bytes, 0).Replace("-", string.Empty).ToLower();
            return hex;
        }
        public static byte[] Hex2bytes(string hex)
        {
            var inputByteArray = new byte[hex.Length / 2];
            for (var x = 0; x < inputByteArray.Length; x++)
            {
                var i = Convert.ToInt32(hex.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            return inputByteArray;
        }
    }
}
