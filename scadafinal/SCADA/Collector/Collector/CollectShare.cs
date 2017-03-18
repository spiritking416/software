using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.NetworkInformation;
using SCADA;
using HNCAPI;
using ScadaHncData;
using LineDevice;
//-------------------------------------added by zb 20151105--------------------------------------start
//using HncDataInterfaces;
using HncDACore;
//-------------------------------------added by zb 20151105----------------------------------------end

namespace Collector
{
    public class CollectShare : CollectDeviceData
    {
        private static HncDACore.Monitor monitor = new HncDACore.Monitor();      //<---------------added by zb
        private static List<HNCData> _ncDatas;
        public List<HNCData> ncDatas
        {
            get { return _ncDatas; }
            set { _ncDatas = value; }
        }
        private List<MES_DISPATCH> m_Taskdata;
        private static Session m_Instance;
        private List<CollectHNCData> gatherHNCLst = new List<CollectHNCData>();
        private List<CollectHNCData> m_gatherHNCLst = new List<CollectHNCData>();
        private List<CNC> m_cnclist;
        private Thread m_connectThread;
        private object connectUpdateLock = new object();
        const Int32 CONNECT_THREAD_SLEEP_TIME = 2000;
        const Int32 EVENT_THREAD_SLEEP_TIME = 10;
        public CollectShare(ShareData shareData, List<CNC> cnclist)
        {
            ncDatas = shareData.ncDatas;
            m_Taskdata = shareData.m_workorderlist;
            m_cnclist = cnclist;
            m_Instance = null;
            m_connectThread = new Thread(new ThreadStart(ConnectThreadFunc));
            //m_eventThread = new Thread(new ThreadStart(EventThreadFunc));
        }
        public static Session Instance()
        {
            if (m_Instance == null)
            {
                m_Instance = SessionManager.Instance().CreateSession();
                m_Instance.EventAvailable += new Session.Eventhandler(OnEventAvailable);
                m_Instance.CloseEvent += M_Instance_CloseEvent;
                //HncDACore.Monitor.OnRemoveDBReady += Monitor_OnRemoveDBReady;
                monitor.OnRemoveDBReady += monitor_OnRemoveDBReady;
                monitor.Start();
            }
            return m_Instance;
        }

        public void Monitor_NcStateChanged( int nState, string MacID, string MacSn, DateTime tHappendTime)
        {
            System.Threading.Tasks.Task.Run(() => monitor.NCStateChange(nState, MacID, MacSn, tHappendTime));
        }
        static void monitor_OnRemoveDBReady()
        {
            monitor.ProcessConnDatPosi(_ncDatas);
        }
        private static void M_Instance_CloseEvent(Session currentSession)
        {
            monitor.CloseSession();
        }
        /// <summary>
        /// 采集器退出
        /// </summary>
        public void CollectExit()
        {
            //m_bEventThreadRunning = false;
            threaFucEvent.Set();
            m_connectThread.Abort();
            if ((System.Threading.ThreadState.Unstarted | System.Threading.ThreadState.AbortRequested )!= m_connectThread.ThreadState)
            {
                m_connectThread.Join();
                m_connectThread = null;
            }
//             LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "Connect thread exit!");
            //if (System.Threading.ThreadState.Unstarted != m_eventThread.ThreadState)
            //{
            //    m_eventThread.Join();
            //    m_eventThread = null;
            //}
            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "Event thread exit!");

