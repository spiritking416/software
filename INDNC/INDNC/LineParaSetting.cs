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
    public partial class LineParaSetting : Form
    {
        UInt16 LineCount;
        UInt16 LineNo;

        public UInt16 LineCountName
        {
            get { return LineCount; }
        }

        public UInt16 LineNoName
        {
            get { return LineNo; }
        }

        public LineParaSetting()
        {
            InitializeComponent();

        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (comboBox1.Items != null)
            {
                comboBox1.Items.Clear();
                comboBox1.Text = null;
            }   
            try
            {
                string str = textBox1.Text;
                UInt16 linenum;
                if (UInt16.TryParse(textBox1.Text, out linenum) != true || linenum > 100)
                {
                    throw new Exception("错误:生产线数量输入错误，请重新输入！");
                }

                for (int i = 1; i <= linenum; ++i)
                    comboBox1.Items.Add("生产线路" + i.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
        }

        private void LineParaSetting_Load(object sender, EventArgs e)
        {
            string str = textBox1.Text;
            string s = comboBox1.Text.Substring(4);
            UInt16 linenum;
            int lineno;
            try
            {
                if (UInt16.TryParse(textBox1.Text, out linenum) != true || linenum > 100)
                {
                    throw new Exception("错误:生产线数量输入错误，请重新输入！");
                }
                for (int i = 1; i <= linenum; ++i)
                    comboBox1.Items.Add("生产线路" + i.ToString());
                if (int.TryParse(s, out lineno) != true)
                {
                    return;
                }
                comboBox1.SelectedIndex = lineno - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
                //MessageBox.Show("ERROR:生产线索引输入错误！", "ERROR");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text;

            try
            {
                string text = comboBox1.SelectedItem.ToString();
                int offset = text.IndexOf("生产线路") + 4;
                string countstring = text.Substring(offset);
                if (UInt16.TryParse(countstring, out LineNo) != true || UInt16.TryParse(str, out LineCount) != true || LineCount > 100)
                {
                    LineCount = 0;
                    LineNo = 0;
                    throw new Exception("错误:生产线数量输入错误，请重新输入！");
                }
                //MessageBox.Show(comboBox1.SelectedIndex.ToString());
                Properties.Settings.Default.Save();
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR:生产线索引输入错误！", "ERROR");
            }  
        }
    }
}
