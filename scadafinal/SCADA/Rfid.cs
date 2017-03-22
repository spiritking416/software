using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using Sygole.HFReader;

namespace SCADA
{
    #region    类的定义
    public class RFID : ObservableObject
    {
        private HFReader reader = new HFReader();
        public delegate void HNC8RFIDEventHandler<HNC8PLCRFIDEvent>(object sender, PLC.HNC8PLCRFIDEvent Args);
        public static event HNC8RFIDEventHandler<PLC.HNC8PLCRFIDEvent> AutoSendHandler;//发给界面的信息
        public event HNC8RFIDEventHandler<PLC.HNC8PLCRFIDEvent> StateChangeHandler;//发给PLC的连接状态
        public int RFIDDataDataTableDataMax = 500;
        public System.Data.DataTable RFIDReadDataDataTable = new System.Data.DataTable();
        public Object RFIDReadDataDataTable_Look = new Object();
        public String[] RFIDDataStr;
        public RFID(string linkAddress, string linkPort, ref String[] RFIDDataStr)
        {
            this.linkAddress = linkAddress;
            this.linkPort = linkPort;
            this.RFIDDataStr = RFIDDataStr;
            String[] Pathstr = SCADA.MainForm.RFIDDataFilePath.Split('\\');
            RFIDDataDataTable_FileName = SCADA.MainForm.RFIDDataFilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            RFIDDataDataTable_FileName = RFIDDataDataTable_FileName.Substring(0, RFIDDataDataTable_FileName.Length - 1);
            if (!System.IO.Directory.Exists(RFIDDataDataTable_FileName))
            {
                System.IO.Directory.CreateDirectory(RFIDDataDataTable_FileName);
            }
            RFIDDataDataTable_FileName += "\\" + Pathstr[Pathstr.Length - 1] + linkPort.ToString();

            RFIDDataDataTable_XMLFile_load(RFIDDataDataTable_FileName);
            this.initLink();
        }

