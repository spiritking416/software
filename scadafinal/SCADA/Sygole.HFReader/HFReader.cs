using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

namespace Sygole.HFReader
{
    public enum Status_enum : byte
    {
        //reader
        SUCCESS = 0x00,
        FAILURE = 0x80,
        NO_TAG_ERROR = 0x90,
        BCC_ERROR = 0xA0,
        BLOCK_SIZE_ERROR = 0xB0,
        TIMEOUT_ERROR = 0xC0,
        GPO_PORT_ERROR = 0xD0,
        //自定义
        NO_RESPONSE = 0xF1,
        PARAM_ERR = 0xF2,
        SERIAL_CLOSED = 0xF3,
    };
    public enum Opcode_enum : byte
    {
        NON_ADDRESS_MODE = 0x00,
        ADDRESS_MODE = 0x01,
        SELECT_MODE = 0x02
    };
    public enum Antenna_enum : byte
    {
        ANT_1 = 0x00,
        ANT_2 = 0x01,
        ANT_3 = 0x02,
        ANT_4 = 0x03
    };
    public enum PowerLevelEnum : byte
    {
        POWER_LEVEL_1W = 0,
        POWER_LEVEL_2W = 1,
        POWER_LEVEL_3W = 2,
        POWER_LEVEL_4W = 3
    }
    public enum GpoEnum : byte { GPO_1 = 0x01, GPO_2 = 0x02, GPO_3 = 0x03, GPO_4 = 0x04 }
    /*
    public enum EncryptRuleEnum : byte { RULE_1 = 0x1, RULE_2 = 0x2 }
    public enum EncryptRuleSetEnum : byte { NONE = 0, RULE_1 = 0x1, RULE_2 = 0x2, ALL = 0x03 }
    */
    public enum AutoReadFuncEnum : byte
    {
        READ_UID = 0, 
        READ_USER_MEMOREY = 1,
        ANTI_COLLISION = 2,
        CUSTOMIZATION = 0xF0
    }
    public enum ConnectStatusEnum { CONNECTED, DISCONNECTED, CONNECTING, CONNECTLOST }
    public delegate void EventHandler<AutoReadEventArgs>(object sender, AutoReadEventArgs Args);
    public class HFReader
    {
        public event EventHandler<AutoReadEventArgs> AutoReadHandler;
        public event EventHandler<GPITriggerEventArgs> GPITriggerHandler;
        public event EventHandler<CommArgs> ConnectLost;

        public static readonly string Version = "Sygole.HFReaderV20150313";
        private Communication commu;
        private string AddrStr = "com1";
        private int port_baud = 3001;
        const int UIDLEN = 8;
        bool InvReqFlg = false;
        byte tempbyte;
        bool AntiReqFlg = false;
        AntiColliResult AntiResult = new AntiColliResult();
        private int DEFAULT_TIMEOUT = 350;
        private int SET_CFG_TIMEOUT = 500;
        private int ANTI_COLLISION_TIMEOUT = 1500;
        Package Resp = new Package();
        public CommEventCS CommEvent;

        private ConnectStatusEnum _ConnectStatus { get; set; }
        public ConnectStatusEnum ConnectStatus { get { return _ConnectStatus; } }

        // 已经被清理过的标记 
        private bool _alreadyDisposed = false;

        public HFReader()
        {

        }

        ~HFReader()
        {
            Dispose();
        }
        // IDisposable的实现，禁止Finalization（终结操作）
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // 避免多次处理 
            if (_alreadyDisposed)
            {
                return;
            }

            try
            {
                StopHeartBeat();
                commu.Dispose();
            }
            catch
            {
            }

            // 设置衍生类的被处理过标记 
            _alreadyDisposed = true;
        }

        #region 连接
        /*public bool SerialPortOpen(string COM)
        {
            return SerialPortOpen(COM, 115200);
        }*/
        
