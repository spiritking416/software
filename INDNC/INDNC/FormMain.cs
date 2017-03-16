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
    public partial class FormMain : Form
    {

        UserControlMachineState machinestate = new UserControlMachineState();
        RedisManager redismanager = new RedisManager();
        RedisPara redispara = new RedisPara();
        UInt16 LineCount = 0;  //生产线数量
        UInt16 LineNo = 0;  //生产线编号
        RedisPara serverpara;  //服务器参数
        bool bThreadIsExit = false; //线程状态
        List<Dictionary<string, UInt16>> machineDB = new List<Dictionary<string, UInt16>>(); //机床SN码与数据库db映射关系
        List<Dictionary<string, string>> machineName = new List<Dictionary<string, string>>(); //机床SN码与机床编号映射关系

        public FormMain()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            CheckForIllegalCrossThreadCalls = false; //解决线程间的访问限制

            //LineNo、LineCount初始化
            LineNo = 0;
            LineCount = 0;
            string lineindex = global::INDNC.Properties.Settings.Default.LineIndex;
            string linecount = global::INDNC.Properties.Settings.Default.LineCount;
            if(lineindex=="" || linecount == "")
            {
                lineindex = "生产线路1";
                linecount = "1";
            }
            string countstring = "";
            if (lineindex != "")
            {
                int offset = lineindex.IndexOf("生产线路") + 4;
                countstring = lineindex.Substring(offset);
            }
            
            try
            {
                if (UInt16.TryParse(countstring, out LineNo) != true)
                    throw new Exception();
                if (UInt16.TryParse(linecount, out LineCount) != true)
                    throw new Exception();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR:软件初始化错误，请设置相关参数！", "ERROR");
            }

            //redispara初始化

            try
            {
                //读取本地Redis服务器参数
                FileStream fs = new FileStream(@"../LocalRedisPara.conf", FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(fs);
                string str = sr.ReadLine();
                //关闭流
                sr.Close();
                fs.Close();
                if (str == null || str == "")
                    return;
                int offsetIP = str.IndexOf("RedisIP=");
                int offsetPort = str.IndexOf("RedisPort=");
                int offsetPW = str.IndexOf("RedisPassword=");
                if (offsetIP < 0 || offsetPort < 0 || offsetPW < 0)
                {
                    throw new Exception("ERROR:本地Redis服务器参数配置错误！");
                }

                offsetIP += 8;
                offsetPort += 10;
                offsetPW += 14;

                int i = str.IndexOf(';');
                redispara.RedisIP = str.Substring(offsetIP, i - offsetIP);
                i = str.IndexOf(';', i + 1);
                redispara.RedisPort = str.Substring(offsetPort, i - offsetPort);
                i = str.IndexOf('\0', i + 1);
                redispara.RedisPassword = str.Substring(offsetPW);
                redispara.connectvalid = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
        }

        /*public struct machine
        {
            UInt16 MachineIndex;
            String MachineState;
            String MachineAlarm;
            
        }*/

        //机床状态初始化
        public void ListViewInitial()
        {
            ushort DB = 0;
            machinestate.listView1.Clear();
            
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
                if (LineNo == 0 ||　LineCount==0)
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

                            //byte[] machinenamebytes = new byte[] { };
                            //byte[] tmpbytes = Encoding.UTF8.GetBytes("MachineNo");
                            //machinenamebytes = LocalClient.HGet(key, tmpbytes);
                            //string machinename = System.Text.Encoding.Default.GetString(machinenamebytes);
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
            }

        }

        //机床状态刷新
        public void ListViewRefrush()
        {
            
            while (bThreadIsExit)//bThreadIsExit 这个布尔值是用来其他线程向这个线程发送关闭线程命令的，用于安全的结束此线程
            {
                try
                {
                    long initialDB = 0;
                    string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort };
                    RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));
                    int lineno = LineNo - 1;
                    if(lineno<0)
                        throw new Exception("生产线设备参数设置错误，请重新设置！");
                    UInt16 connectedmachinenum = 0;

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
                            string tmp = machineName[lineno][key];
                            if(tmp!=null && tmp !="")
                            {
                                ++connectedmachinenum;
                                lvi.Text = tmp;

                                machinestate.listView1.Items.Add(lvi);
                            }
                            
                        }
                    }

                    machinestate.listView1.EndUpdate();

                    
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                    bThreadIsExit = false;
                    machinestate.listView1.Visible = false;
                    label7.Visible = false;
                    label8.Visible = false;
                    label9.Visible = false;
                    label10.Visible = false;
                    label11.Visible = false;
                }
            }

            /*
            RedisPubSubServer pubSubServer = new RedisPubSubServer(redismanager._prcmName, "channel")
            {
                OnMessage = (channel, msg) =>
                {
                    MessageBox.Show(msg);
                    //Console.WriteLine($"从频道：{channel}上接受到消息：{msg},时间：{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}");
                },
                OnStart = () =>
                {
                    //Console.WriteLine("发布服务已启动");
                    //Console.WriteLine("___________________________________________________________________");
                },
                OnStop = () => { 
                    MessageBox.Show("发布服务停止");
                    Console.WriteLine("发布服务停止"); },
                OnUnSubscribe = channel => { Console.WriteLine(channel); },
                //OnError = e => { Console.WriteLine(e.Message); },
                OnFailover = s => { Console.WriteLine(s); },
            };
            pubSubServer.Start();*/
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
                TextBox objText = (TextBox)this.Controls["textBox" + i.ToString()];
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
                Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号

                //测试连接
                MessageBox.Show(Client.Db.ToString());  //db index
                MessageBox.Show(Client.DbSize.ToString());

                //绘制用户界面
                if (machinestate == null)
                    machinestate = new UserControlMachineState();
                machinestate.Visible = true;
                machinestate.Dock = DockStyle.Fill;

                //机床状态监测画面初始化
                ListViewInitial();

                //绘制标题
                if (!machinestate.ListViewTitleDraw(ref (LineNo)))
                {
                    throw new Exception();
                }
                this.panel1.Controls.Add(machinestate);

                //机床状态监测画面刷新
                //machinestate.threadRefrush = new Thread(ListViewRefrush);
                //bThreadIsExit = true;
                //machinestate.threadRefrush.Start();  //线程开始

                //DCAgent
                //DCAgentApi dcagentApi = DCAgentApi.GetInstance("127.0.0.1");
                // HNC_NetConnect 连接Redis数据库并获取连接设备号
                //Int16 clientNo = dcagentApi.HNC_NetConnect("192.168.213.197", 10001);
                //MessageBox.Show(clientNo.ToString()); 
                //bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                //MessageBox.Show(isConnect.ToString());

                //测试
                /*RedisPubSubServer pubSubServer = new RedisPubSubServer(redismanager._prcmName, "channel")
                {
                    OnMessage = (channel, msg) =>
                    {
                        MessageBox.Show(msg);
                        //Console.WriteLine($"从频道：{channel}上接受到消息：{msg},时间：{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}");
                    },
                    OnStart = () =>
                    {
                        //Console.WriteLine("发布服务已启动");
                        //Console.WriteLine("___________________________________________________________________");
                    },
                    OnStop = () => { Console.WriteLine("发布服务停止"); },
                    OnUnSubscribe = channel => { Console.WriteLine(channel); },
                    //OnError = e => { Console.WriteLine(e.Message); },
                    OnFailover = s => { Console.WriteLine(s); },
                };
                pubSubServer.Start();*/

                //Task t1 = new Task();

                /*
                IRedisSubscription subscription = Client.CreateSubscription();
                //接受到消息时
                subscription.OnMessage = (channel, msg) =>
                {
                    MessageBox.Show(msg);
                    //Console.WriteLine($"从频道：{channel}上接受到消息：{msg},时间：{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}");
                    //Console.WriteLine($"频道订阅数目：{subscription.SubscriptionCount}");
                    //Console.WriteLine("___________________________________________________________________");
                };
                //订阅频道时
                subscription.OnSubscribe = (channel) =>
                {
                    MessageBox.Show("订阅客户端：开始订阅" + channel);
                    //Console.WriteLine("订阅客户端：开始订阅" + channel);
                };
                //取消订阅频道时
                subscription.OnUnSubscribe = (a) => { Console.WriteLine("订阅客户端：取消订阅"); };

                //订阅频道
                subscription.SubscribeToChannels("channel");*/

                //Task t1 = new Task();
                //t1.Start();

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
            finally
            {
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (redismanager.RedisManagerNull())
                return;
            try
            {
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                bThreadIsExit = false;
                redismanager.dispose();
                machinestate.Visible = false;
                machinestate.listView1.Clear();
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
                    redispara = redisparasetting.redisparaName;
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
            LineNo = lineparasetting.LineNoName;
            LineCount = lineparasetting.LineCountName;
        }

        private void 生产线路ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 添加或删除设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddOrDelSetting addordelsetting = new AddOrDelSetting();
            addordelsetting.redisparaName = redispara;
            addordelsetting.ShowDialog();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            bThreadIsExit = false;
        }
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

}
