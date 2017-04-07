using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;
using ServiceStack.Redis;

namespace INDNC
{
    public partial class UserControlAlarmAnlysis : UserControl
    {
        private DataTable myTab = new DataTable("mytab");
        List<MachineInfo> CNCinfo = new List<MachineInfo>();
        List<MachineInfo> Robotinfo = new List<MachineInfo>();
        RedisPara serverpara;  //服务器参数
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

        public RedisPara serverparaName
        {
            set
            {
                serverpara = value;
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
            string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort };
            try
            {
                //从连接池获得只读连接客户端
                int initialDB = 0;
                RedisClient Client = new RedisClient();
                if (Client == null || !Client.Ping())
                {
                    throw new Exception("连接服务器失败，请设置服务器参数!");
                }
                //连接云端服务器成功
            }
            catch
            {

            }

                myTab.Columns.Add("ID", Type.GetType("System.Int32"));
            myTab.Columns[0].AutoIncrement = true;
            myTab.Columns[0].Unique = true;
            myTab.Columns[0].AutoIncrementSeed = 1;
            myTab.Columns[0].AutoIncrementStep = 1;
            myTab.Columns.Add("Yvalue", Type.GetType("System.Int32"));
            myTab.Columns.Add("Xvalue", Type.GetType("System.DateTime"));

            myTab.Rows.Add(new object[] { null, 22D, "2015-02-01 8:00" });
            myTab.Rows.Add(new object[] { null, 23D, "2015-03-01 9:00" });
            myTab.Rows.Add(new object[] { null, 25D, "2015-04-01 10:00" });
            myTab.Rows.Add(new object[] { null, 24D, "2015-05-01 11:00" });
            myTab.Rows.Add(new object[] { null, 22D, "2015-06-01 12:00" });
            myTab.Rows.Add(new object[] { null, 21D, "2015-07-01 13:00" });
        }

        //绘制
        public void AlarmDraw()
        {
            try
            {
                InitialTab();
                chart1.Update();
            }
            catch
            {

            }
        }
    }
}
