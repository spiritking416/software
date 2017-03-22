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
using HNCAPI;
using System.Runtime.InteropServices;

namespace SCADA
{
    public partial class HomeForm : Form
    {
        static string[] CNCStateColorPictrueFile = { "..\\picture\\picCNCgreed.png",
        "..\\picture\\picCNCyel.png","..\\picture\\picCNCred.png","..\\picture\\picCNCray.png" };//CNC设备状态图片路径
        static string[] ROBOTStateColorPictrueFile = { "..\\picture\\picRobotgreed.png",
        "..\\picture\\picRobotyel.png","..\\picture\\picRobotgred.png","..\\picture\\picRobotgray.png" };//robot设备状态图片路径

        static string[] RGVStateColorPictrueFile = { "..\\picture\\picRGVgreed.png",
        "..\\picture\\picRGVyel.png","..\\picture\\picRGVgred.png","..\\picture\\picRGVgray.png" };//RGV设备状态图片路径

        static string[] ToolTip_str = { "名称：", "IP：", "运行程序：" };//ToolTip显示信息
        static string[] State_str = { "运行", "空闲", "报警", "离线" };//状态字符串
        static int lineType =1; //产线类型  1：RGV方式    0：ROBROT 一拖二
        static int hOffset = 0; //水平间隔

        class ShowItem//显示对象数据结构
        {
            Button m_bt;
            Panel m_panel;
            Label m_label;
            public int m_showtype;
            public int m_showitemserial;
            string m_ColorPictrue_str;
            int m_label_Height;
            //int Oldstad ;

            public ShowItem(int m_showtype, int m_showitemserial)//初始化
            {
                //Oldstad = -100;
                m_bt = new Button();
                m_panel = new Panel();
                m_label = new Label();
                m_label_Height = 20;
                this.m_showitemserial = m_showitemserial;
                this.m_showtype = m_showtype;
                if (m_showtype == 0)
                {
                    m_ColorPictrue_str = CNCStateColorPictrueFile[3];
                }
                else if (m_showtype == 1)
                {
                    if(lineType == 1)  //2015.11.28 RGV
                         m_ColorPictrue_str = RGVStateColorPictrueFile[3];
                    else
                        m_ColorPictrue_str = ROBOTStateColorPictrueFile[3];
                }
                else
                {
                    m_ColorPictrue_str = ROBOTStateColorPictrueFile[3];
                }
                m_bt.Click += new EventHandler(SkipWindow);//单击事件
                m_bt.MouseHover += new EventHandler(MouseHoverTooltip);//鼠标停留事件
            }
            /// <summary>
            /// 鼠标点击事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void SkipWindow(object sender, System.EventArgs e)
            {
                Button m_button = (Button)sender;
                PostMessage(MainForm.m_Ptr, MainForm.USERMESSAGE + m_showtype, m_showtype + 2, m_showitemserial);
            }

            /// <summary>
            /// 鼠标停留事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void MouseHoverTooltip(object sender, System.EventArgs e)
            {
                ToolTip m_tooltip = new ToolTip();
                if (m_showtype == 0)//CNC
                {
                    CNC m_cnc = MainForm.cnclist[m_showitemserial];
                    string str = "";
                    str += HomeForm.ToolTip_str[0] + m_cnc.JiTaiHao + "\r\n";
                    str += ToolTip_str[1] + m_cnc.ip + "\r\n";
                    str += ToolTip_str[2] + m_cnc.get_gCodeName() + "\r\n";
                    m_tooltip.SetToolTip((Control)sender, str);
                }
                else if (m_showtype == 1)//robot
                {
                    ROBOT m_robot = MainForm.robotlist[m_showitemserial];
                    string str = "";
                    str += HomeForm.ToolTip_str[0] + m_robot.EQUIP_CODE + "\r\n";
//                     str += ToolTip_str[1] + m_cnc.ip + "\r\n";
//                     str += ToolTip_str[2] + m_cnc.get_gCodeName() + "\r\n";
                    m_tooltip.SetToolTip((Control)sender, str);
                }
                
            }

