using System;
using Unisdk;

namespace NeteaseLogin
{
    public class MPaySdkManager
    {
        public MPay mpay = null;
        public string sauthJson = "";

        public void Init()
        {
            if (this.mpay != null)
                return;
            this.mpay = new MPay();
            this.mpay.Init("我的世界启动器", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Netease\\MCLauncher\\config\\mpay\\");
        }

        public string GetLog()
        {
            string cc = Call.Log;
            Call.Log = string.Empty;
            return cc;
        }
        public void Login()
        {
            if (this.mpay == null)
                return;
            //Singleton<AppManager>.Instance.Status = AStatus.LOGINING; //登陆中
            this.mpay.Login();
        }

        public void Logout()
        {
            if (this.mpay == null)
                return;
            this.mpay.Logout();
        }

        public void RunLoop(float deltaTime)
        {
            if (this.mpay == null)
                return;
            this.mpay.RunLoop(deltaTime);
        }

        //public void CreateOrder(string productId, int number)
        //{
        //  if (this.mpay == null)
        //    return;
        //  string uid = Singleton<ModelManager>.Instance.User.UserID.ToString();
        //  string aid = Singleton<ModelManager>.Instance.User.UnisdkAid.ToString();
        //  string hostId = "100";
        //  if ("RELEASE" == Singleton<ModelManager>.Instance.ServerName)
        //    hostId = "0";
        //  Logger.Default.Info("create order parameters, uid:" + uid + ", aid:" + aid + ", hostId:" + hostId + ", productId:" + productId + ", number:" + (object) number);
        //  this.mpay.CreateOrder(uid, aid, hostId, productId, number);
        //}

        public void clean()
        {
            if (this.mpay == null)
                return;
            this.mpay.Clean();
            this.mpay = (MPay)null;
        }
    }
}