        private String RFIDDataDataTable_FileName;
        public void AppExit()
        {
            SCADA.RFIDDATAT.DBWriteToXml(RFIDReadDataDataTable, RFIDDataDataTable_FileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public void RFIDDataDataTable_XMLFile_load(String FilePath)
        {
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(FilePath);
                if (fileInfo.Length > 0)
                {
                    RFIDReadDataDataTable = SCADA.RFIDDATAT.DBReadFromXml(FilePath);
                }
            }
            if (RFIDReadDataDataTable.Columns.Count == 0)
            {
                for (int ii = 0; ii < SCADA.RfidForm.RFIDReadDataStruct.Length; ii++)
                {
                    RFIDReadDataDataTable.Columns.Add(SCADA.RfidForm.RFIDReadDataStruct[ii], typeof(string));
                }
            }
        }

        #region RFID属性
        /// <summary>
        /// 连接地址字符串
        /// </summary>
        private string linkAddress = "192.168.1.214";//IP
        private string linkPort = "10001";//Port
        private int readerID;
        private String _setStatus = "";
        public String setStatus
        {
            get
            {
                return this._setStatus;
            }
            set
            {
                if (this._setStatus != value)
                {
                    this._setStatus = value;
//                     PLC.HNC8PLCRFIDEvent m_SendData = new PLC.HNC8PLCRFIDEvent();
//                     m_SendData.RFIDserial = this.RFIDserial;
//                     m_SendData.EventType = 100;
//                     m_SendData.Event = value;
//                     if (RFID.AutoSendHandler != null)
//                     {
//                         RFID.AutoSendHandler.BeginInvoke(this, m_SendData, null, null);
//                     }
                }
            }
        }

        public int RFIDserial;
        /// <summary>
        /// 实时间连接状态
        /// </summary>
        public StatusVM<LinkStatusEnum> linkStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 状态刷新间隔
        /// </summary>
        public int linkStatusCheckInterval = 5000;

        /// <summary>
        /// 更新数据源表格
        /// </summary>
        public void UpDataReadData2Table()
        {
            lock (RFIDReadDataDataTable_Look)
            {
                if (RFIDReadDataDataTable.Rows.Count >= RFIDDataDataTableDataMax)
                {
                    RFIDReadDataDataTable.Rows.RemoveAt(RFIDDataDataTableDataMax - 1);
                }
                System.Data.DataRow r;
                r = RFIDReadDataDataTable.NewRow();
                RFIDReadDataDataTable.Rows.Add(r);
                for (int ii = RFIDReadDataDataTable.Rows.Count - 1; ii > 0; ii--)
                {
                    for (int jj = 1; jj < RFIDReadDataDataTable.Columns.Count; jj++)
                    {
                        RFIDReadDataDataTable.Rows[ii][jj] = RFIDReadDataDataTable.Rows[ii - 1][jj];
                    }
                    RFIDReadDataDataTable.Rows[ii][0] = ii;
                }

                RFIDReadDataDataTable.Rows[0][0] = 0;
                RFIDReadDataDataTable.Rows[0][1] = DateTime.Now.ToString();
                for (int jj = 0; jj < RFIDDataStr.Length; jj++)
                {
                    RFIDReadDataDataTable.Rows[0][jj + 2] = RFIDDataStr[jj];
                }
            }
            PLC.HNC8PLCRFIDEvent m_SendData = new PLC.HNC8PLCRFIDEvent();
            m_SendData.EventType = 200;
            if (SCADA.RFID.AutoSendHandler != null)
            {
                SCADA.RFID.AutoSendHandler.BeginInvoke(this, m_SendData, null, null);
            }

        }

        #endregion


        #region 配置属性

        /// <summary>
        /// 功率模式（全功率，半功率）
        /// </summary>
        private PowerModeEnum _powerMode;
        public PowerModeEnum powerMode
        {
            get { return _powerMode; }
            set { setValue(ref _powerMode, value, "powerMode"); }
        }

        /// <summary>
        /// 命令持续时间（直到成功，只执行一次）
        /// </summary>
        private CommandModeEnum _commandMode;
        public CommandModeEnum commandMode
        {
            get { return _commandMode; }
            set { setValue(ref _commandMode, value, "commandMode"); }
        }

        /// <summary>
        /// 服务数据发送通讯方式
        /// </summary>
        private ServiceCommunicationModeEnum _serviceCommunicationMode;
        public ServiceCommunicationModeEnum serviceCommunicationMode
        {
            get { return _serviceCommunicationMode; }
            set { setValue(ref _serviceCommunicationMode, value, "serviceCommunicationMode"); }
        }

        /// <summary>
        /// 固件版本号
        /// </summary>
        private string _framewareVersion;
        public string framewareVersion
        {
            get { return _framewareVersion; }
            set { setValue(ref _framewareVersion, value, "framewareVersion"); }
        }

        #endregion

        #region 配置方法

        public void getUserConfig()
        {
            UserCfg cfg = new UserCfg();
            if (reader.GetUserCfg((byte)0x00, ref cfg) == Status_enum.SUCCESS)
            {
                this.readerID = (int)cfg.ReaderID;
                reader.SetReaderID(this.readerID);
                this.commandMode = (CommandModeEnum)cfg.AvailableTime;
                this.serviceCommunicationMode = (ServiceCommunicationModeEnum)cfg.CommPort;
                this.powerMode = (PowerModeEnum)cfg.RFChipPower;
            }
        }

        public void setUserConfig()
        {
            UserCfg cfg = new UserCfg();
            Status_enum stat;

            stat = reader.GetUserCfg((byte)0x00, ref cfg);
            if (stat != Status_enum.SUCCESS)
            {
                return;
            }

            cfg.ReaderID = (byte)this.readerID;
            cfg.AvailableTime = (AvailableTime_enum)this.commandMode;
            cfg.CommPort = (CommPort_enum)this.serviceCommunicationMode;
            cfg.RFChipPower = (RFChipPower_enum)this.powerMode;

            stat = reader.SetUserCfg((byte)this.readerID, cfg);
            if (stat == Status_enum.SUCCESS)
            {
                return;
            }
        }

        public void getFramewareVersion()
        {
            byte[] ver = new byte[20];
            byte len = (byte)ver.Length;
            for (int i = 0; i < len; i++) { ver[i] = 0x00; }
            Status_enum stat = reader.GetSWVer((byte)this.readerID, ref ver, ref len);
            if (stat != Status_enum.SUCCESS) return;
            string v = "";
            for (int i = 0; i < len; i++)
            {
                v += (char)ver[i];
            }
            System.Text.Encoding.ASCII.GetString(ver).Trim();
            this.framewareVersion = v.ToString();
        }

        public void setDefaultUserConfig()
        {
            UserCfg cfg = new UserCfg();
            Status_enum stat;

            stat = reader.GetUserCfg((byte)0x00, ref cfg);
            if (stat != Status_enum.SUCCESS)
            {
                return;
            }

            this.commandMode = CommandModeEnum.JustOnce;
            this.serviceCommunicationMode = ServiceCommunicationModeEnum.RS_232;
            this.powerMode = PowerModeEnum.Full;

            cfg.AvailableTime = (AvailableTime_enum)this.commandMode;
            cfg.CommPort = (CommPort_enum)this.serviceCommunicationMode;
            cfg.RFChipPower = (RFChipPower_enum)this.powerMode;

            stat = reader.SetUserCfg((byte)this.readerID, cfg);
            if (stat != Status_enum.SUCCESS)
            {
                return;
            }
        }

        public void resetMCU()
        {
            reader.SoftReset((byte)this.readerID);
        }

        #endregion

        #region 连接方法

        /// <summary>
        /// 初始化连接
        /// </summary>
        public void initLink()
        {
            if (reader == null) return;
            this.autoLinkStatus = ServiceStatusEnum.Closed;
            this.linkStatus = new StatusVM<LinkStatusEnum>(this, this.getLinkStatus);
            this.linkStatus.now = LinkStatusEnum.Unlink;
            this.linkStatus.StatusChanged += this.linkStatusChangeHandler;
            this.linkStatus.LinkEvent += this.LinkEventHandler;
        }

        private void LinkEventHandler(StatusVM<LinkStatusEnum> sender)
        {
            if (this.autoLinkStatus != ServiceStatusEnum.Opened)
            {
                return;
            }

            //检测状态，并根据需要重连
            //             if (LinkStatusEnum.Linked != sender.now && LinkStatusEnum.Unlinking != sender.now && LinkStatusEnum.Linked == sender.last)
            if (sender.now == LinkStatusEnum.Unlink)
            {
                try
                {
                    if (ClientPingTest(linkAddress))
                    {
                        this.connect();
                    }
                    else
                    {
                        this.setStatus = "网络故障";
                    }
                }
                catch { ;}
            }

        }
        private Boolean ClientPingTest(String ip)
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply pingReply = ping.Send(ip, 1000);
                if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
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

        public void connect()
        {
            if (this.linkAddress == "" || this.linkPort == "")
            {
                //                 this.setStatus("连接地址错误");
                this.linkStatus.now = LinkStatusEnum.Unlink;
                return;
            }
            if (this.linkStatus.now == LinkStatusEnum.Linked || this.linkStatus.now == LinkStatusEnum.Linking)
            {
                return;
            }
            if (reader.Connect(this.linkAddress, int.Parse(this.linkPort)))
            {
//                 TimeCounter tc = new TimeCounter();
//                 tc.start();
//                 while (tc.now() < 500 && this.linkStatus.now != LinkStatusEnum.Linked)
                {
                    this.getUserConfig();
                    this.linkStatus.update();
                }
            }


            //             if (this.linkStatus.now != LinkStatusEnum.Linked)
            //             {
            //                 this.disconnect();
            //             }
            //this.linkStatus.now = LinkStatusEnum.Linked;
            //this.addCommEvent();  //回显数据
        }

        public void disconnect()
        {
            this.stopCheck();
            reader.DisConnect();
            this.linkStatus.reset();
            this.linkStatus.now = LinkStatusEnum.Unlink;
        }


        /// <summary>
        /// 开启自动检测状态与自动重连
        /// </summary>
        public void startCheck()
        {
            if (reader == null) return;
            if (this.linkStatus.isChecking() == false)
            {
                this.autoLinkStatus = ServiceStatusEnum.Opening;
                if (linkStatusCheckInterval < 100) linkStatusCheckInterval = 100;
                this.linkStatus.startCheck(linkStatusCheckInterval);
                this.autoLinkStatus = ServiceStatusEnum.Opened;
                this.setStatus = "开启自动重连";
            }
            else
            {
                stopCheck();
            }
        }

        /// <summary>
        /// 关闭自动检测状态与自动重连
        /// </summary>
        public void stopCheck()
        {
            if (reader == null) return;
            if (this.linkStatus.isChecking() == true)
            {
                this.autoLinkStatus = ServiceStatusEnum.Closing;
                this.linkStatus.stopCheck();
                this.autoLinkStatus = ServiceStatusEnum.Closed;
            }
            this.setStatus = "停止自动重连";
        }

        /// <summary>
        /// 使用底层库函数检测连接状态(未使用）
        /// </summary>
        /// <returns></returns>
        public LinkStatusEnum _getLinkStatus()
        {
            if (reader == null) return LinkStatusEnum.Unknown;

            //             switch (reader.ConnectStatus)
            //             {
            //                 case ConnectStatusEnum.CONNECTED:
            //                     return LinkStatusEnum.Linked;
            //                 case ConnectStatusEnum.DISCONNECTED:
            //                     return LinkStatusEnum.Unlink;
            //                 case ConnectStatusEnum.CONNECTING:
            //                     return LinkStatusEnum.Linking;
            //                 case ConnectStatusEnum.CONNECTLOST:
            //                     return LinkStatusEnum.Unlink;
            //                 default:
            //                     return LinkStatusEnum.Unknown;
            //             }
            return LinkStatusEnum.Unknown;
        }

        /// <summary>
        /// 获取连接状态函数
        /// </summary>
        /// <returns></returns>
        public LinkStatusEnum getLinkStatus()
        {
            if (reader == null) return LinkStatusEnum.Unknown;
            TimeCounter tc = new TimeCounter();

            try
            {
                tc.start();
                TagInfo info = new TagInfo();
                if (reader.CheckStatus() == Status_enum.SUCCESS)
                {
                    //                     this.setStatus = "成功" + tc.ToString() + " 毫秒";
                    return LinkStatusEnum.Linked;
                }
                else
                {
                    //                     this.setStatus = "失败" + tc.ToString() + " 毫秒";
                    return LinkStatusEnum.Unlink;
                }
            }
            catch
            {
                //                 this.setStatus = "失败" + tc.ToString() + " 毫秒";
                return LinkStatusEnum.Unlink;
            }




            //             TimeCounter tc = new TimeCounter();
            //             tc.start();
            // 
            // 
            //             byte[] ver = new byte[20];
            //             byte len = (byte)ver.Length;
            //             for (int i = 0; i < len; i++) { ver[i] = 0x00; }
            // 
            //             try
            //             {
            //                 Status_enum stat = reader.GetSWVer((byte)this.readerID, ref ver, ref len);
            //                 if (stat == Status_enum.SUCCESS)
            //                 {
            //                     this.setStatus = "成功" + tc.ToString() + " 毫秒";
            // 
            //                     return LinkStatusEnum.Linked;
            //                 }
            //                 else
            //                 {
            // //                     this.linkDisconnectionCount++;
            //                     this.setStatus = "失败" + tc.ToString() + " 毫秒";
            // 
            //                     return LinkStatusEnum.Unlink;
            //                 }
            //             }
            //             catch
            //             {
            // //                 this.linkDisconnectionCount++;
            //                 this.setStatus = "失败" + tc.ToString() + " 毫秒";
            // 
            //                 return LinkStatusEnum.Unlink;
            //             }
        }

        #endregion

        #region 读写标签属性

        /// <summary>
        /// 标签读写寻址方式
        /// </summary>
        //private TagAddressionModeEnum tagAddressingMode = default(TagAddressionModeEnum);

        /// <summary>
        /// 最后读到的标签的UID
        /// </summary>
        private string lastTagUID = "";

        /// <summary>
        /// 最后一次设置的标签用户内存数据起始地址
        /// </summary>
        private int lastTagDataStartBlock = 0;

        /// <summary>
        /// 最后一次设置的标签用户内存数据块数量
        /// </summary>
        private int lastTagDataBlockQuantity = 1;

        /// <summary>
        /// 最后一次设置的标签用户内存数据块大小
        /// </summary>
        private TagBlockSizeEnum lastTagDataBlockSize = default(TagBlockSizeEnum);

        /// <summary>
        /// 最后一次读/写到的标签用户内存数据
        /// </summary>
        public string lastTagData;

        #endregion

        #region 读写标签方法

        /// <summary>
        /// 获取标签UID
        /// </summary>
        public void readTagUID()
        {
            byte[] uid = new byte[8];
            if (reader.Inventory((byte)this.readerID, ref uid) != Status_enum.SUCCESS)
            {
                return;
            }

            HexString hs = new HexString(uid);
            if (hs.hex == null)
            {
                return;
            }
            this.lastTagUID = hs.ToString();
        }

        /// <summary>
        /// 读标签用户内存
        /// </summary>
        public void readTagMemory()
        {
            byte[] data = new byte[160];
            byte len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadSBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, 0x00, ref data, ref len))
            {
                //                 this.setStatus("读多块失败");
                return;
            }

