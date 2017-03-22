using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScadaHncData
{
    [Serializable]
    public class PLCData
    {
        public MONITOR_INFO MONITOR_INFO_node = new MONITOR_INFO();
        public PLCData(MONITOR_INFO MONITOR_INFO_node)
        {
            this.MONITOR_INFO_node = MONITOR_INFO_node;
        }
    }

    [Serializable]
    public class AGVData
    {
        public AGV_MONITOR_INFO AGV_MONITOR_INFO_node = new AGV_MONITOR_INFO();
        public AGVData(AGV_MONITOR_INFO AGV_MONITOR_INFO_node)
        {
            this.AGV_MONITOR_INFO_node = AGV_MONITOR_INFO_node;
        }
    }

    [Serializable]
    public class RGVData
    {
        public RGV_LOCATION RGV_LOCATION_node = new RGV_LOCATION();
        public RGVData(RGV_LOCATION RGV_LOCATION_node)
        {
            this.RGV_LOCATION_node = RGV_LOCATION_node;
        }
    }


    #region 设备运行状态，上报
    [Serializable]
    public class EQUIP_STATE
    {
        public string EQUIP_CODE { get; set; } // VARCHAR2(50),设备ID
        public string EQUIP_CODE_CNC { get; set; } // VARCHAR2(50),cnc:SN号
        public string EQUIP_STATE_Column { get; set; } // VARCHAR2(10),设备状态，字符串表述
        public decimal? STATE_VALUE { get; set; } // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
        public DateTime? SWTICH_TIME { get; set; } // DATE,时间戳
        public sbyte? FLAG { get; set; } // NUMBER (1,0)
        public decimal? SN { get; set; } // NUMBER
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
        public int EQUIP_TYPE { get; set; } //设备类型，0:CNC,1:ROBOT,2:PLC,3:RFID,4:RGV
    }
    #endregion
    #region 组盘信息，将小料盘和大料盘关联，上报
    [Serializable]
    public class TRAY_GROUP
    {
        public string BTRAY_ID { get; set; } // VARCHAR2(50)
        public string LINE { get; set; } // VARCHAR2(50)
        public decimal? STRAY_QTY { get; set; } // NUMBER
        public string STRAY_ID { get; set; } // VARCHAR2(50)
        public string STRAY_TYPE { get; set; } // VARCHAR2(50)
        public sbyte? IS_DONE { get; set; } // NUMBER (1,0)
        public sbyte? FLAG { get; set; } // NUMBER (1,0)
        public decimal? SN { get; set; } // NUMBER
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
    }
    #endregion

    #region 所有设备动作事件，上报
    [Serializable]
    public class MONITOR_INFO
    {
        public string EQUIP_CODE { get; set; } // VARCHAR2(50)
        public string ACTION_ID { get; set; } // VARCHAR2(50)
        public DateTime? HAPPEN_TIME { get; set; } // DATE
        public sbyte? FLAG { get; set; } // NUMBER (1,0)
        public decimal? SN { get; set; } // NUMBER
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
    }

    [Serializable]
    public class AGV_MONITOR_INFO
    {
        public string EQUIP_CODE { get; set; } // VARCHAR2(50)
        public string ACTION_ID { get; set; } // VARCHAR2(50)
        public DateTime? HAPPEN_TIME { get; set; } // DATE
        public sbyte? FLAG { get; set; } // NUMBER (1,0)
        public decimal? SN { get; set; } // NUMBER
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
    }

    // RGV小车实时位置，上报

    [Serializable]
    public class RGV_LOCATION
    {
        public string EQUIP_CODE { get; set; } // VARCHAR2(510)
        public string WC_CODE { get; set; } // VARCHAR2(510)
        public string RGV_LOCATION_Column { get; set; } // VARCHAR2(100)
        public sbyte FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public string MARK_DATE { get; set; } // VARCHAR2(10)
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(3)
        public sbyte? STATE { get; set; } // NUMBER (1,0)
        public DateTime? HAPPEN_TIME { get; set; } // DATE
        public decimal? RGV_SPEED { get; set; } // FLOAT(126)
    }


    #endregion
}