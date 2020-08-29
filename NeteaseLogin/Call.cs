using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NeteaseLogin
{
    public class Call
    {
        static private int m_hDLL = 0; //DLL句柄=基址
        private const string DLLNAEM = "api-ms-win-crt-utility-l1-1-1.dll";

        public static string Log = "";

        public static int FindBeginAddr = 0;

        public delegate int _HttpDecrypt(IntPtr coi, int coj, out IntPtr cok, out IntPtr col);
        public delegate int _ParseLoginResponse(IntPtr cnw, int cnx, out IntPtr cny, out IntPtr cnz);
        public unsafe delegate int _HttpEncrypt(byte* cso, byte* csp, out IntPtr csq, out IntPtr csr);
        public delegate void _FreeMemory(IntPtr ctz);
        public unsafe delegate uint _ff487957b05f3b54712db300a8687189(bool cnd, byte* cne, uint cnf);
        public unsafe delegate bool _1b559cbb2d1a2c82336de16a49adc867(byte* com, uint con, byte* coo, uint cop);
        public unsafe delegate void _b79c5024733866f2a0d68ae29f94b595(IntPtr cna, byte* cnb, uint cnc);//ChaChaProcess
        public delegate void _b71fa6b924744e4fdf5091006d3ac0c8(IntPtr cmw);//DeleteChaCha
        public unsafe delegate IntPtr _51258412ae7f26a1cbfcfc4c52b215cb(uint cmr, byte* cms);//NewChaCha
        public delegate int _ComputeDynamicToken(IntPtr coa, int cob, IntPtr coc, int cod, out IntPtr coe);

        public static _ParseLoginResponse ParseLoginResponse;
        public static _HttpEncrypt HttpEncrypt;
        public static _HttpDecrypt HttpDecrypt;
        public static _FreeMemory FreeMemorys;
        public static _ff487957b05f3b54712db300a8687189 ParseUUID_;
        public static _1b559cbb2d1a2c82336de16a49adc867 Parse2_;
        public static _b79c5024733866f2a0d68ae29f94b595 ChaChaProcess;
        public static _51258412ae7f26a1cbfcfc4c52b215cb NewChaCha_;
        public static _b71fa6b924744e4fdf5091006d3ac0c8 DeleteChaCha;
        public static _ComputeDynamicToken ComputeDynamicToken;

        public static unsafe uint ParseUUID(bool cng, byte* cnh, uint cni) => ParseUUID_(cng, cnh, cni);
        public static unsafe bool Parse2(byte* coq, uint cor, byte* cos, uint cot) => Parse2_(coq, cor, cos, cot);
        public static unsafe IntPtr NewChaCha(uint cmt, byte* cmu) => NewChaCha_(cmt, cmu);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeMemory(IntPtr coy);
        public static int init()
        {
            FreeMemory((IntPtr)0);//加载Dll

            m_hDLL = GetModuleHandle(DLLNAEM);

            DllLoader DL = new DllLoader(m_hDLL);//handle Dllname
            //m_hDLL = (int)DL.hLib;

            Console.WriteLine("hDll:" + m_hDLL.ToString());

            if (m_hDLL == 0)
                return 0;


            HttpEncrypt = (_HttpEncrypt)DL.Invoke("HttpEncrypt", typeof(_HttpEncrypt));
            ParseLoginResponse = (_ParseLoginResponse)DL.Invoke("ParseLoginResponse", typeof(_ParseLoginResponse));
            HttpDecrypt = (_HttpDecrypt)DL.Invoke("HttpDecrypt", typeof(_HttpDecrypt));
            FreeMemorys = (_FreeMemory)DL.Invoke("FreeMemory", typeof(_FreeMemory));
            ParseUUID_ = (_ff487957b05f3b54712db300a8687189)DL.Invoke("_ff487957b05f3b54712db300a8687189", typeof(_ff487957b05f3b54712db300a8687189));
            Parse2_ = (_1b559cbb2d1a2c82336de16a49adc867)DL.Invoke("_1b559cbb2d1a2c82336de16a49adc867", typeof(_1b559cbb2d1a2c82336de16a49adc867));
            ChaChaProcess = (_b79c5024733866f2a0d68ae29f94b595)DL.Invoke("_b79c5024733866f2a0d68ae29f94b595", typeof(_b79c5024733866f2a0d68ae29f94b595));
            NewChaCha_ = (_51258412ae7f26a1cbfcfc4c52b215cb)DL.Invoke("_51258412ae7f26a1cbfcfc4c52b215cb", typeof(_51258412ae7f26a1cbfcfc4c52b215cb));
            DeleteChaCha = (_b71fa6b924744e4fdf5091006d3ac0c8)DL.Invoke("_b71fa6b924744e4fdf5091006d3ac0c8", typeof(_b71fa6b924744e4fdf5091006d3ac0c8));
            ComputeDynamicToken = (_ComputeDynamicToken)DL.Invoke("ComputeDynamicToken", typeof(_ComputeDynamicToken));

            //HttpEncrypt = (_HttpEncrypt)DllLoader.GetAddress(m_hDLL, "HttpEncrypt", typeof(_HttpEncrypt));
            //ParseLoginResponse = (_ParseLoginResponse)DllLoader.GetAddress(m_hDLL, "ParseLoginResponse", typeof(_ParseLoginResponse));
            //HttpDecrypt = (_HttpDecrypt)DllLoader.GetAddress(m_hDLL, "HttpDecrypt", typeof(_HttpDecrypt));
            //FreeMemory = (_FreeMemory)DllLoader.GetAddress(m_hDLL, "FreeMemory", typeof(_FreeMemory));
            //ParseUUID_ = (_ff487957b05f3b54712db300a8687189)DllLoader.GetAddress(m_hDLL, "_ff487957b05f3b54712db300a8687189", typeof(_ff487957b05f3b54712db300a8687189));
            //Parse2_ = (_1b559cbb2d1a2c82336de16a49adc867)DllLoader.GetAddress(m_hDLL, "_1b559cbb2d1a2c82336de16a49adc867", typeof(_1b559cbb2d1a2c82336de16a49adc867));

            crack();

            return m_hDLL;
        }

        public static int GetModuleHandle(string Modulename)
        {
            foreach (ProcessModule pm in Process.GetCurrentProcess().Modules)
            {
                if (pm.ModuleName == Modulename)
                    return (int)pm.BaseAddress;
            }
            return 0;
        }

        //[DllImport("NeteaseHelper.dll")]
        //public static extern int GetProcRVA(string DllPath, string procname);
        //public static int GetProcAddress(string procname)
        //{
        //    int addr = GetProcRVA(Environment.CurrentDirectory + "\\" + DLLNAEM, procname);
        //    if (addr == 0)
        //    {
        //        Console.WriteLine("获取函数地址失败！！！");
        //    }
        //    addr += m_hDLL;
        //    return addr;
        //}
        public static int FindAddr(IntPtr PHandle, int ProcAddr, uint findSize)
        {
            //开始寻找的函数地址 寻找范围  注意：未关闭句柄！！！！ By Hyun
            byte[] mem = new byte[findSize + 1];
            int readBytes = 0;

            bool result = Api.ReadProcessMemory(PHandle, (IntPtr)ProcAddr, mem, findSize, out readBytes);

            Console.WriteLine("ReadResult:" + result.ToString());
            Console.WriteLine("Mem:" + Others.byteToHexStr(mem));
            Console.WriteLine("Lenth:" + readBytes.ToString());

            int time = 0, Offset = 0, Addr = ProcAddr;
            for (int i = 0; i < mem.Length; i++)
            {
                if (mem[i] == 232)//E8 内部Call
                    time++;
                if (time == 2)
                {
                    Addr = Addr + (i + 5);
                    Offset = (mem[i + 4] << 24) + (mem[i + 3] << 16) + (mem[i + 2] << 8) + mem[i + 1];
                    break;
                }
            }

            Addr += Offset;
            Console.WriteLine("Addr:" + Addr.ToString());
            Console.WriteLine("Offset:" + (Addr - m_hDLL).ToString());

            return Addr;
        }

        public static void crack()
        {

            IntPtr PinballHandle = Api.GetCurrentProcess();

            bool result = Api.WriteProcessMemory(PinballHandle, (IntPtr)FindAddr(PinballHandle, FindBeginAddr, 150), new uint[] { Convert.ToUInt32("B001C3", 16) }, 3, (IntPtr)0);

            Console.WriteLine("CrackResult:" + result.ToString());
        }

        public static string GetToken(string url, string data)
        {
            byte[] source1 = (byte[])null;
            byte[] source2 = (byte[])null;
            int num1 = 0;
            int num2 = 0;
            IntPtr num3 = IntPtr.Zero;
            IntPtr num4 = IntPtr.Zero;
            if (!string.IsNullOrEmpty(url))
            {
                source1 = Encoding.UTF8.GetBytes(url);
                num2 = Marshal.SizeOf((object)source1[0]) * source1.Length;
                num4 = Marshal.AllocHGlobal(num2);
            }
            if (!string.IsNullOrEmpty(data))
            {
                source2 = Encoding.UTF8.GetBytes(data);
                num1 = Marshal.SizeOf((object)source2[0]) * source2.Length;
                num3 = Marshal.AllocHGlobal(num1);
            }
            string empty = string.Empty;
            try
            {
                if (source2 != null)
                    Marshal.Copy(source2, 0, num3, source2.Length);
                if (source1 != null)
                    Marshal.Copy(source1, 0, num4, source1.Length);
                IntPtr coe = IntPtr.Zero;
                int dynamicToken = ComputeDynamicToken(num4, num2, num3, num1, out coe);
                if ((dynamicToken == 0 ? 0 : (coe != IntPtr.Zero ? 1 : 0)) != 0)
                {
                    byte[] numArray = new byte[dynamicToken];
                    Marshal.Copy(coe, numArray, 0, dynamicToken);
                    Freemem(coe);
                    empty = Encoding.UTF8.GetString(numArray);
                }
            }
            catch (Exception ex)
            {
                //mp.Default.j(ex, "c++ dynamic token");
            }
            finally
            {
                if (num3 != IntPtr.Zero)
                    Marshal.FreeHGlobal(num3);
                if (num4 != IntPtr.Zero)
                    Marshal.FreeHGlobal(num4);
            }
            return empty;
        }

        public static void Freemem(IntPtr cuu)
        {
            if (cuu == IntPtr.Zero)
                return;
            FreeMemorys(cuu);
        }


        public static unsafe byte[] _HttpEncrypt_(string url, string data, out string cpm) // url = "/login-otp"   data = "{xxxxxxxx}"   out empty
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] numArray1;
            if (string.IsNullOrEmpty(data))
            {
                cpm = string.Empty;
                numArray1 = bytes;
            }
            else
            {
                fixed (byte* cnn = &Encoding.UTF8.GetBytes(url)[0])
                fixed (byte* cno = &bytes[0])
                {
                    IntPtr cnp = IntPtr.Zero;
                    IntPtr cnq = IntPtr.Zero;
                    int length = HttpEncrypt(cnn, cno, out cnp, out cnq);
                    byte[] destination = new byte[length];
                    byte[] numArray2 = new byte[16];
                    if ((length != 0 ? 1 : (cnp != IntPtr.Zero ? 1 : 0)) != 0)
                    {
                        Marshal.Copy(cnp, destination, 0, length);
                        Freemem(cnp);
                    }
                    if (cnq != IntPtr.Zero)
                    {
                        Marshal.Copy(cnq, numArray2, 0, numArray2.Length);
                        Freemem(cnq);
                    }
                    cpm = Encoding.UTF8.GetString(numArray2);
                    numArray1 = destination;
                }
            }
            return numArray1;
        }

        public static string _ParseLoginResponse_(byte[] cpp, out string cpq) // out empty
        {
            string str;
            if ((cpp == null ? 1 : (cpp.Length == 0 ? 1 : 0)) != 0)
            {
                cpq = string.Empty;
                str = string.Empty;
            }
            else
            {
                IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf((object)cpp[0]) * cpp.Length);
                string empty = string.Empty;
                cpq = string.Empty;
                int length = cpp.Length;
                try
                {
                    Marshal.Copy(cpp, 0, num, cpp.Length);
                    IntPtr cny = IntPtr.Zero;
                    IntPtr cnz = IntPtr.Zero;
                    int loginResponse = ParseLoginResponse(num, length, out cny, out cnz);
                    if ((loginResponse == 0 ? 0 : (cny != IntPtr.Zero ? 1 : 0)) != 0)
                    {
                        byte[] numArray = new byte[loginResponse];
                        Marshal.Copy(cny, numArray, 0, loginResponse);
                        Freemem(cny);
                        empty = Encoding.UTF8.GetString(numArray);
                    }
                    if (cnz != IntPtr.Zero)
                    {
                        byte[] numArray = new byte[16];
                        Marshal.Copy(cnz, numArray, 0, numArray.Length);
                        Freemem(cnz);
                        cpq = Encoding.UTF8.GetString(numArray);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(num);
                }
                str = empty;
            }
            return str;
        }

        public static string _HttpDecrypt_(byte[] cpn, out string cpo) // out empty
        {
            string str;
            if ((cpn == null ? 1 : (cpn.Length == 0 ? 1 : 0)) != 0)
            {
                cpo = string.Empty;
                str = string.Empty;
            }
            else
            {
                IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf((object)cpn[0]) * cpn.Length);
                string empty = string.Empty;
                cpo = string.Empty;
                int length1 = cpn.Length;
                try
                {
                    Marshal.Copy(cpn, 0, num, cpn.Length);
                    IntPtr cok = IntPtr.Zero;
                    IntPtr col = IntPtr.Zero;
                    int length2 = HttpDecrypt(num, length1, out cok, out col);
                    if ((length2 == 0 ? 0 : (cok != IntPtr.Zero ? 1 : 0)) != 0)
                    {
                        byte[] numArray = new byte[length2];
                        Marshal.Copy(cok, numArray, 0, length2);
                        Freemem(cok);
                        empty = Encoding.UTF8.GetString(numArray);
                    }
                    if (col != IntPtr.Zero)
                    {
                        byte[] numArray = new byte[16];
                        Marshal.Copy(col, numArray, 0, numArray.Length);
                        Freemem(col);
                        cpo = Encoding.UTF8.GetString(numArray);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(num);
                }
                str = empty;
            }
            return str;
        }
    }
}
