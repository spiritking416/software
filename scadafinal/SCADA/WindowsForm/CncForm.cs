using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LineDevice;
using HNCAPI;

namespace SCADA
{
    public partial class CncForm : Form
    {
        public static IntPtr CncFormtr;
        CNC cnc;
        private UserControlParaman ctrlParaman = new UserControlParaman();//NC参数表对象
        bool UpDataCNCId_PROGGCUpdata = true;//cnc设备选择变化时刷新RegVave
        string OldPROGNAME = "";//记住上一次执行的G代码名称
        int RegValeShowType = 0;//寄存器值显示方式：二进制、十进制、十六进制
        int reg_num_x = 0, reg_num_y = 0, reg_num_f = 0, reg_num_g = 0, reg_num_r = 0, reg_num_b = 0;
        public static int tab_index = 0;

        Color[] Bt_bgcoler = { System.Drawing.Color.FromArgb(255, 251, 240), System.Drawing.Color.FromArgb(0, 255, 0) };//按钮颜色
        //         AutoSizeFormClass asc = new AutoSizeFormClass();
        string[] State_str = { "运行", "空闲", "报警", "离线" };//状态字符串

        public CncForm()
        {
            InitializeComponent();
            CncForm.CheckForIllegalCrossThreadCalls = false;
            comboBoxCNC.Items.Clear();
            foreach (CNC m_cnc in MainForm.cnclist)
            {
                string str = "CNC:";
                str += m_cnc.JiTaiHao;
                comboBoxCNC.Items.Add(str);
            }
            dataGridView_REG.ReadOnly = true;
            radioButtonRegValBit.Checked = true;


            rbtnCurrentAlarmCNC.Checked = true;
            panelCNCParam.Controls.Clear();
            panelCNCParam.Controls.Add(ctrlParaman);
        }
//         public System.Threading.AutoResetEvent Get_Reg_threaFucEvent = new System.Threading.AutoResetEvent(true);
        bool threaFucRuningF = true;
//         bool threaFucRuningF_OK = false;

        public static EventHandler<int> DuanDaoEven;

        private void UpDataUIThreaFuc()
        {
            while (threaFucRuningF)
            {
                if (UpDataUIThreaFucDogTimer == null)
                {
//                     UpDataUIThreaFucDogTimer = new System.Timers.Timer(2000);
//                     UpDataUIThreaFucDogTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.UpDataUIThreaFucDogTimerFuc);
//                     UpDataUIThreaFucDogTimer.AutoReset = false;
//                     UpDataUIThreaFucDogTimer.Enabled = true;
                }
                else
                {
//                     if (UpDataUIThreaFucDogTimer != null)
//                     {
//                         UpDataUIThreaFucDogTimer.Interval = 2000;
//                     }
                }

                if (/*threaFucRuningF_OK &&*/ MainForm.InitializeComponentFinish)
                {
                    try
                    {
                        if (cnc.HCNCShareData.sysData.isConnect)
                        {
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 1");
                            Updata_Reg_Value(cnc);
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 2");
                            Updata_NCStandardPanel_Value(cnc);
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 3");
                            Updatacncstate_1s(cnc);
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 4");
                            ThreaSetLaBText(labelLinckText, MainForm.LinckedText);
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 5");
                            ThreaSetPictureBoxBackColor(pictureBoxLinckState, SCADA.Properties.Resources.top_bar_green);
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 6");
                        }
                        else
                        {
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 7");
                            ThreaSetLaBText(labelLinckText, MainForm.UnLinckedText);
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 8");

                            ThreaSetPictureBoxBackColor(pictureBoxLinckState, SCADA.Properties.Resources.top_bar_black);
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 9");

                            if (labelLinckText.Text != MainForm.LinckedText)
                            {
                                LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 10");

                                ClearAllCNCstate();
                                LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 11");

                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    System.Threading.Thread.Sleep(100);
                    if (!this.Visible)
                    {
                        threaFucRuningF = false;
                        LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "UpDataUIThreaFuc 12");
//                         threaFucRuningF_OK = false;
                    }
                }
                else
                {
//                     Get_Reg_threaFucEvent.WaitOne();
                }


            }
            LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
            SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_CNC;
            SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
            SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
            SendParm.Keywords = "UI刷新线程退出";
            SendParm.EventData = "";
            SCADA.MainForm.m_Log.EventHandlershanshu(this, SendParm);

        }

        System.Timers.Timer UpDataUIThreaFucDogTimer = null;

        private void UpDataUIThreaFucDogTimerFuc(object oj,System.Timers.ElapsedEventArgs e)
        {
            if (threaFucRuningF && UpDataUIThreaHander.ThreadState == System.Threading.ThreadState.Running)
            {
                UpDataUIThreaHander.Abort();
                UpDataUIThreaHander.Join();
                StarUpDataUIThreaFuc();
                LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_CNC;
                SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                SendParm.Keywords = "CNC界面看门狗超时";
                SendParm.EventData = "看门狗超时！";
                SCADA.MainForm.m_Log.EventHandlershanshu(this, SendParm);
            }
            UpDataUIThreaFucDogTimer = null;
        }

        /// <summary>
        /// 委托设置文本控件显示
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="str"></param>
        private void ThreaSetLaBText(Label LB, String str)
        {
            if (LB.Text != str && threaFucRuningF)
            {
                if (LB.InvokeRequired && threaFucRuningF)//等待异步
                {
                    // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                    Action<string> actionDelegate = (x) => { LB.Text = x; };
//                     LB.Invoke(actionDelegate, str);
                    LB.BeginInvoke(actionDelegate, str);
                }
                else if (threaFucRuningF)
                {
                    LB.Text = str;
                }
            }
        }
        /// <summary>
        /// 委托设置文本控件显示
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="str"></param>
        private void ThreaSetLaBText(RichTextBox RLB, String str)
        {
            if (RLB.Text != str && threaFucRuningF)
            {
                if (RLB.InvokeRequired && threaFucRuningF)//等待异步
                {
                    // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                    Action<string> actionDelegate = (x) => { RLB.Text = x; };
//                     RLB.Invoke(actionDelegate, str);
                    RLB.BeginInvoke(actionDelegate, str);
                }
                else if (threaFucRuningF)
                {
                    RLB.Text = str;
                }
            }
        }

        ushort serial = 10000;
        private void comboBoxCNC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serial != MainForm.cnclist[comboBoxCNC.SelectedIndex].serial)
            {
                cnc = MainForm.cnclist[comboBoxCNC.SelectedIndex];
                tab_index = comboBoxCNC.SelectedIndex;
                ClearAllCNCstate();
                if (cnc.isConnected())
                {
                    InitdataGridView_REG(cnc);//初始寄存器列表
                    ctrlParaman.InitParamanTreeView(cnc.HCNCShareData.sysData.clientNo);          //读取参数结构
                }
                UpDataCNCId_PROGGCUpdata = true;
                serial = cnc.serial;
                txtIP.Text = cnc.ip;//IP
                textPoort.Text = cnc.port.ToString();
                textGongXu.Text = cnc.OP_CODE;

                if (SCADA.CncForm.DuanDaoEven != null)
                {
                    if (cnc.BDuanDaoBaojing_Old == 2)
                    {
                        SCADA.CncForm.DuanDaoEven.BeginInvoke(cnc, 0, null, null);
                    }
                    else if (cnc.BDuanDaoBaojing_Old == 1)
                    {
                        SCADA.CncForm.DuanDaoEven.BeginInvoke(cnc, 1, null, null);
                    }
                }
                
            }
        }


