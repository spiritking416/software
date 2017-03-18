using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SCADA
{
    public enum ENUM_LOG_LEVEL
    {
        //error级别日志，记录系统运行的严重问题，例如找不到配置文件、内部重要模块初始化失败等造成整个系统功能不可用的情况。
        LEVEL_ERRO = 0,

        //waring级别日志，记录某个业务操作失败，例如一次查询、设置数据失败等某一次单个操作失败，不会影响整个系统不可用的情况。
        LEVEL_WARN = 1,

        //info级别日志，记录当前系统的正常运行场景，例如系统启动、系统退出、某个模块初始化成功，注意不能是大量出现的信息。
        LEVEL_INFO = 2,

        //debug级别日志，记录供调试级的日志，一般用于记录大量的信息供问题定位，例如每个周期每个特殊寄存器的状态。
        //因为写日志操作较多，必须通过SetLogLevel接口设置级别为LEVEL_DEBU时才会记录。而且对应的日志生成后还需要将级别设回LEVEL_INFO，以防止写日志操作过于频繁影响系统运行。
        LEVEL_DEBU = 3,

        MAX_LOG_LEVEL = LEVEL_DEBU
    };



    public class LogApi
    {
        [DllImport("LogDll.dll", EntryPoint = "LogInitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean LogInitialize(ref UInt32 handle, String sLogPath, UInt16 maxSize, Byte maxFileCnt);

        [DllImport("LogDll.dll", EntryPoint = "WriteLogInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteLogInfo(UInt32 handle, Byte logLevel, String logInfo);

        [DllImport("LogDll.dll", EntryPoint = "LogExit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogExit(UInt32 handle);

        [DllImport("LogDll.dll", EntryPoint = "SetLogLevel", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean SetLogLevel(UInt32 handle, Byte logLevel);
    }
}
