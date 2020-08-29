using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NeteaseLogin
{
    public class DllLoader
    {
        #region Win API
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);
        #endregion

        public IntPtr hLib;
        public DllLoader(int handle = 0, String DLLPath = "")
        {
            if (handle == 0)
                hLib = LoadLibrary(DLLPath);
            else
                hLib = (IntPtr)handle;
        }

        ~DllLoader()
        {
            if (hLib != (IntPtr)0)
                FreeLibrary(hLib);
        }


        //将要执行的函数转换为委托
        public Delegate Invoke(string APIName, Type t)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            if (APIName == "HttpEncrypt") Call.FindBeginAddr = (int)api;
            Console.WriteLine("[" + APIName + "]" + api.ToString());
            if (api == (IntPtr)0) { Console.WriteLine("暂不支持你的电脑!"); return null; }
            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
        }
    }
}
