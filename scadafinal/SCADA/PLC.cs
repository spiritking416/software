using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Collector;

namespace PLC
{
    public class PLC_MITSUBISHI_HNC8
    {
        #region 变量定义
        private MITSUBISHI.Component.DotUtlType lpcom_ReferencesCOM;//三菱PLC通信对象
        public CollectHNCPLC m_hncPLCCollector;//Hnc8PLCt通信对象指针
        public bool conneted = false;//标示PLC是否连接正常
        public static string[] DeviceLab = { "xx", "yy", "dd", "Buffer", "ss", "bb" 
                                            ,"cc","mm","ww","ll","ssmm","ff","ffdd","vv","sd"};//标签转换表
        public static short[] DeviceLabArr = { 64, 64, 100, 112, 64, 64, 64, 50, 64, 64, 10, 64, 64, 64, 64 };//各种标签的一次取数据个数
        public static String[] connetedText = { "在线", "离线" };//状态字符串表
        public int serial = 0;//序号
        public Int32 ID = 0;//ID
        public int MITSUBISHIConnetNum = 2;//三菱通信控件的连接号
        public string PassW = "";//三菱控件连接密码
        public String workshop = "#1";//
        public String productionline = "#9";
        public String type = "A";
        public String system = "hnc8";
        public String ip = "192.168.20.99";
        public UInt16 port = 10001;
        public String remark = "";
        public String EQUIP_CODE;//设备ID
        public String SN;//SN号

        public List<MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList = new List<MITSUBISHISignalType[]>();
        public List<HNC8SignalType[]> HNC8PLC_SignalList = new List<HNC8SignalType[]>();
        private bool CollectThreadRuningFlg = true;
        public delegate void EventHandler<PlcEvent>(object sender, PlcEvent Args);
        public delegate void RFIDEventHandler<PlcEvent>(object sender, MITSUBISHIPLCRFIDEvent Args);

        public static event EventHandler<PlcEvent> AutoSendMotionHandler;
        public event EventHandler<PlcEvent> AutoUpDataPLCDataHandler;
        public event RFIDEventHandler<MITSUBISHIPLCRFIDEvent> AutoUpDataRFIDDataHandler;

        public struct MITSUBISHISignalType
        {
            public MITSUBISHISignalType(bool IsShow = true)
            {
                this.Address = 0;
                this.SubAddress = -1;//地址
                this.ArrLabel = "";//描述
                this.Value = 0;//值
                this.EQUIP_CODE = "";//设备ID
                this.ACTION_ID = "-1";//动作ID
                this.MonitoringFlg = false;//信号是否监控，仅监控Value值是否变化
                this.IsShow = IsShow;
                this.FirstGetValue = true;
            }

            public Int32 Address;//地址
            public Int32 SubAddress;
            public String ArrLabel;//描述
            public short Value;//值
            public String EQUIP_CODE ;//设备ID
            public String ACTION_ID;//动作ID
            public bool MonitoringFlg;//信号是否监控，仅监控Value值是否变化
            public bool IsShow ;//界面是否显示监控值
            public bool FirstGetValue;//第一次取值
        };
        public struct MITSUBISHIWriteSignalType
        {
            public Int32 Address;//地址
            public String ArrLabel;//描述
            public short Value;//值
            public bool WriteFlg;//是否写许可
        };
        public struct HNC8SignalType
        {
            public HNC8SignalType(bool IsShow = true)
            {
                this.Address = 0;//地址
                this.SubAddress = -1;//地址
                this.ArrLabel = "";//描述
                this.Value = 0;//值
                this.EQUIP_CODE = "";//设备ID
                this.ACTION_ID = "-1";//动作ID
                this.MonitoringFlg = false;//信号是否监控，仅监控Value值是否变化
                this.IsShow = IsShow;
                this.FirstGetValue = true;
                this.RFIDListIndex = -1;
            }

            public Int32 Address;//地址
            public Int32 SubAddress;//地址
            public String ArrLabel;//描述
            public Int32 Value ;//值
            public String EQUIP_CODE;//设备ID
            public String ACTION_ID;//动作ID
            public bool MonitoringFlg ;//信号是否监控，仅监控Value值是否变化
            public bool IsShow;//界面是否显示监控值
            public bool FirstGetValue;//第一次取值
            public Int32 RFIDListIndex;//留给RFID使用
        };

        public List<RFIDinformation> m_RFIDList = new List<RFIDinformation>();
        #endregion

        public PLC_MITSUBISHI_HNC8( )
        {
            
        }

        public static int GetHncRegType(String Reg)
        {
            int ret = -1;
            if (Reg == "X")
            {
                ret = 0;
            }
            else if (Reg == "Y")
            {
                ret = 1;
            }
            else if (Reg == "F")
            {
                ret = 2;
            }
            else if (Reg == "G")
            {
                ret = 3;
            }
            else if (Reg == "R")
            {
                ret = 4;
            }
            else if (Reg == "W")
            {
                ret = 5;
            }
            else if (Reg == "D")
            {
                ret = 6;
            }
            else if (Reg == "B")
            {
                ret = 7;
            }
            else if (Reg == "P")
            {
                ret = 8;
            }
            return ret;
        }

        #region    PLC数据创建和初始化
        /// <summary>
        /// 初始化信号链表
        /// </summary>
        /// <param name="Device">寄存器类型如：D</param>
        /// <param name="AddressGeshi">地址格式：十六进制、是进制、二进制</param>
        /// <param name="StartIO">Buffer的IO号</param>
        /// <param name="InitCount">寄存器个数</param>
        public void InitMITSUBISHIPLC_Signal(String Device, Int32 AddressGeshi, String StartIO, Int32 InitCount)
        {
            if (remark.Length == 0)
            {
                MITSUBISHIConnetNum = 0;
            }
            else
            {
                MITSUBISHIConnetNum = int.Parse(remark);//remark
            }
            MITSUBISHISignalType[] PLC_SignalArr = new MITSUBISHISignalType[InitCount + 1];//比要监控的多一个，第一个表示要监控的软元件的属性
            MITSUBISHISignalType node = new MITSUBISHISignalType(true);
            node.Address = AddressGeshi;//三菱地址进制格式
            node.ACTION_ID = StartIO;//Buffer的IO号
            node.EQUIP_CODE = Device;//三菱阮元的地址表示
            SwithDevice(Device, ref node.ArrLabel, ref node.Value);
            node.MonitoringFlg = false;
            PLC_SignalArr[0] = node;
            for (int ii = 1; ii <= InitCount; ii++)
            {
                PLC_SignalArr[ii] = node;
            }
            MITSUBISHIPLC_SignalList.Add(PLC_SignalArr);
        }
        public void InitHNC8PLC_Signal(String Device, Int32 AddressGeshi, Int32 HncRegType, Int32 InitCount)
        {

            HNC8SignalType[] PLC_SignalArr = new HNC8SignalType[InitCount + 1];//比要监控的多一个，第一个表示要监控的软元件的属性
            HNC8SignalType node = new HNC8SignalType(true);
            node.ArrLabel = Device;
            node.Address = HncRegType;
            node.Value = AddressGeshi;
            PLC_SignalArr[0] = node;
            for (int ii = 1; ii <= InitCount; ii++)
            {
                PLC_SignalArr[ii] = node;
            }
            HNC8PLC_SignalList.Add(PLC_SignalArr);
        }
        public void InitRobot_Signal(String Device, Int32 InitCount, ref Int32 Star)
        {
            if (system == SCADA.m_xmlDociment.PLC_System[0])
            {
                MITSUBISHISignalType[] Resut = MITSUBISHIPLC_SignalList.Find(
                    delegate(MITSUBISHISignalType[] temp)
                    {
                        return temp[0].EQUIP_CODE == Device;
                    });
                if (Resut == null)
                {
                    Int32 AddressGeshi = Int32.Parse(SCADA.m_xmlDociment.Default_MITSUBISHI_DeviceAddress1[0].Substring(0,2));
                    InitMITSUBISHIPLC_Signal(Device, AddressGeshi, "0", InitCount);
                    Star = 0;
                }
                else
                {
                    Star = Resut.Length - 1;
                    MITSUBISHISignalType[] PLC_SignalArr = new MITSUBISHISignalType[InitCount + Resut.Length];//将原有的复制过来
                    for (int ii = 0; ii < PLC_SignalArr.Length; ii++)
                    {
                        if (ii < Resut.Length)
                        {
                            PLC_SignalArr[ii].Address = Resut[ii].Address;
                            PLC_SignalArr[ii].ArrLabel = Resut[ii].ArrLabel;
                            PLC_SignalArr[ii].Value = Resut[ii].Value;
                            PLC_SignalArr[ii].EQUIP_CODE = Resut[ii].EQUIP_CODE;
                            PLC_SignalArr[ii].ACTION_ID = Resut[ii].ACTION_ID;
                            PLC_SignalArr[ii].MonitoringFlg = Resut[ii].MonitoringFlg;
                        }
                        else
                        {
                            MITSUBISHISignalType node = new MITSUBISHISignalType(true);
                            PLC_SignalArr[ii] = node;
                        }
                    }
                    MITSUBISHIPLC_SignalList.Remove(Resut);
                    MITSUBISHIPLC_SignalList.Add(PLC_SignalArr);
                }

            }
            else if (system == SCADA.m_xmlDociment.PLC_System[1])
            {
                HNC8SignalType[] Resut = HNC8PLC_SignalList.Find(
                                delegate(HNC8SignalType[] temp)
                                {
                                    return temp[0].ArrLabel == Device;
                                });
                if (Resut == null)
                {
                    Star = 0;
                    Int32 AddressGeshi = Int32.Parse(SCADA.m_xmlDociment.Default_HNC8_DeviceAddress1[0].Substring(0, 1));
                    InitHNC8PLC_Signal(Device, AddressGeshi, GetHncRegType(Device), InitCount);
                }
                else
                {
                    Star = Resut.Length - 1;
                    HNC8SignalType[] PLC_SignalArr = new HNC8SignalType[InitCount + Resut.Length];//将原有的复制过来
                    for (int ii = 0; ii < PLC_SignalArr.Length; ii++)
                    {
                        if (ii < Resut.Length)
                        {
                            PLC_SignalArr[ii].Address = Resut[ii].Address;
                            PLC_SignalArr[ii].SubAddress = Resut[ii].SubAddress;
                            PLC_SignalArr[ii].ArrLabel = Resut[ii].ArrLabel;
                            PLC_SignalArr[ii].Value = Resut[ii].Value;
                            PLC_SignalArr[ii].EQUIP_CODE = Resut[ii].EQUIP_CODE;
                            PLC_SignalArr[ii].ACTION_ID = Resut[ii].ACTION_ID;
                            PLC_SignalArr[ii].MonitoringFlg = Resut[ii].MonitoringFlg;
                        }
                        else
                        {
                            HNC8SignalType node = new HNC8SignalType(true);
                            PLC_SignalArr[ii] = node;
                        }
                    }
                    HNC8PLC_SignalList.Remove(Resut);
                    HNC8PLC_SignalList.Add(PLC_SignalArr);
                }
            }
        }