            foreach (CollectHNCData item in m_gatherHNCLst)
            {
                item.ThreadStop();
            }
            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "Collect Exit!");
        }
        /// <summary>
        /// 开启采集线程的线程
        /// </summary>
        private void StartConnectThread()
        {
            for (int ii = 0; ii < m_cnclist.Count; ii++)
            {
                CNC cncref = m_cnclist[ii];
                CollectHNCData m_hcncdatanade = new CollectHNCData(ref cncref, ref _ncDatas);
                m_gatherHNCLst.Add(m_hcncdatanade);
            }

            for (int ii = 0; ii < m_gatherHNCLst.Count;ii++ )
            {
                m_gatherHNCLst[ii].ThreadStart();
            }
            //m_bEventThreadRunning = true;
            m_connectThread.Start();
            threaFucEvent.Set();
        }
        /// <summary>
        /// 定时扫描CNC列表中是否有离线的，如果有做相应处理
        /// </summary>
        public static System.Threading.AutoResetEvent threaFucEvent = new System.Threading.AutoResetEvent(true);
        private void ConnectThreadFunc()
        {
            bool ConnectNoFinish = false;
            List<HNCData> ncDatas = new List<HNCData>();
            while (true)
            {
                if (ConnectNoFinish)
                { 
                    ConnectNoFinish = false;
                    ncDatas.Clear();
                    foreach (CollectHNCData collecncdata in m_gatherHNCLst)
                    {
                        if (!collecncdata.hncdata.sysData.isConnect)
                        {
                            if (ClientPingTest(collecncdata.hncdata.sysData.addr.ip))
                            {
                                collecncdata.hncdata.sysData.clientNo = Instance().HNC_NetConnect(collecncdata.hncdata.sysData.addr.ip, collecncdata.hncdata.sysData.addr.port,true);
                                if (collecncdata.hncdata.sysData.clientNo >= 0 && collecncdata.hncdata.sysData.clientNo < 256)
                                {
//                                     for (int hageii = 0; hageii < 10; hageii++)
                                    {
                                        collecncdata.UpDataChAxList();
//                                         if (collecncdata.hncdata.chanDataLst.Count > 0 && collecncdata.hncdata.axlist.Count > 0)
//                                         {
//                                             break;
//                                         }

                                        ncDatas.Add(collecncdata.hncdata);
                                    }
                                    if (collecncdata.hncdata.chanDataLst.Count > 0 && collecncdata.hncdata.axlist.Count > 0)//确保采集器是初始化OK
                                    {
                                        collecncdata.CollectConstData();
                                        if (collecncdata.hncdata.sysData.clientNo >= 0 && collecncdata.hncdata.sysData.clientNo < 256
                                            && collecncdata.hncdata.sysData.macSN != null && collecncdata.hncdata.sysData.macSN.Length > 0)
                                        {
                                            collecncdata.threaFucRuningF_OK = true;
                                            collecncdata.Get_Reg_threaFucEvent.Set();
                                            collecncdata.hncdata.sysData.isConnect = true;

                                            if (collecncdata.m_cnc.CNCchanDataEventHandler != null)
                                            {
                                                int[] senddata = new int[2];
                                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                                senddata[1] = (int)LineDevice.CNC.CNCState.IDLE;
                                                collecncdata.m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                                            }

                                            AlarmData alm = new AlarmData();
                                            m_Instance.HNC_AlarmRefresh(collecncdata.hncdata.sysData.clientNo);
                                            int alarm_num = 0;
                                            m_Instance.HNC_AlarmGetNum((int)HNCAPI.AlarmType.ALARM_TYPE_ALL, (int)HNCAPI.AlarmLevel.ALARM_ERR, ref alarm_num, collecncdata.hncdata.sysData.clientNo);
                                            for (int i = 0; i < alarm_num; i++)
                                            {
                                                m_Instance.HNC_AlarmGetData((int)HNCAPI.AlarmType.ALARM_TYPE_ALL,  //获取所有类型的报警
                                                                         (int)HNCAPI.AlarmLevel.ALARM_ERR, //仅获取error
                                                                         i,
                                                                         ref alm.alarmNo,     //获取此报警的唯一ID，可用于报警识别
                                                                         ref alm.alarmTxt,             //报警文本
                                                                         collecncdata.hncdata.sysData.clientNo);
                                                alm.isOnOff = 1;
                                                if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                                                {
                                                    EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
//                                                     SendMeg.alardat = new ScadaHncData.AlarmData();
                                                    SendMeg.NeedFindTeX = false;
                                                    SendMeg.BujianID = collecncdata.m_cnc.BujianID;
                                                    SendMeg.alardat.isOnOff = 1;
                                                    SendMeg.alardat = alm;
                                                    MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                                                }

                                                UpdateCurrentAlarmLst(collecncdata.hncdata, alm);
                                            }
                                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                                            SendParm.Keywords = "CNC采集器开启";
                                            SendParm.EventData = collecncdata.m_cnc.BujianID + ":ip = " + collecncdata.hncdata.sysData.addr.ip
                                                + "  链接号 = " + collecncdata.hncdata.sysData.clientNo.ToString()
                                                               + " sn = " + collecncdata.hncdata.sysData.macSN;
                                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                                        }
                                    }
                                    else
                                    {
//                                         SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                                         SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                                         SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                                         SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                                         SendParm.Keywords = "collecncdata.hncdata.chanDataLst.Count = 0";
//                                         SendParm.EventData = collecncdata.hncdata.sysData.addr.ip;
//                                         SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                                    }
                                }
                            }
                        }
                        if (!collecncdata.hncdata.sysData.isConnect)
                        {
                            ConnectNoFinish = true;
                        }
                    }
                }
                else
                {
                    threaFucEvent.WaitOne();
                    ConnectNoFinish = true;
                }

                //---------------------------------added by zb 20151105--------------------------start
                if (ncDatas.Count > 0)
                   monitor.ProcessConnDatPosi(ncDatas);
                //---------------------------------added by zb 20151105--------------------------end

                //                 Thread.Sleep(CONNECT_THREAD_SLEEP_TIME);
            }
        }
        /// <summary>
        /// ping IP测试
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private Boolean ClientPingTest(String ip)
        {
            try 
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip, 1000);
                if (pingReply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private static void OnEventAvailable(HNCAPI.SEventElement e, Int16 clientNo)
        {
            SAlarmEvent almEV = new SAlarmEvent();
            switch (e.code)
            {
                case EVENTDEF.ncEvtAlarmChg://告警上报、消除
                    almEV = CGlbFunc.ByteToStructure<SAlarmEvent>(e.buf, 4);//从4开始偏移
                    GatherAlarmData(almEV, clientNo);
                    break;
                case EVENTDEF.ncEvtPrgStart:
                    UpdateLastProgStartTime(clientNo);
                    break;
                default:
                    break;
            }
          //  monitor.Process(e);
          System.Threading.Tasks.Task.Run(() => monitor.Process(e));
        }
        private static void UpdateLastProgStartTime(Int16 clientNo)
        {
            HNCData result = _ncDatas.Find(
                   delegate(HNCData temp)
                   {
                       return temp.sysData.clientNo == clientNo;
                   }
                   );

            if (result == null)
            {
                return;
            }

            result.sysData.lastProgStartTime = DateTime.Now;
        }
        public void StartCollectBase(Int16 clientNo)
        {
            string macSN = "";
            int ret = 0;
            //开启一个线程隔N长事件收集一次
            HNCData result = ncDatas.Find(
                     delegate(HNCData temp)
                     {
                         return temp.sysData.clientNo == clientNo;
                     }
                     );

            if (result == null)
            {
                return;                
            }

            ret = m_Instance.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_SN_NUM, ref macSN, clientNo);
            if (ret != 0)
            {
                return ;
            }
            CollectHNCData colresult = gatherHNCLst.Find(
                    delegate(CollectHNCData temp)
                    {
                        return String.Equals(temp.hncdata.sysData.macSN, macSN, StringComparison.Ordinal);
                    }
                    );

            if (colresult == null)//没有重复机床，第一次加入
            {
//                 CollectHNCData gatherHdata = new CollectHNCData(result);
//                 gatherHNCLst.Add(gatherHdata);
//                 gatherHdata.ThreadStart();
            }
            else //以前有过同SN的机床
            { 
                if (colresult.collectThread == null)
                {
//                     colresult.bCollect = true;
//                     colresult.ThreadStart();
                }
            }
        }
        public void StopCollectBase(short clientNo)
        {
            String sLogInfo = String.Empty;

            CollectHNCData result = gatherHNCLst.Find(
                  delegate(CollectHNCData temp)
                  {
                      return temp.hncdata.sysData.clientNo == clientNo;
                  });
            if (result == null)
            {
                return;
            }
            else
            {
//                 sLogInfo = "Client disconnect but no ncdata matched! clientNo = " + clientNo.ToString();
//                 LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
            }

            CNC cncItem = m_cnclist.Find(
                delegate(CNC temp)
                {
                    return (temp.ip == result.hncdata.sysData.addr.ip
                            && temp.port == result.hncdata.sysData.addr.port);
                });
            if (cncItem != null)
            {
                //cncItem.HCNCShareData = null;
//                 cncItem.isConnect = false;
            }

            result.bCollect = false;
            result.collectThread.Join();
            result.hncdata.sysData.clientNo = -1;

//             sLogInfo = "Client disconnect! clientNo = " + clientNo.ToString()
//                                + "ip = " + cncItem.ip + "port = " + cncItem.port.ToString();
//             LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
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
                ret = m_Instance.HNC_ChannelGetValue((int)HNCAPI.HncChannel.HNC_CHAN_IS_EXIST, ch, 0, ref chanIsExist, clientNo);
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
            ret = m_Instance.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_CHAN_NUM, ref chanNum, clientNo);
            if (ret != 0)
            {
                return mask;
            }

            for (chNo = 0; chNo < chanNum; chNo++)
            {
                ret = m_Instance.HNC_ChannelGetValue((int)HNCAPI.HncChannel.HNC_CHAN_AXES_MASK, chNo, 0, ref axisMask, clientNo);
                if (ret == 0)
                {
                    mask |= axisMask;
                }
            }

            return mask;
        }
        void GetClientCh(HNCAPI.SEventElement ev, ref short clientNo, ref  short ch)
        {
            short[] info = new short[2];
            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
            clientNo = info[0];
            ch = info[1];        
        }
        /// <summary>
        /// 验证连接号为clientNo的cnc是否存在
        /// </summary>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public bool VerifyClient(short clientNo)
        {
            int ret = 0;
            string macSN = "";
            bool flag = false;
            ret = m_Instance.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_SN_NUM, ref macSN, clientNo);
            if (ret != 0)
            {
                return false;
            }
               
            HNCData result = ncDatas.Find(
                      delegate(HNCData temp)
                      {                            
                          return String.Equals(temp.sysData.macSN, macSN, StringComparison.Ordinal);
                      }
                     );

            if (result == null) //没找到
            {
                flag = true;
            }
            else //找到同SN的机床，以最新clientNo为准
            {
                flag = true;
                if (result.sysData.clientNo != clientNo)
                {
                    result.sysData.clientNo = clientNo;
                }
            }

            return flag;
        }
        public static void GatherAlarmData(SAlarmEvent almEV,short clientNo)
        {
            AlarmData alm = new AlarmData();
            alm.time = DateTime.Now;
            alm.isOnOff = almEV.begin;
            alm.alarmNo = almEV.alarmNo;
            alm.alarmTxt = System.Text.Encoding.Default.GetString(almEV.alarmText).Trim('\0');

            LineDevice.CNC result = MainForm.cnclist.Find(
                    delegate(LineDevice.CNC temp)
                    {
                        return temp.HCNCShareData.sysData.clientNo == clientNo;
                    }
                   );

            if (result != null)
            {
                if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                {
                    EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
//                     SendMeg.alardat = new ScadaHncData.AlarmData();
                    SendMeg.NeedFindTeX = false;
                    SendMeg.BujianID = result.BujianID;
                    SendMeg.alardat = alm;
                    MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                }

                if (result.HCNCShareData.alarmList.Count == result.HCNCShareData.AlmLstLen)
                {
                    result.HCNCShareData.alarmList.RemoveRange(0, result.HCNCShareData.AlmLstLen / 2);
                }
                UpdateCurrentAlarmLst(result.HCNCShareData, alm);
            }
            else if (PLCDataShare.m_plclist != null && PLCDataShare.m_plclist.Count > 0 &&
                PLCDataShare.m_plclist[0].m_hncPLCCollector != null && clientNo == Collector.CollectHNCPLC.m_clientNo)//HNC8 PLC报警
            {
                if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                {
                    EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
//                     SendMeg.alardat = new ScadaHncData.AlarmData();
                    SendMeg.NeedFindTeX = false;
                    SendMeg.BujianID = PLCDataShare.m_plclist[0].m_hncPLCCollector.EQUIP_CODE;
                    SendMeg.alardat = alm;
                    MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                }
            }
        }
        private static void UpdateCurrentAlarmLst(HNCData data, AlarmData alm)
        {

            if (alm.isOnOff == 1)
            {
                if (data.currentAlarmList.Count > 0)
                {
                    bool find = false;
                    for (Int32 i = 0; i < data.currentAlarmList.Count; i++)
                    {
                        if (data.currentAlarmList[i].alarmNo == alm.alarmNo
                            && data.currentAlarmList[i].alarmTxt == alm.alarmTxt
                            && data.currentAlarmList[i].isOnOff == 1)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        data.currentAlarmList.Add(alm);
                    }
                }
                else
                {
                    data.currentAlarmList.Add(alm);
                }
            }
            else
            {
                for (Int32 i = 0; i < data.currentAlarmList.Count; i++)
                {
                    if (data.currentAlarmList[i].alarmNo == alm.alarmNo
                        && data.currentAlarmList[i].alarmTxt == alm.alarmTxt
                        && data.currentAlarmList[i].isOnOff == 1)
                    {
                        data.currentAlarmList.RemoveAt(i);
                        break;
                    }
                }
            }
            data.alarmList.Add(alm);
        }
        public static Int32 HNC_RegGetValue(Int32 type, Int32 index, out int value, Int16 clientNo)
        {
            Int32 ret = -1;

            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X://x
                case (int)HncRegType.REG_TYPE_Y://y
                case (int)HncRegType.REG_TYPE_R://r
                    byte value8 = 0;
                    ret = Instance().HNC_RegGetValue(type, index, ref value8, clientNo);
                    value = value8;
                    break;
                case (int)HncRegType.REG_TYPE_F://f
                case (int)HncRegType.REG_TYPE_G://g
                case (int)HncRegType.REG_TYPE_W://w
                    Int16 value16 = 0;
                    ret = Instance().HNC_RegGetValue(type, index, ref value16, clientNo);
                    value = value16;
                    break;
                case (int)HncRegType.REG_TYPE_D://d
                case (int)HncRegType.REG_TYPE_B://b
                case (int)HncRegType.REG_TYPE_P://p
                    Int32 value32 = 0;
                    ret = Instance().HNC_RegGetValue(type, index, ref value32, clientNo);
                    value = value32;
                    break;
                default:
                    value = -1;
                    break;
            }
            return ret;
        }
        public static Int32 HNC_RegSetValue(Int32 type, Int32 index, ref int value, Int16 clientNo)
        {
            int ret = -1;
            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X://x
                case (int)HncRegType.REG_TYPE_Y://y
                case (int)HncRegType.REG_TYPE_R://r
                    byte value8 = byte.Parse(value.ToString());
                    ret = Instance().HNC_RegSetValue(type, index, value8, clientNo);
                    break;
                case (int)HncRegType.REG_TYPE_F://f
                case (int)HncRegType.REG_TYPE_G://g
                case (int)HncRegType.REG_TYPE_W://w
                    Int16 value16 = Int16.Parse(value.ToString());
                    ret = Instance().HNC_RegSetValue(type, index, value16, clientNo);
                    break;
                case (int)HncRegType.REG_TYPE_D://d
                case (int)HncRegType.REG_TYPE_B://b
                case (int)HncRegType.REG_TYPE_P://p
                    ret = Instance().HNC_RegSetValue(type, index, value, clientNo);
                    break;
                default:
                    break;
            }
            return ret;
        }
        public void StartCollection()
        {
            StartConnectThread();
            //StartEventThread();
        }
    }
}