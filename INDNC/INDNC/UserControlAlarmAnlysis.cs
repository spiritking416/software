using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;

namespace INDNC
{
    public partial class UserControlAlarmAnlysis : UserControl
    {
        private DataTable myTab = new DataTable("mytab");
        List<MachineInfo> CNCinfo = new List<MachineInfo>();
        List<MachineInfo> Robotinfo = new List<MachineInfo>();
        public List<MachineInfo> CNCinfoName
        {
            set
            {
                CNCinfo = value;
            }
        }

        public List<MachineInfo> RobotinfoName
        {
            set
            {
                Robotinfo = value;
            }
        }
        public UserControlAlarmAnlysis()
        {
            InitialDraw();
        }

        private void InitialDraw()
        {
            InitializeComponent();

            chart1.Series[0].YValueMembers = "Yvalue";
            chart1.Series[0].XValueMember = "Xvalue";
            chart1.Series[0].LegendToolTip = "我的成绩"; // 鼠标放到系列上出现的文字  
            chart1.Series[0].LegendText = "当前告警"; // 系列名字  
            chart1.Series[0].XValueType = ChartValueType.Date;

            InitialTab();

            chart1.DataSource = myTab;
            chart1.DataBind();
        }

        private void InitialTab()
        {

        }

        //绘制
        public void AlarmDraw()
        {
            try
            {
                
                chart1.Update();
            }
            catch
            {

            }
        }
    }
}