        /// <summary>
        /// 标签转换表的使用
        /// </summary>
        /// <param name="Device">寄存器类型如：D</param>
        /// <param name="Device_out">输出转换表数组的下标</param>
        /// <param name="DeviceLabArrMun">一次取出寄存器的个数</param>
        public static void SwithDevice(String Device, ref String Device_out, ref short DeviceLabArrMun)
        {        
            int type_i = 0;
//             if (system == SCADA.MainForm.m_xml.PLC_System[0])
//             {
            if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[0])//X
                {
                    type_i = 0;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[0])//Y
                {
                    type_i = 1;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[1])//D
                {
                    type_i = 2;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[2])//Buffer
                {
                    type_i = 3;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[1])//S
                {
                    type_i = 4;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[2])//B
                {
                    type_i = 5;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[3])//C
                {
                    type_i = 6;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[3])//M
                {
                    type_i = 7;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[4])//W
                {
                    type_i = 8;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[4])//L
                {
                    type_i = 9;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[5])//SW
                {
                    type_i = 10;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[5])//F
                {
                    type_i = 11;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[6])//FD
                {
                    type_i = 12;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[6])//V
                {
                    type_i = 13;
                }
            else if (Device == SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[7])//SD
                {
                    type_i = 14;
                }
            Device_out = DeviceLab[type_i];
            DeviceLabArrMun = PLC_MITSUBISHI_HNC8.DeviceLabArr[type_i];
        }
        /// <summary>
        /// 将需要监控的数据初始化到内存数组
        /// </summary>
        /// <param name="Device"></param>
        /// <param name="Index"></param>
        /// <param name="address"></param>
        /// <param name="arrLabel"></param>
        /// <param name="value"></param>
        /// <param name="EQUIP_CODE"></param>
        /// <param name="ACTION_ID"></param>
        /// <param name="MF"></param>
        private void AddMITSUBISHIPLC_SignalNode2List(String Device, Int32 Index, String address,
            String arrLabel, short value, String EQUIP_CODE, String ACTION_ID, bool MF)
        {
            for (int ii = 0; ii < MITSUBISHIPLC_SignalList.Count; ii++)
            {
                MITSUBISHISignalType[] PLC_SignalArr = MITSUBISHIPLC_SignalList[ii];
                if (PLC_SignalArr[0].EQUIP_CODE == Device)//找到对于软元件
                {
                    MITSUBISHISignalType node = new MITSUBISHISignalType(true);
                    String[] addressArr = address.Split('.');
                    if (addressArr.Length == 2)
                    {
                        if (!Int32.TryParse(addressArr[0], out node.Address))
                        {
                            throw new Exception(system + "PLC地址格式错误：" + address);
                        }
                        if (!Int32.TryParse(addressArr[1], out node.SubAddress))
                        {
                            throw new Exception(system + "PLC地址格式错误：" + address);
                        }
                    }
                    else
                    {
                        if (!Int32.TryParse(address, out node.Address))
                        {
                            throw new Exception(system + "PLC地址格式错误：" + address);
                        }
                        node.SubAddress = -1;
                    }
                    node.ArrLabel = arrLabel;
                    node.Value = value;
                    node.EQUIP_CODE = EQUIP_CODE;
                    node.ACTION_ID = ACTION_ID;
                    node.MonitoringFlg = MF;
                    if (node.ACTION_ID == "-1")
                    {
                        node.IsShow = false;
                    }
                    else
                    {
                        node.IsShow = true;
                    }
                    PLC_SignalArr[Index + 1] = node;

                    if (node.ACTION_ID == "CNCCMD")//初始化cnc切入切出控制寄存器位置
                    {
                        for (int cncindex = 0; cncindex < SCADA.MainForm.cnclist.Count;cncindex++ )
                        {
                            if (SCADA.MainForm.cnclist[cncindex].BujianID == node.EQUIP_CODE)
                            {
                                SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1 = ii;
                                SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2 = Index + 1;
                            }
                        }
                    }
                }
            }
        }
        private void AddHNC8PLC_SignalNode2List(String Device, Int32 Index, String address,
            String arrLabel, short value, String EQUIP_CODE, String ACTION_ID, bool MF)
        {
            for (int ii = 0; ii < HNC8PLC_SignalList.Count; ii++)
            {
                HNC8SignalType[] PLC_SignalArr = HNC8PLC_SignalList[ii];
                if (PLC_SignalArr[0].ArrLabel == Device)//找到对于软元件
                {
                    HNC8SignalType node = new HNC8SignalType(true);
                    String[] addressArr = address.Split('.');
                    if (addressArr.Length == 2)
                    {
                        if (!Int32.TryParse(addressArr[0], out node.Address))
                        {
                            throw new Exception(system + "PLC地址格式错误：" + address);
                        }
                        if (!Int32.TryParse(addressArr[1], out node.SubAddress))
                        {
                            throw new Exception(system + "PLC地址格式错误：" + address);
                        }
                    }
                    else
                    {
                        if (!Int32.TryParse(address, out node.Address))
                        {
                            throw new Exception(system + "PLC地址格式错误：" + address);
                        }
                        node.SubAddress = -1;
                    }
                    node.ArrLabel = arrLabel;
                    node.Value = value;
                    node.EQUIP_CODE = EQUIP_CODE;
                    node.ACTION_ID = ACTION_ID;
                    node.MonitoringFlg = MF;
                    if (node.ACTION_ID == "-1")
                    {
                        node.IsShow = false;
                    }
                    else
                    {
                        node.IsShow = true;
                    }
                    PLC_SignalArr[Index + 1] = node;

                    if (node.ACTION_ID == "CNCCMD")//初始化cnc切入切出控制寄存器位置
                    {
                        for (int cncindex = 0; cncindex < SCADA.MainForm.cnclist.Count; cncindex++)
                        {
                            if (SCADA.MainForm.cnclist[cncindex].BujianID == node.EQUIP_CODE)
                            {
                                SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1 = ii;
                                SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2 = Index + 1;
                            }
                        }
                    }

                }
            }
        }
        public void AddSignalNode2List(String Device, Int32 Index, String address,
            String arrLabel, short value, String EQUIP_CODE, String ACTION_ID, bool MF)
        {
            if (system == SCADA.m_xmlDociment.PLC_System[0])
            {
                AddMITSUBISHIPLC_SignalNode2List(Device, Index, address,
            arrLabel, value, EQUIP_CODE, ACTION_ID, MF);
            }
            else if (system == SCADA.m_xmlDociment.PLC_System[1])
            {
                AddHNC8PLC_SignalNode2List(Device, Index, address,
            arrLabel, value, EQUIP_CODE, ACTION_ID, MF);
            }
        }

        public void AddRFIDItem2List(ref String[] Attributes_RFID, ref String[] RFIDReadDataStruct)
        {
            RFIDinformation node = new RFIDinformation(ref Attributes_RFID, ref RFIDReadDataStruct);
            node.FindReadMonitorPos_Value(ref MITSUBISHIPLC_SignalList);
            node.FindWriteMonitorPos_Value(ref MITSUBISHIPLC_SignalList);
            m_RFIDList.Add(node);

        }
        #endregion