        private void ClearAllCNCstate()
        {
            HNC_SYS_NCK_VER.Text = "";//版本
            HNC_SYS_DRV_VER.Text = "";//版本
            HNC_SYS_PLC_VER.Text = "";//版本
            HNC_SYS_CNC_VER.Text = "";//版本
            HNC_SYS_NC_VER.Text = "";//版本
            txtCount.Text = "";//加工数量
            labCurToolNo.Text = "";//当前刀号

            pictureBoxLinckState.Image = SCADA.Properties.Resources.top_bar_black;//连接状态
            ThreaSetLaBText(labelLinckText, MainForm.UnLinckedText);

            labPROGNAME.Text = "";//G代码名

            richTextBoxCurrentProgramRun.Text = "";
            label_acpos_x.Text = "";
            label_acpos_y.Text = "";
            label_acpos_z.Text = "";
            label_acpos_c.Text = "";
            label_cmdpos_x.Text = "";
            label_cmdpos_y.Text = "";
            label_cmdpos_z.Text = "";
            label_cmdpos_c.Text = "";
            label_aci_x.Text = "";
            label_aci_y.Text = "";
            label_aci_z.Text = "";
            label_aci_c.Text = "";
            labFSpeed.Text = "";
            labCurToolNo.Text = "";
            labSpdlSpeed.Text = "";
            labCurLineNo.Text = "-1";
            labFSpeedRate.Text = "";
            labSpdlRate.Text = "";
            labRapidRate.Text = "";
            labPROGNAME.Text = "";
            txtRunProc.Text = "";
            progressBar_ruti_x.Value = 0;
            progressBar_ruti_y.Value = 0;
            progressBar_ruti_z.Value = 0;
            progressBar_ruti_c.Value = 0;
            richTextBoxCurrentProgramRun.Text = "";
            UpdataButtonShow();
            //             dataGridView_REG.Rows.Clear();
            ctrlParaman.ClearAlldata();
        }

