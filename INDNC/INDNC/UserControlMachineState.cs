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

    }
}