        #region "Processing of OnDeviceStatus for DotUtlType Controle"
        /***********************************************************/
        /*  Processing of OnDeviceStatus event by DotUtlType       */
        /***********************************************************/
        private void lpcom_ReferencesCOM_OnDeviceStatus(object sender, MITSUBISHI.Component.DeviceStatusEventArgs e)
        {
//             String lpszEventMsg = "";
//             int iTextLines = 0;
//             String szData = "";

            //Assign array for the read data.
//             iTextLines = txt_Data.Lines.Length;
//             System.String[] arrData = new System.String[iTextLines + 1];
// 
//             //Copy the read data to the 'arrData'.
//             Array.Copy(txt_Data.Lines, arrData, iTextLines);
// 
//             //Add the content of new event to arrData
//             szData = String.Format("0x{0:x04}", e.lData);
//             lpszEventMsg = String.Format("OnDeviceStatus event by DotUtlType [{0}={1}({2})]", e.szDevice, e.lData, szData);
//             arrData[iTextLines] = lpszEventMsg;
// 
//             //The new 'Data' is displayed.
//             txt_Data.Lines = arrData;
// 
//             //The return code of the method is displayed by the hexadecimal.
//             txt_ReturnCode.Text = String.Format("0x{0:x8}", e.lReturnCode);

        }
        #endregion

        #region  "Processing of Open"
        private int Open(String StationNumber, String Password)
        {
            int iReturnCode;				//Return code
            int iLogicalStationNumber;		//LogicalStationNumber
           
            //
            //Processing of Open method
            //
            try
            {
                //Check the 'LogicalStationNumber'.(If succeeded, the value is gotten.)
                if (GetIntValue(StationNumber, out iLogicalStationNumber) != true)
                {
                    return -1;
                }

                //Set the value of 'LogicalStationNumber' to the property.
                lpcom_ReferencesCOM.ActLogicalStationNumber = iLogicalStationNumber;

                //Set the value of 'Password'.
                lpcom_ReferencesCOM.ActPassword = Password;

                //The Open method is executed.
                iReturnCode = this.lpcom_ReferencesCOM.Open();
                //When the Open method is succeeded, disable the TextBox of 'LogocalStationNumber'.
//                 if (iReturnCode == 0)
//                 {
// //                     txt_LogicalStationNumber.Enabled = false;
//                     conneted = true;
//                 }
            }

            //Exception processing
            catch (Exception exception)
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                SendParm.Keywords = "三菱PLC链接出错";
                SendParm.EventData = exception.ToString();
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

                conneted = false;
                return -1;
            }
            return iReturnCode;
            
            //The return code of the method is displayed by the hexadecimal.
//             txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        #endregion


        #region  "Processing of Close button"
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            CollectThreadRuningFlg = false;

            foreach (RFIDinformation RFIDnode in m_RFIDList)
            {
                RFIDnode.AppExit();
            }
            if (m_hncPLCCollector != null)
            {
                foreach (Collector.CollectHNCPLC.Hnc8PLCRFID RFIDnode in m_hncPLCCollector.m_Hnc8PLCRFIDList)
                {
                    RFIDnode.m_rfid.AppExit();
                }
            }

