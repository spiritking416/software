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
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        ServerLink serverlink = new ServerLink();
        RedisClient Client = new RedisClient();
        private void button1_Click_1(object sender, EventArgs e)
        {
            serverlink.ServerIPAddress = textBox1.Text + '.' + textBox2.Text + '.' + textBox3.Text + '.' + textBox4.Text;
            int port = 0;
            if(int.TryParse(textBox5.Text,out port) != true)
            {
                MessageBox.Show("错误:服务器端口输入错误，请重新输入！", "ERROR");
                return;
            }
            serverlink.ServerPort = port;
            serverlink.ServerPassword = textBox6.Text;
            serverlink.Link(ref (Client));
            MessageBox.Show("1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serverlink.Link(ref (Client));
            
            MessageBox.Show("2");
        }
    }
    public partial class ServerLink
    {
        public String ServerIPAddress = null;
        public int ServerPort = 0;
        public String ServerPassword = null;
        public long DBNo = 0;

        public UInt16 Link(ref RedisClient Client)
        {
            try
            {
                Client = new RedisClient(ServerIPAddress, ServerPort, ServerPassword, DBNo);
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                ServerIPAddress = null;
                ServerPort = 0;
                ServerPassword = null;
                DBNo = 0;
                return 1;
            }

            return 0;
        }

    }
}
