using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using HNCAPI;
namespace HncDataInterfaces
{
    [Serializable]
    public class AxisInfo
    {
        public string axName { get; set; }
        public double actPosition { get; set; }
        public double cmdPosition { get; set; }
        public double actPositionWcs { get; set; }
        public double cmdPositionWcs { get; set; }
        public double actPositionRcs { get; set; }
        public double cmdPositionRcs { get; set; }
        public double leftFeed { get; set; }
        public double loadCur { get; set; }
    }

    [Serializable]
    public class AddAxisInfo
    {
        public string axName { get; set; }
        public double actPosition { get; set; }
        public double cmdPosition { get; set; }
        public double actPositionWcs { get; set; }
        public double cmdPositionWcs { get; set; }
        public double actPositionRcs { get; set; }
        public double cmdPositionRcs { get; set; }
        public double leftFeed { get; set; }
        public double loadCur { get; set; }
        public int electricMacPosition { get; set; }   //电机位置
        public int cmdPluse { get; set; }  //指令脉冲
        public int actPluse { get; set; } //实际脉冲
        public double electricMacRollSpeed { get; set; }//电机转速
        public double waveFrequence { get; set; } //波形频率
        public double programPositon { get; set; }  //编程位置
        public int followError { get; set; }
        public int wcsZero { get; set; }
        public int synError { get; set; }
        public int zPluseOffset { get; set; }
        public double driveCurrent { get; set; }
        public double ratedCurrent { get; set; }
        public double compensation { get; set; }
        public double progPos { get; set; }
    }

    [Serializable]
    public class AxisMotion
    {
        public double actSpeed { get; set; }
        public double cmdSpeed { get; set; }
        public double Currentload { get; set; }
        public double spdlOvrd { get; set; }
        public double feedOvrd { get; set; }
        public double actSpdlSpeed { get; set; }
        public double cmdSpdlSpeed { get; set; }
        public double actFeedSpeed { get; set; }
        public double cmdFeedSpeed { get; set; }
        public double rapidOvrd { get; set; }//快移修调
        public double spdlLoad { get; set; }
    }

    [Serializable]
    public class HncChannelValue
    {
        public int breakPointPosition { get; set; }
        public int cuttingToolUse { get; set; }
        public int GcodeRunRow { get; set; }
        public string GcodeName { get; set; }
        public int toolNO { get; set; }
        public int Cntr { get; set; }//加工计数
        public int Stati { get; set; }  //工件总数
       // public int ToolUse { get; set; }
        public int ToolReady { get; set; }
        public int ToolType { get; set; }
        public int Cycle { get; set; }
        public int Hold { get; set; }
    }

    [Serializable]
    public class MacGcodeFile
    {
        public string filename { get; set; }
        public string filesize { get; set; }
        public string filetime { get; set; }
    }

    [Serializable]
    public class MacState
    {
        public string MacID { get; set; }
        public int State { get; set; }
    }

     [Serializable]
    public class ToolPara
    {
        public int ToolNO { get; set; }
        public int ToolPos { get; set; }
        public int ToolType { get; set; }         
        public double ToolLength { get; set; }
        public double ToolRadius { get; set; }
        public double wToolLength { get; set; }
        public double wToolRad { get; set; }
    }

     [Serializable]
     public class ToolBase
     {
         public int ToolNO { get; set; }
         public int ToolPos { get; set; }
       
     }
    public interface IHncData
    {
        String GetformattedResume();
        String GetformattedName();
    }

