
<div align="center">
<img src="https://github.com/hyunsssssssss/NeteaseLauncher/blob/master/docs/logo2.svg" alt="logo" width="200" height="200" align="center" />

### NeteaseLauncher

<strong>此项目已于2019年4月停止更新</strong>
****
</div>

> **简洁、美观、强大** 的第三方网易我的世界客户端



## 介绍

* 简洁：**去除无用功能**，仅保留进入服务器所必须的核心功能

![服务器们](https://github.com/hyunsssssssss/NeteaseLauncher/blob/master/docs/servers.gif "服务器们")

* 美观：**精心设计UI**，拥有细腻的动效与整洁有序的规划

![启动](https://github.com/hyunsssssssss/NeteaseLauncher/blob/master/docs/launch.gif "启动")

* 强大：完整的**客户端依赖补全**，**角色管理**，**账户管理**等功能

![角色选择](https://github.com/hyunsssssssss/NeteaseLauncher/blob/master/docs/userchoose.gif "角色选择")



## 核心原理

1. 加载DLL模块 **api-ms-win-crt-utility-l1-1-1.dll**<sup>[1](#t1)</sup>
2. Patch上述模块的进程文件校验<sup>[1](#t2)</sup>
3. 根据逆向得到的通信协议**动态**调用上述DLL模块的加解密函数
4. 完成通信，使用返回的令牌登录服务器

<a name="t1">1</a>: 主要功能是负责通信的加解密，文件名起混淆作用

<a name="t2">2</a>: 核心模块内存Patch

```c#
    public static int FindAddr(IntPtr PHandle, int ProcAddr, uint findSize)
    {
        //开始寻找的函数地址 寻找范围  注意：未关闭句柄！！！！
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
		//mov al, 1
        //ret
        Console.WriteLine("CrackResult:" + result.ToString());
    }
```



## 文件结构

* **UI**
  * **Res**：导出资源
  * **ui.psd**：主界面UI
  * **about.psd**：关于界面UI
  * **role.psd**：角色选择UI
  * **input.psd**：输入框UI
  * **msg**：msgboxUI
  * **main.e**：UI，调用neteaselogin.dll实现功能
* **C#**
  * **neteaselogin**：提供全部流程所需接口供E调用

