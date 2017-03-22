using System;

namespace Sygole.HFReader
{
    public enum WorkMode_enum : byte { OPERATOR_MODE = 0x00, AUTO_MODE = 0x01 };
    public enum RFChipPower_enum : byte { HALF_POWER = 0x00, FULL_POWER = 0x01 };
    public enum NeedBCC_enum : byte { NO_NEED_BCC = 0x00, NEED_BCC = 0x01 };
    public enum CommPort_enum : byte { RS232 = 0x00, RS485 = 0x01,TCP = 0x02 };
    public enum BlockSize_enum : byte { SIZE_4B = 0x04, SIZE_8B = 0x08 };
    public enum AvailableTime_enum : byte { CONTINUANCE = 0x00, REAL_TIME = 0x01 };
    public class UserCfg
    {
        public WorkMode_enum WorkMode{ get; set; }						// 工作模式 
        public byte ReaderID { get; set; }						        // 读写器ID 
        public RFChipPower_enum RFChipPower { get; set; }				// 功率 
        public NeedBCC_enum NeedBCC { get; set; }						// BCC校验 
        public CommPort_enum CommPort { get; set; }					    // 通讯接口(自动读卡时返回数据接口) 
        public BlockSize_enum BlockSize { get; set; }					// 单个块的大小 典型Icode为4，富士通为8
        public AvailableTime_enum AvailableTime { get; set; }			// 指令有效时间 
        public bool[] AntStatus = new bool[4] {false,false,false,false }; // 自动读卡时四个天线的状态，部分读写器不支持

        public const byte ID_NOCHANGE = 0x00;
        public const byte USERCFGSIZE = 0x07;
        //其他读写器ID值

        public UserCfg()
        {
            this.WorkMode = WorkMode_enum.OPERATOR_MODE;
            this.ReaderID = 0x00;
            this.RFChipPower = RFChipPower_enum.FULL_POWER;
            this.NeedBCC = NeedBCC_enum.NEED_BCC;
            this.CommPort = CommPort_enum.RS232;
            this.BlockSize = BlockSize_enum.SIZE_4B;
            this.AvailableTime = AvailableTime_enum.CONTINUANCE;
        }

        public UserCfg(WorkMode_enum mode, byte ID, RFChipPower_enum power, NeedBCC_enum needbcc, CommPort_enum port, BlockSize_enum size, AvailableTime_enum time)
        {
            this.WorkMode = mode;
            this.ReaderID = ID;
            this.RFChipPower = power;
            this.NeedBCC = needbcc;
            this.CommPort = port;
            this.BlockSize = size;
            this.AvailableTime = time;
        }

        public static byte[] ConvUserCfgToBytes(UserCfg cfg)
        {
            byte[] data = new byte[USERCFGSIZE];
            byte AntStat = 0;

            int pos = 0;

            data[pos++] = (byte)cfg.WorkMode;
            data[pos++] = cfg.ReaderID;
            data[pos++] = (byte)cfg.RFChipPower;
            data[pos++] = (byte)cfg.NeedBCC;
            data[pos++] = (byte)cfg.CommPort;

            
            for (int i = 0; i < cfg.AntStatus.Length; i++)
            {
                AntStat |= (byte)((cfg.AntStatus[i] ? 1 : 0) << i);
            }
            if (AntStat == 0)
            {
                data[pos++] = (byte)cfg.BlockSize;
            }
            else
            {
                data[pos++] = AntStat;
            }

            data[pos++] = (byte)cfg.AvailableTime;

            return data;
        }

        public void ConvBytesToUserCfg(byte[] data)
        {
            int pos = 0;
            this.WorkMode = (WorkMode_enum)data[pos++];
            this.ReaderID = data[pos++];
            this.RFChipPower = (RFChipPower_enum)data[pos++];
            this.NeedBCC = (NeedBCC_enum)data[pos++];
            this.CommPort = (CommPort_enum)data[pos++];
            for (int i = 0; i < AntStatus.Length; i++)
            {
                AntStatus[i] = (((data[pos] >> i) & 0x01) == 1);
            }
            this.BlockSize = (BlockSize_enum)data[pos++];
            this.AvailableTime = (AvailableTime_enum)data[pos++];
        }
    }

    public class CommCfg
    {
        public static int MAC_LEN { get { return 6; } }
        public string IPAddr { get; set; }
        public string Mask { get; set; }
        public string GateWay { get; set; }
        public byte[] MAC = new byte[MAC_LEN];
        public static int PORT_1 { get { return 3001; } }

        public static bool ValidCfgPara(CommCfg cfg)
        {
            try
            {
                System.Net.IPAddress.Parse(cfg.IPAddr);
                System.Net.IPAddress.Parse(cfg.Mask);
                System.Net.IPAddress.Parse(cfg.GateWay);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static byte[] ConvUserCfgToBytes(CommCfg cfg)
        {
            try
            {
                if (!ValidCfgPara(cfg))
                {
                    return null;
                }
                byte[] data = new byte[20];

                byte[] data_temp = tool.IpStringToByte(cfg.IPAddr);
                Array.Copy(data_temp, 0, data, 0, 4);

                data_temp = tool.IpStringToByte(cfg.Mask);
                Array.Copy(data_temp, 0, data, 4, 4);

                data_temp = tool.IpStringToByte(cfg.GateWay);
                Array.Copy(data_temp, 0, data, 8, 4);

                Array.Copy(cfg.MAC, 0, data, 12, 6);

                return data;
            }
            catch { return null; }
        }

        public void ConvBytesToUserCfg(byte[] data)
        {
            this.IPAddr = tool.ByteToIpString(data, 0);
            this.Mask = tool.ByteToIpString(data, 4);
            this.GateWay = tool.ByteToIpString(data, 8);
            Array.Copy(data, 12, this.MAC, 0, 6);
        }
    }
}