     [Serializable]
    public class ListForParaRT
    {
        public int nTotalRecordsNum;
        public List<DataItem_ParaRT> ListForPaData;
    }
    [Serializable]
    public class DataItem_ParaRT
    {
        public string m_sParaCode { get; set; }
        public string m_sParaName { get; set; }
        public string m_sDefaultVal { get; set; }
        public string m_sMaxVal { get; set; }
        public string m_sMinVal { get; set; }
        public int m_sEffectWay { get; set; }
        public string m_sNowVal { get; set; }
        public int m_nDataType { get; set; }
    }
    [Serializable]
    public class paraDataForDiag
    {
        public Int16 m_nBigCls;//大类号
        public string m_sParaCode;//参数号
        public string m_sParaName;//参数名
        public string m_sDefaultVal;//默认值
        public string m_sMaxVal;//最大值
        public string m_sMinVal;//最小值
        public string m_sNowVal;//当前值
    }


    [Serializable]
    public class paraQerTypeForDiag
    {
        public Int16 m_nBigCls;//大类号
        public Int16 m_nSmallCls;//小类号
        public int m_sParaCode;//参数号
    }

    /*Add by leomei 远程下载PLC文件到Web服务器*/
    [Serializable]
    public class PLCFileRemoting
    {
        public string PlcDataName;             //文件的名称
        public byte[] FileByteDatas;           //文件数据缓冲区
        public DateTime PlcFileCreateTimeInSys;//文件创建的时间
        public long FileByteSize;              //文件的字节大小
        public int nFileStatus;                //当前文件信息的一致性，-1--采集器从数控系统读取失败，0--文件不存在，1--文件信息一致(创建时间和文件字节大小)不需要下载，2--最新文件成功，3--新文件存在但是下载失败
        public bool bIsAutoDownLoad;           //如果指定的PlcDataName文件不存在，那么是否自动下载数控系统上已经存在的plc文件,true为
    }

    [Serializable]
    public class CSampDataType
    {
        public DateTime m_dtSample;
        public int m_nGCodeRunRow;
        public int m_nRealSize;
        public float[] m_faSamplData;
        public int m_nTimeInter;//该块点占用时长，以毫秒为单位。
    }

    [Serializable]
    public class CRTDataForWeb
    {
        public List<AxisInfo> axisList;
        public AxisMotion axismovement;
        public HncChannelValue channelValue;
        public int state = 1;//stateItem.State, 
        public string DescText = "运行";//stateItem.DescText,
        public string WorkModeDes = "自动";//workMode.WorkModeDes,
        //public string toolType;
        //public string toolImg;
        public double spdlloar;
        public List<string> Gmodelist;
        public string spdlState;
        public string KeepTimeStr;
        public Dictionary<string, int?> RegvalueArray;
    }

     [Serializable]
    public class GcodeDataForWeb
    {
       
        public int GcodeRunNow;
        public string GcodeName;
        public List<string> Gmodelist;
      
    }

     public enum HCloudReturnCode
     {
         PROCESS_OK = 0, // WebApi函数正确执行(完成成功)
         PROCESS_ERROR,//函数功能执行失败
         PROCESS_WARNING,//函数功能执行完成，有警告(部分成功)
     }

     [Serializable]
     public class WebReturn
     {
         public HCloudReturnCode returnCode;
         public string message;//出错的记录的内容（MacID+时间点）
         public string values; //Json附加键值（成功条数）
     }

    [Serializable]
     public class HealthIndex             //机床健康指数
     {
        public double SpdlHealthIndex { get; set; }    //主轴健康
        public double XaxisHealthIndex { get; set; }   //X轴健康
        public double YaxisHealthIndex { get; set; }   //Y轴健康
        public double ZaxisHealthIndex { get; set; }   //Y轴健康指数
        public double MacHealthIndex{ get; set; }    //机床健康指 
        public double XCurrentCurve { get; set; }   // X轴电流弯度值
        public double XCurrentMean { get; set; }    // X轴电流平均值
        public double XCurrentVib { get; set; }     // X轴电流波动值
        public double YCurrentCurve { get; set; }   // Y轴电流弯度值
        public double YCurrentMean { get; set; }    // Y轴电流平均值
        public double YCurrentVib { get; set; }     // Y轴电流波动值
        public double ZCurrentCurve { get; set; }   // Z轴电流弯度值
        public double ZCurrentMean { get; set; }    // Z轴电流平均值
        public double ZCurrentVib { get; set; }     // Z轴电流波动值
        public double SAccTime { get; set; }        // 主轴加速时间
        public double SDecTime { get; set; }        // 主轴减速时间        
     }



