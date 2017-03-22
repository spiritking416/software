using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCADA;
using HNCAPI;
using System.Threading;
using ScadaHncData;

namespace Collector
{
    public class CollectHNCData : CollectDeviceData,IDisposable
    {
        public LineDevice.CNC m_cnc;
        public HNCData hncdata;
        public bool bCollect;
        public Thread collectThread;

        private CollectSysData gatherSYS;
        private CollectChanData gatherCH;
        private CollectAxisData gatherAXIS;

        public CollectHNCData(ref LineDevice.CNC cnc, ref List<HNCData> hncdatalist)
        {
            gatherSYS = new CollectSysData();
            gatherCH = new CollectChanData();
            gatherAXIS = new CollectAxisData();
            hncdata = new HNCData();
            hncdatalist.Add(hncdata);
            m_cnc = cnc;
            m_cnc.HCNCShareData = hncdata;
            hncdata.sysData.addr.ip = cnc.ip;
            hncdata.sysData.addr.port = cnc.port;
            hncdata.sysData.deviceCode = cnc.BujianID;
            bCollect = true;
        }

        public bool GatherData()
        {
            short clientNo = hncdata.sysData.clientNo;

            bool result = false;
            result = gatherSYS.GatherData(hncdata.sysData);

            for (int ii = 0; ii < hncdata.chanDataLst.Count; ii++)
            {
                result = gatherCH.GatherData(hncdata.chanDataLst[ii], clientNo) || result;
            }
            for (int ii = 0; ii < hncdata.axlist.Count; ii++)
            {
                result = gatherAXIS.GatherData(hncdata.axlist[ii], clientNo) || result;
            }


//             foreach (ChannelData chdata in hncdata.chanDataLst)
//             {
//                 result = gatherCH.GatherData(chdata, clientNo) || result;
//             }
            
//             foreach (AxisData axdata in hncdata.axlist)
//             {
//                 result = gatherAXIS.GatherData(axdata, clientNo) || result;
//             }
            
            return result;
        }//根据clientNo采集数据

        public bool CollectConstData()
        {
            bool result = false;
            short clientNo = hncdata.sysData.clientNo;
            result = gatherSYS.GatherConstData(hncdata.sysData);

            foreach (ChannelData chdata in hncdata.chanDataLst)
            {
                result = gatherCH.GatherConstData(chdata, clientNo) || result;
            }

            foreach (AxisData axdata in hncdata.axlist)
            {
                result = gatherAXIS.GatherConstData(axdata, clientNo) || result;
            }

//             bool result = true;
//             short clientNo = hncdata.sysData.clientNo;
//             result = gatherSYS.GatherConstData(hncdata.sysData);
// 
//             foreach (ChannelData chdata in hncdata.chanDataLst)
//             {
//                 result = gatherCH.GatherConstData(chdata, clientNo) && result;
//             }
// 
//             foreach (AxisData axdata in hncdata.axlist)
//             {
//                 result = gatherAXIS.GatherConstData(axdata, clientNo) && result;
//             }

            return result;
        }

        public void ThreadStart()
        {
            collectThread = new Thread(new ThreadStart(DataCollectThread));
            collectThread.Start(); 
        }

        public void ThreadStop()
        {
            bCollect = false;
            if (System.Threading.ThreadState.Unstarted != collectThread.ThreadState)
            {
                if (!threaFucRuningF_OK)
                {
                    threaFucRuningF_OK = true;
                    Get_Reg_threaFucEvent.Set();
                }
                collectThread.Join();
                collectThread = null;
//                 SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                 SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                 SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                 SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                 SendParm.Keywords = "CNC采集器正常退出";
//                 SendParm.EventData = m_cnc.BujianID + ":ip = " + hncdata.sysData.addr.ip;
//                 SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
            }

        }

