using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HNCAPI.Data;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using ServiceStack.Redis;

namespace HNCAPI
{
    internal partial class Machine : ISubscriber
    {
        public delegate void Eventhandler(SEventElement e, Int16 clientNo);
        public event Eventhandler EventAvailable;
        private System.Collections.Generic.Dictionary<Int32, Queue<SampleData>> _SampleDic;
        private delegate void CloudUploadhandler();
        private delegate void LocalUploadhandler();
        private delegate void InitHandler();
        private delegate void ProgramNameHandler();
        private static object _DBLocker = new object();
        private static Int32 _MacSize = 1;
        private LocalSampleDataWriter _writer;
        private bool _WriteLocal;
        private RedisPubSub _MessagePipe;
        private List<RealTimeDataPublisher> _Publisher;
        private Int32 _CloudDB;
        private Int32 _LocalDB;
        private Int16 _ClientNo;
        private String _MachineIp;
        private UInt16 _MachinePort;
        private Job _Job;
        private SampleSet _SampleSet;
        private String _ProgPath;
        private object _StopLocker;
        private Dictionary<Int32, String> _ProgramIdToName;
        private bool _ScanProgramName;
        private List<SampleConfigure> _conf;
        private HNCAPI.Data.ToolBrokenInfo _info;
        private PooledRedisClientManager _Manager;
        private bool _NeedSample;
        public System.String ProgPath
        {
            get { return _ProgPath; }
            set { _ProgPath = value; }
        }
        public SampleSet SmpSet
        {
            get { return _SampleSet; }
            set { _SampleSet = value; }
        }
        private HNCAPI.Job Job
        {
            get { return _Job; }
            set { _Job = value; }
        }
        public System.String Ip
        {
            get { return _MachineIp; }
            set { _MachineIp = value; }
        }
        public System.UInt16 Port
        {
            get { return _MachinePort; }
            set { _MachinePort = value; }
        }
        public Int16 ClientNo
        {
            get { return _ClientNo; }
            set { _ClientNo = value; }
        }
        private String _MachineSN;
        public System.String MachineSN
        {
            get
            {
                return _MachineSN;
            }
        }

