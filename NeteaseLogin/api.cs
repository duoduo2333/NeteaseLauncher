using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NeteaseLogin
{
    public class Api
    {
        //搜索特征码
        [DllImport("netware.dll")]
        public static extern long ScanTZ(IntPtr hProcess, string s);

        /// <summary>
        /// 在窗口列表中寻找与指定条件相符的第一个子窗口
        /// </summary>
        /// <param name="hwndParent">在其中查找子的父窗口。如设为零，表示使用桌面窗口</param>
        /// <param name="hwndChildAfter">从这个窗口后开始查找。这样便可利用对FindWindowEx的多次调用找到符合条件的所有子窗口</param>
        /// <param name="lpszClass">欲搜索的类名</param>
        /// <param name="lpszWindow">欲搜索的类名</param>
        /// <returns>找到的窗口的句柄。如未找到相符窗口，则返回零。会设置GetLastError</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent,
    IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// FindWindow 寻找窗口列表中第一个符合指定条件的顶级窗口
        /// </summary>
        /// <param name="lpszClass">String，指向包含了窗口类名的空中止（C语言）字串的指针；或设为零，表示接收任何类</param>
        /// <param name="lpszWindow">String，指向包含了窗口文本（或标签）的空中止（C语言）字串的指针；或设为零，表示接收任何窗口标题</param>
        /// <returns>Long，找到窗口的句柄。如未找到相符窗口，则返回零。会设置GetLastError</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

        /// <summary>
        ///  GetForegroundWindow 获取活动窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();



        /// <summary>
        /// 这个 OpenProcess 函数打开一个已存在的进程对象。
        /// </summary>
        /// <param name="dwDesiredAccess">指定这个句柄要求的访问方法。指定API32.TXT文件中以PROCESS_???开头的一个或多个常数</param>
        /// <param name="bInheritHandle">如句柄能够由子进程继承，则为TRUE </param>
        /// <param name="dwProcessId">要打开那个进程的进程标识符 </param>
        /// <returns>Long，如执行成功，返回进程句柄；零表示失败。会设置GetLastError</returns>
        [DllImport("kernel32.dll")]
        public static extern
            IntPtr OpenProcess(UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwProcessId);

        /// <summary>
        ///关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等
        /// </summary>
        /// <param name="hObject">欲关闭的一个对象的句柄 </param>
        /// <returns>非零表示成功，零表示失败</returns>
        [DllImport("kernel32.dll")]
        public static extern
            Int32 CloseHandle(IntPtr hObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hProcess">进程的句柄</param>
        /// <param name="lpBaseAddress">进程地址</param>
        /// <param name="lpBuffer">数据存放地址</param>
        /// <param name="nSize">数据的长度</param>
        /// <param name="lpNumberOfBytesWritten">实际数据的长度</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern
            Boolean WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern
            Boolean WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, UInt32 lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern
            Boolean WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string[] lpBuffer, Int32 nSize, UInt32 lpNumberOfBytesWritten);
        /// <summary>
        /// 为远程进程的句柄
        /// </summary>
        /// <param name="hProcess">为远程进程的句柄</param>
        /// <param name="lpBaseAddress">用于指明远程进程中的地址</param>
        /// <param name="lpBuffer">是本地进程中的内存地址</param>
        /// <param name="nSize">是需要传送的字节数</param>
        /// <param name="lpNumberOfBytesRead">实际读取的字节数</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern
            Boolean ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern
            Boolean ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, uint[] lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern
            Boolean ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// 获取当前进程的一个伪句柄
        /// </summary>
        /// <returns>当前进程的伪句柄</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flg"></param>
        /// <param name="rea"></param>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Boolean ExitWindowsEx(int flg, int rea);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

    }
}