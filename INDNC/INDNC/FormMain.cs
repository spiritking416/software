using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ServiceStack.Redis;
using HncDCAgentDll_CS;
using System.Timers;
using MySql.Data.MySqlClient;

namespace INDNC
{
    public struct RedisPara
    {
        public string RedisIP;
        public string RedisPort;
        public string RedisPassword;
        public bool connectvalid;  

        public void dispose()
        {
            RedisIP = null;
            RedisPort = null;
            RedisPassword = null;
            connectvalid = false;
        }
    }

    public struct MySQLPara
    {
        public string MySQLServer;
        public string MySQLID;
        public string MySQLIDPassword;
        public string MySQLIDDatabase;
        public string MySQLDatabaseCNCTable;
        public string MySQLDatabaseRobotTable;
        public bool connectvalid;

        public void dispose()
        {
            MySQLServer = null;
            MySQLID = null;
            MySQLIDPassword = null;
            MySQLIDDatabase = null;
            connectvalid = false;
        }
    }


    internal partial class FormMain : Form
    {
        //用来记录from是否打开过
        private int[] s = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        UserControlMachineState machinestate = new UserControlMachineState();
        UserControlSetting controlsetting = new UserControlSetting();
        UserControlDataAnalysis dataanlysis = new UserControlDataAnalysis();
        RedisManager redismanager = new RedisManager();
        MySQLPara mysqlpara = new MySQLPara();          //mysql参数
        string LineNo = "";  //生产线编号 格式:#+索引
        string WorkShopNo = "";  //车间编号
        //bool bythreadstate = false; //线程运行flag
        ButtonIndex button_onindex = ButtonIndex.NOButton;
        RedisPara serverpara;  //服务器参数
        bool ThreadRefreshFlag; //刷新线程flag
        bool ThreadWatchFlag; //监视线程flag
        Thread threadwatch;  //线程
        Thread threadrefesh;
        public delegate void ThreadWatchDelegate(); //监视线程结束委托
        public delegate void ThreadRefreshDelegate(); //刷新线程结束委托
        MachineView ComboBoxFlag = MachineView.Visiable;
        List<Dictionary<string, UInt16>> machineDB = new List<Dictionary<string, UInt16>>(); //机床SN码与数据库db映射关系
        List<Dictionary<string, string>> machineName = new List<Dictionary<string, string>>(); //机床SN码与机床编号映射关系
        List<Dictionary<string, UInt16>> machinePort = new List<Dictionary<string, UInt16>>(); //机床SN码与机床Port映射关系
        List<Dictionary<string, string>> machineIP = new List<Dictionary<string, string>>(); //机床SN码与机床IP映射关系
        List<MachineInfo> CNCinfo = new List<MachineInfo>();
        List<MachineInfo> Robotinfo = new List<MachineInfo>();
        List<MachineInfo> DataMachineVisible = new List<MachineInfo>(); //可显示设备
        List<MachineInfo> DataMachineOnline = new List<MachineInfo>();  //在线设备
        List<MachineInfo> DataMachineOffline = new List<MachineInfo>(); //离线设备
        List<MachineInfo> DataMachineAlarm = new List<MachineInfo>();  //告警设备
        List<MachineInfo> DataMachineInvisible = new List<MachineInfo>();  //未显示设备

        public FormMain()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {

            //双缓冲
            this.SetStyle(ControlStyles.ResizeRedraw |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            CheckForIllegalCrossThreadCalls = false; //解决线程间的访问限制
            global::INDNC.Properties.Settings.Default.UserType = 0; //默认操作工

            //LineNo、WorkShopNo
            LineNo = global::INDNC.Properties.Settings.Default.LineIndex; ;
            WorkShopNo = global::INDNC.Properties.Settings.Default.workshopno; ;
            if (LineNo == "" || WorkShopNo == "")
            {
                LineNo = "#1";
                WorkShopNo = "#1";
            }
            //mysqlpara初始化
            try
            {
                mysqlpara.MySQLServer = global::INDNC.Properties.Settings.Default.localserver;
                mysqlpara.MySQLID = global::INDNC.Properties.Settings.Default.localserverid;
                mysqlpara.MySQLIDPassword = global::INDNC.Properties.Settings.Default.localserverpassword;
                mysqlpara.MySQLIDDatabase = global::INDNC.Properties.Settings.Default.localMysqlDatabase;
                mysqlpara.MySQLDatabaseCNCTable = global::INDNC.Properties.Settings.Default.MysqlCNCTable;
                mysqlpara.MySQLDatabaseRobotTable = global::INDNC.Properties.Settings.Default.MysqlRobotTable;
                mysqlpara.connectvalid = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }

            //serverpara初始化
            try
            {
                serverpara.RedisIP = global::INDNC.Properties.Settings.Default.ip1 + '.' + global::INDNC.Properties.Settings.Default.ip2 + '.'
                    + global::INDNC.Properties.Settings.Default.ip3 + '.' + global::INDNC.Properties.Settings.Default.ip4;
                serverpara.RedisPort = global::INDNC.Properties.Settings.Default.port;
                serverpara.RedisPassword = global::INDNC.Properties.Settings.Default.serverpassword;
                serverpara.connectvalid = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }

            
            //初始化设备信息
            //host主机参数  格式“password@ip:port”
            string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort };
            try
            {
                //从连接池获得只读连接客户端
                int initialDB = 0;
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));
                if (Client == null || !Client.Ping())
                {
                    throw new Exception("连接服务器失败，请设置服务器参数!");
                }
                //连接云端服务器成功
                serverpara.connectvalid = true;
                redismanager.DisposeClient(ref (Client));  //dispose客户端

                //测试连接
                // MessageBox.Show(Client.Db.ToString());  //db index
                //MessageBox.Show(Client.DbSize.ToString());

                //属性设置
                machinestate.Dock = DockStyle.Fill;
                machinestate.Height = this.panel1.Height;
                machinestate.Width = this.panel1.Width;