        public int CloudDB
        {
            get
            {
                return _CloudDB;
            }

            set
            {
                _CloudDB = value;
            }
        }
        public int LocalDB
        {
            get
            {
                return _LocalDB;
            }

            set
            {
                _LocalDB = value;
            }
        }
        public Machine(Int16 ClientNo, String ip, UInt16 port, SampleSet set, bool needSample)
        {
            _MacSize++;
            if (_MacSize > 18)
            {
                int ik = 0;
                int j = ik + 1;
            }
            _NeedSample = needSample;
            _StopLocker = new Object();
            _ClientNo = ClientNo;
            _MachineIp = ip;
            _MachinePort = port;
            _SampleDic = new Dictionary<int, Queue<SampleData>>();
            _SampleSet = set;
            _Job = new Job(this);
            _ProgPath = "";
            LocalDB = -1;
            CloudDB = -1;
            _ProgramIdToName = new Dictionary<int, string>();
            _ScanProgramName = true;
            if (needSample)
            {
                for (Int32 i = 56; i <= 66; i++)
                {
                    _ProgramIdToName.Add(i, "");
                }
                _conf = set.GetSampleConfig();
                for (int i = 0; i < _conf.Count; i++)
                {
                    ComponentSample sample = new ComponentSample(_conf[i]);
                    _Job.SampleDic.Add(_conf[i].Channel, sample);
                }
                set.SetSamplConfig(_ClientNo);
                HncApi.HNC_SamplSubscibeData(_ClientNo);
                InitHandler handler = new InitHandler(Init);
                IAsyncResult result = handler.BeginInvoke(new AsyncCallback(InitFinished), "Init finished!");
            }
        }
        private RedisClient getClient()
        {
            try
            {
                if (_Manager == null)
                {
                    String Redisip = System.Configuration.ConfigurationManager.AppSettings["RedisUri"];
                    Int32 Redisport = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RedisPort"]);
                    _Manager = new PooledRedisClientManager(10, 3000, new String[] { Redisip + ":" + Redisport.ToString() });
                    //_Manager.Start();
                }
                RedisClient client = (RedisClient)_Manager.GetClient();

                if (client.Ping()) return client;
            }
            catch (Exception e)
            {
                Console.WriteLine("getClientLocal" + e.Message);
            }
            return null;

        }
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            StopUpload();
        }
        private void MessagePipe_OnToolBroken(HNCAPI.Data.ToolBrokenInfo info)
        {
            _info = info;
            this.SetToolBroken(info.ToolNo);
        }
        private void MessagePipe_OnSubScribeSampleData(SampleMessge msg)
        {
            for (int i = 0; i < this._Publisher.Count; i++)
            {
                if (_Publisher[i].Channel == msg.Channel) return;
            }
            RealTimeDataSubscriber subscriber = new RealTimeDataSubscriber(msg.Channel, msg.ComponentId, msg.DateType);
            RealTimeDataPublisher publisher = new RealTimeDataPublisher(subscriber);
            _Publisher.Add(publisher);
        }
        private void SaveDataToLocalRedis()
        {
            try
            {
                while (_WriteLocal&&_writer!=null)
                {
                    List<SampleCode> list = Job.CreateLines();
                    if (list != null)
                    {
                        for (int i = 0; i < _Publisher.Count; i++)
                        {
                            _Publisher[i].Publish(list);
                        }
                        _writer.WriteToDatabase(list);
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveDataToLocalRedis" + ex.Message);
            }
        }
        private void UploadToCloud()
        {
            while (CloudDB > -1&&_LocalDB>-1)
            {
                try
                {
                    using (ServiceStack.Redis.RedisClient client = getClient())
                    {
                        if (!client.Ping()) continue;
                        client.Db = CloudDB;
                        if(client.Exists("Machine")==0)
                        {
                            RegisterConfigure(client);
                        }
                        ServiceStack.Redis.RedisClient localclient = new ServiceStack.Redis.RedisClient("127.0.0.1", 6379);
                        localclient.Db = LocalDB;
                        if(localclient.Exists("Machine")==0)
                        {
                            RegisterConfigure(localclient);
                        }
                        if (localclient.LLen("SampleData") > 99)
                        {
                            List<String> listData = localclient.GetRangeFromList("SampleData", 0, 99);
                            using (var trans = client.CreatePipeline())              //Calls 'MULTI'
                            {
                                for (int i = 0; i < listData.Count; i++)
                                {
                                    trans.QueueCommand(r => r.AddItemToList("SampleData", listData[i]));
                                }
                                trans.Flush();
                            }
                            localclient.LTrim("SampleData", listData.Count, -1);
                        }
                        UploudGCodeToCloud(client, localclient);
                    } 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Upload to cloud:" + ex.Message);
                }
                System.Threading.Thread.Sleep(10);
            }
        }
        private void UploudGCodeToCloud(ServiceStack.Redis.RedisClient client, ServiceStack.Redis.RedisClient localclient)
        {
            List<String> GCodeFiles = localclient.SearchKeys("GCODE:*");
            if (GCodeFiles != null)
            {
                for (int i = 0; i < GCodeFiles.Count; i++)
                {
                    List<String> listData = localclient.GetRangeFromList(GCodeFiles[i], 0, -1);
                    localclient.Remove(GCodeFiles[i]);
                    client.Remove(GCodeFiles[i]);
                    using (var trans = client.CreatePipeline())              //Calls 'MULTI'
                    {
                        for (int j = 0; j < listData.Count; j++)
                        {
                            trans.QueueCommand(r => r.AddItemToList(GCodeFiles[i], listData[j]));
                        }
                        trans.Flush();
                    }
                }
            }
        }
        private void UploadFinished(IAsyncResult result)
        {
            CloudUploadhandler handler = (CloudUploadhandler)((AsyncResult)result).AsyncDelegate;
            handler.EndInvoke(result);
            //Console.WriteLine(result.AsyncState);
        }
        private void LocalUploadFinished(IAsyncResult result)
        {
            LocalUploadhandler handler = (LocalUploadhandler)((AsyncResult)result).AsyncDelegate;
            handler.EndInvoke(result);
            //Console.WriteLine(result.AsyncState);
        }
        private void Init()
        {
            String SN = null;
            while (true)
            {
                if (0 == HNC_SystemGetValue((int)HncSystem.HNC_SYS_SN_NUM, ref SN))
                {
                    _MachineSN = SN;
                    break;
                }
            }
        }
        private void InitFinished(IAsyncResult resultInit)
        {
            InitHandler handlerInit = (InitHandler)((AsyncResult)resultInit).AsyncDelegate;
            handlerInit.EndInvoke(resultInit);
            ProgramNameHandler nameHandler = new ProgramNameHandler(FillProgramNameMap);
            nameHandler.BeginInvoke(null, null);
            _MessagePipe = new RedisPubSub(this.MachineSN);
            _MessagePipe.OnSubScribeSampleData += MessagePipe_OnSubScribeSampleData;
            _MessagePipe.OnToolBroken += MessagePipe_OnToolBroken;
            _MessagePipe.OnHealthData += _MessagePipe_OnHealthData;
            _Publisher = new List<RealTimeDataPublisher>();
            _MessagePipe.Start();
            _LocalDB = FindLocalDB();
            RegistMachineToClound();
            CloudUploadhandler handler = new CloudUploadhandler(UploadToCloud);
            IAsyncResult result = handler.BeginInvoke(new AsyncCallback(UploadFinished), "Cloud upload finished!");
            System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;

        }
        private void _MessagePipe_OnHealthData(HealthData hthdb)
        {
            try
            {
                HncApi.HNC_VarSetValue((int)HncVarType.VAR_TYPE_SYSTEM_F, 0, 3607, hthdb.SpdlHealthIndex, _ClientNo);//S÷·
                HncApi.HNC_VarSetValue((int)HncVarType.VAR_TYPE_SYSTEM_F, 0, 3609, hthdb.XaxisHealthIndex, _ClientNo);//X÷·
                HncApi.HNC_VarSetValue((int)HncVarType.VAR_TYPE_SYSTEM_F, 0, 3611, hthdb.YaxisHealthIndex, _ClientNo);//Y÷·
                HncApi.HNC_VarSetValue((int)HncVarType.VAR_TYPE_SYSTEM_F, 0, 3613, hthdb.ZaxisHealthIndex, _ClientNo);//Z÷·     
                HncApi.HNC_VarSetValue((int)HncVarType.VAR_TYPE_SYSTEM_F, 0, 3615, hthdb.ToolsHealthIndex, _ClientNo);//Tool
                HncApi.HNC_VarSetValue((int)HncVarType.VAR_TYPE_SYSTEM_F, 0, 3617, hthdb.MacHealthIndex, _ClientNo);//Mac
            }
            catch(Exception ex)
            {
                Console.WriteLine("HealthDataToMachine:" + ex.Message);
            }
        }
        private void FillProgramNameMap()
        {
            while (_ScanProgramName)
            {

                for (Int32 i = 56; i <= 66; i++)
                {
                    String temp = "";
                    if (0 == HNC_FprogGetProgPathByIdx(i, ref temp))
                    {
                        _ProgramIdToName[i] = temp;
                    }
                }
                System.Threading.Thread.Sleep(500);
            }
        }
        private void RegistMachineToClound()
        {
            lock (_DBLocker)
            {
                try
                {

                    String Redisip = System.Configuration.ConfigurationManager.AppSettings["RedisUri"];
                    Int32 Redisport = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RedisPort"]);
                    bool found = false;
                    using (ServiceStack.Redis.RedisClient client = new ServiceStack.Redis.RedisClient(Redisip, Redisport))
                    {
                        for (int i = 0; i < 180; i++)
                        {
                            client.Db = i;
                            if (client.Exists("Machine") == 1)
                            {
                                String SN = client.Get<String>("Machine");
                                if (SN == this.MachineSN)
                                {
                                    found = true;
                                    RegisterConfigure(client);
                                    CloudDB = i;
                                    break;
                                }
                            }

                        }
                        if (!found)
                        {
                            for (int i = 0; i < 180; i++)
                            {
                                client.Db = i;
                                if (client.DbSize == 0)
                                {
                                    CloudDB = i;
                                    RegisterConfigure(client);
                                    found = true;
                                    break;
                                }

                            }

                        }
                        if(!found)
                        {
                            for (int i = 0; i < 180; i++)
                            {
                                client.Db = i;
                                if (client.Exists("Machine") == 0)
                                {
                                    CloudDB = i;
                                    client.FlushDb();
                                    RegisterConfigure(client);
                                     found = true;
                                    break;
                                }

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RegistMachineToClound" + ex.Message);
                }

            }
            
        }

        private void RegisterConfigure(RedisClient client)
        {
            client.Set("Machine", this.MachineSN);
            String ConfigureStr = Newtonsoft.Json.JsonConvert.SerializeObject(_conf);
            client.Set("SampleConfigure", ConfigureStr);
            client.Set("IP", this._MachineIp);
        }

        private Int32 FindLocalDB()
        {
            lock (_DBLocker) 
            { 
                try
                {
                    String Redisip = "127.0.0.1";
                    Int32 Redisport = 6379;
                    bool found = false;
                    ServiceStack.Redis.RedisClient client = new ServiceStack.Redis.RedisClient(Redisip, Redisport);
                    for (int i = 0; i < 22; i++)
                    {
                        client.Db = i;
                        if (client.Exists("Machine") == 1)
                        {
                            String SN = client.Get<String>("Machine");
                            if (SN == this.MachineSN)
                            {
                                String ConfigureStr = Newtonsoft.Json.JsonConvert.SerializeObject(_conf);
                                client.Set("SampleConfigure", ConfigureStr);
                                client.Set("IP", this._MachineIp);
                                found = true;
                                return i;
                            }
                        }

                    }
                    if (!found)
                    {
                        for (int i = 0; i < 22; i++)
                        {
                            client.Db = i;
                            if (client.DbSize == 0)
                            {
                                client.Set("Machine", this.MachineSN);
                                client.Set("IP", this._MachineIp);
                                String ConfigureStr = Newtonsoft.Json.JsonConvert.SerializeObject(_conf);
                                client.Set("SampleConfigure", ConfigureStr);
                                found = true;
                                return i;
                            }

                        }

                    }
                    if (!found)
                    {
                        for (int i = 0; i < 22; i++)
                        {
                            client.Db = i;
                            if (client.Exists("Machine") == 0)
                            {
                                client.FlushDb();
                                client.Set("Machine", this.MachineSN);
                                client.Set("IP", this._MachineIp);
                                String ConfigureStr = Newtonsoft.Json.JsonConvert.SerializeObject(_conf);
                                client.Set("SampleConfigure", ConfigureStr);
                                found = true;
                                return i;
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FindLocalDB" + ex.Message);
                }
                return -1;
            }
        }
        public void EventFunc(SEventElement evt, Int16 clientNo)
        {
            try
            {
                if (EventAvailable != null)
                {
                    EventAvailable(evt, clientNo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EventAvailable:" + ex.Message);
            }
        }
        public void ReceiveEvent(SEventElement evt, Int16 clientNo)
        {
            EventFunctor fac = new EventFunctor(evt, this);
            //fac.fireEvent();
            System.Threading.Thread thread = new System.Threading.Thread(fac.fireEvent);
            thread.Name = "Event thread";
            thread.Start();
        }
        public void ReceiveSample(List<List<Int32>> data, Int16 ClientNo)
        {
            if (_NeedSample)
            {
                if (String.IsNullOrEmpty(_MachineSN)) return;
                _Job.AddData(data);
                if (_writer == null)
                {
                    _WriteLocal = true;
                    _writer = new LocalSampleDataWriter();
                    if (_writer.RegisterDB(this.MachineSN))
                    {
                        LocalUploadhandler handler = new LocalUploadhandler(SaveDataToLocalRedis);
                        IAsyncResult result = handler.BeginInvoke(new AsyncCallback(LocalUploadFinished), "Local upload finished!");
                    }

                }
            }
        }
        public void NewJobAvaliable(String Gcodepath)
        {
            _ProgPath = Gcodepath;
            String[] files = System.IO.Directory.GetFiles(Gcodepath);
            for (int i = 0; i < files.Length; i++)
            {
                String filename = files[i];
                System.IO.FileInfo info = new System.IO.FileInfo(filename);
                String name = filename.Replace(info.DirectoryName + "\\", "");
                GCodeSender sender = new GCodeSender(this, name);
                System.Threading.Thread thread = new System.Threading.Thread(sender.Read);
                thread.Name = "GCode Upload thread";
                thread.Start();
            }
        }
        public void SetToolBroken(Int32 tool)
        {
            HncApi.HNC_RegSetBit((int)HncRegType.REG_TYPE_G, 3013, 15, _ClientNo);
            HncApi.HNC_RegSetBit((int)HncRegType.REG_TYPE_G, 2626, 0, _ClientNo);
        }
        public void CancelToolBroken(bool bIsTrueBroken)
        {
            try
            {
                if (this._info != null)
                {
                    String Redisip = System.Configuration.ConfigurationManager.AppSettings["RedisUri"];
                    Int32 Redisport = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RedisPort_RealTime"]);
                    ServiceStack.Redis.RedisClient client = new ServiceStack.Redis.RedisClient(Redisip, Redisport);
                    client.Db = this._CloudDB;
                    HncMessage<ToolBrokenInfo> toolinfo = new HncMessage<ToolBrokenInfo>();
                    toolinfo.Header = "ToolBrokenFeedBack";
                    _info.IsBroken = bIsTrueBroken;
                    toolinfo.Body = _info;
                    String msg = Newtonsoft.Json.JsonConvert.SerializeObject(toolinfo);
                    client.Set<String>("ToolBrokenFeedBack", msg);
                    _info = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CancelToolBroken:" + ex.Message);
            }
            HncApi.HNC_RegClrBit((int)HncRegType.REG_TYPE_G, 2626, 0, _ClientNo);
            HncApi.HNC_RegClrBit((int)HncRegType.REG_TYPE_G, 3013, 15, _ClientNo);
        }
        public String GetProgramNameById(Int32 Pid)
        {
            if (_ProgramIdToName == null) return "";
            lock (_ProgramIdToName)
            {
                if (_ProgramIdToName.ContainsKey(Pid))
                    return _ProgramIdToName[Pid];
            }
            return "";
        }
        public void StopUpload()
        {
            lock (_StopLocker)
            {
                CloudDB = -1;
                LocalDB = -1;
                _ScanProgramName = false;
                _WriteLocal =false;
            }
        }
    }
}