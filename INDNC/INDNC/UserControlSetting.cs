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
using MySql.Data.MySqlClient;

namespace INDNC
{
    public delegate void btnOkClickEventHander(object sender, EventArgs e); //委托
    public partial class UserControlSetting : UserControl
    {
        private RedisPara serverpara;
        private RedisManager redismanager = new RedisManager();
        private MySQLPara mysqlpara = new MySQLPara();
        private RedisManager localredismanager = new RedisManager();
        public event btnOkClickEventHander btnServerSettingClick; //服务器设置委托
        public event btnOkClickEventHander btnLineSettingClick; //产线设置委托
        private MySqlConnection mysqlconnection; // mysql连接
        private MySqlDataAdapter mysqladapter; //mysql数据适配器
        DataSet mysqlset; //数据集
        string MyconnectionString;
        string workshopno;
        string lineno;

        public RedisPara serverparaName
        {
            get { return serverpara; } 
        }

        public MySQLPara mysqlparaName
        {
            get { return mysqlpara; }
        }
        public string workshopnoName
        {
            get { return workshopno; }
        }
        public string linenoName
        {
            get { return lineno; }
        }
        public UserControlSetting()
        {
            InitializeComponent();
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
                if (int.TryParse(textBoxRedisPort.Text, out port) != true)
                {
                    throw new Exception("错误:云服务器端口号输入错误，请重新输入！");
                }
                serverpara.RedisIP = IP;
                serverpara.RedisPort = textBoxRedisPort.Text;
                serverpara.RedisPassword = textBoxRedisPw.Text;
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

                //本地MySQL服务器连接设置
                mysqlpara.MySQLServer = textBoxMysqlserver.Text;
                mysqlpara.MySQLID = textBoxMysqlID.Text;
                mysqlpara.MySQLIDPassword = textBoxMysqlPW.Text;
                mysqlpara.MySQLIDDatabase = textBoxMysqlDB.Text;
                mysqlpara.connectvalid = false;

                MyconnectionString = "server="+ mysqlpara.MySQLServer+";user id="+ mysqlpara.MySQLID+";password="+mysqlpara.MySQLIDPassword+"; database="+ mysqlpara.MySQLIDDatabase;
                mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                if (Client == null || !mysqlconnection.Ping())
                {
                    throw new Exception("连接本地MySQL服务器失败!");
                }
                //本地服务器连接成功
                mysqlpara.connectvalid = true;

                if (checkBox1.Checked)
                    Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号
                //传值给主窗体
                if (btnServerSettingClick != null)
                    btnServerSettingClick(this, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                redismanager.dispose();
            }
            finally
            {
                if (mysqlconnection != null)
                    mysqlconnection.Close();
            }
        }

        private void tabControlSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlSetting.SelectedIndex)
            {
                case 0:
                    
                    break;
                case 1:
                    

                    break;
                case 2:
                    SelectCNC();
                    break;
                default:
                    
                    break;
            }
        }

        private void SelectCNC()
        {
            try
            {
                MyconnectionString = "server=localhost;user id=root;password=416520;database=cncdatabase";
                mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                MessageBox.Show(mysqlconnection.Ping().ToString());
                mysqladapter = new MySqlDataAdapter("select* from cnctable", mysqlconnection);
                mysqlset = new DataSet();
                //填充和绑定数据
                mysqladapter.Fill(mysqlset, "cnctable");
                dataGridViewCNC.DataSource = mysqlset.Tables["cnctable"];
            }
            catch
            {

            }
            finally
            {
                mysqlconnection.Close();  //关闭
            }
        }

        private void buttonlinesettingsave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号
            workshopno = textBoxworkshop.Text;
            lineno = textBoxline.Text;
            if (btnLineSettingClick != null)
                btnLineSettingClick(this, e);
        }
    }
}
