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

namespace INDNC
{
    public partial class AddOrDelSetting : Form
    {
        RedisPara redispara;

        public enum Operation
        {
            Add = 0,
            Delete = 1
        }

        public RedisPara redisparaName
        {
            set { redispara = value; }
        }

        public AddOrDelSetting()
        {
            InitializeComponent();
        }

        private void AddOrDelSetting_Load(object sender, EventArgs e)
        {
            string text = "";
            UInt16 linecount;
            try
            {
                if (UInt16.TryParse(text, out linecount) != true)
                {
                    throw new Exception("错误:生产线参数错误，请设置！");
                }
                for (int i = 1; i <= linecount; ++i)
                    comboBox1.Items.Add("生产线路" + i.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (setting(Operation.Add))
                MessageBox.Show("设备信息成功添加！", "提示");
            else
                MessageBox.Show("设备信息添加失败！", "提示");

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox)
                {
                    ctrl.Text = "";
                }
                else if (ctrl is ComboBox)
                    ctrl.Text = "";

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (setting(Operation.Delete))
                MessageBox.Show("设备信息成功删除！", "提示");
            else
                MessageBox.Show("设备信息删除失败！", "提示");
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox)
                {
                    ctrl.Text = "";
                }
                else if (ctrl is ComboBox)
                    ctrl.Text = "";
            }
        }


        public bool setting(Operation operation)
        {
            string textSN = textBox2.Text;
            string textNO = textBox1.Text;
            string textIP = textBox3.Text;
            string textPort = textBox4.Text;
            string textDB = textBox5.Text;

            try
            {
                if (comboBox1.SelectedItem == null)
                    throw new Exception("错误:未选择生产线路，请重新选择！");
                String texttmp = comboBox1.SelectedItem.ToString();
                texttmp = texttmp.Substring(4);
                UInt16 lineselect;   //生产线编号
                if (UInt16.TryParse(texttmp, out lineselect) != true)
                {
                    throw new Exception("错误:生产线路错误，请重新设置！");
                }

                //连接本地数据库
                if (redispara.connectvalid == false)
                    throw new Exception("错误:本地Redis数据库参数错误，请重新设置！");
                long initialDb = 0;
                string[] host = { redispara.RedisPassword + '@' + redispara.RedisIP + ':' + redispara.RedisPort };
                RedisManager redismanager = new RedisManager(ref (initialDb), ref (host));
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDb), ref (host));
                if (!Client.Ping())
                {
                    throw new Exception("错误:本地Redis数据库参数错误，请重新设置！");
                }

                //读取生产线N的设备信息 LineNO
                bool LineExist = false;
                string lineno = "Line" + lineselect.ToString();
                //生产线lineno是否存在
                if (Client.SCard(lineno) == 0)
                    LineExist = false;
                else
                    LineExist = true;

                //缺省DB为null
                if (textDB == "")
                    textDB = "null";

                if (operation == Operation.Add)
                {
                    //设备信息输入校正
                    if (textSN == "")
                        throw new Exception("错误:设备SN码未输入，请重新输入！");
                    else if (textNO == "")
                        throw new Exception("错误:设备编号未输入，请重新输入！");
                    else if (textIP == "")
                        throw new Exception("错误:设备IP地址未输入，请重新输入！");
                    else if (textPort == "")
                        throw new Exception("错误:设备端口号未输入，请重新输入！");

                    //LineNO生产线加入设备SN码 key:LineNo(集合类型)
                    Client.SAdd(lineno, Encoding.UTF8.GetBytes(textSN));

                    //增加设备的参数信息，key：SN码(Hash类型)
                    byte[][] keys = new byte[][]
                    {
                        Encoding.UTF8.GetBytes("MachineNo"),
                        Encoding.UTF8.GetBytes("IP"),
                        Encoding.UTF8.GetBytes("Port"),
                        Encoding.UTF8.GetBytes("DB")
                    };

                    byte[][] values = new byte[][]
                    {
                        Encoding.UTF8.GetBytes(textNO),
                        Encoding.UTF8.GetBytes(textIP),
                        Encoding.UTF8.GetBytes(textPort),
                        Encoding.UTF8.GetBytes(textDB)
                    };
                    Client.HMSet("MachineSN:"+textSN, keys, values);
                    return true;
                }
                else if (operation == Operation.Delete)
                {
                    //设备信息输入校正
                    if (textSN == "")
                        throw new Exception("错误:设备SN码未输入，请重新输入！");

                    if (LineExist == true)
                    {
                        Client.Remove(textSN);
                        Client.SRem(lineno, Encoding.UTF8.GetBytes(textSN));
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("生产线" + lineno.ToString() + "生产线数据已全部清除！", "提示");
                        return false;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
                return false;
            }
            return true;
        }

    }

}