                //连接本地服务器
                string MyconnectionString = "server=" + mysqlpara.MySQLServer + ";user id=" + mysqlpara.MySQLID + ";password=" + mysqlpara.MySQLIDPassword + "; database=" + mysqlpara.MySQLIDDatabase;
                MySqlConnection mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                if (Client == null || !mysqlconnection.Ping())
                {
                    throw new Exception("连接本地MySQL服务器失败!");
                }
                //本地服务器连接成功
                mysqlpara.connectvalid = true;
                //获取设备信息
                CNCinfo.Clear();
                Robotinfo.Clear();
                int cncrows = controlsetting.mysqlcncset.Tables[0].Rows.Count;
                int robotrows = controlsetting.mysqlrobotset.Tables[0].Rows.Count;

                MachineInfo tmp_machineinfo = new MachineInfo();
                for (int i = 0; i < cncrows; ++i)
                {
                    tmp_machineinfo.MachineName = controlsetting.mysqlcncset.Tables[0].Rows[i]["机床编号"].ToString();
                    tmp_machineinfo.MachineDB = Int32.Parse(controlsetting.mysqlcncset.Tables[0].Rows[i]["数据库DB"].ToString());
                    tmp_machineinfo.MachineIP = controlsetting.mysqlcncset.Tables[0].Rows[i]["机床IP地址"].ToString();
                    tmp_machineinfo.MachinePort = Int32.Parse(controlsetting.mysqlcncset.Tables[0].Rows[i]["机床IP端口"].ToString());
                    tmp_machineinfo.MachineSN = controlsetting.mysqlcncset.Tables[0].Rows[i]["SN码"].ToString();

                    CNCinfo.Add(tmp_machineinfo);
                }

                for (int i = 0; i < robotrows; ++i)
                {
                    tmp_machineinfo.MachineName = controlsetting.mysqlrobotset.Tables[0].Rows[i]["机器人编号"].ToString();
                    tmp_machineinfo.MachineDB = Int32.Parse(controlsetting.mysqlrobotset.Tables[0].Rows[i]["数据库DB"].ToString());
                    tmp_machineinfo.MachineIP = controlsetting.mysqlrobotset.Tables[0].Rows[i]["机器人IP地址"].ToString();
                    tmp_machineinfo.MachinePort = Int32.Parse(controlsetting.mysqlrobotset.Tables[0].Rows[i]["机器人IP端口"].ToString());
                    tmp_machineinfo.MachineSN = controlsetting.mysqlrobotset.Tables[0].Rows[i]["SN码"].ToString();

                    Robotinfo.Add(tmp_machineinfo);
                };

                dataanlysis.alarmanalysis.CNCinfoName = CNCinfo;
                dataanlysis.alarmanalysis.RobotinfoName = Robotinfo;
                dataanlysis.alarmanalysis.serverparaName = serverpara;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
            

            //事件初始化
            controlsetting.btnServerSettingClick += new btnOkClickEventHander(ControlServerSettingclick);
            controlsetting.btnLineSettingClick += new btnOkClickEventHander(ControlLineSettingclick);
            controlsetting.btnCNCSettingClick += new btnOkClickEventHander(ControlCNCSettingclick);
            controlsetting.btnRobotSettingClick += new btnOkClickEventHander(ControlRobotSettingclick);
            controlsetting.btnRobotSaveClick += new btnOkClickEventHander(ControlRobotSaveclick);
            controlsetting.btnCNCSaveClick += new btnOkClickEventHander(ControlCNCSaveclick);

            machinestate.ComboBoxMachineViewChange += new btnOkClickEventHander(ComboBoxMachineViewChange);
            machinestate.listViewItemMouseMove += new btnOkClickEventHander(listViewItemMouseMove);
        }

