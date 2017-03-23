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
        public event btnOkClickEventHander btnCNCSettingClick; //CNC设备设置委托
        public event btnOkClickEventHander btnRobotSettingClick; //Robot设备设置委托
        private MySqlConnection mysqlconnection; // mysql连接
        private MySqlDataAdapter mysqlcncadapter; //mysql数据适配器
        private MySqlDataAdapter mysqlrobotadapter; //mysql数据适配器
        private DataSet mysqlcncset; //数据集
        private DataSet mysqlrobotset; //数据集
        private string MyconnectionString;
        private string workshopno;
        private string lineno;

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
            panel4.Height = 50;
            panel4.Width = 200;
            panel4.Visible = true;
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
            MyconnectionString = "server=" + mysqlpara.MySQLServer + ";user id=" + mysqlpara.MySQLID + ";password=" + mysqlpara.MySQLIDPassword + "; database=" + mysqlpara.MySQLIDDatabase;
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
                case 3:
                    SelectRobot();
                    break;
                default:
                    
                    break;
            }
        }

        private void SelectCNC()
        {
            try
            {
                if (MyconnectionString==null || MyconnectionString == "")
                    throw new Exception("本地MySQL数据库参数错误，请重新设置！");
                if (mysqlconnection == null)
                    mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                if (!mysqlconnection.Ping())
                {
                    throw new Exception("无法连接本地MySQL服务器！");
                }
                string tmp = "select* from " + mysqlpara.MySQLDatabaseCNCTable;
                if(mysqlcncadapter==null)
                    mysqlcncadapter = new MySqlDataAdapter(tmp, mysqlconnection);
                if (mysqlcncset == null)
                    mysqlcncset = new DataSet();
                else
                    mysqlcncset.Clear();
                //填充和绑定数据
                mysqlcncadapter.Fill(mysqlcncset, mysqlpara.MySQLDatabaseCNCTable);
                dataGridViewCNC.DataSource = mysqlcncset;
                dataGridViewCNC.DataMember = mysqlpara.MySQLDatabaseCNCTable;
                labelCNCTable.Text = "当前数据库表名:" + mysqlpara.MySQLDatabaseCNCTable;
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
            finally
            {
                if(mysqlconnection!=null)
                    mysqlconnection.Close();  //关闭
            }   
        }

        private void SelectRobot()
        {
            try
            {
                if (MyconnectionString == null || MyconnectionString == "")
                    throw new Exception("本地MySQL数据库参数错误，请重新设置！");
                if (mysqlconnection == null)
                    mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                if (!mysqlconnection.Ping())
                {
                    throw new Exception("无法连接本地MySQL服务器！");
                }
                string tmp = "select* from " + mysqlpara.MySQLDatabaseRobotTable;
                mysqlrobotadapter = new MySqlDataAdapter(tmp, mysqlconnection);

                if (mysqlrobotset == null)
                    mysqlrobotset = new DataSet();
                else
                    mysqlrobotset.Clear();
                //填充和绑定数据
                mysqlrobotadapter.Fill(mysqlrobotset, mysqlpara.MySQLDatabaseRobotTable);
                dataGridViewRobot.DataSource = mysqlrobotset;
                dataGridViewRobot.DataMember = mysqlpara.MySQLDatabaseRobotTable;
                labelRobotTable.Text = "当前数据库表名:" + mysqlpara.MySQLDatabaseRobotTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
            finally
            {
                if (mysqlconnection != null)
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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsNumber(e.KeyChar)) || e.KeyChar == (char)8)
                e.Handled = false;
            else if (e.KeyChar == (char)13)
            {
                buttonServerConnect.Focus();
                buttonServerConnect_Click(sender, e);
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
                buttonServerConnect.Focus();
                buttonServerConnect_Click(sender, e);
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
                buttonServerConnect.Focus();
                buttonServerConnect_Click(sender, e);
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
                buttonServerConnect.Focus();
                buttonServerConnect_Click(sender, e);
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("请输入数字", "ERROR");
            }
        }

        private void textBoxRedisPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsNumber(e.KeyChar)) || e.KeyChar == (char)8)
                e.Handled = false;
            else if (e.KeyChar == (char)13)
            {
                buttonServerConnect.Focus();
                buttonServerConnect_Click(sender, e);
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("请输入数字", "ERROR");
            }
        }

        private void buttonCNCSetting_Click(object sender, EventArgs e)
        {
            mysqlpara.MySQLDatabaseCNCTable = textBox5.Text;
            Properties.Settings.Default.Save();
            if (btnCNCSettingClick != null)
                btnCNCSettingClick(this, e);
        }

        private void buttonRobotSetting_Click(object sender, EventArgs e)
        {
            mysqlpara.MySQLDatabaseRobotTable = textBox6.Text;
            Properties.Settings.Default.Save();
            if (btnCNCSettingClick != null)
                btnRobotSettingClick(this, e);
        }

        private void buttonCNCSave_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (MyconnectionString == null || MyconnectionString == "")
                    throw new Exception("本地MySQL数据库参数错误，请重新设置！");
                if (mysqlconnection == null)
                    mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                if (!mysqlconnection.Ping())
                {
                    throw new Exception("无法连接本地MySQL服务器！");
                }
                string tmp = "select* from " + mysqlpara.MySQLDatabaseCNCTable;
                if(mysqlcncadapter==null)
                    mysqlcncadapter = new MySqlDataAdapter(tmp, mysqlconnection);
                if (mysqlcncset == null)
                {
                    mysqlcncset = new DataSet();
                    mysqlcncadapter.Fill(mysqlcncset, mysqlpara.MySQLDatabaseCNCTable);
                    dataGridViewCNC.DataSource = mysqlcncset;
                    dataGridViewCNC.DataMember = mysqlpara.MySQLDatabaseCNCTable;
                }

                //刷新数据
                var mysqlcommamdbuilder = new MySqlCommandBuilder(mysqlcncadapter);
                mysqlcncadapter.Update(mysqlcncset, mysqlpara.MySQLDatabaseCNCTable);

                MessageBox.Show("保存数据成功！", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
            finally
            {
                if (mysqlconnection != null)
                    mysqlconnection.Close();  //关闭
            }
        }

        private void buttonRobotSave_Click(object sender, EventArgs e)
        {
            try
            {

                if (MyconnectionString == null || MyconnectionString == "")
                    throw new Exception("本地MySQL数据库参数错误，请重新设置！");
                if (mysqlconnection == null)
                    mysqlconnection = new MySqlConnection(MyconnectionString);
                mysqlconnection.Open();   //必须Open
                if (!mysqlconnection.Ping())
                {
                    throw new Exception("无法连接本地MySQL服务器！");
                }
                string tmp = "select* from " + mysqlpara.MySQLDatabaseRobotTable;
                if (mysqlrobotadapter == null)
                    mysqlrobotadapter = new MySqlDataAdapter(tmp, mysqlconnection);
                if (mysqlrobotset == null)
                {
                    mysqlrobotset = new DataSet();
                    mysqlrobotadapter.Fill(mysqlrobotset, mysqlpara.MySQLDatabaseRobotTable);
                    dataGridViewRobot.DataSource = mysqlrobotset;
                    dataGridViewRobot.DataMember = mysqlpara.MySQLDatabaseRobotTable;
                }

                //刷新数据
                var mysqlcommamdbuilder = new MySqlCommandBuilder(mysqlrobotadapter);
                mysqlrobotadapter.Update(mysqlrobotset, mysqlpara.MySQLDatabaseRobotTable);

                MessageBox.Show("保存数据成功！", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
            finally
            {
                if (mysqlconnection != null)
                    mysqlconnection.Close();  //关闭
            }
        }

        private void tabPageCNC_Click(object sender, EventArgs e)
        {

        }

        private void tabPageServerSetting_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBoxMysqlDB_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxMysqlPW_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxMysqlserver_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxMysqlID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxRedisPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxRedisPw_TextChanged(object sender, EventArgs e)
        {

        }

        private void label51_Click(object sender, EventArgs e)
        {

        }

        private void tabPageLineSetting_Click(object sender, EventArgs e)
        {

        }

        private void textBoxline_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxworkshop_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewCNC_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void labelCNCTable_Click(object sender, EventArgs e)
        {

        }

        private void tabPageRobotSetting_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void labelRobotTable_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewRobot_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