        public System.Threading.AutoResetEvent Get_Reg_threaFucEvent = new System.Threading.AutoResetEvent(true);
        public bool threaFucRuningF_OK = false;
        static object GatherDataLockOBJ = new object();
        private int baogongold = 0;
        public void DataCollectThread()
        {
            int iiiii = 0;
            while (bCollect)
            {
                if (threaFucRuningF_OK)
                {
                    string macSN = "";
                    if(CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_SN_NUM, ref macSN, hncdata.sysData.clientNo) == 0)
                    {
                        iiiii = 0;
                        if(macSN != hncdata.sysData.macSN)
                        {
                            hncdata.sysData.isConnect = false;
                            threaFucRuningF_OK = false;
                            CollectShare.threaFucEvent.Set();

                            if (m_cnc.CNCchanDataEventHandler != null)
                            {
                                int[] senddata = new int[2];
                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                senddata[1] = (int)LineDevice.CNC.CNCState.DISCON;
                                m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                            }

                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "CNC采集器关闭";
                            SendParm.EventData = hncdata.sysData.addr.ip +"：本次获取SN=" + macSN + "；上周期SN=" + hncdata.sysData.macSN;
                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                    }
                    else
                    {
                        iiiii++;
                        if (iiiii >= 10)
                        {
                            hncdata.sysData.isConnect = false;
                            threaFucRuningF_OK = false;
                            CollectShare.threaFucEvent.Set();

                            if (m_cnc.CNCchanDataEventHandler != null)
                            {
                                int[] senddata = new int[2];
                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                senddata[1] = (int)LineDevice.CNC.CNCState.DISCON;
                                m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                            }

                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "CNC采集器关闭";
                            SendParm.EventData = hncdata.sysData.addr.ip + "：累计连续10次调用网络接口失败！";
                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                    }
                    GatherData();

                    LineDevice.CNC.CNCState CNCStatei = LineDevice.CNC.CNCState.DISCON;
                    if (m_cnc != null && m_cnc.Checkcnc_state(ref CNCStatei) && m_cnc.CNCchanDataEventHandler != null)
                    {
                        int[] senddata = new int[2];
                        senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                        senddata[1] = (int)CNCStatei;
                        m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                    }
                    if (m_cnc != null && baogongold != m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum && m_cnc.CNCchanDataEventHandler != null)
                    {
                        int[] senddata = new int[2];
                        senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCReport;
                        senddata[1] = m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum;
                        m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                        baogongold = m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum;
                    }

                    System.Threading.Thread.Sleep(50);
                }
                else
                {
                    Get_Reg_threaFucEvent.WaitOne();
                    iiiii = 0;
                }
            }
        }

        public void UpDataChAxList()
        {
            hncdata.ConfigCH(GetClientChMask(hncdata.sysData.clientNo));
            hncdata.ConfigAX(GetClientAxMask(hncdata.sysData.clientNo));
        }
        /// <summary>
        /// 获取通道掩码
        /// </summary>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        private Int32 GetClientChMask(Int16 clientNo)
        {
            Int32 mask = 0;

            Int32 ret = 0;
            Int32 ch = 0;
            Int32 chanIsExist = 0;

            for (ch = 0; ch < HNCDATADEF.SYS_CHAN_NUM; ch++)
            {
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_EXIST, ch, 0, ref chanIsExist, clientNo);
                if ((chanIsExist == 1) && (ret == 0))
                {
                    mask |= (1 << ch);
                }
            }

            return mask;
        }

        /// <summary>
        /// 获取轴号掩码
        /// </summary>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        private Int32 GetClientAxMask(Int16 clientNo)
        {
            Int32 mask = 0;

            Int32 axisMask = 0;
            Int32 chNo = 0;
            Int32 chanNum = 0;
            Int32 ret = 0;
            ret = CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_CHAN_NUM, ref chanNum, clientNo);
            if (ret != 0)
            {
                return mask;
            }

            for (chNo = 0; chNo < chanNum; chNo++)
            {
                ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_AXES_MASK, chNo, 0, ref axisMask, clientNo);
                if (ret == 0)
                {
                    mask |= axisMask;
                }
            }

            return mask;
        }

        public void Dispose()
        {
            Get_Reg_threaFucEvent.Dispose();
        }
    }
}