        public void ListViewInitial()
        {
            try
            {
                if (machinestate.listView1.Items != null)
                    machinestate.listView1.Items.Clear();

                DataMachineVisible.Clear();
                DataMachineAlarm.Clear();
                DataMachineInvisible.Clear();
                DataMachineOffline.Clear();
                DataMachineOnline.Clear(); //清除数据buf

                if (serverpara.connectvalid == false)
                    throw new Exception("云端服务器参数错误，请重新设置！");
                int port = -1;
                if (int.TryParse(serverpara.RedisPort, out port) != true)
                {
                    throw new Exception("云端服务器参数错误，请重新设置！");
                }
                RedisClient Client;
                if (serverpara.RedisPassword == "")
                    Client = new RedisClient(serverpara.RedisIP, port, null);  //连接云端服务器
                else
                    Client = new RedisClient(serverpara.RedisIP, port, serverpara.RedisPassword);  //连接云端服务器
                if (!Client.Ping())
                {
                    throw new Exception("未能连接云端服务器，请检查相关参数！");
                }
                int index = 0;
                machinestate.listView1.BeginUpdate();

                foreach (MachineInfo key in CNCinfo)
                {
                    Client.ChangeDb(key.MachineDB);
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    byte[] machineIP = new byte[] { };
                    machineIP = Client.Get("IP");
                    byte[] machinePort = new byte[] { };
                    machinePort = Client.Get("Port");
                    if (machine == null || machineIP == null || machinePort == null)
                    {
                        DataMachineInvisible.Add(key);
                        if (ComboBoxFlag == MachineView.Invisiable)
                        {
                            ++index;
                            ListViewItem lvi = new ListViewItem(index.ToString());
                            lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                            lvi.SubItems.Add(key.MachineName);
                            lvi.SubItems.Add("未显示");
                            lvi.SubItems.Add("参数错误");
                            lvi.SubItems.Add("\\");
                            lvi.SubItems[2].BackColor = Color.LightBlue;
                            machinestate.listView1.Items.Add(lvi);
                        }
                        continue;
                    }
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    string machineIPstr = System.Text.Encoding.Default.GetString(machineIP);
                    string machinePortstr = System.Text.Encoding.Default.GetString(machinePort);

                    int machinePortInt = int.Parse(machinePortstr);
                    if (machinestr == key.MachineSN && machineIPstr == key.MachineIP && machinePortInt == key.MachinePort) //本地和云端数据对应
                    {
                        DataMachineVisible.Add(key);

                        //机床状态
                        DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                        //获取时间
                        byte[] timebyte = Client.Get("TimeStamp");
                        string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                        long timestamp = Convert.ToInt64(timestampstr);
                        var time = System.DateTime.FromBinary(timestamp);

                        Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                        bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                        if (isConnect == false)
                        {
                            DataMachineOffline.Add(key);
                            if (ComboBoxFlag == MachineView.Offline || ComboBoxFlag == MachineView.Visiable)
                            {
                                ++index;
                                ListViewItem lvi = new ListViewItem(index.ToString());
                                lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("离线");
                                lvi.SubItems.Add("无");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Gray;
                                machinestate.listView1.Items.Add(lvi);
                            }
                        }
                        else
                        {

                            byte[] machinealarmbyte = new byte[] { };
                            byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                            machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                            string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                            long machinealarm = Convert.ToInt64(machinealarmstr);

                            if (machinealarm == 0)
                            {
                                DataMachineOnline.Add(key);
                                if (ComboBoxFlag == MachineView.Online || ComboBoxFlag == MachineView.Visiable)
                                {
                                    ++index;
                                    ListViewItem lvi = new ListViewItem(index.ToString());
                                    lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                                    lvi.SubItems.Add(key.MachineName);
                                    lvi.SubItems.Add("在线");
                                    lvi.SubItems.Add("无");
                                    lvi.SubItems.Add(time.ToString());
                                    lvi.SubItems[2].BackColor = Color.Green;
                                    machinestate.listView1.Items.Add(lvi);
                                }
                            }
                            else
                            {
                                DataMachineAlarm.Add(key);
                                if (ComboBoxFlag == MachineView.Alarm || ComboBoxFlag == MachineView.Visiable)
                                {
                                    ++index;
                                    string alarmstr=null;
                                    for(int i=0;i< machinealarm; ++i)
                                    {
                                        alarmbyte = Encoding.UTF8.GetBytes(i.ToString());
                                        machinealarmbyte = Client.HGet("Alarm:AlarmCurrent", alarmbyte);
                                        string tmp = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                        if(tmp != null && tmp != "")
                                        {
                                            if (alarmstr == null)
                                                alarmstr = tmp;
                                            else
                                                alarmstr += "; " + tmp;
                                        }
                                    }
                                  

                                    ListViewItem lvi = new ListViewItem(index.ToString());
                                    lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                                    lvi.SubItems.Add(key.MachineName);
                                    lvi.SubItems.Add("告警");
                                    lvi.SubItems.Add(alarmstr);
                                    lvi.SubItems.Add(time.ToString());
                                    lvi.SubItems[2].BackColor = Color.Red;
                                    machinestate.listView1.Items.Add(lvi);
                                }
                            }
                        }
                    }
                    else
                    {
                        DataMachineInvisible.Add(key);
                        if (ComboBoxFlag == MachineView.Invisiable)
                        {
                            ++index;
                            ListViewItem lvi = new ListViewItem(index.ToString());
                            lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                            lvi.SubItems.Add(key.MachineName);
                            lvi.SubItems.Add("未显示");
                            lvi.SubItems.Add("参数错误");
                            lvi.SubItems.Add("\\");
                            lvi.SubItems[2].BackColor = Color.LightBlue;
                            machinestate.listView1.Items.Add(lvi);
                        }
                    }
                }

                foreach (MachineInfo key in Robotinfo)
                {
                    Client.ChangeDb(key.MachineDB);
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    byte[] machineIP = new byte[] { };
                    machineIP = Client.Get("IP");
                    byte[] machinePort = new byte[] { };
                    machinePort = Client.Get("Port");
                    if (machine == null || machineIP == null || machinePort == null)
                    {
                        DataMachineInvisible.Add(key);
                        if (ComboBoxFlag == MachineView.Invisiable)
                        {
                            ++index;
                            ListViewItem lvi = new ListViewItem(index.ToString());
                            lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                            lvi.SubItems.Add(key.MachineName);
                            lvi.SubItems.Add("未显示");
                            lvi.SubItems.Add("参数错误");
                            lvi.SubItems.Add("\\");
                            lvi.SubItems[2].BackColor = Color.LightBlue;
                            machinestate.listView1.Items.Add(lvi);
                        }
                        continue;
                    }
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    string machineIPstr = System.Text.Encoding.Default.GetString(machineIP);
                    string machinePortstr = System.Text.Encoding.Default.GetString(machinePort);

                    int machinePortInt = int.Parse(machinePortstr);
                    if (machinestr == key.MachineSN && machineIPstr == key.MachineIP && machinePortInt == key.MachinePort) //本地和云端数据对应
                    {
                        DataMachineVisible.Add(key);

                        //机床状态
                        DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                        //获取时间
                        byte[] timebyte = Client.Get("TimeStamp");
                        string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                        long timestamp = Convert.ToInt64(timestampstr);
                        var time = System.DateTime.FromBinary(timestamp);

                        Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                        bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                        if (isConnect == false)
                        {
                            DataMachineOffline.Add(key);
                            if (ComboBoxFlag == MachineView.Offline || ComboBoxFlag == MachineView.Visiable)
                            {
                                ++index;
                                ListViewItem lvi = new ListViewItem(index.ToString());
                                lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("离线");
                                lvi.SubItems.Add("无");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Gray;
                                machinestate.listView1.Items.Add(lvi);
                            }
                        }
                        else
                        {

                            byte[] machinealarmbyte = new byte[] { };
                            byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                            machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                            string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                            long machinealarm = Convert.ToInt64(machinealarmstr);

                            if (machinealarm == 0)
                            {
                                DataMachineOnline.Add(key);
                                if (ComboBoxFlag == MachineView.Online || ComboBoxFlag == MachineView.Visiable)
                                {
                                    ++index;
                                    ListViewItem lvi = new ListViewItem(index.ToString());
                                    lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                                    lvi.SubItems.Add(key.MachineName);
                                    lvi.SubItems.Add("在线");
                                    lvi.SubItems.Add("无");
                                    lvi.SubItems.Add(time.ToString());
                                    lvi.SubItems[2].BackColor = Color.Green;
                                    machinestate.listView1.Items.Add(lvi);
                                }
                            }
                            else
                            {
                                DataMachineAlarm.Add(key);
                                if (ComboBoxFlag == MachineView.Alarm || ComboBoxFlag == MachineView.Visiable)
                                {
                                    ++index;
                                    string alarmstr = null;
                                    for (int i = 0; i < machinealarm; ++i)
                                    {
                                        alarmbyte = Encoding.UTF8.GetBytes(i.ToString());
                                        machinealarmbyte = Client.HGet("Alarm:AlarmCurrent", alarmbyte);
                                        string tmp = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                        if (tmp != null && tmp != "")
                                        {
                                            if (alarmstr == null)
                                                alarmstr = tmp;
                                            else
                                                alarmstr += "; " + tmp;
                                        }
                                    }

                                    ListViewItem lvi = new ListViewItem(index.ToString());
                                    lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                                    lvi.SubItems.Add(key.MachineName);
                                    lvi.SubItems.Add("告警");
                                    lvi.SubItems.Add(alarmstr);
                                    lvi.SubItems.Add(time.ToString());
                                    lvi.SubItems[2].BackColor = Color.Red;
                                    machinestate.listView1.Items.Add(lvi);
                                }
                            }
                        }
                    }
                    else
                    {
                        DataMachineInvisible.Add(key);
                        if (ComboBoxFlag == MachineView.Invisiable)
                        {
                            ++index;
                            ListViewItem lvi = new ListViewItem(index.ToString());
                            lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                            lvi.SubItems.Add(key.MachineName);
                            lvi.SubItems.Add("未显示");
                            lvi.SubItems.Add("参数错误");
                            lvi.SubItems.Add("\\");
                            lvi.SubItems[2].BackColor = Color.LightBlue;
                            machinestate.listView1.Items.Add(lvi);
                        }
                    }
                }

                //设备数量显示
                machinestate.label1.Visible = true;
                machinestate.label2.Visible = true;
                machinestate.label3.Visible = true;
                machinestate.label4.Visible = true;
                machinestate.label5.Visible = true;
                machinestate.label1.Text = "生产线" + LineNo.ToString() + "设备数目:" + (CNCinfo.Count() + Robotinfo.Count()).ToString() + "台";
                machinestate.label2.Text = "在线设备数目:" + DataMachineOnline.Count.ToString() + "台";
                machinestate.label3.Text = "离线设备数目:" + DataMachineOffline.Count.ToString() + "台";
                machinestate.label4.Text = "告警设备数目:" + DataMachineAlarm.Count.ToString() + "台";
                machinestate.label5.Text = "参数错误设备数目:" + DataMachineInvisible.Count.ToString() + "台";

                machinestate.listView1.EndUpdate();

            }
            catch (Exception ex)
            {
                if(ThreadRefreshFlag==true && ThreadWatchFlag==true)
                    MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
        }

        //机床状态刷新

        private void ThreadWatch(object o)
        {
            while(ThreadWatchFlag)
            {
                try
                {
                    if (ThreadRefreshFlag == false)
                    {
                        ThreadRefreshFlag = true;
                        ListViewInitial();
                        ThreadRefreshDelegate callback = new ThreadRefreshDelegate(threadwatchevent);
                        threadrefesh = new Thread(ThreadReadData);
                        threadrefesh.IsBackground = true;
                        threadrefesh.Start(callback);
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
            ThreadWatchDelegate callback2= new ThreadWatchDelegate(threadwatchevent);
            callback2();
        }

        private void ThreadReadData(object obj) //读取数据线程
        {
            try
            {
                if (serverpara.connectvalid == false)
                    throw new Exception("云端服务器参数错误，请重新设置！");
                int port = -1;
                if (int.TryParse(serverpara.RedisPort, out port) != true)
                {
                    throw new Exception("云端服务器参数错误，请重新设置！");
                }
                RedisClient Client;
                if (serverpara.RedisPassword == "")
                    Client = new RedisClient(serverpara.RedisIP, port, null);  //连接云端服务器
                else
                    Client = new RedisClient(serverpara.RedisIP, port, serverpara.RedisPassword);  //连接云端服务器
                if (!Client.Ping())
                {
                    throw new Exception("未能连接云端服务器，请检查相关参数！");
                }

                while (ThreadRefreshFlag)
                {
                    DataMachineVisible.Clear();
                    DataMachineAlarm.Clear();
                    DataMachineInvisible.Clear();
                    DataMachineOffline.Clear();
                    DataMachineOnline.Clear();

                    //更新
                    foreach (MachineInfo key in CNCinfo)
                    {
                        Client.ChangeDb(key.MachineDB);
                        byte[] machine = new byte[] { };
                        machine = Client.Get("Machine");
                        byte[] machineIP = new byte[] { };
                        machineIP = Client.Get("IP");
                        byte[] machinePort = new byte[] { };
                        machinePort = Client.Get("Port");
                        if (machine == null || machineIP == null || machinePort == null)
                        {
                            DataMachineInvisible.Add(key);
                            continue;
                        }
                        string machinestr = System.Text.Encoding.Default.GetString(machine);
                        string machineIPstr = System.Text.Encoding.Default.GetString(machineIP);
                        string machinePortstr = System.Text.Encoding.Default.GetString(machinePort);

                        int machinePortInt = int.Parse(machinePortstr);
                        if (machinestr == key.MachineSN && machineIPstr == key.MachineIP && machinePortInt == key.MachinePort) //本地和云端数据对应
                        {
                            DataMachineVisible.Add(key);

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time = System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                DataMachineOffline.Add(key);
                            }
                            else
                            {
                                byte[] machinealarmbyte = new byte[] { };
                                byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                                machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                                string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                long machinealarm = Convert.ToInt64(machinealarmstr);

                                if (machinealarm == 0)
                                {
                                    DataMachineOnline.Add(key);
                                }
                                else
                                {
                                    DataMachineAlarm.Add(key);
                                }
                            }
                        }
                        else
                        {
                            DataMachineInvisible.Add(key);
                        }
                    }

                    foreach (MachineInfo key in Robotinfo)
                    {
                        Client.ChangeDb(key.MachineDB);
                        byte[] machine = new byte[] { };
                        machine = Client.Get("Machine");
                        byte[] machineIP = new byte[] { };
                        machineIP = Client.Get("IP");
                        byte[] machinePort = new byte[] { };
                        machinePort = Client.Get("Port");
                        if (machine == null || machineIP == null || machinePort == null)
                        {
                            DataMachineInvisible.Add(key);
                            continue;
                        }
                        string machinestr = System.Text.Encoding.Default.GetString(machine);
                        string machineIPstr = System.Text.Encoding.Default.GetString(machineIP);
                        string machinePortstr = System.Text.Encoding.Default.GetString(machinePort);

                        int machinePortInt = int.Parse(machinePortstr);
                        if (machinestr == key.MachineSN && machineIPstr == key.MachineIP && machinePortInt == key.MachinePort) //本地和云端数据对应
                        {
                            DataMachineVisible.Add(key);

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time = System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                DataMachineOffline.Add(key);
                            }
                            else
                            {
                                byte[] machinealarmbyte = new byte[] { };
                                byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                                machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                                string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                long machinealarm = Convert.ToInt64(machinealarmstr);

                                if (machinealarm == 0)
                                {
                                    DataMachineOnline.Add(key);
                                }
                                else
                                {
                                    DataMachineAlarm.Add(key);
                                }
                            }
                        }
                        else
                        {
                            DataMachineInvisible.Add(key);
                        }
                    }

                    if (ComboBoxFlag == MachineView.Visiable)
                    {
                        int index = 0;
                        while (index < DataMachineVisible.Count)
                        {
                            if (index >= machinestate.listView1.Items.Count)
                                throw new Exception();
                            var key = DataMachineVisible[index];
                            Client.ChangeDb(DataMachineVisible[index].MachineDB);
                            var lvi = machinestate.listView1.Items[index];
                            if (lvi == null)
                                throw new Exception();
                            if (lvi.SubItems[0].Text != (index + 1).ToString() || lvi.SubItems[1].Text != key.MachineName)
                                throw new Exception();

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time = System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                lvi.SubItems[2].Text = "离线";
                                lvi.SubItems[3].Text = "无";
                                lvi.SubItems[4].Text = time.ToString();
                                lvi.SubItems[2].BackColor = Color.Gray;
                            }
                            else
                            {

                                byte[] machinealarmbyte = new byte[] { };
                                byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                                machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                                string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                long machinealarm = Convert.ToInt64(machinealarmstr);

                                if (machinealarm == 0)
                                {
                                    lvi.SubItems[2].Text = "在线";
                                    lvi.SubItems[3].Text = "无";
                                    lvi.SubItems[4].Text = time.ToString();
                                    lvi.SubItems[2].BackColor = Color.Green;

                                }
                                else
                                {
                                    lvi.SubItems[2].Text = "告警";
                                    lvi.SubItems[3].Text = "有";
                                    lvi.SubItems[4].Text = time.ToString();
                                    lvi.SubItems[2].BackColor = Color.Red;
                                }
                            }
                            ++index;
                        }
                        if (index != DataMachineVisible.Count)
                            throw new Exception();
                        else if (DataMachineVisible.Count != machinestate.listView1.Items.Count)
                            throw new Exception();
                    }
                    else if (ComboBoxFlag == MachineView.Online)
                    {
                        int index = 0;
                        while (index < DataMachineOnline.Count)
                        {
                            if (index >= machinestate.listView1.Items.Count)
                                throw new Exception();
                            var key = DataMachineOnline[index];
                            Client.ChangeDb(DataMachineOnline[index].MachineDB);
                            var lvi = machinestate.listView1.Items[index];
                            if (lvi == null)
                                throw new Exception();
                            if (lvi.SubItems[0].Text != (index + 1).ToString() || lvi.SubItems[1].Text != key.MachineName)
                                throw new Exception();

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time = System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                throw new Exception();
                            }
                            else
                            {

                                byte[] machinealarmbyte = new byte[] { };
                                byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                                machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                                string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                long machinealarm = Convert.ToInt64(machinealarmstr);

                                if (machinealarm == 0)
                                {
                                    lvi.SubItems[2].Text = "在线";
                                    lvi.SubItems[3].Text = "无";
                                    lvi.SubItems[4].Text = time.ToString();
                                    lvi.SubItems[2].BackColor = Color.Green;

                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                            ++index;
                        }
                        if (index != DataMachineOnline.Count)
                            throw new Exception();
                        else if(DataMachineOnline.Count!= machinestate.listView1.Items.Count)
                            throw new Exception();
                    }
                    else if (ComboBoxFlag == MachineView.Offline)
                    {
                        int index = 0;
                        while (index < DataMachineOffline.Count)
                        {
                            if (index >= machinestate.listView1.Items.Count)
                                throw new Exception();
                            var key = DataMachineOffline[index];
                            Client.ChangeDb(DataMachineOffline[index].MachineDB);
                            var lvi = machinestate.listView1.Items[index];
                            if (lvi == null)
                                throw new Exception();
                            if (lvi.SubItems[0].Text != (index + 1).ToString() || lvi.SubItems[1].Text != key.MachineName)
                                throw new Exception();

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time = System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                lvi.SubItems[2].Text = "离线";
                                lvi.SubItems[3].Text = "无";
                                lvi.SubItems[4].Text = time.ToString();
                                lvi.SubItems[2].BackColor = Color.Gray;
                            }
                            else
                            {
                                throw new Exception();
                            }

                            //更新数据
                            ++index;
                        }
                        if (index != DataMachineOffline.Count)
                            throw new Exception();
                        else if (DataMachineOffline.Count != machinestate.listView1.Items.Count)
                            throw new Exception();
                    }
                    else if (ComboBoxFlag == MachineView.Alarm)
                    {
                        int index = 0;
                        while (index < DataMachineAlarm.Count)
                        {
                            if (index >= machinestate.listView1.Items.Count)
                                throw new Exception();
                            var key = DataMachineAlarm[index];
                            Client.ChangeDb(DataMachineAlarm[index].MachineDB);
                            var lvi = machinestate.listView1.Items[index];
                            if (lvi == null)
                                throw new Exception();
                            if (lvi.SubItems[0].Text != (index + 1).ToString() || lvi.SubItems[1].Text != key.MachineName)
                                throw new Exception();

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time = System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort)key.MachinePort);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                throw new Exception();
                            }
                            else
                            {

                                byte[] machinealarmbyte = new byte[] { };
                                byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                                machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                                string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                long machinealarm = Convert.ToInt64(machinealarmstr);

                                if (machinealarm == 0)
                                {
                                    throw new Exception();
                                }
                                else
                                {
                                    lvi.SubItems[2].Text = "告警";
                                    lvi.SubItems[3].Text = "有";
                                    lvi.SubItems[4].Text = time.ToString();
                                    lvi.SubItems[2].BackColor = Color.Red;
                                }
                            }

                            //更新数据
                            ++index;
                        }
                        if (index != DataMachineAlarm.Count)
                            throw new Exception();
                        else if (DataMachineAlarm.Count != machinestate.listView1.Items.Count)
                            throw new Exception();
                    }
                    else if (ComboBoxFlag == MachineView.Invisiable)
                    {
                        int index = 0;
                        while (index < DataMachineInvisible.Count)
                        {
                            if (index >= machinestate.listView1.Items.Count)
                                throw new Exception();
                            var key = DataMachineInvisible[index];
                            Client.ChangeDb(DataMachineInvisible[index].MachineDB);
                            var lvi = machinestate.listView1.Items[index];
                            if (lvi == null)
                                throw new Exception();
                            if (lvi.SubItems[0].Text != (index + 1).ToString() || lvi.SubItems[1].Text != key.MachineName)
                                throw new Exception();

                            lvi.SubItems[2].Text = "未显示";
                            lvi.SubItems[3].Text = "参数错误";
                            lvi.SubItems[4].Text = "\\";
                            lvi.SubItems[2].BackColor = Color.LightBlue;

                            ++index;
                        }
                        if (index != DataMachineInvisible.Count)
                            throw new Exception();
                        else if (DataMachineInvisible.Count != machinestate.listView1.Items.Count)
                            throw new Exception();
                    }

                    //设备数量显示
                    machinestate.label1.Visible = true;
                    machinestate.label2.Visible = true;
                    machinestate.label3.Visible = true;
                    machinestate.label4.Visible = true;
                    machinestate.label5.Visible = true;
                    machinestate.label1.Text = "生产线" + LineNo.ToString() + "设备数目:" + (CNCinfo.Count() + Robotinfo.Count()).ToString() + "台";
                    machinestate.label2.Text = "在线设备数目:" + DataMachineOnline.Count.ToString() + "台";
                    machinestate.label3.Text = "离线设备数目:" + DataMachineOffline.Count.ToString() + "台";
                    machinestate.label4.Text = "告警设备数目:" + DataMachineAlarm.Count.ToString() + "台";
                    machinestate.label5.Text = "参数错误设备数目:" + DataMachineInvisible.Count.ToString() + "台";
                    Thread.Sleep(1000);
                }
                ThreadRefreshDelegate callback = obj as ThreadRefreshDelegate;
                callback();
            }
            catch (Exception)
            {
                ThreadRefreshFlag = false;
            }
        }

        //
        private void ControlServerSettingclick(object send, System.EventArgs e)
        {
            serverpara = controlsetting.serverparaName;
            mysqlpara = controlsetting.mysqlparaName;
            dataanlysis.alarmanalysis.serverparaName = serverpara;
        }

        private void ControlLineSettingclick(object send, System.EventArgs e)
        {
            WorkShopNo = controlsetting.workshopnoName;
            LineNo = controlsetting.linenoName;
        }

        private void ControlCNCSettingclick(object send, System.EventArgs e)
        {
            mysqlpara = controlsetting.mysqlparaName;
        }

        private void ControlRobotSettingclick(object send, System.EventArgs e)
        {
            mysqlpara = controlsetting.mysqlparaName;
        }

        private void ControlCNCSaveclick(object send, System.EventArgs e)
        {
            int cncrows = controlsetting.mysqlcncset.Tables[0].Rows.Count;
            MachineInfo tmp_machineinfo = new MachineInfo();
            CNCinfo.Clear();
            for (int i = 0; i < cncrows; ++i)
            {
                tmp_machineinfo.MachineName = controlsetting.mysqlcncset.Tables[0].Rows[i]["机床编号"].ToString();
                tmp_machineinfo.MachineDB = Int32.Parse(controlsetting.mysqlcncset.Tables[0].Rows[i]["数据库DB"].ToString());
                tmp_machineinfo.MachineIP = controlsetting.mysqlcncset.Tables[0].Rows[i]["机床IP地址"].ToString();
                tmp_machineinfo.MachinePort = Int32.Parse(controlsetting.mysqlcncset.Tables[0].Rows[i]["机床IP端口"].ToString());
                tmp_machineinfo.MachineSN = controlsetting.mysqlcncset.Tables[0].Rows[i]["SN码"].ToString();

                CNCinfo.Add(tmp_machineinfo);
            }
            dataanlysis.alarmanalysis.CNCinfoName = CNCinfo;
            dataanlysis.alarmanalysis.AlarmDraw();
        }

        private void ControlRobotSaveclick(object send, System.EventArgs e)
        {
            int robotrows = controlsetting.mysqlrobotset.Tables[0].Rows.Count;
            MachineInfo tmp_machineinfo = new MachineInfo();
            Robotinfo.Clear();
            for (int i = 0; i < robotrows; ++i)
            {
                tmp_machineinfo.MachineName = controlsetting.mysqlrobotset.Tables[0].Rows[i]["机器人编号"].ToString();
                tmp_machineinfo.MachineDB = Int32.Parse(controlsetting.mysqlrobotset.Tables[0].Rows[i]["数据库DB"].ToString());
                tmp_machineinfo.MachineIP = controlsetting.mysqlrobotset.Tables[0].Rows[i]["机器人IP地址"].ToString();
                tmp_machineinfo.MachinePort = Int32.Parse(controlsetting.mysqlrobotset.Tables[0].Rows[i]["机器人IP端口"].ToString());
                tmp_machineinfo.MachineSN = controlsetting.mysqlrobotset.Tables[0].Rows[i]["SN码"].ToString();

                Robotinfo.Add(tmp_machineinfo);
            }
            dataanlysis.alarmanalysis.RobotinfoName = Robotinfo;
            dataanlysis.alarmanalysis.AlarmDraw();
        }

        private void threadwatchevent()
        {
            ThreadWatchFlag = false;
        }

        private void threadrefreshevent()
        {
            ThreadRefreshFlag = false;
        }

        private void ComboBoxMachineViewChange(object send, System.EventArgs e)
        {
            if (ComboBoxFlag == machinestate.ComboBoxFlag)
                return;
            ComboBoxFlag = machinestate.ComboBoxFlag;
            //初始化绘制
            ThreadRefreshFlag = false;
        }

        private void listViewItemMouseMove(object send, System.EventArgs e)
        {
            ToolTip tooltip = new ToolTip();
            Point p1 = MousePosition;
            Point p2 = machinestate.PointToClient(p1);
            ListViewItem item = machinestate.listView1.GetItemAt(p2.X, p2.Y);
            if (item == null)
                return;
            string machinename = item.SubItems[1].Text;
            bool findflag = false;
            MachineInfo machine = new MachineInfo();
            foreach (var key in CNCinfo)
            {
                if(key.MachineName== machinename)
                {
                    machine = key;
                    findflag = true;
                    break;
                }

            }

            if (!findflag)
            {
                foreach (var key in Robotinfo)
                {
                    if (key.MachineName == machinename)
                    {
                        machine = key;
                        findflag = true;
                        break;
                    }

                }
            }

            string text = "设备编号:" + machine.MachineName + ",设备IP:" + machine.MachineIP + ",设备端口:" + machine.MachinePort + ",设备SN码:" + machine.MachineSN;
            tooltip.Show(text, machinestate.listView1, new Point(p2.X + 15, p2.Y + 15), 1000);
            tooltip.AutoPopDelay = 2000;
            tooltip.ReshowDelay = 0;
            tooltip.InitialDelay = 0;
            tooltip.Active = true;
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            if (button_onindex == ButtonIndex.ButtonHome)
                return;

            panel1.BackgroundImage = Image.FromFile(@"logo.png");
            this.Refresh();
            button_onindex = ButtonIndex.ButtonHome;
            button_refrush();

        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {

            if (button_onindex == ButtonIndex.ButtonCheck)
                return;
            button_onindex = ButtonIndex.ButtonCheck;
            button_refrush();
            machinestate.Dock = DockStyle.Fill;

            //host主机参数  格式“password@ip:port”
            string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort };
            try
            {
                //从连接池获得只读连接客户端
                int initialDB = 0;
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));
                if (Client == null || !Client.Ping())
                {
                    throw new Exception("连接服务器失败，请设置服务器参数!");
                }
                //连接云端服务器成功
                serverpara.connectvalid = true;
                redismanager.DisposeClient(ref (Client));  //dispose客户端

                //属性设置
                machinestate.Dock = DockStyle.Fill;
                machinestate.Height = this.panel1.Height;
                machinestate.Width = this.panel1.Width;

                //连接本地服务器
                string MyconnectionString = "server=" + mysqlpara.MySQLServer + ";user id=" + mysqlpara.MySQLID + ";password=" + mysqlpara.MySQLIDPassword + "; database=" + mysqlpara.MySQLIDDatabase;
                MySqlConnection mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                if (Client == null || !mysqlconnection.Ping())
                {
                    throw new Exception("连接本地MySQL服务器失败!");
                }
                //本地服务器连接成功
                mysqlpara.connectvalid = true;
                machinestate.comboBoxMachineview.SelectedIndex = 0;

                //绘制标题
                try
                {
                    if (!machinestate.ListViewTitleDraw())
                    {
                        throw new Exception("listview error!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                }

                this.panel1.Controls.Add(machinestate);
                ThreadRefreshFlag = false;
                ThreadWatchFlag = true;
                ThreadWatchDelegate callback = new ThreadWatchDelegate(threadwatchevent);
                threadwatch = new Thread(ThreadWatch);
                threadwatch.IsBackground = true;
                threadwatch.Start(callback);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                if (machinestate != null)
                {
                    machinestate.Visible = false;
                    machinestate = null;
                }
                redismanager.dispose();
                //redispara.dispose();
            }

        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            if (button_onindex == ButtonIndex.ButtonSetting)
                return;
            button_onindex = ButtonIndex.ButtonSetting;
            button_refrush();
            controlsetting.tabControlSetting.SelectedIndex = 0;
            //设置用户控件大小
            controlsetting.Dock = DockStyle.Fill;
            panel1.Controls.Add(controlsetting);
        }
        private void buttonDataAnalysis_Click(object sender, EventArgs e)
        {
            if (button_onindex == ButtonIndex.ButtonDataAnalysis)
                return;
            button_onindex = ButtonIndex.ButtonDataAnalysis;
            button_refrush();
            dataanlysis.Dock = DockStyle.Fill;
            dataanlysis.alarmanalysis.AlarmDraw();
            dataanlysis.radioButtonAlarm.Checked = true;

            panel1.Controls.Add(dataanlysis);
        }

        private void buttonHome_Cancel()
        {
            panel1.Controls.Clear();
            panel1.BackgroundImage = null;
        }

        private void buttonnCheck_Cancel()
        {
            if (redismanager != null && redismanager.RedisManagerNull())
                return;
            try
            {
                redismanager.dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "Error");
            }
            ThreadWatchFlag = false;
            ThreadRefreshFlag = false;
            panel1.Controls.Clear();
            machinestate.listView1.Items.Clear();
            machinestate.listView1.Columns.Clear();
        }

        private void buttonSetting_Cancel()
        {
            panel1.Controls.Clear();
        }

        private void buttonDataAnalysis_Cancel()
        {
            panel1.Controls.Clear();
        }

        private void button_refrush()
        {
            switch (button_onindex)
            {
                case ButtonIndex.ButtonHome:
                    buttonHome.BackgroundImage = Image.FromFile("../../../../Image/TabBackground.bmp");
                    buttonCheck.BackgroundImage = null;
                    buttonSetting.BackgroundImage = null;
                    buttonDataAnalysis.BackgroundImage = null;
                    buttonnCheck_Cancel();
                    buttonSetting_Cancel();
                    buttonDataAnalysis_Cancel();
                    break;
                case ButtonIndex.ButtonCheck:
                    buttonCheck.BackgroundImage = Image.FromFile("../../../../Image/TabBackground.bmp");
                    buttonHome.BackgroundImage = null;
                    buttonSetting.BackgroundImage = null;
                    buttonDataAnalysis.BackgroundImage = null;
                    buttonHome_Cancel();
                    buttonSetting_Cancel();
                    buttonDataAnalysis_Cancel();
                    break;
                case ButtonIndex.ButtonSetting:
                    buttonSetting.BackgroundImage = Image.FromFile("../../../../Image/TabBackground.bmp");
                    buttonHome.BackgroundImage = null;
                    buttonCheck.BackgroundImage = null;
                    buttonDataAnalysis.BackgroundImage = null;
                    buttonHome_Cancel();
                    buttonnCheck_Cancel();
                    buttonDataAnalysis_Cancel();
                    break;
                case ButtonIndex.ButtonDataAnalysis:
                    buttonDataAnalysis.BackgroundImage = Image.FromFile("../../../../Image/TabBackground.bmp");
                    buttonHome.BackgroundImage = null;
                    buttonCheck.BackgroundImage = null;
                    buttonSetting.BackgroundImage = null;
                    buttonHome_Cancel();
                    buttonnCheck_Cancel();
                    buttonSetting_Cancel();
                    break;
                default:
                    break;
            }
        }

        private void tabPageServerSetting_Resize(object sender, EventArgs e)
        {
            if (button_onindex != ButtonIndex.ButtonSetting)
                return;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ThreadWatchFlag = false;
            ThreadRefreshFlag = false;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            buttonHome_Click(sender, e);
        }

        /// 通用按钮点击选项卡 在选项卡上显示对应的窗体
        /// </summary>

    }

    public partial struct ServerPara
    {
        public String ServerIPAddress;
        public int ServerPort;
        public String ServerPassword;
        public long DBNo;

        public ServerPara(String IP,int Port,String Password,long db=0){
            ServerIPAddress = IP;
            ServerPort = Port;
            ServerPassword = Password;
            DBNo = db;
        }
        public String ServerIPAddressName
        {
            get{ return ServerIPAddress; }
            set { ServerIPAddress = value; }
        }

        public int ServerPortName
        {
            get { return ServerPort; }
            set { ServerPort = value; }
        }

        public String ServerPasswordName
        {
            get { return ServerPassword; }
            set { ServerPassword = value; }
        }

        public long DBNoName
        {
            get { return DBNo; }
            set { DBNo = value; }
        }
        public bool Link(ref RedisClient Client)
        {
            try
            {
                long tmp = -1;
                Client = new RedisClient(ServerIPAddress, ServerPort, ServerPassword);
                MessageBox.Show(Client.Ping().ToString()); 
                tmp = Client.DbSize;
                if(tmp!=-1)
                    Properties.Settings.Default.Save();  // 存储上一次成功连接的IP地址和端口号
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                ServerIPAddress = null;
                ServerPort = 0;
                ServerPassword = null;
                DBNo = 0;
                return false;
            } 

            return true;
        }

        public void dispose()
        {
            ServerIPAddress = null;
            ServerPort = 0;
            ServerPassword = null;
            DBNo = 0;
        }

    }

    public  class RedisManager //redis客户端管理，连接池
    {
        private static PooledRedisClientManager _prcm;

        public PooledRedisClientManager _prcmName
        {
            get { return _prcm; }
        }

        public RedisManager(ref int initialDb, ref string[] readWriteHosts)
        {
            _prcm = new PooledRedisClientManager(initialDb, readWriteHosts);
        }

        public RedisManager()
        {
            _prcm = null;
        }

        public void dispose()
        {
            if (_prcm != null)
            {
                _prcm.Dispose();
                _prcm = null;  //销毁对象
            }
        }

        public void DisposeClient(ref RedisClient Client)
        {
            if (_prcm != null)
            {
                _prcm.DisposeClient(Client);   //销毁客户端
            }
        }


        public bool RedisManagerNull()
        {
            if (_prcm != null)
                return false;
            return true;
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private static void CreateManager(ref int initialDb, ref string[] readWriteHosts)
        {
            _prcm = new PooledRedisClientManager(initialDb, readWriteHosts);
        }
   
        public IRedisClient GetClient()
        {
            if (_prcm == null)
            {
                return null;
            }
            try
            {
                return _prcm.GetClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                return null;
            }
        }
        public IRedisClient GetClient( ref int initialDb, ref string[] readWriteHosts)
        {

            CreateManager(ref (initialDb), ref (readWriteHosts));
            try
            {
                return _prcm.GetClient();
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                return null;
            }
        }

        public IRedisClient GetReadOnlyClient(ref int initialDb, ref string[] readWriteHosts)  //只读客户端
        {
            CreateManager(ref (initialDb), ref (readWriteHosts));
            try
            {
                return _prcm.GetReadOnlyClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                return null;
            }      
        }

    }

    public enum ERRORMESSAGE
    {
        NOERR=0
    }

    public enum ButtonIndex {
        NOButton=0,
        ButtonHome=3,
        ButtonCheck,
        ButtonSetting,
        ButtonDataAnalysis
    }

    public struct MachineInfo
    {
        public string MachineName;
        public string MachineIP;
        public string MachineSN;
        public int MachinePort;
        public int MachineDB;

        public void dispose()
        {
            MachineName = null;
            MachineIP = null;
            MachineSN = null; 
            MachinePort = -1;
            MachineDB = -1;
        }
    }

    public enum MachineView
    {
        Visiable = 0,
        Online,
        Offline,
        Alarm,
        Invisiable
    }
}
