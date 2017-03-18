using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LineDevice;
using System.Collections;
namespace SCADA
{
    public partial class RobotForm : Form
    {
        PLC.PLC_MITSUBISHI_HNC8 plc;  
        ROBOT robot;
        public static int ROBOTIndex = 0;
       // AutomaticSize automaticSize = new AutomaticSize();
//         AutoSizeFormClass asc = new AutoSizeFormClass();
        public static IntPtr robotPtr;
        public RobotForm()
        {
            InitializeComponent();
        }

       
        private void RobotForm_Load(object sender, EventArgs e)
        {
//           asc.controllInitializeSize(this);

//            MultiLanguage.lang = MultiLanguage.ReadDefaultLanguage();
//             MultiLanguage.getNames(this);
// 
//             IList list = MultiLanguage.GetLanguageList(MultiLanguage.lang);
//            asc.controllInitializeSize(this);
//            if (Localization.HasLang)
//            {
//                Localization.RefreshLanguage(this);
//                changeDGV();
//            }
           robotPtr = Handle;

           switchRobot.Items.Clear();
           foreach (ROBOT eachRobot in MainForm.robotlist)
           {
               string str = "Robot:";
               str += eachRobot.EQUIP_CODE;
               switchRobot.Items.Add(str);
           }
          

           if (switchRobot.Items.Count > 0 && PLCDataShare.m_plclist.Count > 0)
           {
               plc = PLCDataShare.m_plclist[0];
               switchRobot.SelectedIndex = 0;
               robot = MainForm.robotlist[switchRobot.SelectedIndex];
               timer1.Enabled = true;
           }
        }

      

