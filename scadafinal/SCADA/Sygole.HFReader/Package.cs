using System;

namespace Sygole.HFReader
{
    public class Package
    {
        public const byte HEADER_POS = 0;
        public const byte CMD_POS = 1;
        public const byte ID_POS = 2;
        public const byte STATUS_POS = 3;
        public const byte OPCODE_POS = 4;
        public const byte DATALEN_POS = 5;
        public const byte DATA_POS = 6;
        public const byte BCC_POS = 7;

        public Package()
        {
            this.CMD = CmdEnum.CMD_GET_USER_CONFIG;
            this.ReaderID = 0x00;
            this.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            this.Ant = Antenna_enum.ANT_1;
            this.DataLen = 0x00;
        }

        public Package(CmdEnum cmd, byte id)
        {
            this.CMD = cmd;
            this.ReaderID = id;
            this.States = 0x00;
            this.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            this.Ant = Antenna_enum.ANT_1;
            this.DataLen = 0x00;
            this.BCC = 0x00;
        }

        public Package(CmdEnum cmd, byte id,Antenna_enum ant)
        {
            this.CMD = cmd;
            this.ReaderID = id;
            this.States = 0x00;
            this.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            this.Ant = ant;
            this.DataLen = 0x00;
            this.BCC = 0x00;
        }

        public Package(CmdEnum cmd, byte id, byte[] data, byte offset, byte len)
        {
            this.CMD = cmd;
            this.ReaderID = id;
            this.States = 0x00;
            this.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            this.Ant = Antenna_enum.ANT_1;
            this.DataLen = len;
            Array.Copy(data, offset, this.Datas, 0, len);
            this.BCC = tool.GetBCC(this.Datas, 0,this.DataLen);
        }

        public Package(CmdEnum cmd, Opcode_enum opcode, byte[] uid, byte id, byte[] data, byte offset, byte len)
        {
            int pos = 0;
            this.CMD = cmd;
            this.ReaderID = id;
            this.States = 0x00;
            if((uid != null) && (uid.Length >= 8))
            {
                this.Opcode = opcode;
            }
            else
            {
                this.Opcode = Opcode_enum.NON_ADDRESS_MODE;
            }
            this.Ant = Antenna_enum.ANT_1;
            if (this.Opcode == Opcode_enum.ADDRESS_MODE)
            {
                this.DataLen = (byte)(len + 8);
                Array.Copy(uid, 0, this.Datas, pos, 8);
                pos += 8;
            }
            else
            {
                this.DataLen = len;
            }
            Array.Copy(data, offset, this.Datas, pos, len);
            this.BCC = tool.GetBCC(this.Datas, 0, this.DataLen);
        }

        public static byte Header { get { return 0x1B; } }
        public CmdEnum CMD { get; set; }
        public byte ReaderID { get; set; }
        public byte States{ get; set; }
        public Opcode_enum Opcode { get; set; }
        public Antenna_enum Ant { get; set; }
        public byte DataLen{ get; set; }
        public byte[] Datas = new byte[128];
        public byte BCC { get; set; }


        /// <summary>
        /// 将包转换为数组
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static byte[] ConvPackageToArray(Package package)
        {
            byte[] data = new byte[package.DataLen + 7];
            int pos = 0;

            data[pos++] = Package.Header;
            data[pos++] = (byte)package.CMD;
            data[pos++] = package.ReaderID;
            data[pos++] = package.States;
            data[pos++] = (byte)((byte)package.Opcode | (((byte)package.Ant << 2))); 
            data[pos++] = package.DataLen;
            Array.Copy(package.Datas, 0, data, pos, package.DataLen);
            pos += package.DataLen;
            data[pos++] = tool.GetBCC(package.Datas, 0,package.DataLen);

            return data;
        }

