using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using PLC;
using HNCAPI;

namespace Collector
{
    public class CollectHNCPLC
    {
        public Boolean connectStat;
        public delegate void EventHandler<PlcEvent>(object sender, PlcEvent Args);
        public static event EventHandler<PlcEvent> SendMonitorMsgHandler;
        public String EQUIP_CODE;
        public delegate void RFIDEvent<String>(int action,int value);
        public RFIDEvent<String> m_AutoRFIDHandler ;

        private String m_plcIPAddr;
        private UInt16 m_plcPort;
        public static Int16 m_clientNo = 0;
        private List<PLC_MITSUBISHI_HNC8.HNC8SignalType[]> m_plcDataList;

        public struct PLCCollectItem
        {
            public Int32 type;
            public Int32 index;
            public Int32 bit;
            public Boolean isInput;
            public Boolean isValid;
        }
        public class HNC8Reg//8型系统寄存器数据类型
        {
            public int HncRegType;
            public int index;
            public int bit;
            public int value;
        }
        public struct SetGonxuLianWangJiTai
        {
            public int lianjieshu;//链接数量
            public int lianjieshu_old;//链接数量
            public int lixianshu;//离线数量
        }
        private SetGonxuLianWangJiTai[] SetGonxuLianWangJiTaiShuArr = new SetGonxuLianWangJiTai[3];//工序一到三

        public class Hnc8PLCRFID
        {
            //PLC给上位机信息的寄存器
            public HNC8Reg PLC2PCJiTaiReg = new HNC8Reg();//机台号            
            public HNC8Reg PLC2PCJiTaiFinishMonitorBitReg = new HNC8Reg();//机台号完成标记            
            public HNC8Reg PLC2PCRFIDMonitorBitReg = new HNC8Reg();//RFID到位监控点寄存器
            public HNC8Reg PLC2PCRFIDTypeBitReg = new HNC8Reg();//RFID读写类型寄存器
            //上位机给PLC信息的寄存器
            public HNC8Reg PC2PLCOperationBitReg = new HNC8Reg();//写到PLC的读写完成标志寄存器
            public HNC8Reg PC2PLCRFIDLinkStatBitReg = new HNC8Reg();//写到PLC的RFID网络状态标志寄存器
            public HNC8Reg PC2PLCResultBitReg = new HNC8Reg();//写到PLC的比对结果寄存器
            public Int32 CNCIndex;
            public Int32 GongXu;
            public String ReadOrWriteFlag;
            public Int32[] RFIDDataGeShi;
            public byte RFIDDataCount = 0;//数据总字节数
            public String[] RFIDDataStr;
            private byte[] NeedAndOKJiaGong = new byte[4];
            public delegate void RFIDEvent<String>(object obj,int action);
            public static RFIDEvent<String> m_RFIDDataHandler ;
            public SCADA.RFID m_rfid;
            public String EQUIP_CODE;
            bool ReadWriteFinishFlg = true;
            private String Scanning_EQUIP_CODE;

            public void InitHnc8PLCRFID(ref String RegStr, ref String linkAddress, 
                ref String linkPort, ref int GongXu,ref String ReadOrWriteFlag,
                ref String RFIDDataGeShiStr, ref int RFIDserial, ref String ID, ref String Scanning_EQUIP_CODEstr, int rfidcncindex)
            {
                CNCIndex = rfidcncindex;
                Scanning_EQUIP_CODE = Scanning_EQUIP_CODEstr;
                EQUIP_CODE = ID;
                this.GongXu = GongXu;
                this.ReadOrWriteFlag = ReadOrWriteFlag;
                String[] str = RFIDDataGeShiStr.Split(',');
                RFIDDataGeShi = new Int32[str.Length];
                int ii = 0;
                for (; ii < str.Length; ii++)
                {
                    RFIDDataGeShi[ii] = Int32.Parse(str[ii]);
                    RFIDDataCount += (byte)RFIDDataGeShi[ii];
                }
                RFIDDataStr = new string[ii];

                m_rfid = new SCADA.RFID(linkAddress, linkPort, ref RFIDDataStr);
                m_rfid.RFIDserial = RFIDserial;
                m_rfid.StateChangeHandler += new SCADA.RFID.HNC8RFIDEventHandler<PLC.HNC8PLCRFIDEvent>(this.ChangeRFIDLinkState);


                str = RegStr.Split(',');
                int HncRegType = PLC_MITSUBISHI_HNC8.GetHncRegType(str[0].ToUpper());

                PLC2PCJiTaiReg.HncRegType = HncRegType;
                String[] str1 = str[1].Split('.');
                PLC2PCJiTaiReg.index = Int32.Parse(str1[0]);

                str1 = str[2].Split('.');
                PLC2PCJiTaiFinishMonitorBitReg.HncRegType = HncRegType;
                PLC2PCJiTaiFinishMonitorBitReg.index = Int32.Parse(str1[0]);
                PLC2PCJiTaiFinishMonitorBitReg.bit = Int32.Parse(str1[1]);
                int ret = CollectShare.Instance().HNC_RegClrBit((Int32)PLC2PCJiTaiFinishMonitorBitReg.HncRegType, PLC2PCJiTaiFinishMonitorBitReg.index, PLC2PCJiTaiFinishMonitorBitReg.bit, m_clientNo);

                str1 = str[3].Split('.');
                PLC2PCRFIDMonitorBitReg.HncRegType = HncRegType;
                PLC2PCRFIDMonitorBitReg.index = Int32.Parse(str1[0]);
                PLC2PCRFIDMonitorBitReg.bit = Int32.Parse(str1[1]);
                ret = CollectShare.Instance().HNC_RegClrBit((Int32)PLC2PCRFIDMonitorBitReg.HncRegType, PLC2PCRFIDMonitorBitReg.index, PLC2PCRFIDMonitorBitReg.bit, m_clientNo);

                str1 = str[4].Split('.');
                PLC2PCRFIDTypeBitReg.HncRegType = HncRegType;
                PLC2PCRFIDTypeBitReg.index = Int32.Parse(str1[0]);
                PLC2PCRFIDTypeBitReg.bit = Int32.Parse(str1[1]);

                str1 = str[5].Split('.');
                PC2PLCOperationBitReg.HncRegType = HncRegType;
                PC2PLCOperationBitReg.index = Int32.Parse(str1[0]);
                PC2PLCOperationBitReg.bit = Int32.Parse(str1[1]);

                str1 = str[6].Split('.');
                PC2PLCRFIDLinkStatBitReg.HncRegType = HncRegType;
                PC2PLCRFIDLinkStatBitReg.index = Int32.Parse(str1[0]);
                PC2PLCRFIDLinkStatBitReg.bit = Int32.Parse(str1[1]);

                str1 = str[7].Split('.');
                PC2PLCResultBitReg.HncRegType = HncRegType;
                PC2PLCResultBitReg.index = Int32.Parse(str1[0]);
                PC2PLCResultBitReg.bit = Int32.Parse(str1[1]);
            }

