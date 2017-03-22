using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using Collector;

namespace SCADA
{
    public partial class RfidForm : Form
    {
        public static IntPtr rfidPtr;
//         AutoSizeFormClass asc = new AutoSizeFormClass();
        PLC.PLC_MITSUBISHI_HNC8 plc;//= new PLC.PLC_MITSUBISHI_HNC8();
        public String[] MSGTypeStrArr = { "��Ʒϵ�к�", "��д��" };
        public static String[] RFIDReadDataStruct = { "���", "ʱ��", "��Ʒ���к�01", "��Ʒ���к�02", "������", "���ӹ�����", "�Ѽӹ�����",
                                                        "����01�ӹ���Ϣ", "����02�ӹ���Ϣ", "����03�ӹ���Ϣ", "����04�ӹ���Ϣ" ,
                                                        "����05�ӹ���Ϣ", "����06�ӹ���Ϣ", "����07�ӹ���Ϣ", "����08�ӹ���Ϣ","����09�ӹ���Ϣ"};
        private String[][][] comboBoxRFIDSeletStr;
        public RfidForm()
        {
            InitializeComponent();
        }


        private void RfidForm_Load(object sender, EventArgs e)
        {
            rfidPtr = Handle;
//             asc.controllInitializeSize(this);
//             if (Localization.HasLang)
//             {
//                 Localization.RefreshLanguage(this);
//             }

//             if (MainForm.m_ShowRfidDataTable != null)
//             {
//                 MainForm.m_ShowRfidDataTable.MassgeHandler += new EventHandler<string>(this.RfidDataTableUpdataHandler);
//             }
            ////��RFID������Ϣ���ͳ���
            String[] comboBoxRFIDPLCSeleStr = new String[PLCDataShare.m_plclist.Count];
            comboBoxRFIDSeletStr = new String[PLCDataShare.m_plclist.Count][][];
            for (int ii = 0; ii < PLCDataShare.m_plclist.Count; ii++)
            {
                comboBoxRFIDPLCSeleStr[ii] = PLCDataShare.m_plclist[ii].serial.ToString() + ":" + PLCDataShare.m_plclist[ii].system;
                if (PLCDataShare.m_plclist[0].system == m_xmlDociment.PLC_System[0])
                {
                    PLCDataShare.m_plclist[0].AutoUpDataRFIDDataHandler += new PLC.PLC_MITSUBISHI_HNC8.RFIDEventHandler<PLC.MITSUBISHIPLCRFIDEvent>(this.MITSUBISHIPlcRFIDAutoUpDataHandler);
                }
                else if (PLCDataShare.m_plclist[0].system == m_xmlDociment.PLC_System[1])
                {
                    SCADA.RFID.AutoSendHandler += new SCADA.RFID.HNC8RFIDEventHandler<PLC.HNC8PLCRFIDEvent>(this.HNC8RfidDataTableUpdataHandler);
                }
                Int32 SUM = Int32.Parse(MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, -1, m_xmlDociment.Default_Attributes_str1[0]));
                String[][] Temp = new String[SUM][];
                Int32 Tempii = 0;
                for (int jj = 0; jj < SUM; jj++)
                {
                    if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, jj, m_xmlDociment.Default_Attributes_RFID[4]) == ii.ToString())
                    {
                        Temp[Tempii] = new String[(int)m_xmlDociment.Attributes_RFID.remark + 1];
                        Temp[Tempii][0] = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, jj, m_xmlDociment.Default_Path_str[8]);
                        for (int kk = 1; kk < m_xmlDociment.Default_Attributes_RFID.Length; kk++)
                        {
                            Temp[Tempii][kk] = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, jj, m_xmlDociment.Default_Attributes_RFID[kk]);
                        }
                        Tempii++;
                    }

                }
                comboBoxRFIDSeletStr[ii] = new String[Tempii][];
                for (int jj = 0; jj < Tempii; jj++)
                {
                    comboBoxRFIDSeletStr[ii][jj] = Temp[jj];
                }
            }
            comboBoxRFIDPLCSele.DataSource = comboBoxRFIDPLCSeleStr;
            comboBoxMSGType.DataSource = MSGTypeStrArr;

            /////��ʼ��dataGridViewRFIDReadMessage
            dataGridViewRFIDReadMessage.AllowUserToAddRows = false;
            dataGridViewRFIDReadMessage.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dataGridViewRFIDReadMessage.ReadOnly = true;
            dataGridViewRFIDReadMessage.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void RfidForm_SizeChanged(object sender, EventArgs e)
        {
//             asc.controlAutoSize(this);
            this.WindowState = (System.Windows.Forms.FormWindowState)(2);
        }

        /// <summary>
        /// ������Ϣ��ʾ���ݷ�ʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxMSGType_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxMSGType.Text == MSGTypeStrArr[0])//���ղ�Ʒ���к���ʾ
            {
                groupBoxConnet.Visible = false;
                groupBoxSelePLC.Visible = false;
                groupBoxSeleRFID.Visible = false;
                groupBox_IP.Visible = false;
                groupBox_Poort.Visible = false;
                groupBox_ReadDStar.Visible = false;
                groupBox_WriteDStar.Visible = false;
//                 RFIDDataDataTable_RemoveAll_bt.Visible = true;
                if (MainForm.m_ShowRfidDataTable != null)
                {
                    DataSource = MainForm.m_ShowRfidDataTable.RFIDDataDataTable;
                    timer_UpData_OK = true;
                }
            }
            else if (comboBoxMSGType.Text == MSGTypeStrArr[1])//���ն�д����ʾ
            {
                groupBoxConnet.Visible = true;
                groupBoxSelePLC.Visible = true;
                groupBoxSeleRFID.Visible = true;
//                 RFIDDataDataTable_RemoveAll_bt.Visible = false;
                comboBoxRFIDSelet_TextChanged(null, null);
            }
        }


        /// <summary>
        /// RFID����PLCѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private int plcserial = -1;
        private void comboBoxRFIDPLCSele_TextChanged(object sender, EventArgs e)
        {
            plc = PLCDataShare.m_plclist[comboBoxRFIDPLCSele.SelectedIndex];
            plcserial = plc.serial;
            String[] comboBoxRFIDSeletStr12 = new String[comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex].Length];
            for (int ii = 0; ii < comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex].Length; ii++)
            {
                comboBoxRFIDSeletStr12[ii] = comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][ii][(int)m_xmlDociment.Attributes_RFID.serial]
                    + ":" + comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][ii][(int)m_xmlDociment.Attributes_RFID.id];
            }
            comboBoxRFIDSelet.DataSource = comboBoxRFIDSeletStr12;
        }

        /// <summary>
        /// RFIDѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private int RFIDserial = -1;
        private bool RFIDconneted = false;
        private void comboBoxRFIDSelet_TextChanged(object sender, EventArgs e)
        {
            String[] SplitStr1 = comboBoxRFIDPLCSele.Text.Split(':');
            if(!int.TryParse(comboBoxRFIDSelet.Text.Split(':')[0],out RFIDserial))
            {
                return;
            }
            if (SplitStr1[1] == m_xmlDociment.PLC_System[1])
            {
                groupBox_IP.Visible = true;
                labelRFIDIPADRESSVULE.Text = comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][comboBoxRFIDSelet.SelectedIndex][(int)m_xmlDociment.Attributes_RFID.ip];
                groupBox_Poort.Visible = true;
                labelRFIDPOORTVELU.Text = comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][comboBoxRFIDSelet.SelectedIndex][(int)m_xmlDociment.Attributes_RFID.port];

                groupBox_ReadDStar.Visible = false;
                groupBox_WriteDStar.Visible = false;

                Collector.CollectHNCPLC.Hnc8PLCRFID m_fid = plc.m_hncPLCCollector.m_Hnc8PLCRFIDList.Find(
                        delegate(Collector.CollectHNCPLC.Hnc8PLCRFID temp)
                        {
                            return (temp.m_rfid.RFIDserial == RFIDserial);
                        }
                        );
                if (m_fid != null)
                {
                    //����״̬
                    if (m_fid.m_rfid.linkStatus.now == LinkStatusEnum.Linked)
                    {
                        RFIDconneted = true;
                    }
                    else
                    {
                        RFIDconneted = false;
                    }
                    labelStadeText.Text = m_fid.m_rfid.setStatus;
                    ///�������Դ
                    DataSource = m_fid.m_rfid.RFIDReadDataDataTable;
                    timer_UpData_OK = true;
                }
            }
            else if (SplitStr1[1] == m_xmlDociment.PLC_System[0])
            {
                groupBox_IP.Visible = false;
                groupBox_Poort.Visible = false;

                groupBox_ReadDStar.Visible = true;
                groupBox_WriteDStar.Visible = true;

                labelReadAdressStarValue.Text = comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][comboBoxRFIDSelet.SelectedIndex][(int)m_xmlDociment.Attributes_RFID.ReadDevice] +
                                            comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][comboBoxRFIDSelet.SelectedIndex][(int)m_xmlDociment.Attributes_RFID.ReadAddressStar];
                labelWriteAdressStarValue.Text = comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][comboBoxRFIDSelet.SelectedIndex][(int)m_xmlDociment.Attributes_RFID.WriteDevice] +
                                            comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][comboBoxRFIDSelet.SelectedIndex][(int)m_xmlDociment.Attributes_RFID.WriteAddressStar];

                PLC.RFIDinformation RFIDItem = plc.m_RFIDList.Find(
                    delegate(PLC.RFIDinformation temp)
                    {
                        return (temp.RFIDSerial == int.Parse(comboBoxRFIDSeletStr[comboBoxRFIDPLCSele.SelectedIndex][comboBoxRFIDSelet.SelectedIndex][(int)m_xmlDociment.Attributes_RFID.serial]));
                    }
                    );

                if (RFIDItem == null)
                {
                    dataGridViewRFIDReadMessage.DataSource = null;
                    return;
                }
                else
                {
                    DataSource = RFIDItem.RFIDReadDataDataTable;
                    timer_UpData_OK = true;
                }
                RFIDconneted = plc.conneted;
            }
        }

        private void MITSUBISHIPlcRFIDAutoUpDataHandler(object sender, PLC.MITSUBISHIPLCRFIDEvent Args)
        {
            if (Args.plcserial == -1)
            {
                PLC.PLC_MITSUBISHI_HNC8 m_PLC = (PLC.PLC_MITSUBISHI_HNC8)sender;
                RFIDconneted = m_PLC.conneted;
            }
            else
            {
                if (plcserial == Args.plcserial)
                {
                    if (Args.RFIDserial == RFIDserial)
                    {
                        DataSource = plc.m_RFIDList[Args.RFIDserial].RFIDReadDataDataTable;
                        timer_UpData_OK = true;

//                         DataTable DataSource = plc.m_RFIDList[Args.RFIDserial].RFIDReadDataDataTable;
//                         if (dataGridViewRFIDReadMessage.InvokeRequired)//�ȴ��첽
//                         {
//                             // ��һ���ؼ���InvokeRequired����ֵΪ��ʱ��˵����һ��������������߳��������
//                             Action<DataTable> actionDelegate = (x) =>
//                             {
//                                 lock (plc.m_RFIDList[Args.RFIDserial].RFIDReadDataDataTable_Look)
//                                 {
//                                     dataGridViewRFIDReadMessage.DataSource = null;
//                                     dataGridViewRFIDReadMessage.DataSource = x;
//                                 }
//                             };
//                             dataGridViewRFIDReadMessage.Invoke(actionDelegate, DataSource);
//                         }
//                         else
//                         {
//                             lock (plc.m_RFIDList[Args.RFIDserial].RFIDReadDataDataTable_Look)
//                             {
//                                 dataGridViewRFIDReadMessage.DataSource = null;
//                                 dataGridViewRFIDReadMessage.DataSource = DataSource;
//                             }
//                         }
                    }
                }
            }
        }


        /// <summary>
        /// ί�������ı��ؼ���ʾ
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="str"></param>
        private void ThreaSetLaBText(Label LB, String str)
        {
            //             if (LB.Text != str)
            {
                if (LB.InvokeRequired)//�ȴ��첽
                {
                    // ��һ���ؼ���InvokeRequired����ֵΪ��ʱ��˵����һ��������������߳��������
                    Action<string> actionDelegate = (x) => { LB.Text = x; };
                    LB.BeginInvoke(actionDelegate, str);
                }
                else
                {
                    LB.Text = str;
                }
            }
        }

        private void ThreaSetPictureBoxBackColor(PictureBox PB, Bitmap BT)
        {
            if (PB.Image != BT)
            {
                if (PB.InvokeRequired)//�ȴ��첽
                {
                    // ��һ���ؼ���InvokeRequired����ֵΪ��ʱ��˵����һ��������������߳��������
                    Action<Bitmap> actionDelegate = (x) => { PB.Image = x; };
                    PB.BeginInvoke(actionDelegate, BT);
                }
                else
                {
                    PB.Image = BT;
                }
            }
        }

        private void HNC8RfidDataTableUpdataHandler(object sender, PLC.HNC8PLCRFIDEvent e)
        {
            if (groupBoxSeleRFID.Visible)
            {
                RFID m_rfid = (RFID)sender;
                if (m_rfid.RFIDserial == RFIDserial)
                {
                    switch (e.EventType)
                    {
                        case -1://����
                            RFIDconneted = false;
                            break;
                        case 0://ͨ������
                            RFIDconneted = true;
                            break;
                        case 100://��д״̬����
//                             ThreaSetLaBText(labelStadeText, e.Event);
                            break;
                        case 200:
                            if (groupBoxSeleRFID.Visible && dataGridViewRFIDReadMessage.Visible)
                            {
                                DataSource = m_rfid.RFIDReadDataDataTable;
                                timer_UpData_OK = true;
                            }
                            break;
                        case 300:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        DataTable DataSource;
        private void RfidDataTableUpdataHandler(object sender, String e)
        {
            if (!groupBoxSeleRFID.Visible && dataGridViewRFIDReadMessage.Visible
                && MainForm.m_ShowRfidDataTable != null && MainForm.m_ShowRfidDataTable.RFIDDataDataTable != null)
            {
                DataSource = MainForm.m_ShowRfidDataTable.RFIDDataDataTable;
                timer_UpData_OK = true;
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MainForm.LanguageChangeMsg:
//                     Localization.RefreshLanguage(this);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CollectShare.Instance().HNC_RegSetBit((Int32)HNCAPI.HncRegType.REG_TYPE_R, 22, 1, Collector.CollectHNCPLC.m_clientNo);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CollectShare.Instance().HNC_RegSetBit((Int32)HNCAPI.HncRegType.REG_TYPE_R, 22, 0, Collector.CollectHNCPLC.m_clientNo);
        }

        private bool timer_UpData_OK = false;
        private bool RFIDconneted_old = true;
        private bool dataGridViewRFIDReadMessage_Op = true;
        private void timer_UpData_Tick(object sender, EventArgs e)
        {
            if (timer_UpData_OK && dataGridViewRFIDReadMessage_Op && dataGridViewRFIDReadMessage.Visible)
            {
                dataGridViewRFIDReadMessage.DataSource = null;
                dataGridViewRFIDReadMessage.DataSource = DataSource.Copy();
//                 Grid(DataSource.Copy());
                timer_UpData_OK = false;
            }
            if ((RFIDconneted != RFIDconneted_old || (labelLinckText.Text != MainForm.LinckedText && labelLinckText.Text != MainForm.UnLinckedText))
                 && labelLinckText.Visible)
            {
                if (RFIDconneted)
                {
                    pictureBox_LinkStade.Image = SCADA.Properties.Resources.top_bar_green;
                    labelLinckText.Text = MainForm.LinckedText;
                }
                else
                {
                    pictureBox_LinkStade.Image = SCADA.Properties.Resources.top_bar_black;
                    labelLinckText.Text = MainForm.UnLinckedText;
                }
                RFIDconneted_old = RFIDconneted;
            }
        }

        private delegate void myDelegate(DataTable dt);//����ί��
        private void Grid(DataTable dt)
        {
            if (this.dataGridViewRFIDReadMessage.InvokeRequired)
            {
                this.BeginInvoke(new myDelegate(Grid), new object[] { dt });
            }
            else
            {
                try
                {
                    this.dataGridViewRFIDReadMessage.DataSource = null;
                    this.dataGridViewRFIDReadMessage.DataSource = dt;
                    dt = null;
                }
                catch
                {

                }
            }
        }


        private void dataGridViewRFIDReadMessage_MouseEnter(object sender, EventArgs e)
        {
            dataGridViewRFIDReadMessage_Op = false;
        }

        private void dataGridViewRFIDReadMessage_MouseLeave(object sender, EventArgs e)
        {
            dataGridViewRFIDReadMessage_Op = true;
        }

        private void RFIDDataDataTable_RemoveAll_bt_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show("ȷ��������е�RFID��Ϣ��", "����", MessageBoxButtons.YesNoCancel,
             MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.Yes)
            {
                DataSource.Rows.Clear();
                timer_UpData_OK = true;
            }
        }

        private void dataGridViewRFIDReadMessage_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (dataGridViewRFIDReadMessage.Rows[e.RowIndex].IsNewRow)
                return;
        }

        private void button_UpDataMessage_Click(object sender, EventArgs e)
        {
            RfidDataTableUpdataHandler(null,null);
        }

        private void dataGridViewRFIDReadMessage_VisibleChanged(object sender, EventArgs e)
        {

        }

    }
}
