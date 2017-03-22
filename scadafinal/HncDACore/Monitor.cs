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
using System.Transactions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.ServiceModel.Security;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using log4net;
using HNCAPI;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log.xml", Watch = true)]
namespace HncDACore
{
    public partial class Monitor
    {


        private AutoResetEvent _EventThConnectNc = new AutoResetEvent(false);
        private Thread _ThConnectNc=null;

        private delegate void InitHandler();
        public delegate void RemoveDBReadyHandler();
        public event RemoveDBReadyHandler OnRemoveDBReady;
        private static pgData db=null;
        private int CmpID;
        private ushort AgentRemotingPort;
        private string AgentCode;
        private ushort LocalPort;
        private string LocalIP;
        private string m_sMacStaLogOperator;
        private bool m_bWriteNCLog = false;
        private AutoResetEvent _EventAutoRst = new AutoResetEvent(false);
        private volatile static List<MachineState> _ConnectionList = new List<MachineState>();
        private static List<MacStateLog> _macStateList = new List<MacStateLog>(); //存储机床状态全局链表
        private static MacStateLogList _macStateLogList = new MacStateLogList();//加锁对象      
        private TcpServerChannel channel = null;
        private SQLiteDatabase m_SqliteDB = null;
        private bool _RemoteDBIsOK;
        private static bool m_bOffDatIsAllUpdated = false;
        private System.Timers.Timer WebServiceTimer = null;
        private static Session globalSession;
        public static Session GlobalSession
        {
            get
            {
                if (globalSession == null)
                {
                    globalSession = SessionManager.Instance().CreateSession();
                }
                return globalSession;
            }

        }
        public  static List<MachineState> ConnectionList
        {
            get
            {
                return _ConnectionList;
            }

            set
            {
                _ConnectionList = value;
            }
        }
        public static List<MacStateLog> MacStateList
        {
            get
            {
                return _macStateList;
            }

            set
            {
                _macStateList = value;
            }
        }
        public static MacStateLogList macStateLogList
        {
            get
            {
                return _macStateLogList;
            }

            set
            {
                _macStateLogList = value;
            }
        }
        public bool RemoteDBIsReady 
        {
            get
            {
                return _RemoteDBIsOK;
            }

            set
            {
                _RemoteDBIsOK = value;
                if (_RemoteDBIsOK && OnRemoveDBReady != null)
                    OnRemoveDBReady();
            }
        }
        public static bool OffDatIsAllUpdated
        {
            get
            {
                return m_bOffDatIsAllUpdated;
            }

            set
            {
                m_bOffDatIsAllUpdated = value;
            }
        }
        public void CloseSession()
        {
            if (globalSession != null)
            {
                globalSession.Close();
            }
        }
        public static bool InitSession(string strLocalIP, ushort nLocalPort)
        {
            if ((null == strLocalIP) || strLocalIP.Equals(""))
            {
                return false;
            }

/*
            //_dataLock = new object();
            int ret = SessionManager.Instance().InitNet(strLocalIP, nLocalPort);

            if (0 != ret)
            {
                return false;
            }
*/
            return true;
        }
        public Monitor()
        {
          
        }
        public void Start() 
        {
            AgentCode = System.Configuration.ConfigurationManager.AppSettings["AgentCode"];
            AgentRemotingPort = ushort.Parse(System.Configuration.ConfigurationManager.AppSettings["CNCLocalPort"]);

            LocalIP = System.Configuration.ConfigurationManager.AppSettings["CNCLocalIP"];
            LocalPort = ushort.Parse(System.Configuration.ConfigurationManager.AppSettings["CNCLocalPort"]);
            m_sMacStaLogOperator = System.Configuration.ConfigurationManager.AppSettings["Operator"] + "_";
            m_bWriteNCLog = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["bWriteNCLog"]);
            OutputMessage2Console("程序启动中...\n");
            OutputMessage2Console("采集器编码AgentCode:" + AgentCode + ";采集器IP:" + System.Configuration.ConfigurationManager.AppSettings["sIPAddressForDataAgent"] + ";采集端口号 Port:" + LocalPort.ToString() + "；远程端口号RemotePort:" + AgentRemotingPort.ToString() + "\n");

            InitHandler handler = new InitHandler(Init);
            handler.BeginInvoke(new AsyncCallback(InitFinished), "初始化成功!!!");



        } 
        //输入：refPgData 服务器端数据库引用，bIsInsert 是插入操作，objDataElemt 如果是插入操作->插入的数据，否则，传入null.
        public static bool SaveToDBIsOK(ref pgData refPgData, bool bIsInsert, object objDataElemt)
        {
            try
            {
                if (bIsInsert && null != objDataElemt)
                {
                    switch (objDataElemt.GetType().Name.ToString())
                    {
                        case "CncMC": refPgData.AddToCncMC((CncMC)objDataElemt); break;
                        case "Company": refPgData.AddToCompany((Company)objDataElemt); break;
                        case "DCAgent": refPgData.AddToDCAgent((DCAgent)objDataElemt); break;
                        case "Fault": refPgData.AddToFault((Fault)objDataElemt); break;
                        case "fault_diagnosis": refPgData.AddTofault_diagnosis((fault_diagnosis)objDataElemt); break;
                        case "FaultCause": refPgData.AddToFaultCause((FaultCause)objDataElemt); break;
                        case "FaultLocation": refPgData.AddToFaultLocation((FaultLocation)objDataElemt); break;
                        case "FaultRepair": refPgData.AddToFaultRepair((FaultRepair)objDataElemt); break;
                        case "FaultType": refPgData.AddToFaultType((FaultType)objDataElemt); break;
                        case "Gcode": refPgData.AddToGcode((Gcode)objDataElemt); break;
                        case "GcodeAppMac": refPgData.AddToGcodeAppMac((GcodeAppMac)objDataElemt); break;
                        case "GcodeDesc": refPgData.AddToGcodeDesc((GcodeDesc)objDataElemt); break;
                        case "Gcodestate": refPgData.AddToGcodestate((Gcodestate)objDataElemt); break;
                        case "hncuser": refPgData.AddTohncuser((hncuser)objDataElemt); break;
                        case "Holidays": refPgData.AddToHolidays((Holidays)objDataElemt); break;
                        case "Lathe": refPgData.AddToLathe((Lathe)objDataElemt); break;
                        case "MachineStructure": refPgData.AddToMachineStructure((MachineStructure)objDataElemt); break;
                        case "MachineType": refPgData.AddToMachineType((MachineType)objDataElemt); break;
                        //case "MacStateLog": refPgData.AddToMacStateLog((MacStateLog)objDataElemt); break;
                        case "Miller": refPgData.AddToMiller((Miller)objDataElemt); break;
                        case "solution": refPgData.AddTosolution((solution)objDataElemt); break;
                        case "NcLogs": refPgData.AddToNcLogs((NcLogs)objDataElemt); break;
                        case "users": refPgData.AddTousers((users)objDataElemt); break;
                        case "WorkingTime": refPgData.AddToWorkingTime((WorkingTime)objDataElemt); break;
                        case "WorkModeDic": refPgData.AddToWorkModeDic((WorkModeDic)objDataElemt); break;
                        case "Workshop": refPgData.AddToWorkshop((Workshop)objDataElemt); break;
                        case "NCMachines": refPgData.AddToNCMachines((NCMachines)objDataElemt); break;
                        case "NCModel": refPgData.AddToNCModel((NCModel)objDataElemt); break;
                        case "NCParameter": refPgData.AddToNCParameter((NCParameter)objDataElemt); break;
                        case "NCParameterDict": refPgData.AddToNCParameterDict((NCParameterDict)objDataElemt); break;
                        case "NCParaVer": refPgData.AddToNCParaVer((NCParaVer)objDataElemt); break;
                        case "Performance": refPgData.AddToPerformance((Performance)objDataElemt); break;
                        case "Software": refPgData.AddToSoftware((Software)objDataElemt); break;
                        case "GCodeSample": refPgData.AddToGCodeSample((GCodeSample)objDataElemt); break;
                        case "GCodeAxisSample": refPgData.AddToGCodeAxisSample((GCodeAxisSample)objDataElemt); break;
                        default: break;
                    }
                }
                refPgData.SaveChanges();
            }
            catch
            {
                //RemoteDBIsReady2 = false;
                return false;
            }
            return true;
        }
        #region 私有函数专区
        private void Init()
        {
            String lip = LocalIP.Clone().ToString();
            if (InitSession(lip, LocalPort))
            {

                InitLocalDB();//初始化离线数据库；webapi认证；上传离线数据。
                InitConnection();//填充机床数据链表；更新机床列表数据到离线数据库；.NetRemoting调用远程对象服务初始化

                // ConnectMachinePosi(true);//采集器主动连机床，更新机床状态MacStateLog，更新机床列表ConnectionList //delete 20160623
                //InitExtFunTimer();
                //WebServiceTimer.Start();//启动定时器 

                
            }
            else
            {
                OutputMessage2Console("初始化本地Session对象失败\n");
            }
        }
        private void InitFinished(IAsyncResult result)
        {
            InitHandler handler = (InitHandler)((AsyncResult)result).AsyncDelegate;
            handler.EndInvoke(result);
            Console.WriteLine(result.AsyncState);
            //           InitWebService();//初始化向服务器报告定时器状态定时器 //delete 20160723

            //_ThConnectNc = new Thread(TheadConnectNcFunc);//leomei 20160723,依旧使用Session _MachineDic静态变量方式
            //_ThConnectNc.Start();
        }

        public void StopThreadConnect()
        {
            _EventThConnectNc.Reset();
            if (null == _ThConnectNc)
            {
                _ThConnectNc.Join();
                _ThConnectNc = null;
            }
        }
        
