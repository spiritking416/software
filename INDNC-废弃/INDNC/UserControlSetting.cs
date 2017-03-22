using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceStack.Redis;
using System.IO;

namespace INDNC
{
    public delegate void btnOkClickEventHander(object sender, EventArgs e); //委托
    public partial class UserControlSetting : UserControl
    {
        private RedisPara serverpara = new RedisPara();
        private RedisManager redismanager = new RedisManager();
        private RedisPara redispara = new RedisPara();
        private UInt16 WorkShopNo;
        private UInt16 LineNo;
        private RedisManager localredismanager = new RedisManager();
        public event btnOkClickEventHander btnServerSettingClick; //事件
        public event btnOkClickEventHander btnLineParaSettingClick; //事件

        public RedisPara serverparaName
        {
            get { return serverpara; }
        }

        public RedisPara redisparaName
        {
            get { return redispara; }
        }

        public UInt16 WorkShopNoName
        {
            get { return WorkShopNo; }
        }

        public UInt16 LineNoName
        {
            get { return LineNo; }
        }
        public UserControlSetting()
        {
            InitializeComponent();
            //this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom; //size动态变化  
            //tabControlSetting.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom; //size动态变化    
        }

        private void buttonServerConnect_Click(object sender, EventArgs e)
        {
            try
            {
                //云服务器连接ip,por,password设置
                for (int i = 1; i <= 4; ++i)
                {
                    //检验输入是否为数字，数字是否介于0~255之间
                    int tmp = -1;
                    TextBox objText = (TextBox)this.panel1.Controls["textBox" + i.ToString()];
                    if (int.TryParse(objText.Text, out tmp) != true)
                    {
                        throw new Exception("错误:云服务器IP地址输入错误，请重新输入！");
                    }
                    else if (!(tmp >= 0 && tmp <= 255))
                    {
                        throw new Exception("错误:云服务器IP地址输入错误，请重新输入！");
                    }

                }
                String IP = textBox1.Text + '.' + textBox2.Text + '.' + textBox3.Text + '.' + textBox4.Text;
                int port = 0;
                if (int.TryParse(textBox5.Text, out port) != true)
                {
                    throw new Exception("错误:云服务器端口号输入错误，请重新输入！");
                }
                serverpara.RedisIP = IP;
                serverpara.RedisPort = textBox5.Text;
                serverpara.RedisPassword = textBox6.Text;
                serverpara.connectvalid = false;

                //host主机参数  格式“password@ip:port”
                string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort };
                //从连接池获得只读连接客户端
                long initialDB = 0;
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));
                if (Client == null || !Client.Ping())
                {
                    throw new Exception("连接云服务器失败!");
                }
                //云服务器连接成功
                serverpara.connectvalid = true;

                //本地服务器连接ip,por,password设置
                for (int i = 7; i <= 10; ++i)
                {
                    //检验输入是否为数字，数字是否介于0~255之间
                    int tmpint = -1;
                    TextBox objText = (TextBox)this.panel2.Controls["textBox" + i.ToString()];
                    if (int.TryParse(objText.Text, out tmpint) != true)
                    {
                        throw new Exception("错误:云服务器IP地址输入错误，请重新输入！");
                    }
                    else if (!(tmpint >= 0 && tmpint <= 255))
                    {
                        throw new Exception("错误:云服务器IP地址输入错误，请重新输入！");
                    }

                }
                IP = textBox7.Text + '.' + textBox8.Text + '.' + textBox9.Text + '.' + textBox10.Text;
                port = 0;
                if (int.TryParse(textBox11.Text, out port) != true)
                {
                    throw new Exception("错误:云服务器端口号输入错误，请重新输入！");
                }

                redispara.RedisIP = IP;
                redispara.RedisPort = textBox11.Text;
                redispara.RedisPassword = textBox12.Text;
                redispara.connectvalid = false;

                //localhost主机参数  格式“password@ip:port”
                string[] localhost = { redispara.RedisPassword + '@' + redispara.RedisIP + ':' + redispara.RedisPort};
                //从连接池获得只读连接客户端
                initialDB = 0;
                Client = (RedisClient)localredismanager.GetReadOnlyClient(ref (initialDB), ref (localhost));
                if (Client == null || !Client.Ping())
                {
                    throw new Exception("连接本地服务器失败!");
                }
                //本地服务器连接成功
                redispara.connectvalid = true;
                /*if (redispara.connectvalid)
                {
                    try
                    {
                        FileStream fs = new FileStream(@"../LocalRedisPara.conf", FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write("RedisIP=" + redispara.RedisIP + ";RedisPort=" + redispara.RedisPort + ";RedisPassword=" + redispara.RedisPassword);
                        //清空缓冲区
                        sw.Flush();
                        //关闭流
                        sw.Close();
                        fs.Close();
                    }
                    catch (IOException ex)
                    {
                        throw ex;
                    }
                }*/

                if (checkBox1.Checked)
                    Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号
                //传值给主窗体
                if (btnServerSettingClick != null)
                    btnServerSettingClick(this, e);
                MessageBox.Show("服务器参数设置完毕", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                redismanager.dispose();
            }
        }

        private void buttonlineparasave_Click(object sender, EventArgs e)
        {
            try
            {
                UInt16 tmp = 0;
                if (UInt16.TryParse(textBox13.Text, out tmp) != true)
                {
                    throw new Exception("错误:输入错误，请重新输入！");
                }
                WorkShopNo = tmp;
                if (UInt16.TryParse(textBox14.Text, out tmp) != true)
                {
                    throw new Exception("错误:输入错误，请重新输入！");
                }
                LineNo = tmp;
                Properties.Settings.Default.Save();
                MessageBox.Show("产线参数设置成功！", "提示");
                if (btnLineParaSettingClick != null)
                    btnLineParaSettingClick(this, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }

        }
    }
}
