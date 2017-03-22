using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScadaHncData
{
    [Serializable]
    public class Addr
    {
        public String ip;
        public UInt16 port;
    }

    [Serializable]
    public class SysVer
    {
        public String nc;
        public String cnc;
        public String ncu;
        public String plc;
        public String drv;
    }

    [Serializable]
    public class SystemData : BaseNC
    {
        public String macSN;
        public short clientNo;
        public bool isConnect;
        public Addr addr;
        public SysVer sysver;
        public Int32 accessLevel;
        public Int32 moveUnit;
        public Int32 turnUnit;
        public Int32 metric;
        public String deviceCode;
        public DateTime lastProgStartTime;
    }
}