    [Serializable]
    [XmlRoot("SimuData")]
    public class SimuData
    {
        [XmlElement("m_sPosi_X")]
        public string m_sPosi_X { get; set; }
        [XmlElement("m_sPosi_Y")]
        public string m_sPosi_Y { get; set; }
        [XmlElement("m_sPosi_Z")]
        public string m_sPosi_Z { get; set; }
        [XmlElement("m_sFeed")]
        public string m_sFeed { get; set; }
        [XmlElement("m_sSped")]
        public string m_sSped { get; set; }
        [XmlElement("m_sCurtMain")]
        public string m_sCurtMain { get; set; }
    }

     //  [Serializable]
     //public class GetGmodeInfor
     //{
     //    public string Gmode { get; set; }
     ////    public string IsModal { get; set; }
     //    public string Des { get; set; }
     //}
    public interface IHncDataService
    {
        IHncData Getresume();
        //String GetformattedResume();
        List<AxisInfo> GetAxisInfo(string MacID);
        List<AddAxisInfo> GetAddAxisInfo(string MacID);
        AxisMotion GetAxisMotion(string MacID);
        HncChannelValue GetHncChannelValue(string MacID);
        List<MacGcodeFile> GetMacGcodeFile(string MacID);
        List<ToolPara> GetToolPara(string MacID);
        List<ToolBase> GetToolBase(string MacID);
        bool SendGcodeFile(string MacID, string GcodeName, string GcodeContent);
        string GetGcodeContent(string MacID, string GcodeName);
        List<string> GetGmode(string MacID);
        int? GetToolData(int toolNO, int QerToolPara, string QermacID);
        int paraGetVal(int fileNo, int recNo, string ParaCode, short clientNo, ref string sParaVal);
        int GetParaFormMac(int fileNo, int recNo, string ParaCode, string sMacID, ref string sParaVal);
        ListForParaRT GetParaValRealtime(int fileNo, int recNo, int nPageNum, int nRecordPerPage, int nTotalRecordsIndex, string macID);
        int Save2DBForPara_FromIF(string macID, string sVerTitle);
        int GetNowMachineState(string sMacID);
        string GetSysValue(string sMacID, HncSystem nType);
        string[] GetSysValue(string sMacID, HncSystem[] naType);
        GcodeDataForWeb GetGcodeData(string MacID);
        CRTDataForWeb GetSummaryData(string MacID);
        bool DownLoadPLCData(string MacID, string PlcDataName);
        PLCFileRemoting DownLoadPLCData2WevServer(string MacID, PLCFileRemoting svrPLCInfo);
        /**
         * 函数功能：获取数控系统上一个寄存器的值。
         * 参数：  MacID，数控装置的MacID编号;
         *         strRegType,寄存器的类型，分别有X、Y、F、G、R、W、D、B、P ，此参数类型为这9中类型中的一种，格式为"X"或者"x","Y"或者"y";
         *         nIndex,与参数strRegType配合使用，某种寄存器的编号,
         *                 取值范围  X : 0--511 
         *                           Y : 0--511 
         *                           B : 0--1721 
         *                           F : 0--3119 
         *                           G : 0-- 3119 
         *                           R : 0--399 
         *                           W : 0--199 
         *                           D : 0--99 
         *                           P : 0--199 
         * 返回值：取值成功则返回寄存器的数值，失败则返回null;
         *          返回值的范围，  X 寄存器，8位
         *                          Y 寄存器，8位
         *                          F 寄存器，16位
         *                          G 寄存器，16位
         *                          R 寄存器，8位 
         *                          W 寄存器，16位 
         *                          D 寄存器，32位 
         *                          B 寄存器，32位
         *                          P 寄存器，32位
         * 备注：由于返回值都是整型，所以取值之后应该将其转化到寄存器的取值范围。
         **/
        int? GetRegValueOne(string MacID, string strRegType, int nIndex);

