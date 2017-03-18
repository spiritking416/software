using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScadaHncData
{
    [Serializable]
    public class Report_Infos
    {
        public SystemData sysData;
        public List<PROD_REPORT> reportList;
    }


    [Serializable]
    public class HNCData : BaseNC
    {
        public readonly Int32 AlmLstLen = 200;
        public readonly Int32 MachLstLen = 200;
        public SystemData sysData;
        public List<ChannelData> chanDataLst;
        public List<AxisData> axlist;

        public List<PROD_REPORT> reportList;
        public List<AlarmData> alarmList;
        public List<AlarmData> currentAlarmList;
        
        private const Int32 AxisTypesCount = 9;

        public HNCData(/*Int16 clientNo, int chMask, int axMask*/)
        {
            sysData = new SystemData();
            sysData.sysver = new SysVer();
            sysData.addr = new Addr();
            chanDataLst = new List<ChannelData>();
            axlist = new List<AxisData>();

            alarmList = new List<AlarmData>();
            currentAlarmList = new List<AlarmData>();
            reportList = new List<PROD_REPORT>();

            sysData.clientNo = -1;//clientNo;  //记住这里还要获得clientno!!!!!~~~#@$#

            sysData.macSN = "";
            sysData.addr.ip = "";
            sysData.addr.port = 0;
            sysData.sysver.cnc = "";
            sysData.sysver.drv = "";
            sysData.sysver.nc = "";
            sysData.sysver.ncu = "";
            sysData.sysver.plc = "";
            sysData.accessLevel = 0;
            sysData.moveUnit = 1;
            sysData.turnUnit = 1;
            sysData.metric = 1;
            sysData.deviceCode = "";
            sysData.lastProgStartTime = DateTime.Now;


//             ConfigCH(clientNo, chMask);
//             ConfigAX(clientNo, axMask);
        }

        /// <summary>
        /// 通道参数初始化
        /// </summary>
        /// <param name="clientNo"></param>
        /// <param name="chMask"></param>
        public void ConfigCH(int chMask)
        {
            chanDataLst.Clear();
            const Int32 MAX_CH_NUM = 4;
            ChannelData chData;
            for (Int32 i = 0; i < MAX_CH_NUM; i++)
            {
                if ((chMask >> i & 0x01) == 1)
                {
                    chData = new ChannelData();
                    chData.chNo = i;
                    chanDataLst.Add(chData);
                }
            }
        }

        /// <summary>
        /// 轴参数初始化
        /// </summary>
        /// <param name="clientNo"></param>
        /// <param name="axMask"></param>
        public void ConfigAX(int axMask)    
        {
            axlist.Clear();
            const Int32 MAX_AXIS_NUM = 32;
            AxisData axis;
            for (Int32 i = 0; i < MAX_AXIS_NUM; i++)
            {
                if ((axMask >> i & 0x01) == 1)
                {
                    axis = new AxisData();
                    axis.axisNo = i;
                    axlist.Add(axis);
                }
            }
        } 
    }
}