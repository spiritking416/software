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
using System.Threading;

namespace INDNC
{
    public partial class UserControlMachineState : UserControl
    {
        public Thread threadRefrush;

        public UserControlMachineState()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            comboBoxMachineview.Items.Add("可连接设备");
            comboBoxMachineview.Items.Add("在线设备");
            comboBoxMachineview.Items.Add("离线设备");
            comboBoxMachineview.Items.Add("告警设备数目");
            comboBoxMachineview.Items.Add("未显示设备数目");
            comboBoxMachineview.SelectedIndex = 0;
        }

        public bool ListViewTitleDraw()
        {
            try
            {
                this.listView1.BeginUpdate();

                int width = (this.listView1.Width - 60) / 4;
                this.listView1.Columns.Add("序号", 60, HorizontalAlignment.Left);
                this.listView1.Columns.Add("机床编号", width, HorizontalAlignment.Left);
                this.listView1.Columns.Add("机床状态", width, HorizontalAlignment.Left);
                this.listView1.Columns.Add("当前告警", width, HorizontalAlignment.Left);
                this.listView1.Columns.Add("发生时间", width, HorizontalAlignment.Left);

                this.listView1.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                return false;
            }

            return true;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