            /// <summary>
            /// 显示对象
            /// </summary>
            /// <param name="Parent"></param>
            /// <param name="m_panel_Width"></param>
            /// <param name="m_panel_Height"></param>
            /// <param name="m_panel_Left"></param>
            /// <param name="m_panel_Top"></param>
            public void ShowItem2window(Form Parent,int m_panel_Width, int m_panel_Height, int m_panel_Left, int m_panel_Top, int index)
            {

                m_panel.Width = m_panel_Width;
                m_panel.Height = m_panel_Height;
                m_panel.Left = m_panel_Left;
                m_panel.Top = m_panel_Top;

                m_bt.Parent = m_panel;
                m_label.Parent = m_panel;
                if (m_showtype == 0)
                {
                    m_label.Text = MainForm.cnclist[m_showitemserial].JiTaiHao;
                }
                else if (m_showtype == 1)
                {
                    m_label.Text = MainForm.robotlist[m_showitemserial].EQUIP_CODE;
                }
                m_label.AutoSize = false;
                m_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                double TextSize = m_panel_Width / 6.7;
                m_label.Font = new System.Drawing.Font("宋体", float.Parse(TextSize.ToString()), FontStyle.Bold);

//                 m_bt.Tag = 9999;
//                 m_label.Tag = 9999;
                m_bt.Width = m_panel.Width;
                m_bt.Height = m_panel_Height - m_label_Height;
                m_label.Top = m_bt.Height;
                m_label.Width = m_panel.Width;
                m_label.Height = m_label_Height;
                m_bt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                Parent.Controls.Add(m_panel);
               
                Size m_imagesize = new Size();
                m_imagesize.Width = m_bt.Width;
                m_imagesize.Height = m_bt.Height;

                if (m_showtype == 1)//robot 2015.11.28
                {
                    if (lineType == 1)  
                    {
                        if (index == MainForm.cnclist.Count )
                            m_ColorPictrue_str = ROBOTStateColorPictrueFile[3];
                        else
                            m_ColorPictrue_str = RGVStateColorPictrueFile[3];
                    }
                    else
                    {
                        if (index == MainForm.cnclist.Count )
                             m_panel.Top -= 70;
                        else if(index == (MainForm.cnclist.Count + 1) )
                        {
                            m_panel.Left -= hOffset;
                            m_panel.Top += 70;
                        }
                        else
                            m_panel.Left -= hOffset;

                        m_ColorPictrue_str = ROBOTStateColorPictrueFile[3];
                    }
                }

                Bitmap m_bitmap = new Bitmap(System.Drawing.Image.FromFile(m_ColorPictrue_str), m_imagesize);
//                 m_bitmap.MakeTransparent(Color.White);//设置背景是透明的
                m_bt.Image = m_bitmap;
                m_bt.FlatStyle = FlatStyle.Flat;//样式
                m_bt.ForeColor = Color.Transparent;//前景
                m_bt.FlatAppearance.BorderSize = 0;
                m_panel.Show();
            }


            /// <summary>
            /// 更新对象状态
            /// </summary>
            bool ROBOTstad = false;
            public void UpdataShowItem()
            {
                LineDevice.CNC.CNCState stad = 0;//状态
                if (m_showtype == 0)//CNC
                {
                    MainForm.cnclist[m_showitemserial].Checkcnc_state(ref stad);
//                     if(Oldstad != stad)
                    {
                        String ShowStr = MainForm.cnclist[m_showitemserial].JiTaiHao;
                        switch (stad)
                        {
                            case LineDevice.CNC.CNCState.DISCON:
                                m_label.Text = ShowStr;
                                m_ColorPictrue_str = CNCStateColorPictrueFile[3];
                                break;
                            case LineDevice.CNC.CNCState.IDLE:
                                m_label.Text = ShowStr;
                                m_ColorPictrue_str = CNCStateColorPictrueFile[1];
                                break;
                            case LineDevice.CNC.CNCState.ALARM:
                                m_label.Text = ShowStr;
                                m_ColorPictrue_str = CNCStateColorPictrueFile[2];
                                break;
                            case LineDevice.CNC.CNCState.RUNING://运行
                                m_label.Text = ShowStr;
                                m_ColorPictrue_str = CNCStateColorPictrueFile[0];
                                break;
                            default:
                                break;
                        }
                        Size m_imagesize = new Size();
                        m_imagesize.Width = m_bt.Width;
                        m_imagesize.Height = m_bt.Height;
                        Bitmap m_bitmap = new Bitmap(System.Drawing.Image.FromFile(m_ColorPictrue_str), m_imagesize);
                        if (m_bitmap != m_bt.Image)
                        {
                            m_bt.Image = m_bitmap;
                        }
                    }
                }
                else if (m_showtype == 10)//ROBOT
                {
                    if (PLCDataShare.m_plclist.Count > 0)
                    {
                        String ShowStr = MainForm.robotlist[m_showitemserial].EQUIP_CODE ;
                        bool connet = false;
                        if (PLCDataShare.m_plclist[0].system == m_xmlDociment.PLC_System[0])
                        {
                            connet = PLCDataShare.m_plclist[0].conneted;
                        }
                        else if (PLCDataShare.m_plclist[0].system == m_xmlDociment.PLC_System[1])
                        {
                            connet = PLCDataShare.m_plclist[0].m_hncPLCCollector.connectStat;
                        }
                        if (ROBOTstad != connet)
                        {
                            if (connet)
                            {
                                m_label.Text = ShowStr;
                                if (m_showitemserial != MainForm.cnclist.Count)
                                {
                                    if (lineType == 1)  //2015.11.28 RGV
                                        m_ColorPictrue_str = RGVStateColorPictrueFile[0];
                                    else
                                        m_ColorPictrue_str = ROBOTStateColorPictrueFile[0];
                                }

                            }
                            else
                            {
                                m_label.Text = ShowStr;
                                if (m_showitemserial != MainForm.cnclist.Count)
                                {
                                    if (lineType == 1)  //2015.11.28 RGV
                                        m_ColorPictrue_str = RGVStateColorPictrueFile[3];
                                    else
                                        m_ColorPictrue_str = ROBOTStateColorPictrueFile[3];
                                }
                            }
                            Size m_imagesize = new Size();
                            m_imagesize.Width = m_bt.Width;
                            m_imagesize.Height = m_bt.Height;
                            Bitmap m_bitmap = new Bitmap(System.Drawing.Image.FromFile(m_ColorPictrue_str), m_imagesize);
                            m_bt.Image = m_bitmap;
                            ROBOTstad = connet;
                        }
                    }
                }
            }
        }

