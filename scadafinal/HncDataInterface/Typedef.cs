using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HncDataInterfaces
{
    public class HncDataDef
    {
        public const Int32 PAR_PROP_DATA_LEN = 68;
        public const Int32 MAX_FILE_NUM_PER_DIR = 128;
        public const Int32 DATA_STR_LEN = 8;
        public const Int32 AXIS_DATA_LEN = 32;
        public const Int32 SYS_DATA_LEN = 64;
        public const Int32 SAMPLEDATA_MAX_NUM = 10000;
        public const Int32 PROG_NAME_LEN = 60;
        public const Int32 PAR_FILE_NAME_LEN = 16;
        public const Int32 ALARM_DATA_LEN = 64;
        public const Int32 SDATAPROPOTY_LEN = 64;
        public const Int32 SPARAMVALUE_LEN = 8;
        public const Int32 MAX_RESERVE_DATA_LEN = 128;

        public const Int32 PARAMAN_FILE_NCU = 0;				// NC参数
        public const Int32 PARAMAN_FILE_MAC = 1;				// 机床用户参数
        public const Int32 PARAMAN_FILE_CHAN = 2;				// 通道参数
        public const Int32 PARAMAN_FILE_AXIS = 3;				// 坐标轴参数
        public const Int32 PARAMAN_FILE_ACMP = 4;				// 误差补偿参数
        public const Int32 PARAMAN_FILE_CFG = 5;				// 设备接口参数
        public const Int32 PARAMAN_FILE_TABLE = 6;				// 数据表参数
        public const Int32 PARAMAN_FILE_BOARD = 7;				// 主站参数
        public const Int32 PARAMAN_MAX_FILE_LIB = 7;			// 参数结构文件最大分类数
        public const Int32 PARAMAN_MAX_PARM_PER_LIB = 1000;	// 各类参数最大条目数 
        public const Int32 PARAMAN_MAX_PARM_EXTEND = 1000;		// 分支扩展参数最大条目数
        public const Int32 PARAMAN_LIB_TITLE_SIZE = 16;		// 分类名字符串最大长度
        public const Int32 PARAMAN_REC_TITLE_SIZE = 16;		// 子类名字符串最大长度
        public const Int32 PARAMAN_ITEM_NAME_SIZE = 64;		// 参数条目字符串最大长度
        public const Int32 SERVO_PARM_START_IDX = 200;			// 伺服参数起始参数号
        // 0x000F：0,用第1编码器反馈 1,用第2编码器反馈  2,无反馈
        // 0x00F0：0,用第1编码器指令 1,用第2编码器指令
        // 0x0100：0,跟随误差由伺服驱动器反馈 1,跟踪误差由系统计算
        // 0x1000：0,默认采用32位脉冲计数 1,采用64位脉冲计数
        public const Int32 AX_ENCODER_MASK = 0x00FF;
        public const Int32 AX_NC_CMD_MASK = 0x00F0;
        public const Int32 AX_NC_TRACK_ERR = 0x0100;
        public const Int32 AX_NC_CYC64_MODE = 0x1000;
        //
        public const Int32 VAR_SYS_VNUM = 10000;
        public const Int32 VAR_CHAN_VNUM = 2000;
        public const Int32 VAR_AXIS_VNUM = 100;
    };
    public enum PAR_STORE_TYPE
    {
        TYPE_NULL = 0,	//空类型
        TYPE_INT,		//整型
        TYPE_FLOAT,		//实型
        TYPE_EXPR,		//表达式
        TYPE_VAR,		//简单变量
        TYPE_STRING,	//字符串
        TYPE_UINT,		//无符号整型
        TYPE_BOOL,		//布尔型
        TYPE_FUNC,		//函数表达式
        TYPE_ARR,		//数组表达式
        TYPE_HEX4,		//十六进制格式
        TYPE_BYTE		//字节类型
    };
}
