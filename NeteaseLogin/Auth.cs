using AuthSdkWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeteaseLogin
{

    public class AuthHelper
    {
        Auth auth = new Auth();
        private uint uid = 0;

        public void SendHeartBeatPacket()//30s一次
        {
            this.auth.Send(0, "iamwpf");
        }
        public void Login(uint UserID)//https://x19.update.netease.com/authserver.list
        {

            this.uid = UserID;
            ThreadManager tm = new ThreadManager();
            tm.Create(new Action(delegate () { this.auth.Login(uid, "42.186.82.199", 11000); }));
            //userid ip port
        }
        public void Auth(ulong ServerID, int GameProcessID, int port)
        {
            ThreadManager tm = new ThreadManager();
            tm.Create(new Action(delegate () { this.authsession(ServerID, GameProcessID, port); }));

        }
        public void Stop()
        {
            this.auth.CloseAuthentication();
        }
        public void Logout()
        {
            this.auth.Logout();
        }
        void authsession(ulong ServerID, int GameProcessID, int port)
        {
            this.auth.MakeAuthentication(ServerID, GameProcessID, this.uid, port);//ServerId(Convert.ToUInt64) UserID (GameM OnLaunched gameProcess ID)
        }
    }
    public class Auth : MclAuthWrapper
    {
        private void Print(string str)
        {
            str = "[Auth]" + str;
            Call.Log += str + "\r\n";
            Console.WriteLine(str);
        }

        //public void Resettimer()
        //{
        //    if (this.Timer != null)
        //    {
        //        this.Timer.Stop();
        //        this.Timer = null;
        //    }
        //}

        [CompilerGenerated]
        private void b(object[] jnq)
        {
            base.Send(0, "iamwpf");
        }

        protected override void OnLog(string log, int level)
        {
            Print("[LOG_" + level.ToString() + "]" + log);
            //switch (level)
            //{
            //    case 1:
            //        mp.Default.i(log, new object[0]);
            //        break;

            //    case 2:
            //        mp.Default.k(log, new object[0]);
            //        break;

            //    default:
            //        break;
            //}
        }

        protected override void OnLoginFinish(int code)
        {
            Print("[OnLoginFinish]" + code.ToString());
            if (code == 0)
            {
                Print("HeartBeatTimer_Start");
            }
            //aiq<ahz>.Instance.g(code);
        }

        protected override void OnLogoutFinish(int code)
        {
            Print("[OnLogoutFinish]" + code.ToString());
            Print("HeartBeatTimer_Stop");
            //this.a();
            //aiq<ahz>.Instance.h(code);
        }

        protected override void OnLoseConnection(int info, int code)
        {
            //aib aib2 = (aib)info;
            //if ((aib2 != aib.b) && (aib2 != (aib.b | aib.a)))
            //{
            //}
            Print("HeartBeatTimer_Stop");
            //if ((info != 0) && (code != 0))
            //{
            //    aiq<ahz>.Instance.j(code);
            //}
            //aiq<ahz>.Instance.i(code);
        }

        protected override void OnMakeAuthenticationFinish(ulong gameId, int code)
        {
            Print("[OnMakeAuthenticationFinish]" + code.ToString());
            //aiq<ahz>.Instance.m(gameId, code);
        }

        protected override void OnReConnect()
        {
            //aiq<ahz>.Instance.d(null);
        }

        protected override void OnRecv(int proto, string response)
        {
        }

    }
}