            public bool RFIDLinkState = false;
            public bool RFIDLinkState_old = true;

            private void ChangeRFIDLinkState(object sender, PLC.HNC8PLCRFIDEvent e)
            {
                switch (e.EventType)
                {
                    case -1://离线
                        RFIDLinkState = false;
                        RFIDLinkState_old = true;
                        break;
                    case 0://通信正常
                        RFIDLinkState = true;
                        RFIDLinkState_old = false;
                        break;
                    case 100://读写状态更新
                        break;
                    case 200:
                        break;
                    case 300:
                        break;
                    default:
                        break;
                }
            }


            //更新RFID状态标记
            public void UpDataRFIDLinkState2PLC()
            {
                int value = 0;
                if (RFIDLinkState != RFIDLinkState_old)
                {
                    if (CollectShare.Instance().HNC_RegGetValue((Int32)PC2PLCRFIDLinkStatBitReg.HncRegType, PC2PLCRFIDLinkStatBitReg.index, ref value, m_clientNo) == 0)
                    {
                        value = value & (1 << PC2PLCRFIDLinkStatBitReg.bit);
                        value >>= PC2PLCRFIDLinkStatBitReg.bit;
                        if (RFIDLinkState)
                        {
                            if (value != 1)
                            {
                                if(CollectShare.Instance().HNC_RegSetBit((Int32)PC2PLCRFIDLinkStatBitReg.HncRegType, PC2PLCRFIDLinkStatBitReg.index, PC2PLCRFIDLinkStatBitReg.bit, m_clientNo) == 0)
								{
									RFIDLinkState_old = true;
								}
                            }
                        }
                        else
                        {
                            if (value != 0)
                            {
                                if (CollectShare.Instance().HNC_RegClrBit((Int32)PC2PLCRFIDLinkStatBitReg.HncRegType, PC2PLCRFIDLinkStatBitReg.index, PC2PLCRFIDLinkStatBitReg.bit, m_clientNo) == 0)
								{
									RFIDLinkState_old = false;
								}
                            }
                        }
                    }
                }
            }

            //写入加工信息
            public bool WriteRFIDMsg(int CNCIndex)
            {
                bool falg = false;
                int GongXu_i = SCADA.MainForm.cnclist[CNCIndex].GetOP_CODE_Index();//获取工序编码
                if (GongXu_i > 0 && GongXu_i < 17)//只有工序1到16有效
                {
                    if (GongXu_i > 0 && GongXu_i <= 8)
                    {
                        NeedAndOKJiaGong[2] = (byte)(NeedAndOKJiaGong[2] | (1 << (GongXu_i - 1)));
                    }
                    else
                    {
                        NeedAndOKJiaGong[3] = (byte)(NeedAndOKJiaGong[3] | (1 << (GongXu_i - 9)));
                    }

                    for (int ii = 0; ii < 10; ii++)
                    {
//                         String sLogInfo = EQUIP_CODE + ":写入加工工序:" + GongXu_i.ToString();
//                         SCADA.LogApi.WriteLogInfo(SCADA.MainForm.logHandle, (Byte)SCADA.ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
                        falg = m_rfid.WriteUserMessage(28, NeedAndOKJiaGong);
                        if (falg)
                        {
//                             sLogInfo = EQUIP_CODE + ":写入加工工序OK";
//                             SCADA.LogApi.WriteLogInfo(SCADA.MainForm.logHandle, (Byte)SCADA.ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
                            break;
                        }
                    }
                    RFIDDataStr[(GongXu_i + 4)] = SCADA.MainForm.cnclist[CNCIndex].JiTaiHao + "0";//机台:E01+质量：0/1
//                         String sLogInfo = EQUIP_CODE + ":写入加信息:" + SCADA.MainForm.cnclist[CNCIndex].JiTaiHao + "0";
//                         SCADA.LogApi.WriteLogInfo(SCADA.MainForm.logHandle, (Byte)SCADA.ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
                        falg = WriteRFIDData((short)(GongXu_i + 4));//写入已经加工工序
//                         if (falg)
//                         {
//                             sLogInfo = EQUIP_CODE + ":写入加信息OK";
//                             SCADA.LogApi.WriteLogInfo(SCADA.MainForm.logHandle, (Byte)SCADA.ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
//                         }
                    if (falg)
                    {
                        if (SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler != null)
                        {
                            SCADA.RFIDSTRMassgeStr m_massg = new SCADA.RFIDSTRMassgeStr(2, 0, RFIDDataStr, GongXu_i.ToString(), EQUIP_CODE);
                            SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler.BeginInvoke(this, m_massg, null, null);
                        }

//                         SCADA.MainForm.m_ShowRfidDataTable.WriteRfid(ref RFIDDataStr, ref EQUIP_CODE, GongXu_i);
                    }
                }
                return falg;
            }

            //读  比较信息  给出结果
            public void CompareFIDMsg()
            {
                int setvalue = 0;
                if (JudgeGongXu())//如果是本站的工序
                {
                    setvalue = 7;//00000111
                }
                else
                {
                    setvalue = 3;//00000011
                }
                for (int fascs = 0; fascs < 10; fascs++)
                {
                    if (CollectShare.Instance().HNC_RegSetValue((Int32)PC2PLCResultBitReg.HncRegType, PC2PLCResultBitReg.index, setvalue, m_clientNo) == 0)
                    {
                        break;
                    }
                }

            }

            //RFID触发信号操作
            public void RFIDtriggerHandler()
            {
                if (ReadWriteFinishFlg)
                {
                    ReadWriteFinishFlg = false;
                    if(ReadRFIDData())
                    {
//                         if (m_RFIDDataHandler != null)
//                         {
//                             m_RFIDDataHandler.Invoke(this, 0);
//                         }
                        CompareFIDMsg();
                        m_rfid.UpDataReadData2Table();
                        if (SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler != null)
                        {
                            SCADA.RFIDSTRMassgeStr m_massg = new SCADA.RFIDSTRMassgeStr(0, 0, RFIDDataStr, null, EQUIP_CODE);
                            SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler.BeginInvoke(this, m_massg, null, null);
                        }

//                         SCADA.MainForm.m_ShowRfidDataTable.ReadRfid(ref RFIDDataStr, ref EQUIP_CODE);

                        if (SendMonitorMsgHandler != null)//发送仿真需要的RFID触发数据
                        {
                            PlcEvent SendData = new PlcEvent();
                            SendData.EQUIP_CODE = RFIDDataStr[0]; ;
                            SendData.ACTION_ID = Scanning_EQUIP_CODE;
                            SendMonitorMsgHandler.BeginInvoke(this, SendData, null, null);
                        }
                    }
                    ReadWriteFinishFlg = true;
                }
            }

