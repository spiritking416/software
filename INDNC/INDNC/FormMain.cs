using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ServiceStack.Redis;
using System.IO;
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
        RedisManager redismanager = new RedisManager();
        MySQLPara mysqlpara = new MySQLPara();          //mysql参数
        UInt16 LineCount = 5;  //生产线数量
        string LineNo = "";  //生产线编号 格式:#+索引
        string WorkShopNo = "";  //车间编号
        //bool bythreadstate = false; //线程运行flag
        ButtonIndex button_onindex = ButtonIndex.NOButton;
        RedisPara serverpara;  //服务器参数
        System.Timers.Timer t;  //timer
        bool sizechange = false;
        bool firsttimerun = false;
        //CustomTabControl customtabcontrol = new CustomTabControl();
        List<Dictionary<string, UInt16>> machineDB = new List<Dictionary<string, UInt16>>(); //机床SN码与数据库db映射关系
        List<Dictionary<string, string>> machineName = new List<Dictionary<string, string>>(); //机床SN码与机床编号映射关系
        List<Dictionary<string, UInt16>> machinePort = new List<Dictionary<string, UInt16>>(); //机床SN码与机床Port映射关系
        List<Dictionary<string, string>> machineIP = new List<Dictionary<string, string>>(); //机床SN码与机床IP映射关系
        List<MachineInfo> CNCinfo = new List<MachineInfo>();
        List<MachineInfo> Robotinfo = new List<MachineInfo>();

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
            if(LineNo == "" || WorkShopNo == "")
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
        }

        //机床状态初始化
        public void ListViewInitial()
        {
            machinestate.listView1.Items.Clear();
            try
            {
                if (serverpara.connectvalid == false)
                    throw new Exception("云端服务器参数错误，请重新设置！");
                int port = -1;
                if (int.TryParse(serverpara.RedisPort, out port) != true)
                {
                    throw new Exception("云端服务器参数错误，请重新设置！");
                }
                RedisClient Client = new RedisClient(serverpara.RedisIP, port, serverpara.RedisPassword);  //连接云端服务器
                if (!Client.Ping())
                {
                    throw new Exception("未能连接云端服务器，请检查相关参数！");
                }

                //绘制机床状态
                UInt16 connectedmachinenum = 0;  //服务器可连接机床数量
                UInt16 Alarmmachinenum = 0;   //告警机床数量
                UInt16 workmachinenum = 0;  //在线机床数量
                UInt16 disconnectedmachinenum = 0;  //离线机床数量
                UInt16 invisiblemachinenum = 0;  //因参数错误未显示机床数量
                UInt16 index = 0;

                machinestate.listView1.BeginUpdate();
                foreach (MachineInfo key in CNCinfo)
                {
                    Client.Db = key.MachineDB;
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    if (machine == null)
                    {
                        ++invisiblemachinenum;
                        continue;
                    }
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    if (machinestr == key.MachineSN) //本地和云端数据对应
                    {
                        ++index;
                        ListViewItem lvi = new ListViewItem(index.ToString());
                        lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                        ++connectedmachinenum;

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

                            ++disconnectedmachinenum;
                            lvi.SubItems.Add(key.MachineName);
                            lvi.SubItems.Add("离线");
                            lvi.SubItems.Add("无");
                            lvi.SubItems.Add(time.ToString());
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
                                ++workmachinenum;
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("在线");
                                lvi.SubItems.Add("无");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Green;
                            }
                            else
                            {
                                ++Alarmmachinenum;
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("告警");
                                lvi.SubItems.Add("\\");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Red;
                            }
                        }
                        machinestate.listView1.Items.Add(lvi);
                    }
                    else
                    {
                        ++invisiblemachinenum;
                    }
                }

                foreach (MachineInfo key in Robotinfo)
                {
                    Client.Db = key.MachineDB;
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    if (machine == null)
                    {
                        ++invisiblemachinenum;
                        continue;
                    }
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    if (machinestr == key.MachineSN) //本地和云端数据对应
                    {
                        ++index;
                        ListViewItem lvi = new ListViewItem(index.ToString());
                        lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                        ++connectedmachinenum;

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
                            ++disconnectedmachinenum;
                            lvi.SubItems.Add(key.MachineName);
                            lvi.SubItems.Add("离线");
                            lvi.SubItems.Add("无");
                            lvi.SubItems.Add(time.ToString());
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
                                ++workmachinenum;
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("在线");
                                lvi.SubItems.Add("无");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Green;
                            }
                            else
                            {
                                ++Alarmmachinenum;
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("告警");
                                lvi.SubItems.Add("\\");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Red;
                            }
                        }
                        machinestate.listView1.Items.Add(lvi);
                    }
                    else
                    {
                        ++invisiblemachinenum;
                    }
                }

                machinestate.listView1.EndUpdate();

                //设备数量显示
                machinestate.label1.Visible = true;
                machinestate.label2.Visible = true;
                machinestate.label3.Visible = true;
                machinestate.label4.Visible = true;
                machinestate.label5.Visible = true;
                machinestate.label1.Text = "生产线" + LineNo.ToString() + "设备数目:" + (CNCinfo.Count() + Robotinfo.Count()).ToString() + "台";
                machinestate.label2.Text = "在线设备数目:" + workmachinenum.ToString() + "台";
                machinestate.label3.Text = "离线设备数目:" + disconnectedmachinenum.ToString() + "台";
                machinestate.label4.Text = "告警设备数目:" + Alarmmachinenum.ToString() + "台";
                machinestate.label5.Text = "未显示设备数目:" + invisiblemachinenum.ToString() + "台";
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                if (t != null && t.Enabled)
                    t.Enabled = false;
            }
        }

        //机床状态刷新
        public void ListViewRefrush(Object source, ElapsedEventArgs e)
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
                RedisClient Client = new RedisClient(serverpara.RedisIP, port, serverpara.RedisPassword);  //连接云端服务器
                if (!Client.Ping())
                {
                    throw new Exception("未能连接云端服务器，请检查相关参数！");
                }

                //绘制机床状态
                UInt16 connectedmachinenum = 0;  //服务器可连接机床数量
                UInt16 Alarmmachinenum = 0;   //告警机床数量
                UInt16 workmachinenum = 0;  //在线机床数量
                UInt16 disconnectedmachinenum = 0;  //离线机床数量
                UInt16 invisiblemachinenum = 0;  //因参数错误未显示机床数量
                UInt16 index = 0;

                machinestate.listView1.BeginUpdate();
                foreach (MachineInfo key in CNCinfo)
                {
                    Client.Db = key.MachineDB;
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    if(machine==null)
                    {
                        ++invisiblemachinenum;
                        continue;
                    }
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    if (machinestr == key.MachineSN) //本地和云端数据对应
                    {
                        ListViewItem lvi = machinestate.listView1.Items[index];
                        ++connectedmachinenum;

                        //机床状态
                        DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                        //获取时间
                        byte[] timebyte = Client.Get("TimeStamp");
                        string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                        long timestamp = Convert.ToInt64(timestampstr);
                        var time = System.DateTime.FromBinary(timestamp);

                        Int16 clientNo = dcagentApi.HNC_NetConnect(key.MachineIP, (ushort) key.MachinePort);
                        bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                        if (isConnect == false)
                        {
                            
                            ++disconnectedmachinenum;
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
                                ++workmachinenum;
                                lvi.SubItems[2].Text = "在线";
                                lvi.SubItems[3].Text = "无";
                                lvi.SubItems[4].Text = time.ToString();
                                lvi.SubItems[2].BackColor = Color.Green;
                            }
                            else
                            {
                                ++Alarmmachinenum;
                                lvi.SubItems[2].Text = "告警";
                                lvi.SubItems[3].Text = "有";
                                lvi.SubItems[4].Text = time.ToString();
                                lvi.SubItems[2].BackColor = Color.Red;
                            }
                        }
                        ++index;
                    }
                    else
                    {
                        ++invisiblemachinenum;
                    }
                }

                foreach (MachineInfo key in Robotinfo)
                {
                    Client.Db = key.MachineDB;
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    if (machine == null)
                    {
                        ++invisiblemachinenum;
                        continue;
                    }
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    if (machinestr == key.MachineSN) //本地和云端数据对应
                    {
                        ++index;
                        ListViewItem lvi = new ListViewItem(index.ToString());
                        lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                        ++connectedmachinenum;

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
                            ++disconnectedmachinenum;
                            lvi.SubItems.Add(key.MachineName);
                            lvi.SubItems.Add("离线");
                            lvi.SubItems.Add("无");
                            lvi.SubItems.Add(time.ToString());
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
                                ++workmachinenum;
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("在线");
                                lvi.SubItems.Add("无");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Green;
                            }
                            else
                            {
                                ++Alarmmachinenum;
                                lvi.SubItems.Add(key.MachineName);
                                lvi.SubItems.Add("告警");
                                lvi.SubItems.Add("\\");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[2].BackColor = Color.Red;
                            }
                        }
                        machinestate.listView1.Items.Add(lvi);
                    }
                    else
                    {
                        ++invisiblemachinenum;
                    }
                }

                machinestate.listView1.EndUpdate();

                //设备数量显示
                machinestate.label1.Visible = true;
                machinestate.label2.Visible = true;
                machinestate.label3.Visible = true;
                machinestate.label4.Visible = true;
                machinestate.label5.Visible = true;
                machinestate.label1.Text = "生产线" + LineNo.ToString() + "设备数目:" +(CNCinfo.Count()+Robotinfo.Count()).ToString() + "台";
                machinestate.label2.Text = "在线设备数目:" + workmachinenum.ToString() + "台";
                machinestate.label3.Text = "离线设备数目:" + disconnectedmachinenum.ToString() + "台";
                machinestate.label4.Text = "告警设备数目:" + Alarmmachinenum.ToString() + "台";
                machinestate.label5.Text = "未显示设备数目:" + invisiblemachinenum.ToString() + "台";
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                if (t != null && t.Enabled)
                    t.Enabled = false;
            }
        }

        private void mySQL数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedisParaSetting redisparasetting = new RedisParaSetting();
            redisparasetting.ShowDialog();
            RedisPara tmp = new RedisPara();
            tmp = redisparasetting.redisparaName;
            if (tmp.connectvalid)
            {
                //if (tmp.RedisIP == redispara.RedisIP && tmp.RedisPort == redispara.RedisPort && tmp.RedisPassword == redispara.RedisPassword)
                    //return;
                try
                {
                    FileStream fs = new FileStream(@"../LocalRedisPara.conf", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write("RedisIP="+tmp.RedisIP+";RedisPort="+tmp.RedisPort+";RedisPassword="+tmp.RedisPassword);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
                catch (IOException ex)
                {
                    MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                }
            }
        }

        private void 产线设备参数SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LineParaSetting lineparasetting = new LineParaSetting();
            lineparasetting.ShowDialog();
            LineCount = lineparasetting.LineCountName;
        }

        private void 添加或删除设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddOrDelSetting addordelsetting = new AddOrDelSetting();
            addordelsetting.ShowDialog();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            /*
            sizechange = true;
            if (firsttimerun)
            {
                if (t != null && t.Enabled)
                    t.Enabled = false;
                machinestate.listView1.Columns.Clear();

                //绘制标题
                if (!machinestate.ListViewTitleDraw())
                {
                    throw new Exception();
                }
                this.panel1.Controls.Add(machinestate);

                //机床状态监测画面初始化
                ListViewInitial();

                //机床状态监测画面刷新
                t = new System.Timers.Timer(1000);   //实例化Timer类，设置间隔时间为10000毫秒；   
                t.Elapsed += new System.Timers.ElapsedEventHandler(ListViewRefrush); //到达时间的时候执行事件；   
                t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
                t.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   
            }*/
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            if (button_onindex == ButtonIndex.ButtonHome)
                return;
            button_onindex = ButtonIndex.ButtonHome;
            button_refrush();

        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (button_onindex == ButtonIndex.ButtonCheck)
                return;
            button_onindex = ButtonIndex.ButtonCheck;
            button_refrush();

            //host主机参数  格式“password@ip:port”
            string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort };

            try
            {
                //从连接池获得只读连接客户端
                long initialDB = 0;
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));
                if (Client == null || !Client.Ping())
                {
                    throw new Exception("连接服务器失败，请设置服务器参数!");
                }
                //连接云端服务器成功
                serverpara.connectvalid = true;
                redismanager.DisposeClient(ref (Client));  //dispose客户端
                firsttimerun = true;  //第一次运行完成

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
                string tmp = "select 机床编号,机床IP地址,机床IP端口,SN码,数据库DB from " + mysqlpara.MySQLDatabaseCNCTable;
                MySqlDataAdapter mysqlcncadapter = new MySqlDataAdapter(tmp, mysqlconnection);
                tmp = "select 机器人编号,机器人IP地址,机器人IP端口,SN码,数据库DB from " + mysqlpara.MySQLDatabaseRobotTable;
                MySqlDataAdapter mysqlrobotadapter = new MySqlDataAdapter(tmp, mysqlconnection);
                DataSet mysqlcncset = new DataSet();
                DataSet mysqlrobotset = new DataSet();
                mysqlcncadapter.Fill(mysqlcncset, mysqlpara.MySQLDatabaseCNCTable);  //填充和绑定数据
                mysqlrobotadapter.Fill(mysqlrobotset, mysqlpara.MySQLDatabaseRobotTable);
                int cncrows = mysqlcncset.Tables[0].Rows.Count;
                int robotrows = mysqlrobotset.Tables[0].Rows.Count;

                MachineInfo tmp_machineinfo = new MachineInfo();
                for(int i = 0; i < cncrows; ++i)
                {
                    tmp_machineinfo.MachineName = mysqlcncset.Tables[0].Rows[i]["机床编号"].ToString();
                    tmp_machineinfo.MachineDB = Int32.Parse(mysqlcncset.Tables[0].Rows[i]["数据库DB"].ToString());
                    tmp_machineinfo.MachineIP = mysqlcncset.Tables[0].Rows[i]["机床IP地址"].ToString();
                    tmp_machineinfo.MachinePort = Int32.Parse(mysqlcncset.Tables[0].Rows[i]["机床IP端口"].ToString());
                    tmp_machineinfo.MachineSN = mysqlcncset.Tables[0].Rows[i]["SN码"].ToString();

                    CNCinfo.Add(tmp_machineinfo);
                }

                for (int i = 0; i < robotrows; ++i)
                {
                    tmp_machineinfo.MachineName = mysqlrobotset.Tables[0].Rows[i]["机器人编号"].ToString();
                    tmp_machineinfo.MachineDB = Int32.Parse(mysqlrobotset.Tables[0].Rows[i]["数据库DB"].ToString());
                    tmp_machineinfo.MachineIP = mysqlrobotset.Tables[0].Rows[i]["机器人IP地址"].ToString();
                    tmp_machineinfo.MachinePort = Int32.Parse(mysqlrobotset.Tables[0].Rows[i]["机器人IP端口"].ToString());
                    tmp_machineinfo.MachineSN = mysqlrobotset.Tables[0].Rows[i]["SN码"].ToString();

                    Robotinfo.Add(tmp_machineinfo);
                }

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

                //机床状态监测画面初始化
                ListViewInitial();

                //机床状态监测画面刷新  
                t = new System.Timers.Timer(1000);   //实例化Timer类，设置间隔时间为10000毫秒；   
                t.Elapsed += new System.Timers.ElapsedEventHandler(ListViewRefrush); //到达时间的时候执行事件；   
                t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
                t.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   

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
            //设置用户控件大小
            controlsetting.Dock = DockStyle.Fill;
            panel1.Controls.Add(controlsetting);
            controlsetting.btnServerSettingClick += new btnOkClickEventHander(ControlServerSettingclick);
            controlsetting.btnLineSettingClick += new btnOkClickEventHander(ControlLineSettingclick);
            controlsetting.btnCNCSettingClick+= new btnOkClickEventHander(ControlCNCSettingclick);
            controlsetting.btnRobotSettingClick += new btnOkClickEventHander(ControlRobotSettingclick);
        }

        //
        private void ControlServerSettingclick(object send, System.EventArgs e)
        {
            serverpara = controlsetting.serverparaName;
            mysqlpara = controlsetting.mysqlparaName;
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

        private void buttonHome_Cancel()
        {
            panel1.Controls.Clear();
        }

        private void buttonnCheck_Cancel()
        {
            panel1.Controls.Clear();
            machinestate.listView1.Items.Clear();
            machinestate.listView1.Columns.Clear();

            if (redismanager != null && redismanager.RedisManagerNull())
                return;
            try
            {
                if (t != null && t.Enabled)
                    t.Enabled = false;
                sizechange = false;
                redismanager.dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "Error");
            }
        }
        private void buttonSetting_Cancel()
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
                    buttonnCheck_Cancel();
                    buttonSetting_Cancel();
                    break;
                case ButtonIndex.ButtonCheck:
                    buttonCheck.BackgroundImage = Image.FromFile("../../../../Image/TabBackground.bmp");
                    buttonHome.BackgroundImage = null;
                    buttonSetting.BackgroundImage = null;
                    buttonHome_Cancel();
                    buttonSetting_Cancel();
                    break;
                case ButtonIndex.ButtonSetting:
                    buttonSetting.BackgroundImage = Image.FromFile("../../../../Image/TabBackground.bmp");
                    buttonHome.BackgroundImage = null;
                    buttonCheck.BackgroundImage = null;
                    buttonHome_Cancel();
                    buttonnCheck_Cancel();
                    break;
                default:
                    break;
            }
        }

        private void tabControlSetting_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPageServerSetting_Resize(object sender, EventArgs e)
        {
            if (button_onindex != ButtonIndex.ButtonSetting)
                return;

        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

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
                Client = new RedisClient(ServerIPAddress, ServerPort, ServerPassword, DBNo);
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

        public RedisManager(ref long initialDb, ref string[] readWriteHosts)
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
        private static void CreateManager(ref long initialDb,ref string[] readWriteHosts)
        {
            _prcm = new PooledRedisClientManager(readWriteHosts, readWriteHosts, new RedisClientManagerConfig
            {
                MaxWritePoolSize = 20,//“写”链接池链接数
                MaxReadPoolSize = 20,//“写”链接池链接数
                DefaultDb=0,
                AutoStart = true
            }, initialDb,null, null);
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
        public IRedisClient GetClient( ref long initialDb, ref string[] readWriteHosts)
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

        public IRedisClient GetReadOnlyClient(ref long initialDb, ref string[] readWriteHosts)  //只读客户端
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
        ButtonSetting
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
}