        /// <summary>
        /// 将
        /// </summary>
        /// <returns></returns>
        public static Package ConvArrayToPackage(byte[] data, int len)
        {
            if (data[HEADER_POS] != Header)
            {
                return null;
            }

            if ((len < BCC_POS) || (len < data[DATALEN_POS] + 7) || (data.Length < data[DATALEN_POS] + 7))
            {
                return null;
            }

            if (tool.GetBCC(data, DATA_POS, data[DATALEN_POS]) != data[BCC_POS + data[DATALEN_POS] - 1])
            {
                return null;
            }

            Package package = new Package();
            int pos = 1;

            package.CMD = (CmdEnum)data[pos++];
            package.ReaderID = data[pos++];
            package.States = data[pos++];
            package.Opcode = (Opcode_enum)(data[pos] & 0x3);
            package.Ant = (Antenna_enum)((data[pos++] >> 2) & 0x3);
            package.DataLen = data[pos++];
            Array.Copy(data, pos, package.Datas, 0, package.DataLen);
            pos += package.DataLen;
            package.BCC = data[pos++];

            return package;
        }

        public void copy(Package pack)
        {
            if (pack != null)
            {
                this.ReaderID = pack.ReaderID;
                this.States = pack.States;
                this.Opcode = pack.Opcode;
                this.Ant = pack.Ant;
                this.DataLen = pack.DataLen;
                Array.Copy(pack.Datas, this.Datas, pack.DataLen);
                this.BCC = pack.BCC;
                this.CMD = pack.CMD;
            }
        }
    }

    public enum CmdEnum : byte
    {
         CMD_INVENTORY = 0x01,
         CMD_ANTI_COLLISION = 0x03,
         CMD_SET_USER_CONFIG = 0x1C,
         CMD_GET_VERSION = 0x1E,
         CMD_READ_SBLOCK = 0x20,
         CMD_WRITE_SBLOCK = 0x21,
         CMD_LOCK_SBLOCK = 0x22,
         CMD_READ_MBLOCKS = 0x23,
         CMD_WRITE_MBLOCK = 0x24,
         CMD_SELECT_TAG = 0x25,
         CMD_RESET_TO_READY = 0x26,
         CMD_GET_VICC_INFO = 0x2B,
         CMD_GET_SECURITY = 0x2C,
         CMD_RESET_MCU = 0xAC,
         CMD_GPO_CTRL = 0xA9,
         CMD_SET_AUTO_READ_FUNC = 0xA0,
         CMD_GET_AUTO_READ_FUNC = 0xA1,
//         CMD_PROGRAM_CIPHER_TEXT = 0xA3,
//         CMD_SET_TAG_CHECK_RULE = 0xA2,
         CMD_DEFAULT_URSE_CFG = 0x1A,
         CMD_GPI_TRRIGER = 0xAA,
         CMD_GET_USER_CONFIG = 0xAE,
         CMD_SET_POWER_LEVEL = 0xBE,
         CMD_GET_POWER_LEVEL = 0xBF,
         CMD_GET_PIN_LEVEL = 0xA4,
         CMD_GET_TCP_CFG = 0xB0,
         CMD_SET_TCP_CFG = 0xB1,
    }

    public class CommArgs
    {
        public byte ReaderID { get; set; }
        public string addr { get; set; }
        public int port { get; set; }
    }

    public class AutoReadEventArgs : EventArgs
    {
        public AutoReadEventArgs()
        {
            comm = new CommArgs();
        }
        public byte[] UID = new byte[8];
        public CommArgs comm { get; set; }
        public Antenna_enum ant { get; set; }
    }

    public class GPITriggerEventArgs : EventArgs
    {
        public GPITriggerEventArgs()
        {
            comm = new CommArgs();
        }
        public GpiEnum Gpi { get; set; }
        public CommArgs comm { get; set; }

    }

    public class CommEventArgs : EventArgs
    {
        public CommEventArgs()
        {
        }
        public byte[] CommDatas = new byte[128];
        public int CommDatasLen { get; set; }
        public CommArgs comm { get; set; }
    }

    public enum GpiEnum : byte { GPI_0 = 0x00, GPI_1 = 0x01, GPI_2 = 0x02, GPI_3 = 0x03, GPI_4 = 0x04 }
}
