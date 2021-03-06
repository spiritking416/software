using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum TimePeriod
    {
    	HEARTBEAT_TIME = 0,// 心跳周期
    	HEARTBEAT_TIMEOUT_COUNT,	   // 心跳超时周期
    	API_TIMEOUT_TIME,              // 默认接口超时时间
    	PARMSAVE_TIMEOUT_TIME,         // 参数保存超时时间
    	PARMSAVEAS_TIMEOUT_TIME,       // 参数另存为超时时间
    	BACKUP_TIMEOUT_TIME,           // 备份超时时间
    	UPDATE_TIMEOUT_TIME,           // 升级超时时间
    	FILE_CHECK_TIMEOUT_TIME,       // 文件校验超时时间
    };

    public class HNCNET
    {
       public const Int32 VERSION_LEN =  32 ;
    }
}