            if (len == 4)
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_4_Bytes;
            }
            else
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_8_Bytes;
            }

            byte start = (byte)this.lastTagDataStartBlock;
            //最多读8块
            if (this.lastTagDataBlockQuantity > 8) this.lastTagDataBlockQuantity = 8;
            byte cnt = (byte)this.lastTagDataBlockQuantity;
            len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
            {
                this.setStatus = "读多块失败";
                return;
            }
            HexString hs = new HexString(data, len);
            this.lastTagData = hs.ToString((int)this.lastTagDataBlockSize);
            this.setStatus = "读多块成功";
        }

        /// <summary>
        /// 写标签用户内存
        /// </summary>
        public void writeTagMemory()
        {
            byte[] data = new byte[16];
            byte len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadSBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, 0x00, ref data, ref len))
            {
                return;
            }

            if (len == 4)
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_4_Bytes;
            }
            else
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_8_Bytes;
            }

            byte start = (byte)this.lastTagDataStartBlock;
            //最多读8块
            if (this.lastTagDataBlockQuantity > 8) this.lastTagDataBlockQuantity = 8;
            byte cnt = (byte)this.lastTagDataBlockQuantity;
            int blockSize = (int)this.lastTagDataBlockSize;
            int byteLen = ((int)cnt * blockSize);
            HexString hs = new HexString(this.lastTagData, byteLen);
            if (hs.len() != byteLen)
            {
                return;
            }

            else if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, hs.hex))
            {
                return;
            }
            this.lastTagData = hs.ToString((int)this.lastTagDataBlockSize);
        }

        #endregion

        #region 标签安全与信息属性

        /// <summary>
        /// 最后一次获得的标签块大小信息
        /// </summary>
        private int _lastTagBlockSize = 0;
        public int lastTagBlockSize
        {
            get { return _lastTagBlockSize; }
            set { setValue(ref _lastTagBlockSize, value, "lastTagBlockSize"); }
        }

        /// <summary>
        /// 最后一次获得的标签块数量信息
        /// </summary>
        private int _lastTagBlockQuantity = 0;
        public int lastTagBlockQuantity
        {
            get { return _lastTagBlockQuantity; }
            set { setValue(ref _lastTagBlockQuantity, value, "lastTagBlockQuantity"); }
        }

        #endregion

        #region 标签安全与信息方法

        public void readTagSystemInfo()
        {
            TagInfo info = new TagInfo();
            if (Status_enum.SUCCESS != reader.GetTagInfo((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, ref info, Antenna_enum.ANT_1))
            {
                return;
            }

            this.lastTagBlockSize = (int)info.BlockSize + 1;
            if ((int)info.BlockSize == 3)
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_4_Bytes;
            }
            else
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_8_Bytes;
            }

            this.lastTagBlockQuantity = (int)info.BlockCnt + 1;
            this.lastTagDataBlockQuantity = (int)info.BlockCnt + 1;
        }

        public void readTagSecurityInfo()
        {
            byte start = (byte)this.lastTagDataStartBlock;
            if (this.lastTagDataBlockQuantity > 8) this.lastTagDataBlockQuantity = 8; //最多读8块
            byte cnt = (byte)this.lastTagDataBlockQuantity;
            int blockSize = (int)this.lastTagDataBlockSize;
            int byteLen = ((int)cnt * blockSize);
            byte[] data = new byte[byteLen];

            if (Status_enum.SUCCESS != reader.GetTagSecurity((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data))
            {
                return;
            }

            HexString hs = new HexString(data, byteLen);
            this.lastTagData = hs.ToString((int)this.lastTagDataBlockSize);
        }

        #endregion


        #region 标签分析器

        /// <summary>
        /// 标签内存数据
        /// </summary>
        private string _lastTagMemory = "";
        public string lastTagMemory
        {
            get { return _lastTagMemory; }
            set { setValue(ref _lastTagMemory, value, "lastTagMemory"); }
        }

        public void tagAnalyserRead()
        {
            byte[] data = new byte[160];
            byte len = (byte)data.Length;

            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity;
            int blockSize = this.lastTagBlockSize;

            this.lastTagMemory = "";
            int i = 0;
            byte start;
            byte cnt;
            HexString hs;

            TimeCounter tc = new TimeCounter();
            tc.start();

            //以8块为单位读出块数据
            for (i = 0; i < blockCount / 8; i++)
            {
                start = (byte)(i * 8);
                cnt = (byte)8;
                len = (byte)data.Length;
                if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                {
                    this.setStatus = "数据读出失败";
                    return;
                }
                hs = new HexString(data, len);
                this.lastTagMemory += hs.ToString(blockSize);
            }

            //读出剩余块
            start = (byte)(i * 8);
            cnt = (byte)(blockCount % 8);
            len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
            {
                this.setStatus = "数据读出失败";
                return;
            }
            hs = new HexString(data, len);
            this.lastTagMemory += hs.ToString(blockSize);

            this.setStatus = "分析成功。耗时 " + tc.ToString() + " 毫秒";
            return;
        }

        public void tagAnalyserWrite()
        {
            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity; //块数量
            int blockSize = this.lastTagBlockSize; //块大小

            int i = 0;
            byte start;
            byte cnt;
            HexString hs;

            TimeCounter tc = new TimeCounter();
            tc.start();

            hs = new HexString(this.lastTagMemory);
            byte[] data;

            int writeBlockCount = hs.len() / blockSize;
            bool dataCutFlag = false;
            if ((hs.len() % blockSize) != 0 || writeBlockCount > blockCount)
            {
                dataCutFlag = true;
            }

            if (writeBlockCount > 0)
            {
                for (i = 0; i < writeBlockCount / 8; i++)
                {
                    start = (byte)(i * 8);
                    cnt = (byte)8;
                    data = hs.getHex(start * blockSize, cnt * blockSize);
                    if (data == null)
                    {
                        this.setStatus = "数据错误，写入失败";
                        return;
                    }

                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                    {
                        this.setStatus = "数据错误，写入失败";
                        return;
                    }
                }

                start = (byte)(i * 8);
                cnt = (byte)(writeBlockCount % 8);
                data = hs.getHex(start * blockSize, cnt * blockSize);
                if (data == null)
                {
                    this.setStatus = "数据错误，写入失败";
                    return;
                }

                if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                {
                    this.setStatus = "数据错误，写入失败";
                    return;
                }
            }
            else
            {
                this.setStatus = "数据过短或格式错误，写入失败";
                return;
            }

            if (dataCutFlag == true)
            {
                this.setStatus = "数据末尾被截断,写入成功,耗时 " + tc.ToString() + " 毫秒";
            }
            else
            {
                this.setStatus = "写入成功,耗时 " + tc.ToString() + " 毫秒";
            }

        }

        #endregion

        #region 标签分析器ASCII

        /// <summary>
        /// 标签内存数据ASCII格式
        /// </summary>
        private string _lastTagMemoryASCII = "";
        public string lastTagMemoryASCII
        {
            get { return _lastTagMemoryASCII; }
            set { setValue(ref _lastTagMemoryASCII, value, "lastTagMemoryASCII"); }
        }

        public void tagAnalyserReadASCII()
        {
            byte[] data = this.tagReadAll();
            HexASCII ha = new HexASCII(data);
            lastTagMemoryASCII = ha.ToString();
        }

        public void tagAnalyserWriteASCII()
        {
            HexASCII ha = new HexASCII(lastTagMemoryASCII);
            this.tagWriteAll(ha.hex);
        }

        /// <summary>
        /// 读取所有标签数据
        /// </summary>
        /// <returns>标签数据byte[]</returns>
        public byte[] tagReadAll()
        {
            linkStatus.threakeepCheckEventOK = false;

            byte[] data = new byte[160];
            byte len = (byte)data.Length;

            //计时
            TimeCounter tc = new TimeCounter();
            tc.start();

            //获取标签块大小与块数量
            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity;
            int blockSize = this.lastTagBlockSize;

            //返回的数据，初始化
            byte[] tagMemory = new byte[blockCount * blockSize];
            for (int j = 0; j < tagMemory.Length; j++)
            {
                tagMemory[j] = 0x00;
            }

            int i = 0;
            byte start;
            byte cnt;

            //以8块为单位读出块数据
            for (i = 0; i < blockCount / 8; i++)
            {
                start = (byte)(i * 8);
                cnt = (byte)8;
                len = (byte)data.Length;
                if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                {
                    this.setStatus = "数据读出失败";
                    linkStatus.threakeepCheckEvent.Set();
                    return tagMemory;
                }
                Array.Copy(data, 0, tagMemory, start * blockSize, len);
            }

            //读出剩余块
            start = (byte)(i * 8);
            cnt = (byte)(blockCount % 8);
            len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
            {
                this.setStatus = "数据读出失败";
                return tagMemory;
            }
            Array.Copy(data, 0, tagMemory, start * blockSize, len);
            this.setStatus = "读出成功:" + tc.ToString() + " 毫秒";
            linkStatus.threakeepCheckEvent.Set();
            return tagMemory;
        }

        /// <summary>
        /// 写入所有数据
        /// </summary>
        /// <param name="tagMemory">需要写入的数据byte[]</param>
        public void tagWriteAll(byte[] tagMemory)
        {
            //计时
            TimeCounter tc = new TimeCounter();
            tc.start();

            //获取标签块大小与块数量
            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity;
            int blockSize = this.lastTagBlockSize;

            int i = 0;
            byte start;
            byte cnt;

            byte[] data = new byte[blockSize * 8];

            int writeBlockCount = tagMemory.Length / blockSize;
            bool dataCutFlag = false;

            //非块大小对齐
            if ((tagMemory.Length % blockSize) != 0)
            {
                writeBlockCount = writeBlockCount + 1;
            }

            //超出长度
            if (writeBlockCount > blockCount)
            {
                dataCutFlag = true;
                writeBlockCount = blockCount;
            }

            //开始写入
            if (writeBlockCount > 0)
            {
                for (i = 0; i < writeBlockCount / 8; i++)
                {
                    start = (byte)(i * 8);
                    cnt = (byte)8;
                    Array.Copy(tagMemory, start * blockSize, data, 0, cnt * blockSize);

                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                    {
                        this.setStatus = "数据错误，写入失败";
                        return;
                    }
                }

                //写入剩余块
                start = (byte)(i * 8);
                cnt = (byte)(writeBlockCount % 8);
                data = new byte[cnt * blockSize];
                for (int j = 0; j < data.Length; j++)
                {
                    if (j + start * blockSize < tagMemory.Length)
                    {
                        data[j] = tagMemory[j + start * blockSize];
                    }
                    else
                    {
                        data[j] = 0x00;
                    }
                }

                if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                {
                    this.setStatus = "数据错误，写入失败";
                    return;
                }
            }
            else
            {
                this.setStatus = "数据过短或格式错误，写入失败";
                return;
            }

            if (dataCutFlag == true)
            {
                this.setStatus = "数据末尾被截断,写入成功,耗时 " + tc.ToString() + " 毫秒";
            }
            else
            {
                this.setStatus = "写入成功,耗时 " + tc.ToString() + " 毫秒";
            }
        }

        public bool ReadUserMessage(byte DataLenth, ref byte[] tagMemory)
        {
            linkStatus.threakeepCheckEventOK = false;

            //计时
            TimeCounter tc = new TimeCounter();
            tc.start();

            //             byte[] tagMemory = new byte[DataLenth];
            for (int jj = 0; jj < DataLenth; jj++)
            {
                tagMemory[jj] = 0x00;
            }

            byte blockSize = 4;
            byte start = 0;
            byte cnt = (byte)8;
            byte len = (byte)(blockSize * cnt);
            byte[] data = new byte[len];
            for (int ii = 0; ii < DataLenth; ii += blockSize * cnt)
            {
                start = (byte)(ii / blockSize);
                if ((ii + len) <= DataLenth)
                {
                    if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                    {
                        this.setStatus = "数据读出失败!";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
                else
                {
                    cnt = (byte)((DataLenth - ii) / blockSize);
                    len = (byte)(blockSize * cnt);
                    data = new byte[len];
                    if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                    {
                        this.setStatus = "数据读出失败!";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
                Array.Copy(data, 0, tagMemory, start * 4, len);
            }
            this.setStatus = "读出成功:" + tc.ToString() + " 毫秒";
            linkStatus.threakeepCheckEvent.Set();
            return true;
        }
        public bool WriteUserMessage(int addresstart, byte[] Data)
        {
            linkStatus.threakeepCheckEventOK = false;
            //计时
            TimeCounter tc = new TimeCounter();
            tc.start();

            byte blockSize = 4;
            byte cnt = (byte)8;
            if ((Data.Length % blockSize) != 0)//只能写入整数块
            {
                this.setStatus = "数据错误" + tc.ToString() + " 毫秒";
                return false;
            }
            byte len = (byte)(blockSize * cnt);
            byte Star = (byte)(addresstart / blockSize);
            for (int ii = 0; ii < Data.Length; ii += len)
            {
                Star = (byte)(Star + (ii / blockSize));
                if ((ii + len) <= Data.Length)
                {
                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, Star, cnt, blockSize, Data))
                    {
                        this.setStatus = "写入失败" + tc.ToString() + " 毫秒";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
                else
                {
                    cnt = (byte)((Data.Length - ii) / blockSize);
                    //                     len = (byte)(4 * cnt);
                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, Star, cnt, blockSize, Data))
                    {
                        this.setStatus = "写入失败" + tc.ToString() + " 毫秒";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
            }
            this.setStatus = "写入成功" + tc.ToString() + " 毫秒";
            linkStatus.threakeepCheckEvent.Set();
            return true;
        }


        #endregion


        #region 自动重连

        /// <summary>
        /// 自动重连状态
        /// </summary>
        private ServiceStatusEnum _autoLinkStatus = default(ServiceStatusEnum);
        public ServiceStatusEnum autoLinkStatus
        {
            get { return _autoLinkStatus; }
            set { setValue(ref _autoLinkStatus, value, "autoLinkStatus"); }
        }

        /// <summary>
        /// 连接状态改变事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        public void linkStatusChangeHandler(StatusVM<LinkStatusEnum> sender)
        {
            PLC.HNC8PLCRFIDEvent m_SendData = new PLC.HNC8PLCRFIDEvent();
            m_SendData.RFIDserial = this.RFIDserial;
            if (sender.now == LinkStatusEnum.Unlink)
            {
                m_SendData.EventType = -1;
            }
            else if (sender.now == LinkStatusEnum.Linked)
            {
                m_SendData.EventType = 0;
            }

            if (RFID.AutoSendHandler != null)
            {
                RFID.AutoSendHandler.BeginInvoke(this, m_SendData, null, null);
            }
            if (StateChangeHandler != null)
            {
                StateChangeHandler.BeginInvoke(this, m_SendData, null, null);
            }

            //如果不开启自动读卡，直接返回
            //             if (this.autoLinkStatus != ServiceStatusEnum.Opened)
            //             {
            //                 return;
            //             }

            //检测状态，并根据需要重连
            //             if (LinkStatusEnum.Linked != sender.now && LinkStatusEnum.Unlinking != sender.now && LinkStatusEnum.Linked == sender.last)
            //             if (LinkStatusEnum.Linked != sender.now)
            //             {
            //                 try
            //                 {
            //                     this.connect();
            //                 }
            //                 catch { ;}
            //                 this.linkStatus.update();
            //             }
        }


        #endregion

        #region RFIDReadDataDataTable和XML的转
        /// <summary>
        /// 将DataTable的内容写入到XML文件中
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="address">XML文件地址</param>
//         public static bool WriteToXml(System.Data.DataTable dt, string address)
//         {
//             try
//             {
//                 //如果文件DataTable.xml存在则直接删除
//                 if (System.IO.File.Exists(address))
//                 {
//                     System.IO.File.Delete(address);
//                 }
// 
//                 System.Xml.XmlTextWriter writer =
//                     new System.Xml.XmlTextWriter(address, Encoding.GetEncoding("GBK"));
//                 writer.Formatting = System.Xml.Formatting.Indented;
// 
//                 //XML文档创建开始
//                 writer.WriteStartDocument();
// 
//                 writer.WriteComment("DataTable: " + dt.TableName);
// 
//                 writer.WriteStartElement("DataTable"); //DataTable开始
// 
//                 writer.WriteAttributeString("TableName", dt.TableName);
//                 writer.WriteAttributeString("CountOfRows", dt.Rows.Count.ToString());
//                 writer.WriteAttributeString("CountOfColumns", dt.Columns.Count.ToString());
// 
//                 writer.WriteStartElement("ClomunName", ""); //ColumnName开始
// 
//                 for (int i = 0; i < dt.Columns.Count; i++)
//                 {
//                     writer.WriteAttributeString(
//                         "Column" + i.ToString(), dt.Columns[i].ColumnName);
//                 }
// 
//                 writer.WriteEndElement(); //ColumnName结束
// 
//                 //按行各行
//                 for (int j = 0; j < dt.Rows.Count; j++)
//                 {
//                     writer.WriteStartElement("Row" + j.ToString(), "");
// 
//                     //打印各列
//                     for (int k = 0; k < dt.Columns.Count; k++)
//                     {
//                         writer.WriteAttributeString(
//                             "Column" + k.ToString(), dt.Rows[j][k].ToString());
//                     }
// 
//                     writer.WriteEndElement();
//                 }
// 
//                 writer.WriteEndElement(); //DataTable结束
// 
//                 writer.WriteEndDocument();
//                 writer.Close();
// 
//                 //XML文档创建结束
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//                 return false;
//             }
// 
//             return true;
//         }
// 
//         /// <summary>
//         /// 从XML文件中读取一个DataTable
//         /// </summary>
//         /// <param name="dt">数据源</param>
//         /// <param name="address">XML文件地址</param>
//         /// <returns></returns>
//         public System.Data.DataTable ReadFromXml(string address)
//         {
//             System.Data.DataTable dt = new System.Data.DataTable();
// 
//             try
//             {
//                 if (!System.IO.File.Exists(address))
//                 {
//                     throw new Exception("文件不存在!");
//                 }
// 
//                 System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
//                 xmlDoc.Load(address);
// 
//                 System.Xml.XmlNode root = xmlDoc.SelectSingleNode("DataTable");
// 
//                 //读取表名
//                 dt.TableName = ((System.Xml.XmlElement)root).GetAttribute("TableName");
//                 //Console.WriteLine("读取表名： {0}", dt.TableName);
// 
//                 //读取行数
//                 int CountOfRows = 0;
//                 if (!int.TryParse(((System.Xml.XmlElement)root).
//                     GetAttribute("CountOfRows").ToString(), out CountOfRows))
//                 {
//                     throw new Exception("行数转换失败");
//                 }
// 
//                 //读取列数
//                 int CountOfColumns = 0;
//                 if (!int.TryParse(((System.Xml.XmlElement)root).
//                     GetAttribute("CountOfColumns").ToString(), out CountOfColumns))
//                 {
//                     throw new Exception("列数转换失败");
//                 }
// 
//                 //从第一行中读取记录的列名
//                 foreach (System.Xml.XmlAttribute xa in root.ChildNodes[0].Attributes)
//                 {
//                     dt.Columns.Add(xa.Value);
//                     //Console.WriteLine("建立列： {0}", xa.Value);
//                 }
// 
//                 //从后面的行中读取行信息
//                 for (int i = 1; i < root.ChildNodes.Count; i++)
//                 {
//                     string[] array = new string[root.ChildNodes[0].Attributes.Count];
//                     for (int j = 0; j < array.Length; j++)
//                     {
//                         array[j] = root.ChildNodes[i].Attributes[j].Value.ToString();
//                     }
//                     dt.Rows.Add(array);
//                     //Console.WriteLine("行插入成功");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//                 return new System.Data.DataTable();
//             }
// 
//             return dt;
//         }

        #endregion
    }
    #endregion

    #region StatusVM<STATUS_TYPE>

    /// <summary>
    /// 实时状态显示VM
    /// </summary>
    /// <typeparam name="STATUS_TYPE">状态量的类型（如枚举或整数等）</typeparam>
    /// <bug>
    ///  1 - 没有实现析构函数，删除对象是建议手动调用析构，否则关闭动作将执行很长时间（等待线程被清理）
    ///  2 - 状态跟新读取的原子性没有保证
    /// </bug>
    /// <example>
    /// in xaml:
    ///     <Window.Resources>
    ///         <Style x:Key="StatusStyle" TargetType="Shape">
    ///             <Style.Triggers>
    ///                 <DataTrigger Binding="{Binding Path=s.now}" Value="{x:Static vm:LightStatus.ON}">
    ///                     <Setter Property="Fill" Value="Green" />
    ///                 </DataTrigger>
    ///                 <DataTrigger Binding="{Binding Path=s.now}" Value="{x:Static vm:LightStatus.OFF}">
    ///                     <Setter Property="Fill" Value="#FF6666" />
    ///                 </DataTrigger>
    ///             </Style.Triggers>
    ///         </Style>
    ///     </Window.Resources>
    /// </example>
    public class StatusVM<STATUS_TYPE> : ObservableObject //IDisposable
    {
        public StatusVM(object owner, Func<STATUS_TYPE> getStatusFunc)
        {
            this.owner = owner;
            this.getStatusFunc = getStatusFunc;
        }

        ~StatusVM()
        {
            this.stopCheck();
        }

        #region 属性

        /// <summary>
        /// 自动更新线程Handle
        /// </summary>
        private Thread keepCheckThread = null;

        /// <summary>
        /// 停止自动更新线程Flag
        /// </summary>
        private bool StopCheckFlag = true;

        /// <summary>
        /// 自动更新间隔（毫秒）
        /// </summary>
        private int checkInterval_ms = 0;

        /// <summary>
        /// 状态变更事件 function(object this)
        /// 如：public void linkStatusChangeHandler(StatusVM<LinkStatusEnum> sender);
        /// </summary>
        public event Action<StatusVM<STATUS_TYPE>> StatusChanged;
        public event Action<StatusVM<STATUS_TYPE>> LinkEvent;
        //         public delegate void RFIDEventHandler<PlcEvent>(object sender, MITSUBISHIPLCRFIDEvent Args);
        // 
        //         public event EventHandler<PlcEvent> AutoSendHandler;

        /// <summary>
        /// 状态拥有者
        /// </summary>
        public object owner;

        /// <summary>
        /// 自定义的状态更新函数
        /// </summary>
        public Func<STATUS_TYPE> getStatusFunc;

        /// <summary>
        /// 当前状态
        /// </summary>
        private STATUS_TYPE _now = default(STATUS_TYPE);
        public STATUS_TYPE now
        {
            get { return _now; }
            set
            {
                if (!object.Equals(_now, value))
                {
                    _last = _now;
                    _now = value;

                    if (this.StatusChanged != null)
                    {
                        this.StatusChanged.BeginInvoke(this, null, null);
                    }

                    OnPropertyChanged("now");
                    OnPropertyChanged("last");
                }
                if (this.LinkEvent != null)
                {
                    this.LinkEvent.Invoke(this);
                }

            }
        }

        /// <summary>
        /// 上一时刻的状态
        /// </summary>
        private STATUS_TYPE _last = default(STATUS_TYPE);
        public STATUS_TYPE last
        {
            get { return _last; }
        }

        #endregion

        #region 更新状态方法

        /// <summary>
        /// 更新状态函数
        /// </summary>
        /// <returns></returns>
        public STATUS_TYPE update()
        {
            //STATUS_TYPE tmp;
            //             lock (this._now)
            {
                this.now = this.getStatusFunc();
            }
            return this.now;
        }

        public void reset()
        {
            //写入两次now,以刷新last
            this.now = default(STATUS_TYPE);
            this.now = default(STATUS_TYPE);
        }

        #endregion

        #region 自动更新状态线程方法

        /// <summary>
        /// 自动更新状态线程函数
        /// </summary>
        public System.Threading.AutoResetEvent threakeepCheckEvent = new System.Threading.AutoResetEvent(true);
        public bool threakeepCheckEventOK = true;
        private void keepCheck()
        {

            while (!StopCheckFlag)
            {
                if (threakeepCheckEventOK)
                {
                    this.update();
                }
                else
                {
                    threakeepCheckEvent.WaitOne();
                    threakeepCheckEventOK = true;
                }
                Thread.Sleep(this.checkInterval_ms);
            }
        }

        /// <summary>
        /// 启动自动更新状态线程
        /// </summary>
        public void startCheck(int checkInterval_ms = 3000)
        {
            if (this.keepCheckThread == null)
            {
                this.checkInterval_ms = checkInterval_ms;
                this.StopCheckFlag = false;
                this.keepCheckThread = new Thread(this.keepCheck);
                this.keepCheckThread.IsBackground = true;
                this.keepCheckThread.Start();
            }

        }

        /// <summary>
        /// 停止自动更新状态线程
        /// </summary>
        public void stopCheck()
        {
            if (this.keepCheckThread != null && this.keepCheckThread.IsAlive)
            {
                this.StopCheckFlag = true;
                this.keepCheckThread.Join();
                this.keepCheckThread = null;
                this.checkInterval_ms = 0;
            }
        }

        /// <summary>
        /// 是否已经启动自动检测
        /// </summary>
        /// <returns></returns>
        public bool isChecking()
        {
            return !(this.StopCheckFlag);
        }

        #endregion

        public override string ToString()
        {
            return this.now.ToString();
        }
    }

    #endregion

    #region SxReaderVMl类
    //     public class SxReaderVM : ObservableObject
    //     {
    //         public SxReaderVM()
    //         {
    //             this.messages = new AsyncObservableCollection<MessageVM>();
    //         }
    // 
    //         /// <summary>
    //         /// 读写器类型
    //         /// </summary>
    //         private ReaderTypeEnum _readerType = default(ReaderTypeEnum);
    //         public ReaderTypeEnum readerType
    //         {
    //             get { return _readerType; }
    //             set { setValue(ref _readerType, value, "readerType"); }
    //         }
    // 
    //         /// <summary>
    //         /// 读写器ID
    //         /// </summary>
    //         private int _readerID = 0;
    //         public int readerID
    //         {
    //             get { return _readerID; }
    //             set { setValue(ref _readerID, value, "readerID"); }
    //         }
    // 
    //         /// <summary>
    //         /// 连接地址字符串
    //         /// </summary>
    //         private string _linkAddress = "";
    //         public string linkAddress
    //         {
    //             get { return _linkAddress; }
    //             set { setValue(ref _linkAddress, value, "linkAddress"); }
    //         }
    // 
    //         private string _linkArgument = "";
    //         public string linkArgument
    //         {
    //             get { return _linkArgument; }
    //             set { setValue(ref _linkArgument, value, "linkArgument"); }
    //         }
    // 
    //         /// <summary>
    //         /// 连接建立方式
    //         /// </summary>
    //         private LinkModeEnum _linkMode = default(LinkModeEnum);
    //         public LinkModeEnum linkMode
    //         {
    //             get { return _linkMode; }
    //             set { setValue(ref _linkMode, value, "linkMode"); }
    //         }
    // 
    //         /// <summary>
    //         /// 程序运行状态
    //         /// </summary>
    //         private string _runTimeStatus = "";
    //         public string runTimeStatus
    //         {
    //             get { return _runTimeStatus; }
    //             set { setValue(ref _runTimeStatus, value, "runTimeStatus"); }
    //         }
    // 
    //         /// <summary>
    //         /// 设置新程序运行状态
    //         /// </summary>
    //         /// <param name="stat"></param>
    //         /// <param name="level"></param>
    //         /// <param name="sender"></param>
    //         public void setStatus(string stat, int level = 0, string sender = "")
    //         {
    //             this.runTimeStatus = stat;
    //             //addMessage(stat,level,sender); //死锁！！！
    //         }
    // 
    //         #region 消息列表属性
    // 
    //         /// <summary>
    //         /// 消息列表
    //         /// </summary>
    //         public AsyncObservableCollection<MessageVM> messages { get; set; }
    // 
    //         /// <summary>
    //         /// 消息列表最大保消息数（0为无限制）
    //         /// </summary>
    //         private int _maxMessagesLength = 10;
    //         public int maxMessagesLength
    //         {
    //             get { return _maxMessagesLength; }
    //             set { setValue(ref _maxMessagesLength, value, "maxMessagesLength"); }
    //         }
    // 
    //         #endregion
    // 
    //         #region 消息列表方法
    // 
    //         public void addMessage(string message, int level = 0, string sender = "")
    //         {
    //             MessageVM msg = new MessageVM(message, level, sender);
    //             this.addMessage(msg);
    //         }
    // 
    //         public void addMessage(MessageVM msg)
    //         {
    //             lock (this.messages)
    //             {
    //                 if (this.maxMessagesLength > 0 && this.messages.Count() >= this.maxMessagesLength)
    //                 {
    //                     while (this.messages.Count() >= this.maxMessagesLength)
    //                     {
    //                         this.messages.RemoveAt(0);
    //                     }
    //                 }
    //                 this.messages.Add(msg);
    //             }
    //         }
    // 
    //         #endregion
    //     }
    #endregion

    #region 可视化对象（绑定UI元素内容，更改时自动刷新UI）
    /// <summary>
    /// 可视化对象（绑定UI元素内容，更改时自动刷新UI）
    /// </summary>

    public class ObservableObject : INotifyPropertyChanged
    {

        protected void setValue<T>(ref T property, T value, string propertyName)
        {
            if (object.Equals(property, value))
                return;

            property = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class AsyncObservableObject : INotifyPropertyChanged
    {

        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        protected void setValue<T>(ref T property, T value, string propertyName)
        {
            if (object.Equals(property, value))
                return;

            property = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(propertyName);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                _synchronizationContext.Send(RaisePropertyChanged, propertyName);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            _OnPropertyChanged((string)param);
        }

        protected void _OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    #endregion

    #region 工具
    public class TimeCounter
    {
        DateTime startTime;//开始

        public TimeCounter()
        {
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 开始/重新开始计时（记录开始时间）
        /// </summary>
        public void start()
        {
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 返回开始到当前的毫秒数
        /// </summary>
        /// <returns>开始到当前的毫秒数</returns>
        public double now()
        {
            return (DateTime.Now - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 返回开始到当前的毫秒数的字符串
        /// </summary>
        /// <returns>开始到当前的毫秒数的字符串</returns>
        public override string ToString()
        {
            return (DateTime.Now - startTime).TotalMilliseconds.ToString("0");
        }
    }
    public class HexString
    {
        /// <summary>
        /// 十六进制字符样本，其序号就是其所代表的值
        /// </summary>
        private static string hex_std = "0123456789ABCDEF";
        public byte[] hex;

        public HexString()
        {
            hex = null;
        }

        public HexString(string s)
        {
            if (s == null)
            {
                hex = null;
                return;
            }

            string hexStr = adjust(s);
            if (hexStr != null)
            {
                hex = stringToByte(hexStr);
            }
            else
            {
                hex = null;
            }
        }

        public HexString(string s, int hexLen)
        {
            if (s == null || hexLen * 2 > s.Length)
            {
                hex = null;
                return;
            }

            string hexStr = adjust(s);
            if (hexStr != null)
            {
                hex = stringToByte(hexStr, hexLen);
            }
            else
            {
                hex = null;
            }
        }

        public HexString(byte[] barr)
        {
            if (barr != null && barr.Length > 0)
            {
                hex = new byte[barr.Length];
                Array.Copy(barr, hex, barr.Length);
            }
            else
            {
                hex = null;
            }
        }

        public HexString(byte[] barr, int len)
        {
            if (barr != null && barr.Length > 0 && barr.Length >= len)
            {
                hex = new byte[len];
                Array.Copy(barr, hex, len);
            }
            else
            {
                hex = null;
            }
        }

        public int len()
        {
            if (hex == null)
            {
                return 0;
            }
            else
            {
                return hex.Length;
            }
        }

        public static string adjust(string hexstr)
        {
            hexstr = hexstr.ToUpper();
            string s = "";
            //去除字符串中的非大写十六进制字符
            for (int i = 0; i < hexstr.Length; i++)
            {
                if (hex_std.IndexOf(hexstr[i]) >= 0)
                {
                    s += hexstr[i];
                }
            }
            //检测是否为偶数个字符
            if (s.Length == 0 || s.Length % 2 != 0)
            {
                return null;
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// 截取二进制数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns>如果超出长度，返回null</returns>
        public byte[] getHex(int start, int size)
        {
            if (this.hex.Length < start + size)
            {
                return null;
            }

            byte[] barr = new byte[size];
            for (int i = 0; i < size; i++)
            {
                barr[i] = this.hex[start + i];
            }
            return barr;
        }

        public static byte[] stringToByte(string hexstr, int hexLen = 0)
        {
            byte[] barr;
            string s = adjust(hexstr);
            if (s == null || hexLen * 2 > hexstr.Length)
            {
                return null;
            }

            if (hexLen == 0)
            {
                hexLen = s.Length / 2;

            }

            barr = new byte[hexLen];
            for (int i = 0; i < barr.Length; i++)
            {
                barr[i] = (byte)(hex_std.IndexOf(s[i * 2]) * 16 + hex_std.IndexOf(s[i * 2 + 1]));
            }
            return barr;
        }

        public static string byteToString(byte[] barr, int hexLen = 0)
        {
            string s = "";
            if (barr == null || hexLen > barr.Length)
            {
                return s;
            }
            if (hexLen == 0)
            {
                hexLen = barr.Length;
            }
            for (int i = 0; i < hexLen; i++)
            {
                s += hex_std[((barr[i] >> 4) & 0x0f)];
                s += hex_std[((barr[i]) & 0x0f)];
            }
            return s;
        }


        public override string ToString()
        {
            return byteToString(hex);
        }


        public string ToString(int groupLen, string seperator = " ")
        {
            string s = this.ToString();
            string ret = "";
            int arrLen = this.len();

            //如果分组长度不大于0，则设置为每个byte都加入空格
            if (groupLen <= 0)
            {
                groupLen = 1;
            }

            for (int i = 0; i < arrLen; i++)
            {
                ret += s[2 * i];
                ret += s[2 * i + 1];
                if ((i + 1) % groupLen == 0 && i != arrLen - 1)
                {
                    ret += seperator;
                }
            }
            return ret;
        }
    }
    public class HexASCII
    {
        public byte[] hex;

        public HexASCII()
        {
            hex = null;
        }

        public HexASCII(string s)
        {
            if (s == null)
            {
                hex = null;
                return;
            }

            hex = ASCIIToByte(s);

        }

        public HexASCII(string s, int hexLen)
        {
            if (s == null || hexLen * 2 > s.Length)
            {
                hex = null;
                return;
            }

            hex = ASCIIToByte(s, hexLen);
        }

        public HexASCII(byte[] barr)
        {
            if (barr != null && barr.Length > 0)
            {
                hex = new byte[barr.Length];
                Array.Copy(barr, hex, barr.Length);
            }
            else
            {
                hex = null;
            }
        }

        public HexASCII(byte[] barr, int len)
        {
            if (barr != null && barr.Length > 0 && barr.Length >= len)
            {
                hex = new byte[len];
                Array.Copy(barr, hex, len);
            }
            else
            {
                hex = null;
            }
        }

        public int len()
        {
            if (hex == null)
            {
                return 0;
            }
            else
            {
                return hex.Length;
            }
        }

        public static string byteToASCII(byte[] barr, int hexLen = 0)
        {
            string s = "";
            if (barr == null || hexLen > barr.Length)
            {
                return s;
            }
            if (hexLen == 0)
            {
                hexLen = barr.Length;
            }
            for (int i = 0; i < hexLen; i++)
            {
                s += (char)barr[i];
            }
            return s;
        }

        public static byte[] ASCIIToByte(string asciistr, int hexLen = 0)
        {
            byte[] barr;
            string s = asciistr;
            if (s == null || hexLen > asciistr.Length)
            {
                return null;
            }

            if (hexLen == 0)
            {
                hexLen = s.Length;
            }

            barr = new byte[hexLen];
            for (int i = 0; i < barr.Length; i++)
            {
                barr[i] = (byte)(asciistr[i]);
            }
            return barr;
        }

        public override string ToString()
        {
            return byteToASCII(hex);
        }
    }
    #endregion

    #region 思谷读写器数据类型
    /// <summary>
    /// 读写器连接方式
    /// </summary>
    public enum LinkModeEnum
    {
        TCP_IP = 0,
        RS_232
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public enum LinkStatusEnum
    {
        Unlink = 0,
        Linked = 1,
        Unlinking = 2,
        Linking = 3,
        Unknown = 4
    }

    /// <summary>
    /// 读写器类型
    /// </summary>
    public enum ReaderTypeEnum
    {
        S6 = 6,
        S7 = 7
    }

    /// <summary>
    /// 功率模式
    /// </summary>
    public enum PowerModeEnum
    {
        Half = 0,
        Full = 1
    }

    /// <summary>
    /// 指令有效时间
    /// </summary>
    public enum CommandModeEnum
    {
        UntilSuccess = 0,
        JustOnce = 1,
    }

    /// <summary>
    /// 自动读标签通讯模式
    /// </summary>
    public enum ServiceCommunicationModeEnum
    {
        RS_232 = 0,
        RS_485 = 1,
        TCP_IP = 2
    }

    /// <summary>
    /// 默认读写标签块大小
    /// </summary>
    public enum TagBlockSizeEnum
    {
        Size_4_Bytes = 4,
        Size_8_Bytes = 8
    }

    /// <summary>
    /// 标签操作寻址方式
    /// </summary>
    public enum TagAddressionModeEnum
    {
        None = 0, //不使用寻址
        UID //通过UID寻址
    }

    /// <summary>
    /// 服务状态（自动读标签等）
    /// </summary>
    public enum ServiceStatusEnum
    {
        Closed = 0,
        Opened,
        Closing,
        Opening
    }
    #endregion
}
