using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace NeteaseLogin
{
    public static class TranMessage
    {
        public struct LoginMsg
        {
            public byte ErrCode;
        }

        public struct JoinRoomMsg
        {
            public byte ErrCode;

            public ushort ByteLength;

            public List<uint> UserIDs;
        }

        public struct GuestNotifyMsg
        {
            public byte IsEnter;

            public uint UserID;
        }

        public struct VirtualConnectMsg
        {
            public uint UserID;
        }

        public struct VirtualDisConnectMsg
        {
            public uint UserID;
        }

        public struct ChangeRoomNameMsg
        {
            public byte ErrCode;
        }

        public struct CreateRoomMsg
        {
            public byte ErrCode;

            public uint roomID;
        }

        public struct RoomListMsg
        {
            public byte ErrCode;

            public ushort Length;

            public List<RoomMsg> Rooms;
        }

        public struct RoomListGlobalMsg
        {
            public byte ErrCode;

            public ushort Length;

            public List<RoomGlobalMsg> Rooms;
        }

        public struct RoomMsg
        {
            public uint HostId;

            public uint RoomId;

            public byte Capacity;

            public byte Residual;

            public byte RoomType;

            public string RoomName;

            public string HostName;

            public GameDescription GameInfo;
        }

        public struct RoomGlobalMsg
        {
            public uint HostId;

            public uint RoomId;

            public byte Capacity;

            public byte Residual;

            public byte RoomType;

            public string RoomName;

            public string HostName;

            public GameDescription GameInfo;

            public string ServerAddr;
        }
    }
    public class SimpleUnpack
    {
        private byte[] content;

        private int idx;

        private ushort lastLen;

        public SimpleUnpack(byte[] bytes)
        {
            content = bytes;
            idx = 0;
            lastLen = 0;
        }

        public void Unpack<T>(ref T content)
        {
            Type typeFromHandle = typeof(T);
            FieldInfo[] fields = typeFromHandle.GetFields();
            FieldInfo[] array = fields;
            foreach (FieldInfo fieldInfo in array)
            {
                object value = fieldInfo.GetValue(content);
                Type fieldType = fieldInfo.FieldType;
                InnerUnpack(ref value, fieldType);
                object obj = content;
                fieldInfo.SetValue(obj, value);
                content = ConvertValue<T>(obj);
            }
        }

        public static T ConvertValue<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private void InnerUnpack(ref object value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.UInt32:
                    value = BitConverter.ToUInt32(content, idx);
                    idx += 4;
                    break;
                case TypeCode.Int32:
                    value = BitConverter.ToInt32(content, idx);
                    idx += 4;
                    break;
                case TypeCode.UInt16:
                    value = BitConverter.ToUInt16(content, idx);
                    idx += 2;
                    lastLen = (ushort)value;
                    break;
                case TypeCode.Int16:
                    value = BitConverter.ToInt16(content, idx);
                    idx += 2;
                    break;
                case TypeCode.String:
                    {
                        ushort num4 = lastLen;
                        value = Encoding.UTF8.GetString(content, idx, num4);
                        idx += num4;
                        break;
                    }
                case TypeCode.Byte:
                    value = content[idx];
                    idx++;
                    break;
                case TypeCode.Object:
                    if (type == typeof(GameDescription))
                    {
                        ushort num = lastLen;
                        string @string = Encoding.UTF8.GetString(content, idx, num);
                        value = JsonConvert.DeserializeObject<GameDescription>(@string);
                        idx += num;
                    }
                    else if (type == typeof(byte[]))
                    {
                        ushort num2 = lastLen;
                        value = content.Skip(idx).Take(num2).ToArray();
                        idx += num2;
                    }
                    else if (type == typeof(List<uint>))
                    {
                        ushort num3 = lastLen;
                        List<uint> list = new List<uint>();
                        while (num3 > 0)
                        {
                            uint item = BitConverter.ToUInt32(content, idx);
                            list.Add(item);
                            idx += 4;
                            num3 = (ushort)(num3 - 4);
                        }
                        value = list;
                    }
                    else if (!(type == typeof(List<TranMessage.RoomMsg>)) && !(type == typeof(List<TranMessage.RoomGlobalMsg>)))
                    {
                        //Logger.Default.Debug("type:" + type);
                        //Logger.Default.Debug("input type error & case (TypeCode.Object)");
                    }
                    break;
                default:
                    //Logger.Default.Debug("input type error");
                    break;
            }
        }
    }
    public class CC
    {
        public short a;

        public ushort b;

        public string c;
    }
    public class ChaCha : ChaChaX
    {
        public ChaCha(byte[] eay) : base(8, eay)
        {
        }
    }
    public class ChaChaX
    {
        private IntPtr ctx;

        protected unsafe ChaChaX(uint lv, byte[] key)
        {
            fixed (byte* key2 = &key[0])
            {
                ctx = Call.NewChaCha(lv, key2);
            }
        }

        ~ChaChaX()
        {
            Call.DeleteChaCha(ctx);
        }

        public unsafe void Process(byte[] data)
        {
            if (data.Length != 0)
            {
                fixed (byte* data2 = &data[0])
                {
                    Call.ChaChaProcess(ctx, data2, (uint)data.Length);
                }
            }
        }
    }

    public class EncryptionHelper
    {
        private byte[] key = new byte[10];

        public EncryptionHelper(byte[] key)
        {
            key.CopyTo((Array)this.key, 0);
        }

        public unsafe uint Encrypt(uint w)
        {
            fixed (byte* key = &this.key[0])
                return Call.ParseUUID(true, key, w);
        }

        public unsafe uint Decrypt(uint w)
        {
            fixed (byte* key = &this.key[0])
                return Call.ParseUUID(false, key, w);
        }

        public unsafe static bool c(byte[] cpf, byte[] cpg)
        {
            fixed (byte* coq = &cpf[0])
            {
                fixed (byte* cos = &cpg[0])
                {
                    return Call.Parse2(coq, 32u, cos, 16u);
                }
            }
        }
    }
    public class VerificationAccount
    {
        public string username { get; set; }

        public string uuid { get; set; }

        public string accessToken { get; set; }

        public VerificationAccount(string username, string uuid, string accessToken)
        {
            this.username = username;
            this.uuid = uuid;
            this.accessToken = accessToken;
        }
    }

    public class Others
    {

        public const int UUID_LENGTH = 32;
        private static byte[] Skip32Key = Encoding.UTF8.GetBytes("SaintSteve");
        private static EncryptionHelper c = new EncryptionHelper(Skip32Key);
        public static VerificationAccount CurrentAccount;

        public sealed class Session
        {
            public byte[] uuid;
            public byte[] accessToken;
        }

        public static string ByteToString(byte[] jbi, int jbj = 0, int jbk = 0)
        {
            string str = "";
            if ((jbk == 0) || (jbk > jbi.Length))
            {
                jbk = jbi.Length;
            }
            for (int i = jbj; i < jbk; i++)
            {
                str = str + $"{jbi[i]:x2}";
            }
            return str;
        }
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where<int>((Func<int, bool>)(x => x % 2 == 0)).Select<int, byte>((Func<int, byte>)(x => Convert.ToByte(hex.Substring(x, 2), 16))).ToArray<byte>();
        }

        public static string NameGuidFromBytes(uint userid, string roleName)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(new UTF8Encoding().GetBytes(roleName));
            byte[] bytes = BitConverter.GetBytes(c.Encrypt(userid));
            Console.WriteLine("Encript:" + byteToHexStr(bytes));
            Buffer.BlockCopy(bytes, 0, hash, 12, bytes.Length);
            hash[6] &= (byte)15;
            hash[6] |= (byte)48;
            hash[8] &= (byte)63;
            hash[8] |= (byte)128;
            return byteToHexStr(hash);
        }

        public static string NameHypixelGuidFromBytes(string roleName)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(new UTF8Encoding().GetBytes(roleName));
            hash[6] &= (byte)15;
            hash[6] |= (byte)64;
            hash[8] &= (byte)63;
            hash[8] |= (byte)128;
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower().Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
        }

        public static VerificationAccount GetFakeAccount()
        {
            byte[] data = new byte[0x10];
            RandomNumberGenerator.Create().GetBytes(data);
            byte[] buffer2 = new byte[0x10];
            RandomNumberGenerator.Create().GetBytes(buffer2);
            return new VerificationAccount("cheat", byteToHexStr(data), byteToHexStr(buffer2));//name id token
        }
        public static VerificationAccount GetVerificationAccountEx(uint userid, string token, string roleName)
        {
            VerificationAccount FakeAccount = GetFakeAccount();
            VerificationAccount verificationAccount = new VerificationAccount(roleName, NameGuidFromBytes(userid, roleName), FakeAccount.accessToken);
            Console.WriteLine("昵称：" + verificationAccount.username);
            Console.WriteLine("uuid：" + verificationAccount.uuid);
            Console.WriteLine("accessToken：" + verificationAccount.accessToken);
            return CurrentAccount = verificationAccount;
            //verificationAccount.username + "|" + verificationAccount.uuid + "|" + verificationAccount.accessToken
        }
        public static uint GetUserIdFromUUID(string uuid)
        {
            uuid = uuid.Replace("-", "");
            if (uuid.Length != 32)
            {
                Call.Log += "[Skin]参数uuid长度有误\r\n";
                Console.WriteLine("参数uuid长度有误");
                return 0u;
            }
            byte[] value = HexStringToByteArray(uuid);
            uint w = BitConverter.ToUInt32(value, 12);
            return c.Decrypt(w);
        }
    }
}
