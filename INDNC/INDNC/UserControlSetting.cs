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
using System.Timers;

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
        System.Timers.Timer t;  //timer

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

            /*
            global::INDNC.Properties.Settings.Default.UserCount = 0;
            global::INDNC.Properties.Settings.Default.User1 = "";
            global::INDNC.Properties.Settings.Default.User2 = "";
            global::INDNC.Properties.Settings.Default.User1PW = "";
            global::INDNC.Properties.Settings.Default.User2PW = "";
            Properties.Settings.Default.Save();*/

            //初始化CNC Robot panel位置
            dataGridViewCNC.Location = new Point(3, 3);
            panel6.Location = new Point(3, 451);
            dataGridViewCNC.Height = 447;
            dataGridViewCNC.Width = 945;
            panel6.Height = 45;
            panel6.Width = 945;
            dataGridViewCNC.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            panel6.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            dataGridViewRobot.Location = new Point(3, 3);
            panel5.Location = new Point(3, 451);
            dataGridViewRobot.Height = 447;
            dataGridViewRobot.Width = 945;
            panel5.Height = 45;
            panel5.Width = 945;
            dataGridViewRobot.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            panel5.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;


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
                MessageBox.Show("服务器参数设置完毕", "提示");
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
            var UserCount = global::INDNC.Properties.Settings.Default.UserCount;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("操作工");
            comboBox1.Items.Add("管理员");
            switch (UserCount)
            {
                case 5:
                    comboBox1.Items.Add(Properties.Settings.Default.User1);
                    comboBox1.Items.Add(Properties.Settings.Default.User2);
                    comboBox1.Items.Add(Properties.Settings.Default.User3);
                    comboBox1.Items.Add(Properties.Settings.Default.User4);
                    comboBox1.Items.Add(Properties.Settings.Default.User5);
                    break;
                case 4:
                    comboBox1.Items.Add(Properties.Settings.Default.User1);
                    comboBox1.Items.Add(Properties.Settings.Default.User2);
                    comboBox1.Items.Add(Properties.Settings.Default.User3);
                    comboBox1.Items.Add(Properties.Settings.Default.User4);
                    break;
                case 3:
                    comboBox1.Items.Add(Properties.Settings.Default.User1);
                    comboBox1.Items.Add(Properties.Settings.Default.User2);
                    comboBox1.Items.Add(Properties.Settings.Default.User3);
                    break;
                case 2:
                    comboBox1.Items.Add(Properties.Settings.Default.User1);
                    comboBox1.Items.Add(Properties.Settings.Default.User2);
                    break;
                case 1:
                    comboBox1.Items.Add(Properties.Settings.Default.User1);
                    break;
                default:
                    break;
            }
            short UserType = global::INDNC.Properties.Settings.Default.UserType;
            comboBox1.SelectedIndex = UserType;
            //管理员
            if (UserType==1)
            {
                label_CurrentUsername.Text = "管理员";
                groupBox_UserManerge.Visible = true;
                button_UserOnOrOff.Visible = true;
                button_ChangeUserPassword.Visible = true;
                button_UserOnOrOff.Text = "注销";
                button_ChangeUserPassword.Text = "修改密码";
                label_UserPasswor1.Visible = false;
                textBox_UserPassword1.Visible = false;
            }
            //操作工
            else if(UserType == 0)
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
            else
            {
                label_CurrentUsername.Text = comboBox1.Text;
                groupBox_UserManerge.Visible = false;
                button_UserOnOrOff.Visible = true;
                button_ChangeUserPassword.Visible = true;
                button_UserOnOrOff.Text = "注销";
                button_ChangeUserPassword.Text = "修改密码";
                label_UserPasswor1.Visible = false;
                textBox_UserPassword1.Visible = false;
            }
        }

        private void buttonlinesettingsave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save(); // 存储上一次成功连接的IP地址和端口号
            workshopno = textBoxworkshop.Text;
            lineno = textBoxline.Text;
            MessageBox.Show("产线参数设置完毕", "提示");
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
            MessageBox.Show("CNC设备数据库名设置完毕", "提示");
            if (btnCNCSettingClick != null)
                btnCNCSettingClick(this, e);
        }

        private void buttonRobotSetting_Click(object sender, EventArgs e)
        {
            mysqlpara.MySQLDatabaseRobotTable = textBox6.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Robot设备数据库名设置完毕", "提示");
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
            short UserType = global::INDNC.Properties.Settings.Default.UserType;
            if (comboBox1.SelectedIndex == 0)
            {
                if (UserType==0) //操作工
                {
                    groupBox_UserManerge.Visible = false;
                    label_UserPasswor1.Visible = false;
                    textBox_UserPassword1.Visible = false;
                    label_UserPasswor2.Visible = false;
                    textBox_UserPassword2.Visible = false;
                    label_Tisp.Visible = false;
                    button_UserOnOrOff.Visible = false;
                    button_ChangeUserPassword.Visible = false;
                }
                else if (UserType == 1) //管理员
                {
                    button_UserOnOrOff.Visible = false;
                    button_ChangeUserPassword.Visible = false;
                    label_Tisp.Text = "请先注销管理员账户！";
                    label_Tisp.Visible = true;
                    //label_Tisp两秒后不可见
                    t = new System.Timers.Timer(5000);
                    t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                    t.AutoReset = false;
                    t.Start();
                }
                else  //其它用户
                {
                    button_UserOnOrOff.Visible = false;
                    button_ChangeUserPassword.Visible = false;
                    label_Tisp.Text = "请先注销当前账户！";
                    label_Tisp.Visible = true;
                    //label_Tisp两秒后不可见
                    t = new System.Timers.Timer(5000);
                    t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                    t.AutoReset = false;
                    t.Start();
                }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                if (UserType == 0)
                {
                    label_UserPasswor1.Visible = true;
                    textBox_UserPassword1.Visible = true;
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "登录";
                }
                else if (UserType == 1)
                {
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "注销";
                    button_ChangeUserPassword.Visible = true;
                    button_ChangeUserPassword.Text = "修改密码";
                    groupBox_UserManerge.Visible = true;
                }
                else
                {
                    label_UserPasswor1.Visible = true;
                    textBox_UserPassword1.Visible = true;
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "登录";
                }
            }
            else
            {
                int index = comboBox1.SelectedIndex;
                if (UserType == 0)
                {
                    label_UserPasswor1.Visible = true;
                    textBox_UserPassword1.Visible = true;
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "登录";
                }
                else if (UserType == 1)
                {
                    label_UserPasswor1.Visible = true;
                    textBox_UserPassword1.Visible = true;
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "登录";
                }
                else
                {
                    if (index != UserType)
                    {
                        label_UserPasswor1.Visible = true;
                        textBox_UserPassword1.Visible = true;
                        button_UserOnOrOff.Visible = true;
                        button_UserOnOrOff.Text = "登录";
                    }
                    if (index == UserType)
                    {
                        button_UserOnOrOff.Visible = true;
                        button_UserOnOrOff.Text = "注销";
                        button_ChangeUserPassword.Visible = true;
                        button_ChangeUserPassword.Text = "修改密码";
                        textBox_UserPassword1.Visible = false;
                        label_UserPasswor1.Visible = false;
                    }
                }
                
            }
        }

        private void button_UserOnOrOff_Click(object sender, EventArgs e)
        {
            short UserType = global::INDNC.Properties.Settings.Default.UserType;
            if (comboBox1.SelectedIndex == 0)
            {

            }
            else if (comboBox1.SelectedIndex == 1) //管理员登录
            {
                if (UserType == 0 || UserType>1)
                {
                    if(textBox_UserPassword1.Text== global::INDNC.Properties.Settings.Default.SuperUserPassword)
                    {
                        label_Tisp.Visible = true;
                        label_Tisp.Text = "管理员:登录成功!";
                        //label_Tisp两秒后不可见
                        t = new System.Timers.Timer(5000);   
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);  
                        t.AutoReset = false;
                        t.Start();   
                        label_CurrentUsername.Text = "管理员";
                        global::INDNC.Properties.Settings.Default.UserType = 1;
                        textBox_UserPassword1.Visible = false;
                        label_UserPasswor1.Visible = false;
                        button_UserOnOrOff.Visible = true;
                        button_ChangeUserPassword.Visible = true;
                        button_UserOnOrOff.Text = "注销";
                        button_ChangeUserPassword.Text = "修改密码";
                        groupBox_UserManerge.Visible = true;
                        comboBox2.Visible = false;
                        textBox_ChangeUserName.Visible = true;
                        radioButton_AddUser.Checked = true;
                        radioButton_DeleUser.Checked = false;
                        textBox_UserPassword1.Text = "";
                    }
                    else
                    {
                        label_Tisp.Visible = true;
                        label_Tisp.Text = "管理员:密码错误，登录失败!";
                        //label_Tisp两秒后不可见
                        t = new System.Timers.Timer(5000);
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                        t.AutoReset = false;
                        textBox_UserPassword1.Text = "";
                        t.Start();
                    }
                }
                else if (UserType == 1) //管理员注销
                {
                    if (label_UserPasswor2.Visible == false)
                    {
                        label_Tisp.Visible = true;
                        label_Tisp.Text = "管理员:注销成功!";
                        //label_Tisp两秒后不可见
                        t = new System.Timers.Timer(5000);
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                        t.AutoReset = false;
                        t.Start();
                        label_CurrentUsername.Text = "操作者";
                        global::INDNC.Properties.Settings.Default.UserType = 0;
                        button_ChangeUserPassword.Visible = false;
                        groupBox_UserManerge.Visible = false;
                        label_UserName.Visible = true;
                        label_UserPasswor1.Visible = true;
                        textBox_UserPassword1.Visible = true;
                        button_UserOnOrOff.Visible = true;
                        button_UserOnOrOff.Text = "登录";
                        comboBox1.Visible = true;

                    }
                    else
                    {
                        textBox_UserPassword1.Visible = false;
                        label_UserPasswor1.Visible = false;
                        button_UserOnOrOff.Visible = true;
                        button_ChangeUserPassword.Visible = true;
                        button_UserOnOrOff.Text = "注销";
                        button_ChangeUserPassword.Text = "修改密码";
                        groupBox_UserManerge.Visible = true;
                        label_UserPasswor2.Visible = false;
                        textBox_UserPassword2.Visible = false;
                        comboBox1.Visible = true;
                        label_UserName.Visible = true;
                        textBox_UserPassword1.Text = "";
                        textBox_UserPassword2.Text = "";
                    }
                }
                else 
                {
                    
                }
            }
            else //其它用户登录
            {
                if (label_UserPasswor2.Visible == true)
                {
                    textBox_UserPassword1.Visible = false;
                    label_UserPasswor1.Visible = false;
                    button_UserOnOrOff.Visible = true;
                    button_ChangeUserPassword.Visible = true;
                    button_UserOnOrOff.Text = "注销";
                    button_ChangeUserPassword.Text = "修改密码";
                    groupBox_UserManerge.Visible = false;
                    label_UserPasswor2.Visible = false;
                    textBox_UserPassword2.Visible = false;
                    comboBox1.Visible = true;
                    label_UserName.Visible = true;
                    textBox_UserPassword1.Text = "";
                    textBox_UserPassword2.Text = "";
                }
                else
                {
                    if (UserType >= 2)
                    {
                        label_Tisp.Visible = true;
                        label_Tisp.Text = label_CurrentUsername.Text + ":注销成功!";
                        //label_Tisp两秒后不可见
                        t = new System.Timers.Timer(5000);
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                        t.AutoReset = false;
                        t.Start();
                        label_CurrentUsername.Text = "操作者";
                        global::INDNC.Properties.Settings.Default.UserType = 0;
                        button_ChangeUserPassword.Visible = false;
                        groupBox_UserManerge.Visible = false;
                        label_UserName.Visible = true;
                        label_UserPasswor1.Visible = true;
                        textBox_UserPassword1.Visible = true;
                        button_UserOnOrOff.Visible = true;
                        button_UserOnOrOff.Text = "登录";
                        comboBox1.Visible = true;

                    }
                    else
                    {
                        string name = comboBox1.Text;
                        string pw = null;
                        short index = 0;
                        if (UserType == 0 || UserType == 1)
                        {
                            if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User1)
                            {
                                pw = global::INDNC.Properties.Settings.Default.User1PW;
                                index = 2;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User2)
                            {
                                pw = global::INDNC.Properties.Settings.Default.User2PW;
                                index = 3;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User3)
                            {
                                pw = global::INDNC.Properties.Settings.Default.User3PW;
                                index = 4;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User4)
                            {
                                pw = global::INDNC.Properties.Settings.Default.User4PW;
                                index = 5;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User5)
                            {
                                pw = global::INDNC.Properties.Settings.Default.User5PW;
                                index = 6;
                            }

                            if (textBox_UserPassword1.Text == pw)
                            {
                                label_Tisp.Visible = true;
                                label_Tisp.Text = name + ":登录成功!";
                                //label_Tisp两秒后不可见
                                t = new System.Timers.Timer(5000);
                                t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                                t.AutoReset = false;
                                t.Start();
                                label_CurrentUsername.Text = name;
                                global::INDNC.Properties.Settings.Default.UserType = index;
                                textBox_UserPassword1.Visible = false;
                                label_UserPasswor1.Visible = false;
                                button_UserOnOrOff.Visible = true;
                                button_ChangeUserPassword.Visible = true;
                                button_UserOnOrOff.Text = "注销";
                                button_ChangeUserPassword.Text = "修改密码";
                                groupBox_UserManerge.Visible = false;
                                comboBox2.Visible = false;
                                textBox_UserPassword1.Text = "";
                            }
                            else
                            {
                                label_Tisp.Visible = true;
                                label_Tisp.Text = name + ":密码错误，登录失败!";
                                //label_Tisp两秒后不可见
                                t = new System.Timers.Timer(5000);
                                t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                                t.AutoReset = false;
                                textBox_UserPassword1.Text = "";
                                t.Start();
                            }
                        }
                    }
                }               
            }
        }

        private void label_ChangeUserTips_disappear(Object source, ElapsedEventArgs e)
        {
            label_ChangeUserTips.Visible = false;
        }

        private void label_Tisp_disappear(Object source, ElapsedEventArgs e)
        {
            label_Tisp.Visible = false;
        }

        private void button_ChangeUserPassword_Click(object sender, EventArgs e)
        {
            short UserType = global::INDNC.Properties.Settings.Default.UserType;
            if (UserType == 0)
            {
                
            }
            else if (UserType == 1)
            {

                if (label_UserPasswor2.Visible == false)
                {
                    label_UserPasswor1.Visible = true;
                    textBox_UserPassword1.Visible = true;
                    label_UserPasswor2.Visible = true;
                    textBox_UserPassword2.Visible = true;
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "取消";
                    button_ChangeUserPassword.Visible = true;
                    button_ChangeUserPassword.Text = "确定";
                    label_UserName.Visible = false;
                    comboBox1.Visible = false;
                    label_Tisp.Visible = false;
                }
                else
                {
                    if (textBox_UserPassword1.Text == textBox_UserPassword2.Text)
                    {
                        if (textBox_UserPassword1.Text.Length < 6)
                        {
                            label_Tisp.Visible = true;
                            label_Tisp.Text = "管理员:修改密码失败,密码长度不能小于6位！";
                            t = new System.Timers.Timer(5000);
                            t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                            t.AutoReset = false;
                            t.Start();
                            textBox_UserPassword1.Text = "";
                            textBox_UserPassword2.Text = "";
                        }
                        else
                        {
                            global::INDNC.Properties.Settings.Default.SuperUserPassword = textBox_UserPassword1.Text;
                            Properties.Settings.Default.Save();
                            label_Tisp.Visible = true;
                            label_Tisp.Text = "管理员:修改密码成功!";
                            //label_Tisp两秒后不可见
                            t = new System.Timers.Timer(5000);
                            t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                            t.AutoReset = false;
                            t.Start();

                            textBox_UserPassword1.Visible = false;
                            label_UserPasswor1.Visible = false;
                            button_UserOnOrOff.Visible = true;
                            button_ChangeUserPassword.Visible = true;
                            button_UserOnOrOff.Text = "注销";
                            button_ChangeUserPassword.Text = "修改密码";
                            groupBox_UserManerge.Visible = true;
                            comboBox1.Visible = false;
                            textBox_UserPassword2.Visible = false;
                            label_UserPasswor2.Visible = false;
                            textBox_UserPassword1.Text = "";
                            textBox_UserPassword2.Text = "";
                        }
                        
                    }
                    else
                    {
                        label_Tisp.Visible = true;
                        label_Tisp.Text = "管理员:修改密码失败,两次输入的密码不一致！";
                        t = new System.Timers.Timer(5000);
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                        t.AutoReset = false;
                        t.Start();
                        textBox_UserPassword1.Text = "";
                        textBox_UserPassword2.Text = "";
                    }
                }
                
            }
            else //其它用户更改秘密
            {
                if (label_UserPasswor2.Visible == false)
                {
                    label_UserPasswor1.Visible = true;
                    textBox_UserPassword1.Visible = true;
                    label_UserPasswor2.Visible = true;
                    textBox_UserPassword2.Visible = true;
                    button_UserOnOrOff.Visible = true;
                    button_UserOnOrOff.Text = "取消";
                    button_ChangeUserPassword.Visible = true;
                    button_ChangeUserPassword.Text = "确定";
                    label_UserName.Visible = false;
                    comboBox1.Visible = false;
                    label_Tisp.Visible = false;
                }
                else
                {
                    string name = comboBox1.Text;
                    if (textBox_UserPassword1.Text == textBox_UserPassword2.Text)
                    {
                        if (textBox_UserPassword1.Text.Length < 6)
                        {
                            label_Tisp.Visible = true;
                            label_Tisp.Text = name+":修改密码失败,密码长度不能小于6位！";
                            t = new System.Timers.Timer(5000);
                            t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                            t.AutoReset = false;
                            t.Start();
                            textBox_UserPassword1.Text = "";
                            textBox_UserPassword2.Text = "";
                        }
                        else
                        {
                            if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User1)
                            {
                                global::INDNC.Properties.Settings.Default.User1PW = textBox_UserPassword1.Text;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User2)
                            {
                                global::INDNC.Properties.Settings.Default.User2PW = textBox_UserPassword1.Text;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User3)
                            {
                                global::INDNC.Properties.Settings.Default.User3PW = textBox_UserPassword1.Text;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User4)
                            {
                                global::INDNC.Properties.Settings.Default.User4PW = textBox_UserPassword1.Text;
                            }
                            else if (comboBox1.Text == global::INDNC.Properties.Settings.Default.User5)
                            {
                                global::INDNC.Properties.Settings.Default.User5PW = textBox_UserPassword1.Text;
                            }

                            Properties.Settings.Default.Save();
                            label_Tisp.Visible = true;
                            label_Tisp.Text = name+ ":修改密码成功!";
                            //label_Tisp两秒后不可见
                            t = new System.Timers.Timer(5000);
                            t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                            t.AutoReset = false;
                            t.Start();

                            textBox_UserPassword1.Visible = false;
                            label_UserPasswor1.Visible = false;
                            button_UserOnOrOff.Visible = true;
                            button_ChangeUserPassword.Visible = true;
                            button_UserOnOrOff.Text = "注销";
                            button_ChangeUserPassword.Text = "修改密码";
                            groupBox_UserManerge.Visible = false;
                            comboBox1.Visible = false;
                            textBox_UserPassword2.Visible = false;
                            label_UserPasswor2.Visible = false;
                            textBox_UserPassword1.Text = "";
                            textBox_UserPassword2.Text = "";
                        }

                    }
                    else
                    {
                        label_Tisp.Visible = true;
                        label_Tisp.Text = name+":修改密码失败,两次输入的密码不一致！";
                        t = new System.Timers.Timer(5000);
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_Tisp_disappear);
                        t.AutoReset = false;
                        t.Start();
                        textBox_UserPassword1.Text = "";
                        textBox_UserPassword2.Text = "";
                    }
                }
            }
        }

        private void radioButton_AddUser_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_AddUser.Checked){
                comboBox2.Visible = false;
                textBox_ChangeUserName.Visible = true;
                radioButton_DeleUser.Checked = false;
                label_ChangeUserPasswor1.Visible = true;
                label_ChangeUserPasswor2.Visible = true;
                textBox_ChangeUserPasswor1.Visible = true;
                textBox_ChangeUserPasswor2.Visible = true;
                button_EditUserOK.Text = "添加";
            }
            else
            {
                comboBox2.Visible = true;
                textBox_ChangeUserName.Visible = false;
                radioButton_DeleUser.Checked = true;
                label_ChangeUserPasswor1.Visible = false;
                label_ChangeUserPasswor2.Visible = false;
                textBox_ChangeUserPasswor1.Visible = false;
                textBox_ChangeUserPasswor2.Visible = false;
                button_EditUserOK.Text = "删除";

                var UserCount = global::INDNC.Properties.Settings.Default.UserCount;
                comboBox2.Items.Clear();
                switch (UserCount)
                {
                    case 5:
                        comboBox2.Items.Add(Properties.Settings.Default.User1);
                        comboBox2.Items.Add(Properties.Settings.Default.User2);
                        comboBox2.Items.Add(Properties.Settings.Default.User3);
                        comboBox2.Items.Add(Properties.Settings.Default.User4);
                        comboBox2.Items.Add(Properties.Settings.Default.User5);
                        break;
                    case 4:
                        comboBox2.Items.Add(Properties.Settings.Default.User1);
                        comboBox2.Items.Add(Properties.Settings.Default.User2);
                        comboBox2.Items.Add(Properties.Settings.Default.User3);
                        comboBox2.Items.Add(Properties.Settings.Default.User4);
                        break;
                    case 3:
                        comboBox2.Items.Add(Properties.Settings.Default.User1);
                        comboBox2.Items.Add(Properties.Settings.Default.User2);
                        comboBox2.Items.Add(Properties.Settings.Default.User3);
                        break;
                    case 2:
                        comboBox2.Items.Add(Properties.Settings.Default.User1);
                        comboBox2.Items.Add(Properties.Settings.Default.User2);
                        break;
                    case 1:
                        comboBox2.Items.Add(Properties.Settings.Default.User1);
                        break;
                    default:
                        break;
                }
                if(comboBox2.Items.Count>0)
                    comboBox2.SelectedIndex = 0;
            }
        }

        private void button_EditUserOK_Click(object sender, EventArgs e)
        {
            //增加用户
            if (radioButton_AddUser.Checked)
            {
                if(textBox_ChangeUserPasswor1.Text== textBox_ChangeUserPasswor2.Text)
                {
                    if (textBox_ChangeUserPasswor1.Text.Length < 6)
                    {
                        label_ChangeUserTips.Visible = true;
                        label_ChangeUserTips.Text = "添加用户失败:密码长度不能小于6位！";
                        t = new System.Timers.Timer(5000);
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_ChangeUserTips_disappear);
                        t.AutoReset = false;
                        t.Start();
                        textBox_ChangeUserPasswor1.Text = "";
                        textBox_ChangeUserPasswor2.Text = "";
                    }
                    else
                    {
                        short UserType = global::INDNC.Properties.Settings.Default.UserType;
                        ++global::INDNC.Properties.Settings.Default.UserCount;
                        short UserCount = global::INDNC.Properties.Settings.Default.UserCount;
                        if (UserCount > 5)
                        {
                            label_ChangeUserTips.Visible = true;
                            label_ChangeUserTips.Text = "添加用户失败:用户数目不能超过五个！";
                            t = new System.Timers.Timer(5000);
                            t.Elapsed += new System.Timers.ElapsedEventHandler(label_ChangeUserTips_disappear);
                            t.AutoReset = false;
                            t.Start();
                            textBox_ChangeUserPasswor1.Text = "";
                            textBox_ChangeUserPasswor2.Text = "";
                            return;
                        }
                        switch (UserCount)
                        {
                            case 1:
                                Properties.Settings.Default.User1 = textBox_ChangeUserName.Text;
                                Properties.Settings.Default.User1PW = textBox_ChangeUserPasswor1.Text;
                                comboBox1.Items.Add(Properties.Settings.Default.User1);
                                comboBox2.Items.Add(Properties.Settings.Default.User1);
                                break;
                            case 2:
                                Properties.Settings.Default.User2 = textBox_ChangeUserName.Text;
                                Properties.Settings.Default.User2PW = textBox_ChangeUserPasswor1.Text;
                                comboBox1.Items.Add(Properties.Settings.Default.User2);
                                comboBox2.Items.Add(Properties.Settings.Default.User2);
                                break;
                            case 3:
                                Properties.Settings.Default.User3 = textBox_ChangeUserName.Text;
                                Properties.Settings.Default.User3PW = textBox_ChangeUserPasswor1.Text;
                                comboBox1.Items.Add(Properties.Settings.Default.User3);
                                comboBox2.Items.Add(Properties.Settings.Default.User3);
                                break;
                            case 4:
                                Properties.Settings.Default.User4 = textBox_ChangeUserName.Text;
                                Properties.Settings.Default.User4PW = textBox_ChangeUserPasswor1.Text;
                                comboBox1.Items.Add(Properties.Settings.Default.User4);
                                comboBox2.Items.Add(Properties.Settings.Default.User4);
                                break;
                            case 5:
                                Properties.Settings.Default.User5 = textBox_ChangeUserName.Text;
                                Properties.Settings.Default.User5PW = textBox_ChangeUserPasswor1.Text;
                                comboBox1.Items.Add(Properties.Settings.Default.User5);
                                comboBox2.Items.Add(Properties.Settings.Default.User5);
                                break;
                            default:
                                break;
                        }
                        label_ChangeUserTips.Visible = true;
                        label_ChangeUserTips.Text = "添加用户"+ textBox_ChangeUserName.Text+"成功！";
                        t = new System.Timers.Timer(5000);
                        t.Elapsed += new System.Timers.ElapsedEventHandler(label_ChangeUserTips_disappear);
                        t.AutoReset = false;
                        t.Start();
                        textBox_ChangeUserName.Text = "";
                        textBox_ChangeUserPasswor1.Text = "";
                        textBox_ChangeUserPasswor2.Text = "";
                        Properties.Settings.Default.Save();
                    }
                }
                else
                {
                    label_ChangeUserTips.Visible = true;
                    label_ChangeUserTips.Text = "添加用户失败:两次输入的密码不一致！";
                    t = new System.Timers.Timer(5000);
                    t.Elapsed += new System.Timers.ElapsedEventHandler(label_ChangeUserTips_disappear);
                    t.AutoReset = false;
                    t.Start();
                    textBox_ChangeUserPasswor1.Text = "";
                    textBox_ChangeUserPasswor2.Text = "";
                }
            }
            else //删除用户
            {
                string name = comboBox2.Text;
                if(comboBox2.Text== global::INDNC.Properties.Settings.Default.User1)
                {
                    global::INDNC.Properties.Settings.Default.User1= "";
                    global::INDNC.Properties.Settings.Default.User1PW = "";  
                }
                else if(comboBox2.Text == global::INDNC.Properties.Settings.Default.User2)
                {
                    global::INDNC.Properties.Settings.Default.User2 = "";
                    global::INDNC.Properties.Settings.Default.User2PW = "";
                }
                else if (comboBox2.Text == global::INDNC.Properties.Settings.Default.User3)
                {
                    global::INDNC.Properties.Settings.Default.User3 = "";
                    global::INDNC.Properties.Settings.Default.User3PW = "";
                }
                else if (comboBox2.Text == global::INDNC.Properties.Settings.Default.User4)
                {
                    global::INDNC.Properties.Settings.Default.User4 = "";
                    global::INDNC.Properties.Settings.Default.User4PW = "";
                }
                else if (comboBox2.Text == global::INDNC.Properties.Settings.Default.User5)
                {
                    global::INDNC.Properties.Settings.Default.User5 = "";
                }
                comboBox1.Items.Remove(name);
                comboBox2.Items.Remove(name);
                if (comboBox2.Items.Count > 0)
                    comboBox2.SelectedIndex = 0;
                else
                    comboBox2.Text = "";
                label_ChangeUserTips.Visible = true;
                label_ChangeUserTips.Text = "删除用户" + name + "成功！";
                t = new System.Timers.Timer(5000);
                t.Elapsed += new System.Timers.ElapsedEventHandler(label_ChangeUserTips_disappear);
                t.AutoReset = false;
                t.Start();
                comboBox1.Update();
                comboBox2.Update();
                --global::INDNC.Properties.Settings.Default.UserCount;
                Properties.Settings.Default.Save();
            }
        }
    }
}
