using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Collector
{
    class CGlbFunc
    {
        public static void GetLocalIpAddr(ref string localIP)
        {
            string hostName = Dns.GetHostName();
            IPHostEntry localHost = Dns.GetHostEntry(hostName);
            IPAddress localIpAddr = null;
            //localIpAddr = localHost.AddressList[1]; // AddressList[0]:IPV6    AddressList[1]:IPV4
            foreach (IPAddress ip in localHost.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIpAddr = ip;
                    break;
                }
            }
            localIP = localIpAddr.ToString();
        }

        public static T ByteToStructure<T>(byte[] dataBuffer,int startIndex)
        {
            object structure = null;
            int size = Marshal.SizeOf(typeof(T));
            if (size > dataBuffer.Length)
            {
                //返回空
                return (default(T));
            }
            //Debug.Assert(size == dataBuffer.Length);

            IntPtr allocIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(dataBuffer, startIndex, allocIntPtr, size);
                structure = Marshal.PtrToStructure(allocIntPtr, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(allocIntPtr);
            }
            return (T)structure;
        }
    }

    class ErrLog
    {
        public static void writeErrLog(string ex)
        {
            string FILE_NAME = "D:\\ErrLog.txt";
            string c = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (!File.Exists(FILE_NAME))
            {
                StreamWriter sr = File.CreateText(FILE_NAME);
                sr.Close();
            }
            StreamWriter x = new StreamWriter(FILE_NAME, true, System.Text.Encoding.Default);
            x.Write("exception:"+ ex);
            x.WriteLine("\t" + c);
            x.Close();
        }
    }
}
