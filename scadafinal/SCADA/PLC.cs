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
        #region ��������
        private MITSUBISHI.Component.DotUtlType lpcom_ReferencesCOM;//����PLCͨ�Ŷ���
        public CollectHNCPLC m_hncPLCCollector;//Hnc8PLCtͨ�Ŷ���ָ��
        public bool conneted = false;//��ʾPLC�Ƿ���������
        public static string[] DeviceLab = { "xx", "yy", "dd", "Buffer", "ss", "bb" 
                                            ,"cc","mm","ww","ll","ssmm","ff","ffdd","vv","sd"};//��ǩת����
        public static short[] DeviceLabArr = { 64, 64, 100, 112, 64, 64, 64, 50, 64, 64, 10, 64, 64, 64, 64 };//���ֱ�ǩ��һ��ȡ���ݸ���
        public static String[] connetedText = { "����", "����" };//״̬�ַ�����
        public int serial = 0;//���
        public Int32 ID = 0;//ID
        public int MITSUBISHIConnetNum = 2;//����ͨ�ſؼ������Ӻ�
        public string PassW = "";//����ؼ���������
        public String workshop = "#1";//
        public String productionline = "#9";
        public String type = "A";
        public String system = "hnc8";
        public String ip = "192.168.20.99";
        public UInt16 port = 10001;
        public String remark = "";
        public String EQUIP_CODE;//�豸ID
        public String SN;//SN��

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
                this.SubAddress = -1;//��ַ
                this.ArrLabel = "";//����
                this.Value = 0;//ֵ
                this.EQUIP_CODE = "";//�豸ID
                this.ACTION_ID = "-1";//����ID
                this.MonitoringFlg = false;//�ź��Ƿ��أ������Valueֵ�Ƿ�仯
                this.IsShow = IsShow;
                this.FirstGetValue = true;
            }

            public Int32 Address;//��ַ
            public Int32 SubAddress;
            public String ArrLabel;//����
            public short Value;//ֵ
            public String EQUIP_CODE ;//�豸ID
            public String ACTION_ID;//����ID
            public bool MonitoringFlg;//�ź��Ƿ��أ������Valueֵ�Ƿ�仯
            public bool IsShow ;//�����Ƿ���ʾ���ֵ
            public bool FirstGetValue;//��һ��ȡֵ
        };
        public struct MITSUBISHIWriteSignalType
        {
            public Int32 Address;//��ַ
            public String ArrLabel;//����
            public short Value;//ֵ
            public bool WriteFlg;//�Ƿ�д���
        };
        public struct HNC8SignalType
        {
            public HNC8SignalType(bool IsShow = true)
            {
                this.Address = 0;//��ַ
                this.SubAddress = -1;//��ַ
                this.ArrLabel = "";//����
                this.Value = 0;//ֵ
                this.EQUIP_CODE = "";//�豸ID
                this.ACTION_ID = "-1";//����ID
                this.MonitoringFlg = false;//�ź��Ƿ��أ������Valueֵ�Ƿ�仯
                this.IsShow = IsShow;
                this.FirstGetValue = true;
                this.RFIDListIndex = -1;
            }

            public Int32 Address;//��ַ
            public Int32 SubAddress;//��ַ
            public String ArrLabel;//����
            public Int32 Value ;//ֵ
            public String EQUIP_CODE;//�豸ID
            public String ACTION_ID;//����ID
            public bool MonitoringFlg ;//�ź��Ƿ��أ������Valueֵ�Ƿ�仯
            public bool IsShow;//�����Ƿ���ʾ���ֵ
            public bool FirstGetValue;//��һ��ȡֵ
            public Int32 RFIDListIndex;//����RFIDʹ��
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

        #region    PLC���ݴ����ͳ�ʼ��
        /// <summary>
        /// ��ʼ���ź�����
        /// </summary>
        /// <param name="Device">�Ĵ��������磺D</param>
        /// <param name="AddressGeshi">��ַ��ʽ��ʮ�����ơ��ǽ��ơ�������</param>
        /// <param name="StartIO">Buffer��IO��</param>
        /// <param name="InitCount">�Ĵ�������</param>
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
            MITSUBISHISignalType[] PLC_SignalArr = new MITSUBISHISignalType[InitCount + 1];//��Ҫ��صĶ�һ������һ����ʾҪ��ص���Ԫ��������
            MITSUBISHISignalType node = new MITSUBISHISignalType(true);
            node.Address = AddressGeshi;//�����ַ���Ƹ�ʽ
            node.ACTION_ID = StartIO;//Buffer��IO��
            node.EQUIP_CODE = Device;//������Ԫ�ĵ�ַ��ʾ
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

            HNC8SignalType[] PLC_SignalArr = new HNC8SignalType[InitCount + 1];//��Ҫ��صĶ�һ������һ����ʾҪ��ص���Ԫ��������
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
                    MITSUBISHISignalType[] PLC_SignalArr = new MITSUBISHISignalType[InitCount + Resut.Length];//��ԭ�еĸ��ƹ���
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
                    HNC8SignalType[] PLC_SignalArr = new HNC8SignalType[InitCount + Resut.Length];//��ԭ�еĸ��ƹ���
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
        /// ��ǩת�����ʹ��
        /// </summary>
        /// <param name="Device">�Ĵ��������磺D</param>
        /// <param name="Device_out">���ת����������±�</param>
        /// <param name="DeviceLabArrMun">һ��ȡ���Ĵ����ĸ���</param>
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
        /// ����Ҫ��ص����ݳ�ʼ�����ڴ�����
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
                if (PLC_SignalArr[0].EQUIP_CODE == Device)//�ҵ�������Ԫ��
                {
                    MITSUBISHISignalType node = new MITSUBISHISignalType(true);
                    String[] addressArr = address.Split('.');
                    if (addressArr.Length == 2)
                    {
                        if (!Int32.TryParse(addressArr[0], out node.Address))
                        {
                            throw new Exception(system + "PLC��ַ��ʽ����" + address);
                        }
                        if (!Int32.TryParse(addressArr[1], out node.SubAddress))
                        {
                            throw new Exception(system + "PLC��ַ��ʽ����" + address);
                        }
                    }
                    else
                    {
                        if (!Int32.TryParse(address, out node.Address))
                        {
                            throw new Exception(system + "PLC��ַ��ʽ����" + address);
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

                    if (node.ACTION_ID == "CNCCMD")//��ʼ��cnc�����г����ƼĴ���λ��
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
                if (PLC_SignalArr[0].ArrLabel == Device)//�ҵ�������Ԫ��
                {
                    HNC8SignalType node = new HNC8SignalType(true);
                    String[] addressArr = address.Split('.');
                    if (addressArr.Length == 2)
                    {
                        if (!Int32.TryParse(addressArr[0], out node.Address))
                        {
                            throw new Exception(system + "PLC��ַ��ʽ����" + address);
                        }
                        if (!Int32.TryParse(addressArr[1], out node.SubAddress))
                        {
                            throw new Exception(system + "PLC��ַ��ʽ����" + address);
                        }
                    }
                    else
                    {
                        if (!Int32.TryParse(address, out node.Address))
                        {
                            throw new Exception(system + "PLC��ַ��ʽ����" + address);
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

                    if (node.ACTION_ID == "CNCCMD")//��ʼ��cnc�����г����ƼĴ���λ��
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
                SendParm.Keywords = "����PLC���ӳ���";
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
        /// �ر�����
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
        /// ȡ����Ԫ��
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
                    || PLC_SignalArr0.ArrLabel == DeviceLab[7] || PLC_SignalArr0.ArrLabel == DeviceLab[10])//X��Y��M��SM
                {
                    addrassStr = "xx0";
                    if (PLC_SignalArr0.Address == 10)//ʮ����
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
                    if (PLC_SignalArr0.Address == 10)//ʮ����
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
                SendParm.Keywords = "��ȡ����PLC�����쳣";
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
                SendParm.Keywords = "��ȡ����PLC���ݳ���";
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
                else //if (PLC_SignalArr0.ArrLabel == DeviceLab[0] || PLC_SignalArr0.ArrLabel == DeviceLab[1] || PLC_SignalArr0.ArrLabel == DeviceLab[7])//X��Y��M
                {
                    addrassStr = "xx0";
                    if (PLC_SignalArr0.Address == 10)//ʮ����
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
//                     if (PLC_SignalArr0.Address == 10)//ʮ����
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
                SendParm.Keywords = "��������PLC�����쳣";
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
                SendParm.Keywords = "��������PLC���ݳ���";
                SendParm.EventData = addrassStr;
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                return false;
            }
        }
        #endregion

        #region ����PLC�������
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

        #region PLC���ݻ�ȡˢ��
        /// <summary>
        /// ����PLC��������
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
                    if (PLC_SignalArr[ii].MonitoringFlg || PLC_SignalArr[ii].IsShow)//������ʾֻ�п��ü���ֵˢ��
                    {
                        addarStar = PLC_SignalArr[ii].Address;
                        if (addarStar < nim || addarStar >= addarEnd)//�����ϴ��Ѿ�ȡ�õ����ݷ�Χ��,ȡ����������
                        {
                            if (ReadDeviceRandom2Array(ref PLC_SignalArr[0], ref addarStar, ref addarEnd, ref value))
                            {
                                nim = addarStar;
                            }
                            else
                            {
                                return false;//�������
                            }
                        }
                        if (PLC_SignalArr[ii].SubAddress != -1)//���λ
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
                            (PLC_SignalArr[ii].FirstGetValue || PLC_SignalArr[ii].Value != value1))//PLC��ص��豸����
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
                            if (PLC_SignalArr[ii].ACTION_ID == "RGV")//RGV�����ϱ�
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
            return true;//�ɹ�����
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
        /// ��ʼ�ռ��߳�
        /// </summary>
        Thread CollectThread;
        public void StarCollectThread()
        {
            if (system == SCADA.m_xmlDociment.PLC_System[0])//����PLC
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
            else if (system == SCADA.m_xmlDociment.PLC_System[1])//����8��PLC
            {
                m_hncPLCCollector = new CollectHNCPLC(EQUIP_CODE, ip, port, HNC8PLC_SignalList);
                ///��ʼ��RFID
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
                        if (ReadOrWriteFlag == "Read")//���ϴ���Ҫ�����
                        {
                            GongXu_PAdress += "=" + GongXu_II.ToString();
                            GongxuList.Add(GongXu_PAdress);
                        }
                        else if (ReadOrWriteFlag == "LineEnd")//���ϴ���β
                        {
                            GongxuList.Add(GongXu_PAdress);
                        }
                    }
                }
                ///��ʼ�����ղ���
                m_hncPLCCollector.SetGongXuParam(ref GongxuList);
                m_hncPLCCollector.ConnectPLC();
            }

            CollectThread = new Thread(new ParameterizedThreadStart(CollectThreadRuning));
            CollectThread.Start();
        }

        public void StopStarCollectThread()
        {
            CollectThreadRuningFlg = false;
            if (system == SCADA.m_xmlDociment.PLC_System[0])//����PLC
            {
                lpcom_ReferencesCOM.Close();
            }
            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
            SendParm.Keywords = "PLCֹͣ���ݲɼ�";
            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

//             else if (system == SCADA.m_xmlDociment.PLC_System[1])//����8��PLC
//             {
//                 m_hncPLCCollector.StopCollect();
//             }
        }

        /// <summary>
        /// �ռ��߳�
        /// </summary>
        /// <param name="obj"></param>
        private void CollectThreadRuning(object obj)
        {
            if (system == SCADA.m_xmlDociment.PLC_System[0])//����PLC
            {
                while (CollectThreadRuningFlg)
                {
                    if(!conneted)
                    {
                        if (0 == Open(MITSUBISHIConnetNum.ToString(), PassW))
                        {
                            for (int jj = 0; jj < MITSUBISHIPLC_SignalList.Count; jj++)//��ʼ����һ��ȡֵ��־
                            {
                                 for (int ii = 1; ii < MITSUBISHIPLC_SignalList[jj].Length; ii++)
                                {
                                    MITSUBISHIPLC_SignalList[jj][ii].FirstGetValue = true;
                                }
                            }
                            PlcEvent SendData = new PlcEvent();
                            SendData.Type = "State";
                            SendData.ArrLabel = connetedText[0];
                            SendData.Value = 1;//������Ϊ1
                            SendAutoSendHandler(SendAutoSendHandler_type.AutoUpDataPLCDataHandler, SendData);
                            MITSUBISHIPLCRFIDEvent SendData2 = new MITSUBISHIPLCRFIDEvent();
                            SendData2.plcserial = -1;
                            SendAutoSendHandler(SendAutoSendHandler_type.AutoUpDataRFIDDataHandler, SendData2);


                            if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.StateChageEvenHandle != null)
                            {
                                ScadaHncData.EQUIP_STATE m_EQUIP_STATE = new ScadaHncData.EQUIP_STATE();
                                m_EQUIP_STATE.EQUIP_TYPE = 2;
                                m_EQUIP_STATE.EQUIP_CODE = EQUIP_CODE;// VARCHAR2(50),�豸ID
                                m_EQUIP_STATE.EQUIP_CODE_CNC = SN; // VARCHAR2(50),cnc:SN��
                                m_EQUIP_STATE.STATE_VALUE = 1; // FLOAT(126),-1������״̬��0��һ��״̬������״̬����1��ѭ������������״̬����2���������֣�����״̬����3����ͣ״̬������״̬��
                                SCADA.MainForm.m_CheckHander.StateChageEvenHandle.BeginInvoke(this, m_EQUIP_STATE, null, null);
                            }

                            conneted = true;
                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "��������PLC�ɼ����ɹ�";
                            SendParm.EventData = "���Ӻ�=" + MITSUBISHIConnetNum.ToString();
                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                        else
                        {
                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "��������PLC�ɼ���ʧ��";
                            SendParm.EventData = "���Ӻ�=" + MITSUBISHIConnetNum.ToString();
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
                            //////CNC�����г����ƽӿ�
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

                            ///RFID����Ϣˢ��
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
                                SendData11.Value = 0;//����Ϊ0
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
                                    m_EQUIP_STATE.EQUIP_CODE = EQUIP_CODE;// VARCHAR2(50),�豸ID
                                    m_EQUIP_STATE.EQUIP_CODE_CNC = SN; // VARCHAR2(50),cnc:SN��
                                    m_EQUIP_STATE.STATE_VALUE = -1; // FLOAT(126),-1������״̬��0��һ��״̬������״̬����1��ѭ������������״̬����2���������֣�����״̬����3����ͣ״̬������״̬��
                                    SCADA.MainForm.m_CheckHander.StateChageEvenHandle.BeginInvoke(this, m_EQUIP_STATE, null, null);
                                }

                            }
                            Thread.Sleep(2000);
                        }
                    }
                }
            }
            else if (system == SCADA.m_xmlDociment.PLC_System[1])//����8��PLC
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
        public string Type{ get; set; }//����
        public string Address{ get; set; }//��ַ
        public string ArrLabel{ get; set; }//����
        public short Value{ get; set; }//ֵ
        public String EQUIP_CODE;//�豸ID
        public String ACTION_ID;//�豸ID
    }


    public class MITSUBISHIPLCRFIDEvent
    {
        public int plcserial { get; set; }//plc���
        public int RFIDserial { get; set; }//RFID���
        public string Event { get; set; }//�¼�����
    }
    public class HNC8PLCRFIDEvent
    {
        public int plcserial { get; set; }//plc���
        public int RFIDserial { get; set; }//RFID���
        public int EventType { get; set; }//�¼�����
        public string Event { get; set; }//�¼�����
    }
     
    public class RFIDinformation
    {
        #region ��������
        public MITSUBISHI.Component.DotUtlType lpcom_ReferencesCOM;//����PLCͨ�Ŷ���
        public int RFIDinformationShortLenth { get; set; }
        private short[] RFIDinformationShort;//112 BYTE,С���̱���
        private int[] NodeLengthArr;//�����ڵ��ֽ���
        public String[] NodeStrArr;//�������ַ���
        public bool RFIDType = true;//ʵ���ж�д����Ӧ��Ϊ�棬��ԣ���ļӹ���ϢΪ��
        public int cncindex;//��ԣ���ļӹ���Ϣ��Ӧ��̨
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
        public String EQUIP_CODE;//�豸ID
        public int Gongxu;//�յ��ļӹ�����
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
        /// ���Ը���NodeLengthArr���������ݽڵ�
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
            for (int ii = 0; ii < Attributes_RFID_AddressSet.Length; ii++)//�����ַ��ʽ
            {
                ReadNodeLengthArr[ii] = int.Parse(Attributes_RFID_AddressSet[ii]);
            }

            Attributes_RFID_AddressSet = Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.WriteAddressSet].Split(',');
            WriteNodeLengthArr = new int[Attributes_RFID_AddressSet.Length];
            for (int ii = 0; ii < Attributes_RFID_AddressSet.Length; ii++)//д���ַ��ʽ
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
                Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.ReadDevice], 0, RFIDReadinformationShortLenth);//��ʼ������ַ�б�
            InitMITSUBISHIPLC_RFIDList(ref MITSUBISHIPLC_RFIDWriteList,
                Attributes_RFID[(int)SCADA.m_xmlDociment.Attributes_RFID.WriteDevice], 0, RFIDWriteinformationShortLenth);//��ʼ��д��ַ�б�
        }

        #region RFID������
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
                SendData1.Event = "������Ϣ";
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
        /// ��ʼ��RFID��ؼĴ�����
        /// </summary>
        /// <param name="Device"></param>
        /// <param name="StartIO"></param>
        /// <param name="InitCount"></param>
        private void InitMITSUBISHIPLC_RFIDList(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHISignalList, String Device, short StartIO, Int32 InitCount)
        {
            PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[] PLC_SignalArr = new PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[InitCount + 1];//��Ҫ��صĶ�һ������һ����ʾҪ��ص���Ԫ��������
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
        /// Ѱ�Ҷ����λλ��
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
        /// Ѱ��д���λλ��
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
        /// �ж��Ƿ���RFID��Ϣ������
        /// </summary>
        /// <param name="MITSUBISHIPLC_SignalList"></param>
        /// <returns></returns>
        public bool IsReadRFIDFlg(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList)
        {
            bool Flg = false;
            if (ReadMonitorBitAddress1 != -1 && ReadMonitorBitAddress2 != -1 )
            {
                short value = MITSUBISHIPLC_SignalList[ReadMonitorBitAddress1][ReadMonitorBitAddress2].Value;
                if (value != 0)//���λ����
                {
                    MITSUBISHIPLC_RFIDReadMonitor.WriteFlg = true;
                    MITSUBISHIPLC_RFIDReadMonitor.Value = 0;
                    Flg = true;
                }
            }
            return Flg;
        }

        /// <summary>
        /// �ж��Ƿ���RFID��Ϣд����
        /// </summary>
        /// <param name="MITSUBISHIPLC_SignalList"></param>
        /// <returns></returns>
        public bool IsWriteRFIDFlg(ref List<PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[]> MITSUBISHIPLC_SignalList)
        {
            bool Flg = false;
            if (WriteMonitorBitAddress1 != -1 && WriteMonitorBitAddress2 != -1)
            {
                short value = MITSUBISHIPLC_SignalList[WriteMonitorBitAddress1][WriteMonitorBitAddress2].Value;
                if (value != 0)//���λ����
                {
                    MITSUBISHIPLC_RFIDWriteMonitor.WriteFlg = true;
                    MITSUBISHIPLC_RFIDWriteMonitor.Value = 0;
                    Flg = true;
                }
            }
            return Flg;
        }


        /// <summary>
        /// ����ɽ���־��ԭ
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
//                     SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.��Ϣ;
//                     SendParm.EventID = ((int)SCADA.LogData.Node2Level.��Ϣ).ToString();
//                     SendParm.Keywords = "RFID��ؼĴ���д�ɹ�";
//                     SendParm.EventData = szLabel + "=" + ResetMonitor.Value.ToString();
//                     SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }
                else
                {
                    SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.WARNING;
                    SendParm.EventID = ((int)SCADA.LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "RFID��ؼĴ���дʧ��";
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
            for (int ii = 1; ii < DataList[0].Length; ii++)//���������õ�RFIDinformationShort
            {
                RFIDinformationShort[ii - 1] = DataList[0][ii].Value;
            }
            Ascii2String1(ReadOrWrite);//������תΪ�ַ���
        }

        public void SetNodeStrArr(String[] NodeStrArr)
        {
            if (NodeStrArr.Length != NodeLengthArr.Length)
            {
                System.Windows.Forms.MessageBox.Show("NodeStrArr����С��NodeLengthArr��",
                    "����", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
                System.Windows.Forms.MessageBox.Show("RFIDinformationShort����С��RFIDinformationShortLenth��",
                    "����", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            this.RFIDinformationShort = RFIDinformationShort;
        }
        public void GetRFIDinformationShort(ref short[] RFIDinformationShort)
        {
            RFIDinformationShort = this.RFIDinformationShort;
        }

        /// <summary>
        /// ��С�������ݽṹ��Short[]תΪ�ַ��� 
        /// </summary>
        /// <returns></returns>
        public bool Ascii2String1(bool ReadOrWrite)
        {
            try
            {
                if (ReadOrWrite)//��
                {
                    NodeLengthArr = ReadNodeLengthArr;
                    RFIDinformationShort = RFIDReadinformationShort;
                }
                else//д
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
                    if (!RFIDType && cncindex >= 0)//����̨��Ӳ�Ʒϵ�к�
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
        /// ��С�������ݽṹ���ַ���תΪShort[]
        /// </summary>
        /// <returns></returns>
        public bool String2Ascii1()
        {
            try
            {
                for (int jj = 0; jj < RFIDinformationShort.Length; jj++)//��ʼ��Short[]
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
        /// ��src�����е�srcOffset��countת��Ϊ�ַ���
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
        /// ��������Դ���
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
