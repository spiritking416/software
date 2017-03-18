using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;
using System.Collections;   //zb:20150427
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.Linq.SqlClient;
using System.Text;
using HncDataInterfaces;
using hncData;
using System.Configuration;
using System.Threading;
using System.Net;
using System.Xml;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Timers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HNCBase.Models;
using System.Data.SQLite;   //zb:20150424
using Newtonsoft.Json;  //zb:20150428
using System.Runtime.InteropServices; //zb:20150610
//----------------------------------------
using System.Transactions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.ServiceModel.Security;

using System.IO.Compression;
using System.Text.RegularExpressions;
namespace HncDACore
{
    //定义Alarm链表数据项
    public class ListData_Alarm
    {
        public DateTime CreateTime;
        public int ID;
        public string detail;
        //zb-start
        public int alarmNo;//报警编号，只是从接口中把编号读出，无法完成解析。
        //zb-end
    }
    public struct LogData_Alarm
    {
        public List<ListData_Alarm> LogContent;
    }
    public class MachineState
    {
        public string MacID
        { get; set; }
        public string IP
        { get; set; }
        public ushort Port
        { get; set; }
        public short ClientNO
        { get; set; }
        public bool Connected
        { get; set; }
        public int PanelGet
        { get; set; }
        public int EventGet
        { get; set; }
        public int FilechgeGet
        { get; set; }
        public int WorkGet
        { get; set; }
        public int m_nMachineState { get; set; } // 本条添加zb:20141229
        public DateTime m_dtCreaTimeOfState;//状态产生的时间
        public string m_sMachineSNCode { get; set; } // 本条添加zb:20150205
        public int m_bSamplChFlag;//已被占用的采样通道标志，zb：20150703
        public int m_nSamplStatFlagForFile;//采样功能正处于的状态标志，-2->开启采样失败，-1->采样超时退出，0->默认，未开启采样功能,1->正在执行采样,2->通过实时采样写文件
        public int m_nSamplStatFlag;//采样功能正处于的状态标志，-2->开启采样失败，-1->采样超时退出，0->默认，未开启采样功能,1->正在执行采样

        public string m_sAxisName;//轴名
        public string m_sAxisNo;//轴号
        public string m_sRunGCode;//正在运行的G代码名字

        public string m_sNCVer;//机床正在使用的NC版本

        public NCLogAutoDownloadConfig m_cNCLogConfig { get; set; } // 机床下载日志配置信息，本条添加zb:20151015
    }

    public class CSampleData
    {
        public int[] m_anXVal;
        public int[] m_anYVal;
        public int[] m_anZVal;
    }
    //----------------------------------重写Timer类：zb20150610------------------------------start
    public class TaskTimer : System.Timers.Timer
    {
        public DateTime m_dtStartSamplTime;//存储开始采样的时间
        public int m_nStartSamplGCodeRow;//存储开始采样的G代码行号
        public int[][] m_aSampData;//多采集的采样点缓存区
        public int[] m_naSamplCh;//该机床使用的采样通道
        public string[] m_saSamplAxis;//采样的轴名
        public int[] m_nSampIndex;//采样缓冲期的索引值
        public int m_nSampFreq;//nc采样频率
        public string m_sMacID;//机床标示符
        public string m_sProgName;//G代码名字
        public string m_sFileCreateTime;//采样开始时间
        public double[] m_faDevCur;//轴对应的驱动单元电流值
        public int m_nFileBlockSize;//存储文件的块的大小，以存储点的个数为单位
        public int[] m_naTotalPoint;//每个轴文件存储的点的个数
        public double[] m_daMaxValue;//每个轴文件采样最大值
        public double[] m_daRMS;//每个轴算术平均值
        public double[] m_daAverage;//每个轴平均值
        public string m_sUsedSamplCh;//使用了哪些采样通道
        public Queue<CRTSampDataType>[] m_queArrRTData;
        public Queue<CRTSampDataType>[] m_queArrRTDataForFile;
        public bool m_bIsFirstRunTimer;
        public TaskTimer() : base()
        {
        }
    }
    //----------------------------------重写Timer类：zb20150610------------------------------end
    public class CSampDataType
    {
        public DateTime m_dtSample;
        public int m_nGCodeRunRow;
        public int m_nRealSize;
        public float[] m_faSamplData;
        public int m_nTimeInter;//该块点占用时长，以毫秒为单位。
    }
    public class CRTSampDataType
    {
        public CSampDataType m_cSampDataType;
        public string m_sAxisName;
        public CRTSampDataType()
        {

        }
    }
    public class CSampTimeAndRows
    {
        public DateTime m_dtSampTime;
        public int m_nGCodeRow;
    }
    public class NCLogAutoDownloadConfig
    {
        public bool PANEL2Dld;
        public bool WORK2Dld;
        public bool EVENT2Dld;
        public bool FILECHANGE2Dld;
        public bool ALARM2Dld;
    }
    /// <summary>
    /// 链表加锁操作对象
    /// </summary>
    public class MacStateLogList : IEnumerable<MacStateLog>
    {
        object syncObject = new object();        
        public void AddLog(MacStateLog macStateLog)
        {
            lock (syncObject)
            {
                HncDACore.Monitor.MacStateList.Add(macStateLog);
            }
        }
        public void AddLogRange(List<MacStateLog> macStatelist)
        {
            lock (syncObject)
            {
                HncDACore.Monitor.MacStateList.AddRange(macStatelist);
            }
        }
        public void RemoveLogRange(List<MacStateLog> macStatelist)
        {
            lock (syncObject)
            {
                if (HncDACore.Monitor.MacStateList.Count >= macStatelist.Count)
                {

                }
                HncDACore.Monitor.MacStateList.RemoveRange(0, macStatelist.Count);
            }
        }
        public void RemoveCount(int count)
        {
            lock (syncObject)
            {
                if (HncDACore.Monitor.MacStateList.Count >=count)
                {
                HncDACore.Monitor.MacStateList.RemoveRange(0, count);
                }
            }
        }
        public void ClearLogAll()
        {
            lock (syncObject)
            {
                HncDACore.Monitor.MacStateList.Clear();
            }
        }
        public void RemoveLog()
        {
            lock (syncObject)
            {
                if (HncDACore.Monitor.MacStateList.Count > 0)
                {
                    HncDACore.Monitor.MacStateList.RemoveAt(0);
                }
            }
        }
        public IEnumerator<MacStateLog> GetEnumerator()
        {
            lock (syncObject)
            {
                foreach (var v in HncDACore.Monitor.MacStateList)
                {
                    yield return v;
                }
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