            //RFID给出结果后PLC给的机台号的操作
            private bool JiTaiOkFinishFlg = true;
            public void RFIDJiTaiOkHandler()
            {
                if (JiTaiOkFinishFlg)
                {
                    JiTaiOkFinishFlg = false;
                    Int32 val = 0;
                    int ret = -1; 
                    for (int ii = 0; ii < 100;ii++ )
                    {
                        ret = CollectShare.Instance().HNC_RegGetValue((Int32)PLC2PCJiTaiReg.HncRegType, PLC2PCJiTaiReg.index, ref val, m_clientNo);//获取机台号
//                         String sLogInfo = "GetHNC_RegGetValue_ii=" + ii.ToString() +
//                                             ";ret = " + ret.ToString() + ";val = " + val.ToString();
//                         SCADA.LogApi.WriteLogInfo(SCADA.MainForm.logHandle, (Byte)SCADA.ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);

                        if (ret == 0 && val > 0 && val <= 22)
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    if (val >= 1 && val <= SCADA.MainForm.cnclist.Count())
                    {
                        if (WriteRFIDMsg(val - 1))
                        {
                            int  setvalue = 6;//00000110
                            for (int fascs = 0; fascs < 10; fascs++)
                            {
                                if (CollectShare.Instance().HNC_RegSetValue((Int32)PC2PLCResultBitReg.HncRegType, PC2PLCResultBitReg.index, setvalue, m_clientNo) == 0)
                                {
                                    {
                                        break;
                                    }
                                }
                            }
                            if (SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler != null)
                            {
                                SCADA.RFIDSTRMassgeStr m_massg = new SCADA.RFIDSTRMassgeStr(1, val - 1, RFIDDataStr, null, EQUIP_CODE);
                                SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler.BeginInvoke(this, m_massg, null, null);
                            }

//                                 SCADA.MainForm.m_ShowRfidDataTable.JiTaiJiaGong(val - 1, RFIDDataStr);//添加加工机台号
                        }
                    }
                    JiTaiOkFinishFlg = true;
                }
            }
            /// <summary>
            /// 读标签数据到RFIDDataStr
            /// </summary>
            /// <returns></returns>
            public bool ReadRFIDData()
            {
                byte[] data = new byte[RFIDDataCount];
                bool readFlg = false;
                for (int ii = 0; ii < 20;ii++ )
                {
                    readFlg = m_rfid.ReadUserMessage(RFIDDataCount, ref data);
                    if(readFlg)
                    {
                        break;
                    }
                }
                if (!readFlg)
                {
                    SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                    SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "RFID读失败";
                    SendParm.EventData = EQUIP_CODE + "读失败";
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }
                if (readFlg)
                {
                    byte[] m_data;
                    try
                    {
                        int StarIndex = 0;
                        for (int ii = 0; ii < RFIDDataGeShi.Length; ii++)
                        {
                            m_data = new byte[RFIDDataGeShi[ii]];
                            Array.Copy(data, StarIndex, m_data, 0, RFIDDataGeShi[ii]);
                            if (ii == 3)//代加工位
                            {
                                NeedAndOKJiaGong[0] = m_data[0];
                                NeedAndOKJiaGong[1] = m_data[1];
                                String WeiStr = "";
                                String tenmmm = "";
                                for (int ee = m_data.Length - 1; ee >= 0; ee--)
                                {
                                    tenmmm = System.Convert.ToString(m_data[ee], 2);
                                    while (tenmmm.Length < 8)
                                    {
                                        tenmmm = "0" + tenmmm;
                                    }
                                    WeiStr += tenmmm;
                                }
                                RFIDDataStr[ii]  = WeiStr;
                            }
                            else if (ii == 4)//已加工位
                            {
                                NeedAndOKJiaGong[2] = m_data[0];
                                NeedAndOKJiaGong[3] = m_data[1];
                                String WeiStr = "";
                                String tenmmm = "";
                                for (int ee = m_data.Length - 1; ee >= 0; ee--)
                                {
                                    tenmmm = System.Convert.ToString(m_data[ee], 2);
                                    while (tenmmm.Length < 8)
                                    {
                                        tenmmm = "0" + tenmmm;
                                    }
                                    WeiStr += tenmmm;
                                }
                                RFIDDataStr[ii] = WeiStr;
                            }
                            else
                            {
                                SCADA.HexASCII ha = new SCADA.HexASCII(m_data);
                                RFIDDataStr[ii] = ha.ToString().Trim();
                            }
                            StarIndex += RFIDDataGeShi[ii];
                        }

                    }
                    catch(System.Exception ex)
                    {
                        for (int ii = 0; ii < RFIDDataGeShi.Length; ii++)
                        {
                            RFIDDataStr[ii] = null;
                        }
                        readFlg = false;
                        SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                        SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_RFID;
                        SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                        SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                        SendParm.Keywords = "RFID数据转成字符串失败";
                        SendParm.EventData = EQUIP_CODE + "错误原因：" + ex.ToString();
                        SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

                    }
                }
                return readFlg;
            }

            /// <summary>
            /// 写数据到标签
            /// </summary>
            /// <param name="Index"></param>
            /// <returns></returns>
            private bool WriteRFIDData(Int16 Index)
            {
                bool WriteFlag = false;
                if(Index >= 0 && Index < RFIDDataGeShi.Length)
                {
                    int Star = 0;
                    for (int ii = 0; ii < Index;ii++ )
                    {
                        Star += RFIDDataGeShi[ii];
                    }
                    SCADA.HexASCII ha = new SCADA.HexASCII(RFIDDataStr[Index]);
                    for (int ii = 0; ii < 20;ii++)
                    {
                        WriteFlag = m_rfid.WriteUserMessage(Star, ha.hex);
                        if (WriteFlag)
                        {
                            break;
                        }
                    }
                    if (!WriteFlag)
                    {
                        SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                        SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_RFID;
                        SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                        SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                        SendParm.Keywords = "RFID写失败";
                        SendParm.EventData = EQUIP_CODE + "写失败";
                        SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                    }
                }
                return WriteFlag;
            }
            public bool JudgeGongXu()
            {
                bool Flag = false;
                if (ReadOrWriteFlag == "LineEnd")//线尾
                {
                    for (int ii = 0; ii < RFIDDataStr[3].Length;ii++ )
                    {
                        if (RFIDDataStr[3].Substring(ii,1) == "1")
                        {
                            if (RFIDDataStr[4].Substring(ii, 1) == "0")
                            {
                                Flag = true;
                                break;
                            }
                        }
                    }
                }
                else if (ReadOrWriteFlag == "Read")
                {
                    String Short_name = "";
                    if (CNCIndex >= 0 && CNCIndex < SCADA.MainForm.cnclist.Count())
                    {
                        if (SCADA.MainForm.cnclist[CNCIndex].NcTaskManage != null )
                        {
                            Short_name = SCADA.MainForm.cnclist[CNCIndex].NcTaskManage.Get_DISPATCHShortname();
                        }
                        if(Short_name == "")
                        {
                            if(CNCIndex%2 == 0)//
                            {
                                CNCIndex++;
                            }
                            else
                            {
                                CNCIndex--;
                            }
                            if (CNCIndex >= 0 && CNCIndex < SCADA.MainForm.cnclist.Count())
                            {
                                if (SCADA.MainForm.cnclist[CNCIndex].NcTaskManage != null)
                                {
                                    Short_name = SCADA.MainForm.cnclist[CNCIndex].NcTaskManage.Get_DISPATCHShortname();
                                }
                            }
                        }
                    }
                    if (Short_name != "" && string.Equals(RFIDDataStr[2],Short_name,StringComparison.Ordinal))//物料判断
                    {
                        for (int ii = RFIDDataStr[4].Length - 1; ii >= 0; ii--)
                        {
                            if (RFIDDataStr[4].Substring(ii, 1) == "0")//判断工序
                            {
                                if (RFIDDataStr[3].Substring(ii, 1) == "1" && GongXu == (RFIDDataStr[4].Length - ii))
                                {
                                    Flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                return Flag;
            }
        }
        public List<Hnc8PLCRFID> m_Hnc8PLCRFIDList = new List<Hnc8PLCRFID>();
        #region    RFIDDATAGET
//         public class RFIDDATAGET
//         {
//             public int CNCIndex = -1;
//             public String[] RFIDDataStr;
//             public RFIDDATAGET(ref String[] RFIDDataStr)
//             {
//                 this.RFIDDataStr = new String[RFIDDataStr.Length];
//                 this.RFIDDataStr = (String[])RFIDDataStr.Clone();
//                 CNCIndex = -1;
//             }
//         }
//         public List<RFIDDATAGET> m_RFIDDATAGETList = new List<RFIDDATAGET>();

//         private object HandleRFIDDATAGETList_Lock = new object();
//         private int m_RFIDDATAGETList_maxConut = 10;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action">0:读 ；1:加入机台号</param>
//         private void HandleRFIDDATAGETList(object obj,int action)///处理RFID发过来的数据
//         {
//             Hnc8PLCRFID m_RFIDItem = (Hnc8PLCRFID)obj;
//             lock (HandleRFIDDATAGETList_Lock)
//             {
//                 while (m_RFIDDATAGETList.Count > m_RFIDDATAGETList_maxConut)
//                 {
//                     m_RFIDDATAGETList.RemoveAt(0);
//                 }
//                 RFIDDATAGET resut = m_RFIDDATAGETList.Find(
//                             delegate(RFIDDATAGET temp)
//                             {
//                                 return (temp.RFIDDataStr[0] == m_RFIDItem.RFIDDataStr[0] && temp.RFIDDataStr[1] == m_RFIDItem.RFIDDataStr[1]);
//                             }
//                             );
//                 if (resut == null)//添加RFIDDataStr
//                 {
//                     if (action == 0)//触发
//                     {
//                         RFIDDATAGET m_node = new RFIDDATAGET(ref m_RFIDItem.RFIDDataStr);
//                         m_RFIDItem.CompareFIDMsg();
//                         if (m_RFIDItem.ReadOrWriteFlag == "Read")
//                         {
//                             m_RFIDDATAGETList.Add(m_node);
//                         }
//                         m_RFIDItem.m_rfid.UpDataReadData2Table();
//                         SCADA.MainForm.m_ShowRfidDataTable.ReadRfid(ref m_RFIDItem.RFIDDataStr, ref m_RFIDItem.EQUIP_CODE);
//                     }
//                 }
//                 else
//                 {
//                     if (action == 0)//触发
//                     {
//                         if (resut.CNCIndex != -1)//写
//                         {
//                             if (m_RFIDItem.WriteRFIDMsg(resut.CNCIndex))
//                             {
//                                 m_RFIDDATAGETList.Remove(resut);//写完删掉
//                                 if (m_RFIDItem.ReadRFIDData())//再次读入
//                                 {
//                                     RFIDDATAGET m_node = new RFIDDATAGET(ref m_RFIDItem.RFIDDataStr);
//                                     m_RFIDItem.CompareFIDMsg();
//                                     if (m_RFIDItem.ReadOrWriteFlag == "Read")
//                                     {
//                                         m_RFIDDATAGETList.Add(m_node);
//                                     }
//                                     m_RFIDItem.m_rfid.UpDataReadData2Table();
//                                     SCADA.MainForm.m_ShowRfidDataTable.ReadRfid(ref m_RFIDItem.RFIDDataStr, ref m_RFIDItem.EQUIP_CODE);
//                                 }
//                             }
//                         }
//                         else
//                         {
//                              m_RFIDItem.CompareFIDMsg();
//                              if (m_RFIDItem.ReadOrWriteFlag != "Read")
//                              {
//                                  m_RFIDDATAGETList.Remove(resut);
//                              }
//                              m_RFIDItem.m_rfid.UpDataReadData2Table();
//                              SCADA.MainForm.m_ShowRfidDataTable.ReadRfid(ref m_RFIDItem.RFIDDataStr, ref m_RFIDItem.EQUIP_CODE);
//                         }
//                     }
//                     else if (action == 1)//给出加工机台号
//                     {
//                         resut.CNCIndex = m_RFIDItem.CNCIndex;
//                         SCADA.MainForm.m_ShowRfidDataTable.JiTaiJiaGong(m_RFIDItem.CNCIndex, m_RFIDItem.GongXu.ToString(), resut.RFIDDataStr);//添加加工机台号
//                     }
//                 }
//             }
//         }
        #endregion

        public CollectHNCPLC(String EQUIP_CODE, String ip, UInt16 port, List<PLC_MITSUBISHI_HNC8.HNC8SignalType[]> PLC_SignalList)
        {
            this.EQUIP_CODE = EQUIP_CODE;
            m_plcIPAddr = ip;
            m_plcPort = port;
            connectStat = false;
            m_plcDataList = PLC_SignalList;
            for (int ii = 0; ii < SetGonxuLianWangJiTaiShuArr.Length; ii++)
            {
                SetGonxuLianWangJiTaiShuArr[ii].lianjieshu_old = -1;
            }
        }

        public bool AddRFID(ref String RegStr, ref String linkAddress, ref String linkPort, 
            ref int GongXu, ref String ReadOrWriteFlag, ref String RFIDDataGeShiStr,
            ref int RFIDserial,ref String ID,ref String Scanning_EQUIP_CODEstr,int rfidcncindex)
        {
            bool Flg = false;
            try
            {
                Hnc8PLCRFID m_Hnc8PLCRFID = new Hnc8PLCRFID();
                m_Hnc8PLCRFID.InitHnc8PLCRFID(ref RegStr, ref linkAddress, ref linkPort, ref GongXu,
                    ref ReadOrWriteFlag, ref RFIDDataGeShiStr, ref RFIDserial, ref ID, ref Scanning_EQUIP_CODEstr, rfidcncindex);
                m_Hnc8PLCRFIDList.Add(m_Hnc8PLCRFID);
                m_Hnc8PLCRFID.m_rfid.startCheck();
                Flg = true;
//                 if(Hnc8PLCRFID.m_RFIDDataHandler == null)
//                 {
//                     Hnc8PLCRFID.m_RFIDDataHandler += new Hnc8PLCRFID.RFIDEvent<String>(this.HandleRFIDDATAGETList);
//                 }
            }
            catch
            {

            }
            return Flg;
        }

        /// <summary>
        /// 给PLC初始化P参数
        /// </summary>
        /// <returns></returns>
        private bool InitHNC8PLC()
        {
            bool Flg = false;
            if (connectStat)
            {
                if (!IsSetGongXuOK(ref m_InitGongXuParaList))
                {
                    Flg = SetGongXu2PLC(ref m_InitGongXuParaList);
                }
                else
                {
                    Flg = true;
                }
            }
            SetmRFIDListIndex2plcDataList();//将RFIDListIndex设置到plcDataList的Value中
            m_AutoRFIDHandler = new RFIDEvent<String>(this.RFIDHandler);
            return Flg;
        }

        #region 初始化工序相关操作
        private List<HNC8Reg> m_InitGongXuParaList = new List<HNC8Reg>();
        public bool SetGongXuParam(ref List<String> GongxuList)
        {
            bool Flg = false;
            int ii = 0;
            //hem 20160620
            if (GongxuList.Count < 1) return false;
            for (; ii < GongxuList.Count-1; ii++)
            {
                String[] GongXuarr = GongxuList[ii].Split('=');
                HNC8Reg m_reg = new HNC8Reg();
                m_reg.HncRegType = PLC_MITSUBISHI_HNC8.GetHncRegType(GongXuarr[0].Substring(0, 1));
                m_reg.index = int.Parse(GongXuarr[0].Substring(1, GongXuarr[0].Length - 1));
                m_reg.value = int.Parse(GongXuarr[1]);
                m_InitGongXuParaList.Add(m_reg);
            }
            HNC8Reg m_reglast = new HNC8Reg();
            String[] GongXuarr11 = GongxuList[ii].Split('.');
            m_reglast.HncRegType = PLC_MITSUBISHI_HNC8.GetHncRegType(GongXuarr11[0].Substring(0, 1));
            m_reglast.index = int.Parse(GongXuarr11[0].Substring(1,GongXuarr11[0].Length-1));
            m_reglast.bit = int.Parse(GongXuarr11[1]);
            m_InitGongXuParaList.Add(m_reglast);

            return Flg;
        }
        private bool IsSetGongXuOK(ref List<HNC8Reg> m_ParaList)
        {
            bool Flg = true;
            Int32 tempi = 0;
            int ii = 0;
            //hem 20160620
            if (m_ParaList.Count < 1) return false;
            for (; ii < m_ParaList.Count - 1; ii++)
            {
                int ret = CollectShare.Instance().HNC_ParamanGetI32((Int32)HNCDATADEF.PARAMAN_FILE_MAC, 0, m_ParaList[ii].index + 300, ref tempi, m_clientNo);///P参数从300开始偏移
                if (ret != 0 || tempi != m_ParaList[ii].value)
                {
                    Flg = false;
                    break;
                }
            }
            if (Flg)
            {
                if (CollectShare.Instance().HNC_RegSetBit(m_ParaList[ii].HncRegType, m_ParaList[ii].index, m_ParaList[ii].bit, m_clientNo) != 0) //设置完成位
                {
                    Flg = false;
                }
            }
            return Flg;
        }
        private bool SetGongXu2PLC(ref List<HNC8Reg> m_ParaList)
        {
            bool Flg = true;
            int ii = 0;
            //hem 20160620
            if (m_ParaList.Count < 1) return false;
            for (; ii < m_ParaList.Count - 1; ii++)
            {
                int ret = CollectShare.Instance().HNC_ParamanSetI32((Int32)HNCDATADEF.PARAMAN_FILE_MAC, 0, m_ParaList[ii].index + 300, m_ParaList[ii].value, m_clientNo);///P参数从300开始偏移
                if (ret != 0)
                {
                    Flg = false;
                    break;
                }
            }
            if (Flg)
            {
                if (CollectShare.Instance().HNC_RegSetBit(m_ParaList[ii].HncRegType, m_ParaList[ii].index, m_ParaList[ii].bit, m_clientNo) != 0) //设置完成位
                {
                    Flg = false;
                }
                CollectShare.Instance().HNC_ParamanSave(m_clientNo);
            }
            return Flg;
        }
        #endregion

        //设置PLC监控和RFID监控位和对应关系，PLC监控点的Value对应于m_Hnc8PLCRFIDList的Index
        private void SetmRFIDListIndex2plcDataList()
        {
            for (int jj = 0; jj < m_plcDataList.Count; jj++)
            {
                for (int ii = 1; ii < m_plcDataList[jj].Length; ii++)
                {
                    if (m_plcDataList[jj][ii].EQUIP_CODE == "RFID")//RFID监控类型
                    {
                        for(int kk = 0;kk < m_Hnc8PLCRFIDList.Count;kk++)
                        {
                            if((m_Hnc8PLCRFIDList[kk].PLC2PCRFIDMonitorBitReg.index == m_plcDataList[jj][ii].Address
                                        && m_Hnc8PLCRFIDList[kk].PLC2PCRFIDMonitorBitReg.bit == m_plcDataList[jj][ii].SubAddress)||
                                 (m_Hnc8PLCRFIDList[kk].PLC2PCJiTaiFinishMonitorBitReg.index == m_plcDataList[jj][ii].Address
                                        && m_Hnc8PLCRFIDList[kk].PLC2PCJiTaiFinishMonitorBitReg.bit == m_plcDataList[jj][ii].SubAddress
                                ))
                            {
                                m_plcDataList[jj][ii].RFIDListIndex = kk;
                            }
//                             else if (m_Hnc8PLCRFIDList[kk].PLC2PCJiTaiFinishMonitorBitReg.index == m_plcDataList[jj][ii].Address
//                                         && m_Hnc8PLCRFIDList[kk].PLC2PCJiTaiFinishMonitorBitReg.bit == m_plcDataList[jj][ii].SubAddress)
//                             {
//                                 m_plcDataList[jj][ii].RFIDListIndex = kk;
//                             }
                        }
                    }
                }
            }
        }

        public void Collect()
        {
            if (!connectStat)
            {
                ConnectPLC();
            }
            else
            {
                CollectPLCData();
                //////CNC切入切出控制接口
                for (int cncindex = 0; cncindex < SCADA.MainForm.cnclist.Count; cncindex++)
                {
                    if (SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1 != -1 &&
                        m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].SubAddress != -1)
                    {
                        if (SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SetValue !=
                            m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].Value)
                        {
                            if (SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SetValue == 1)
                            {
                                CollectShare.Instance().HNC_RegSetBit(m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][0].Address,
                                                    m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].Address,
                                                    m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].SubAddress,
                                                    m_clientNo);
                            }
                            else
                            {
                                CollectShare.Instance().HNC_RegClrBit(m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][0].Address,
                                                    m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].Address,
                                                    m_plcDataList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].SubAddress,
                                                    m_clientNo);
                            }
                        }
                    }
                }
            }
        }

        private Boolean ClientPingTest(String ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip, 500);
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

        public void ConnectPLC()//PLC链接
        {
            if (!ClientPingTest(m_plcIPAddr))
            {
                return;
            }

            m_clientNo = CollectShare.Instance().HNC_NetConnect(m_plcIPAddr, m_plcPort,true);

            if (m_clientNo >= 0 && m_clientNo < 255)
            {
                connectStat = true;
                connectStat = InitHNC8PLC();//下发机台工序参数
                for (int ii = 0; ii < m_Hnc8PLCRFIDList.Count;ii++ )
                {
                    m_Hnc8PLCRFIDList[ii].RFIDLinkState = true;
                    m_Hnc8PLCRFIDList[ii].RFIDLinkState_old = false;
                }

                for (int jj = 0; jj < m_plcDataList.Count; jj++)//第一次取值标志
                {
                    for (int ii = 1; ii < m_plcDataList[jj].Length; ii++)
                    {
                        m_plcDataList[jj][ii].FirstGetValue = true;
                    }
                }

                //
                if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.StateChageEvenHandle != null)
                {
                    ScadaHncData.EQUIP_STATE m_EQUIP_STATE = new ScadaHncData.EQUIP_STATE();
                    m_EQUIP_STATE.EQUIP_TYPE = 2;
                    m_EQUIP_STATE.EQUIP_CODE = EQUIP_CODE;// VARCHAR2(50),设备ID
                    m_EQUIP_STATE.EQUIP_CODE_CNC = ""; // VARCHAR2(50),cnc:SN号
                    m_EQUIP_STATE.STATE_VALUE = 1; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                    SCADA.MainForm.m_CheckHander.StateChageEvenHandle.BeginInvoke(this, m_EQUIP_STATE, null, null);
                }

                int alarm_num = 0;
                CollectShare.Instance().HNC_AlarmRefresh(m_clientNo);
                CollectShare.Instance().HNC_AlarmGetNum((int)AlarmType.ALARM_TYPE_ALL, (int)AlarmLevel.ALARM_LEVEL_ALL, ref alarm_num, m_clientNo);
                int alarm_ii = 0;
                string alarmText = "";
                for (int i = 0; i < alarm_num; i++)
                {
                    CollectShare.Instance().HNC_AlarmGetData((int)AlarmType.ALARM_TYPE_ALL,  //获取所有类型的报警
                                             (int)AlarmLevel.ALARM_LEVEL_ALL, //仅获取error
                                             i,
                                             ref alarm_ii,     //获取此报警的唯一ID，可用于报警识别
                                             ref alarmText,             //报警文本
                                             m_clientNo);
                    if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                    {
                        SCADA.EquipmentCheck.AlarmSendData SendMeg = new SCADA.EquipmentCheck.AlarmSendData();
//                         SendMeg.alardat = new ScadaHncData.AlarmData();
                        SendMeg.NeedFindTeX = false;
                        SendMeg.BujianID = SCADA.PLCDataShare.m_plclist[0].m_hncPLCCollector.EQUIP_CODE;
                        SendMeg.alardat.isOnOff = 1;
                        SendMeg.alardat.alarmTxt = alarmText;
                        SendMeg.alardat.alarmNo = alarm_ii;
                        SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                    }
                }

            }
            else
            {
                connectStat = false;
            }
        }

        public int dangqianchanliang = 0;
        public int lishichanliang = 0;
        int SetGonxuLianWangJiTaiSum;
        /// <summary>
        /// PLC周期获取寄存器值的线程函数
        /// </summary>
        private void CollectPLCData()
        {
            Int32 ret = 0;
            Int32 Up_ii = -1;
            int val = 0;
            int val1 = 0;

            try {

                CollectShare.Instance().HNC_RegGetValue((int)HncRegType.REG_TYPE_D, 81, ref dangqianchanliang, m_clientNo);//当前产量
                CollectShare.Instance().HNC_RegGetValue((int)HncRegType.REG_TYPE_D, 82, ref lishichanliang, m_clientNo);//历史产量
                if (CollectShare.Instance().HNC_RegGetValue((int)HncRegType.REG_TYPE_D, 83, ref val, m_clientNo) == 0 && val == 0)//PLC和上位机通信心跳
                {
                    val = 1;
                    CollectShare.Instance().HNC_RegSetValue((int)HncRegType.REG_TYPE_D, 83, val, m_clientNo);
                }

                for(int ii = 0;ii < m_Hnc8PLCRFIDList.Count;ii++)
                {
                    m_Hnc8PLCRFIDList[ii].UpDataRFIDLinkState2PLC();//更新RFID网络状态
                }

                for (int ii = 0; ii < SetGonxuLianWangJiTaiShuArr.Length;ii++ )
                {
                    SetGonxuLianWangJiTaiShuArr[ii].lianjieshu = 0;
                    SetGonxuLianWangJiTaiShuArr[ii].lixianshu = 0;
                }
                SetGonxuLianWangJiTaiSum = 0;

                for (int jj = 0; jj < m_plcDataList.Count; jj++)
                {
                    Up_ii = -1;
                    for (int ii = 1; ii < m_plcDataList[jj].Length; ii++)
                    {
                        if (m_plcDataList[jj][ii].MonitoringFlg || m_plcDataList[jj][ii].IsShow)//只刷新动作监控和界面要显示的值
                        {
                            if (m_plcDataList[jj][ii].EQUIP_CODE == "CNCCONECT")
                            {
                                SetGonxuLianWangJiTaiSum++;
                            }
                            if (m_plcDataList[jj][ii].Address != Up_ii)
                            {
                                ret = CollectShare.Instance().HNC_RegGetValue(m_plcDataList[jj][0].Address, m_plcDataList[jj][ii].Address, ref val, m_clientNo);
                                Up_ii = m_plcDataList[jj][ii].Address;
                            }
                            if (ret == 0)
                            {
                                if (m_plcDataList[jj][ii].SubAddress != -1)//监控位
                                {
                                    val1 = val;
                                    val1 = val1 & (1 << m_plcDataList[jj][ii].SubAddress);
                                    val1 >>= m_plcDataList[jj][ii].SubAddress;
                                }
                                else
                                {
                                    val1 = val;
                                }
                                if (m_plcDataList[jj][ii].EQUIP_CODE == "RFID")
                                {
                                    if (val1 == 1)
                                    {
                                        if (m_plcDataList[jj][ii].ACTION_ID == "trigger")
                                        {
                                            m_AutoRFIDHandler.BeginInvoke(0, m_plcDataList[jj][ii].RFIDListIndex, null, null);
                                            for (int yyy = 0; yyy < 3; yyy++)
                                            {
                                                if (0 == CollectShare.Instance().HNC_RegClrBit((Int32)m_Hnc8PLCRFIDList[m_plcDataList[jj][ii].RFIDListIndex].PLC2PCRFIDMonitorBitReg.HncRegType,
                                                m_Hnc8PLCRFIDList[m_plcDataList[jj][ii].RFIDListIndex].PLC2PCRFIDMonitorBitReg.index,
                                                m_Hnc8PLCRFIDList[m_plcDataList[jj][ii].RFIDListIndex].PLC2PCRFIDMonitorBitReg.bit, m_clientNo))//清除触发监控位
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        else if (m_plcDataList[jj][ii].ACTION_ID == "OK")
                                        {
                                            m_AutoRFIDHandler.BeginInvoke(1, m_plcDataList[jj][ii].RFIDListIndex, null, null);
                                            for (int yyy = 0; yyy < 3; yyy++)
                                            {
                                                if (0 == CollectShare.Instance().HNC_RegClrBit((Int32)m_Hnc8PLCRFIDList[m_plcDataList[jj][ii].RFIDListIndex].PLC2PCJiTaiFinishMonitorBitReg.HncRegType,
                                                m_Hnc8PLCRFIDList[m_plcDataList[jj][ii].RFIDListIndex].PLC2PCJiTaiFinishMonitorBitReg.index,
                                                m_Hnc8PLCRFIDList[m_plcDataList[jj][ii].RFIDListIndex].PLC2PCJiTaiFinishMonitorBitReg.bit, m_clientNo))//清除触发监控位
                                                {
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                }
                                else if (m_plcDataList[jj][ii].EQUIP_CODE == "CNCCONECT")
                                {
                                    int gongxu = 0;
                                    if (int.TryParse(m_plcDataList[jj][ii].ACTION_ID, out gongxu) && gongxu > 0 && gongxu < 4)
                                    {
                                        if (val1 == 1)
                                        {
                                            SetGonxuLianWangJiTaiShuArr[gongxu - 1].lianjieshu++;
                                        }
                                        else
                                        {
                                            SetGonxuLianWangJiTaiShuArr[gongxu - 1].lixianshu++;
                                        }
                                    }
                                }
                                else if (m_plcDataList[jj][ii].EQUIP_CODE == "OneInOneOut")
                                {
                                    int jitai = 0;
                                    if (int.TryParse(m_plcDataList[jj][ii].ACTION_ID, out jitai) && jitai > 0 && jitai < 23)
                                    {
                                        jitai--;
                                        if (val1 == 1 && SCADA.MainForm.cnclist[jitai].OneInOneOut)
                                        {
                                            SCADA.MainForm.cnclist[jitai].OneInOneOut = false;
                                        }
                                        else if (!SCADA.MainForm.cnclist[jitai].OneInOneOut)
                                        {
                                            SCADA.MainForm.cnclist[jitai].OneInOneOut = true;
                                        }
                                    }
                                }
                                else if (m_plcDataList[jj][ii].ACTION_ID == "PLCALARM" &&
                                    (m_plcDataList[jj][ii].Value != val1 || m_plcDataList[jj][ii].FirstGetValue))//PLC监控的设备报警
                                {
                                    if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                                    {
                                        SCADA.EquipmentCheck.AlarmSendData SendMeg = new SCADA.EquipmentCheck.AlarmSendData();
//                                         SendMeg.alardat = new ScadaHncData.AlarmData();
                                        SendMeg.NeedFindTeX = true;
                                        SendMeg.BujianID = m_plcDataList[jj][ii].EQUIP_CODE;
                                        SendMeg.alardat.alarmNo = val1;
                                        SendMeg.alardat.time = DateTime.Now;
                                        SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                                        if (m_plcDataList[jj][ii].FirstGetValue)
                                        {
                                            m_plcDataList[jj][ii].FirstGetValue = false;
                                        }
                                    }
                                }
                                else if ((m_plcDataList[jj][ii].Value != val1 || m_plcDataList[jj][ii].FirstGetValue) &&
                                    m_plcDataList[jj][ii].MonitoringFlg/* && val1 != 0*/)
                                {
                                    if (SendMonitorMsgHandler != null && val1 != 0)
                                    {
                                        PlcEvent m_event = new PlcEvent();
                                        String ValueStr = val1.ToString();
                                        if (ValueStr.Length == 1)
                                        {
                                            ValueStr = "0" + ValueStr;
                                        }
                                        m_event.EQUIP_CODE = m_plcDataList[jj][ii].EQUIP_CODE;
                                        if (m_plcDataList[jj][ii].ACTION_ID == "null")
                                        {
                                            m_event.ACTION_ID = ValueStr;
                                        }
                                        else
                                        {
                                            m_event.ACTION_ID = m_plcDataList[jj][ii].ACTION_ID + ValueStr;
                                        }
                                        SendMonitorMsgHandler.BeginInvoke(this, m_event, null, null);
                                        if (m_plcDataList[jj][ii].FirstGetValue)
                                        {
                                            m_plcDataList[jj][ii].FirstGetValue = false;
                                        }
                                    }
                                }
                                m_plcDataList[jj][ii].Value = val1;//
                            }
                        }
                    }
                }
                int SetGonxuLianWangJiTaiSumnew = 0;
                for (int ii = 0; ii < SetGonxuLianWangJiTaiShuArr.Length; ii++)//设置工序机台链接数
                {
                    SetGonxuLianWangJiTaiSumnew += SetGonxuLianWangJiTaiShuArr[ii].lianjieshu;
                    SetGonxuLianWangJiTaiSumnew += SetGonxuLianWangJiTaiShuArr[ii].lixianshu;
                }
                if (SetGonxuLianWangJiTaiSum == SetGonxuLianWangJiTaiSumnew)
                {
                    for (int ii = 0; ii < SetGonxuLianWangJiTaiShuArr.Length; ii++)//设置工序机台链接数
                    {
                        if (/*SetGonxuLianWangJiTaiShuArr[ii].lianjieshu_old != SetGonxuLianWangJiTaiShuArr[ii].lianjieshu*/
                            CollectShare.Instance().HNC_RegGetValue((int)HncRegType.REG_TYPE_D, ii + 1, ref SetGonxuLianWangJiTaiSumnew, m_clientNo) == 0
                            && SetGonxuLianWangJiTaiSumnew != SetGonxuLianWangJiTaiShuArr[ii].lianjieshu)
                        {
                            if (CollectShare.Instance().HNC_RegSetValue((int)HncRegType.REG_TYPE_D, ii + 1, SetGonxuLianWangJiTaiShuArr[ii].lianjieshu, m_clientNo) == 0)
                            {
//                                 SetGonxuLianWangJiTaiShuArr[ii].lianjieshu_old = SetGonxuLianWangJiTaiShuArr[ii].lianjieshu;
                            }
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                SendParm.Keywords = "监控寄存器失败";
                SendParm.EventData = ex.ToString();
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
            }

            if (ret != 0)
            {
                connectStat = false;
                if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.StateChageEvenHandle != null)
                {
                    ScadaHncData.EQUIP_STATE m_EQUIP_STATE = new ScadaHncData.EQUIP_STATE();
                    m_EQUIP_STATE.EQUIP_TYPE = 2;
                    m_EQUIP_STATE.EQUIP_CODE = EQUIP_CODE;// VARCHAR2(50),设备ID
                    m_EQUIP_STATE.EQUIP_CODE_CNC = ""; // VARCHAR2(50),cnc:SN号
                    m_EQUIP_STATE.STATE_VALUE = -1; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                    SCADA.MainForm.m_CheckHander.StateChageEvenHandle.BeginInvoke(this, m_EQUIP_STATE, null, null);
                }

            }
        }

        /// <summary>
        /// PLC监控位中跟RFID相关的触发信号处理入口函数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="value"></param>
        private void RFIDHandler(int action, int value)
        {
            if (action == 0)//RFID触发
            {
                m_Hnc8PLCRFIDList[value].RFIDtriggerHandler();
            }
            else if (action == 1)//机台号接受
            {
                m_Hnc8PLCRFIDList[value].RFIDJiTaiOkHandler();
            }
        }
        #region 获取PLC当前报警和历史报警
        /**
  * 获取当前报警的报警号和报警文本
  */
        public void get_alarm_data(ref  System.Windows.Forms.DataGridView DGV)
        {
            if (connectStat)
            {
                int alarm_num = 0;
                CollectShare.Instance().HNC_AlarmRefresh(m_clientNo);
                CollectShare.Instance().HNC_AlarmGetNum((int)AlarmType.ALARM_TYPE_ALL, (int)AlarmLevel.ALARM_LEVEL_ALL, ref alarm_num, m_clientNo);
                if (alarm_num < 1)
                {
                    DGV.Rows.Clear();
                }
                else
                {
                    if (DGV.RowCount != alarm_num)
                    {
                        DGV.RowCount = alarm_num;
                    }
                }
                int alarm_ii = 0;
                string alarmText = "";
                for (int i = 0; i < alarm_num; i++)
                {
                    int DGVRowIndex = alarm_num - i - 1;
                    CollectShare.Instance().HNC_AlarmGetData((int)AlarmType.ALARM_TYPE_ALL,  //获取所有类型的报警
                                             (int)AlarmLevel.ALARM_LEVEL_ALL, //仅获取error
                                             i,
                                             ref alarm_ii,     //获取此报警的唯一ID，可用于报警识别
                                             ref alarmText,             //报警文本
                                             m_clientNo);
                    DGV.Rows[DGVRowIndex].Cells[0].Value = i;
                    DGV.Rows[DGVRowIndex].Cells[1].Value = TransAlarmNoToStr(alarm_ii);
                    DGV.Rows[DGVRowIndex].Cells[2].Value = alarmText;
                }
            }
        }

        /**
  * 获取报警历史的报警内容
  */
        private AlarmHisData[] alarmhistoryData = new AlarmHisData[HNCALARM.ALARM_HISTORY_MAX_NUM];  
        public void get_alarm_histroy_data(ref  System.Windows.Forms.DataGridView DGV)
        {
            if (connectStat)
            {
                int alarm_history_num = 0;
                CollectShare.Instance().HNC_AlarmGetHistoryNum(ref alarm_history_num, m_clientNo);
                CollectShare.Instance().HNC_AlarmGetHistoryData(0, //从第index个报警历史开始获取
                                                   ref alarm_history_num, //（传入）共获取count个报警历史, //（传出）实际获取的报警历史个数
                                                   alarmhistoryData,//历史报警内容：包括报警号、产生时间、消除时间和文本
                                               m_clientNo);
                if (alarm_history_num < 1)
                {
                    DGV.Rows.Clear();
                }
                else
                {
                    if (DGV.RowCount != alarm_history_num)
                    {
                        DGV.RowCount = alarm_history_num;
                    }
                }

                for (int i = 0; i < alarm_history_num; i++)
                {
                    int DGVRowIndex = alarm_history_num - i - 1;
                    DGV.Rows[DGVRowIndex].Cells[0].Value = i;
                    DGV.Rows[DGVRowIndex].Cells[1].Value = TransAlarmNoToStr(alarmhistoryData[i].alarmNo); ;
                    DGV.Rows[DGVRowIndex].Cells[2].Value = alarmhistoryData[i].timeBegin.year + "-" + alarmhistoryData[i].timeBegin.month
                        + "-" + alarmhistoryData[i].timeBegin.day + " " + alarmhistoryData[i].timeBegin.hour + ":" + alarmhistoryData[i].timeBegin.minute
                        + ":" + alarmhistoryData[i].timeBegin.second
                        +"---" + alarmhistoryData[i].timeEnd.year + "-" + alarmhistoryData[i].timeEnd.month
                        + "-" + alarmhistoryData[i].timeEnd.day + " " + alarmhistoryData[i].timeEnd.hour + ":" + alarmhistoryData[i].timeEnd.minute
                        + ":" + alarmhistoryData[i].timeEnd.second + 
                        ":   " + alarmhistoryData[i].text;
                }
            }
        }
        /// <summary>
        /// 将报警号转为字符串
        /// </summary>
        /// <param name="alarmNo"></param>
        /// <returns></returns>
        private String TransAlarmNoToStr(int alarmNo)
        {
            // 报警号共计9位，
            // 通道、语法：(1位报警类型)+(1位报警级别)+(3位通道号)+(4位报警内容编号)；
            // 轴、伺服 ：(1位报警类型)+(1位报警级别)+(3位轴号) +(4位报警内容编号)；
            // 其它 ：(1位报警类型)+(1位报警级别)+(7位报警内容编号)
            int type = alarmNo / 100000000;
            int level = (alarmNo % 100000000) / 10000000;
            String[] typeStr = { "系统", "通道", "轴", "伺服", "PLC", "设备", "语法", "PLC",
				"HMI" };
            String[] levelStr = { "报警", "提示" };
            String str = "";
            switch (type)
            {
                case 1:
                case 6:
                    int chNo = (alarmNo % 10000000) / 10000;
                    str = typeStr[type] + levelStr[level] + "： " + "通道" + chNo + "_"
                            + alarmNo % 10000;
                    break;
                case 2:
                case 3:
                    int axNo = (alarmNo % 10000000) / 10000;
                    str = typeStr[type] + levelStr[level] + "： " + "轴" + axNo + "_"
                            + alarmNo % 10000;
                    break;
                default:
                    str = typeStr[type] + levelStr[level] + "： " + alarmNo % 10000000;
                    break;
            }
            return str;
        }
        #endregion
    }
}
