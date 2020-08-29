using MPayNameSpace;
using NeteaseLogin;
using System;
using System.Windows;

namespace Unisdk
{
    public class MPay : CppCliUnisdkMPay
    {
        private const int NT_LOGIN_OK = 0;
        private const int NT_LOGIN_CANCEL = 1;
        private const int NT_LOGIN_WRONG_PASSWD = 2;
        private const int NT_LOGIN_NET_UNAVAILABLE = 3;
        private const int NT_LOGIN_SDK_SERV_ERR = 4;
        private const int NT_LOGIN_NET_TIME_OUT = 5;
        private const int NT_LOGIN_SDK_NOT_INIT = 6;
        private const int NT_LOGIN_UNKNOWN_ERR = 10;
        private const int NT_LOGIN_NEED_GS_CONFIRM = 11;
        private const int NT_LOGIN_NEED_RELOGIN = 12;
        private const int NT_LOGIN_BIND_OK = 13;
        private const int NT_CHECKORDER_PREPARING = 0;
        private const int NT_CHECKORDER_CHECKING = 1;
        private const int NT_CHECKORDER_CHECK_OK = 2;
        private const int NT_CHECKORDER_CHECK_ERR = 3;



        protected override void onCheckOrderFinish(int errorCode, int orderStatus, string productId, uint productCount, string orderId, string errReason)
        {
            throw new NotImplementedException();
        }

        protected override void onInitFinish(int code)
        {
            Call.Log += "[MPay][onInitFinish]" + (object)code + "\r\n";
            if (code == 0)
            {
                //Singleton<LogManager>.Instance.LogLoginUI();
            }
            else
            {
                Call.Log += "[MPay]mpay init error" + "\r\n";
                //Shutdown();
            }
        }

        protected override void onLog(string log)
        {
            //throw new NotImplementedException();
        }

        protected override void onLoginFinish(int code)
        {
            Call.Log += "[MPay][onLoginFinish]" + (object)code + "\r\n";

            if (code == 0)
            {
                Call.Log += "[MPay]Getting sauth..." + "\r\n";
                string ss = this.GetSAuthPropStr();
                Call.Log += "[MPay][sauthJson]" + ss + "\r\n";
                //Login();
            }
            else
            {
                if (1 != code)
                    return;
                //Shutdown();
            }
        }

        protected override void onLogoutFinish(int code)
        {
            Call.Log += "[MPay][onLogoutFinish]" + (object)code + "\r\n";
            if (code != 0)
                return;
            Login();
        }

    }
}
