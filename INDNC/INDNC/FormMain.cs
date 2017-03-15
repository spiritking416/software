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

        UserControlMachineState machinestate;
        RedisManager redismanager = new RedisManager();
        RedisPara redispara = new RedisPara();
        UInt16 LineCount = 0;  //生产线数量
        UInt16 LineNo = 0;  //生产线编号
        ServerPara serverpara;  //服务器参数
        bool bThreadIsExit = false; //线程状态

        public FormMain()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            //LineNo、LineCount初始化
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
            
        }

        //机床状态刷新
        public void ListViewRefrush()
        {

            while (bThreadIsExit)//bThreadIsExit 这个布尔值是用来其他线程向这个线程发送关闭线程命令的，用于安全的结束此线程
            {
                //long initialDB = 0;
                //string[] host = { serverpara.ServerPassword + '@' + serverpara.ServerIPAddress + ':' + serverpara.ServerPort };
                //RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));

                
                MessageBox.Show("sdfsadf");
                Thread.Sleep(1000);
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
            String password = textBox6.Text;
            serverpara = new ServerPara(IP,port,password);

            //host主机参数  格式“password@ip:port”
            string[] host = { serverpara.ServerPassword + '@' + serverpara.ServerIPAddress + ':' + serverpara.ServerPort }; 

            try
            {
                //从连接池获得只读连接客户端
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (serverpara.DBNo), ref (host));
                if (Client==null ||　!Client.Ping())
                {
                    throw new Exception("连接服务器失败!");
                }
                //连接成功s
                redismanager.DisposeClient(ref (Client));  //dispose客户端
                Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号

                //测试连接
                MessageBox.Show(Client.Db.ToString());  //db index
                MessageBox.Show(Client.DbSize.ToString());

                //绘制用户界面
                machinestate = new UserControlMachineState();
                machinestate.Visible = true;
                machinestate.Dock = DockStyle.Fill;

                //绘制标题
                if (!machinestate.ListViewTitleDraw(ref (LineNo)))
                {
                    throw new Exception();
                }
                this.panel1.Controls.Add(machinestate);


                machinestate.threadRefrush = new Thread(ListViewRefrush);
                bThreadIsExit = true;
                machinestate.threadRefrush.Start();  //线程开始


                //DCAgent
                DCAgentApi dcagentApi = DCAgentApi.GetInstance("127.0.0.1");

                // HNC_NetConnect 连接Redis数据库并获取连接设备号
                Int16 clientNo = dcagentApi.HNC_NetConnect("192.168.213.197", 10001);

                MessageBox.Show(clientNo.ToString());
                bool isConnect = dcagentApi.HNC_NetIsConnect(clientNo);
                MessageBox.Show(isConnect.ToString());

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
                serverpara.dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (redismanager.RedisManagerNull())
                return;
            try
            {
                bThreadIsExit = false;
                redismanager.dispose();
                machinestate.Visible = false;
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
                if (tmp.RedisIP == redispara.RedisIP && tmp.RedisPort == redispara.RedisPort && tmp.RedisPassword == redispara.RedisPassword)
                    return;
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
                AutoStart = true,
            }, initialDb,null,null);
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