        /// <summary>
        /// 1s刷新一次状态
        /// </summary>
        /// <param name="cnctmp">选中的cnc设备</param>
        System.Data.DataTable dataGridViewalarmdata_DataSource;//= new System.Data.DataTable();
        private void Updatacncstate_1s(CNC cnctmp)
        {
            if (cnc.isConnected())
            {
                ThreaSetLaBText(txtIP, cnctmp.ip);
                ThreaSetLaBText(HNC_SYS_NCK_VER, "NCK版本：" + cnctmp.HCNCShareData.sysData.sysver.ncu);
                ThreaSetLaBText(HNC_SYS_DRV_VER, "DRV版本：" + cnctmp.HCNCShareData.sysData.sysver.drv);
                ThreaSetLaBText(HNC_SYS_PLC_VER, "PLC版本：" + cnctmp.HCNCShareData.sysData.sysver.plc);
                ThreaSetLaBText(HNC_SYS_CNC_VER, "CNC版本：" + cnctmp.HCNCShareData.sysData.sysver.cnc);
                ThreaSetLaBText(HNC_SYS_NC_VER, "NC版本：" + cnctmp.HCNCShareData.sysData.sysver.nc);

                ThreaSetLaBText(txtCount, cnctmp.get_partNum().ToString());
                ThreaSetLaBText(labCurToolNo, cnctmp.get_toolUse().ToString("D4"));
                //                 string str = "";
                //                 CNC.CNCState state = cnctmp.Checkcnc_state();
                //                 if (state == CNC.CNCState.DISCON)
                //                 {
                //                     str = State_str[3];
                //                 }
                //                 else if (state == CNC.CNCState.IDLE)
                //                 {
                //                     str = State_str[1];
                //                 }
                //                 else if (state == CNC.CNCState.ALARM)
                //                 {
                //                     str = State_str[2];
                //                 }
                //                 else if (state == CNC.CNCState.RUNING)
                //                 {
                //                     str = State_str[0];
                //                 }

                OldPROGNAME = labPROGNAME.Text;
                ThreaSetLaBText(labPROGNAME, cnctmp.get_gCodeName());

                string[] strgcodename;
                if (cnctmp.get_gCodeName() != null && cnctmp.get_gCodeName().Length > 0)
                {
                    strgcodename = cnctmp.get_gCodeName().Split('/');
                    ThreaSetLaBText(txtRunProc, strgcodename[strgcodename.Length - 1]);
                }
                else
                {
                    ThreaSetLaBText(txtRunProc, "");
                }

                if (UpDataCNCId_PROGGCUpdata == true || OldPROGNAME != cnctmp.get_gCodeName())//重新上载G代码文件并显示
                {
                    if (labPROGNAME.Text != "N/A" && labPROGNAME.Text != "")
                    {
                        LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "cnctmp.netFileGet 1");
                        if (cnctmp.netFileGet(".\\cncgc.nc", labPROGNAME.Text) == 0)
                        {
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "cnctmp.netFileGet 2");
                            AppendMessage(".\\cncgc.nc", int.Parse(labCurLineNo.Text));
                        }
                        else
                        {
                            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "cnctmp.netFileGet 3");
                            ThreaSetLaBText(richTextBoxCurrentProgramRun, "");
                        }
                        LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "cnctmp.netFileGet 4");
                    }
                    else
                    {
                        ThreaSetLaBText(richTextBoxCurrentProgramRun, "");
                    }
                    UpDataCNCId_PROGGCUpdata = false;
                }
                UpdataButtonShow();
                if (rbtnCurrentAlarmCNC.Checked)
                {
                    cnc.UpAlarmShowDataTable(ref dataGridViewalarmdata_DataSource);
                }
                else
                {
                    cnc.UpHisAlarmShowDataTable(ref dataGridViewalarmdata_DataSource);
                }
                ThreadGridEdet(dataGridViewalarmdata);
            }
        }

        private delegate void DelSetDvSource(DataGridView dgvEx);
        private void ThreadGridEdet(DataGridView dgvEx)
        {
            if (dgvEx.InvokeRequired && threaFucRuningF)
            {
                DelSetDvSource d = new DelSetDvSource(ThreadGridEdet);
//                 this.Invoke(d, new object[] { dgvEx });
                this.BeginInvoke(d, new object[] { dgvEx });
            }
            else if (threaFucRuningF)
            {
                try
                {
                    if (dgvEx.Rows.Count != dataGridViewalarmdata_DataSource.Rows.Count)
                    {
                        dgvEx.RowCount = dataGridViewalarmdata_DataSource.Rows.Count;
                    }
                    for (int ii = 0; ii < dgvEx.Rows.Count; ii++)
                    {
                        if (dgvEx.Rows[ii].Displayed)
                        {
                            for (int jj = 0; jj < dataGridViewalarmdata_DataSource.Columns.Count; jj++)
                            {
                                if (dgvEx.Rows[ii].Cells[jj].Value != dataGridViewalarmdata_DataSource.Rows[ii][jj])
                                {
                                    dgvEx.Rows[ii].Cells[jj].Value = dataGridViewalarmdata_DataSource.Rows[ii][jj];
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void ThreaSetPictureBoxBackColor(PictureBox PB, Bitmap BT)
        {
            if (PB.Image != BT && PB != null && threaFucRuningF)
            {
                if (PB.InvokeRequired && threaFucRuningF)//等待异步
                {
                    // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                    Action<Bitmap> actionDelegate = (x) =>
                    {
                        if (PB.Image != x && threaFucRuningF)
                        {
                            PB.Image = x;
                        }
                    };
//                     PB.Invoke(actionDelegate, BT);
                    PB.BeginInvoke(actionDelegate, BT);
                }
                else if (threaFucRuningF)
                {
                    if (PB.Image != BT)
                    {
                        PB.Image = BT;
                    }
                }
            }
        }

        private void btnBackColorSet(ref Button btn, int flag)
        {
            if (flag != 0)
            {
                flag = 1;
            }
            if (btn.BackColor != Bt_bgcoler[flag])
            {
                btn.BackColor = Bt_bgcoler[flag];
            }
        }

        /// <summary>
        /// 更新运行界面按钮显示状态
        /// </summary>
        private void UpdataButtonShow()
        {
            if (btnAuto.Visible && cnc != null)
            {
                //                 if (cnc.yreg_val_Arr.UpDataBegin != 480)
                //                 {
                //                     cnc.yreg_val_Arr.UpDataBegin = 480;
                //                 }
                //                 if (cnc.yreg_val_Arr.UpDataEnd != 486)
                //                 {
                //                     cnc.yreg_val_Arr.UpDataEnd = 486;
                //                 }
                int tmp = 0;
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_Y, 480, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnAuto, tmp & 0x0001);//自动,Y480.0
                    btnBackColorSet(ref btnSingleSegmentCNC, tmp & 0x0002);//单段,Y480.1
                    btnBackColorSet(ref btnManualCNC, tmp & 0x0004);//手动,Y480.2
                    btnBackColorSet(ref btnRunIncrementCNC, tmp & 0x0008);//增量,Y480.3
                    btnBackColorSet(ref btnReZeroCNC, tmp & 0x0010);//回参考点,Y480.4
                    btnBackColorSet(ref btnhuandaozhuangdaoCNC, tmp & 0x0020);//换刀允许,Y480.5
                    btnBackColorSet(ref btndaosongjingCNC, tmp & 0x0040);//刀具松紧,Y480.6
                    btnBackColorSet(ref btnkongrunCNC, tmp & 0x0080);//空运行,Y480.7
                }
                tmp = 0;
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_Y, 481, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnGCSkipCNC, tmp & 0x0001);//程序跳段,Y481.0
                    btnBackColorSet(ref btnxuanzhetingCNC, tmp & 0x0002);//选择停,Y481.1
                    btnBackColorSet(ref btnZsuoCNC, tmp & 0x0004);//Z轴锁住,Y481.2
                    btnBackColorSet(ref btnjichuangshuoCNC, tmp & 0x0008);//机床锁住,Y481.3
                    btnBackColorSet(ref btnProtectDoorCNC, tmp & 0x0010);//防护门,Y481.4
                    btnBackColorSet(ref btnzhaomingCNC, tmp & 0x0020);//机床照明,Y481.5
                    btnBackColorSet(ref btnToolzhengzhuan, tmp & 0x0040);//刀库正转,Y481.6
                    btnBackColorSet(ref btnToolfanzhuan, tmp & 0x0080);//刀库反转,Y481.7
                }
                tmp = 0;
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_Y, 482, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnaddACNC, tmp & 0x0001);//+A,Y482.0
                    btnBackColorSet(ref btnaddzCNC, tmp & 0x0002);//+Z,Y482.1
                    btnBackColorSet(ref btnsubyCNC, tmp & 0x0004);//-Y,Y482.2
                    btnBackColorSet(ref btnF1CNC, tmp & 0x0008);//新F1,Y482.3
                    btnBackColorSet(ref btnF2CNC, tmp & 0x0010);//x10,Y482.4
                    btnBackColorSet(ref btnF3CNC, tmp & 0x0020);//x100,Y482.5
                    btnBackColorSet(ref btnF4CNC, tmp & 0x0040);//x1000,Y482.6
                    btnBackColorSet(ref btnF1, tmp & 0x0080);//F1,Y482.6
                }
                tmp = 0;
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_Y, 483, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnhuandaozhuangdaoCNC, tmp & 0x0001);//换刀/装刀,Y483.0
                    btnBackColorSet(ref btnaddxCNC, tmp & 0x0002);//+X,Y483.1
                    btnBackColorSet(ref btnkuaijingCNC, tmp & 0x0004);//快进,Y483.2
                    btnBackColorSet(ref btnsubxCNC, tmp & 0x0008);//-X,Y483.3
                    btnBackColorSet(ref btnzhuzhoudingxiangCNC, tmp & 0x0010);//主轴定向,Y483.4
                    btnBackColorSet(ref btnshouyaoshiqie, tmp & 0x0020);//手摇试切,Y483.5
                    btnBackColorSet(ref btnChongxue, tmp & 0x0040);//冲屑,Y483.6
                    btnBackColorSet(ref btnCoolingCNC, tmp & 0x0080);//冷却,Y483.7
                }
                tmp = 0;
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_Y, 484, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnToolHome, tmp & 0x0001);//刀库回零,Y484.0
                    btnBackColorSet(ref button3AxsHome, tmp & 0x0002);//三轴回零,Y484.0
                    btnBackColorSet(ref btnaddyCNC, tmp & 0x0004);//+Y,Y484.2
                    btnBackColorSet(ref btnsubzCNC, tmp & 0x0008);//-Z,Y484.3
                    btnBackColorSet(ref btnsubACNC, tmp & 0x0010);//-A,Y484.4
                    btnBackColorSet(ref btnSpindleForwardCNC, tmp & 0x0020);//主轴正转,Y484.5
                    btnBackColorSet(ref btnSpindleStopCNC, tmp & 0x0040);//主轴停止,Y484.6
                    btnBackColorSet(ref btnzhuzhoufanzhuangCNC, tmp & 0x0080);//主轴反转,Y484.7
                }
                tmp = 0;
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_Y, 485, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnLubricationCNC, tmp & 0x0001);//润滑,Y485.0
                    btnBackColorSet(ref btnchaochenjiechuCNC, tmp & 0x0004);//解除超程,Y485.2
                    btnBackColorSet(ref btnjichuangshuoCNC, tmp & 0x0008);//机床锁住,Y485.3
                    btnBackColorSet(ref btnProtectDoorCNC, tmp & 0x0010);//防护门,Y485.4
                    btnBackColorSet(ref btnzhaomingCNC, tmp & 0x0020);//机床照明,Y485.5
                    btnBackColorSet(ref btnToolzhengzhuan, tmp & 0x0040);//进给保持2,Y485.6
                    btnBackColorSet(ref btnToolTest, tmp & 0x0080);//手动换刀,Y485.7
                }
                tmp = 0;
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_Y, 486, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnxhqdCNC, tmp & 0x0010);//循环启动,Y486.4
                    btnBackColorSet(ref btnFeedHoldRunCNC, tmp & 0x0020);//进给保持,Y486.5
                }

                //                 if (cnc.rreg_val_Arr.UpDataBegin != 29)
                //                 {
                //                     cnc.rreg_val_Arr.UpDataBegin = 29;
                //                 }
                //                 if (cnc.rreg_val_Arr.UpDataEnd != 29)
                //                 {
                //                     cnc.rreg_val_Arr.UpDataEnd = 29;
                //                 }
                //                 tmp = cnc.rreg_val_Arr.reg_val_arr[29];
                if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)HncRegType.REG_TYPE_R, 29, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
                {
                    btnBackColorSet(ref btnEStopCNC, tmp & 0x0010);//急停,R29.4
                }
            }
        }



        delegate void richTextBoxCurrentProgramRunAppendCallback(string name, int lineNum);
        public void AppendMessage(string name, int lineNum)
        {
            if (richTextBoxCurrentProgramRun.InvokeRequired)
            {
                richTextBoxCurrentProgramRunAppendCallback d = new richTextBoxCurrentProgramRunAppendCallback(AppendMessage);
//                 richTextBoxCurrentProgramRun.Invoke(d, new object[] { name, lineNum });
                richTextBoxCurrentProgramRun.BeginInvoke(d, new object[] { name, lineNum });
            }
            else
            {
                try
                {
                    UpdataGCodeShow(name, lineNum);
                }
                catch (System.Exception ex)
                {
                    LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.System_runing;
                    SendParm.LevelIndex = (int)LogData.Node2Level.ERROR;
                    SendParm.EventID = ((int)LogData.Node2Level.ERROR).ToString();
                    SendParm.Keywords = "G代码显示错误";
                    SendParm.EventData = ex.ToString();
                    //                     SendParm.Provider = "MainForm";
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(null, SendParm, null, null);
                }
            }
        }


        /// <summary>
        /// 更新G代码显示
        /// </summary>
        /// <param name="name">G代码文件名</param>
        /// <param name="lineNum">执行到第几行了</param>
        ///                     static int up_stari = 0;
        int up_stari = 0;
        int up_endi = 0;
        private void UpdataGCodeShow(string name, int lineNum)
        {
            string str = "";
            if (name.Length != 0)
            {
                ThreaSetLaBText(richTextBoxCurrentProgramRun, "");
                System.IO.FileStream f = new System.IO.FileStream(name, System.IO.FileMode.Open);
                System.IO.StreamReader reader = new System.IO.StreamReader(f, Encoding.Default);
                int lineID = 0;
                while (!reader.EndOfStream)
                {
                    str = "N";
                    str += lineID.ToString("D4");
                    str += "   ";
                    str += reader.ReadLine().ToString();
                    str += Environment.NewLine;
                    lineID++;
                    richTextBoxCurrentProgramRun.AppendText(str);
                }
                reader.Close();
                f.Close();
            }

            if (richTextBoxCurrentProgramRun.Text.Length > 0)
            {
                str = "";
                str += "N";
                str += lineNum.ToString("D4");
                str += "   ";
                int stari = richTextBoxCurrentProgramRun.Text.IndexOf(str);
                lineNum++;
                str = "";
                str += "N";
                str += lineNum.ToString("D4");
                str += "   ";
                int endi = richTextBoxCurrentProgramRun.Text.IndexOf(str);
                if (stari != -1)
                {
                    if (endi != -1)//最后一行
                    {
                        endi = endi - stari;
                    }
                    else
                    {
                        endi = richTextBoxCurrentProgramRun.Text.Length - stari;
                    }
                    richTextBoxCurrentProgramRun.Select(up_stari, up_endi);
                    richTextBoxCurrentProgramRun.SelectionBackColor = richTextBoxCurrentProgramRun.BackColor;
                    richTextBoxCurrentProgramRun.Select(stari, endi);
                    richTextBoxCurrentProgramRun.SelectionBackColor = Color.BurlyWood;
                    //                     richTextBoxCurrentProgramRun.ScrollToCaret();//此行导致程序崩溃
                    up_stari = stari;
                    up_endi = endi;
                }
            }
        }


        private void Updata_NCStandardPanel_Value(CNC cnctmp)
        {
            if (labFSpeed.Visible)
            {
                ThreaSetLaBText(labFSpeed, cnctmp.get_act_feedrate().ToString("F1"));
                ThreaSetLaBText(labSpdlSpeed, cnctmp.get_act_spdl_speed().ToString("F1"));
                ThreaSetLaBText(labFSpeedRate, cnctmp.get_feed_override().ToString("D3") + "%");
                ThreaSetLaBText(labRapidRate, cnctmp.get_rapid_override().ToString("D3") + "%");
                ThreaSetLaBText(labSpdlRate, cnctmp.get_spdl_override().ToString("D3") + "%");
                int gcrunningline = (int)cnctmp.get_run_row();
                int gOldline = 0;
                if (int.TryParse(labCurLineNo.Text, out gOldline) && gcrunningline != gOldline)
                {
                    ThreaSetLaBText(labCurLineNo, gcrunningline.ToString("D4"));
                    UpdataGCodeShow("", gcrunningline);
                }
                int axis = 0;
                for (int i = 0; i < cnc.HCNCShareData.axlist.Count; i++)
                {
                    axis = cnc.HCNCShareData.axlist[i].axisNo;
                    double cmdData = 0, wcsData = 0, cur_i = 0, eding_i = 0;
                    int moveunit = 100000;
                    cmdData = cnctmp.get_axis_act_pos(axis);
                    wcsData = cnctmp.get_axis_cmd_pos(axis);
                    cur_i = cnctmp.get_axis_rated_cur(axis);
                    eding_i = cnctmp.get_axis_load_cur(axis);
                    int cur_i_show = 0;
                    if (eding_i != 0)
                    {
                        cur_i_show = (int)((eding_i / cur_i) * 100);
                        if (cur_i_show < 0)
                        {
                            cur_i_show = 0;
                        }
                        else if (cur_i_show > 150)
                        {
                            cur_i_show = 150;
                        }
                    }
                    //moveunit = cnctmp.get_sys_move_unit();
                    if (axis == 0)
                    {
                        ThreaSetLaBText(label_acpos_x, ((double)cmdData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_cmdpos_x, ((double)wcsData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_aci_x, cur_i_show.ToString() + "%");

                        if (cur_i_show <= 50)
                        {
                            //progressBar_ruti_x.bacColor = Color.Gray;
                        }
                        else if (cur_i_show <= 100)
                        {
                            //progressBar_ruti_x.ForeColor = Color.Green;
                        }
                        else
                        {
                            //progressBar_ruti_x.ForeColor = Color.Red;
                        }
                        progressBar_ruti_x.Value = cur_i_show;

                    }
                    else if (axis == 1)
                    {
                        ThreaSetLaBText(label_acpos_y, ((double)cmdData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_cmdpos_y, ((double)wcsData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_aci_y, cur_i_show.ToString() + "%");

                        progressBar_ruti_y.Value = cur_i_show;
                    }
                    else if (axis == 2)
                    {
                        ThreaSetLaBText(label_acpos_z, ((double)cmdData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_cmdpos_z, ((double)wcsData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_aci_z, cur_i_show.ToString() + "%");

                        progressBar_ruti_z.Value = cur_i_show;
                    }
                    else if (axis == 5)
                    {
                        ThreaSetLaBText(label_acpos_c, ((double)cmdData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_cmdpos_c, ((double)wcsData / moveunit).ToString("F3"));
                        ThreaSetLaBText(label_aci_c, cur_i_show.ToString() + "%");

                        progressBar_ruti_c.Value = cur_i_show;
                    }
                }
            }
        }
        /// <summary>
        /// 初始化dataGridView_REG列表
        /// </summary>
        /// <param name="cnctmp"></param>
        private void InitdataGridView_REG(CNC cnctmp)
        {
            int index, tmp = 0;
            reg_num_x = cnctmp.get_reg_num((int)HncRegType.REG_TYPE_X);
            reg_num_y = cnctmp.get_reg_num((int)HncRegType.REG_TYPE_Y);
            reg_num_f = cnctmp.get_reg_num((int)HncRegType.REG_TYPE_F);
            reg_num_g = cnctmp.get_reg_num((int)HncRegType.REG_TYPE_G);
            reg_num_r = cnctmp.get_reg_num((int)HncRegType.REG_TYPE_R);
            reg_num_b = cnctmp.get_reg_num((int)HncRegType.REG_TYPE_B);
            tmp = (reg_num_x > reg_num_y) ? reg_num_x : reg_num_y;
            tmp = (tmp > reg_num_f) ? tmp : reg_num_f;
            tmp = (tmp > reg_num_g) ? tmp : reg_num_g;
            tmp = (tmp > reg_num_r) ? tmp : reg_num_r;
            tmp = (tmp > reg_num_b) ? tmp : reg_num_b;
            if (tmp > dataGridView_REG.RowCount)
            {
                for (index = dataGridView_REG.RowCount; index < tmp; index++)
                {
                    int ii = dataGridView_REG.Rows.Add();
                    dataGridView_REG.Rows[ii].Cells[0].Value = ii;
                }
            }
            else if (tmp < dataGridView_REG.RowCount)
            {
                for (index = dataGridView_REG.RowCount - 1; index >= tmp; index--)
                {
                    dataGridView_REG.Rows.RemoveAt(index);
                }
            }
        }

        //         private void UpDatacncRegIndex()
        //         {
        //             if (cnc != null)
        //             {
        //                 if (dataGridView_REG.Visible)
        //                 {
        //                     int dgvminrow = dataGridView_REG.Rows.GetFirstRow(DataGridViewElementStates.Displayed);//显示在屏幕上的第一行
        //                     int dgvmaxrow = dataGridView_REG.Rows.GetLastRow(DataGridViewElementStates.Displayed);//显示在屏幕上的最后一行
        //                     if (dgvminrow < cnc.xreg_val_Arr.Lenth && dgvminrow != cnc.xreg_val_Arr.UpDataBegin)
        //                     {
        //                         cnc.xreg_val_Arr.UpDataBegin = dgvminrow;
        //                     }
        //                     if (dgvmaxrow < cnc.xreg_val_Arr.Lenth && dgvmaxrow != cnc.xreg_val_Arr.UpDataEnd)
        //                     {
        //                         cnc.xreg_val_Arr.UpDataEnd = dgvmaxrow;
        //                     }
        // 
        //                     if (dgvminrow < cnc.yreg_val_Arr.Lenth && dgvminrow != cnc.yreg_val_Arr.UpDataBegin)
        //                     {
        //                         cnc.yreg_val_Arr.UpDataBegin = dgvminrow;
        //                     }
        //                     if (dgvmaxrow < cnc.yreg_val_Arr.Lenth && dgvmaxrow != cnc.yreg_val_Arr.UpDataEnd)
        //                     {
        //                         cnc.yreg_val_Arr.UpDataEnd = dgvmaxrow;
        //                     }
        // 
        //                     if (dgvminrow < cnc.rreg_val_Arr.Lenth && dgvminrow != cnc.rreg_val_Arr.UpDataBegin)
        //                     {
        //                         cnc.rreg_val_Arr.UpDataBegin = dgvminrow;
        //                     }
        //                     if (dgvmaxrow < cnc.rreg_val_Arr.Lenth && dgvmaxrow != cnc.rreg_val_Arr.UpDataEnd)
        //                     {
        //                         cnc.rreg_val_Arr.UpDataEnd = dgvmaxrow;
        //                     }
        // 
        //                     if (dgvminrow < cnc.freg_val_Arr.Lenth && dgvminrow != cnc.freg_val_Arr.UpDataBegin)
        //                     {
        //                         cnc.freg_val_Arr.UpDataBegin = dgvminrow;
        //                     }
        //                     if (dgvmaxrow < cnc.freg_val_Arr.Lenth && dgvmaxrow != cnc.freg_val_Arr.UpDataEnd)
        //                     {
        //                         cnc.freg_val_Arr.UpDataEnd = dgvmaxrow;
        //                     }
        // 
        //                     if (dgvminrow < cnc.greg_val_Arr.Lenth && dgvminrow != cnc.greg_val_Arr.UpDataBegin)
        //                     {
        //                         cnc.greg_val_Arr.UpDataBegin = dgvminrow;
        //                     }
        //                     if (dgvmaxrow < cnc.greg_val_Arr.Lenth && dgvmaxrow != cnc.greg_val_Arr.UpDataEnd)
        //                     {
        //                         cnc.greg_val_Arr.UpDataEnd = dgvmaxrow;
        //                     }
        // 
        //                     if (dgvminrow < cnc.breg_val_Arr.Lenth && dgvminrow != cnc.breg_val_Arr.UpDataBegin)
        //                     {
        //                         cnc.breg_val_Arr.UpDataBegin = dgvminrow;
        //                     }
        //                     if (dgvmaxrow < cnc.breg_val_Arr.Lenth && dgvmaxrow != cnc.breg_val_Arr.UpDataEnd)
        //                     {
        //                         cnc.breg_val_Arr.UpDataEnd = dgvmaxrow;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     if (!cnc.threaFucRuningF_OK)
        //                     {
        //                         cnc.threaFucRuningF_OK = false;
        //                     }
        //                 }
        //             }
        // 
        //         }

        /// <summary>
        /// 刷新寄存器
        /// </summary>
        /// <param name="cnctmp"></param> 
        private void Updata_Reg_Value(CNC cnctmp)
        {
            if (dataGridView_REG.Visible)
            {
                int index_1, index_2;
                int dgvminrow = dataGridView_REG.Rows.GetFirstRow(DataGridViewElementStates.Displayed);//显示在屏幕上的第一行
                int dgvmaxrow = dataGridView_REG.Rows.GetLastRow(DataGridViewElementStates.Displayed);//显示在屏幕上的最后一行

                if (dgvminrow < 0 || dgvminrow > dgvmaxrow || dgvmaxrow >= dataGridView_REG.Rows.Count)
                {
                    return;
                }
                for (index_1 = dgvminrow; index_1 < dgvmaxrow; index_1++)
                {
                    for (index_2 = 1; index_2 < dataGridView_REG.ColumnCount; index_2++)
                    {
                        HncRegType Reg_type = HncRegType.REG_TYPE_X;
                        if (index_2 == 1)
                        {
                            if (index_1 >= reg_num_x)
                            {
                                continue;
                            }
                        }
                        else if (index_2 == 2)
                        {
                            if (index_1 >= reg_num_y)
                            {
                                continue;
                            }
                            Reg_type = HncRegType.REG_TYPE_Y;
                        }
                        else if (index_2 == 3)
                        {
                            if (index_1 >= reg_num_f)
                            {
                                continue;
                            }
                            Reg_type = HncRegType.REG_TYPE_F;
                        }
                        else if (index_2 == 4)
                        {
                            if (index_1 >= reg_num_g)
                            {
                                continue;
                            }
                            Reg_type = HncRegType.REG_TYPE_G;
                        }
                        else if (index_2 == 5)
                        {
                            if (index_1 >= reg_num_r)
                            {
                                continue;
                            }
                            Reg_type = HncRegType.REG_TYPE_R;
                        }
                        else if (index_2 == 6)
                        {
                            if (index_1 >= reg_num_b)
                            {
                                continue;
                            }
                            Reg_type = HncRegType.REG_TYPE_B;
                        }
                        dataGridView_REG.Rows[index_1].Cells[index_2].Value = GetRegValueChage2string(Reg_type, index_1, RegValeShowType);
                    }
                }
            }
        }

        /// <summary>
        /// 从下位机中获取到对应寄存器值，将其转换为二进制或者十进制或者十六进制
        /// 返回一个字符串
        /// </summary>
        /// <param name="Reg_type">寄存器类型</param>
        /// <param name="Ren_index">寄存器index</param>
        /// <param name="flag">0：二进制，1：十进制，2：十六进制</param>
        private string GetRegValueChage2string(HncRegType Reg_type, int Ren_index, int flag)
        {
            string str = "";
            int tmp = 0;
            uint uint_tmp = 0, shi = 0;
            if (cnc.isConnected() && Collector.CollectShare.HNC_RegGetValue((int)Reg_type, Ren_index, out tmp, cnc.HCNCShareData.sysData.clientNo) == 0)
            {
                //             cnc.get_reg_val((int)Reg_type, Ren_index, ref tmp);
                if (Reg_type == HncRegType.REG_TYPE_X || Reg_type == HncRegType.REG_TYPE_Y || Reg_type == HncRegType.REG_TYPE_R)
                {
                    tmp &= 0x00ff;                  //因为X，Y,R及R寄存器是8位的
                    uint_tmp = 7;
                    shi = 3;
                }
                else if (Reg_type == HncRegType.REG_TYPE_F || Reg_type == HncRegType.REG_TYPE_G)
                {
                    tmp &= 0xffff;                  //因为X，Y及R寄存器是16位的
                    uint_tmp = 15;
                    shi = 5;
                }
                else if (Reg_type == HncRegType.REG_TYPE_B)
                {
                    uint_tmp = (uint)tmp;
                    uint_tmp &= 0xffffffff;                  //因为B及R寄存器是32位的
                    tmp = (int)uint_tmp;
                    uint_tmp = 31;
                    shi = 10;
                }
                if (flag == 0)//二进制
                {
                    str = Convert.ToString(tmp, 2);
                    str += "B";
                    while (str.Length - 1 < uint_tmp + 1)
                        str = "0" + str;
                }
                else if (flag == 1)//十进制
                {
                    str = tmp.ToString();
                    str += "D";
                    while (str.Length - 1 < shi)
                        str = "0" + str;
                }
                else if (flag == 2)//十六进制
                {
                    str = Convert.ToString(tmp, 16);
                    str += "H";
                    while (str.Length - 1 < (uint_tmp + 1) / 4)
                        str = "0" + str;
                }
            }
            return str;
        }

        private void radioButtonRegValBit_CheckedChanged(object sender, EventArgs e)
        {
            RegValeShowType = 0;
        }

        private void radioButtonRegValDecimal_CheckedChanged(object sender, EventArgs e)
        {
            RegValeShowType = 1;
        }

        private void radioButtonRegValHex_CheckedChanged(object sender, EventArgs e)
        {
            RegValeShowType = 2;
        }


        /// <summary>
        /// 复位按钮响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetCNC_Click(object sender, EventArgs e)
        {
            //btnResetCNC.BackColor = Color.Red;
        }

        //         private delegate void FlushClient();//代理
        private void CncForm_Load(object sender, EventArgs e)
        {
            CncFormtr = this.Handle;
            DuanDaoEven = new EventHandler<int>(this.DuanDaoBaojing);
            button_Duandao.Visible = true;
            if (tab_index < 0)
            {
                tab_index = 0;
            }
            dataGridView_REG.AllowUserToAddRows = false;
            dataGridViewalarmdata.AllowUserToAddRows = false;
            dGVKnife.AllowUserToAddRows = false;
        }

        /// <summary>
        /// CNC界面刷新时响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CncForm_VisibleChanged(object sender, EventArgs e)
        {
            if (((CncForm)sender).Visible && comboBoxCNC.Items.Count > 0)
            {
                comboBoxCNC.SelectedIndex = tab_index;
            }
            StarUpDataUIThreaFuc();

//             threaFucRuningF_OK = true;
//             Get_Reg_threaFucEvent.Set();
        }
        System.Threading.Thread UpDataUIThreaHander;
        private void StarUpDataUIThreaFuc()
        {
            if (UpDataUIThreaHander == null)
            {
                UpDataUIThreaHander = new System.Threading.Thread(this.UpDataUIThreaFuc);
                UpDataUIThreaHander.Name = "UpDataUIThreaFuc";
                UpDataUIThreaHander.Priority = System.Threading.ThreadPriority.AboveNormal;
                UpDataUIThreaHander.Start();
            }
            else
            {
                if (UpDataUIThreaHander.ThreadState == System.Threading.ThreadState.Running && threaFucRuningF)
                {
                    return;
//                     UpDataUIThreaHander.Abort();
//                     UpDataUIThreaHander.Join();
                }
                if (!threaFucRuningF || UpDataUIThreaHander.ThreadState != System.Threading.ThreadState.Running)
                {
                    UpDataUIThreaHander = null;
                    threaFucRuningF = true;
                    UpDataUIThreaHander = new System.Threading.Thread(this.UpDataUIThreaFuc);
                    UpDataUIThreaHander.Start();
                }
            }
        }


        private void CncForm_SizeChanged(object sender, EventArgs e)
        {
            //              asc.controlAutoSize(this);
            this.WindowState = (System.Windows.Forms.FormWindowState)(2);
        }
        private void changeDGVCnc()
        {
            //             dataGridView_REG.Columns[0].HeaderText = Localization.Forms["CncForm"]["Column_Reg_index"];
            //             dataGridView_REG.Columns[1].HeaderText = Localization.Forms["CncForm"]["Column_RegVal_x"];
            //             dataGridView_REG.Columns[2].HeaderText = Localization.Forms["CncForm"]["Column_RegVal_y"];
            //             dataGridView_REG.Columns[3].HeaderText = Localization.Forms["CncForm"]["Column_RegVal_f"];
            //             dataGridView_REG.Columns[4].HeaderText = Localization.Forms["CncForm"]["Column_RegVal_g"];
            //             dataGridView_REG.Columns[5].HeaderText = Localization.Forms["CncForm"]["Column_RegVal_r"];
            //             dataGridView_REG.Columns[6].HeaderText = Localization.Forms["CncForm"]["Column_RegVal_b"];
            //             dataGridViewalarmdata.Columns[0].HeaderText = Localization.Forms["CncForm"]["CNCAlarmNumi"];
            //             dataGridViewalarmdata.Columns[1].HeaderText = Localization.Forms["CncForm"]["CNCAlarmNum"];
            //             dataGridViewalarmdata.Columns[2].HeaderText = Localization.Forms["CncForm"]["CNCAlarmContent"];
            //             dataGridViewalarmdata.Columns[3].HeaderText = Localization.Forms["CncForm"]["CNCAlarmReason"];

            //   dataGridViewParam.
            //      listView11.Columns[0].
        }
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MainForm.LanguageChangeMsg:
                    //                     Localization.RefreshLanguage(this);
                    //                     changeDGVCnc();
                    break;
                case MainForm.ClosingMsg:
                    threaFucRuningF = false;
//                     threaFucRuningF_OK = true;
//                     Get_Reg_threaFucEvent.Set();
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void button_Duandao_Click(object sender, EventArgs e)
        {
            if (cnc != null && cnc.HCNCShareData != null && cnc.HCNCShareData.sysData != null &&
                cnc.HCNCShareData.sysData.clientNo > -1 && cnc.HCNCShareData.sysData.clientNo < 256)
            {
                if (MessageBox.Show(MessageString.CNCForm_ToolBrokenInfo, MessageString.SetForms_Information, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Collector.CollectShare.Instance().CancelToolBroken(cnc.HCNCShareData.sysData.clientNo, true);
                }
                else
                {
                    Collector.CollectShare.Instance().CancelToolBroken(cnc.HCNCShareData.sysData.clientNo, false);
                }
            }
        }

        private void DuanDaoBaojing(object ob, int typ)
        {
            if (ob != null)
            {
                CNC m_cnc = (CNC)ob;
                if (cnc != null && m_cnc == cnc)
                {
                    switch (typ)
                    {
                        case 0:
                            if (!button_Duandao.Visible)
                            {
                                button_Duandao.Visible = true;
                            }
                            break;
                        case 1:
                            if (button_Duandao.Visible)
                            {
                                button_Duandao.Visible = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
