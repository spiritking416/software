using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceStack.Redis;
using System.IO;

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
        public FormMain()
        {
            InitializeComponent();
        }

        /*public struct machine
        {
            UInt16 MachineIndex;
            String MachineState;
            String MachineAlarm;
            
        }*/

        UserControlMachineState machinestate;
        UInt16 lineno = 0;  //生产线编号
        RedisManager redismanager = new RedisManager();
        RedisPara redispara = new RedisPara();

        //MySqlConnection mysqlconnetion = new MySqlConnection();

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
            ServerPara serverpara = new ServerPara(IP,port,password);

            //host主机参数  格式“password@ip:port”
            string[] host = { serverpara.ServerPassword + '@' + serverpara.ServerIPAddress + ':' + serverpara.ServerPort }; 

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
                    throw new Exception("本地Redis服务器参数配置错误！");
                int offsetIP = str.IndexOf("RedisIP=");
                int offsetPort = str.IndexOf("RedisPort=");
                int offsetPW = str.IndexOf("RedisPassword=");
                if(offsetIP<0 || offsetPort<0 || offsetPW < 0)
                {
                    throw new Exception("本地Redis服务器参数配置错误！");
                }

                offsetIP += 8;
                offsetPort += 10;
                offsetPW += 14;

                int i = str.IndexOf(';');
                redispara.RedisIP = str.Substring(offsetIP, i- offsetIP);
                i = str.IndexOf(';', i + 1);
                redispara.RedisPort = str.Substring(offsetPort, i - offsetPort);
                i = str.IndexOf('\0', i + 1);
                redispara.RedisPassword = str.Substring(offsetPW);
                redispara.connectvalid = true;

                //从连接池获得只读连接客户端
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (serverpara.DBNo), ref (host));
                if (Client==null ||　!Client.Ping())
                {
                    throw new Exception("连接服务器失败!");
                }
                Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号

                //测试连接
                MessageBox.Show(Client.Db.ToString());  //db index
                MessageBox.Show(Client.DbSize.ToString());

                //绘制用户界面
                machinestate = new UserControlMachineState();
                machinestate.Visible = true;
                machinestate.Dock = DockStyle.Fill;

                if (this.toolStripMenuItem1.CheckState == CheckState.Checked)
                    lineno = 1;
                else if (this.toolStripMenuItem2.CheckState == CheckState.Checked)
                    lineno = 2;
                else if (this.toolStripMenuItem3.CheckState == CheckState.Checked)
                    lineno = 3;
                else if (this.toolStripMenuItem4.CheckState == CheckState.Checked)
                    lineno = 4;
                else
                    lineno = 0;

                    if (!machinestate.ListViewDraw(ref (Client),ref (lineno)))
                {
                    throw new Exception();
                }
                this.panel1.Controls.Add(machinestate);

                //Task t1 = new Task();
                //t1.Start();

            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                if (machinestate != null)
                {
                    machinestate.Visible = false;
                    machinestate = null;
                }
                redismanager.dispose();
                redispara.dispose();
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem1.CheckState== CheckState.Checked)
            {
                return;
            }
            global::INDNC.Properties.Settings.Default.line1 = CheckState.Checked;
            global::INDNC.Properties.Settings.Default.line2 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line3 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line4 = CheckState.Unchecked;
            this.toolStripMenuItem1.CheckState = global::INDNC.Properties.Settings.Default.line1;
            this.toolStripMenuItem2.CheckState = global::INDNC.Properties.Settings.Default.line2;
            this.toolStripMenuItem3.CheckState = global::INDNC.Properties.Settings.Default.line3;
            this.toolStripMenuItem4.CheckState = global::INDNC.Properties.Settings.Default.line4;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem2.CheckState == CheckState.Checked)
            {
                return;
            }
            global::INDNC.Properties.Settings.Default.line2 = CheckState.Checked;
            global::INDNC.Properties.Settings.Default.line1 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line3 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line4 = CheckState.Unchecked;
            this.toolStripMenuItem1.CheckState = global::INDNC.Properties.Settings.Default.line1;
            this.toolStripMenuItem2.CheckState = global::INDNC.Properties.Settings.Default.line2;
            this.toolStripMenuItem3.CheckState = global::INDNC.Properties.Settings.Default.line3;
            this.toolStripMenuItem4.CheckState = global::INDNC.Properties.Settings.Default.line4;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem3.CheckState == CheckState.Checked)
            {
                return;
            }
            global::INDNC.Properties.Settings.Default.line3 = CheckState.Checked;
            global::INDNC.Properties.Settings.Default.line2 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line1 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line4 = CheckState.Unchecked;
            this.toolStripMenuItem1.CheckState = global::INDNC.Properties.Settings.Default.line1;
            this.toolStripMenuItem2.CheckState = global::INDNC.Properties.Settings.Default.line2;
            this.toolStripMenuItem3.CheckState = global::INDNC.Properties.Settings.Default.line3;
            this.toolStripMenuItem4.CheckState = global::INDNC.Properties.Settings.Default.line4;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem4.CheckState == CheckState.Checked)
            {
                return;
            }
            global::INDNC.Properties.Settings.Default.line4 = CheckState.Checked;
            global::INDNC.Properties.Settings.Default.line2 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line3 = CheckState.Unchecked;
            global::INDNC.Properties.Settings.Default.line1 = CheckState.Unchecked;
            this.toolStripMenuItem1.CheckState = global::INDNC.Properties.Settings.Default.line1;
            this.toolStripMenuItem2.CheckState = global::INDNC.Properties.Settings.Default.line2;
            this.toolStripMenuItem3.CheckState = global::INDNC.Properties.Settings.Default.line3;
            this.toolStripMenuItem4.CheckState = global::INDNC.Properties.Settings.Default.line4;
        }

        private void mySQL数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedisParaSetting redisparasetting = new RedisParaSetting();
            redisparasetting.ShowDialog();
            RedisPara tmp = new RedisPara();
            tmp = redisparasetting.redisparaName;
            if (tmp.connectvalid)
            {
                if (tmp.RedisPassword == "")
                    tmp.RedisPassword = "null";
                if (tmp.RedisIP == "")
                    tmp.RedisIP = "null";
                if (tmp.RedisPort == "")
                    tmp.RedisPort = "null";

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
                }
                catch (IOException ex)
                {
                    MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                }
            }
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
