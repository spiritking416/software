using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INDNC
{
    public partial class AddOrDelSetting : Form
    {
        public AddOrDelSetting()
        {
            InitializeComponent();
        }

        private void AddOrDelSetting_Load(object sender, EventArgs e)
        {
            string text = global::INDNC.Properties.Settings.Default.LineCount;
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
            setting(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }


        public void setting(UInt16 operation)
        {
            string textSN = textBox1.Text;
            string textNO = textBox2.Text;

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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }

        }
    }
}
