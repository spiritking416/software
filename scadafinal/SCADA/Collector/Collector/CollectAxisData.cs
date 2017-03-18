using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HNCAPI;
using ScadaHncData;
using SCADA;

namespace Collector
{
    public class CollectAxisData : CollectDeviceAXData
    {

        class AxisNameCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {               
                int ret = 0;
                string axisName = "";
                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_NAME, axdata.axisNo, ref axisName, clientNo);
                if (ret == 0)
                {
                    axdata.axisName = axisName;
                }
                return (ret == 0 ? true : false);
            }
        }

        class AxisTypeCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                int ret = 0;
                int axisType=0;
                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_TYPE, axdata.axisNo, ref axisType, clientNo);
                if (ret == 0)
                {
                    axdata.axisType = axisType;
                }
                return (ret == 0 ? true : false);
            }
        }

        class AxisDistCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                Int32 ret = 0;
                const Int32 PAR_AX_PM_MUNIT = 4;// 坐标轴参数
                
                ret = CollectShare.Instance().HNC_ParamanGetI32((Int32)HNCDATADEF.PARAMAN_FILE_AXIS, axdata.axisNo, PAR_AX_PM_MUNIT, ref axdata.dist, clientNo);
                if (ret != 0)
                {
                    return false;
                }

                return (ret == 0 ? true : false);
            }
        }

        class AxisPulseCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                Int32 ret = 0;
                const Int32 PAR_AX_PM_PULSE = 5;// 坐标轴参数

                ret = CollectShare.Instance().HNC_ParamanGetI32((Int32)HNCDATADEF.PARAMAN_FILE_AXIS, axdata.axisNo, PAR_AX_PM_PULSE, ref axdata.pulse, clientNo);
                if (ret != 0)
                {
                    return false;
                }

                return (ret == 0 ? true : false);
            }
        }

        class ActPosCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                Int32 ret = 0;
                Int32 pos = 0;

                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, axdata.axisNo, ref pos, clientNo);
                if (ret != 0)
                {
                    return false;
                }
                axdata.actPos = (Double)pos;
                return true;
            }
        }

        class CmdPosCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                Int32 ret = 0;
                Int32 pos = 0;

                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS, axdata.axisNo, ref pos, clientNo);
                if (ret != 0)
                {
                    return false;
                }

                axdata.cmdPos = (Double)pos;

                return true;
            }
        }

        class FollowErrCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                Int32 ret = 0;

                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_FOLLOW_ERR, axdata.axisNo, ref axdata.followErr, clientNo);
                if (ret != 0)
                {
                    return false;
                }

                return true;
            }
        }

        class SvCurrentCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                int ret = 0;
                double svCurrent = 0;
                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_RATED_CUR, axdata.axisNo, ref svCurrent, clientNo);
                if (ret == 0)
                {
                    axdata.svCurrent = svCurrent;
                }
                return (ret == 0 ? true : false);
            }
        }

        class LoadCurrentCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                int ret = 0;
                double loadCurrent = 0;
                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, axdata.axisNo, ref loadCurrent, clientNo);
                if (ret == 0)
                {
                    axdata.loadCurrent = loadCurrent;
                }
                return (ret == 0 ? true : false);
            }
        }

        class DrvVerCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                int ret = 0;
                string drvVer = "";
                ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_DRIVE_VER, axdata.axisNo, ref drvVer, clientNo);
                if (ret == 0)
                {
                    axdata.drvVer = drvVer;
                }
                return (ret == 0 ? true : false);
            }
        }

        List<CollectDeviceAXData> axGatherlist;
        List<CollectDeviceAXData> axConstlist;
        AxisNameCollector axnameGather;
        AxisTypeCollector axisTypeGather;
        AxisDistCollector distGather;
        AxisPulseCollector pulseGather;
        ActPosCollector actPosGather;
        CmdPosCollector cmdPosGather;
        FollowErrCollector followErrGather;
        SvCurrentCollector svCurrentGather;
        LoadCurrentCollector loadCurrentGather;
        DrvVerCollector drvVerGather;

        public CollectAxisData()
        {
            axnameGather = new AxisNameCollector();
            axisTypeGather = new AxisTypeCollector();
            distGather = new AxisDistCollector();
            pulseGather = new AxisPulseCollector();
            actPosGather = new ActPosCollector();
            cmdPosGather = new CmdPosCollector();
            followErrGather = new FollowErrCollector();
            svCurrentGather = new SvCurrentCollector();
            loadCurrentGather = new LoadCurrentCollector();
            drvVerGather = new DrvVerCollector();

            axGatherlist = new List<CollectDeviceAXData>();    
            axGatherlist.Add(actPosGather);
            axGatherlist.Add(cmdPosGather);
            axGatherlist.Add(followErrGather);
            axGatherlist.Add(svCurrentGather);
            axGatherlist.Add(loadCurrentGather);
            axGatherlist.Add(drvVerGather);

            axConstlist = new List<CollectDeviceAXData>();
            axConstlist.Add(axnameGather);
            axConstlist.Add(axisTypeGather);
            axConstlist.Add(distGather);
            axConstlist.Add(pulseGather);
        }

        public override bool GatherData(AxisData  axdata,short clientNo)
        {
            bool result = true;
            foreach (CollectDeviceAXData temp in axGatherlist)
            {
                result = temp.GatherData(axdata, clientNo) && result;
            }

            return result;
        }

        public bool GatherConstData(AxisData axdata, short clientNo)
        {
            bool result = true;
            foreach (CollectDeviceAXData temp in axGatherlist)
            {
                result = temp.GatherData(axdata, clientNo) && result;
            }

            return result;
        } 

        ~CollectAxisData()
        {
            axGatherlist.Clear();
        }
    }

}