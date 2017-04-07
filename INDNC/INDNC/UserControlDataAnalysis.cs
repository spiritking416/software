using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace INDNC
{
    public partial class UserControlDataAnalysis : UserControl
    {
        public UserControlAlarmAnlysis alarmanalysis = new UserControlAlarmAnlysis();

        public UserControlDataAnalysis()
        {
            InitializeComponent();
            alarmanalysis.Dock = DockStyle.Fill;
        }

        private void radioButtonAlarm_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAlarm.Checked == false)
            {
                panel2.Controls.Clear();
                return;
            }

            panel2.Controls.Add(alarmanalysis);
        }
    }
}
