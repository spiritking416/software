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
        MySQLPara mysqlpara = new MySQLPara();
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

        public FormMain()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            CheckForIllegalCrossThreadCalls = false; //解决线程间的访问限制

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
        {/*
            if(machinestate!=null)
                machinestate.listView1.Items.Clear();
            ushort DB = 0;
            if (machinestate != null && machinestate.listView1 != null && machinestate.listView1.Items != null)
                machinestate.listView1.Items.Clear();
            
            try
            {
                if(serverpara.connectvalid==false)
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
                if (int.TryParse(redispara.RedisPort, out port) != true)
                {
                    throw new Exception("Redis本地服务器参数错误，请重新设置！");
                }
                RedisClient LocalClient = new RedisClient(redispara.RedisIP, port, redispara.RedisPassword);  //连接本地服务器
                if (!LocalClient.Ping())
                {
                    throw new Exception("未能连接本地服务器，请检查相关参数！");
                }
                if (LineNo == 0 ||　WorkShopNo==0)
                    throw new Exception("生产线设备参数设置错误，请重新设置！");
                string lineindex = "Line" + LineNo.ToString();
                byte[][] value = new byte[][] { };
                value = LocalClient.SMembers(lineindex);
                List<string> machineSN = new List<string>();  //生产线包含的设备SN码集合
                for (int i = 0; i < value.Length; ++i)
                    machineSN.Add(System.Text.Encoding.Default.GetString(value[i]));
                string databases_tring;
                byte[][] tmp = new byte[][] { };
                tmp = Client.ConfigGet("databases");
                databases_tring = System.Text.Encoding.Default.GetString(tmp[1]);
                int databases=-1;  //云服务器db数量
                if (int.TryParse(databases_tring, out databases) != true)
                {
                    throw new Exception("云端服务器参数错误，请重新设置！");
                }
                //遍历云端服务器db
                for(int i = 0; i < LineCount; ++i)
                {
                    machineDB.Add(new Dictionary<string, ushort>());
                    machineName.Add(new Dictionary<string, string>());
                    machinePort.Add(new Dictionary<string, ushort>());
                    machineIP.Add(new Dictionary<string, string>());
                }
                UInt16 linenoindex = (UInt16)(LineNo - 1);
                for(int i = 0; i < databases; ++i)
                {
                    DB = (ushort)i;
                    Client.Db = DB;
                    byte[] tmppara = new byte[] { };
                    tmppara = Client.Get("Machine");
                    string SN = null;
                    if (tmppara != null)
                    {
                        SN = System.Text.Encoding.Default.GetString(tmppara);
                        if (machineSN.Contains(SN))
                        {
                            machineDB[linenoindex][SN] = DB;
                        }
                            
                    }     
                }

                //设置本地设备DB && 建立SN与机床编号映射
                LocalClient.Db = 0;
                byte[] textkey = Encoding.UTF8.GetBytes("DB");
                foreach(string key in machineDB[linenoindex].Keys)
                {
                    string texttmp = machineDB[linenoindex][key].ToString();
                    byte[] hashvalue = Encoding.UTF8.GetBytes(texttmp);
                    LocalClient.HSet("MachineSN:" + key, textkey, hashvalue);  //设置本地设备DB参数

                    byte[] machinenamebytes = new byte[] { };
                    byte[] tmpbytes = Encoding.UTF8.GetBytes("MachineNo");
                    machinenamebytes =LocalClient.HGet("MachineSN:" + key, tmpbytes);
                    string machinename= System.Text.Encoding.Default.GetString(machinenamebytes);
                    machineName[linenoindex][key] = machinename;
                }

                //初始化绘制机床状态
                UInt16 connectedmachinenum = 0;  //服务器可连接机床数量
                UInt16 Alarmmachinenum = 0;   //告警机床数量
                UInt16 workmachinenum = 0;  //在线机床数量
                UInt16 disconnectedmachinenum = 0;  //离线机床数量
                UInt16 invisiblemachinenum = 0;  //因参数错误未显示机床数量
                int lineno = LineNo - 1;

                machinestate.listView1.BeginUpdate();  
                foreach (string key in machineDB[lineno].Keys)
                {
                    Client.Db = machineDB[lineno][key];
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    if (machinestr == key)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                        string tmpstr = machineName[lineno][key];
                        if (tmpstr != null && tmpstr != "")
                        {
                            ++connectedmachinenum;
                            lvi.Text = tmpstr;

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);
                            // HNC_NetConnect 连接Redis数据库并获取连接设备号
                            //获取本地服务器上记录的机床IP,Port
                            byte[] machinebytes = new byte[] { };
                            byte[] tmpbytes = Encoding.UTF8.GetBytes("IP");
                            machinebytes = LocalClient.HGet("MachineSN:" + key, tmpbytes);
                            string machineip = System.Text.Encoding.Default.GetString(machinebytes);
                            tmpbytes = Encoding.UTF8.GetBytes("Port");
                            machinebytes = LocalClient.HGet("MachineSN:" + key, tmpbytes);
                            string machineportstr = System.Text.Encoding.Default.GetString(machinebytes);
                            ushort machineport = 0;
                            if (UInt16.TryParse(machineportstr, out machineport) != true)
                            {
                                MessageBox.Show("因生产线设备参数错误，SN码为" + key + "，编号为" + lvi.Text + "的设备未显示,请修改设备参数！");
                                ++invisiblemachinenum;
                                continue;  //跳过此设备
                            }

                            //获取云服务器上记录的机床IP,Port
                            machinebytes = Client.Get("IP");
                            string machineserverip = System.Text.Encoding.Default.GetString(machinebytes);
                            machinebytes = Client.Get("Port");
                            string machineserverportstr = System.Text.Encoding.Default.GetString(machinebytes);
                            ushort machineserverport = 0;
                            if (UInt16.TryParse(machineserverportstr, out machineserverport) != true)
                            {
                                MessageBox.Show("因生产线设备参数错误，SN码为" + key + "，编号为" + lvi.Text + "的设备未显示,请修改设备参数！");
                                ++invisiblemachinenum;
                                continue;  //跳过此设备
                            }

                            if(machineip!=machineserverip || machineport != machineserverport)
                            {
                                MessageBox.Show("因云端和本地设备参数有出入，SN码为"+ key + "，编号为" + lvi.Text + "的设备未显示,请修改设备参数！");
                                ++invisiblemachinenum;
                                continue;  //跳过此设备
                            }
                            machineIP[lineno][key] = machineip;  //key-ip
                            machinePort[lineno][key] = machineport;   //key-port

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time= System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(machineip, machineport);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                ++disconnectedmachinenum;
                                lvi.SubItems.Add("离线");
                                lvi.SubItems.Add("\\");
                                lvi.SubItems.Add(time.ToString());
                                lvi.SubItems[1].BackColor = Color.Gray;                     
                            }
                            else
                            {
                                byte[] machinealarmbyte = new byte[] { };
                                byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_CURRENT");
                                machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                                string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                                long machinealarm= Convert.ToInt64(machinealarmstr);

                                if (machinealarm == 0)
                                {
                                    ++workmachinenum;
                                    lvi.SubItems.Add("在线");
                                    lvi.SubItems.Add("\\");
                                    lvi.SubItems.Add(time.ToString());
                                    lvi.SubItems[1].BackColor = Color.Green;
                                }
                                else
                                {
                                    ++Alarmmachinenum;
                                    lvi.SubItems.Add("告警");
                                    lvi.SubItems.Add("\\");
                                    lvi.SubItems.Add(time.ToString());
                                    lvi.SubItems[1].BackColor = Color.Red;
                                }
                            }

                            machinestate.listView1.Items.Add(lvi);
                        }

                    }
                }

                machinestate.listView1.EndUpdate();

                //设备数量显示
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label7.Text = "生产线" + LineNo.ToString() + "设备数目:" + machineDB[lineno].Count.ToString() + "台";
                label8.Text = "在线设备数目:" + workmachinenum.ToString() + "台";
                label9.Text = "离线设备数目:" + disconnectedmachinenum.ToString() + "台";
                label10.Text = "告警设备数目:" + Alarmmachinenum.ToString() + "台";
                label11.Text = "未显示设备数目:" + invisiblemachinenum.ToString() + "台";
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }*/
        }

        //机床状态刷新
        public void ListViewRefrush(Object source, ElapsedEventArgs e)
        {
            /*
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
                if (int.TryParse(redispara.RedisPort, out port) != true)
                {
                    throw new Exception("Redis本地服务器参数错误，请重新设置！");
                }
                RedisClient LocalClient = new RedisClient(redispara.RedisIP, port, redispara.RedisPassword);  //连接本地服务器
                if (!LocalClient.Ping())
                {
                    throw new Exception("未能连接本地服务器，请检查相关参数！");
                }

                //绘制机床状态
                UInt16 connectedmachinenum = 0;  //服务器可连接机床数量
                UInt16 Alarmmachinenum = 0;   //告警机床数量
                UInt16 workmachinenum = 0;  //在线机床数量
                UInt16 disconnectedmachinenum = 0;  //离线机床数量
                UInt16 invisiblemachinenum = 0;  //因参数错误未显示机床数量
                int lineno = LineNo - 1;
                int index = 0;

                machinestate.listView1.BeginUpdate();
                foreach (string key in machineDB[lineno].Keys)
                {
                    Client.Db = machineDB[lineno][key];
                    byte[] machine = new byte[] { };
                    machine = Client.Get("Machine");
                    string machinestr = System.Text.Encoding.Default.GetString(machine);
                    if (machinestr == key)
                    {
                        string tmpstr = machineName[lineno][key];
                        if (tmpstr != null && tmpstr != "")
                        {
                            if (index >= machinestate.listView1.Items.Count)
                                break;
                            ListViewItem lvi = machinestate.listView1.Items[index];
                            if (lvi.Text != tmpstr)
                            {
                                ++invisiblemachinenum;
                                continue; //跳过未显示的key
                            }

                            ++index;
                            lvi.UseItemStyleForSubItems = false; //可以设置单元格背景
                            ++connectedmachinenum;

                            //机床状态
                            DCAgentApi dcagentApi = DCAgentApi.GetInstance(serverpara.RedisIP);

                            //获取时间
                            byte[] timebyte = Client.Get("TimeStamp");
                            string timestampstr = System.Text.Encoding.Default.GetString(timebyte);
                            long timestamp = Convert.ToInt64(timestampstr);
                            var time = System.DateTime.FromBinary(timestamp);

                            Int16 clientNo = dcagentApi.HNC_NetConnect(machineIP[lineno][key], machinePort[lineno][key]);
                            bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                            if (isConnect == false)
                            {
                                ++disconnectedmachinenum;
                                lvi.SubItems[1].Text = "离线";
                                lvi.SubItems[2].Text = "\\";
                                lvi.SubItems[3].Text = time.ToString();
                                lvi.SubItems[1].BackColor = Color.Gray;
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
                                    lvi.SubItems[1].Text = "在线";
                                    lvi.SubItems[2].Text = "\\";
                                    lvi.SubItems[3].Text = time.ToString();
                                    lvi.SubItems[1].BackColor = Color.Green;
                                }
                                else
                                {
                                    ++Alarmmachinenum;
                                    lvi.SubItems[1].Text = "告警";
                                    lvi.SubItems[2].Text = "\\";
                                    lvi.SubItems[3].Text = time.ToString();
                                    lvi.SubItems[1].BackColor = Color.Red;
                                }
                            }
                        }

                    }
                }

                machinestate.listView1.EndUpdate();

                //设备数量显示
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label7.Text = "生产线" + LineNo.ToString() + "设备数目:" + machineDB[lineno].Count.ToString() + "台";
                label8.Text = "在线设备数目:" + workmachinenum.ToString() + "台";
                label9.Text = "离线设备数目:" + disconnectedmachinenum.ToString() + "台";
                label10.Text = "告警设备数目:" + Alarmmachinenum.ToString() + "台";
                label11.Text = "未显示设备数目:" + invisiblemachinenum.ToString() + "台";
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                if (t != null && t.Enabled)
                    t.Enabled = false;
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);

            //redispara = redisparasetting.redisparaName;
            //服务器连接ip,por,password设置
            for (int i = 1; i <= 4; ++i)
            {
                //检验输入是否为数字，数字是否介于0~255之间
                int tmp = -1;
                TextBox objText = (TextBox)this.panel2.Controls["textBox" + i.ToString()];
                if (int.TryParse(objText.Text, out tmp) != true)
                {
                    MessageBox.Show("错误:服务器IP地址输入错误，请重新输入！", "ERROR");
                    return;
                }
                else if(!(tmp>=0 && tmp <= 255))
                {
                    MessageBox.Show("错误:服务器IP地址输入错误，请重新输入！", "ERROR");
                    return;
                }
                    
            }
            String IP= textBox1.Text + '.' + textBox2.Text + '.' + textBox3.Text + '.' + textBox4.Text;
            int port = 0;
            if (int.TryParse(textBox5.Text, out port) != true)
            {
                MessageBox.Show("错误:服务器端口号输入错误，请重新输入！", "ERROR");
                return;
            }
            serverpara.RedisIP = IP;
            serverpara.RedisPort = textBox5.Text;
            serverpara.RedisPassword= textBox6.Text;
            serverpara.connectvalid = false;

            //host主机参数  格式“password@ip:port”
            string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort }; 

            try
            {
                //从连接池获得只读连接客户端
                long initialDB = 0;
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));
                //byte[] ConnectTimeout = System.BitConverter.GetBytes(3);
                //Client.ConfigSet("repl-ping-slave-period", ConnectTimeout);
                if (Client==null ||　!Client.Ping())
                {
                    throw new Exception("连接服务器失败!");
                }
                //连接成功
                serverpara.connectvalid = true;
                redismanager.DisposeClient(ref (Client));  //dispose客户端
                Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号\
                firsttimerun = true;  //第一次运行完成

                //测试连接
                MessageBox.Show(Client.Db.ToString());  //db index
                MessageBox.Show(Client.DbSize.ToString());

                //绘制用户界面
                machinestate.Visible = true;
                machinestate.listView1.Visible = true;
                machinestate.Dock = DockStyle.Fill;

                //绘制标题
                machinestate.Height = this.panel1.Height;
                machinestate.Width = this.panel1.Width;
                if (machinestate.listView1 == null)
                    machinestate.listView1 = new ListView();

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

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                redismanager.dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (redismanager!= null && redismanager.RedisManagerNull())
                return;
            try
            {
                if (t!=null && t.Enabled)
                    t.Enabled = false;
                sizechange = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                redismanager.dispose();
                machinestate.Visible = false;
                machinestate.listView1.Visible = false;
                machineDB.Clear();
                serverpara.dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "Error");
            }
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsNumber(e.KeyChar)) || e.KeyChar == (char)8)
                e.Handled = false;
            else if(e.KeyChar == (char)13)
            {
                button1.Focus();
                button1_Click(sender, e);
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("请输入数字", "ERROR");
            }       
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsNumber(e.KeyChar)) || e.KeyChar == (char)8)
                e.Handled = false;
            else if (e.KeyChar == (char)13)
            {
                button1.Focus();
                button1_Click(sender, e);
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("请输入数字", "ERROR");
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsNumber(e.KeyChar)) || e.KeyChar == (char)8)
                e.Handled = false;
            else if (e.KeyChar == (char)13)
            {
                button1.Focus();
                button1_Click(sender, e);
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("请输入数字", "ERROR");
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsNumber(e.KeyChar)) || e.KeyChar == (char)8)
                e.Handled = false;
            else if (e.KeyChar == (char)13)
            {
                button1.Focus();
                button1_Click(sender, e);
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("请输入数字", "ERROR");
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsNumber(e.KeyChar)) || e.KeyChar == (char)8 )
                e.Handled = false;
            else if (e.KeyChar == (char)13)
            {
                button1.Focus();
                button1_Click(sender, e);
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("请输入数字", "ERROR");
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button1.Focus();
                button1_Click(sender, e);
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
            }
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
                //连接成功
                serverpara.connectvalid = true;
                redismanager.DisposeClient(ref (Client));  //dispose客户端
                firsttimerun = true;  //第一次运行完成

                //测试连接
                MessageBox.Show(Client.Db.ToString());  //db index
                MessageBox.Show(Client.DbSize.ToString());

                //属性设置
                machinestate.Dock = DockStyle.Fill;
                machinestate.Height = this.panel1.Height;
                machinestate.Width = this.panel1.Width;

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
                //t = new System.Timers.Timer(1000);   //实例化Timer类，设置间隔时间为10000毫秒；   
                //t.Elapsed += new System.Timers.ElapsedEventHandler(ListViewRefrush); //到达时间的时候执行事件；   
                //t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
                //t.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   

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
        }

        //
        private void ControlServerSettingclick(object send, System.EventArgs e)
        {
            serverpara = controlsetting.serverparaName;
            mysqlpara = controlsetting.mysqlparaName;
            MessageBox.Show("服务器参数设置完毕", "提示");
        }

        private void ControlLineSettingclick(object send, System.EventArgs e)
        {
            WorkShopNo = controlsetting.workshopnoName;
            LineNo = controlsetting.linenoName;
            MessageBox.Show("产线参数设置完毕", "提示");
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
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
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

}