        //打开串口
        public bool SerialPortOpen(string COM, int baud)
        {
            try
            {
                Connect(COM,baud);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //关闭串口
        public bool SerialPortClose()
        {
            DisConnect();
            return true;
        }

        public void DisConnect()
        {
            StopHeartBeat();
            if (commu != null)
            {
                commu.Dispose();
                commu = null;
            }
            _ConnectStatus = ConnectStatusEnum.DISCONNECTED;
        }

        /*public bool Connect(string addr)
        {
            if (addr.ToUpper().Contains("COM"))
            {
                return Connect(addr, (int)115200);
            }
            else
            {
                return Connect(addr, (ushort)3001);
            }
        }*/

        public bool Connect(string addr, ushort port)
        {
            return Connect(addr, (int)port);
        }

        public bool Connect(string addr, int baud)
        {
            port_baud = baud;
            AddrStr = addr.ToUpper();

            DisConnect();

            _ConnectStatus = ConnectStatusEnum.CONNECTING;
            if (commu == null)
            {
                if (AddrStr.Contains("COM"))
                {
                    commu = new COM();
                }
                else
                {
                    commu = new TCP();
                }
            }

            if (commu.Connect(AddrStr, port_baud))
            {
                commu.dealDataEvent += new Communication.DealDataEvent(RespDataProcess);
                _ConnectStatus = ConnectStatusEnum.CONNECTED;
                return true;
            }
            else
            {
                commu.Dispose();
                commu = null;
                _ConnectStatus = ConnectStatusEnum.DISCONNECTED;
                return false;
            }
        }

        #endregion

        #region 发送，等待响应

        private void RespInit()
        {
            lock (Resp)
            {
                Resp.CMD = 0x00;
            }
        }

        private bool SendPackage(Package package)
        {
            byte[] datas = Package.ConvPackageToArray(package);

            if ((datas != null) && (commu != null))
            {
                //此处作了修改2015-11-25
                //把对commu的所和null的判断靠近
                //但这仍然是一个非现成安全的函数
                lock (commu)
                {
                    if (CommEvent != null)
                    {
                        CommEvent.SendHandler(datas, datas.Length);
                    }
                    return commu.SendData(datas, 0, datas.Length);
                }
            }

            return false;
        }

        private Status_enum WaitResp(CmdEnum CMD, ref byte[] data, ref byte len)
        {
            return WaitResp(CMD, ref data, ref len, DEFAULT_TIMEOUT);
        }

        const int checkInterval = 5;
        private Status_enum WaitResp(CmdEnum CMD, ref byte[] data, ref byte len, int timeout)
        {
            int CheckTime = timeout / checkInterval;

            RespInit();
            while (CheckTime-- != 0)//等待接收
            {
                lock (Resp)
                {
                    if (Resp.CMD == CMD)
                    {
                        if (Resp.States == 0x00)
                        {
                            for (int i = 0; i < Resp.DataLen; i++)
                            {
                                if (data == null)
                                {
                                    break;
                                }
                                data[i] = Resp.Datas[i];
                            }
                            len = Resp.DataLen;
                        }
                        break;
                    }
                }
                System.Threading.Thread.Sleep(checkInterval);
            }
            return CheckTime > 0 ? (Status_enum)Resp.States : Status_enum.NO_RESPONSE;
        }
        #endregion 

        #region 命令执行
        //盘存
        public Status_enum Inventory(byte ReaderID, ref byte[] UID)
        {
            return Inventory(ReaderID, ref UID, Antenna_enum.ANT_1);
        }
        public Status_enum Inventory(byte ReaderID, ref byte[] UID,Antenna_enum ant)
        {
            if (UID.Length < UIDLEN)
            {
                return Status_enum.PARAM_ERR;
            }
            try
            {
                InvReqFlg = true;
                Package package = new Package(CmdEnum.CMD_INVENTORY, ReaderID,ant);

                if (SendPackage(package))
                {
                    return WaitResp(package.CMD, ref UID, ref tempbyte);
                }
                else
                {
                    return Status_enum.SERIAL_CLOSED;
                }
            }
            finally
            {
                InvReqFlg = false;
            }
        }

        //读单块
        public Status_enum ReadSBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte Block, ref byte[] BlockDatas, ref byte len)
        {
            return ReadSBlock(ReaderID, Opcode, UID, Block, ref BlockDatas, ref len, Antenna_enum.ANT_1);
        }
        public Status_enum ReadSBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte Block, ref byte[] BlockDatas, ref byte len, Antenna_enum ant)
        {
            /*            if (BlockDatas.Length < 4)
                        {
                            return Status_enum.PARAM_ERR;
                        }
                        */
            byte[] data = new byte[] { Block };
            Package package = new Package(CmdEnum.CMD_READ_SBLOCK,Opcode,UID, ReaderID, data, 0, (byte)data.Length);
            package.Ant = ant;

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref BlockDatas, ref len);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //写单块
        public Status_enum WriteSBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte Block, int len, byte[] BlockDatas)
        {
            return WriteSBlock(ReaderID, Opcode, UID, Block, len, BlockDatas, Antenna_enum.ANT_1);
        }
        public Status_enum WriteSBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte Block, int len, byte[] BlockDatas, Antenna_enum ant)
        {
            byte[] temp = null;

            //单块大小为4字节或8字节
            if (((len != 4) && (len != 8)) || (BlockDatas.Length < len))
            {
                return Status_enum.PARAM_ERR;
            }

            Package package = new Package(CmdEnum.CMD_WRITE_SBLOCK, ReaderID, ant);

            if ((UID != null) && (UID.Length >= 8))
            {
                package.Opcode = Opcode;
            }
            else
            {
                package.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            }
            if (package.Opcode == Opcode_enum.ADDRESS_MODE)
            {
                Array.Copy(UID, 0, package.Datas, package.DataLen, 8);
                package.DataLen += 8;
            }
            package.Datas[package.DataLen++] = Block;
            Array.Copy(BlockDatas, 0, package.Datas, package.DataLen, len);
            package.DataLen += (byte)len;
            package.BCC = tool.GetBCC(package.Datas, 0, package.DataLen);

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //锁单块
        public Status_enum LockSBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte Block)
        {
            return LockSBlock(ReaderID, Opcode, UID, Block, Antenna_enum.ANT_1);
        }
        public Status_enum LockSBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte Block,Antenna_enum ant)
        {
            byte[] temp = null;

            byte[] data = new byte[] { Block };
            Package package = new Package(CmdEnum.CMD_LOCK_SBLOCK,Opcode,UID, ReaderID, data, 0, (byte)data.Length);
            package.Ant = ant;

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //读多块
        public Status_enum ReadMBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, ref byte[] datas,ref byte len)
        {
            return ReadMBlock(ReaderID, Opcode, UID, StartBlock, BlockCnt, ref datas, ref len, Antenna_enum.ANT_1);
        }
        public Status_enum ReadMBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, ref byte[] datas, ref byte len,Antenna_enum ant)
        {
            byte[] data = new byte[] { StartBlock, BlockCnt };
            Package package = new Package(CmdEnum.CMD_READ_MBLOCKS,Opcode,UID, ReaderID, data, 0, (byte)data.Length);
            package.Ant = ant;
            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref datas, ref len);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //写多块
        public Status_enum WriteMBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, int BlockSize, byte[] BlockDatas)
        {
            return WriteMBlock(ReaderID, Opcode, UID, StartBlock, BlockCnt, BlockSize, BlockDatas, Antenna_enum.ANT_1);
        }
        public Status_enum WriteMBlock(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, int BlockSize, byte[] BlockDatas, Antenna_enum ant)
        {
            byte[] temp = null;

            //单块大小为4字节或8字节
            if (((BlockSize != 4) && (BlockSize != 8))
                || (BlockDatas.Length < BlockCnt * BlockSize)
                || (BlockCnt > 8))
            {
                return Status_enum.PARAM_ERR;
            }

            Package package = new Package(CmdEnum.CMD_WRITE_MBLOCK, ReaderID, ant);
            if ((UID != null) && (UID.Length >= 8))
            {
                package.Opcode = Opcode;
            }
            else
            {
                package.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            }
            if (package.Opcode == Opcode_enum.ADDRESS_MODE)
            {
                Array.Copy(UID, 0, package.Datas, package.DataLen, 8);
                package.DataLen += 8;
            }
            package.Datas[package.DataLen++] = StartBlock;
            package.Datas[package.DataLen++] = BlockCnt;
            Array.Copy(BlockDatas, 0, package.Datas, package.DataLen, BlockCnt * BlockSize);
            package.DataLen += (byte)(BlockCnt * BlockSize);
            package.BCC = tool.GetBCC(package.Datas, 0, package.DataLen);
            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //选择标签
        //UID必须为完整UID
        public Status_enum SelectTag(byte ReaderID, byte[] UID)
        {
            byte[] temp = null;

            Package package = new Package(CmdEnum.CMD_SELECT_TAG, ReaderID);

            if (SendPackage(package))
            {
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //复位标签
        public Status_enum ResetTag(byte ReaderID, Opcode_enum Opcode, byte[] UID)
        {
            byte[] temp = null;
            Package package = new Package(CmdEnum.CMD_RESET_TO_READY, ReaderID);

            if (SendPackage(package))
            {
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //获取标签系统信息
        public Status_enum GetTagInfo(byte ReaderID, Opcode_enum Opcode, byte[] UID_I,ref byte InformationFlag,
            ref byte[] UID_O,ref byte DSFID, ref byte AFI,ref byte BlockCnt, ref byte BlockSize, ref byte ICReference )
        {
            byte[] temp = new byte[14];

            if (UID_O.Length < UIDLEN)
            {
                return Status_enum.PARAM_ERR;
            }

            Package package = new Package(CmdEnum.CMD_GET_VICC_INFO, ReaderID);
            if ((UID_I != null) && (UID_I.Length >= 8))
            {
                package.Opcode = Opcode;
            }
            else
            {
                package.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            }
            if (package.Opcode == Opcode_enum.ADDRESS_MODE)
            {
                Array.Copy(UID_I, 0, package.Datas, package.DataLen, 8);
                package.DataLen += 8;
                package.BCC = tool.GetBCC(package.Datas, 0, package.DataLen);
            }
            if (SendPackage(package))
            {
                Status_enum status = WaitResp(package.CMD, ref temp, ref tempbyte);
                if (status == Status_enum.SUCCESS)
                {
                    int pos = 0;
                    InformationFlag = temp[pos++];
                    for (int i = 0; i < UIDLEN; i++)
                    {
                        UID_O[i] = temp[pos++];
                    }
                    DSFID = temp[pos++];
                    AFI = temp[pos++];
                    BlockCnt = temp[pos++];
                    BlockSize = temp[pos++];
                    ICReference = temp[pos++];
                }
                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }
        public Status_enum GetTagInfo(byte ReaderID, Opcode_enum Opcode, byte[] UID, ref TagInfo info,Antenna_enum ant)
        {
            byte[] temp = new byte[14];

            Package package = new Package(CmdEnum.CMD_GET_VICC_INFO, ReaderID,ant);
            if ((UID != null) && (UID.Length >= 8))
            {
                package.Opcode = Opcode;
            }
            else
            {
                package.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            }
            if (package.Opcode == Opcode_enum.ADDRESS_MODE)
            {
                Array.Copy(UID, 0, package.Datas, package.DataLen, 8);
                package.DataLen += 8;
                package.BCC = tool.GetBCC(package.Datas, 0, package.DataLen);
            }
            if (SendPackage(package))
            {
                Status_enum status = WaitResp(package.CMD, ref temp, ref tempbyte);
                if (status == Status_enum.SUCCESS)
                {
                    if (info == null) info = new TagInfo();
                    int pos = 0;
                    info.InformationFlag = temp[pos++];
                    for (int i = 0; i < UIDLEN; i++)
                    {
                        info.UID[i] = temp[pos++];
                    }
                    info.DSFID = temp[pos++];
                    info.AFI = temp[pos++];
                    info.BlockCnt = temp[pos++];
                    info.BlockSize = temp[pos++];
                    info.ICrefcerence = temp[pos++];
                }
                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //获取标签块安全状态
        public Status_enum GetTagSecurity(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, ref byte[] TagSecurity)
        {
            return GetTagSecurity(ReaderID, Opcode, UID, StartBlock, BlockCnt, ref TagSecurity, Antenna_enum.ANT_1);
        }
        public Status_enum GetTagSecurity(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, ref byte[] TagSecurity, Antenna_enum ant)
        {
            if (TagSecurity.Length < BlockCnt)
            {
                return Status_enum.PARAM_ERR;
            }

            byte[] data = new byte[] { StartBlock, BlockCnt };
            Package package = new Package(CmdEnum.CMD_GET_SECURITY,Opcode,UID, ReaderID, data, 0, (byte)data.Length);
            package.Ant = ant;
            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref TagSecurity, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //设置用户配置
        public Status_enum SetUserCfg(byte ReaderID, UserCfg cfg)
        {
            byte[] temp = null;
            byte[] cfg_array = UserCfg.ConvUserCfgToBytes(cfg);
            Package package = new Package(CmdEnum.CMD_SET_USER_CONFIG, ReaderID, cfg_array, 0, (byte)cfg_array.Length);

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte, SET_CFG_TIMEOUT);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //查看用户配置
        public Status_enum GetUserCfg(byte ReaderID,ref UserCfg cfg)
        {
            byte[] temp = new byte[UserCfg.USERCFGSIZE];
            Package package = new Package(CmdEnum.CMD_GET_USER_CONFIG, ReaderID);

            if (SendPackage(package))
            {
                //接收
                Status_enum status = WaitResp(package.CMD, ref temp, ref tempbyte);

                if ((status == Status_enum.SUCCESS) && (tempbyte == UserCfg.USERCFGSIZE))
                {
                    cfg.ConvBytesToUserCfg(temp);
                }
                else
                {
                    status = Status_enum.FAILURE;
                }

                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //设置网络配置参数
        public Status_enum SetCommCfg(byte ReaderID, CommCfg cfg)
        {
            byte[] temp = null;
            byte[] cfg_array = CommCfg.ConvUserCfgToBytes(cfg);

            if (cfg_array == null)
            {
                return Status_enum.PARAM_ERR;
            }

            Package package = new Package(CmdEnum.CMD_SET_TCP_CFG, ReaderID, cfg_array, 0, 12);

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte, SET_CFG_TIMEOUT);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //查看网络配置参数
        public Status_enum GetCommCfg(byte ReaderID, ref CommCfg cfg)
        {
            byte[] temp = new byte[20];
            Package package = new Package(CmdEnum.CMD_GET_TCP_CFG, ReaderID);

            if (SendPackage(package))
            {
                //接收
                Status_enum status = WaitResp(package.CMD, ref temp, ref tempbyte);

                if (status == Status_enum.SUCCESS)
                {
                    if (cfg == null)
                    { cfg = new CommCfg(); }
                    cfg.ConvBytesToUserCfg(temp);
                }
                else
                {
                    status = Status_enum.FAILURE;
                }

                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //查看软件版本号
        public Status_enum GetSWVer(byte ReaderID, ref byte[] SoftVer,ref byte len)
        {
            Package package = new Package(CmdEnum.CMD_GET_VERSION, ReaderID);

            if (SendPackage(package))
            {
                return WaitResp(package.CMD, ref SoftVer, ref len);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //软复位
        public Status_enum SoftReset(byte ReaderID)
        {
            Package package = new Package(CmdEnum.CMD_RESET_MCU, ReaderID);

            if (SendPackage(package))
            {
                return Status_enum.SUCCESS;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //恢复默认配置(包括ReaderID)
        public Status_enum DefaultCfg(byte ReaderID)
        {
            Package package = new Package(CmdEnum.CMD_DEFAULT_URSE_CFG, ReaderID);

            if (SendPackage(package))
            {
                return Status_enum.SUCCESS;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }



        public Status_enum GetGpiStatus(byte ReaderID, ref byte GpiStatus, ref byte cnt)
        {
            Package package = new Package(CmdEnum.CMD_GET_PIN_LEVEL, ReaderID);

            if (SendPackage(package))
            {
                byte[] temp = new byte[2];

                Status_enum status = WaitResp(package.CMD, ref temp, ref cnt);
                if (status == Status_enum.SUCCESS)
                {
                    cnt = temp[0];
                    GpiStatus = temp[1];
                }
                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        public Status_enum CheckStatus()
        {
            byte cnt = new byte();
            Package package = new Package(CmdEnum.CMD_GET_PIN_LEVEL, ReaderID);
            if (SendPackage(package))
            {
                byte[] temp = new byte[2];
                Status_enum status = WaitResp(package.CMD, ref temp, ref cnt);
                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }


        //GPO控制
        public Status_enum GPOCtrl(byte ReaderID, byte GPO, bool connect)
        {
            byte[] temp = null;

            byte[] data = new byte[] { GPO, (byte)(connect ? 0x01 : 0x00) };
            Package package = new Package(CmdEnum.CMD_GPO_CTRL, ReaderID, data, 0, (byte)data.Length);

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //GPO控制
        public Status_enum GPOCtrl(byte ReaderID, GpoEnum GPO, bool connect)
        {
            return GPOCtrl(ReaderID, (byte)GPO, connect);
        }

        //设置功率
        public Status_enum SetPowerLevel(byte ReaderID, PowerLevelEnum PowerLevel)
        {
            byte[] temp = null;

            byte[] data = new byte[] { (byte)PowerLevel };
            Package package = new Package(CmdEnum.CMD_SET_POWER_LEVEL, ReaderID, data, 0, (byte)data.Length);

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //查看功率
        public Status_enum GetPowerLevel(byte ReaderID, ref PowerLevelEnum PowerLevel)
        {
            byte[] temp = new byte[2];

            Package package = new Package(CmdEnum.CMD_GET_POWER_LEVEL, ReaderID);

            if (SendPackage(package))
            {
                //接收
                Status_enum status = WaitResp(package.CMD, ref temp, ref tempbyte);
                if (status == Status_enum.SUCCESS)
                {
                    PowerLevel = (PowerLevelEnum)temp[0];
                }
                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //设置自动读取的功能
        public Status_enum SetAutoReadFunc(byte ReaderID, AutoReadFuncEnum func, byte block)
        {
            byte[] temp = null;

            byte[] data = new byte[] { 0x00, (byte)(func), block };
            Package package = new Package(CmdEnum.CMD_SET_AUTO_READ_FUNC, ReaderID, data, 0, (byte)data.Length);

            if (SendPackage(package))
            {
                //接收
                return WaitResp(package.CMD, ref temp, ref tempbyte);
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        //获取自动读取的功能
        public Status_enum GetAutoReadFunc(byte ReaderID, ref AutoReadFuncEnum func, ref byte block)
        {
            byte[] temp = new byte[3];

            Package package = new Package(CmdEnum.CMD_GET_AUTO_READ_FUNC, ReaderID);

            if (SendPackage(package))
            {
                //接收
                Status_enum status = WaitResp(package.CMD, ref temp, ref tempbyte);
                if (status == Status_enum.SUCCESS)
                {
                    func = (AutoReadFuncEnum)temp[1];
                    block = temp[2];
                }
                return status;
            }
            else
            {
                return Status_enum.SERIAL_CLOSED;
            }
        }

        public Status_enum AntiCollision(byte ReaderID, List<Antenna_enum> AntList, ref AntiColliResult result)
        {
            byte[] temp = new byte[1]{0};

            foreach (Antenna_enum ant in AntList)
            {
                temp[0] |= (byte)(1 << (int)ant);
            }

            Package package = new Package(CmdEnum.CMD_ANTI_COLLISION, ReaderID, temp, 0, 1);

            AntiReqFlg = true;
            AntiResult.UidList.Clear();
            if (SendPackage(package))
            {
                //接收
                Status_enum status = WaitResp(package.CMD, ref temp, ref tempbyte, ANTI_COLLISION_TIMEOUT);
                result = new AntiColliResult(AntiResult);
                AntiReqFlg = false;
                AntiResult.UidList.Clear();
                return status;
            }
            else
            {
                AntiReqFlg = false;
                return Status_enum.SERIAL_CLOSED;
            } 
        }
#endregion

        #region 接收
        //帧数据的处理函数
        //此函数将对数据组成基本这结构交给UHFReader处理
        private void RespDataProcess(byte[] data, int length)
        {
            Package resp = Package.ConvArrayToPackage(data, length);

            if (CommEvent != null)
            {
                CommEvent.ReceiveHandler(data, length);
            }

            CmdProcess(resp);
        }

        //对解出的帧，分命令进行处理
        private void CmdProcess(Package resp)
        {
            switch (resp.CMD)
            {
                case CmdEnum.CMD_INVENTORY:
                    if ((InvReqFlg == false) && (AntiReqFlg == false) && (AutoReadHandler != null) && (resp.DataLen == UIDLEN))
                    {
                        AutoReadEventArgs args = new AutoReadEventArgs();
                        args.comm.ReaderID = resp.ReaderID;
                        args.comm.addr = AddrStr;
                        args.comm.port = port_baud;
                        args.ant = resp.Ant;
                        for (int i = 0; i < UIDLEN; i++)
                        {
                            args.UID[i] = resp.Datas[i];
                        }
                        AutoReadHandler.BeginInvoke(this, args, null, null);
                        return;
                    }
                    else if ((AntiReqFlg == true) && (resp.DataLen == UIDLEN))
                    {
                        AntiResult.add(resp.Datas, resp.Ant);
                        return;
                    }
                    break;
                case CmdEnum.CMD_GPI_TRRIGER:
                    if (GPITriggerHandler != null)
                    {
                        GPITriggerEventArgs args = new GPITriggerEventArgs();
                        args.comm.ReaderID = resp.ReaderID;
                        args.comm.addr = AddrStr;
                        args.comm.port = port_baud;
                        if (resp.DataLen == 0)//兼容前期未返回端口号的读写器软件
                        {
                            args.Gpi = GpiEnum.GPI_1;
                        }
                        else
                        {
                            args.Gpi = (GpiEnum)resp.Datas[0];
                        }
                        GPITriggerHandler.BeginInvoke(this, args, null, null);
                    }
                    return;
                default:
                    break;
            }
            lock (Resp)
            {
                if (!WaitHeartBeatCmdResp || resp.CMD != HeartBeatCmd)
                {
                    Resp.copy(resp);
                }
                else
                {
                    HeartBeatResp.copy(resp);
                }
            }
        }
        #endregion

        #region HeartBeat
        Thread HeartBeatThread = null;
        bool HeartBeatThreadExit = true;
        const int HeartBeatMissedCnt_MAX = 3;
        byte ReaderID = 0x00;
        const CmdEnum HeartBeatCmd = CmdEnum.CMD_GET_PIN_LEVEL;
        bool WaitHeartBeatCmdResp = false;
        Package HeartBeatResp = new Package();
        public void SetReaderID(int ReaderID)
        {
            this.ReaderID = byte.Parse(ReaderID.ToString());
        }

        public bool StartHeartBeat()
        {
            UserCfg cfg = new UserCfg();
            if (GetUserCfg(0x00, ref cfg) == Status_enum.SUCCESS)
            {
                ReaderID = cfg.ReaderID;
            }
            else
            {
                return false;
            }

            if (HeartBeatThreadExit && ((HeartBeatThread == null) || (HeartBeatThread.IsAlive == false)))
            {
                HeartBeatThreadExit = false;
                HeartBeatThread = new Thread(HeartBeatThreadFunc);
                HeartBeatThread.IsBackground = true;
                HeartBeatThread.Start();
            }

            return true;
        }

        public void StopHeartBeat()
        {
            HeartBeatThreadExit = true;
        }

        private Status_enum WaitHeartBeatResp(CmdEnum CMD, ref byte[] data, ref byte len, int timeout)
        {
            int CheckTime = timeout / checkInterval;

            lock (HeartBeatResp)
            {
                HeartBeatResp.CMD = 0x00;
            }

            while (CheckTime-- != 0)//等待接收
            {
                lock (HeartBeatResp)
                {
                    if (HeartBeatResp.CMD == CMD)
                    {
                        if (HeartBeatResp.States == 0x00)
                        {
                            for (int i = 0; i < HeartBeatResp.DataLen; i++)
                            {
                                if (data == null)
                                {
                                    break;
                                }
                                data[i] = HeartBeatResp.Datas[i];
                            }
                            len = HeartBeatResp.DataLen;
                        }
                        break;
                    }
                }
                System.Threading.Thread.Sleep(checkInterval);
            }
            return CheckTime > 0 ? (Status_enum)HeartBeatResp.States : Status_enum.NO_RESPONSE;
        }

        bool ExecHeartBeat()
        {
            byte gpiStatus = 0, gpiCnt = 0;
            int i = 0;

            for (i = 0; i < 2; i++)
            {
                try
                {
                    WaitHeartBeatCmdResp = true;
                    Package package = new Package(CmdEnum.CMD_GET_PIN_LEVEL, ReaderID);

                    if (SendPackage(package))
                    {
                        byte[] temp = new byte[2];

                        Status_enum status = WaitHeartBeatResp(package.CMD, ref temp, ref tempbyte, DEFAULT_TIMEOUT);
                        if (status == Status_enum.SUCCESS)
                        {
                            gpiCnt = temp[0];
                            gpiStatus = temp[1];
                        }
                        return true;
                    }
                }
                finally
                {
                    WaitHeartBeatCmdResp = false;
                }
            }

            return false;
        }

        void HeartBeatThreadFunc(object obj)
        {
            int HeartBeatMissedCnt = 0;

            while (!HeartBeatThreadExit)
            {
                System.Threading.Thread.Sleep(1000);

                if (ExecHeartBeat())
                    HeartBeatMissedCnt = 0;
                else
                    HeartBeatMissedCnt++;

                if(HeartBeatMissedCnt >= HeartBeatMissedCnt_MAX)
                {
                    _ConnectStatus = ConnectStatusEnum.CONNECTLOST;
                    StopHeartBeat();
                    if (ConnectLost != null)
                    {
                        CommArgs args = new CommArgs();
                        args.ReaderID = 0;
                        args.addr = AddrStr;
                        args.port = port_baud;
                        ConnectLost.BeginInvoke(this, args, null, null);
                    }
                }
            }
        }
        #endregion
    }
}
