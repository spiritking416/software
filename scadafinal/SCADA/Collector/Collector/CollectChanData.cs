using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HNCAPI;
using ScadaHncData;

namespace Collector
{
    public class CollectChanData : CollectDeviceCHData
    {
        #region collector
        public class MacTypeCollector : CollectDeviceCHData
        {
            public override bool GatherData( ChannelData chData, short clientNo)
            {
                int ret = 0;
                int macType = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_MAC_TYPE, chData.chNo, 0, ref macType, clientNo);
                if (ret == 0)
                {
                    chData.macType = macType;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class AxisMaskCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int axisMask = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_AXES_MASK, chData.chNo, 0, ref axisMask, clientNo);
                if (ret == 0)
                {
                    chData.axisMask = axisMask;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class ChNameCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {            
                int ret = 0;
                string chName = "";
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_NAME, chData.chNo,0,ref chName, clientNo);
                if (ret == 0)
                {
                    chData.chName = chName;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class CmdfFeedrateCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                double cmdFeedrate = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_CMD_FEEDRATE, chData.chNo, 0, ref cmdFeedrate, clientNo);
                if (ret == 0)
                {
                    chData.cmdFeedrate = cmdFeedrate;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class ActFeedrateCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                double actFeedrate = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_FEEDRATE, chData.chNo, 0, ref actFeedrate, clientNo);
                if (ret == 0)
                {
                    chData.actFeedrate = actFeedrate;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class FeedOverrideCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int feedOverride = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_FEED_OVERRIDE, chData.chNo, 0, ref feedOverride, clientNo);
                if (ret == 0)
                {
                    chData.feedOverride = feedOverride;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class SpindleOverrideCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int spindleOverride = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_SPDL_OVERRIDE, chData.chNo, 0, ref spindleOverride, clientNo);
                if (ret == 0)
                {
                    chData.spindleOverride = spindleOverride;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class RapidOverrideCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int rapidOverride = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_RAPID_OVERRIDE, chData.chNo, 0, ref rapidOverride, clientNo);
                if (ret == 0)
                {
                    chData.rapidOverride = rapidOverride;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class WorkModeCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int workMode = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_MODE, chData.chNo, 0, ref workMode, clientNo);
                if (ret == 0)
                {
                    chData.workMode = workMode;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsCyclingCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isCycling = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_CYCLE, chData.chNo, 0, ref isCycling, clientNo);
                if (ret == 0)
                {
                   chData.isCycling = isCycling;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsMdiCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isMdi = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_MDI, chData.chNo, 0, ref isMdi, clientNo);
                if (ret == 0)
                {
                    chData.isMdi = isMdi;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsHoldingCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isHolding = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_HOLD, chData.chNo, 0, ref isHolding, clientNo);
                if (ret == 0)
                {
                    chData.isHolding = isHolding;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsEstopSelCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isEstop = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_ESTOP, chData.chNo, 0, ref isEstop, clientNo);
                if (ret == 0)
                {
                    chData.isEstop = isEstop;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsHomingCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isHoming = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_HOMING, chData.chNo, 0, ref isHoming, clientNo);
                if (ret == 0)
                {
                    chData.isHoming = isHoming;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsThreadingCollector : CollectDeviceCHData
        {
            public override  bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isThreading = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_THREADING, chData.chNo, 0, ref isThreading, clientNo);
                if (ret == 0)
                {
                    chData.isThreading = isThreading;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsProgSelCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isProgSel = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_PROGSEL, chData.chNo, 0, ref isProgSel, clientNo);
                if (ret == 0)
                {
                    chData.isProgSel = isProgSel;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsProgEndCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isProgEnd = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_PROGEND, chData.chNo, 0, ref isProgEnd, clientNo);
                if (ret == 0)
                {
                    chData.isProgEnd = isProgEnd;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsRewindedCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isRewinded = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_REWINDED, chData.chNo, 0, ref isRewinded, clientNo);
                if (ret == 0)
                {
                    chData.isRewinded = isRewinded;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class IsResetingCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int isReseting = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_RESETTING, chData.chNo, 0, ref isReseting, clientNo);
                if (ret == 0)
                {
                    chData.isReseting = isReseting;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class RunLineCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int runLine = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_RUN_ROW, chData.chNo, 0, ref runLine, clientNo);
                if (ret == 0)
                {
                    chData.runLine = runLine;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class GCodeNameCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                string gCodeName = "";
                ret = CollectShare.Instance().HNC_FprogGetFullName(chData.chNo, ref gCodeName, clientNo);
                if (ret == 0)
                {
                    chData.gCodeName = gCodeName;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class PartNumCollector : CollectDeviceCHData
        {
//             public LineDevice.CNC m_cnc;
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int partNum = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_PART_CNTR, chData.chNo, 0, ref partNum, clientNo);
                if (ret == 0)
                {
//                     if (chData.partNum != partNum && m_cnc != null && m_cnc.CNCchanDataEventHandler != null)
//                     {
//                         int[] senddata = new int[2];
//                         senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCReport;
//                         senddata[1] = partNum;
//                         m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
//                     }
                    chData.partNum = partNum;
                }
                else
                {
                    SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                    SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                    SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                    SendParm.Keywords = "NCN采集数据失败";
                    SendParm.EventData = clientNo.ToString() + "  采集加工完成数失败！";
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }
                return (ret == 0 ? true : false);
            }
        }

        public class ToolUseCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                int toolUse = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_TOOL_USE, chData.chNo, 0, ref toolUse, clientNo);
                if (ret == 0)
                {
                    chData.toolUse = toolUse;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class SpdlSpeedCollector : CollectDeviceCHData
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                Double spdlSpeed = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_SPDL_SPEED, chData.chNo, 0, ref spdlSpeed, clientNo);
                if (ret == 0)
                {
                    chData.spdlSpeed = spdlSpeed;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class TOOLPOS_NUMCollector : CollectDeviceCHData//获取刀位总数 
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                Double spdlSpeed = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_SPDL_SPEED, chData.chNo, 0, ref spdlSpeed, clientNo);
                if (ret == 0)
                {
                    chData.spdlSpeed = spdlSpeed;
                }
                return (ret == 0 ? true : false);
            }
        }
        public class TOOL_NUMCollector : CollectDeviceCHData//获取当前刀具数  
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                Double spdlSpeed = 0;
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_SPDL_SPEED, chData.chNo, 0, ref spdlSpeed, clientNo);
                if (ret == 0)
                {
                    chData.spdlSpeed = spdlSpeed;
                }
                return (ret == 0 ? true : false);
            }
        }

        public class TOOL_NOCollector : CollectDeviceCHData//获取当前刀具号，这个刀具号是标签里面的号
        {
            public override bool GatherData(ChannelData chData, short clientNo)
            {
                int ret = 0;
                Double TOOL_NO = 0;
                ret = CollectShare.Instance().HNC_ToolGetToolPara(chData.toolUse, (int)ToolParaIndex.INFTOOL_ID, ref TOOL_NO, clientNo);
                if (ret == 0)
                {
                    chData.TOOL_NO = (long)TOOL_NO;
                }
                return (ret == 0 ? true : false);
            }
        }
        
        #endregion

        List<CollectDeviceCHData> chGatherlist;
        List<CollectDeviceCHData> chConstList;

        MacTypeCollector macTypeGather;
        AxisMaskCollector axisMaskGather;
        ChNameCollector chNameGather;
        CmdfFeedrateCollector cmdfFeedrateGather;
        ActFeedrateCollector actFeedrateGather;
        FeedOverrideCollector feedOverrideGather;
        SpindleOverrideCollector spindleOverrideGather;
        RapidOverrideCollector rapidOverrideGather;
        WorkModeCollector workModeGather;
        IsCyclingCollector isCyclingGather;
        IsMdiCollector isMdiGather;
        IsHoldingCollector isHoldingGather;
        IsEstopSelCollector isEstopSelGather;
        IsHomingCollector isHomingGather;
        IsThreadingCollector isThreadingGather;
        IsProgSelCollector isProgSelGather;
        IsProgEndCollector isProgEndGather;
        IsRewindedCollector isRewindedGather;
        IsResetingCollector isResetingGather;
        RunLineCollector runLineGather;
        GCodeNameCollector gCodeNameGather;
        PartNumCollector partNumGather;
        ToolUseCollector toolUseGather;
        SpdlSpeedCollector spdlSpeedGather;

        TOOL_NOCollector TOOL_NOGather;
        public CollectChanData()
        {
            macTypeGather = new MacTypeCollector();
            axisMaskGather = new AxisMaskCollector();
            chNameGather = new ChNameCollector();
            cmdfFeedrateGather = new CmdfFeedrateCollector();
            actFeedrateGather = new ActFeedrateCollector();
            feedOverrideGather = new FeedOverrideCollector();
            spindleOverrideGather = new SpindleOverrideCollector();
            rapidOverrideGather = new RapidOverrideCollector();
            workModeGather = new WorkModeCollector();
            isCyclingGather = new IsCyclingCollector();
            isMdiGather = new IsMdiCollector();
            isHoldingGather = new IsHoldingCollector();
            isEstopSelGather = new IsEstopSelCollector();
            isHomingGather = new IsHomingCollector();
            isThreadingGather = new IsThreadingCollector();
            isProgSelGather = new IsProgSelCollector();
            isProgEndGather = new IsProgEndCollector();
            isRewindedGather = new IsRewindedCollector();
            isResetingGather = new IsResetingCollector();
            runLineGather = new RunLineCollector();
            gCodeNameGather = new GCodeNameCollector();
            partNumGather = new PartNumCollector();
//             partNumGather.m_cnc = m_cnc;

            toolUseGather = new ToolUseCollector();
            spdlSpeedGather = new SpdlSpeedCollector();

            TOOL_NOGather = new TOOL_NOCollector();

            chGatherlist = new List<CollectDeviceCHData>();
            chGatherlist.Add(macTypeGather);
            chGatherlist.Add(axisMaskGather);
            chGatherlist.Add(chNameGather);
            chGatherlist.Add(cmdfFeedrateGather);
            chGatherlist.Add(actFeedrateGather);
            chGatherlist.Add(feedOverrideGather);
            chGatherlist.Add(spindleOverrideGather);
            chGatherlist.Add(rapidOverrideGather);
            chGatherlist.Add(workModeGather);
            chGatherlist.Add(isCyclingGather);
            chGatherlist.Add(isMdiGather);
            chGatherlist.Add(isHoldingGather);
            chGatherlist.Add(isEstopSelGather);
            chGatherlist.Add(isHomingGather);
            chGatherlist.Add(isThreadingGather);
            chGatherlist.Add(isProgSelGather);
            chGatherlist.Add(isProgEndGather);
            chGatherlist.Add(isRewindedGather);
            chGatherlist.Add(isResetingGather);
            chGatherlist.Add(runLineGather);
            chGatherlist.Add(gCodeNameGather);
            chGatherlist.Add(partNumGather);
            chGatherlist.Add(toolUseGather);
            chGatherlist.Add(spdlSpeedGather);
            chGatherlist.Add(TOOL_NOGather);


            chConstList = new List<CollectDeviceCHData>();

            //chConstList.Clear();
        }

        public override bool GatherData(ChannelData chdata, short clientNo)
        {
            bool result = true;

            foreach(CollectDeviceCHData temp in chGatherlist)
            {
                result = temp.GatherData(chdata, clientNo) && result;
            }  

            return result;
        }

        public bool GatherConstData(ChannelData chdata, short clientNo)
        {
            bool result = true;

            foreach (CollectDeviceCHData temp in chConstList)
            {
                result = temp.GatherData(chdata, clientNo) && result;
            }

            return result;
        }

        ~CollectChanData()
        {
            chGatherlist.Clear();
        }
    }

}