        private void RobotForm_SizeChanged(object sender, EventArgs e)
        {
//            asc.controlAutoSize(this);
            this.WindowState = (System.Windows.Forms.FormWindowState)(2);
        }
        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] SetImage(string path)
        {
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            //   System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            byte[] bydata = new byte[fs.Length];
            fs.Read(bydata, 0, bydata.Length);
            fs.Close();
            return bydata;

        }
        /// <summary>
        /// 图片格式转换
        /// </summary>
        /// <param name="imageByte"></param>
        /// <returns></returns>
        public System.Drawing.Image GerImage(byte[] imageByte)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(imageByte);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            return img;
        }

 
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                UpdateRobotSignal(robot);               
            }
        }

        private bool switchRobot_Selected = false;
        private void switchRobot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (plc != null || robot == null)
            {
                robot = MainForm.robotlist[switchRobot.SelectedIndex];
                ROBOTIndex = switchRobot.SelectedIndex;
                switchRobot_Selected = true;
            }
        }

        /// <summary>
        /// 获取PLC信号并将其列表出来
        /// </summary>
        /// <param name="plc"></param>
        PLC.PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[] MITSUBISHIPLC_SignalList_result1;
        PLC.PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[] MITSUBISHIPLC_SignalList_result2;
        PLC.PLC_MITSUBISHI_HNC8.HNC8SignalType[] HNC8PLC_SignalList_result1;
        PLC.PLC_MITSUBISHI_HNC8.HNC8SignalType[] HNC8PLC_SignalList_result2;
        private void getRobotSingalList2plcDGV(String Device, Int32 Star, Int32 Count, System.Windows.Forms.DataGridView DGV
            , ref PLC.PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[] MITSUBISHIPLC_SignalList_result, ref PLC.PLC_MITSUBISHI_HNC8.HNC8SignalType[] HNC8PLC_SignalList_result)
        {
            DGV.Rows.Clear();
            if (plc.system == m_xmlDociment.PLC_System[0])//三菱
            {
                if (MITSUBISHIPLC_SignalList_result != null)
                {
                    for (int ii = 1; ii < MITSUBISHIPLC_SignalList_result.Length; ii++)
                    {
                        if (MITSUBISHIPLC_SignalList_result[ii].IsShow && MITSUBISHIPLC_SignalList_result[ii].ACTION_ID == "-1")
                        {
                            MITSUBISHIPLC_SignalList_result[ii].IsShow = false;
                        }
                    }
                }

                MITSUBISHIPLC_SignalList_result = plc.MITSUBISHIPLC_SignalList.Find(
                        delegate(PLC.PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[] temp)
                        {
                            return String.Equals(temp[0].EQUIP_CODE, Device, StringComparison.Ordinal);
                        }
                        );

                if (MITSUBISHIPLC_SignalList_result != null)//找到
                {
                    DGV.RowCount = Count;
                    for (int ii = 0; ii <Count; ii++)
                    {
                        int result_ii = ii + 1 + Star;
                        if (MITSUBISHIPLC_SignalList_result[0].Address == 10)//10进制
                        {
                            DGV.Rows[ii].Cells[0].Value = Device + MITSUBISHIPLC_SignalList_result[result_ii].Address;
                        }
                        else//十六进制
                        {
                            DGV.Rows[ii].Cells[0].Value = Device + "0x" + String.Format("{0:X}", MITSUBISHIPLC_SignalList_result[result_ii].Address);
                        }
                        DGV.Rows[ii].Cells[1].Value = MITSUBISHIPLC_SignalList_result[result_ii].ArrLabel;
                        if (MITSUBISHIPLC_SignalList_result[result_ii].Value == 0 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_black)
                        {
                            DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_black;
                        }
                        else if (MITSUBISHIPLC_SignalList_result[result_ii].Value == 1 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_green)
                        {
                            DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_green;
                        }
                        if (MITSUBISHIPLC_SignalList_result[result_ii].ACTION_ID == "-1")//显示类型
                        {
                            MITSUBISHIPLC_SignalList_result[result_ii].IsShow = false;//将所有的显示类型采集属性设置为false
                        }
                        else
                        {
                            MITSUBISHIPLC_SignalList_result[result_ii].IsShow = true;
                        }
                    }
                }
            }
            else if (plc.system == m_xmlDociment.PLC_System[1])//hnc8
            {
                if (HNC8PLC_SignalList_result != null)
                {
                    for (int ii = 1; ii < HNC8PLC_SignalList_result.Length; ii++)
                    {
                        if (HNC8PLC_SignalList_result[ii].IsShow && HNC8PLC_SignalList_result[ii].ACTION_ID == "-1")
                        {
                            HNC8PLC_SignalList_result[ii].IsShow = false;
                        }
                    }
                }

                HNC8PLC_SignalList_result = plc.HNC8PLC_SignalList.Find(
                        delegate(PLC.PLC_MITSUBISHI_HNC8.HNC8SignalType[] temp)
                        {
                            return String.Equals(temp[0].ArrLabel, Device, StringComparison.Ordinal);
                        }
                        );

                if (HNC8PLC_SignalList_result != null)//没找到
                {
                    DGV.RowCount = Count;
                    for (int ii = 0; ii < Count; ii++)
                    {
                        int result_ii = ii + 1 + Star;
                        if (HNC8PLC_SignalList_result[result_ii].SubAddress == -1)//是值
                        {
                            DGV.Rows[ii].Cells[0].Value = HNC8PLC_SignalList_result[result_ii].Address;
                        }
                        else
                        {
                            DGV.Rows[ii].Cells[0].Value = HNC8PLC_SignalList_result[result_ii].Address.ToString() + "." + HNC8PLC_SignalList_result[result_ii].SubAddress.ToString();
                        }
                        DGV.Rows[ii].Cells[1].Value = HNC8PLC_SignalList_result[result_ii].ArrLabel;
                        if (HNC8PLC_SignalList_result[result_ii].Value == 0 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_black)
                        {
                            DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_black;
                        }
                        else if (HNC8PLC_SignalList_result[result_ii].Value == 1 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_green)
                        {
                            DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_green;
                        }
                        if (HNC8PLC_SignalList_result[result_ii].ACTION_ID == "-1")
                        {
                            HNC8PLC_SignalList_result[result_ii].IsShow = false;//将所有的显示类型采集属性设置为false
                        }
                        else
                        {
                            HNC8PLC_SignalList_result[result_ii].IsShow = true;//将所有的显示类型采集属性设置为false
                        }
                    }
                }
            }
        }

        private void UpdateRobotSignal(ROBOT robot)
        {
            if (switchRobot_Selected)
            {
                getRobotSingalList2plcDGV("X", robot.PLCAdressStar_X, robot.AdressSum_X,robotDGVInPut
                    , ref MITSUBISHIPLC_SignalList_result1, ref HNC8PLC_SignalList_result1);
                getRobotSingalList2plcDGV("Y", robot.PLCAdressStar_Y, robot.AdressSum_Y, robotDGVOutPut
                    , ref MITSUBISHIPLC_SignalList_result2, ref HNC8PLC_SignalList_result2);
                switchRobot_Selected = false;
            }
            else
            {
                UpdateRobotSignal("X", robot.PLCAdressStar_X, ref robotDGVInPut
                    , ref MITSUBISHIPLC_SignalList_result1, ref HNC8PLC_SignalList_result1);
                UpdateRobotSignal("Y", robot.PLCAdressStar_Y, ref robotDGVOutPut
                    , ref MITSUBISHIPLC_SignalList_result2, ref HNC8PLC_SignalList_result2);
            }


        }

        private void UpdateRobotSignal(String Device, Int32 Star,ref System.Windows.Forms.DataGridView DGV
            , ref  PLC.PLC_MITSUBISHI_HNC8.MITSUBISHISignalType[] MITSUBISHIPLC_SignalList_result, ref PLC.PLC_MITSUBISHI_HNC8.HNC8SignalType[] HNC8PLC_SignalList_result)
        {
//             if (DGV.Visible)
            {
                for (int ii = 0; ii < DGV.RowCount; ii++)
                {
                    int index = Star + ii + 1;
                    if (DGV.Rows[ii].Displayed)
                    {
                        if (plc.system == m_xmlDociment.PLC_System[0] && MITSUBISHIPLC_SignalList_result != null)//三菱
                        {
                            if (!MITSUBISHIPLC_SignalList_result[index].IsShow)
                            {
                                MITSUBISHIPLC_SignalList_result[index].IsShow = true;//开启采集
                            }

                            if (MITSUBISHIPLC_SignalList_result[index].Value == 0 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_black)
                                {
                                    DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_black;
                                }
                            else if (MITSUBISHIPLC_SignalList_result[index].Value == 1 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_green)
                                {
                                    DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_green;
                                }
                        }
                        else if (plc.system == m_xmlDociment.PLC_System[1] && HNC8PLC_SignalList_result != null)//hnc8
                        {
                            if (!HNC8PLC_SignalList_result[index].IsShow)
                            {
                                HNC8PLC_SignalList_result[index].IsShow = true;//开启采集
                            }

                            if (HNC8PLC_SignalList_result[index].Value == 0 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_black)
                                {
                                    DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_black;
                                }
                            else if (HNC8PLC_SignalList_result[index].Value == 1 && DGV.Rows[ii].Cells[2].Value != SCADA.Properties.Resources.top_bar_green)
                                {
                                    DGV.Rows[ii].Cells[2].Value = SCADA.Properties.Resources.top_bar_green;
                                }
                        }
                    }
                    else
                    {
                        if (plc.system == m_xmlDociment.PLC_System[0] && MITSUBISHIPLC_SignalList_result != null &&
                            MITSUBISHIPLC_SignalList_result[index].IsShow && MITSUBISHIPLC_SignalList_result[index].ACTION_ID == "-1")//三菱
                        {
                            MITSUBISHIPLC_SignalList_result[index].IsShow = false;
                        }
                        else if (plc.system == m_xmlDociment.PLC_System[1] && MITSUBISHIPLC_SignalList_result != null &&
                            HNC8PLC_SignalList_result[index].IsShow && HNC8PLC_SignalList_result[index].ACTION_ID == "-1")
                        {
                            HNC8PLC_SignalList_result[index].IsShow = false;
                        }
                    }
                }
            }
        }


        private void changeDGV()
        {
//             robotDGVInPut.Columns[0].HeaderText = Localization.Forms["RobotForm"]["input_id"];
//             robotDGVInPut.Columns[1].HeaderText = Localization.Forms["RobotForm"]["input_name"];
//             robotDGVInPut.Columns[2].HeaderText = Localization.Forms["RobotForm"]["input_getPicture"];
//             robotDGVOutPut.Columns[0].HeaderText = Localization.Forms["RobotForm"]["output_id"];
//             robotDGVOutPut.Columns[1].HeaderText = Localization.Forms["RobotForm"]["output_name"];
//             robotDGVOutPut.Columns[2].HeaderText = Localization.Forms["RobotForm"]["output_getPicture"];
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MainForm.LanguageChangeMsg:
//                     Localization.RefreshLanguage(this);
//                     changeDGV();
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void RobotForm_VisibleChanged(object sender, EventArgs e)
        {
            if (((RobotForm)sender).Visible && switchRobot.Items.Count > 0)
            {
                switchRobot.SelectedIndex = ROBOTIndex;
            }

        }
    }

}