        private void TheadConnectNcFunc()
        {

            while (!_EventThConnectNc.WaitOne(1000))
            {
                try
                {
                    foreach (var vIt in ConnectionList)
                    {
                        //if(vIt.Connected)
                        //{
                        //    continue;
                        //}

                        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                        System.Net.NetworkInformation.PingReply pingReply = ping.Send(vIt.IP, 1000);
                        if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        {
                            short nClientNO = GlobalSession.HNC_NetConnect(vIt.IP, vIt.Port, true);
                            if ((nClientNO >= 0) && (nClientNO < 256))
                            {
                                if (vIt.ClientNO == nClientNO)
                                {
                                    continue;
                                }
                                else
                                {
                                    vIt.ClientNO = nClientNO;
                                    if ((vIt.ClientNO >= 0) && (256 > vIt.ClientNO))
                                    {
                                        Console.WriteLine(vIt.MacID + " ,ClientNO is different. ");
                                    }
                                }
                            }

                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void InitWebService()
        {
            this.WebServiceTimer = new System.Timers.Timer();
            this.WebServiceTimer.Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServTmrInter_OffLine"]);
            WebServiceTimer.Interval = 180000;
            WebServiceTimer.Elapsed += new ElapsedEventHandler(WebService_Tick);
            WebServiceTimer.Enabled = false;
            WebServiceTimer.Start();
        }
        //函数功能：1.初始化离线数据库，程序中已有则读取，无则新建立离线数据库。
        //          2.更新远程数据库标志位
        //          3.调用WriteRemoteDB()->上传离线数据库数据
        private async void InitLocalDB()
        {
            //初始化离线数据库，无则新建并打开，有则打开；
            if (null == System.Configuration.ConfigurationManager.AppSettings["sLocalDBName"] || "hncLocalData.db" == System.Configuration.ConfigurationManager.AppSettings["sLocalDBName"])
            {
                m_SqliteDB = new SQLiteDatabase();//初始化离线数据库，无则新建并打开，有则打开；无参数说明是默认数据库hncLocalData.db
            }
            else
            {
                m_SqliteDB = new SQLiteDatabase(System.Configuration.ConfigurationManager.AppSettings["sLocalDBName"]);
            }
            //db = new pgData();
            if (0 == m_SqliteDB.QerTableIsExist("SqliteLocalData"))
            {
                m_SqliteDB.ExecuteNonQuery("create table SqliteLocalData(ID INTEGER PRIMARY KEY AUTOINCREMENT,CreateTime text,Type text,JsonContent text,IsUploaded INTEGER)");
            }
            if (0 == m_SqliteDB.QerTableIsExist("SqliteNCMachines"))
            {
                m_SqliteDB.ExecuteNonQuery("create table SqliteNCMachines(ID integer,MacID text,MacSN text,IPAddress text,DCAgentPort text,NCLogConfig text)");
            }
            OutputMessage2Console("【数据库】离线数据库初始化->成功\n");
            try
            {
                int nAuthenFlag = IsAuthenticated(); //采集器webapi认证
                if (-2 == nAuthenFlag) //-2->用户名或密码错误，认证失败
                {
                    OutputMessage2Console("输入任何键后退出！\n");
                    //Console.ReadKey();
                    //Environment.Exit(1);
                    //程序退出
                }
                else if (-1 == nAuthenFlag)
                {
                    RemoteDBIsReady = false; //网络故障
                }
                else if (0 == nAuthenFlag) //网络正常，用户名和密码认证成功
                {
                    RemoteDBIsReady = true;
                   await ReportDaAgentIP(); //向服务器报告采集器IP地址
                }
            }
            catch
            {
                RemoteDBIsReady = false;
                OutputMessage2Console("【采集器】采集器WeiApi认证->异常\n");
            }
            WriteDBSta2Console();
            //delete 20160723
            //if (RemoteDBIsReady)
            //{
            //    //把本地数据上传远程
            //    if (0 == (await WriteRemoteDB()))
            //        OffDatIsAllUpdated = true;
            //    else
            //        OffDatIsAllUpdated = false;
            //}
        }
        private void InitConnection()//调用WebApi->Connect函数放在InitConnectionList()中
        {
            db = new pgData();
            CmpID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["CmpID"]);
            /*           int ret =SessionManager.Instance().InitNet(LocalIP, LocalPort);
                       if (ret != 0)
                       {
                           OutputMessage2Console("【采集器】主机调用HNC_NetInit接口初始化->失败\n输入任何键后退出！\n");
                        }
                       else
                       {
                           OutputMessage2Console("【采集器】主机调用HNC_NetInit接口初始化->成功\n");
                       }
           */
            InitConnectionList(); //更新机床列表链表//delete 20160724
            try
            {
                InitRemoteService();
                OutputMessage2Console("【采集器】初始化RemoteService->成功\n");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                OutputMessage2Console("【采集器】初始化RemoteService->失败\n");
            }

 
        }
        /// <summary>
        /// 初始化.NetRemoting调用远程对象服务。
        /// </summary>
        private void InitRemoteService()
        {
            channel = new TcpServerChannel(AgentRemotingPort);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(HncDataService),"HncDataService", WellKnownObjectMode.SingleCall);
        }
        private string TransGModeToStr(int mid)
        {
            string midTostring = mid.ToString();
            string modeSTR;
            if (midTostring.Length == 1)
            {
                modeSTR = "G0" + midTostring;
            }
            else
            {
                modeSTR = "G" + midTostring;
            }

            return modeSTR;
        }
        private int GetValue(short clientNo, string[] value)
        {
            int rowNum = 0;
            Int16 dupNum = 0;//参数重复个数
            Int16 dupNo = 0;//参数重复编号
            Int32 index = -1;//参数的索引值
            int ret = GlobalSession.HNC_ParamanGetSubClassProp(1, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
            if (rowNum < 0)
            {
                return -2;//子类行数小于0的异常
            }
            ret = GlobalSession.HNC_ParamanRewriteSubClass(1, 0, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (ret < 0)
            {
                return -3;//加载子类时出错的异常
            }

            for (int i = 0; i < rowNum; i++)
            {
                ret = GlobalSession.HNC_ParamanTransRow2Index(1, 0, i, ref index, ref dupNum, ref dupNo, clientNo);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
                if (index < 0)
                {
                    return -1;
                }
                Int32 parmID = -1;
                ret = GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
                if (ret < 0)
                {
                    return -2;
                }
                if (parmID.ToString("D6") == "010220")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[0] = GetStringFrmByte(araayBt);
                    continue;
                }
                else if (parmID.ToString("D6") == "010221")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[1] = GetStringFrmByte(araayBt);
                    continue;
                }
                else if (parmID.ToString("D6") == "010223")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[2] = GetStringFrmByte(araayBt);
                    continue;
                }
                else if (parmID.ToString("D6") == "010224")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[3] = GetStringFrmByte(araayBt);
                    break;
                }
            }
            return 0;
        }
        private MacStateLog GetForMacStateLog(MachineState a, int nStateTemp)
        {
            MacStateLog openlog = new MacStateLog();
            openlog.CreateTime = DateTime.Now;//新状态的生成时间
            openlog.MacID = a.MacID;
            openlog.State = nStateTemp;/* -1->离线 0->空闲 1->运行 2->默认 3->报警*/

            string progName = "";
            int wkMode = 0;
            int ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_MODE, 0, 0, ref wkMode, a.ClientNO);//工作模式 1~7
            if (0 == ret)
                openlog.WorkMode = wkMode.ToString();

            ret = GlobalSession.HNC_FprogGetFullName(0, ref progName, a.ClientNO);//取当前运行G代码名字
            if (0 == ret && progName.StartsWith("../prog/"))
                openlog.GcodeName = progName;

            int spdlOverRide = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_SPDL_OVERRIDE, 0, 0, ref spdlOverRide, a.ClientNO);//主轴倍率
            if (0 == ret)
            {
                openlog.SpindleRate = spdlOverRide;
            }
            int feedOverRide = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_FEED_OVERRIDE, 0, 0, ref feedOverRide, a.ClientNO);//进给修调
            if (0 == ret)
            {
                openlog.FeedRate = feedOverRide;
            }

            if (3 == nStateTemp)
            {
                int alarmID = 0;
                string alarmTxt = "";
                ret = GlobalSession.HNC_AlarmGetData((int)AlarmType.ALARM_TYPE_ALL, (int)AlarmLevel.ALARM_LEVEL_ALL, 0, ref alarmID, ref alarmTxt, a.ClientNO);//取告警ID和文本
                if (0 == ret)
                {
                    if (alarmTxt.StartsWith("\""))
                    {
                        int nSecondIndex = alarmTxt.IndexOf('\"', 1);
                        if (nSecondIndex < 0)
                            nSecondIndex = alarmTxt.Length - 2;
                        alarmTxt = alarmTxt.Substring(1, --nSecondIndex);
                    }
                    openlog.DescText = alarmTxt;
                    openlog.StateCode = alarmID.ToString();
                }
            }
            openlog.Operator = m_sMacStaLogOperator;
            openlog.CmpID = CmpID;
            return openlog;
        }
        private void WriteDBSta2Console()
        {
            if (RemoteDBIsReady)
            {
                OutputMessage2Console("【数据库】远程数据库状态->正常\n");
            }
            else
            {
                OutputMessage2Console("【数据库】远程数据库状态->离线\n");
            }
        }

        private Object _lockSyncObj = new object();
        private void UpConnectedList(MachineState macState)
        {
            lock(_lockSyncObj)
            {
                var macst = ConnectionList.Where(t => t.IP.Equals(macState.IP)).GetEnumerator();

                if (!macst.MoveNext())
                {
                    ConnectionList.Add(macState);
                }
                else
                {
                    macst.Current.ClientNO = macState.ClientNO;
                    macst.Current.Port = macState.Port;
                    macst.Current.m_sNCVer = macState.m_sNCVer;
                    macst.Current.m_sMachineSNCode = macState.m_sMachineSNCode;
                    macst.Current.m_nMachineState = macState.m_nMachineState;
                    macst.Current.Connected = macState.Connected;
                }
            }

        }
        public void ProcessConnDatPosi(List<ScadaHncData.HNCData> ncDatas)
        {

            foreach(var vItem in ncDatas)
            {
                MachineState macState = new MachineState();
                macState.ClientNO = vItem.sysData.clientNo;
                macState.IP = vItem.sysData.addr.ip;
                macState.Port = vItem.sysData.addr.port;
                macState.m_sNCVer = vItem.sysData.sysver.cnc;
                macState.m_sMachineSNCode = vItem.sysData.macSN;
                macState.m_nMachineState = 0;
                macState.m_dtCreaTimeOfState = DateTime.Now;
                macState.m_sAxisName = "x,y,z,s";
                macState.m_sAxisNo = "0,1,2,5";
                macState.Connected = true;

                UpConnectedList(macState);

            }
            _EventAutoRst.Set();

           // ConnectMachinePosi(false);
        }
        /// <summary>
        /// 采集器主动获取机床数据，更新机床状态MacStateLog，更新机床列表ConnectionList
        /// </summary>
        /// <param name="bIsStartCon">true:打印控制台消息；false:不打印控制台消息</param>
        private void ConnectMachinePosi(bool bIsStartCon)
        {
            if (ConnectionList != null)
            {
                foreach (var a in ConnectionList)//每一个单独发出去
                {
                    if (a.Connected)
                    {
                        if (GlobalSession.HNC_NetIsConnect(a.ClientNO))//不发消息
                        {
                            int nStateTemp = GetStateValue(a.ClientNO);
                            MacStateLog cNowState = GetForMacStateLog(a, nStateTemp);
                            cNowState.Operator += "P";
                            cNowState.DescText = DescTextNotNull(nStateTemp, cNowState.DescText);
                            if (!bIsStartCon)
                            {
                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    _macStateLogList.AddLog(cNowState);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(cNowState);
                                }
                            }

                            a.m_nMachineState = cNowState.State;
                            a.m_sRunGCode = cNowState.GcodeName;
                            a.m_dtCreaTimeOfState = cNowState.CreateTime;
                            continue;
                        }
                        else
                        {
                            a.Connected = false;
                            var cDisState = new MacStateLog();
                            cDisState.CreateTime = DateTime.Now;
                            cDisState.MacID = a.MacID;
                            cDisState.State = -1;
                            cDisState.CmpID = CmpID;
                            cDisState.Operator = m_sMacStaLogOperator + "P"; ;    //测试点，以后删除整行！！！
                            cDisState.DescText = "离线";
                            if (!bIsStartCon)
                            {
                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(cDisState);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(cDisState);
                                }
                            }
                            a.m_nMachineState = -1;//更新Connectlist中的机床状态
                            a.m_dtCreaTimeOfState = cDisState.CreateTime;
                            a.m_sRunGCode = cDisState.GcodeName;
                            a.Connected = false;//断线设置状态
                            a.ClientNO = -1;//还原ClientNo到-1
                        }
                    }
                    else if (null != a.IP)
                    {
                        int nClientNoTemp = GlobalSession.HNC_NetConnect(a.IP, (ushort)a.Port,false);
                        if (nClientNoTemp >= 0 && nClientNoTemp < 255)
                        {
                            a.ClientNO = (short)nClientNoTemp;
                        }
                        else
                        {
                            if (bIsStartCon)
                                OutputMessage2Console("【采集器主动连接机床】" + "MacID:" + a.MacID + "->通过IP地址连接失败！" + "; IP:" + a.IP + "; Port:" + a.Port.ToString() + "\n");
                            //-1状态发一个
                            //第一次连接设备时将当前离线状态发给服务器，更新服务器端机床状态 20160412 CH
                            MacStateLog badCon = new MacStateLog();
                            a.Connected = false;
                            badCon.CreateTime = DateTime.Now;
                            badCon.MacID = a.MacID;
                            badCon.State = -1;
                            badCon.CmpID = CmpID;
                            badCon.Operator = m_sMacStaLogOperator + "P";
                            badCon.DescText = "离线";
                            if (!bIsStartCon)
                            {
                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(badCon);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(badCon);

                                }
                            }
                            a.m_nMachineState = -1;//更新Connectlist中的机床状态
                            a.m_dtCreaTimeOfState = badCon.CreateTime;
                            a.m_sRunGCode = badCon.GcodeName;
                            a.Connected = false;//断线设置状态
                            a.ClientNO = -1;//还原ClientNo到-1
                            continue;
                        }

                        if(GlobalSession.HNC_NetIsConnect(a.ClientNO))
                        {

                            if (bIsStartCon)
                            {
                                string sNCVer = "";
                                int ret = GlobalSession.HNC_SystemGetValue((int)HncSystem.HNC_SYS_NC_VER, ref sNCVer, a.ClientNO); //NC版本
                                if (0 == ret)
                                    a.m_sNCVer = sNCVer;
                                string sMacSNTemp = "";
                                ret = GlobalSession.HNC_SystemGetValue((int)HncSystem.HNC_SYS_SN_NUM, ref sMacSNTemp, a.ClientNO);//通过clientNo获取SN码
                                if (0 != ret)
                                    OutputMessage2Console("【采集器主动连接机床】" + "采集器主动建立连接时，获取SN码失败！" + "\n");
                                if ("" == sMacSNTemp) sMacSNTemp = "空";
                                a.m_sMachineSNCode = sMacSNTemp;
                                OutputMessage2Console("【采集器主动连接机床】" + "MacID:" + a.MacID + "->通过IP地址连接成功！" + "; IP:" + a.IP + "; Port:" + a.Port.ToString() + "; ClientNo:" + a.ClientNO.ToString() + "; SN:" + sMacSNTemp + "; NCVer:" + a.m_sNCVer + "\n");
                            }
                            MacStateLog tbNewCon = new MacStateLog();
                            tbNewCon.CreateTime = DateTime.Now;//新状态的生成时间
                            tbNewCon.CmpID = CmpID;
                            tbNewCon.MacID = a.MacID;
                            tbNewCon.State = 6;/* 6->仅仅只是表示建立连接*/
                            tbNewCon.DescText = "连接建立";
                            tbNewCon.Operator = m_sMacStaLogOperator + "P"; ;
                            if (!bIsStartCon)
                            {
                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(tbNewCon);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(tbNewCon);
                                }
                            }
                            int nStateTemp = GetStateValue(a.ClientNO);
                            MacStateLog cNowState = GetForMacStateLog(a, nStateTemp);
                            cNowState.Operator += "P";
                            a.Connected = true;
                            a.m_nMachineState = nStateTemp;//更新Connectlist中的机床状态
                            a.m_dtCreaTimeOfState = cNowState.CreateTime;
                            a.m_sRunGCode = cNowState.GcodeName;
                            switch (nStateTemp)
                            {
                                case 0:
                                case 2: cNowState.DescText = "空闲"; break;
                                case 1: cNowState.DescText = "运行"; break;
                                default: break;
                            }
                            cNowState.DescText = DescTextNotNull(nStateTemp, cNowState.DescText);
                            if (!bIsStartCon)
                            {
                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(cNowState);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(cNowState);
                                }
                            }

                        }
                    }
                }
            }
        }
        private async void UploadOfflineData()
        {
            OffDatIsAllUpdated = false;
            //把本地数据上传远程
            if (0 < m_SqliteDB.QerTableIsExist("SqliteLocalData"))
            {
                DataTable dt = m_SqliteDB.GetDataTable("SELECT * FROM SqliteLocalData WHERE Type = '2' AND IsUploaded = 0  ORDER BY ID ASC");
                while (dt.Rows.Count > 0)//只要大于0，就循环上传
                {
                    if (0 != (await WriteRemoteDB()))
                    {
                        OffDatIsAllUpdated = false;
                        return;
                    }
                    dt = m_SqliteDB.GetDataTable("SELECT * FROM SqliteLocalData WHERE Type = '2' AND IsUploaded = 0  ORDER BY ID ASC");
                }
            }
            OffDatIsAllUpdated = true;
        }
        //函数功能：把要写入MacStateLog数据表的记录写入数据表
        private async Task<int> WriteRemoteDB()
        {
            //把本地数据上传远程
            if (0 < m_SqliteDB.QerTableIsExist("SqliteLocalData"))
            {
                DataTable dt = m_SqliteDB.GetDataTable("SELECT * FROM SqliteLocalData WHERE Type = '2' AND IsUploaded = 0  ORDER BY ID ASC");
                if (0 < dt.Rows.Count)
                {
                    OutputMessage2Console("正在上传离线数据...总共需上传离线数据：" + dt.Rows.Count.ToString() + "组\n");
                    int nPerCntForUploadTbl = int.Parse(System.Configuration.ConfigurationManager.AppSettings["nPerCntForUploadTbl"]);
                    int nLoopCnt = dt.Rows.Count / nPerCntForUploadTbl + ((0 == dt.Rows.Count % nPerCntForUploadTbl) ? 0 : 1);
                    int nUploadedSum = 0;
                    for (int i = 0; i < nLoopCnt; i++)
                    {                    
                        AsyncResultObj asyncRst=await UploadOfflineDa2List();
                        int nUploadedCnt = asyncRst.UploadedCnt;
                        int nUploadFlag = asyncRst.Rst;

                        if (0 == nUploadFlag)
                        {
                            nUploadedSum += nUploadedCnt;
                            OutputMessage2Console("已上传：" + nUploadedSum.ToString() + "组\n");
                        }
                        else if (-2 == nUploadFlag)
                        {
                            OutputMessage2Console("已上传：离线数据存入链表错误，远程连接失败");
                            return -2;
                        }
                        else
                        {
                            nUploadedSum += nUploadedCnt;
                            OutputMessage2Console("已上传：" + nUploadedSum.ToString() + "条 " + "出错：" + nUploadedCnt.ToString() + "条\n");
                        }
                    }
                }
            }
            return 0;
        }
        /*
         * 函数功能：采集器WebApi认证         
         * 返回值：0->用户名，密码正确，WebApi认证成功；-1->网络错误，认证失败；-2->用户名或密码错误，认证失败。
         */
        private int IsAuthenticated()
        {
            OutputMessage2Console("【采集器】WebApi认证...");
            
            try
            {
                string sLoginURI = System.Configuration.ConfigurationManager.AppSettings["AuthenticationURLAddress"];
                string sUserID = System.Configuration.ConfigurationManager.AppSettings["sDataAgentUserID"];
                string sPassword = System.Configuration.ConfigurationManager.AppSettings["sDataAgentPassword"];
                CookieContainer m_Cookie = new CookieContainer();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sLoginURI);
                request.CookieContainer = m_Cookie;
                HttpWebResponse response1 = null;
                response1 = (HttpWebResponse)request.GetResponse();
                Stream stream = response1.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();
                stream.Close();
                reader.Close();
                response1.Close();
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("UserName", sUserID);
                parameters.Add("Password", sPassword);
                HttpWebRequest request_f = WebRequest.Create(sLoginURI) as HttpWebRequest;
                request_f.Method = "POST";
                request_f.ContentType = "application/x-www-form-urlencoded";
                request_f.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                request_f.CookieContainer = m_Cookie;
                if (!(parameters == null || parameters.Count == 0))
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        }
                        i++;
                    }
                    Encoding requestEncoding = Encoding.UTF8;
                    byte[] data = requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream1 = request_f.GetRequestStream())
                    {
                        stream1.Write(data, 0, data.Length);
                    }
                }
                response1 = (HttpWebResponse)request_f.GetResponse();


                CookieCollection mycok = m_Cookie.GetCookies(new Uri(sLoginURI));
                response1 = HttpWebResponseUtility.CreateGetHttpResponse(sLoginURI, null, null, mycok);

                if (response1!=null&&2 != mycok.Count)
                {
                    OutputMessage2Console("->失败(用户名或密码错误)\n");
                    return -2;    //用户名、密码错误，认证失败。
                }
                OutputMessage2Console("->成功\n");
                return 0;
            }
            catch (WebException e)
            {
                if (e.Status.ToString() == "ConnectFailure")
                {
                    OutputMessage2Console("->失败(网络故障)\n");
                    return -1;  //网络异常，无法连接到指定url。
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IsAuthenticated:" + ex.Message);
            }

            return -1;
            
        }
        //输入：nJsonType数据表类型 1->采集器启动时，向数据库写入自己IP地址【已删除该项】。2->向数据写入状态MacStateLog表。
        //      sJsonContent 写入本地数据库的Json数据块。
        //输出：true->此次入库操作成功(本地或服务器端);false->此次入库操作失败(本地或服务器端);
        private bool SaveLocalRecordIsOK(int nJsonType, string sJsonContent)
        {
            try
            {
                m_SqliteDB.ExecuteNonQuery("insert into SqliteLocalData (CreateTime,Type,JsonContent,IsUploaded) values('" + DateTime.Now.ToString() + "','" + nJsonType.ToString() + "','" + sJsonContent + "'," + 0 + ")");
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 填充机床列表ConnectionList链表初始值
        /// </summary>
        private async void InitConnectionList()//填充ConnectionList链表初始值
        {
            if (!(await UpdateMacConnectionList()))
            {
                //----------------------------------使用本地数据库-------------------------start
                RemoteDBIsReady = false;
                //ConnectionList = new List<MachineState>();
                DataTable dt = m_SqliteDB.GetDataTable("select * from SqliteNCmachines");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MachineState temp = new MachineState();
                        temp.MacID = dt.Rows[i][1].ToString();
                        temp.ClientNO = -1;
                        temp.m_nMachineState = -1;
                        temp.m_sMachineSNCode = dt.Rows[i][2].ToString();
                        temp.IP = dt.Rows[i][3].ToString();
                        temp.Port = ushort.Parse(dt.Rows[i][4].ToString());
                        if (null != dt.Rows[i][5].ToString())
                            temp.m_cNCLogConfig = JsonConvert.DeserializeObject<NCLogAutoDownloadConfig>(dt.Rows[i][5].ToString());
                        temp.m_bSamplChFlag = 0x0000;
                        temp.m_nSamplStatFlag = 0;
                        temp.m_nSamplStatFlagForFile = 0;
                        ConnectionList.Add(temp);
                    }
                    OutputMessage2Console("【ConnLst信息】WebApi初始化ConnLst出错！->使用本地DB 共" + dt.Rows.Count.ToString() + "条\n");
                }
                else
                {
                    OutputMessage2Console("【采集器】采集器首次运行且未认证！\n");
                }
                //----------------------------------使用本地数据库-------------------------end
            }
        }
        /// <summary>
        /// 获取远程服务器机床列表，更新到机床列表链表。
        /// </summary>
        /// <returns>false：更新失败;true:更新成功。</returns>
        private async Task<bool> UpdateMacConnectionList()
        {
            //-----------------------------------WebApi方法------------------start
            if (!RemoteDBIsReady)
            {
                return false;
            }
            try
            {
           //     int nMacID2DelSum = 0;
           //     int nMacID2InsertSum = 0;
                List<MachineState> newConnectionList = new List<MachineState>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["WebApiURLAddress"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response_zb = null;
                List<MachineState> lstMacID2Del = new List<MachineState>();
                try
                {
                    response_zb = await client.GetAsync("Machine/Get?AgentCode=" + AgentCode).ConfigureAwait(false);
                }
                catch
                {
                    OutputMessage2Console("【WebApi】WebApi下拉ConnLst异常\n");
                    return false;
                }
                if (null != response_zb && response_zb.IsSuccessStatusCode)
                {
                    IEnumerable<MachineIPPort> list =await response_zb.Content.ReadAsAsync<IEnumerable<MachineIPPort>>().ConfigureAwait(false);
                    foreach (var a in list)
                    {
                        MachineState cMacTemp = new MachineState();
                        cMacTemp.MacID = a.MacID;
                        cMacTemp.IP = a.IP;
                        cMacTemp.m_sMachineSNCode = a.MacSN;
                        cMacTemp.Port = (ushort)a.Port;
                        if (null != a.NCLogConfig)
                            cMacTemp.m_cNCLogConfig = JsonConvert.DeserializeObject<NCLogAutoDownloadConfig>(a.NCLogConfig);
                        cMacTemp.ClientNO = (short)-1;
                        cMacTemp.m_nMachineState = (short)-1;
                        cMacTemp.Connected = false;
                        cMacTemp.PanelGet = 0;
                        cMacTemp.EventGet = 0;
                        cMacTemp.FilechgeGet = 0;
                        cMacTemp.WorkGet = 0;
                        newConnectionList.Add(cMacTemp);
                    }
                }
                else
                {
                    //Something has gone wrong, handle it here
                    OutputMessage2Console("【WebApi】WebApi下拉ConnLst出错\n");
                    return false;
                }
            _EventAutoRst.WaitOne();
                //-----------------------------------WebApi方法------------------end
                if (null != ConnectionList && null != newConnectionList)
                {
                /*
                foreach (var a in ConnectionList)
                {
                    bool CanDel = true;
                    foreach (var aNew in newConnectionList)
                    {
                        if (a.IP == aNew.IP)
                        {
                            a.MacID = aNew.MacID;
                           // CanDel = false;
                            break;
                        }
                    }
                    
                    if (CanDel)
                    {
                        lstMacID2Del.Add(a);
                        ++nMacID2DelSum;
                    }
                    */

                foreach (var aNew in newConnectionList)
                {
                    var macst = ConnectionList.Where(t => t.IP.Equals(aNew.IP)).GetEnumerator();
                    if (!macst.MoveNext())
                    {
                        ConnectionList.Add(aNew);
                    }
                    else
                    {
                       macst.Current.MacID = aNew.MacID;
                    }
                }
                /*
                foreach (var MacID2Del in lstMacID2Del)
                {
                    ConnectionList.Remove(MacID2Del);
                    OutputMessage2Console("【ConnLst信息】删除机床->MacID:" + MacID2Del.MacID + "\n");
                }

              foreach (var aNew in newConnectionList)
                {
                    bool CanIns = true;
                    foreach (var a in ConnectionList)
                    {
                        if (a.MacID == aNew.MacID)
                        {
                            CanIns = false;
                            break;
                        }
                    }
                    if (CanIns)
                    {
                        OutputMessage2Console("【ConnLst信息】添加机床->MacID:" + aNew.MacID + "\n");
                        ConnectionList.Add(aNew);
                        ++nMacID2InsertSum;
                    }
                }
                OutputMessage2Console("【ConnLst信息】更新:" + (nMacID2InsertSum + nMacID2DelSum).ToString()
                    + " 新增:" + nMacID2InsertSum.ToString() + " 删除:" + nMacID2DelSum.ToString() + " 当前监控总数:" + newConnectionList.Count.ToString() + "\n");

                */
            }          
/*
            List<NCMachines> lstLastMacLst = new List<NCMachines>();
            foreach (var T in ConnectionList)//newConnectionList)
            {
                NCMachines cNCMacTemp = new NCMachines();
                cNCMacTemp.MacID = T.MacID;
                cNCMacTemp.MacSN = T.m_sMachineSNCode;
                cNCMacTemp.IPAddress = T.IP;
                cNCMacTemp.DCAgentPort = T.Port;
                if (null != T.m_cNCLogConfig)
                    cNCMacTemp.NCLogConfig = JsonConvert.SerializeObject(T.m_cNCLogConfig);
                lstLastMacLst.Add(cNCMacTemp);
            }
            UpdateOfflineMacLst2Sqlite(lstLastMacLst); //将服务器上获取的最新机床列表数据存入离线数据库
*/          
            
            return true;

            }
            catch(Exception ex)
            {
                Console.WriteLine("UpdateConnectionList:" + ex.Message);
            }
            return false;
            
        }
        private void NCLogService()
        {
            foreach (var a in ConnectionList)
            {
                if (a.Connected && null != a.m_cNCLogConfig) //连接是成功的,下载日志文件
                {
                    if (a.m_cNCLogConfig.PANEL2Dld)//cLogDldFlag.ContainsKey("PANEL") && cLogDldFlag["PANEL"])
                    {
                        a.PanelGet = LogDownload("PANEL", a.MacID, a.ClientNO);
                    }
                    if (a.m_cNCLogConfig.EVENT2Dld)//cLogDldFlag.ContainsKey("EVENT") && cLogDldFlag["EVENT"])
                    {
                        a.EventGet = LogDownload("EVENT", a.MacID, a.ClientNO);
                    }
                    if (a.m_cNCLogConfig.FILECHANGE2Dld)//cLogDldFlag.ContainsKey("FILECHGE") && cLogDldFlag["FILECHGE"])
                    {
                        a.FilechgeGet = LogDownload("FILECHGE", a.MacID, a.ClientNO);
                    }
                    if (a.m_cNCLogConfig.WORK2Dld)//cLogDldFlag.ContainsKey("WORK") && cLogDldFlag["WORK"])
                    {
                        a.WorkGet = LogDownload("WORK", a.MacID, a.ClientNO);
                    }
                    if (a.m_cNCLogConfig.ALARM2Dld)//cLogDldFlag.ContainsKey("ALARM") && cLogDldFlag["ALARM"])
                    {
                        AlarmLogWrite(a.MacID, a.ClientNO);
                    }
                }
            }
        }
        private int GetStateValue(Int16 ClientNO)//根据老方法获取机床的状态值，校准机床初始值
        {
            const Int32 EMERGENCY_STOP = 3;
            const Int32 CYCLING = 1;
            const Int32 RUNNING = 1;
            const Int32 HOLDING = 0;
            const Int32 RESETTING = 0;
            const Int32 PROEND = 0;
            const Int32 DEFAULT = 2;
            int ret;
            int eStop = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_ESTOP, 0, 0, ref eStop, ClientNO);//是否急停 3
            if (ret == 0&&1==eStop) return EMERGENCY_STOP;
            int Cycle = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_CYCLE, 0, 0, ref Cycle, ClientNO);//循环启动 1
            if (ret == 0 && 1 == Cycle) return CYCLING;
            int Running = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_RUNNING, 0, 0, ref Running, ClientNO);//运行中 1
            if (ret == 0 && 1 == Running) return RUNNING;
            int Hold = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_HOLD, 0, 0, ref Hold, ClientNO);//进给保持 0
            if (ret == 0 && 1 == Hold) return HOLDING;
            int Progend = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_PROGEND, 0, 0, ref Progend, ClientNO);//程序运行完成 0
            if (ret == 0 && 1 == Progend) return PROEND;
            int Resetting = 0;
            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_RESETTING, 0, 0, ref Resetting, ClientNO);//复位
            if (ret == 0 && 1 == Resetting) return RESETTING;
            return DEFAULT;
        }
        private int WrtAxisInfo2Conlst(ref MachineState a)
        {
            if (! GlobalSession.HNC_NetIsConnect(a.ClientNO))
            {
                return -1;
            }
            string sAxisNo = null;
            string sAxisName = null;
            string sAxisFlag = "";
            int ret = paraGetVal(1, 0, "010017", a.ClientNO, ref sAxisFlag);
            if (ret != 0 || sAxisFlag.Length <= 2)
            {
                OutputMessage2Console("配轴获取010017出错，代码->" + ret.ToString() + "\n");
                return -1;
            }
            string sMask = "";
            ret = paraGetVal(1, 0, "010041", a.ClientNO, ref sMask);
            if (ret != 0)
            {
                OutputMessage2Console("配轴获取010041出错，代码->" + ret.ToString() + "\n");
                return -1;
            }
            int nMask = -1;
            if ("" != sMask)
            {
                nMask = Convert.ToInt32(sMask);
            }
            sAxisFlag = sAxisFlag.Substring(2);
            int nAxisInfoFlag = Int32.Parse(sAxisFlag, System.Globalization.NumberStyles.HexNumber);
            for (int i = 0; i < 8; i++)
            {
                if ((nAxisInfoFlag & (1 << i)) != 0)
                {
                    string sAxisNameTemp = null;
                    ret = GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_NAME, i, ref sAxisNameTemp, a.ClientNO);
                    if (0 == ret)
                    {
                        if (null != sAxisNameTemp)
                        {
                            if (1 == nMask && ("C" == sAxisNameTemp || "c" == sAxisNameTemp))
                                continue;
                            sAxisNo += i.ToString() + ",";
                            sAxisName += sAxisNameTemp + ",";
                        }
                        else
                            return -1;
                    }
                    else
                    {
                        OutputMessage2Console("配轴获取轴名出错，代码->" + ret.ToString());
                        return -1;
                    }
                }
            }
            if (((null == sAxisName) || (2 > sAxisName.Length)) || ((null == sAxisNo) || (2 > sAxisNo.Length)))
            {
                return -1;
            }
            a.m_sAxisName = sAxisName.Remove(sAxisName.Length - 1);//去末尾逗号
            a.m_sAxisNo = sAxisNo.Remove(sAxisNo.Length - 1);//去末尾逗号
            return 0;
        }
        private int LogDownload(string Logfile, string MacID, short ClientNO)
        {
            int LogDownFlag = 0;
            string str = System.AppDomain.CurrentDomain.BaseDirectory;
            //更改者:张博----------------------------------start--------------------------
            if (!(Directory.Exists(str + "LOGS\\")))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(str + "LOGS\\");
            }
            string localName = str + "Logs/" + MacID + "--" + DateTime.Now.ToString("yyyy-MM-dd") + "--" + Logfile + ".LOG";//+DateTime.Now.ToShortTimeString(); //本地接收文件后，文件的保存路径
                                                                                                                            //            string localName = str + "Logs/" + MacID + "--" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "--" + Logfile + ".LOG";//+DateTime.Now.ToShortTimeString(); //本地接收文件后，文件的保存路径
                                                                                                                            //更改者:张博----------------------------------end----------------------------           
                                                                                                                            //string localName = "D://PANEL.LOG"; //本地接收文件后，文件的保存路径
            string dstName = "../data/" + Logfile + ".LOG";  //待获取的文件，在目标机器的路径名

            int ret = GlobalSession.HNC_NetFileGet(localName, dstName, ClientNO);

            if (ret != 0)
            {
                OutputMessage2Console("【LogErr】" + MacID + ":" + Logfile + "获取失败!\n");
                var LocalFile = new FileInfo(localName);
                if (LocalFile.Exists)
                {
                    if (LocalFile.Length == 0)
                    {
                        LocalFile.Delete();
                    }
                }
            }
            else
            {
                OutputMessage2Console("【LogInfo】" + MacID + ":" + Logfile + "获取成功!\n");
                LogDownFlag = 1;
                #region  日志文件写入数据库操作
                //lbMacid.Text = MacID;
                switch (Logfile)
                {
                    case "WORK":
                        //->加入处理文件函数----------------------------------------------------------------------------
                        LogData Work = LogsRead.LogRead(localName);
                        List<ListData> Content = Work.LogContent;
                        if (Content == null)
                        { break; }

                        //lbsumno.Text = Content.Count().ToString();
                        //lbLogtype.Text = "WORK（2加工信息）";
                        int i = 0;

                        hncData.NcLogs lastItem1 = new NcLogs();
                        try
                        {
                            lastItem1 = db.NcLogs.Where(t => t.MacID == MacID && t.LogTypeID == 2).OrderByDescending(t => t.ID).FirstOrDefault();
                        }
                        catch
                        {
                            UpdateDBFlag(false);
                        }

                        if (lastItem1 != null)
                        {
                            var LastfileLog = Content.Where(t => t.CreateTime == lastItem1.CreateTime && t.detail == lastItem1.ItemDetail).FirstOrDefault();
                            if (LastfileLog != null)
                            {
                                Content = Content.Where(t => t.ID > LastfileLog.ID).ToList();
                            }
                        }
                        int count_i = 0;
                        foreach (var part in Content)
                        {
                            var Log = new NcLogs();
                            Log.MacID = MacID;
                            Log.LogTypeID = 2;
                            Log.CreateTime = part.CreateTime;
                            Log.SubID = part.type;
                            Log.LastData = part.oldData;
                            Log.NowData = part.newData;
                            Log.ItemDetail = part.detail;
                            Log.ChNo = part.num;
                            Log.SecondData = part.SecondData;
                            //更改者：张博------------------开始------------------
                            Log.EventCodeContent = part.ProName;
                            if (part.GXX != null) Log.EventCodeContent = part.GXX;
                            //更改者：张博------------------结束------------------
                            Log.DCAgentCode = AgentCode;
                            ++i;
                            //lbnowno.Text = i.ToString();
                            SaveToDBIsOK(ref db, true, Log);
                            ++count_i;
                        }
                        //更改者：张博------------------开始------------------
                        //tb_databaselog.Text += MacID + "存入WORK日志: " + Content.Count + "-" +count_i + "条" + "\r\n";
                        //更改者：张博------------------结束------------------
                        break;

                    case "PANEL":
                        LogData_Panel Panel = LogsRead.LogRead_Panel(localName);
                        List<ListData_Panel> PanelContent = Panel.LogContent;
                        if (PanelContent == null)
                        { break; }

                        int y = 0;
                        hncData.NcLogs lastItem2 = new NcLogs();
                        try
                        {
                            lastItem2 = db.NcLogs.Where(t => t.MacID == MacID && t.LogTypeID == 4).OrderByDescending(t => t.ID).FirstOrDefault();
                        }
                        catch
                        {
                            UpdateDBFlag(false);
                        }

                        if (lastItem2 != null)
                        {
                            var LastfileLog = PanelContent.Where(t => t.CreateTime == lastItem2.CreateTime && t.detail == lastItem2.ItemDetail).FirstOrDefault();
                            if (LastfileLog != null)
                            {
                                PanelContent = PanelContent.Where(t => t.ID > LastfileLog.ID).ToList();
                            }
                        }
                        foreach (var part in PanelContent)
                        {
                            var Log = new NcLogs();
                            Log.MacID = MacID;
                            Log.LogTypeID = 4;
                            Log.CreateTime = part.CreateTime;
                            Log.SubID = part.type;
                            Log.ItemDetail = part.detail;//数控面板显示的事件内容
                            Log.EventSource = part.src;
                            Log.EventCodeContent = part.code;//事件内容
                            Log.DCAgentCode = AgentCode;
                            y++;
                            //lbnowno.Text = y.ToString();
                            SaveToDBIsOK(ref db, true, Log);
                        }
                        //更改者：张博------------------开始------------------
                        //tb_databaselog.Text += MacID + "存入PANEL日志: " + PanelContent.Count + "条" + "\r\n";
                        //更改者：张博------------------结束------------------
                        break;

                    case "EVENT":
                        LogData_Event Event = LogsRead.LogRead_Event(localName);
                        List<ListData_Event> EventContent = Event.LogContent;
                        if (EventContent == null)
                        { break; }

                        //lbsumno.Text = EventContent.Count().ToString();
                        //lbLogtype.Text = "EVENT（6事件）";
                        int z = 0;

                        hncData.NcLogs lastItem3 = new NcLogs();
                        try
                        {
                            lastItem3 = db.NcLogs.Where(t => t.MacID == MacID && t.LogTypeID == 6).OrderByDescending(t => t.ID).FirstOrDefault();
                        }
                        catch
                        {
                            UpdateDBFlag(false);
                        }
                        if (lastItem3 != null)
                        {
                            var LastfileLog = EventContent.Where(t => t.CreateTime == lastItem3.CreateTime && t.detail == lastItem3.ItemDetail).FirstOrDefault();
                            if (LastfileLog != null)
                            {
                                EventContent = EventContent.Where(t => t.ID > LastfileLog.ID).ToList();
                            }
                        }
                        foreach (var part in EventContent)
                        {
                            var Log = new NcLogs();
                            Log.MacID = MacID;
                            Log.LogTypeID = 6;
                            Log.CreateTime = part.CreateTime;
                            Log.SubID = part.type;
                            Log.ItemDetail = part.detail;//数控面板显示的事件内容
                            Log.EventSource = part.src;
                            Log.EventCodeContent = part.code;//事件内容
                            Log.DCAgentCode = AgentCode;
                            z++;
                            //lbnowno.Text = z.ToString();
                            SaveToDBIsOK(ref db, true, Log);
                        }
                        //更改者：张博------------------开始------------------
                        //tb_databaselog.Text += MacID + "存入EVENT日志: " + EventContent.Count + "条" + "\r\n";
                        //更改者：张博------------------结束------------------                        
                        break;

                    case "FILECHGE":
                        LogData_FileChange FileChange = LogsRead.LogRead_FileChange(localName);
                        List<ListData_FileChange> FileChangeContent = FileChange.LogContent;
                        if (FileChangeContent == null)
                        { break; }

                        //lbsumno.Text = FileChangeContent.Count().ToString();
                        //lbLogtype.Text = "FILECHGE（3文件修改）";
                        int a = 0;

                        //int LastLogID = 0;
                        hncData.NcLogs lastItem4 = new NcLogs();
                        try
                        {
                            lastItem4 = db.NcLogs.Where(t => t.MacID == MacID && t.LogTypeID == 3).OrderByDescending(t => t.ID).FirstOrDefault();
                        }
                        catch
                        {
                            UpdateDBFlag(false);
                        }
                        if (lastItem4 != null)
                        {
                            var LastfileLog = FileChangeContent.Where(t => t.CreateTime == lastItem4.CreateTime && t.detail == lastItem4.ItemDetail).FirstOrDefault();
                            if (LastfileLog != null)
                            {
                                FileChangeContent = FileChangeContent.Where(t => t.ID > LastfileLog.ID).ToList();
                            }
                        }
                        foreach (var part in FileChangeContent)
                        {
                            var Log = new NcLogs();
                            Log.MacID = MacID;
                            Log.LogTypeID = 3;
                            Log.CreateTime = part.CreateTime;
                            Log.ItemDetail = part.detail;
                            Log.DCAgentCode = AgentCode;
                            a++;
                            //lbnowno.Text = a.ToString();
                            SaveToDBIsOK(ref db, true, Log);
                        }
                        //更改者：张博------------------开始------------------
                        //tb_databaselog.Text += MacID + "存入FileChange日志: " + FileChangeContent.Count + "条" + "\r\n";
                        //更改者：张博------------------结束------------------
                        break;
                }
                #endregion
            }
            return LogDownFlag;
        }
        private LogData_Alarm LogRead_Alarm(string MacID, short ClintNo)
        {
            LogData_Alarm myLogData = new LogData_Alarm { };
            List<ListData_Alarm> ListLink = new List<ListData_Alarm>();
            GlobalSession.HNC_AlarmRefresh(ClintNo);
            int row = 0;
            int alarmCount = 0;
            int RetAlarmGet = GlobalSession.HNC_AlarmGetHistoryNum(ref alarmCount, ClintNo);

            //int alarmCountNow = 0;
            //int rett = 0;
            //rett = GlobalSession.HNC_AlarmGetNum((int)AlarmType.ALARM_TYPE_ALL, (int)AlarmLevel.ALARM_LEVEL_ALL, ref alarmCountNow, ClintNo);

            int ret = 0;
            AlarmHisData[] alarmHis = new AlarmHisData[alarmCount];
            ret = GlobalSession.HNC_AlarmGetHistoryData(1, ref alarmCount, alarmHis, ClintNo);
            //tb_databaselog.Text += MacID + "报警条目总数：" + alarmCount.ToString() + "\r\n";
            if (ret == 0)
            {
                //tb_databaselog.Text += MacID + "报警日志获取成功。" + "\r\n";
                for (row = 0; row < alarmCount; row++)
                {
                    if (alarmHis[row].timeBegin.year > 0)
                    {
                        ListLink.Add(new ListData_Alarm()
                        {
                            detail = alarmHis[row].text,
                            CreateTime = new DateTime(alarmHis[row].timeBegin.year, alarmHis[row].timeBegin.month, alarmHis[row].timeBegin.day, alarmHis[row].timeBegin.hour, alarmHis[row].timeBegin.minute, alarmHis[row].timeBegin.second),
                            ID = row,
                            //zb-start
                            alarmNo = alarmHis[row].alarmNo
                            //zb-end
                        });
                    }
                }
                //-----------------------------------20150112添加把报警内容写入文件代码----->检查能够读取的报警内容start
                //FileStream f = new FileStream("Alarm.txt", FileMode.Create);
                //StreamWriter sw = new StreamWriter(f, Encoding.Default);
                //foreach (var a in ListLink)
                //{
                //    sw.Write(a.ID.ToString());
                //    sw.Write("---");
                //    sw.Write(a.CreateTime.ToString());
                //    sw.Write("---");
                //    sw.WriteLine(a.detail.ToString());
                //}
                //sw.Close();
                //f.Close();
                //MessageBox.Show("EVENT.log写入记事本成功!");
                //-----------------------------------20150112添加把报警内容写入文件代码----->检查能够读取的报警内容end
            }
            else
            {
                //tb_databaselog.Text += MacID + "报警日志获取失败！" + "\r\n";
            }
            myLogData.LogContent = ListLink;
            return myLogData;
        }
        private void AlarmLogWrite(string MacID, short ClintNo)
        {
            LogData_Alarm AlarmLog = LogRead_Alarm(MacID, ClintNo);
            List<ListData_Alarm> Content = AlarmLog.LogContent;

            if (Content == null)
            {
                //tb_databaselog.Text += MacID + "无报警内容！" + "\r\n";
                return;
            }

            //lbsumno.Text = Content.Count().ToString();
            //lbLogtype.Text = "Alarm（1报警）";
            int i = 0;

            hncData.NcLogs lastItem1 = new NcLogs();
            try
            {
                lastItem1 = db.NcLogs.Where(t => t.MacID == MacID && t.LogTypeID == 1).OrderByDescending(t => t.ID).FirstOrDefault();
            }
            catch
            {
                UpdateDBFlag(false);
            }
            if (lastItem1 != null)
            {
                var LastfileLog = Content.Where(t => t.CreateTime == lastItem1.CreateTime && t.detail == lastItem1.ItemDetail).FirstOrDefault();
                if (LastfileLog != null)
                {
                    Content = Content.Where(t => t.ID > LastfileLog.ID).ToList();
                }
            }
            foreach (var part in Content)
            {
                var Log = new NcLogs();
                Log.MacID = MacID;
                Log.LogTypeID = 1;
                Log.CreateTime = part.CreateTime;
                Log.ItemDetail = part.detail;
                //zhangbo ------------------start-----------------------------
                Log.SubID = part.alarmNo;
                //zhangbo ------------------end-----------------------------
                Log.DCAgentCode = AgentCode;
                i++;
                //lbnowno.Text = i.ToString();
                SaveToDBIsOK(ref db, true, Log);
            }
            //更改者：张博------------------开始------------------
            //tb_databaselog.Text += MacID + "存入Alarm日志: " + Content.Count + "条" + "\r\n";
            //更改者：张博------------------结束------------------
        }
        private int ParmWrite()
        {
            int ret = 1;
            int writeModel = 0;
            string sParamVersion = "PM003.0";
            foreach (var t in ConnectionList)
            {
                hncData.pgData dbnew = new hncData.pgData();
                var list_NCD = dbnew.NCParameterDict.AsQueryable();
                var list_NCP = dbnew.NCParameter.AsQueryable();
                var list_mid = from p in list_NCD
                               where (p.ParamVersion == sParamVersion)
                               select p;
                var list_p = from p in list_NCP
                             where (p.MacID == t.MacID)
                             select p;
                if (list_mid.Count() > 0 && list_p.Count() <= 0)
                {
                    writeModel = 1; //只写NCParameter
                }
                else if (list_mid.Count() <= 0 && list_p.Count() > 0)
                {
                    writeModel = 2; //只写NCParameterDict
                }
                else if (list_mid.Count() <= 0 && list_p.Count() <= 0)
                {
                    writeModel = 3; //NCParameterDict和NCParameter都写
                }
                else continue;

                int fileNum = 7;//系统最大分类数，大类
                for (int i = 0; i < fileNum; i++)
                {
                    int recNum = -1;//对应大类的子类分类数，小类
                    ret = GlobalSession.HNC_ParamanGetSubClassProp(i, (Byte)ParaSubClassProp.SUBCLASS_NUM, ref recNum, 0);//根据大的分类类别0-6共7个索引，获取参数的子类的属性。属性包括：子类名，子类行数，子类数。
                    //本函数根据(Byte)ParaSubClassProp.SUBCLASS_NUM的不同，返回不同的值到ref中

                    if (recNum < 0)
                    {
                        ret = recNum;
                        break;
                    }
                    else if (recNum > 0)
                    {
                        for (int j = 0; j < recNum; j++)
                        {
                            ret = GetParItem(i, j, t.ClientNO, t.MacID, writeModel);
                            if (ret < 0)
                            {
                                break;//当某个大类的某个小类读取出错时，跳出循环，不再继续读，返回错误代码ret。
                            }
                        }
                    }
                    if (ret < 0)
                    {
                        break;
                    }
                }
            }
            return ret;
        }
        /*返回值：0->成功，-1...-4->各种不同的错误异常*/
        private int GetParItem(int fileNo, int recNo, short clientNo, string MacID, int writeModel)//fileNo大类，recNo小类，通过大小类索引，把连接链表里面每台的机床的每个小类下面的参数存库
        {
            string sParamVersion = "PM003.0";
            int ret = 0;
            if (true)
            {
                if (true)
                {
                    int rowNum = 0;
                    ret = GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
                    if (rowNum < 0)
                    {
                        return -2;//子类行数小于0的异常
                    }

                    ret = GlobalSession.HNC_ParamanRewriteSubClass(fileNo, recNo, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
                    if (ret < 0)
                    {
                        return -3;//加载子类时出错的异常
                    }

                    string[] strPar = new string[8];//读出每条参数的暂存数组
                    int myEffectWay = 999;
                    int myDataType = 999;
                    int index1 = -1;

                    //FileStream f = new FileStream("zb.txt", FileMode.Append);
                    //StreamWriter sw = new StreamWriter(f, Encoding.Default);


                    for (int i = 0; i < rowNum; i++)//把每行参数读取出来，放在strPtr数组中，每一个数组元素对应当前行参数的某列数据
                    {


                        ret = GetParContent(fileNo, recNo, i, clientNo, strPar, ref myEffectWay, ref myDataType, ref index1);//（大类号，子类号，当前列号，clientNo，字符串数组）
                        if (ret == -1)
                        {
                            break; //在读某一条参数的内容时出现异常，跳出，不再继续读该小分类的参数
                        }
                        //=================本条参数入库操作开始
                        if (2 == writeModel || 3 == writeModel)
                        {
                            var openlog = new NCParameterDict();
                            openlog.ParaCode = strPar[0];
                            openlog.ParaName = strPar[1];
                            openlog.DefaultVal = strPar[4];
                            openlog.MaxVal = strPar[6];
                            openlog.MinVal = strPar[5];
                            openlog.EffectWay = myEffectWay;
                            openlog.DataType = myDataType;
                            openlog.ParaType = fileNo;//ParaType存大类
                            openlog.ParamVersion = sParamVersion;
                            openlog.SubID = recNo;
                            SaveToDBIsOK(ref db, true, openlog);
                        }

                        //----------------------------------------------------------------------------------------------------
                        //if (strPar[0] == "010220")
                        //{
                        //    sw.Write(strPar[0]);
                        //    sw.Write("---");
                        //    sw.Write(index1);
                        //    sw.Write("---");
                        //    sw.Write(fileNo);
                        //    sw.Write("---");
                        //    sw.Write(recNo);
                        //    sw.Write("---");
                        //    sw.Write(myDataType);
                        //    sw.Write("---");
                        //    sw.Write(myEffectWay);
                        //    sw.Write("当前值===>");
                        //    sw.Write(strPar[2]);
                        //    sw.WriteLine("");
                        //    break;
                        //}
                        //----------------------------------------------------------------------------------------------------

                        //写当前值表
                        if (1 == writeModel || 3 == writeModel)
                        {
                            var writeNcNow = new NCParameter();
                            writeNcNow.ParaCode = strPar[0];
                            writeNcNow.MacID = MacID;
                            if (strPar[2].Length > 32)
                            {
                                strPar[2] = strPar[2].Substring(0, 32);
                            }
                            writeNcNow.NowVal = strPar[2];
                            writeNcNow.ChangeTime = DateTime.Now;
                            SaveToDBIsOK(ref db, true, writeNcNow);
                        }
                        //=================本条参数入库操作结束
                    }//该小类的入库操作结束

                    //sw.Close();
                    //f.Close();
                }
                //else ret = -4;//读取机床连接链表ConnectionList为空异常
            }
            return ret;/*返回值：0->成功，-1...-4->各种不同的错误异常*/
        }
        private int GetParContent(int fileNo, int recNo, int row, short clientNo, string[] strPar, ref int EffectWay, ref int DataType, ref int index1)//参数为（大类号，子类号，当前列号，clientNo，字符串数组），根据参数读出一条记录的内容。
        {
            Int16 dupNum = 0;//参数重复个数
            Int16 dupNo = 0;//参数重复编号
            Int32 index = -1;//参数的索引值


            int ret = -1;
            ret = GlobalSession.HNC_ParamanTransRow2Index(fileNo, recNo, row, ref index, ref dupNum, ref dupNo, clientNo);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
            if (index < 0)
            {
                return -1;
            }
            index1 = index;

            //获取生效方式
            Int32 actType = -1;
            ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ACT, ref actType, clientNo);
            if (actType < 0)
            {
                return -1;
            }
            /*
             * 0->保存生效
             * 1->立即生效
             * 2->复位生效
             * 3->重启生效
             * 4->隐藏未启用
             */
            EffectWay = actType;

            //获取参数号
            Int32 parmID = -1;
            ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
            if (ret < 0)
            {
                return -1;
            }
            strPar[0] = parmID.ToString("D6");
            //if (strPar[0] == "010220")
            //{
            //    index1 = index;
            //    break;
            //}
            //continue;


            //获取参数名称
            ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_NAME, ref strPar[1], clientNo);
            if (ret < 0)
            {
                return -1;
            }


            //获取参数储存类型
            Int32 storeType = -1;
            ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_STORE, ref storeType, clientNo);
            if (storeType < 0)
            {
                return -1;
            }
            DataType = storeType;

            //获取参数值、默认值、最小值和最大值
            int iVal = 0;
            double dVal = 0;
            switch (storeType)
            {
                case (sbyte)PAR_STORE_TYPE.TYPE_BOOL:
                case (sbyte)PAR_STORE_TYPE.TYPE_UINT:
                case (sbyte)PAR_STORE_TYPE.TYPE_INT:
                    {
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[2] = iVal.ToString();//参数当前值

                        strPar[3] = iVal.ToString();//参数当前值
                        strPar[7] = iVal.ToString();//参数当前值

                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_DFVALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[4] = iVal.ToString();//参数默认值

                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MINVALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[5] = iVal.ToString();//最小值

                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MAXVALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[6] = iVal.ToString();//最大值

                        break; //本条case块结束
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_FLOAT:
                    {
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[2] = dVal.ToString("F6");
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_DFVALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[4] = dVal.ToString("F6");
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MINVALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[5] = dVal.ToString("F6");
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MAXVALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[6] = dVal.ToString("F6");
                        break;
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_STRING:
                    {
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref strPar[2], clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[4] = "N/A";
                        strPar[5] = "N/A";
                        strPar[6] = "N/A";
                        break;
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_HEX4:
                    {
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                        strPar[2] = "0x" + iVal.ToString("X2");
                        strPar[4] = "N/A";
                        strPar[5] = "N/A";
                        strPar[6] = "N/A";
                        break;
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_BYTE:
                    {
                        sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                        ret = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                        strPar[2] = GetStringFrmByte(araayBt);
                        strPar[4] = "N/A";
                        strPar[5] = "N/A";
                        strPar[6] = "N/A";
                        break;
                    }
                default:
                    {
                        strPar[2] = null;
                        strPar[4] = null;
                        strPar[5] = null;
                        strPar[6] = null;
                        break;
                    }
            }

            if (ret < 0)
            {
                return -1;
            }
            return 0;
        }
        private static string GetStringFrmByte(sbyte[] array)
        {
            string strByte = "";

            int len = 0;
            for (len = 0; len < array.Length; len++)
            {
                if (array[len] < 0)
                {
                    break;
                }
            }
            if (len == 0)
            {
                return strByte;
            }
            strByte += array[0];
            for (int i = 1; i < len; i++)
            {
                strByte += ",";
                strByte += array[i];
            }
            return strByte;
        }
        private int UpdateNCPara()
        {
            int nRet = 0;
            foreach (var a in ConnectionList)
            {
                var tableNCParaVer = new NCParaVer();
                tableNCParaVer.MacID = a.MacID;
                tableNCParaVer.VerTitle = "测试参数值添加";
                tableNCParaVer.VerCreateTime = DateTime.Now;
                SaveToDBIsOK(ref db, true, tableNCParaVer);
                hncData.NCParaVer tableQerVerID = new NCParaVer();
                try
                {
                    tableQerVerID = db.NCParaVer.Where(t => t.MacID == a.MacID).OrderByDescending(t => t.VerID).FirstOrDefault();
                }
                catch
                {
                    UpdateDBFlag(false);
                }
                int fileNum = 7;//系统最大分类数，大类
                for (int i = 0; i < fileNum; i++)
                {
                    int recNum = -1;//对应大类的子类分类数，小类
                    nRet = GlobalSession.HNC_ParamanGetSubClassProp(i, (Byte)ParaSubClassProp.SUBCLASS_NUM, ref recNum, 0);//根据大的分类类别0-6共7个索引，获取参数的子类的属性。属性包括：子类名，子类行数，子类数。
                    //本函数根据(Byte)ParaSubClassProp.SUBCLASS_NUM的不同，返回不同的值到ref中

                    if (recNum < 0)
                    {
                        nRet = recNum;
                        break;
                    }
                    else if (recNum > 0)
                    {
                        for (int j = 0; j < recNum; j++)
                        {
                            nRet = UpdateParaItem(i, j, a.ClientNO, a.MacID, tableQerVerID.VerID);
                            if (nRet < 0)
                            {
                                break;//当某个大类的某个小类读取出错时，跳出循环，不再继续读，返回错误代码ret。
                            }
                        }
                        SaveToDBIsOK(ref db, false, null);
                    }
                    if (nRet < 0)
                    {
                        break;
                    }
                }

            }
            return nRet;
        }
        private int UpdateParaItem(int fileNo, int recNo, short clientNo, string MacID, long lVerID)
        {
            int ret = 0;
            int rowNum = 0;
            ret = GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
            if (rowNum < 0)
            {
                return -2;//子类行数小于0的异常
            }

            ret = GlobalSession.HNC_ParamanRewriteSubClass(fileNo, recNo, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (ret < 0)
            {
                return -3;//加载子类时出错的异常
            }

            string[] strPar = new string[8];//读出每条参数的暂存数组
            int myEffectWay = 999;
            int myDataType = 999;
            int index1 = -1;



            for (int i = 0; i < rowNum; i++)//把每行参数读取出来，放在strPtr数组中，每一个数组元素对应当前行参数的某列数据
            {
                ret = GetParContent(fileNo, recNo, i, clientNo, strPar, ref myEffectWay, ref myDataType, ref index1);//（大类号，子类号，当前列号，clientNo，字符串数组）
                if (ret == -1)
                {
                    break; //在读某一条参数的内容时出现异常，跳出，不再继续读该小分类的参数
                }

                //写当前值表
                var writeNcNow = new NCParameter();
                writeNcNow.ParaCode = strPar[0];
                writeNcNow.MacID = MacID;
                if (strPar[2].Length > 32)
                {
                    strPar[2] = strPar[2].Substring(0, 32);
                }
                writeNcNow.NowVal = strPar[2];
                writeNcNow.ChangeTime = DateTime.Now;
                writeNcNow.ParaVerID = lVerID;
                //SaveToDBIsOK(ref db, true, writeNcNow);
            }
            return ret;
        }
        private static int paraGetVal(int fileNo, int recNo, string ParaCode, short clientNo, ref string sParaVal)
        {
            int nRet = -1;
            int rowNum = 0;

            nRet = GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
            if (nRet != 0)
            {
                return nRet;//子类行数小于0的异常
            }

            nRet = GlobalSession.HNC_ParamanRewriteSubClass(fileNo, recNo, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (nRet != 0 && nRet != 1)
            {
                return nRet;//加载子类时出错的异常
            }

            for (int i = 0; i < rowNum; i++)//把每行参数读取出来
            {
                Int16 dupNum = 0;//参数重复个数
                Int16 dupNo = 0;//参数重复编号
                Int32 index = -1;//参数的索引值

                nRet = GlobalSession.HNC_ParamanTransRow2Index(fileNo, recNo, i, ref index, ref dupNum, ref dupNo, clientNo);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
                if (nRet != 0)
                {
                    return nRet;
                }

                Int32 parmID = -1;
                nRet = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
                if (nRet != 0)
                {
                    return nRet;
                }

                if (parmID.ToString("D6") == ParaCode)
                {
                    Int32 storeType = -1;
                    nRet = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_STORE, ref storeType, clientNo);
                    if (nRet != 0)
                    {
                        return nRet;
                    }

                    int iVal = 0;
                    double dVal = 0;

                    switch (storeType)
                    {
                        case (sbyte)PAR_STORE_TYPE.TYPE_BOOL:
                        case (sbyte)PAR_STORE_TYPE.TYPE_UINT:
                        case (sbyte)PAR_STORE_TYPE.TYPE_INT:
                            {
                                nRet = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                                if (nRet != 0)
                                {
                                    break;
                                }
                                sParaVal = iVal.ToString();
                                break; //本条case块结束
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_FLOAT:
                            {
                                nRet = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref dVal, clientNo);
                                if (nRet != 0)
                                {
                                    break;
                                }
                                sParaVal = dVal.ToString("F6");

                                break;
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_STRING:
                            {
                                nRet = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref sParaVal, clientNo);
                                if (nRet != 0)
                                {
                                    return nRet;
                                }
                                break;
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_HEX4:
                            {
                                nRet = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                                if (nRet != 0)
                                {
                                    return nRet;
                                }
                                sParaVal = "0x" + iVal.ToString("X2");
                                break;
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_BYTE:
                            {
                                sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                                nRet = GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                                if (nRet != 0)
                                {
                                    return nRet;
                                }
                                sParaVal = GetStringFrmByte(araayBt);
                                break;
                            }
                        default:
                            {
                                sParaVal = null;
                                return -1;

                            }
                    }
                    break;
                }
            }
            return 0;
        }
        //调用WebApi->Connect函数向服务器报告采集器IP地址，true->成功;false -> 失败。
        private async Task<bool> ReportDaAgentIP()
        {
          
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["WebApiURLAddress"]);
                string sVersionTemp = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = null;
                response = await client.GetAsync("DataAgent/Connect?sAgentCode=" + AgentCode + "&sPort=" + AgentRemotingPort + "&sVersion=" + sVersionTemp).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    //this.tb_log.Text += "采集器向服务器报告IP出错！时间：" + DateTime.Now.ToString() + "\r\n";
                    return false;
                }
            }
            catch
            {
                //this.tb_log.Text += "采集器向服务器报告IP出错！时间：" + DateTime.Now.ToString() + "\r\n";
                return false;
            }
            return true;
        }
        /// <summary>
        /// 写入MacStateLog中字段DescText不能为空
        /// </summary>
        /// <param name="state">机床状态</param>
        /// <param name="descText">状态显示</param>
        /// <returns></returns>
        private string DescTextNotNull(int state, string descText)
        {
            if (descText == null)
            {
                switch (state)
                {
                    case 0:
                    case 2: descText = "空闲"; break;
                    case 1: descText = "运行"; break;
                    case 3: descText = "急停"; break;
                    case 4: descText = "加工完成"; break;
                    case 6: descText = "建立连接"; break;
                    case -1: descText = "离线"; break;
                    case -2: descText = "采集器断线"; break;
                    default: descText = "离线"; break;
                }
            }
            return descText;
        }
        /// <summary>
        /// 上传机床状态数据，更新远程连接标志位。
        /// </summary>
        /// <param name="macStateList">机床状态链表数据</param>
        private async void MacStateList2Remote(List<MacStateLog> macStateList)
        {
            int nUploadDatFlag = await UploadQueueEventDat2DB(macStateList);
            if (-2 == nUploadDatFlag || 2 == nUploadDatFlag)
            {
                UpdateDBFlag(false);
            }
        }
        //保存日志到可执行文件目录的方法
        private static void WriteLog(Type t, string msg)
        {
            ILog log = log4net.LogManager.GetLogger(t);
            log.Info(msg);
        }
        private void OutputMessage2Console(string sMess2Console)
        {
            Console.Write(DateTime.Now.ToString() + " " + sMess2Console);

            Monitor.WriteLog(typeof(Monitor), DateTime.Now.ToString() + " " + sMess2Console);
        }
        private void UpdateDBFlag(bool bDBTestIsOK)
        {
            if (bDBTestIsOK)
            {
                OutputMessage2Console("【数据库】远程数据库状态->正常\n");
            }
            else
            {
                RemoteDBIsReady = false;
                OutputMessage2Console("【数据库】远程数据库状态->离线\n");
            }
        }
        private void WrtMacSta2Console()
        {
            if (ConnectionList != null)
            {
                foreach (var a in ConnectionList)
                {
                    if (a.Connected)
                    {
                        OutputMessage2Console("【连接状态】" + "MacID:" + a.MacID + "->已连接！\n" + "IP:" + a.IP
                            + " Port:" + a.Port.ToString() + " ClientNo:" + a.ClientNO.ToString() + " SN:" + a.m_sMachineSNCode + " STE:" + a.m_nMachineState + " NCVer:" + a.m_sNCVer + "\n");
                    }
                    else
                    {
                        OutputMessage2Console("【连接状态】" + "MacID:" + a.MacID + "->离线！\n" + "IP:" + a.IP
                            + " Port:" + a.Port.ToString() + " ClientNo:" + a.ClientNO.ToString() + " SN:" + a.m_sMachineSNCode + " STE:" + a.m_nMachineState + " NCVer:" + a.m_sNCVer + "\n");
                    }
                }
            }
        }
        private bool UpdateOfflineMacLst2Sqlite(List<NCMachines> lstLastMacLst)
        {
            if (null == lstLastMacLst) return false;
            if (m_SqliteDB.ClearTable("SqliteNCMachines"))
            {
                foreach (var a in lstLastMacLst)
                {
                    m_SqliteDB.ExecuteNonQuery("insert into SqliteNCMachines values(" + a.ID.ToString() + ",'" + a.MacID + "','" + a.MacSN + "','" + a.IPAddress + "','" + a.DCAgentPort + "','" + a.NCLogConfig + "')");
                }
                OutputMessage2Console("【数据库】获取远程机床列表->正常 共" + lstLastMacLst.Count.ToString() + "条\n");
            }
            else
            {
                OutputMessage2Console("【数据库】离线数据库->SqliteNCMachines操作异常！\n");
                return false;
            }
            return true;
        }
        private async void WebService_Tick(object sender, EventArgs e)
        {
            //-----------------------------------------------告知服务器采集器状态--------------------------------start

            bool bDBFlagIsOK = false;
            try
            {
                this.WebServiceTimer.Enabled = false;
                OutputMessage2Console("----->HEARTBEAT<-----【WebApi】正在向服务器报告IP认证...\n");
                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["WebApiURLAddress"]);
                string sVersionTemp = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = null;
                response = await client.GetAsync("DataAgent/Connect?sAgentCode=" + AgentCode + "&sPort=" + AgentRemotingPort + "&sVersion=" + sVersionTemp).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    OutputMessage2Console("【WebApi】采集器向服务器报告IP认证->失败！\n");
                }
                bDBFlagIsOK = true;
                OutputMessage2Console("【WebApi】采集器向服务器报告IP认证->成功！\n");
            }
            catch
            {
                bDBFlagIsOK = false;
                OutputMessage2Console("【WebApi】采集器向服务器报告IP->发生异常！\n");
            }
            //-----------------------------------------------告知服务器采集器状态--------------------------------end

            if (bDBFlagIsOK)
            {
                UpdateDBFlag(true);
                if (!(RemoteDBIsReady && OffDatIsAllUpdated))
                {
                    UploadOfflineData();
                    RemoteDBIsReady = true;//<---如果上面上传出错，只打开数据库正常标志位
                }
               await UpdateMacConnectionList();
            }
            else
                UpdateDBFlag(false);
            WrtMacSta2Console();
            if (RemoteDBIsReady)
            {
                this.WebServiceTimer.Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServTmrInter"]);
            }
            else
            {
                this.WebServiceTimer.Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServTmrInter_OffLine"]);
            }
            if (m_bWriteNCLog)
                NCLogService();
            this.WebServiceTimer.Enabled = true;
        }

        private object _lockNCStateChanged = new object();
        //-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
        private string[] _StrNcStateDescrip = new string[] { "离线状态","一般状态（空闲状态）","循环启动（运行状态）","进给保持（空闲状态）","急停状态（报警状态）"};
        public void NCStateChange(int nState,string MacID,string MacSn,DateTime tHappendTime)
        {
            if((-1 > nState)||(3 < nState))
            {
                return;
            }
            MacStateLog openlog = null;

            try
            {
                lock (_lockNCStateChanged)
                {
                    var macSt = ConnectionList.Where(t => t.m_sMachineSNCode == MacSn).GetEnumerator();
                    if (!macSt.MoveNext())
                    {
                        return;
                    }

                    if(String.IsNullOrEmpty(macSt.Current.MacID))
                    {
                        macSt.Current.MacID = MacID;
                    }

                    macSt.Current.m_nMachineState = nState;
                    openlog = GetForMacStateLog(macSt.Current, nState);

                    openlog.Operator += "E";
                    openlog.DescText = _StrNcStateDescrip[nState + 1];
                    macSt.Current.m_dtCreaTimeOfState = (null == tHappendTime) ? DateTime.Now : tHappendTime;
                    macSt.Current.m_sRunGCode = openlog.GcodeName;

                    //if (RemoteDBIsReady && OffDatIsAllUpdated)
                    //{
                    //    macStateLogList.AddLog(openlog);
                    //}
                    //else
                    //{
                    //    WrtRecord2LocalDB(openlog);
                    //}
                }

                //if ((null != openlog)&&(null != db))
                if (null != openlog)
                {
                    System.Threading.Tasks.Task.Run(()=> UploadEventDat2DB(openlog));
                    //SaveToDBIsOK(ref db, true, openlog);
                }
            }
            catch(Exception ex)
            {
                OutputMessage2Console("NCStateChange:" + ex.StackTrace);
            }
        }


        private object _lockProcessObj = new object();
        //现有仅检测部分事件代码，且把进给保持当作空闲对待。保存报警号、报警内容等信息的时候，可能也存在不需要取的地方取值。
        public void Process(SEventElement ev)
        {

            short[] info = new short[2];

            if ((null == ConnectionList) || (1 > ConnectionList.Count)) //1216是空事件
            {
                return;
            }

            int ret = 0;
            lock (_lockProcessObj)
            {
                switch (ev.code)
                {
                    case HNCAPI.EVENTDEF.ncEvtConnect://新建连接消息
                        {
                            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                            short nClientNo = info[0];
                            if (HNCAPI.EVENTDEF.EV_SRC_NET != ev.src || nClientNo < 0 || nClientNo >= 255) break;

                            bool bCheckFrmIPAdd = true;
                            string tempStr = "";
                            UInt16 tempUS = 0;
                            string sMacSNTemp = "";

                            int ret1 = GlobalSession.HNC_NetGetIpaddr(ref tempStr, ref tempUS, nClientNo);//通过clientNo获取IP地址和端口号
                            if (ret1 != 0)
                            {
                                OutputMessage2Console("【事件进程】建立连接获取IP地址失败！" + " ClientNo:" + nClientNo.ToString() + "\n");
                                break;
                            }
                            ret1 = GlobalSession.HNC_SystemGetValue((int)HncSystem.HNC_SYS_SN_NUM, ref sMacSNTemp, nClientNo);//通过clientNo获取SN码
                            if (ret1 != 0)
                            {
                                OutputMessage2Console("【事件进程】建立连接获取SN码失败！" + " ClientNo:" + nClientNo.ToString() + "\n");
                                break;
                            }
                            if ("" == sMacSNTemp) sMacSNTemp = "空";

                            var a = ConnectionList.Where(t => t.IP == tempStr).FirstOrDefault();
                            if (null == a)//当前数据库中没有该机床
                            {
                                a = ConnectionList.Where(t => t.m_sMachineSNCode == sMacSNTemp).FirstOrDefault();
                                if (null == a)
                                {
                                    OutputMessage2Console("【事件进程】未知SN->" + sMacSNTemp + " IP:" + tempStr + " Port:" + tempUS.ToString() + " ClientNo:" + nClientNo.ToString() + " SN:" + sMacSNTemp + "\n");
                                    break;
                                }
                                bCheckFrmIPAdd = false;
                            }

                            if (null != a && !String.IsNullOrEmpty(a.MacID))
                            {
                                //Queue<MacStateLog> queConDat = new Queue<MacStateLog>();
                                //更改当前应用程序的ConnectionList状态
                                a.Connected = true;
                                a.ClientNO = nClientNo;//存储ClientNo
                                if (bCheckFrmIPAdd)
                                    OutputMessage2Console("【事件进程】" + "MacID:" + a.MacID + "->通过IP地址连接成功！" + "IP:" + tempStr + " Port:" + tempUS.ToString() + " ClientNo:" + a.ClientNO.ToString() + " SN:" + sMacSNTemp + " NCVer:" + a.m_sNCVer + "\n");
                                else
                                    OutputMessage2Console("【事件进程】" + "MacID:" + a.MacID + "->通过SN码连接成功！" + "IP:" + tempStr + " Port:" + tempUS.ToString() + "  ClientNo:" + a.ClientNO.ToString() + " SN:" + sMacSNTemp + " NCVer:" + a.m_sNCVer + "\n");

                                //delete by leomei
                                ////if (0 != WrtAxisInfo2Conlst(ref a))
                                ////{
                                ////    OutputMessage2Console("【事件进程】" + "MacID:" + a.MacID + "->读取轴信息出错！\n");
                                ////    break;
                                ////}

                                List<MacStateLog> tbList = new List<MacStateLog>();
                                MacStateLog tbNewCon = new MacStateLog();
                                tbNewCon.CreateTime = DateTime.Now;//新状态的生成时间
                                tbNewCon.CmpID = CmpID;
                                tbNewCon.MacID = a.MacID;
                                tbNewCon.State = 6;/* 6->仅仅只是表示建立连接*/
                                tbNewCon.DescText = "连接建立";
                                tbNewCon.Operator = m_sMacStaLogOperator + "E";
                                //queConDat.Enqueue(tbNewCon);
                                tbList.Add(tbNewCon);
                                //OutputMessage2Console("【事件进程】MacID:" + a.MacID + tbNewCon.DescText + "IP:" + a.IP + " Port:" + a.Port + "\n");

                                int nStateTemp = GetStateValue(a.ClientNO);
                                MacStateLog openlog = GetForMacStateLog(a, nStateTemp);
                                openlog.Operator += "E";

                                string sNCVer = "";
                                ret = GlobalSession.HNC_SystemGetValue((int)HncSystem.HNC_SYS_NC_VER, ref sNCVer, a.ClientNO); //NC版本
                                if (0 == ret)
                                    a.m_sNCVer = sNCVer;

                                a.m_nMachineState = nStateTemp;//更新Connectlist中的机床状态
                                a.m_dtCreaTimeOfState = openlog.CreateTime;
                                a.m_sRunGCode = openlog.GcodeName;
                                switch (nStateTemp)
                                {
                                    case 0:
                                    case 2: openlog.DescText = "空闲"; break;
                                    case 1: openlog.DescText = "运行"; break;
                                    default: break;
                                }
                                openlog.DescText = DescTextNotNull(nStateTemp, openlog.DescText);
                                //queConDat.Enqueue(openlog);
                                tbList.Add(openlog);
                                //OutputMessage2Console("【事件进程】MacID:" + a.MacID + openlog.DescText + "IP:" + a.IP + " Port:" + a.Port + "\n");
                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLogRange(tbList);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(tbList);
                                }
                            }
                            break;
                        }

                    case HNCAPI.EVENTDEF.ncEvtDisConnect:
                        {
                            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                            short nClientNo = info[0];
                            if (HNCAPI.EVENTDEF.EV_SRC_NET != ev.src || nClientNo < 0 || nClientNo >= 255) break;

                            var a = ConnectionList.Where(t => t.ClientNO == nClientNo).FirstOrDefault();
                            if (null != a && !String.IsNullOrEmpty(a.MacID))
                            {
                                var openlog = new MacStateLog();
                                openlog.CreateTime = DateTime.Now;
                                openlog.MacID = a.MacID;
                                openlog.State = -1;
                                openlog.CmpID = CmpID;
                                openlog.Operator = m_sMacStaLogOperator + "E";    //测试点，以后删除整行！！！
                                openlog.DescText = "离线";

                                a.m_nMachineState = -1;//更新Connectlist中的机床状态
                                a.m_dtCreaTimeOfState = openlog.CreateTime;
                                a.m_sRunGCode = openlog.GcodeName;
                                a.Connected = false;//断线设置状态
                                a.ClientNO = -1;//还原ClientNo到-1
                                OutputMessage2Console("【事件进程】MacID:" + a.MacID + "->因网络故障断线！" + "IP:" + a.IP + " Port:" + a.Port + "\n");

                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(openlog);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(openlog);
                                }
                            }
                            break;
                        }

                    case HNCAPI.EVENTDEF.ncEvtEstop:
                    case HNCAPI.EVENTDEF.ncEvtNckAlarm:
                    case HNCAPI.EVENTDEF.ncEvtFaultIrq:
                        {
                            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                            short nClientNo = info[0];
                            if (nClientNo < 0 || nClientNo >= 255) break;

                            var a = ConnectionList.Where(t => t.ClientNO == nClientNo).FirstOrDefault();
                            if (null != a && !String.IsNullOrEmpty(a.MacID))
                            {
                                //报警状态下的描述，在GetForMacStateLog函数中取得。
                                MacStateLog openlog = GetForMacStateLog(a, 3);
                                openlog.Operator += "E";
                                a.m_nMachineState = 3;//更新Connectlist中的机床状态
                                a.m_dtCreaTimeOfState = openlog.CreateTime;
                                a.m_sRunGCode = openlog.GcodeName;
                                openlog.DescText = DescTextNotNull(3, openlog.DescText);
                                OutputMessage2Console("【事件进程】MacID:" + a.MacID + openlog.DescText + "IP:" + a.IP + " Port:" + a.Port + "\n");

                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(openlog);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(openlog);
                                }
                            }
                            break;
                        }

                    case HNCAPI.EVENTDEF.ncEvtAlarmChg://报警消除处理，报警产生抛弃
                        {
                            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                            short nClientNo = info[0];
                            if (nClientNo < 0 || nClientNo >= 255) break;


                            int eStop = 0;
                            ret = GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_ESTOP, 0, 0, ref eStop, nClientNo);//是否急停 3
                            if (0 == eStop && 0 == ret)
                            {
                                var a = ConnectionList.Where(t => t.ClientNO == nClientNo).FirstOrDefault();
                                if (null != a && !String.IsNullOrEmpty(a.MacID))
                                {
                                    MacStateLog openlog = GetForMacStateLog(a, 0);
                                    openlog.Operator += "E";
                                    openlog.DescText = "解除报警";
                                    a.m_nMachineState = 0;//更新Connectlist中的机床状态
                                    a.m_dtCreaTimeOfState = openlog.CreateTime;
                                    a.m_sRunGCode = openlog.GcodeName;
                                    OutputMessage2Console("【事件进程】MacID:" + a.MacID + "->解除报警！" + "IP:" + a.IP + " Port:" + a.Port + "\n");

                                    if (RemoteDBIsReady && OffDatIsAllUpdated)
                                    {
                                        macStateLogList.AddLog(openlog);
                                    }
                                    else
                                    {
                                        WrtRecord2LocalDB(openlog);
                                    }
                                }
                            }
                            break;
                        }

                    case HNCAPI.EVENTDEF.ncEvtPrgStart:
                        {
                            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                            short nClientNo = info[0];
                            if (nClientNo < 0 || nClientNo >= 255) break;


                            var a = ConnectionList.Where(t => t.ClientNO == nClientNo).FirstOrDefault();
                            if (null != a && !String.IsNullOrEmpty(a.MacID))
                            {
                                MacStateLog openlog = GetForMacStateLog(a, 1);
                                openlog.Operator += "E";
                                openlog.DescText = "程序运行";

                                a.m_nMachineState = 1;//更新Connectlist中的机床状态
                                a.m_dtCreaTimeOfState = openlog.CreateTime;
                                a.m_sRunGCode = openlog.GcodeName;
                                OutputMessage2Console("【事件进程】MacID:" + a.MacID + "->程序运行！" + "IP:" + a.IP + " Port:" + a.Port + "\n");

                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(openlog);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(openlog);
                                }
                            }
                            break;
                        }

                    case HNCAPI.EVENTDEF.ncEvtPrgEnd:
                        {
                            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                            short nClientNo = info[0];
                            if (nClientNo < 0 || nClientNo >= 255) break;


                            var a = ConnectionList.Where(t => t.ClientNO == nClientNo).FirstOrDefault();
                            if (null != a && !String.IsNullOrEmpty(a.MacID))
                            {
                                MacStateLog openlog = GetForMacStateLog(a, 0);
                                openlog.Operator += "E";
                                openlog.DescText = "程序运行结束";
                                a.m_nMachineState = 0;//更新Connectlist中的机床状态
                                a.m_dtCreaTimeOfState = openlog.CreateTime;
                                a.m_sRunGCode = openlog.GcodeName;
                                OutputMessage2Console("【事件进程】MacID:" + a.MacID + "->程序运行结束！" + "IP:" + a.IP + " Port:" + a.Port + "\n");

                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(openlog);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(openlog);
                                }
                            }
                            break;
                        }
                    case HNCAPI.EVENTDEF.ncEvtPrgHold://进给保持的时候，状态暂时定义为空闲状态->0
                        {
                            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                            short nClientNo = info[0];
                            if (nClientNo < 0 || nClientNo >= 255) break;


                            var a = ConnectionList.Where(t => t.ClientNO == nClientNo).FirstOrDefault();
                            if (null != a && !String.IsNullOrEmpty(a.MacID))
                            {
                                MacStateLog openlog = GetForMacStateLog(a, 0);
                                openlog.Operator += "E";
                                openlog.DescText = "进给保持";
                                a.m_nMachineState = 0;//更新Connectlist中的机床状态
                                a.m_dtCreaTimeOfState = openlog.CreateTime;
                                a.m_sRunGCode = openlog.GcodeName;
                                OutputMessage2Console("【事件进程】MacID:" + a.MacID + "->进给保持！" + "IP:" + a.IP + " Port:" + a.Port + "\n");

                                if (RemoteDBIsReady && OffDatIsAllUpdated)
                                {
                                    macStateLogList.AddLog(openlog);
                                }
                                else
                                {
                                    WrtRecord2LocalDB(openlog);
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

        }



        private class AsyncResultObj
        {
            public int Rst;
            public int UploadedCnt;
        }
        /// <summary>
        /// 将离线数据存入链表，然后传入服务器
        /// </summary>
        /// <param name="nUploadedCnt">上传成功的条数</param>
        /// <returns>0：上传成功；-1：链表没有数据；-2：远程访问失败</returns>
        private async Task<AsyncResultObj> UploadOfflineDa2List()
        {
            AsyncResultObj asyncRst = new AsyncResultObj();
            asyncRst.UploadedCnt = 0;
            asyncRst.Rst = -1;
            try {

                //Queue<MacStateLog> queOfflineDa = new Queue<MacStateLog>();
                List<MacStateLog> listOfflineDa = new List<MacStateLog>();
                int nPerCntForUploadTbl = int.Parse(System.Configuration.ConfigurationManager.AppSettings["nPerCntForUploadTbl"]);
                int[] naIDForSqliteDB = new int[nPerCntForUploadTbl];//暂存Sqliite中上传记录的ID
                int nIDArrayIndex = 0;
                //把本地数据上传远程

                if (0 < m_SqliteDB.QerTableIsExist("SqliteLocalData"))
                {
                    DataTable dt = m_SqliteDB.GetDataTable("SELECT * FROM SqliteLocalData WHERE Type = '2' AND IsUploaded = 0  ORDER BY ID ASC LIMIT " + nPerCntForUploadTbl.ToString());
                    if (0 < dt.Rows.Count)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            List<MacStateLog> listOfflineDaTP = new List<MacStateLog>();
                            naIDForSqliteDB[nIDArrayIndex++] = Convert.ToInt32(dt.Rows[i][0]);
                            listOfflineDaTP = JsonConvert.DeserializeObject<List<MacStateLog>>(dt.Rows[i][3].ToString());
                            listOfflineDa.AddRange(listOfflineDaTP);
                        }
                        macStateLogList.AddLogRange(listOfflineDa);
                        asyncRst.UploadedCnt = dt.Rows.Count;
                    }
                }
                if (null != MacStateList && MacStateList.Count > 0)
                {
                    int nDatIsWrted2DB = 0;
                    MacStateList.Sort(delegate (MacStateLog x, MacStateLog y) { return x.CreateTime.CompareTo(y.CreateTime); });
                    List<MacStateLog> listTemp = new List<MacStateLog>();
                    listTemp.AddRange(MacStateList);
                    HttpResponseMessage result = await WriteDat2MacStateLog(listTemp);
                    if (null != result && result.IsSuccessStatusCode)
                    {
                        string sWrted2DBFlag = await result.Content.ReadAsAsync<string>().ConfigureAwait(false);
                        string sWrted2DBFlagFr = sWrted2DBFlag.Substring(0, 1);
                        if (sWrted2DBFlagFr == "E" || sWrted2DBFlagFr == "S" || sWrted2DBFlagFr == "K")
                        {
                            nDatIsWrted2DB = Convert.ToInt32(sWrted2DBFlag.Substring(1));
                        }
                        if ("K" == sWrted2DBFlag.Substring(0, 1))
                            ++nDatIsWrted2DB;
                        for (int i = 0; i < asyncRst.UploadedCnt; i++)
                        {
                            m_SqliteDB.ExecuteNonQuery("UPDATE SqliteLocalData SET IsUploaded = 1 WHERE ID = '" + naIDForSqliteDB[i].ToString() + "'");
                        }
                        if (listTemp.Count == nDatIsWrted2DB)
                        {
                            OutputMessage2Console("【离线数据】把链表数据往外写->全部成功：" + nDatIsWrted2DB + "条\n");
                        }
                        else
                        {
                            OutputMessage2Console("【离线数据】把链表数据往外写->成功：" + nDatIsWrted2DB + "条--失败：" + (listTemp.Count - nDatIsWrted2DB) + "条\n");
                        }
                        macStateLogList.RemoveCount(listTemp.Count);

                        asyncRst.Rst = 0;
                        return asyncRst;
                    }
                    else
                    {
                        macStateLogList.RemoveCount(listTemp.Count);//将刚刚没有传输成功但是存到缓存链表的数据清理掉
                        asyncRst.Rst = -2;//远程连接失败
                        return asyncRst; 
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            asyncRst.Rst = 1;
            return asyncRst;
        }
        private async Task<HttpResponseMessage> WriteDat2MacStateLog(Queue<MacStateLog> queDat2MacStateLog)
        {
           
            try
            {
                string sSerializeOfflineData = JsonConvert.SerializeObject(queDat2MacStateLog);
                Byte[] tempArray = Encoding.Default.GetBytes(sSerializeOfflineData);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["WebApiURLAddress"]);

                //--------------------传数据---------------start
                var multipartFormDataContent = new MultipartFormDataContent();

                var values = new[]
                {
                new KeyValuePair<string, string>("Id", Guid.NewGuid().ToString()),
                //new KeyValuePair<string, string>("Key", "awesome"),
                //new KeyValuePair<string, string>("From", "khalid@home.com")
                //other values
            };

                foreach (var keyValuePair in values)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }

                multipartFormDataContent.Add(new ByteArrayContent(tempArray),
                            '"' + "File" + '"',
                            '"' + "test137m1139" + '"');

                var requestUri = System.Configuration.ConfigurationManager.AppSettings["WebApiURLAddress"] + "Machine/SaveMacStateLog";
                var result = await client.PostAsync(requestUri, multipartFormDataContent).ConfigureAwait(false);
                return result;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
            //-----------------------传数据---------------end            
        }
        /// <summary>
        /// 将机床状态链表数据传给服务器
        /// </summary>
        /// <param name="listDat2MacStateLog">机床状态链表</param>
        /// <returns>HttpResponseMessage</returns>
        private async Task<HttpResponseMessage> WriteDat2MacStateLog(List<MacStateLog> listDat2MacStateLog)
        {
           
            try
            {
                string sSerializeOfflineData = JsonConvert.SerializeObject(listDat2MacStateLog);
                Byte[] tempArray = Encoding.Default.GetBytes(sSerializeOfflineData);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["WebApiURLAddress"]);

                //--------------------传数据---------------start
                var multipartFormDataContent = new MultipartFormDataContent();

                var values = new[]
                {
                new KeyValuePair<string, string>("Id", Guid.NewGuid().ToString())
                };

                foreach (var keyValuePair in values)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }

                multipartFormDataContent.Add(new ByteArrayContent(tempArray),
                            '"' + "File" + '"',
                            '"' + "test137m1139" + '"');

                var requestUri = System.Configuration.ConfigurationManager.AppSettings["WebApiURLAddress"] + "Machine/SaveMacStateLog";
                var result = await client.PostAsync(requestUri, multipartFormDataContent).ConfigureAwait(false);
                return result;
            }
            catch (Exception e)
            {
                Console.Write("WriteDat2MacStateLog:" + e.Message);
                return null;
            }
            //-----------------------传数据---------------end            
        }
        private async Task<int> UploadEventDat2DB(MacStateLog tblEventDatItem)
        {
            if (null != tblEventDatItem)
            {
                try
                {
                    Queue<MacStateLog> queOfflineDa = new Queue<MacStateLog>();
                    queOfflineDa.Enqueue(tblEventDatItem);
                    if (RemoteDBIsReady)
                    {
                        HttpResponseMessage result = await WriteDat2MacStateLog(queOfflineDa);
                        if (null != result && result.IsSuccessStatusCode)
                        {
                            string sWrted2DBFlag =await result.Content.ReadAsAsync<string>().ConfigureAwait(false);
                            string SWrted2DBFlagFr = sWrted2DBFlag.Substring(0, 1);
                            int nDatIsWrted2DB = 0;
                            if (SWrted2DBFlagFr == "E" || SWrted2DBFlagFr == "S" || SWrted2DBFlagFr == "K")
                            {
                                nDatIsWrted2DB = Convert.ToInt32(sWrted2DBFlag.Substring(1));
                            }
                            if ("K" == sWrted2DBFlag.Substring(0, 1))
                                ++nDatIsWrted2DB;
                            if (queOfflineDa.Count == nDatIsWrted2DB)
                                return 0;
                        }
                    }
                    //单个事件，不需判断，远程数据库未成功，直接写入离线数据库
                    tblEventDatItem.Operator += "S";
                    string sSerializeJSON = JsonConvert.SerializeObject(tblEventDatItem);
                    if (SaveLocalRecordIsOK(2, sSerializeJSON))
                        return 2;//写入本地数据库成功，返回2
                    else
                        return -2;//写入本地数据库出错，返回-2
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            }
            return -1;
        }
        private async Task<int> UploadQueueEventDat2DB(Queue<MacStateLog> tblEventDatItem)
        {
            if (null != tblEventDatItem && tblEventDatItem.Count != 0)
            {
                int nDatIsWrted2DB = 0;
                if (RemoteDBIsReady)
                {
                    HttpResponseMessage result = await WriteDat2MacStateLog(tblEventDatItem);
                    if (null != result && result.IsSuccessStatusCode)
                    {
                        string sWrted2DBFlag =await result.Content.ReadAsAsync<string>().ConfigureAwait(false);
                        string SWrted2DBFlagFr = sWrted2DBFlag.Substring(0, 1);
                        if (SWrted2DBFlagFr == "E" || SWrted2DBFlagFr == "S" || SWrted2DBFlagFr == "K")
                        {
                            nDatIsWrted2DB = Convert.ToInt32(sWrted2DBFlag.Substring(1));
                        }
                        if ("K" == sWrted2DBFlag.Substring(0, 1))
                            ++nDatIsWrted2DB;
                        if (tblEventDatItem.Count == nDatIsWrted2DB)
                            return 0;
                    }
                }
                if (nDatIsWrted2DB > 0)
                {
                    for (int i = 0; i < nDatIsWrted2DB; i++)
                        tblEventDatItem.Dequeue();
                }
                while (0 != tblEventDatItem.Count)
                {
                    MacStateLog tblDataTemp = tblEventDatItem.Dequeue();
                    tblDataTemp.Operator += "S";
                    string sSerializeJSON = JsonConvert.SerializeObject(tblDataTemp);
                    if (!SaveLocalRecordIsOK(2, sSerializeJSON))
                        return -2;
                }
                return 2;
            }
            return -1;
        }
        /// <summary>
        /// 上传机床状态链表数据函数
        /// </summary>
        /// <param name="tblEventDatItem">链表数据</param>
        /// <returns>2或-2：远程连接失败；0：远程连接正常，上传数据正常；-1：链表没有数据</returns>
        private async Task<int> UploadQueueEventDat2DB(List<MacStateLog> tblEventDatItem)
        {
            try
            { 

            if (null != tblEventDatItem && tblEventDatItem.Count > 0)
            {
                int nDatIsWrted2DB = 0;
                //IEnumerable<MacStateLog> dataList = null;
                //var dataList = from t in tblEventDatItem orderby t.CreateTime descending select t;
                //var p = typeof(MacStateLog).GetProperty("CreateTime");
                //tblEventDatItem.Select(i => p.GetValue(i).ToString()).ToList();
                tblEventDatItem.Sort(delegate (MacStateLog x, MacStateLog y) { return x.CreateTime.CompareTo(y.CreateTime); });
                List<MacStateLog> listTemp = new List<MacStateLog>();
                listTemp.AddRange(tblEventDatItem);
                if (RemoteDBIsReady)
                {

                    HttpResponseMessage result = await WriteDat2MacStateLog(listTemp);
                    if (null != result && result.IsSuccessStatusCode)
                    {
                        string sWrted2DBFlag = await result.Content.ReadAsAsync<string>().ConfigureAwait(false);
                        string sWrted2DBFlagFr = sWrted2DBFlag.Substring(0, 1);
                        if (sWrted2DBFlagFr == "E" || sWrted2DBFlagFr == "S" || sWrted2DBFlagFr == "K")
                        {
                            nDatIsWrted2DB = Convert.ToInt32(sWrted2DBFlag.Substring(1));
                        }
                        if ("K" == sWrted2DBFlag.Substring(0, 1))
                            ++nDatIsWrted2DB;
                        if (listTemp.Count == nDatIsWrted2DB)
                        {
                            OutputMessage2Console("【实时数据】把链表数据往外写->全部成功：" + tblEventDatItem[0].DescText + nDatIsWrted2DB + "条\n");
                        }
                        else
                        {
                            OutputMessage2Console("【实时数据】把链表数据往外写->成功：" + tblEventDatItem[0].DescText + nDatIsWrted2DB + "条--失败：" + (tblEventDatItem.Count - nDatIsWrted2DB) + "条\n");
                        }
                        macStateLogList.RemoveCount(listTemp.Count); //
                        return 0;
                    }
                    else
                    {
                        return -2; //远程连接失败,更改远程标志位
                    }
                }
                return 2;
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }
        private int WrtRecord2LocalDB(Queue<MacStateLog> tblEventDatItem)
        {
            if (null == tblEventDatItem)
                return -1;
            while (0 != tblEventDatItem.Count)
            {
                MacStateLog tblDataTemp = tblEventDatItem.Dequeue();
                tblDataTemp.Operator += "S";
                string sSerializeJSON = JsonConvert.SerializeObject(tblDataTemp);
                if (!SaveLocalRecordIsOK(2, sSerializeJSON))
                    return -2;
            }
            return 0;
        }
        /// <summary>
        /// 将机床状态链表存入sqlite数据库
        /// </summary>
        /// <param name="tblEventDatItem"></param>
        /// <returns></returns>
        private int WrtRecord2LocalDB(List<MacStateLog> tblEventDatItem)
        {
            if (null == tblEventDatItem)
                return -1;
            if (0 < tblEventDatItem.Count)
            {
                List<MacStateLog> tblDataTempList = new List<MacStateLog>();
                for (int i = 0; i < tblEventDatItem.Count; i++)
                {
                    MacStateLog tblDataTemp = new MacStateLog();
                    tblDataTemp = tblEventDatItem.ElementAt(i);
                    tblDataTemp.Operator += "S";
                    tblDataTempList.Add(tblDataTemp);
                }
                macStateLogList.RemoveLogRange(tblDataTempList);
                string sSerializeJSON = JsonConvert.SerializeObject(tblDataTempList);
                if (!SaveLocalRecordIsOK(2, sSerializeJSON))
                    return -2;
            }
            return 0;
        }
        /// <summary>
        /// 将单条机床状态数据存入sqlite数据库
        /// </summary>
        /// <param name="tblEventDatItem"></param>
        /// <returns></returns>
        private int WrtRecord2LocalDB(MacStateLog tblEventDatItem)
        {
            if (null == tblEventDatItem)
                return -1;
            tblEventDatItem.Operator += "S";
            List<MacStateLog> listTemp = new List<MacStateLog>();
            listTemp.Add(tblEventDatItem);
            string sSerializeJSON = JsonConvert.SerializeObject(listTemp);
            if (SaveLocalRecordIsOK(2, sSerializeJSON))
                return 0;//写入本地数据库成功，返回0
            else
                return -2;//写入本地数据库出错，返回-2
        }
        #endregion

    }
}