        //实时批量采样获取数据接口
        CSampDataType[] GetSamplData(string MacID, string[] SamplAxis, HncSampleType SamplType);

        //开启写文件批量采样接口 返回值0->成功，-1->MacID空错误，-2->采样频率，块大小配置出差，-3->调用StartSampl出错
        int StartSmplRecord(string MacID, string[] SamplAxis, HncSampleType SamplType);
        //获取写文件批量采样状态，返回值-2->开启采样失败，-1->参数非法，0->默认，未开启采样功能,1->正在执行采样，2->与实时显示同步
        int GetSamplState(string MacID);
        //停止写文件批量采样， 0->成功，非0->错误代码
 //del_leomei       int StopSamplRecord(string MacID);
        /**
         * 函数功能：获取数控系统上一组寄存器的值。
         * 参数：  MacID，数控装置的MacID编号;
         *         strRegType,寄存器的类型，分别有X、Y、F、G、R、W、D、B、P ，此参数类型为这9中类型中的一种，格式为"X"或者"x","Y"或者"y";
         *         LstIndex,与参数strRegType配合使用，某种寄存器的编号链表
         *                 取值范围  X : 0--511 
         *                           Y : 0--511 
         *                           B : 0--1721 
         *                           F : 0--3119 
         *                           G : 0-- 3119 
         *                           R : 0--399 
         *                           W : 0--199 
         *                           D : 0--99 
         *                           P : 0--199 
         * 返回值：取值成功则返回一组寄存器的数值，失败则返回null;
         *          返回值的范围，  X 寄存器，8位
         *                          Y 寄存器，8位
         *                          F 寄存器，16位
         *                          G 寄存器，16位
         *                          R 寄存器，8位 
         *                          W 寄存器，16位 
         *                          D 寄存器，32位 
         *                          B 寄存器，32位
         *                          P 寄存器，32位
         * 备注：由于返回值都是整型链表，所以取值之后应该将其转化到寄存器的取值范围。
         **/
        List<int?> GetRegsValuesMore(string MacID, string strRegType, List<int> LstIndex);
        //-------------------------------给手机小组接口----------------------start-------------------------------
        List<string> GetGcodeInfo_ForArd(string MacID); //by zb
        UInt32 GetGcodeRunRow(string MacID);
        //-------------------------------给手机小组接口----------------------end---------------------------------


        //为诊断页面提供接口 zb
        /*
         * 本函数实时读取数控系统所需查询的参数内容信息，并返回包含参数信息的链表。
         * 输入：strMacID->待查询的数控系统标识
         *       lstQerPara->链表类型，包含待查询的【大类号，小类号，参数号】三个查询要求。
         * 输出：paraDataForDiag类型的链表，链表数据项包括【参数大类号，参数号，参数名，当前值，默认值，最小值，最大值】
         */
        List<paraDataForDiag> getParaForDiag(string strMacID, List<paraQerTypeForDiag> lstQerPara);
        //------------------------------给创景数据接口----------------------------start
        SimuData[] GetSimuDataSet(string sMacID);
        //------------------------------给创景数据接口----------------------------end
        bool SendSampXmlData2NC(string sMacID, string sGCodeName, DateTime dtXmlFileName, HncDataInterfaces.HealthIndex cHealthDat);


        #if RemotoController //远程控制，为了安全起见，请慎重使用

        /**
         *  以下代码仅供苏州富强科技有限公司企业展会临时采集程序使用
         *  2016-01-22
         */
        //----------------------------------------------------------

        int SysCtrlStop(string MacID);
        int SysCtrlReady(string MacID);
        int SysCtrlRun(string MacID, string GCodeName);

        //----------------------------------------------------------
        #endif
    }
}