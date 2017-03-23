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
                    TextBox objText = (TextBox)this.groupBox1.Controls["textBox" + i.ToString()];
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
                case 4:
                    SelectUserManage();
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
                labelCNCCount.Text = "当前设备数目:" + mysqlcncset.Tables[mysqlpara.MySQLDatabaseCNCTable].Rows.Count;
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
                labelRobotCount.Text = "当前设备数目:" + mysqlrobotset.Tables[mysqlpara.MySQLDatabaseRobotTable].Rows.Count;
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

        private void SelectUserManage()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("操作工");
            comboBox1.Items.Add("管理员");
            comboBox1.SelectedIndex = 0;
            bool SuperUser = global::INDNC.Properties.Settings.Default.SuperUser;
            //管理员
            if (SuperUser)
            {
                groupBox_UserManerge.Visible = true;
            }
            //操作工
            else
            {
                label_CurrentUsername.Text = "操作工";
                groupBox_UserManerge.Visible = false;
                label_UserPasswor1.Visible = false;
                textBox_UserPassword1.Visible = false;
                label_UserPasswor2.Visible = false;
                textBox_UserPassword2.Visible = false;
                label_Tisp.Visible = false;
                button_UserOnOrOff.Visible = false;
                button_ChangeUserPassword.Visible = false;
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool SuperUser = global::INDNC.Properties.Settings.Default.SuperUser;
            if (comboBox1.SelectedIndex == 0)
            {
                if (SuperUser)
                {
                    button_UserOnOrOff.Visible = false;
                    button_UserOnOrOff.Text = "注销";
                    button_ChangeUserPassword.Visible = false;
                    button_ChangeUserPassword.Text = "修改密码";
                    MessageBox.Show("请先注销管理员账户！");
                }
                else
                {
                    label_CurrentUsername.Text = "操作工";
                    groupBox_UserManerge.Visible = false;
                    label_UserPasswor1.Visible = false;
                    textBox_UserPassword1.Visible = false;
                    label_UserPasswor2.Visible = false;
                    textBox_UserPassword2.Visible = false;
                    label_Tisp.Visible = false;
                    button_UserOnOrOff.Visible = false;
                    button_ChangeUserPassword.Visible = false;
                }

            }
            else if (comboBox1.SelectedIndex == 1)
            {
                if (SuperUser)
                {
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "注销";
                    button_ChangeUserPassword.Visible = true;
                    button_ChangeUserPassword.Text = "修改密码";
                    groupBox_UserManerge.Visible = true;
                    label_CurrentUsername.Text = "管理员";
                }
                else
                {
                    label_UserPasswor1.Visible = true;
                    textBox_UserPassword1.Visible = true;
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "登录";
                }
            }
        }

        private void button_UserOnOrOff_Click(object sender, EventArgs e)
        {
            bool SuperUser = global::INDNC.Properties.Settings.Default.SuperUser;
            if (comboBox1.SelectedIndex == 0)
            {

            }
            else if (comboBox1.SelectedIndex == 1)
            {

            }
        }
    }
}