            if (system == SCADA.m_xmlDociment.PLC_System[0])
            {
                int iReturnCode;	//Return code
                try
                {
                    if (lpcom_ReferencesCOM != null)
                        iReturnCode = lpcom_ReferencesCOM.Close();
                }
                //Exception processing
                catch (Exception exception)
                {
                    Console.WriteLine(exception.StackTrace);
                    return;
                }
            }
        }
        #endregion


        #region  "Processing of ReadDeviceRandom2 button"
        public bool ReadDeviceRandom2(ref System.String[] arrLabel, int DeviceSizeRandom, ref short[] arrDeviceValue)
        {
            int iReturnCode;				    //Return code

            //
            //Processing of ReadDeviceRandom2 method
            //
            try
            {
                //he ReadDeviceRandom2 method is executed.
                iReturnCode = lpcom_ReferencesCOM.ReadDeviceRandom2(ref arrLabel,
                                                                DeviceSizeRandom,
                                                                ref arrDeviceValue);
            }

            //Exception processing			
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
                return false;
            }


            //
            //Display the read data
            //
            //When method is successful. Display the read data
            if (iReturnCode == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region  "Processing of GetRandom and SetRandom to big[]"
        /// <summary>
        /// 取出软元件
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="addrass"></param>
        /// <param name="SizeRandom"></param>
        /// <param name="arrDeviceValue"></param>
        /// <returns></returns>
        private bool ReadDeviceRandom2Array(ref  MITSUBISHISignalType PLC_SignalArr0, ref System.Int32 addrassStar, ref System.Int32 addrassEnd, ref short[] arrDeviceValue)
        {
            Int32 iReturnCode = -1;				    //Return code
            addrassStar = addrassStar - addrassStar % PLC_SignalArr0.Value;
            addrassEnd = addrassStar + PLC_SignalArr0.Value;
            String addrassStr = "xx0";
            try
            {
                if (PLC_SignalArr0.ArrLabel == DeviceLab[3])//Buffer
                {
                    iReturnCode = lpcom_ReferencesCOM.ReadBuffer(int.Parse(PLC_SignalArr0.ACTION_ID), addrassStar, PLC_SignalArr0.Value, ref arrDeviceValue);
                }
                else if (PLC_SignalArr0.ArrLabel == DeviceLab[0] || PLC_SignalArr0.ArrLabel == DeviceLab[1]
                    || PLC_SignalArr0.ArrLabel == DeviceLab[7] || PLC_SignalArr0.ArrLabel == DeviceLab[10])//X、Y、M、SM
                {
                    addrassStr = "xx0";
                    if (PLC_SignalArr0.Address == 10)//十进制
                    {
                        addrassStr = PLC_SignalArr0.ArrLabel + addrassStar.ToString();
                    }
                    else if (PLC_SignalArr0.Address == 16)
                    {
                        addrassStr = PLC_SignalArr0.ArrLabel + String.Format("{0:X}", addrassStar);
                    }

                    //                     String addrassStr = PLC_SignalArr0.ArrLabel + String.Format("{0:X}", addrassStar);
                    iReturnCode = lpcom_ReferencesCOM.ReadDeviceRandom2(ref addrassStr, PLC_SignalArr0.Value, ref arrDeviceValue);
                }
                //                 else if ()//
                //                 {
                //                     String addrassStr = PLC_SignalArr0.ArrLabel + addrassStar.ToString();
                //                     iReturnCode = lpcom_ReferencesCOM.ReadDeviceRandom2(ref addrassStr, PLC_SignalArr0.Value, ref arrDeviceValue);
                //                 }
                else
                {
                    addrassStr = "xx0";
                    if (PLC_SignalArr0.Address == 10)//十进制
                    {
                        addrassStr = PLC_SignalArr0.ArrLabel + addrassStar.ToString();
                    }
                    else if (PLC_SignalArr0.Address == 16)
                    {
                        addrassStr = PLC_SignalArr0.ArrLabel + String.Format("{0:X}", addrassStar);
                    }
                    iReturnCode = lpcom_ReferencesCOM.ReadDeviceBlock2(ref addrassStr, PLC_SignalArr0.Value, ref arrDeviceValue);
                }
            }
            catch (Exception exception)
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                SendParm.Keywords = "获取三菱PLC数据异常";
                SendParm.EventData = exception.ToString();
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                return false;
            }
            if (iReturnCode == 0)
            {
                return true;
            }
            else
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                SendParm.Keywords = "获取三菱PLC数据出错";
                SendParm.EventData = addrassStr;
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                return false;
            }
        }
        private bool SetDeviceRandom2Array(ref  MITSUBISHISignalType PLC_SignalArr0, ref System.Int32 addrassStar, ref System.Int32 addrassLenth, ref short[] arrDeviceValue)
        {
            Int32 iReturnCode = -1;				    //Return code
            String addrassStr = "xx0";
            try
            {
                if (PLC_SignalArr0.ArrLabel == DeviceLab[3])//Buffer
                {
                    iReturnCode = lpcom_ReferencesCOM.WriteBuffer(int.Parse(PLC_SignalArr0.ACTION_ID), addrassStar, addrassLenth, arrDeviceValue);
                }
                else //if (PLC_SignalArr0.ArrLabel == DeviceLab[0] || PLC_SignalArr0.ArrLabel == DeviceLab[1] || PLC_SignalArr0.ArrLabel == DeviceLab[7])//X、Y、M
                {
                    addrassStr = "xx0";
                    if (PLC_SignalArr0.Address == 10)//十进制
                    {
                        addrassStr = PLC_SignalArr0.ArrLabel + addrassStar.ToString();
                    }
                    else if (PLC_SignalArr0.Address == 16)
                    {
                        addrassStr = PLC_SignalArr0.ArrLabel + String.Format("{0:X}", addrassStar);
                    }
                    iReturnCode = lpcom_ReferencesCOM.WriteDeviceRandom2(ref addrassStr, addrassLenth, ref arrDeviceValue);
                }
//                 else
//                 {
//                     addrassStr = "xx0";
//                     if (PLC_SignalArr0.Address == 10)//十进制
//                     {
//                         addrassStr = PLC_SignalArr0.ArrLabel + addrassStar.ToString();
//                     }
//                     else if (PLC_SignalArr0.Address == 16)
//                     {
//                         addrassStr = PLC_SignalArr0.ArrLabel + String.Format("{0:X}", addrassStar);
//                     }
//                     iReturnCode = lpcom_ReferencesCOM.WriteDeviceBlock2(ref addrassStr, addrassLenth, arrDeviceValue);
//                 }
            }
            catch (Exception exception)
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                SendParm.Keywords = "设置三菱PLC数据异常";
                SendParm.EventData = exception.ToString();
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                return false;
            }
            if (iReturnCode == 0)
            {
                return true;
            }
            else
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                SendParm.Keywords = "设置三菱PLC数据出错";
                SendParm.EventData = addrassStr;
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                return false;
            }
        }
        #endregion

        #region 发送PLC监控数据
        public enum SendAutoSendHandler_type
        {
            AutoSendMotionHandler = 0,
            AutoUpDataPLCDataHandler,
            AutoUpDataRFIDDataHandler
        }
        public static void SendAutoSendMotionHandler(object obj, PlcEvent sendobj)
        {
            if (AutoSendMotionHandler != null)
            {
                AutoSendMotionHandler.BeginInvoke(obj, sendobj, null, null);
            }
        }
        public void SendAutoSendHandler(SendAutoSendHandler_type type, object sendobj)
        {
            switch (type)
            {
                case SendAutoSendHandler_type.AutoSendMotionHandler:
                    if (AutoSendMotionHandler != null)
                    {
                        AutoSendMotionHandler.BeginInvoke(this, (PlcEvent)sendobj, null, null);
                    }
                    break;
                case SendAutoSendHandler_type.AutoUpDataPLCDataHandler:
                    if (AutoUpDataPLCDataHandler != null)
                    {
                        AutoUpDataPLCDataHandler.BeginInvoke(this, (PlcEvent)sendobj, null, null);
                    }
                    break;
                case SendAutoSendHandler_type.AutoUpDataRFIDDataHandler:
                    if (AutoUpDataRFIDDataHandler != null)
                    {
                        AutoUpDataRFIDDataHandler.BeginInvoke(this, (MITSUBISHIPLCRFIDEvent)sendobj, null, null);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region PLC数据获取刷新
        /// <summary>
        /// 三菱PLC更新数据
        /// </summary>
        public bool UpDataMITSUBISHIPLC_SignalList(ref List<MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList)
        {
            MITSUBISHISignalType [] PLC_SignalArr;
            for (int jj = 0; jj < MITSUBISHIPLC_SignalList.Count; jj++)
            {
                PLC_SignalArr = MITSUBISHIPLC_SignalList[jj];
                Int32 addarStar = 0;
                Int32 addarEnd = 0;
                short[] value = new short[PLC_SignalArr[0].Value];
                short value1 = 0;
                Int32 nim = -1;
                for (int ii = 1; ii < PLC_SignalArr.Length; ii++)
                {
                    if (PLC_SignalArr[ii].MonitoringFlg || PLC_SignalArr[ii].IsShow)//界面显示只有看得见的值刷新
                    {
                        addarStar = PLC_SignalArr[ii].Address;
                        if (addarStar < nim || addarStar >= addarEnd)//不在上次已经取得的数据范围内,取出数据数组
                        {
                            if (ReadDeviceRandom2Array(ref PLC_SignalArr[0], ref addarStar, ref addarEnd, ref value))
                            {
                                nim = addarStar;
                            }
                            else
                            {
                                return false;//网络故障
                            }
                        }
                        if (PLC_SignalArr[ii].SubAddress != -1)//监控位
                        {
                            value1 = value[PLC_SignalArr[ii].Address - nim];
                            value1 = short.Parse((value1 & (1 << PLC_SignalArr[ii].SubAddress)).ToString());
                            value1 >>= PLC_SignalArr[ii].SubAddress;
                        }
                        else
                        {
                            value1 = value[PLC_SignalArr[ii].Address - nim];
                        }

                        if (PLC_SignalArr[ii].ACTION_ID == "PLCALARM" &&
                            (PLC_SignalArr[ii].FirstGetValue || PLC_SignalArr[ii].Value != value1))//PLC监控的设备报警
                        {
                            if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                            {
                                SCADA.EquipmentCheck.AlarmSendData SendMeg = new SCADA.EquipmentCheck.AlarmSendData();
//                                 SendMeg.alardat = new ScadaHncData.AlarmData();
                                SendMeg.NeedFindTeX = true;
                                SendMeg.BujianID = PLC_SignalArr[ii].EQUIP_CODE;
                                SendMeg.alardat.alarmNo = value1;
//                                 SendMeg.alardat.time = DateTime.Now;
                                SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                                if (PLC_SignalArr[ii].FirstGetValue)
                                {
                                    PLC_SignalArr[ii].FirstGetValue = false;
                                }
                            }
                        }
                        else if (PLC_SignalArr[ii].MonitoringFlg &&
                            (PLC_SignalArr[ii].Value != value1 || PLC_SignalArr[ii].FirstGetValue) &&
                            PLC_SignalArr[ii].EQUIP_CODE != "null")
                        {
                            PlcEvent SendData = new PlcEvent();
                            if (PLC_SignalArr[ii].ACTION_ID == "RGV")//RGV动作上报
                            {
                                SendData.Type = PLC_SignalArr[ii].ACTION_ID;
                                SendData.ACTION_ID = PLC_SignalArr[ii - 3].Value.ToString();
                                Int32 Speed = PLC_SignalArr[ii - 1].Value << 16;
                                Speed += PLC_SignalArr[ii - 2].Value;
                                SendData.ArrLabel = Speed.ToString();
                                SendData.Value = value1;
                                SendData.EQUIP_CODE = PLC_SignalArr[ii].EQUIP_CODE;
                            }
                            else if (value1 > 0)
                            {
                                String ValueStr = value1.ToString();
                                if (ValueStr.Length == 1)
                                {
                                    ValueStr = "0" + ValueStr;
                                }
                                SendData.EQUIP_CODE = PLC_SignalArr[ii].EQUIP_CODE;
                                if(PLC_SignalArr[ii].ACTION_ID == "null")
                                {
                                    SendData.ACTION_ID = ValueStr;
                                }
                                else
                                {
                                    SendData.ACTION_ID = PLC_SignalArr[ii].ACTION_ID + ValueStr;
                                }
                            }
                            SendAutoSendHandler(SendAutoSendHandler_type.AutoSendMotionHandler, SendData);
                            if (PLC_SignalArr[ii].FirstGetValue)
                            {
                                PLC_SignalArr[ii].FirstGetValue = false;
                            }
                        }
                        PLC_SignalArr[ii].Value = value1;
                    }
                }
            }
            return true;//成功返回
        }

        #region "Processing of getting 32bit integer from TextBox"
        private bool GetIntValue(String Value_str, out int iGottenIntValue)
        {
            iGottenIntValue = 0;
            //Get the value as 32bit integer from a TextBox
            try
            {
                iGottenIntValue = Convert.ToInt32(Value_str);
            }

            //When the value is nothing or out of the range, the exception is processed.
            catch (Exception exExcepion)
            {
                //                 MessageBox.Show(exExcepion.Message,
                //                                   Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exExcepion.StackTrace);
                return false;
            }

            //Normal End
            return true;
        }
        #endregion

        /// <summary>
        /// 开始收集线程
        /// </summary>
        Thread CollectThread;
        public void StarCollectThread()
        {
            if (system == SCADA.m_xmlDociment.PLC_System[0])//三菱PLC
            {
                /* Create instance for ACT Controls*************************************/
                lpcom_ReferencesCOM = new MITSUBISHI.Component.DotUtlType();

                /* Set EventHandler for ACT Controls************************************/
                lpcom_ReferencesCOM.OnDeviceStatus +=
                    new MITSUBISHI.Component.DotUtlType.DeviceStatusEventHandler(lpcom_ReferencesCOM_OnDeviceStatus);
                /**************************************************************************/  

                ///
                for (int ii = 0; ii < m_RFIDList.Count;ii++ )
                {
                    m_RFIDList[ii].lpcom_ReferencesCOM = lpcom_ReferencesCOM;
                }
            }
            else if (system == SCADA.m_xmlDociment.PLC_System[1])//华中8型PLC
            {
                m_hncPLCCollector = new CollectHNCPLC(EQUIP_CODE, ip, port, HNC8PLC_SignalList);
                ///初始化RFID
                List<String> GongxuList = new List<String>();
                string get_str = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, -1, SCADA.m_xmlDociment.Default_Attributes_str1[(int)SCADA.m_xmlDociment.Attributes_RFID.serial]);
                for (int ii = 0; ii < int.Parse(get_str); ii++)
                {
                    int rfidplcii = int.Parse(SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.PLCserial]));
                    if (rfidplcii == serial)
                    {
                        String RegStr = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ReadDevice]);
                        RegStr +=  "," + SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ReadAddressStar]);
                        RegStr +=  "," + SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.WriteAddressStar]);
                        String IPAddress = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ip]);
                        String Port = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.port]);
                        String GongXu_PAdress = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.MonitorBit]);
                        String ReadOrWriteFlag = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.WriteDevice]);
                        String RFIDDataGeShiStr = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ReadAddressSet]);
                        String jitaihao = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.jitaihao]);
                        String ID = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.id]);
                        String Scanning_EQUIP_CODEstr = SCADA.MainForm.m_xml.m_Read(SCADA.m_xmlDociment.PathRoot_RFID, ii, SCADA.m_xmlDociment.Default_Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.remark]);
                        int GongXu_II = 0;
                        int rfidcncindex = -1;
                        for (int cncindex = 0; cncindex < SCADA.MainForm.cnclist.Count; cncindex++)
                        {
                            if (SCADA.MainForm.cnclist[cncindex].JiTaiHao == jitaihao)
                            {
                                GongXu_II = SCADA.MainForm.cnclist[cncindex].GetOP_CODE_Index();
                                rfidcncindex = cncindex;
                                break;
                            }
                        }
                        if (!m_hncPLCCollector.AddRFID(ref RegStr, ref IPAddress, ref Port, ref GongXu_II, ref ReadOrWriteFlag,
                            ref RFIDDataGeShiStr, ref ii, ref ID,ref Scanning_EQUIP_CODEstr,rfidcncindex))
                        {
                            throw new Exception("AddRFID Errer!");
                        }
                        if (ReadOrWriteFlag == "Read")//上料带需要工序绑定
                        {
                            GongXu_PAdress += "=" + GongXu_II.ToString();
                            GongxuList.Add(GongXu_PAdress);
                        }
                        else if (ReadOrWriteFlag == "LineEnd")//下料带线尾
                        {
                            GongxuList.Add(GongXu_PAdress);
                        }
                    }
                }
                ///初始化工艺参数
                m_hncPLCCollector.SetGongXuParam(ref GongxuList);
                m_hncPLCCollector.ConnectPLC();
            }

            CollectThread = new Thread(new ParameterizedThreadStart(CollectThreadRuning));
            CollectThread.Start();
        }

        public void StopStarCollectThread()
        {
            CollectThreadRuningFlg = false;
            if (system == SCADA.m_xmlDociment.PLC_System[0])//三菱PLC
            {
                lpcom_ReferencesCOM.Close();
            }
            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
            SendParm.Keywords = "PLC停止数据采集";
            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

//             else if (system == SCADA.m_xmlDociment.PLC_System[1])//华中8型PLC
//             {
//                 m_hncPLCCollector.StopCollect();
//             }
        }

        /// <summary>
        /// 收集线程
        /// </summary>
        /// <param name="obj"></param>
        private void CollectThreadRuning(object obj)
        {
            if (system == SCADA.m_xmlDociment.PLC_System[0])//三菱PLC
            {
                while (CollectThreadRuningFlg)
                {
                    if(!conneted)
                    {
                        if (0 == Open(MITSUBISHIConnetNum.ToString(), PassW))
                        {
                            for (int jj = 0; jj < MITSUBISHIPLC_SignalList.Count; jj++)//初始化第一次取值标志
                            {
                                 for (int ii = 1; ii < MITSUBISHIPLC_SignalList[jj].Length; ii++)
                                {
                                    MITSUBISHIPLC_SignalList[jj][ii].FirstGetValue = true;
                                }
                            }
                            PlcEvent SendData = new PlcEvent();
                            SendData.Type = "State";
                            SendData.ArrLabel = connetedText[0];
                            SendData.Value = 1;//连接上为1
                            SendAutoSendHandler(SendAutoSendHandler_type.AutoUpDataPLCDataHandler, SendData);
                            MITSUBISHIPLCRFIDEvent SendData2 = new MITSUBISHIPLCRFIDEvent();
                            SendData2.plcserial = -1;
                            SendAutoSendHandler(SendAutoSendHandler_type.AutoUpDataRFIDDataHandler, SendData2);


                            if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.StateChageEvenHandle != null)
                            {
                                ScadaHncData.EQUIP_STATE m_EQUIP_STATE = new ScadaHncData.EQUIP_STATE();
                                m_EQUIP_STATE.EQUIP_TYPE = 2;
                                m_EQUIP_STATE.EQUIP_CODE = EQUIP_CODE;// VARCHAR2(50),设备ID
                                m_EQUIP_STATE.EQUIP_CODE_CNC = SN; // VARCHAR2(50),cnc:SN号
                                m_EQUIP_STATE.STATE_VALUE = 1; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                                SCADA.MainForm.m_CheckHander.StateChageEvenHandle.BeginInvoke(this, m_EQUIP_STATE, null, null);
                            }

                            conneted = true;
                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "开启三菱PLC采集器成功";
                            SendParm.EventData = "链接号=" + MITSUBISHIConnetNum.ToString();
                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                        else
                        {
                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "开启三菱PLC采集器失败";
                            SendParm.EventData = "链接号=" + MITSUBISHIConnetNum.ToString();
                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                    }
                    else
                    {
//                         PlcEvent SendDataasddf = new PlcEvent();
//                         SendDataasddf.ACTION_ID = "1111111";
//                         SendDataasddf.EQUIP_CODE = "Test";
//                         SendAutoSendHandler(SendAutoSendHandler_type.AutoSendMotionHandler, SendDataasddf);
//                         SendDataasddf = new PlcEvent();
//                         SendDataasddf.ACTION_ID = "2222222";
//                         SendDataasddf.EQUIP_CODE = "Test";
//                         SendAutoSendHandler(SendAutoSendHandler_type.AutoSendMotionHandler, SendDataasddf);

                        if (UpDataMITSUBISHIPLC_SignalList(ref MITSUBISHIPLC_SignalList))
                        {
                            //////CNC切入切出控制接口
                            for (int cncindex = 0; cncindex < SCADA.MainForm.cnclist.Count; cncindex++)
                            {
                                if (SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1 != -1)
                                {
                                    if (SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SetValue != 
                                        MITSUBISHIPLC_SignalList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].Value)
                                    {
                                        int WriteLenth = 1;
                                        short[] valuearr = new short[WriteLenth];
                                        valuearr[0] = short.Parse(SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SetValue.ToString());
                                        SetDeviceRandom2Array(
                                            ref MITSUBISHIPLC_SignalList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][0],
                                            ref MITSUBISHIPLC_SignalList[SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList1][SCADA.MainForm.cnclist[cncindex].SetCNCCMDRe.SignalList2].Address,
                                            ref WriteLenth, ref valuearr);
                                    }
                                }
                            }

                            ///RFID读信息刷新
                            for (int ii = 0; ii < m_RFIDList.Count; ii++)
                            {
                                if (m_RFIDList[ii].IsReadRFIDFlg(ref MITSUBISHIPLC_SignalList))
                                {
                                     if (UpDataMITSUBISHIPLC_SignalList(ref m_RFIDList[ii].MITSUBISHIPLC_RFIDReadList))
                                    {
                                        m_RFIDList[ii].MassgeHandlerFucRun("0");
//                                         if (AutoSendMotionHandler != null && m_RFIDList[ii].Scanning_EQUIP_CODE.Length >= 12
//                                             && m_RFIDList[ii].NodeStrArr != null)
//                                         {
//                                             PlcEvent SendData = new PlcEvent();
//                                             SendData.EQUIP_CODE = m_RFIDList[ii].NodeStrArr[0];
//                                             SendData.ACTION_ID = m_RFIDList[ii].Scanning_EQUIP_CODE;
//                                             SendAutoSendHandler(SendAutoSendHandler_type.AutoSendMotionHandler, SendData);
//                                         }
                                    }
                                }
                                else if (m_RFIDList[ii].RFIDType && m_RFIDList[ii].IsWriteRFIDFlg(ref MITSUBISHIPLC_SignalList))
                                {
                                    if (UpDataMITSUBISHIPLC_SignalList(ref m_RFIDList[ii].MITSUBISHIPLC_RFIDWriteList))
                                    {
                                        m_RFIDList[ii].MassgeHandlerFucRun("1");
                                    }
                                }
                            }
                            Thread.Sleep(50);
                        }
                        else
                        {
                            if (CollectThreadRuningFlg)
                            {
                                PlcEvent SendData11 = new PlcEvent();
                                SendData11.Type = "State";
                                SendData11.ArrLabel = connetedText[1];
                                SendData11.Value = 0;//掉线为0
                                SendAutoSendHandler(SendAutoSendHandler_type.AutoUpDataPLCDataHandler, SendData11);
                                conneted = false;
                                if (lpcom_ReferencesCOM != null)
                                {
                                    lpcom_ReferencesCOM.Close();
                                }
                                MITSUBISHIPLCRFIDEvent SendData1 = new MITSUBISHIPLCRFIDEvent();
                                SendData1.plcserial = -1;
                                SendAutoSendHandler(SendAutoSendHandler_type.AutoUpDataRFIDDataHandler, SendData1);

                                if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.StateChageEvenHandle != null)
                                {
                                    ScadaHncData.EQUIP_STATE m_EQUIP_STATE = new ScadaHncData.EQUIP_STATE();
                                    m_EQUIP_STATE.EQUIP_TYPE = 2;
                                    m_EQUIP_STATE.EQUIP_CODE = EQUIP_CODE;// VARCHAR2(50),设备ID
                                    m_EQUIP_STATE.EQUIP_CODE_CNC = SN; // VARCHAR2(50),cnc:SN号
                                    m_EQUIP_STATE.STATE_VALUE = -1; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                                    SCADA.MainForm.m_CheckHander.StateChageEvenHandle.BeginInvoke(this, m_EQUIP_STATE, null, null);
                                }

                            }
                            Thread.Sleep(2000);
                        }
                    }
                }
            }
            else if (system == SCADA.m_xmlDociment.PLC_System[1])//华中8型PLC
            {
                while (CollectThreadRuningFlg)
                {
                    if (m_hncPLCCollector.connectStat != conneted)
                    {
                       conneted =  m_hncPLCCollector.connectStat;
                    }
                    m_hncPLCCollector.Collect();
                    Thread.Sleep(10);
                }
            }
        }
        #endregion

        #region  "Processing of MITSUBISHIReadBuffer2RFIDinformation"
        private bool MITSUBISHIReadBuffer2RFIDinformation(int iStartIO, int iAddress, ref RFIDinformation m_RFIDinformation)
        {
            try
            {
                short[] RFIDinformationShort = new short[]{};
                m_RFIDinformation.GetRFIDinformationShort(ref RFIDinformationShort);
                if (lpcom_ReferencesCOM.ReadBuffer(iStartIO, iAddress, m_RFIDinformation.RFIDinformationShortLenth, ref RFIDinformationShort)
                    == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        #endregion

        #region "Processing of RFIDinformationWriteBuffer2MITSUBISHI"
        private bool RFIDinformationWriteBuffer2MITSUBISHI(int iStartIO, int iAddress, ref RFIDinformation m_RFIDinformation)
        {
            try
            {
                short[] RFIDinformationShort = new short[] { };
                m_RFIDinformation.GetRFIDinformationShort(ref RFIDinformationShort);
                if (lpcom_ReferencesCOM.WriteBuffer(iStartIO, iAddress, m_RFIDinformation.RFIDinformationShortLenth, RFIDinformationShort)
                    == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        #endregion

    }


    public class PlcEvent
    {
        public string Type{ get; set; }//类型
        public string Address{ get; set; }//地址
        public string ArrLabel{ get; set; }//描述
        public short Value{ get; set; }//值
        public String EQUIP_CODE;//设备ID
        public String ACTION_ID;//设备ID
    }


    public class MITSUBISHIPLCRFIDEvent
    {
        public int plcserial { get; set; }//plc序号
        public int RFIDserial { get; set; }//RFID序号
        public string Event { get; set; }//事件描述
    }
    public class HNC8PLCRFIDEvent
    {
        public int plcserial { get; set; }//plc序号
        public int RFIDserial { get; set; }//RFID序号
        public int EventType { get; set; }//事件类型
        public string Event { get; set; }//事件描述
    }
     
    public class RFIDinformation
    {
        #region 变量定义
        public MITSUBISHI.Component.DotUtlType lpcom_ReferencesCOM;//三菱PLC通信对象
        public int RFIDinformationShortLenth { get; set; }
        private short[] RFIDinformationShort;//112 BYTE,小料盘编码
        private int[] NodeLengthArr;//各个节点字节数
        public String[] NodeStrArr;//各个节字符串
        public bool RFIDType = true;//实际有读写器对应的为真，佳裕给的加工信息为假
        public int cncindex;//佳裕给的加工信息对应机台
        public String Scanning_EQUIP_CODE = "";

        private int RFIDReadinformationShortLenth;
        private short[] RFIDReadinformationShort;
        private int[] ReadNodeLengthArr;
        private String[] ReadNodeStrArr;
        private int RFIDWriteinformationShortLenth;
        private short[] RFIDWriteinformationShort;
        private int[] WriteNodeLengthArr;
        private String[] WriteNodeStrArr;

        private String[] Attributes_RFID;
        public int RFIDSerial = 0;
        public String ReadDevice;
        public String EQUIP_CODE;//设备ID
        public int Gongxu;//收到的加工工序
        public int RFIDDataDataTableDataMax = 500;
        public DataTable RFIDReadDataDataTable = new DataTable("2");
        public Object RFIDReadDataDataTable_Look = new Object();

        public List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_RFIDReadList = new List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]>();
        public List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_RFIDWriteList = new List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]>();
        public PLC_MITSUBISHI_HNC8.MITSUBISHIWriteSignalType MITSUBISHIPLC_RFIDReadMonitor = new PLC_MITSUBISHI_HNC8.MITSUBISHIWriteSignalType();
        public PLC_MITSUBISHI_HNC8.MITSUBISHIWriteSignalType MITSUBISHIPLC_RFIDWriteMonitor = new PLC_MITSUBISHI_HNC8.MITSUBISHIWriteSignalType();

        public int ReadMonitorBitAddress1 = -1;
        public int ReadMonitorBitAddress2 = -1;
        public int WriteMonitorBitAddress1 = -1;
        public int WriteMonitorBitAddress2 = -1;

        private String RFIDDataDataTable_FileName;
        private event EventHandler<String> MassgeHandler;
        #endregion
        /// <summary>
        /// 可以根据NodeLengthArr来定义数据节点
        /// </summary>
        /// <param name="NodeLengthArr"></param>
        public RFIDinformation(ref String[] Attributes_RFID, ref String[] RFIDReadDataStruct)
        {
            MassgeHandler = new EventHandler<String>(this.MassgeHandlerFuc);
            if (Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.remark].Length == 3)
            {
                for(int ii = 0;ii < SCADA.MainForm.cnclist.Count;ii++)
                {
                    if (SCADA.MainForm.cnclist[ii].JiTaiHao
                        == Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.remark])
                    {
                        cncindex = ii;
                    }
                }
                RFIDType = false;
            }
            else if (Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.remark].Length >= 12)
            {
                Scanning_EQUIP_CODE = Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.remark];
            }

            this.Attributes_RFID = (String[])Attributes_RFID.Clone();
            EQUIP_CODE = Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.id];
            RFIDSerial = int.Parse(Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.serial]);

            String[] Attributes_RFID_AddressSet = Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ReadAddressSet].Split(',');
            ReadNodeLengthArr = new int[Attributes_RFID_AddressSet.Length];
            for (int ii = 0; ii < Attributes_RFID_AddressSet.Length; ii++)//读入地址格式
            {
                ReadNodeLengthArr[ii] = int.Parse(Attributes_RFID_AddressSet[ii]);
            }

            Attributes_RFID_AddressSet = Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.WriteAddressSet].Split(',');
            WriteNodeLengthArr = new int[Attributes_RFID_AddressSet.Length];
            for (int ii = 0; ii < Attributes_RFID_AddressSet.Length; ii++)//写入地址格式
            {
                WriteNodeLengthArr[ii] = int.Parse(Attributes_RFID_AddressSet[ii]);
            }

            String[] Pathstr = SCADA.MainForm.RFIDDataFilePath.Split('\\');
            RFIDDataDataTable_FileName = SCADA.MainForm.RFIDDataFilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            RFIDDataDataTable_FileName = RFIDDataDataTable_FileName.Substring(0, RFIDDataDataTable_FileName.Length - 1);
            if (!System.IO.Directory.Exists(RFIDDataDataTable_FileName))
            {
                System.IO.Directory.CreateDirectory(RFIDDataDataTable_FileName);
            }
            RFIDDataDataTable_FileName += "\\" + Pathstr[Pathstr.Length - 1] + RFIDSerial.ToString();
            RFIDDataDataTable_XMLFile_load(RFIDDataDataTable_FileName, ref RFIDReadDataStruct);

            ReadNodeStrArr = new String[ReadNodeLengthArr.Length];
            for (int ii = 0; ii < ReadNodeLengthArr.Length; ii++)
            {
                RFIDReadinformationShortLenth += ReadNodeLengthArr[ii];
            }
            RFIDReadinformationShort = new short[RFIDReadinformationShortLenth];

            WriteNodeStrArr = new String[WriteNodeLengthArr.Length];
            for (int ii = 0; ii < WriteNodeLengthArr.Length; ii++)
            {
                RFIDWriteinformationShortLenth += WriteNodeLengthArr[ii];
            }
            RFIDWriteinformationShort = new short[RFIDWriteinformationShortLenth];

            InitMITSUBISHIPLC_RFIDList(ref MITSUBISHIPLC_RFIDReadList,
                Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ReadDevice], 0, RFIDReadinformationShortLenth);//初始化读地址列表
            InitMITSUBISHIPLC_RFIDList(ref MITSUBISHIPLC_RFIDWriteList,
                Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.WriteDevice], 0, RFIDWriteinformationShortLenth);//初始化写地址列表
        }

        #region RFID读触发
        private bool GoInAready = false;
        private void MassgeHandlerFuc(object sender,String str)
        {
            if (!GoInAready)
            {
                GoInAready = true;
                if (str == "0")
                {
                    SetMITSUBISHIData(ref MITSUBISHIPLC_RFIDReadList, true);

                    if (SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler != null)
                    {
                        SCADA.RFIDSTRMassgeStr m_massg = new SCADA.RFIDSTRMassgeStr(0,0,NodeStrArr,null,EQUIP_CODE);
                        SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler.BeginInvoke(this, m_massg, null, null);
                    }
                    

//                     SCADA.MainForm.m_ShowRfidDataTable.ReadRfid(ref NodeStrArr, ref EQUIP_CODE);
                    if (Scanning_EQUIP_CODE.Length >= 12
                            && NodeStrArr != null)
                    {
                        PlcEvent SendData = new PlcEvent();
                        SendData.EQUIP_CODE = NodeStrArr[0];
                        SendData.ACTION_ID = Scanning_EQUIP_CODE;
                        PLC_MITSUBISHI_HNC8.SendAutoSendMotionHandler(this, SendData);
                    }

                }
                else
                {
                    SetMITSUBISHIData(ref MITSUBISHIPLC_RFIDWriteList, false);
                    if (SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler != null)
                    {
                        SCADA.RFIDSTRMassgeStr m_massg = new SCADA.RFIDSTRMassgeStr(2, 0, NodeStrArr, Gongxu.ToString(), EQUIP_CODE);
                        SCADA.MainForm.m_ShowRfidDataTable.GetMassgeHandler.BeginInvoke(this, m_massg, null, null);
                    }

//                     SCADA.MainForm.m_ShowRfidDataTable.WriteRfid(ref NodeStrArr, ref EQUIP_CODE, Gongxu);
                }
                UpDataReadData2Table();
                MITSUBISHIPLCRFIDEvent SendData1 = new MITSUBISHIPLCRFIDEvent();
                SendData1.plcserial = 0;
                SendData1.RFIDserial = RFIDSerial;
                SendData1.Event = "读到信息";
                SCADA.PLCDataShare.m_plclist[0].SendAutoSendHandler(PLC_MITSUBISHI_HNC8.SendAutoSendHandler_type.AutoUpDataRFIDDataHandler, SendData1);
                GoInAready = false;
            }
        }

        public void MassgeHandlerFucRun(String str)
        {
            if (str == "0")
            {
                ReSetRFIDFlg(ref MITSUBISHIPLC_RFIDReadMonitor);
            }
            else
            {
                ReSetRFIDFlg(ref MITSUBISHIPLC_RFIDWriteMonitor);
            }
            MassgeHandler.BeginInvoke(this,str,null,null);
        }
        #endregion

        /// <summary>
        /// 初始化RFID监控寄存器表
        /// </summary>
        /// <param name="Device"></param>
        /// <param name="StartIO"></param>
        /// <param name="InitCount"></param>
        private void InitMITSUBISHIPLC_RFIDList(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHISignalList, String Device, short StartIO, Int32 InitCount)
        {
            PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[] PLC_SignalArr = new PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[InitCount + 1];//比要监控的多一个，第一个表示要监控的软元件的属性
            PLC_MITSUBISHI_HNC8.MITSUBISHISignalType node = new PLC_MITSUBISHI_HNC8.MITSUBISHISignalType(true);

            Int32 AddressGeshi = -1;
            for (int ii = 1; ii <= SCADA.m_xmlDociment.Default_MITSUBISHI_Device1.Length; ii++)
            {
                if (SCADA.m_xmlDociment.Default_MITSUBISHI_Device1[ii] == Device)
                {
                    AddressGeshi = Int32.Parse(SCADA.m_xmlDociment.Default_MITSUBISHI_DeviceAddress1[ii].Split('-')[0]);
                    break;
                }
            }
            if (AddressGeshi == -1)
            {
                for (int ii = 1; ii <= SCADA.m_xmlDociment.Default_MITSUBISHI_Device2.Length; ii++)
                {
                    if (SCADA.m_xmlDociment.Default_MITSUBISHI_Device2[ii] == Device)
                    {
                        AddressGeshi = Int32.Parse(SCADA.m_xmlDociment.Default_MITSUBISHI_DeviceAddress2[ii].Split('-')[0]);
                        break;
                    }
                }
            }
            
            node.Address = AddressGeshi;
            node.ACTION_ID = StartIO.ToString();
            node.EQUIP_CODE = Device;
            PLC_MITSUBISHI_HNC8.SwithDevice(Device, ref node.ArrLabel, ref node.Value);
            node.MonitoringFlg = false;
            PLC_SignalArr[0] = node;

            int StarAdress ;
            if (MITSUBISHISignalList == MITSUBISHIPLC_RFIDReadList)
            {
                StarAdress = int.Parse(Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ReadAddressStar]);
            }
            else
            {
                StarAdress = int.Parse(Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.WriteAddressStar]);
            }
            for (int ii = 1; ii <= InitCount; ii++)
            {
                node.Address = StarAdress;
                node.ArrLabel = "RFID";
                node.Value = 0;
                node.EQUIP_CODE = "RFIDEQUIP";
                node.ACTION_ID = "RFIDACTION";
                node.MonitoringFlg = false;
                PLC_SignalArr[ii] = node;
                StarAdress++;
            }
            MITSUBISHISignalList.Add(PLC_SignalArr);
        }

        /// <summary>
        /// 寻找读监控位位置
        /// </summary>
        /// <param name="MITSUBISHIPLC_SignalList"></param>
        public void FindReadMonitorPos_Value(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList)
        {
            String[] ReadMonitor = Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.MonitorBit].Split(',');
            int Address = int.Parse(ReadMonitor[1]);
            for (int ii = 0; ii < MITSUBISHIPLC_SignalList.Count;ii++ )
            {
                if (MITSUBISHIPLC_SignalList[ii][0].EQUIP_CODE == ReadMonitor[0])
                {
                    for (int jj = 1; jj < MITSUBISHIPLC_SignalList[ii].Length; jj++)
                    {
                        if (MITSUBISHIPLC_SignalList[ii][jj].Address == Address)
                        {
                            ReadMonitorBitAddress1 = ii;
                            ReadMonitorBitAddress2 = jj;
                            break;
                        }
                    }
                }
                if (ReadMonitorBitAddress1 != -1)
                {
                    break;
                }
            }

            if (ReadMonitorBitAddress1 != -1 && ReadMonitorBitAddress2 != -1)
            {
                MITSUBISHIPLC_RFIDReadMonitor.Address = MITSUBISHIPLC_SignalList[ReadMonitorBitAddress1][ReadMonitorBitAddress2].Address;
                MITSUBISHIPLC_RFIDReadMonitor.ArrLabel = MITSUBISHIPLC_SignalList[ReadMonitorBitAddress1][0].ArrLabel;
                MITSUBISHIPLC_RFIDReadMonitor.WriteFlg = false;
            }
        }

        /// <summary>
        /// 寻找写监控位位置
        /// </summary>
        /// <param name="MITSUBISHIPLC_SignalList"></param>
        public void FindWriteMonitorPos_Value(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList)
        {
            String[] WriteMonitor = Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.MonitorBit].Split(',');
            int Address = int.Parse(WriteMonitor[2]);
            for (int ii = 0; ii < MITSUBISHIPLC_SignalList.Count; ii++)
            {
                if (MITSUBISHIPLC_SignalList[ii][0].EQUIP_CODE == WriteMonitor[0])
                {
                    for (int jj = 1; jj < MITSUBISHIPLC_SignalList[ii].Length; jj++)
                    {
                        if (MITSUBISHIPLC_SignalList[ii][jj].Address == Address)
                        {
                            WriteMonitorBitAddress1 = ii;
                            WriteMonitorBitAddress2 = jj;
                            break;
                        }
                    }
                }
                if (WriteMonitorBitAddress1 != -1)
                {
                    break;
                }
            }

            if (WriteMonitorBitAddress1 != -1 && WriteMonitorBitAddress2 != -1)
            {
                MITSUBISHIPLC_RFIDWriteMonitor.Address = MITSUBISHIPLC_SignalList[WriteMonitorBitAddress1][WriteMonitorBitAddress2].Address;
                MITSUBISHIPLC_RFIDWriteMonitor.ArrLabel = MITSUBISHIPLC_SignalList[WriteMonitorBitAddress1][0].ArrLabel;
                MITSUBISHIPLC_RFIDWriteMonitor.WriteFlg = false;
            }
        }

        /// <summary>
        /// 判断是否有RFID信息读触发
        /// </summary>
        /// <param name="MITSUBISHIPLC_SignalList"></param>
        /// <returns></returns>
        public bool IsReadRFIDFlg(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList)
        {
            bool Flg = false;
            if (ReadMonitorBitAddress1 != -1 && ReadMonitorBitAddress2 != -1 )
            {
                short value = MITSUBISHIPLC_SignalList[ReadMonitorBitAddress1][ReadMonitorBitAddress2].Value;
                if (value != 0)//监控位允许
                {
                    MITSUBISHIPLC_RFIDReadMonitor.WriteFlg = true;
                    MITSUBISHIPLC_RFIDReadMonitor.Value = 0;
                    Flg = true;
                }
            }
            return Flg;
        }

        /// <summary>
        /// 判断是否有RFID信息写触发
        /// </summary>
        /// <param name="MITSUBISHIPLC_SignalList"></param>
        /// <returns></returns>
        public bool IsWriteRFIDFlg(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList)
        {
            bool Flg = false;
            if (WriteMonitorBitAddress1 != -1 && WriteMonitorBitAddress2 != -1)
            {
                short value = MITSUBISHIPLC_SignalList[WriteMonitorBitAddress1][WriteMonitorBitAddress2].Value;
                if (value != 0)//监控位允许
                {
                    MITSUBISHIPLC_RFIDWriteMonitor.WriteFlg = true;
                    MITSUBISHIPLC_RFIDWriteMonitor.Value = 0;
                    Flg = true;
                }
            }
            return Flg;
        }


        /// <summary>
        /// 读完成将标志还原
        /// </summary>
        /// <param name="MITSUBISHIPLC_SignalList"></param>
        /// <returns></returns>
        public bool ReSetRFIDFlg(ref PLC_MITSUBISHI_HNC8.MITSUBISHIWriteSignalType ResetMonitor)
        {
            bool Flg = false;
            if (ResetMonitor.WriteFlg)
            {
                string szLabel = ResetMonitor.ArrLabel + ResetMonitor.Address.ToString();
                short[] iDataArray = new short[1];
                iDataArray[0] = ResetMonitor.Value;
                if (lpcom_ReferencesCOM.WriteDeviceRandom2(ref szLabel, 1, ref iDataArray) == 0)
                {
                    Flg = true;
                    ResetMonitor.WriteFlg = false;
//                     SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                     SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
//                     SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                     SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                     SendParm.Keywords = "RFID监控寄存器写成功";
//                     SendParm.EventData = szLabel + "=" + ResetMonitor.Value.ToString();
//                     SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }
                else
                {
                    SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                    SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "RFID监控寄存器写失败";
                    SendParm.EventData = szLabel + "=" + ResetMonitor.Value.ToString();
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }
            }
            return Flg;
        }

        public void SetMITSUBISHIData(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> DataList,bool ReadOrWrite)
        {
            if (ReadOrWrite)
            {
                RFIDinformationShort = RFIDReadinformationShort;
                NodeStrArr = ReadNodeStrArr;
            }
            else
            {
                RFIDinformationShort = RFIDWriteinformationShort;
                NodeStrArr = WriteNodeStrArr;
            }
            for (int ii = 1; ii < DataList[0].Length; ii++)//将编码设置到RFIDinformationShort
            {
                RFIDinformationShort[ii - 1] = DataList[0][ii].Value;
            }
            Ascii2String1(ReadOrWrite);//将编码转为字符串
        }

        public void SetNodeStrArr(String[] NodeStrArr)
        {
            if (NodeStrArr.Length != NodeLengthArr.Length)
            {
                System.Windows.Forms.MessageBox.Show("NodeStrArr长度小于NodeLengthArr！",
                    "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            this.NodeStrArr = NodeStrArr;
        }
        public void GetNodeStrArr(ref String[] NodeStrArr)
        {
            NodeStrArr = this.NodeStrArr;
        }
        public void SetRFIDinformationShort(short[] RFIDinformationShort)
        {
            if (RFIDinformationShort.Length != RFIDinformationShortLenth)
            {
                System.Windows.Forms.MessageBox.Show("RFIDinformationShort长度小于RFIDinformationShortLenth！",
                    "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            this.RFIDinformationShort = RFIDinformationShort;
        }
        public void GetRFIDinformationShort(ref short[] RFIDinformationShort)
        {
            RFIDinformationShort = this.RFIDinformationShort;
        }

        /// <summary>
        /// 将小料盘数据结构体Short[]转为字符串 
        /// </summary>
        /// <returns></returns>
        public bool Ascii2String1(bool ReadOrWrite)
        {
            try
            {
                if (ReadOrWrite)//读
                {
                    NodeLengthArr = ReadNodeLengthArr;
                    RFIDinformationShort = RFIDReadinformationShort;
                }
                else//写
                {
                    NodeLengthArr = WriteNodeLengthArr;
                    RFIDinformationShort = RFIDWriteinformationShort;
                }
                int srcOffset = 0;
                for (int ii = 0; ii < NodeLengthArr.Length;ii++ )
                {
                    NodeStrArr[ii] = ShorArr2String(ref RFIDinformationShort, srcOffset, NodeLengthArr[ii]);
                    srcOffset += NodeLengthArr[ii];
                }
                if (ReadOrWrite)
                {
                    if (!RFIDType && cncindex >= 0)//给机台添加产品系列号
                    {
                        Gongxu = int.Parse(RFIDinformationShort[15].ToString());
                        SCADA.MainForm.cnclist[cncindex].reportChanPingXuLieHao[0] = NodeStrArr[0];
                        SCADA.MainForm.cnclist[cncindex].reportChanPingXuLieHao[1] = NodeStrArr[1];
                    }

                    NodeStrArr[3] = System.Convert.ToString(RFIDinformationShort[14], 2);
                    while (NodeStrArr[3].Length < 16)
                    {
                        NodeStrArr[3] = "0" + NodeStrArr[3];
                    }
                    NodeStrArr[4] = System.Convert.ToString(RFIDinformationShort[15], 2);
                    while (NodeStrArr[4].Length < 16)
                    {
                        NodeStrArr[4] = "0" + NodeStrArr[4];
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将小料盘数据结构体字符串转为Short[]
        /// </summary>
        /// <returns></returns>
        public bool String2Ascii1()
        {
            try
            {
                for (int jj = 0; jj < RFIDinformationShort.Length; jj++)//初始化Short[]
                {
                    RFIDinformationShort[jj] = 0;
                }
                int dstOffset = 0;
                for (int ii = 0; ii < NodeStrArr.Length; ii++)
                {
                    String2ShorArr(ref NodeStrArr[ii], ref RFIDinformationShort, dstOffset, NodeLengthArr[ii]);
                    dstOffset += NodeLengthArr[ii];
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 将src数组中的srcOffset到count转换为字符串
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcOffset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public String ShorArr2String(ref short[] src, int srcOffset,int count)
        {
            int bytesrcOffset = srcOffset * sizeof(short);
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] temp = new byte[count * sizeof(short)];
            Buffer.BlockCopy(src, bytesrcOffset, temp, 0, temp.Length);
            byte[] temp1 = new byte[0];
            int ii = 0;
            for ( ; ii < temp.Length; ii++)
            {
                if (temp[ii] == 0)
                {
                    temp1 = new byte[ii];
                    Buffer.BlockCopy(temp, 0, temp1, 0, temp1.Length);
                    break;
                }
            }
            if (ii < temp.Length)
            {
                return asciiEncoding.GetString(temp1);
            }
            else
            {
                return asciiEncoding.GetString(temp);
            }
        }

        public void String2ShorArr(ref String src, ref short[] dst, int dstOffset, int count)
        {
            if (src == null)
            {
                src = "";
            }
            if (src.Length > count * sizeof(short))
            {
                throw new Exception("String2ShorArr is not valid.");
            }
            byte[] bpara = System.Text.Encoding.Default.GetBytes(src);
            Buffer.BlockCopy(bpara, 0, dst, dstOffset * sizeof(short), bpara.Length);
        }

        /// <summary>
        /// 跟新数据源表格
        /// </summary>
        public void UpDataReadData2Table()
        {
            lock (RFIDReadDataDataTable_Look)
            {
                if (RFIDReadDataDataTable.Rows.Count == RFIDDataDataTableDataMax)
                {
                    RFIDReadDataDataTable.Rows.RemoveAt(RFIDDataDataTableDataMax - 1);
                }
                DataRow r;
                r = RFIDReadDataDataTable.NewRow();
                r[0] = "0";
                if (RFIDReadDataDataTable.Rows.Count == 0)
                {
                    RFIDReadDataDataTable.Rows.Add(r);
                }
                else
                {
                    RFIDReadDataDataTable.Rows.InsertAt(r, 0);
                    for (int ii = 0; ii < RFIDReadDataDataTable.Rows.Count;ii++ )
                    {
                        RFIDReadDataDataTable.Rows[ii][0] = ii;
                    }
                }
                RFIDReadDataDataTable.Rows[0][1] = DateTime.Now.ToString();
                for (int jj = 0; jj < NodeStrArr.Length; jj++)
                {
                    RFIDReadDataDataTable.Rows[0][jj + 2] = NodeStrArr[jj];
                }

            }
        }


        public void AppExit()
        {
            SCADA.RFIDDATAT.DBWriteToXml(RFIDReadDataDataTable, RFIDDataDataTable_FileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public void RFIDDataDataTable_XMLFile_load(String FilePath, ref String[] RFIDReadDataStruct)
        {
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(FilePath);
                if (fileInfo.Length > 0)
                {
//                     RFIDReadDataDataTable.ReadXmlSchema(FilePath);
                    RFIDReadDataDataTable = SCADA.RFIDDATAT.DBReadFromXml(FilePath);
                }
            }
            if (RFIDReadDataDataTable.Columns.Count == 0)
            {
                for (int ii = 0; ii < RFIDReadDataStruct.Length; ii++)
                {
                    RFIDReadDataDataTable.Columns.Add(RFIDReadDataStruct[ii], typeof(string));
                }
            }
        }
    }
}
