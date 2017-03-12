using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;
using ServiceStack.Redis;

namespace INDNC
{
    public partial class RedisParaSetting : Form
    {
        RedisPara redispara = new RedisPara();

        public RedisPara redisparaName
        {
            get { return redispara; }
        }


        public RedisParaSetting()
        {
            InitializeComponent();
            redispara.connectvalid = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            redispara.connectvalid = false;  
            String IP = textBox1.Text;
            int port = 0;

            redispara.RedisIP = textBox1.Text;
            redispara.RedisPort = textBox2.Text;
            redispara.RedisPassword = textBox3.Text;

            if (int.TryParse(textBox2.Text, out port) != true)
            {
                MessageBox.Show("错误:服务器端口号输入错误，请重新输入！", "ERROR");
                return;
            }
            String password = textBox3.Text;
            long initialDb = 0;
            string[] host = { redispara.RedisPassword + '@' + redispara.RedisIP + ':' + redispara.RedisPort };
            try
            {
                RedisManager redismanager = new RedisManager(ref (initialDb), ref (host));
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDb), ref (host));
                if (!Client.Ping())
                {
                    throw new Exception("连接服务器失败!");
                }
                MessageBox.Show("测试连接本地Redis数据库成功！", "提示");
                redispara.connectvalid = true;
                this.Close();
            }
            catch(Exception ex)
            { 
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                redispara.connectvalid = false;
            }    
        }
    }
}