        List<ShowItem> Showlist = new List<ShowItem>();
//         int cnc_num = 0;
        int robot_num = 0;

        [DllImport("user32.dll")]
        public static extern void PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public HomeForm()
        {
            InitializeComponent();
        }


        private void HomeForm_Load(object sender, EventArgs e)
        {
            pictureBoxUnConnetColor.BackColor = new Bitmap(System.Drawing.Image.FromFile(CNCStateColorPictrueFile[3])).GetPixel(30, 30);
            pictureBoxKongXianColor.BackColor = new Bitmap(System.Drawing.Image.FromFile(CNCStateColorPictrueFile[1])).GetPixel(30, 30);
            pictureBoxRuningColor.BackColor = new Bitmap(System.Drawing.Image.FromFile(CNCStateColorPictrueFile[0])).GetPixel(30, 30);
            pictureBoxArrColor.BackColor = new Bitmap(System.Drawing.Image.FromFile(CNCStateColorPictrueFile[2])).GetPixel(30, 30);
            if (MainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]) == m_xmlDociment.Default_linetype_value[0])
            {
                lineType = 0;
            }
            else if (MainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]) == m_xmlDociment.Default_linetype_value[1])
            {
                lineType = 1;
            }
            robot_num = 0;
            Showlist.Clear();
            if (MainForm.cnclist.Count > 0)
            {
                foreach (CNC m_cnc in MainForm.cnclist)
                {
                    Showlist.Add(new ShowItem(0, m_cnc.serial));
                }
            }
            if (MainForm.robotlist.Count > 0)
            {
                foreach (ROBOT m_robot in MainForm.robotlist)
                {
                    Showlist.Add(new ShowItem(1, m_robot.serial));
                    robot_num++;
                }
            }
            NewShowItem();
            HomeTimer.Enabled = true;
            HomeTimer.Interval = 500;
        }

        private void HomeTimer_Tick(object sender, EventArgs e)
        {
            if (this.Visible && MainForm.InitializeComponentFinish)
            {
                foreach (ShowItem m_Item in Showlist)
                {
                    m_Item.UpdataShowItem();
                }
            }
        }

        private void NewShowItem()
        {
            int showitem_minWidth = 60;//图片宽度最小界限

            int showitem_Width = 100;//图片初始宽度
            int showitem_Height = 120;//图片初始高度
            int showitem_Left = 50;
            int showitem_Top = 80;
            int offsetLeft = 0; 

            float Width = (float)MainForm.cnclist.Count / 2;
            int int_Width = MainForm.cnclist.Count / 2;
            if (Width > int_Width)//奇数
            {
                int_Width++;
            }
            if (int_Width == 0)
            {
                int_Width = robot_num;
            }
            if (int_Width != 0)
            {
                hOffset = int_Width = (this.Width - 200) / (int_Width + 1); //2015.11.28 +1
                while (int_Width - 10 < showitem_Width && showitem_Width > showitem_minWidth)
                {
                    showitem_Width -= 5;
                    showitem_Height = (int)(showitem_Width * 1.2);
                }
                showitem_Left = 0;
                int index = 0;
                foreach (ShowItem m_Item in Showlist)
                {
                    if (m_Item.m_showtype == 0)//cnc
                    {
                        if (index % 2 == 0)
                        {
                            showitem_Top = 80;
                            showitem_Left = int_Width * (index / 2);
                        }
                        else
                        {
                            showitem_Top = this.Height - showitem_Height - 80;
                        }
                        offsetLeft = int_Width;  //2015.11.28
                    }
                    else if (m_Item.m_showtype == 1)//机器人
                    {
                        showitem_Left = int_Width * (index - MainForm.cnclist.Count);
                        showitem_Top = this.Height / 2 - showitem_Height / 2;
                        offsetLeft = 0; //2015.11.28
                    }
                    else if (m_Item.m_showtype == 2)//PLC
                    {
                    }

                    m_Item.ShowItem2window(this, showitem_Width, showitem_Height, showitem_Left + offsetLeft, showitem_Top, index);
                    index++;
                }
            }
        }
    }      
}


