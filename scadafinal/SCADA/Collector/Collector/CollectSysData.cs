using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HNCAPI;
using ScadaHncData;

namespace Collector
{
    public class CollectSysData : CollectDeviceData
    {
        public class MacSnCollector : CollectDeviceData
        {
            public  override bool GatherData(SystemData sysdata)
            {
                short clientNo = sysdata.clientNo;
                int ret = 0;
                string macSN = "";
                ret = CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_SN_NUM, ref macSN, clientNo);
                if (ret == 0)
                {
                    sysdata.macSN = macSN;
                    if (macSN.Length == 0)
                    {
                        ret = -1;
                    }
                }
                return (ret == 0 ? true : false);
            }
        }

        public class SysVerCollector : CollectDeviceData
        {

            public override bool GatherData(SystemData sysdata)
            {
                short clientNo = sysdata.clientNo;
                int ret = 0;
                SysVer sysver = new SysVer();
                ret += CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_NCK_VER, ref sysver.ncu, clientNo);
                ret += CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_DRV_VER, ref sysver.drv, clientNo);
                ret += CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_PLC_VER, ref sysver.plc, clientNo);
                ret += CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_CNC_VER, ref sysver.cnc, clientNo);
                ret += CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_NC_VER, ref sysver.nc, clientNo);
                if (ret == 0)
                {
                    sysdata.sysver = sysver;
                }

                return (ret == 0 ? true : false);
            }
        }

        public class AddrCollector : CollectDeviceData
        {
            public override bool GatherData(SystemData sysdata)
            {
                short clientNo = sysdata.clientNo;
                int ret = 0;
                Addr addr = new Addr();
                ret = CollectShare.Instance().HNC_NetGetIpaddr(ref addr.ip,ref addr.port,clientNo);       
                if (ret == 0)
                {
                    sysdata.addr = addr;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class AccessLevelCollector : CollectDeviceData
        {
            public override bool GatherData(SystemData sysdata)
            {
                short clientNo = sysdata.clientNo;
                int ret = 0;
                int accessLevel = 0;
                ret = CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_ACCESS_LEVEL, ref accessLevel, clientNo);
                if (ret == 0)
                {
                    sysdata.accessLevel = accessLevel;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class MoveUnitCollector : CollectDeviceData
        {
            public override bool GatherData(SystemData sysdata)
            {
                short clientNo = sysdata.clientNo;
                int ret = 0;
                int moveUnit = 0;
                ret = CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_MOVE_UNIT, ref moveUnit, clientNo);
                if (ret == 0)
                {
                    sysdata.moveUnit = moveUnit;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class TurnUnitCollector : CollectDeviceData
        {
            public override bool GatherData(SystemData sysdata)
            {
                short clientNo = sysdata.clientNo;
                int ret = 0;
                int turnUnit = 0;
                ret = CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_TURN_UNIT, ref turnUnit, clientNo);
                if (ret == 0)
                {
                    sysdata.turnUnit = turnUnit;
                }
                return (ret == 0 ? true : false);
            }
        }

        AccessLevelCollector accesslevGather;
        SysVerCollector sysverGather ;
        MacSnCollector macSNGather;
        MoveUnitCollector moveUnitGather;
        TurnUnitCollector turnUnitGather;
        List<CollectDeviceData> sysGatherlist;
        List<CollectDeviceData> sysConstList;
        
        public CollectSysData()
        {
            accesslevGather = new AccessLevelCollector();
            sysverGather = new SysVerCollector();
            macSNGather = new MacSnCollector();
            //addrGather = new AddrCollector();
            moveUnitGather = new MoveUnitCollector();
            turnUnitGather = new TurnUnitCollector();

            sysGatherlist = new List<CollectDeviceData>();
            sysGatherlist.Add(accesslevGather);

            sysConstList = new List<CollectDeviceData>();
            sysConstList.Add(sysverGather);
            sysConstList.Add(macSNGather);
            //sysConstList.Add(addrGather);
            sysConstList.Add(moveUnitGather);
            sysConstList.Add(turnUnitGather);
        }

        public override bool GatherData(SystemData sysdata)
        {
            bool result = true;
            foreach(CollectDeviceData temp in sysGatherlist)
            {
                result = temp.GatherData(sysdata) && result;
            }
            return result;
        }

        public bool GatherConstData(SystemData sysdata)
        {
            bool result = true;
            foreach (CollectDeviceData temp in sysConstList)
            {
                result = temp.GatherData(sysdata) && result;
            }
            return result;
        }

        ~CollectSysData()
        {
            sysGatherlist.Clear();
        }
    }
}