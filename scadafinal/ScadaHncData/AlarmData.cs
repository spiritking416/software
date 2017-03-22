
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ScadaHncData
{
    [Serializable]
    public class AlarmData
    {
        public DateTime time;
        public Int32 isOnOff;
        public Int32 alarmNo;
        public String alarmTxt;
    }

    [Serializable]
    public class SExtraEvent
    {
        public Int16 clientNo;
        public Int16 chNo;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public class SAlarmEvent
    {
        public Int32 alarmNo;// 报警号
        public Int32 begin;// 发生还是消除
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] alarmText;
    }
}