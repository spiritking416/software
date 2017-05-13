using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;
using ServiceStack.Redis;

using System.Text;


namespace INDNC
{
    public partial class UserControlAlarmAnlysis : UserControl
    {
        private DataTable myTab = new DataTable("mytab");
        List<MachineInfo> CNCinfo = new List<MachineInfo>();
        List<MachineInfo> Robotinfo = new List<MachineInfo>();
        RedisPara serverpara;  //服务器参数
        RedisManager redismanager = new RedisManager();
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
            myTab.Columns.Add("ID", Type.GetType("System.Int32"));
            myTab.Columns.Add("Yvalue", Type.GetType("System.Int64"));
            myTab.Columns.Add("Xvalue", Type.GetType("System.String"));
            myTab.Columns[0].AutoIncrement = true;
            myTab.Columns[0].Unique = true;
            myTab.Columns[0].AutoIncrementSeed = 1;
            myTab.Columns[0].AutoIncrementStep = 1;
            chart1.Series[0].YValueMembers = "Yvalue";
            chart1.Series[0].XValueMember = "Xvalue";
            chart1.Series[0].LegendToolTip = "机床的历史告警数目"; // 鼠标放到系列上出现的文字  
            chart1.Series[0].LegendText = "历史告警"; // 系列名字  
            chart1.Series[0].XValueType = ChartValueType.Date;
        }

        private void InitialDraw()
        {
            InitializeComponent();
        }

        private void InitialTab()
        {
            
            try
            {
                
                if (serverpara.connectvalid == false)
                    throw new Exception("服务器参数获取失败，请检查参数设置是否有误！");
                string[] host = { serverpara.RedisPassword + '@' + serverpara.RedisIP + ':' + serverpara.RedisPort };

                //从连接池获得只读连接客户端
                int initialDB = 0;
                RedisClient Client = (RedisClient)redismanager.GetReadOnlyClient(ref (initialDB), ref (host));
                if (Client == null || !Client.Ping())
                {
                    throw new Exception("连接服务器失败，请设置服务器参数!");
                }
                //连接云端服务器成功

                myTab.Clear();
                foreach (var key in CNCinfo)
                {
                    Client.ChangeDb(key.MachineDB);

                    byte[] machinealarmbyte = new byte[] { };
                    byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_HISTORY");
                    machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                    if (machinealarmbyte == null)
                        continue;
                    string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                    long machinealarm = Convert.ToInt64(machinealarmstr);

                    myTab.Rows.Add(new object[] { null, machinealarm, key.MachineName });
                }
                foreach (var key in Robotinfo)
                {
                    Client.ChangeDb(key.MachineDB);

                    byte[] machinealarmbyte = new byte[] { };
                    byte[] alarmbyte = Encoding.UTF8.GetBytes("ALARMNUM_HISTORY");
                    machinealarmbyte = Client.HGet("Alarm:AlarmNum", alarmbyte);
                    if (machinealarmbyte == null)
                        continue;
                    string machinealarmstr = System.Text.Encoding.Default.GetString(machinealarmbyte);
                    long machinealarm = Convert.ToInt64(machinealarmstr);

                    myTab.Rows.Add(new object[] { null, machinealarm, key.MachineName });
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
            }
        }

        //绘制
        public void AlarmDraw()
        {
            try
            {
                InitialTab();
                chart1.DataSource = myTab;
                chart1.DataBind();
            }
            catch
            {

            }
        }
    }
}
