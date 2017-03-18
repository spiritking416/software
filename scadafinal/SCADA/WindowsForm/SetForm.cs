using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;


namespace SCADA
{
    public partial class SetForm : Form
    {
        #region   定义窗体类需要的成员和构造函数
        //调整窗体类
//         AutoSizeFormClass asc = new AutoSizeFormClass();
        public class Dgv_Xml_PathType
        {
            public DataGridView Dgv;
            public string PathStr;
            public bool DgvDatChange;
        }

        //窗体切换时数据是否更改的标志
        List<Dgv_Xml_PathType> DgvLis = new List<Dgv_Xml_PathType>();
        //保持标记
        bool DGVRowsAddFlag = false;
        public static IntPtr setFormPtr;
        List<Binding> m_bindingList = new List<Binding>();
        int dgvPLCSet_CurrentRow_Index_Old = -1;
        int dgvRobotSet_CurrentRow_Index_Old = -1;
        String[] UserPasswordStrArr = { MessageString.SetForms_Login, MessageString.SetForms_Logout, MessageString.SetForms_Cancel,MessageString.SetForms_ChangePwd, MessageString.SetForms_Confirm};
        String LogInMessage =MessageString.SetForms_RightNotEnough;
        private System.EventHandler<String> MsgTipsHandler ;
        enum UserPasswordStrArrIndex
        {
            LOGIN = 0,
            LOGOUT,
            CANCEL,
            CHANGE_PWD,
            CONFIRM
        }
        //构造函数初始化界面
        public SetForm()
        {
            InitializeComponent();
            Dgv_Xml_PathType m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = dgvCNCSet;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_CNC;
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = dgvRobotSet;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_ROBOT;
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = DGVRobotInputSignalS;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_ROBOT_Item + "0" + "/" + m_xmlDociment.Default_Path_str[(int)m_xmlDociment.Path_str.X];
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = DGVRobotOutputSignalS;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_ROBOT_Item + "0" + "/" + m_xmlDociment.Default_Path_str[(int)m_xmlDociment.Path_str.Y];
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = dgvPLCSet;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_PLC;
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = PLCInputSignalSDefine;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_PLC_Item + "0" + "/" + m_xmlDociment.Default_Path_str[(int)m_xmlDociment.Path_str.X];
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = PLCOutputSignalSDefine;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_PLC_Item + "0" + "/" + m_xmlDociment.Default_Path_str[(int)m_xmlDociment.Path_str.Y];
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = dgvRFIDSet;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_RFID;
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);

            m_DgvLisNode = new Dgv_Xml_PathType();
            m_DgvLisNode.Dgv = dgv_PLCAlarmTb;
            m_DgvLisNode.PathStr = m_xmlDociment.PathRoot_PLCAlarmTb;
            m_DgvLisNode.DgvDatChange = false;
            DgvLis.Add(m_DgvLisNode);


            Binding m_Binding = new Binding("Text", dgvCNCSet, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox_CNCNUM.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            m_Binding = new Binding("Text", dgvRobotSet, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox_ROBOTNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            m_Binding = new Binding("Text", DGVRobotInputSignalS, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox_SeleRobotIntSNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);


            m_Binding = new Binding("Text", DGVRobotOutputSignalS, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox_SeleRobotOutSNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            m_Binding = new Binding("Text", dgvPLCSet, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox_PLCDVGROSNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            m_Binding = new Binding("Text", PLCInputSignalSDefine, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox__SelePLCIntDVGROSNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            m_Binding = new Binding("Text", PLCOutputSignalSDefine, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox__SelePLCOutDVGROSNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            m_Binding = new Binding("Text", dgvRFIDSet, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox_RFidDgvRowsNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            m_Binding = new Binding("Text", dgv_PLCAlarmTb, "RowCount", false);
            m_Binding.Format += new ConvertEventHandler(OnCountryFromFormat);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBox_PLCAlarmNum.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);

            label_Tisp.Text = "";
            label_Tisp.ForeColor = Color.Red;
            label_ChangeUserTips.Text = "";
            label_ChangeUserTips.ForeColor = Color.Red;
            comboBox_UserNmae.DataSource = MainForm.m_xml.GetUserNameStrArr();
            if (comboBox_UserNmae.Items.Count > 0)
            {
                comboBox_UserNmae.SelectedIndex = 0;
                label_CurrentUsername.Text = comboBox_UserNmae.Text;
                button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGOUT];//注销
                button_ChangeUserPassword.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.CHANGE_PWD];//修改密码
            }
            SetUserPasswoedItemShowOrHide();
            LogIn = false;
            MsgTipsHandler = new EventHandler<string>(this.MsgTipsHandlerFuc);
        }
        #endregion

        #region   数量改变响应入口
        void OnCountryFromFormat(object sender, ConvertEventArgs e)
        {
            try
            {
                DataGridView DGV = (DataGridView)(((Binding)sender).DataSource);
                int ii = int.Parse(e.Value.ToString());
                int conut_min = 1;
                if (DGV == PLCInputSignalSDefine || DGV == PLCOutputSignalSDefine)//PLC寄存器监控可以为0
                {
                    conut_min = 0;
                }

                if (e.Value == null || e.Value == DBNull.Value)
                {
                    return;
                }
                else if (ii > conut_min)
                {
                    if (DGV.RowCount == ii && DGVRowsAddFlag)
                    {
                        for (int jj = 0; jj < ii; jj++)
                        {
                            if (DGV.Rows[jj].Cells[0].Value == null || DGV.Rows[jj].Cells[0].Value.ToString() == "")
                            {
                                if (DGV.ColumnCount == m_xmlDociment.Default_Attributesstr1_value.Length)
                                {
                                    DGV.Rows[jj].SetValues(m_xmlDociment.Default_Attributesstr1_value);
                                }
                                else if (DGV.ColumnCount == m_xmlDociment.Default_Attributesstr2_value.Length)
                                {
                                    DGV.Rows[jj].SetValues(m_xmlDociment.Default_Attributesstr2_value);
                                }
                                else if (DGV.ColumnCount == m_xmlDociment.Default_Attributes_RFID_value.Length)
                                {
                                    DGV.Rows[jj].SetValues(m_xmlDociment.Default_Attributes_RFID_value);
                                }
                                DGV.Rows[jj].Cells[0].Value = jj.ToString();
                            }
                        }
                        DGVRowsAddFlag = false;
                    }
                    e.Value = ii.ToString();
                }
                else
                {
                    e.Value = conut_min.ToString();//"1";
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

            }
        }
        void OnCountryFromParse(object sender, ConvertEventArgs e)
      {
          int ii = 0;
          try
          {
              DataGridView DGV = (DataGridView)(((Binding)sender).DataSource);
              ii = int.Parse(e.Value.ToString());
              if ( ii >= 0)
              {
                    if (ii == 0 )
                    {
                        if (DGV != PLCInputSignalSDefine && DGV != PLCOutputSignalSDefine)//PLC寄存器监控也许为0
                        {
                            ii++;
                        }
                    }
                    if (DGV.RowCount < ii)//
                    {
                        DGVRowsAddFlag = true;
                    }
                    else
                    {
                        DGVRowsAddFlag = false;
                    }
                    if (DGV.RowCount != ii)//数据被改变
                    {
                        RefreshDgvDataChangeF(DGV);
                    }
                    e.Value = ii.ToString();
              }
          }
          catch (System.Exception ex)
          {
                Console.WriteLine(ex.StackTrace);
            }
      }
        #endregion

        #region DataGridView 复制粘贴保存快捷键
        private void DataGirdViewCopy(DataGridView DGV)
        {
            try
            {
                if (DGV.GetCellCount(DataGridViewElementStates.Selected) > 0)
                {
                    Clipboard.SetDataObject(DGV.GetClipboardContent());
                }
            }
            catch
            {
                // 不处理
            }
        }
        private bool DataGirdViewPaste(DataGridView DGV)
        {
            bool flagert = true;
            try
            {
                int RowIndex = DGV.CurrentCell.RowIndex;
                int ColumnIndex = DGV.CurrentCell.ColumnIndex;

                // 获取剪切板的内容，并按行分割
                string pasteText = Clipboard.GetText();
                if (string.IsNullOrEmpty(pasteText))
                    return flagert;
                pasteText = pasteText.Replace('\r', '\0');
                string[] lines = pasteText.Split('\n');
                for (int jj = RowIndex; jj < RowIndex + lines.Length; jj++)
                { 
                    if (jj >= DGV.Rows.Count)
                    {
                        break;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(lines[jj - RowIndex].Trim()))
                            continue;
                        string[] vals = lines[jj - RowIndex].Split('\t');// 按 Tab 分割数据
                        for (int ii = ColumnIndex; ii < vals.Length + ColumnIndex; ii++)
                        {
                            if (DGV.ColumnCount <= ii)
                            {
                                break;
                            }
                            else
                            {
                                DGV.Rows[jj].Cells[ii].Value = vals[ii - ColumnIndex].Trim("\0".ToCharArray()); 
                            }
                        }
                        if (vals.Length > 1)
                        {
                            flagert = false;
                        }
                    }
                }
                if (lines.Length > 1)
                {
                    flagert = false;
                }
            }
            catch
            {
                // 不处理
            }
            return flagert;
        }


        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control focusedControl = null;
            IntPtr focusedHandle = GetFocus();
            if (focusedHandle != IntPtr.Zero)
            focusedControl = Control.FromChildHandle(focusedHandle);
            focusedControl = focusedControl.Parent.Parent;
            if (focusedControl != null && focusedControl.GetType() == typeof(DataGridView))
            {
                if (keyData == (Keys.Control | Keys.V))//Ctrl+V
                {
                    if (!DataGirdViewPaste((DataGridView)focusedControl))
                    {
                        return true;
                    }
                }
                else if (keyData == (Keys.Control | Keys.C))//Ctrl+C
                {
                    DataGirdViewCopy((DataGridView)focusedControl);
                }
            }
            if (keyData == (Keys.Control | Keys.S))//Ctrl+S
            {

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
        /// <summary>
        /// 刷新DgvLi中的数据改变标记
        /// </summary>
        /// <param name="dgv"></param>
        void RefreshDgvDataChangeF(DataGridView dgv)
        {
            for (int ii = 0; ii < DgvLis.Count; ii++)
            {
                if (DgvLis[ii].Dgv == dgv)
                {
                    DgvLis[ii].DgvDatChange = true;
                }
            }
        }

        /// <summary>
        /// 刷新DgvLi数据在Xml中的路径
        /// </summary>
        /// <param name="dgv"></param>
        void RefreshDgvXmlPath(DataGridView dgv,Int32 Index)
        {
            if (dgv.Name == DGVRobotInputSignalS.Name)
            {
                DgvLis[2].PathStr = m_xmlDociment.PathRoot_ROBOT_Item + Index.ToString() + "/" + m_xmlDociment.Default_Path_str[(int)m_xmlDociment.Path_str.X];
            }
            else if (dgv.Name == DGVRobotOutputSignalS.Name)
            {
                DgvLis[3].PathStr = m_xmlDociment.PathRoot_ROBOT_Item + Index.ToString() + "/" + m_xmlDociment.Default_Path_str[(int)m_xmlDociment.Path_str.Y];
            }
            else if (dgv.Name == PLCInputSignalSDefine.Name)
            {
                DgvLis[5].PathStr = m_xmlDociment.PathRoot_PLC_Item + Index.ToString() + "/" + comboBoxPLCDevice1.Text;//MainForm.m_xml.Default_Path_str[6];
            }
            else if (dgv.Name == PLCOutputSignalSDefine.Name)
            {
                DgvLis[6].PathStr = m_xmlDociment.PathRoot_PLC_Item + Index.ToString() + "/" + comboBoxPLCDevice2.Text;//MainForm.m_xml.Default_Path_str[7];
            }
        }

        /// <summary>
        /// 获取DgvLi数据再Xml中的路径
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        string GetDgvXmlLisPath(DataGridView dgv)
        {
            string path = "";
            for (int ii = 0; ii < DgvLis.Count; ii++)
            {
                if (DgvLis[ii].Dgv == dgv)
                {
                    path = DgvLis[ii].PathStr;
                }
            }
            return path;
        }

        /// <summary>
        /// 刷新列表行数
        /// </summary>
        /// <param name="ReadValue_name"></param>
        void BindingLisReadValue(DataGridView dgv)
        {
            foreach (Binding m_b in m_bindingList)
            {
                if (((DataGridView)(m_b.DataSource)) == dgv)
                {
                    m_b.ReadValue();
                }
            }
        }


        #region   SetForm窗体装载
        private void SetForm_Load(object sender, EventArgs e)
        {
            //改变datagridview的编辑属性
            dgvCNCSet.AllowUserToAddRows = false;
            dgvCNCSet.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgvPLCSet.AllowUserToAddRows = false;
            dgvPLCSet.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgvRobotSet.AllowUserToAddRows = false;
            dgvRobotSet.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgvRFIDSet.AllowUserToAddRows = false;
            dgvRFIDSet.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGVRobotInputSignalS.AllowUserToAddRows = false;
            DGVRobotInputSignalS.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGVRobotOutputSignalS.AllowUserToAddRows = false;
            DGVRobotOutputSignalS.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            PLCInputSignalSDefine.AllowUserToAddRows = false;
            PLCInputSignalSDefine.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            PLCOutputSignalSDefine.AllowUserToAddRows = false;
            PLCOutputSignalSDefine.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgv_PLCAlarmTb.AllowUserToAddRows = false;
            dgv_PLCAlarmTb.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            //设置列的只读属性         
            dgvCNCSet.Columns[0].ReadOnly = true;
            dgvCNCSet.Columns[2].ReadOnly = true;
            dgvCNCSet.Columns[3].ReadOnly = true;
            dgvCNCSet.Columns[4].ReadOnly = true;
            dgvCNCSet.Columns[5].ReadOnly = true;

            dgvPLCSet.Columns[0].ReadOnly = true;
            dgvPLCSet.Columns[2].ReadOnly = true;
            dgvPLCSet.Columns[3].ReadOnly = true;
            dgvPLCSet.Columns[4].ReadOnly = true;
            dgvPLCSet.Columns[5].ReadOnly = true;

            dgvRobotSet.Columns[0].ReadOnly = true;
            dgvRobotSet.Columns[2].ReadOnly = true;
            dgvRobotSet.Columns[3].ReadOnly = true;
            dgvRobotSet.Columns[4].ReadOnly = true;
            dgvRobotSet.Columns[5].ReadOnly = true;

            DGVRobotInputSignalS.Columns[0].ReadOnly = true;
            DGVRobotInputSignalS.Columns[3].ReadOnly = true;
            DGVRobotOutputSignalS.Columns[0].ReadOnly = true;
            DGVRobotOutputSignalS.Columns[3].ReadOnly = true;

            PLCInputSignalSDefine.Columns[0].ReadOnly = true;
            PLCInputSignalSDefine.Columns[3].ReadOnly = true;
            PLCOutputSignalSDefine.Columns[0].ReadOnly = true;
            PLCOutputSignalSDefine.Columns[3].ReadOnly = true;

            //设置列不能自动排序       
            for (int i = 0; i < dgvCNCSet.Columns.Count; i++)
            {
                dgvCNCSet.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < dgvPLCSet.Columns.Count; i++)
            {
                dgvPLCSet.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < dgvRobotSet.Columns.Count; i++)
            {
                dgvRobotSet.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < DGVRobotInputSignalS.Columns.Count; i++)
            {
                DGVRobotInputSignalS.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < DGVRobotOutputSignalS.Columns.Count; i++)
            {
                DGVRobotOutputSignalS.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < PLCInputSignalSDefine.Columns.Count; i++)
            {
                PLCInputSignalSDefine.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < PLCOutputSignalSDefine.Columns.Count; i++)
            {
                PLCOutputSignalSDefine.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            ///////////CNC本地IP选择处理
            System.Net.IPAddress CNCLocalIp;
            if (!System.Net.IPAddress.TryParse(MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]), out CNCLocalIp))
            {
                MainForm.m_xml.m_UpdateAttribute(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0], m_xmlDociment.Default_CNCLocalIVvalue[0]);
                MainForm.m_xml.SaveXml2File(MainForm.XMLSavePath);
            }
            comboBox_LocalIpAddr.SelectedText = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]);

            //////////////////////PLC系统
            comboBoxSelePLCSystem.DataSource = m_xmlDociment.PLC_System;
            /////////产线类型选择

            comboBoxLineType.DataSource = m_xmlDociment.Default_linetype_value;
            for(int ii = 0;ii < m_xmlDociment.Default_linetype_value.Length;ii++)
            {
                if (MainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]) == m_xmlDociment.Default_linetype_value[ii])
                {
                    comboBoxLineType.SelectedIndex = ii;
                }
            }
            comboBoxPLCDevice1.DataSource = m_xmlDociment.Default_MITSUBISHI_Device1;
            comboBoxPLCDevice2.DataSource = m_xmlDociment.Default_MITSUBISHI_Device2;



//             string str = "";
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            int jj = 0,kk =0;
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection allAddress = adapterProperties.UnicastAddresses;
                if (allAddress.Count > 0)
                {
//                     str += "interface   " + jj + "description:\n\t " + adapter.Description + "\n ";
                    foreach (UnicastIPAddressInformation addr in allAddress)
                    {
                        if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            //comboBox_LocalIpAddr.Items.Add(addr.Address);
                        }
                        if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            comboBox_LocalIpAddr.Items.Add(addr.Address);
                            if (addr.Address.ToString() == MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]))
                            {
                                kk = jj;
                            }
                            jj++;
                        }
                    }
                }
            }
            comboBox_LocalIpAddr.SelectedIndex = kk;

            textBoxStartIO.Text = "0";

            InitdgvSet(ref dgvRFIDSet, ref m_xmlDociment.Default_RFID_STR);
            dgvRFIDSet.Columns[0].ReadOnly = true;
            dgvRFIDSet.Columns[2].ReadOnly = true;
            dgvRFIDSet.Columns[3].ReadOnly = true;
            dgvRFIDSet.Columns[2].Visible = false;
            dgvRFIDSet.Columns[3].Visible = false;
            for (int i = 0; i < dgvRFIDSet.Columns.Count; i++)
            {
                dgvRFIDSet.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            InitdgvSet(ref dgv_PLCAlarmTb, ref m_xmlDociment.Default_PLCAlarmTb_STR);
            dgv_PLCAlarmTb.Columns[0].ReadOnly = true;
            for (int i = 0; i < dgvRFIDSet.Columns.Count; i++)
            {
                dgvRFIDSet.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }


            setFormPtr = Handle;
//             asc.controllInitializeSize(this);
//             if (Localization.HasLang)
//             {
//                 Localization.RefreshLanguage(this);
//                 changeDGVSetting();
//             }
        }
        #endregion

        #region   表格标题行语音切换注册
        private void changeDGVSetting()
        {
//             dgvCNCSet.Columns[0].HeaderText = Localization.Forms["SetForm"]["colNumberCNCSet"];
//             dgvCNCSet.Columns[1].HeaderText = Localization.Forms["SetForm"]["colIDSet"];
//             dgvCNCSet.Columns[2].HeaderText = Localization.Forms["SetForm"]["colWorkshopSet"];
//             dgvCNCSet.Columns[3].HeaderText = Localization.Forms["SetForm"]["colProductionLineSet"];
//             dgvCNCSet.Columns[4].HeaderText = Localization.Forms["SetForm"]["colTypeSet"];
//             dgvCNCSet.Columns[5].HeaderText = Localization.Forms["SetForm"]["colDataSet"];
//             dgvCNCSet.Columns[6].HeaderText = Localization.Forms["SetForm"]["colIPSet"];
//             dgvCNCSet.Columns[7].HeaderText = Localization.Forms["SetForm"]["colPortSet"];
// 
//             dgvRobotSet.Columns[0].HeaderText = Localization.Forms["SetForm"]["colNumberRobotSet"];
//             dgvRobotSet.Columns[1].HeaderText = Localization.Forms["SetForm"]["colRobotIDSet"];
//             dgvRobotSet.Columns[2].HeaderText = Localization.Forms["SetForm"]["colWorkShopRobotSet"];
//             dgvRobotSet.Columns[3].HeaderText = Localization.Forms["SetForm"]["colProductionLineRobotSet"];
//             dgvRobotSet.Columns[4].HeaderText = Localization.Forms["SetForm"]["colTypeRobotSet"];
//             dgvRobotSet.Columns[5].HeaderText = Localization.Forms["SetForm"]["colModelRobotSet"];
//             dgvRobotSet.Columns[6].HeaderText = Localization.Forms["SetForm"]["colModelRobotIp"];
//             dgvRobotSet.Columns[7].HeaderText = Localization.Forms["SetForm"]["colModelRobotPort"];
// 
//             dgvPLCSet.Columns[0].HeaderText = Localization.Forms["SetForm"]["colNumberPLCSet"];
//             dgvPLCSet.Columns[1].HeaderText = Localization.Forms["SetForm"]["colPLCIDSet"];
//             dgvPLCSet.Columns[2].HeaderText = Localization.Forms["SetForm"]["colWorkShopPLCSet"];
//             dgvPLCSet.Columns[3].HeaderText = Localization.Forms["SetForm"]["colProductionLinePLCSet"];
//             dgvPLCSet.Columns[4].HeaderText = Localization.Forms["SetForm"]["colTypePLCSet"];
//             dgvPLCSet.Columns[5].HeaderText = Localization.Forms["SetForm"]["colModelPLCSet"];
//             dgvPLCSet.Columns[6].HeaderText = Localization.Forms["SetForm"]["colIPPLCSet"];
//             dgvPLCSet.Columns[7].HeaderText = Localization.Forms["SetForm"]["colPortPLCSet"];
// 
//             dgvRFIDSet.Columns[0].HeaderText = Localization.Forms["SetForm"]["colNumberRFIDSet"];
//             dgvRFIDSet.Columns[1].HeaderText = Localization.Forms["SetForm"]["colIDRFIDSet"];
//             dgvRFIDSet.Columns[2].HeaderText = Localization.Forms["SetForm"]["colWorkShopRFIDSet"];
//             dgvRFIDSet.Columns[3].HeaderText = Localization.Forms["SetForm"]["colProductionLineRFIDSet"];
//             dgvRFIDSet.Columns[4].HeaderText = Localization.Forms["SetForm"]["colTypeRFIDSet"];
//             dgvRFIDSet.Columns[5].HeaderText = Localization.Forms["SetForm"]["colModelRFIDSet"];
//             dgvRFIDSet.Columns[6].HeaderText = Localization.Forms["SetForm"]["colIPRFIDSet"];
//             dgvRFIDSet.Columns[7].HeaderText = Localization.Forms["SetForm"]["colPortRFIDSet"];
//             dgvRFIDSet.Columns[8].HeaderText = Localization.Forms["SetForm"]["colControlRFIDSet"];


//             DGVRobotSignalSDefine.Columns[0].HeaderText = Localization.Forms["SetForm"]["robotNum"];
//             DGVRobotSignalSDefine.Columns[1].HeaderText = Localization.Forms["SetForm"]["RobotSignalNum"];
//             DGVRobotSignalSDefine.Columns[2].HeaderText = Localization.Forms["SetForm"]["RobotAddress"];
//             DGVRobotSignalSDefine.Columns[3].HeaderText = Localization.Forms["SetForm"]["RobotSignalName"];
// 
//             DGVPLCSignalSDefine.Columns[0].HeaderText = Localization.Forms["SetForm"]["PLCNum"];
//             DGVPLCSignalSDefine.Columns[1].HeaderText = Localization.Forms["SetForm"]["PLCSignalNum"];
//             DGVPLCSignalSDefine.Columns[2].HeaderText = Localization.Forms["SetForm"]["plcAddress"];
//             DGVPLCSignalSDefine.Columns[3].HeaderText = Localization.Forms["SetForm"]["plcSignalName"];
            
        }
        #endregion

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MainForm.LanguageChangeMsg:
                    Localization.RefreshLanguage(this);
                    changeDGVSetting();
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }       
        #region   窗体自动调整布局设置
        private void SetForm_SizeChanged(object sender, EventArgs e)
        {
//             asc.controllInitializeSize(this);
//             asc.controlAutoSize(this);
            this.WindowState = (System.Windows.Forms.FormWindowState)(2);
        }
        #endregion

        #region   数据显示
        //整个窗体可见时显示的处理
        private void SetForm_VisibleChanged(object sender, EventArgs e)
        {
            if (((SetForm)sender).Visible)
            {
                tabControlSet.SelectedIndex = 0; //设置默认显示
                txtCNCSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[0]);
                txtRobotSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_ROBOT, -1, m_xmlDociment.Default_Attributes_str1[0]);
                txtPLCSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, -1, m_xmlDociment.Default_Attributes_str1[0]);
                txtRFIDSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, -1, m_xmlDociment.Default_Attributes_str1[0]);

                //车间和产线的显示  
                txtWSSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[2]);
                txtPLSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[3]);
            }
        }
        #endregion

        #region   判断输入参数是否合法
        private void txtCNCSet_Leave(object sender, EventArgs e)
        {
            //初始化正则表达式
            Regex digitregex = new Regex(@"^(?:\d|[123]\d|40)$");
            //判断文本框内容是否符合正则表达式
            if (digitregex.IsMatch(txtCNCSet.Text) == true)
            { }
            else
            {
                MessageBox.Show(MessageString.SetForms_0_40, MessageString.SetForms_Information, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNCSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[0]);
                this.txtCNCSet.Focus();
            }
        }

        private void txtRobotSet_Leave(object sender, EventArgs e)
        {
            //初始化正则表达式
            Regex digitregex = new Regex(@"^(?:\d|[1]\d|20)$");
            //判断文本框内容是否符合正则表达式
            if (digitregex.IsMatch(txtRobotSet.Text) == true)
            { }
            else
            {
                MessageBox.Show(MessageString.SetForms_0_20, MessageString.SetForms_Information, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRobotSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_ROBOT, -1, m_xmlDociment.Default_Attributes_str1[0]);
                this.txtRobotSet.Focus();
            }
        }

        private void txtPLCSet_Leave(object sender, EventArgs e)
        {
            //初始化正则表达式
            Regex digitregex = new Regex(@"^(?:\d|[1]\d|20)$");
            //判断文本框内容是否符合正则表达式
            if (digitregex.IsMatch(txtPLCSet.Text) == true)
            { }
            else
            {
                MessageBox.Show(MessageString.SetForms_0_20, MessageString.SetForms_Information, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPLCSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, -1, m_xmlDociment.Default_Attributes_str1[0]);
                this.txtPLCSet.Focus();
            }
        }

        private void txtRFIDSet_Leave(object sender, EventArgs e)
        {
            //初始化正则表达式
            Regex digitregex = new Regex(@"^(?:\d|[1]\d|20)$");
            //判断文本框内容是否符合正则表达式
            if (digitregex.IsMatch(txtRFIDSet.Text) == true)
            { }
            else
            {
                MessageBox.Show(MessageString.SetForms_0_20, MessageString.SetForms_Information, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRFIDSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, -1, m_xmlDociment.Default_Attributes_str1[0]);
                this.txtRFIDSet.Focus();
            }

        }

        private void txtWSSet_Leave(object sender, EventArgs e)
        {
            //初始化正则表达式
//             Regex digitregex = new Regex(@"^#(\d?|10)$");
            //判断文本框内容是否符合正则表达式
//             if (txtWSSet.Text != "")
//             {
//             }
//             else
//             {
//                 MessageBox.Show("请输入字符串", " 提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                 txtWSSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[2]);
//                 this.txtWSSet.Focus();
//             }
        }

        private void txtPLSet_Leave(object sender, EventArgs e)
        {
        }

        #endregion

        #region    保存配置

        /// <summary>
        /// 保存
        /// </summary>
        private void SaveDgvData2XmlAndFile(int flag_i)
        {
            if(LogIn)
            {
                bool SaveF = false;
                setSave_Click(null, null);
                bool MessageBoxPosFla = false;
                for (int ii = 0; ii < DgvLis.Count; ii++)
                {
                    if (DgvLis[ii].DgvDatChange)
                    {
                        if (!MessageBoxPosFla)
                        {
                            string PosMessageStr =MessageString.SetForms_PosMessageStr1;
                            if (flag_i == 1)
                            {
                                PosMessageStr = MessageString.SetForms_PosMessageStr2;
                            }
                            DialogResult select;
                            select = MessageBox.Show(PosMessageStr, MessageString.SetForms_Information, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            //判断用户的选择
                            if (select != DialogResult.OK)
                            {
                                if (flag_i == 0)
                                {
                                    for (int jj = ii; jj < DgvLis.Count; jj++)
                                    {
                                        DgvLis[jj].DgvDatChange = false;
                                    }
                                }
                                if (comboBoxPLCDevice1.Text != m_xmlDociment.Default_MITSUBISHI_Device1[2])//Buffer
                                {
                                    return;
                                }
                            }
                            else
                            {
                                SaveF = true;
                            }
                            MessageBoxPosFla = true;
                        }
                        refreshDgvData2Xml(DgvLis[ii].Dgv, DgvLis[ii].PathStr);
                        DgvLis[ii].DgvDatChange = false;
                    }
                }
                if (comboBoxPLCDevice1.Text == m_xmlDociment.Default_MITSUBISHI_Device1[2] && dgvPLCSet != null)//Buffer
                {
                    String iStartIOStr = textBoxStartIO_Old.ToString();
                    String pathstr = m_xmlDociment.PathRoot_PLC_Item + dgvPLCSet.CurrentRow.Index.ToString()
                                        + "/" + comboBoxPLCDevice1.Text;
                    if (MainForm.m_xml.CheckNodeExist(pathstr))
                    {
                        if (iStartIOStr != MainForm.m_xml.m_Read(pathstr, -1, m_xmlDociment.Default_Attributes_str2[4]))
                        {
                            MainForm.m_xml.m_UpdateAttribute(pathstr, -1, m_xmlDociment.Default_Attributes_str2[4], iStartIOStr);
                            SaveF = true;
                        }
                    }
                }
                if (SaveF)
                {
                    MainForm.m_xml.SaveXml2File(MainForm.XMLSavePath);
                    LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                    SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                    SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                    SendParm.Keywords = MessageString.SetForms_parameter_Save;
                    SendParm.EventData =MessageString.SetForms_parameter_Save_OK;
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }
            }
        }
        private void btnSaveCNC_Click(object sender, EventArgs e)
        {
            SaveDgvData2XmlAndFile(0);
        }
        private void btnSaveROBOT_Click(object sender, EventArgs e)
        {
            SaveDgvData2XmlAndFile(0);
        }
        private void btnSavePLC_Click(object sender, EventArgs e)
        {
            SaveDgvData2XmlAndFile(0);
        }
        private void btnSaveRFID_Click(object sender, EventArgs e)
        {
            SaveDgvData2XmlAndFile(0);
        }
        private void button_savePLCAlarmTb_Click(object sender, EventArgs e)
        {
            SaveDgvData2XmlAndFile(0);
        }

        //总数的修改和保存
        private void setSave_Click(object sender, EventArgs e)
        {
            if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[2]) != txtWSSet.Text ||
                MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[3]) != txtPLSet.Text ||
                (dgvPLCSet.CurrentRow != null && MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC_Item + dgvPLCSet.CurrentRow.Index.ToString(),
                                            -1, m_xmlDociment.Default_Attributes_str1[5]) != comboBoxSelePLCSystem.Text) ||
               MainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]) != comboBoxLineType.Text ||
                MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]) != comboBox_LocalIpAddr.Text)
            {
                if (MessageBox.Show(MessageString.SetForms_DataUpdate,MessageString.SetForms_Information, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[2]) != txtWSSet.Text)
                    {
                        MainForm.m_xml.ChangeDefault_AllAttributes(m_xmlDociment.Default_Attributes_str1[2], txtWSSet.Text);
                    }
                    if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[3]) != txtPLSet.Text)
                    {
                        MainForm.m_xml.ChangeDefault_AllAttributes(m_xmlDociment.Default_Attributes_str1[3], txtPLSet.Text);
                    }
                    if (dgvPLCSet.CurrentRow != null)
                    {
                        if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC_Item + dgvPLCSet.CurrentRow.Index.ToString(),
                                            -1, m_xmlDociment.Default_Attributes_str1[5]) != comboBoxSelePLCSystem.Text)//说明PLC系统被改变
                        {
                            MainForm.m_xml.ChangeDefault_PlcSystemAttributes(dgvPLCSet.CurrentRow.Index, comboBoxSelePLCSystem.Text);
                            refreshXmlData2DGV(m_xmlDociment.PathRoot_PLC, dgvPLCSet);
                            BindingLisReadValue(dgvPLCSet);
                            UpDataPLCItemMessge(dgvPLCSet.CurrentRow.Index);
                        }

                    }
                    if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]) != comboBox_LocalIpAddr.Text)//CNC使用的本地
                    {
                        MainForm.m_xml.m_UpdateAttribute(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0], comboBox_LocalIpAddr.Text);
                    }
                    if (MainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]) != comboBoxLineType.Text)
                    {
                        MainForm.m_xml.m_UpdateAttribute(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0], comboBoxLineType.Text);
                    }

                    MainForm.m_xml.SaveXml2File(MainForm.XMLSavePath);
                }
                else
                {
                }
//                 MainForm.m_xml.SaveXml2File(MainForm.XMLSavePath);
            }
            return;
        }
        #endregion

        #region      客户切换界面
        private void tabControlSet_Selecting(object sender, TabControlCancelEventArgs e)
        {
            SaveDgvData2XmlAndFile(0);
        }
        private void tabControlSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlSet.SelectedIndex == 0)
            {
                txtCNCSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[0]);//CNC数量
                txtRobotSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_ROBOT, -1, m_xmlDociment.Default_Attributes_str1[0]);//机器人数量
                txtPLCSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, -1, m_xmlDociment.Default_Attributes_str1[0]);//PLC数量
                txtRFIDSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, -1, m_xmlDociment.Default_Attributes_str1[0]);//RFIDs数量
                //车间和产线的显示  
                txtWSSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[2]);//车间标识
                txtPLSet.Text = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[3]);//产线标识
            }
            else if (tabControlSet.SelectedIndex == 1)
            {
                refreshXmlData2DGV(m_xmlDociment.PathRoot_CNC, dgvCNCSet);
                BindingLisReadValue(dgvCNCSet);
            }
            else if (tabControlSet.SelectedIndex == 2)
            {
                refreshXmlData2DGV(m_xmlDociment.PathRoot_ROBOT, dgvRobotSet);
                BindingLisReadValue(dgvRobotSet);

                String pathstr = m_xmlDociment.PathRoot_ROBOT_Item + dgvRobotSet.CurrentRow.Index.ToString()
                    + "/" + m_xmlDociment.Default_Path_str[6];
                refreshXmlData2DGV(pathstr, DGVRobotInputSignalS);
                BindingLisReadValue(DGVRobotInputSignalS);

                pathstr = m_xmlDociment.PathRoot_ROBOT_Item + dgvRobotSet.CurrentRow.Index.ToString()
                   + "/" + m_xmlDociment.Default_Path_str[7];
                refreshXmlData2DGV(pathstr, DGVRobotOutputSignalS);
                BindingLisReadValue(DGVRobotOutputSignalS);

            }
            else if (tabControlSet.SelectedIndex == 3)
            {
                refreshXmlData2DGV(m_xmlDociment.PathRoot_PLC, dgvPLCSet);
                BindingLisReadValue(dgvPLCSet);

                if (dgvPLCSet.CurrentRow != null)
                {
                    UpDataPLCItemMessge(dgvPLCSet.CurrentRow.Index);
                }
            }
            else if (tabControlSet.SelectedIndex == 4)
            {
                refreshXmlData2DGV(m_xmlDociment.PathRoot_RFID, dgvRFIDSet);
                BindingLisReadValue(dgvRFIDSet);
            }
            else if (tabControlSet.SelectedIndex == 6)
            {
                refreshXmlData2DGV(m_xmlDociment.PathRoot_PLCAlarmTb, dgv_PLCAlarmTb);
                BindingLisReadValue(dgv_PLCAlarmTb);
            }
        }

        #endregion

        #region   dgvCNCSet数据合法性检查
        string dgvCNCSet_Cell_Old;
        private void dgvCNCSet_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dgvCNCSet_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dgvCNCSet_Cell_Old = "";
            }
        }
        private void dgvCNCSet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, btnSaveCNC))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvCNCSet_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvCNCSet_Cell_Old;
                    return;
                }
                if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.port)//端口合法性检查
                {
                    if (int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 1024)
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvCNCSet_Cell_Old;
                        return;
                    }
                }
//                 else if (e.ColumnIndex == 1)//ID合法性检查
//                 {
//                     if (int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvCNCSet_Cell_Old;
//                         return;
//                     }
//                 }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.ip)//IP合法性检查
                {
                    System.Net.IPAddress ip;
                    if (!System.Net.IPAddress.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ip))
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvCNCSet_Cell_Old;
                        return;
                    }
                }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != dgvCNCSet_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvCNCSet_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region   dgvRobotSet数据合法性检查
        string dgvRobotSet_Cell_Old;
        private void dgvRobotSet_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dgvRobotSet_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dgvRobotSet_Cell_Old = "";
            }
        }
        private void dgvRobotSet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, btnSaveROBOT))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRobotSet_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRobotSet_Cell_Old;
                    return;
                }

                if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.port)//端口合法性检查
                {
                    if (int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 1024)
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRobotSet_Cell_Old;
                        return;
                    }
                }
//                 else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.id)//ID合法性检查
//                 {
//                     if (int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRobotSet_Cell_Old;
//                         return;
//                     }
//                 }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.ip)//IP合法性检查
                {
                    System.Net.IPAddress ip;
                    if (!System.Net.IPAddress.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ip))
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRobotSet_Cell_Old;
                        return;
                    }
                }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != dgvRobotSet_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRobotSet_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region   dgvPLCSet数据合法性检查
        string dgvPLCSet_Cell_Old;
        private void dgvPLCSet_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dgvPLCSet_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dgvPLCSet_Cell_Old = "";
            }
        }
        private void dgvPLCSet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, btnSavePLC))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCSet_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCSet_Cell_Old;
                    return;
                }

                if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.port)//端口合法性检查
                {
                    if (int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 1024)
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCSet_Cell_Old;
                        return;
                    }
                }
//                 else if (e.ColumnIndex == 1)//ID合法性检查
//                 {
//                     if (int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCSet_Cell_Old;
//                         return;
//                     }
//                 }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.ip)//IP合法性检查
                {
                    System.Net.IPAddress ip;
                    if (!System.Net.IPAddress.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ip))
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCSet_Cell_Old;
                        return;
                    }
                }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != dgvPLCSet_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCSet_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region   dgvRFIDSet数据合法性检查
        string dgvRFIDSet_Cell_Old;
        private void dgvRFIDSet_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dgvRFIDSet_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dgvRFIDSet_Cell_Old = "";
            }
        }
        private void dgvRFIDSet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, btnSaveRFID))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                    return;
                }
                String PLCSystem = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, int.Parse(dgv.Rows[e.RowIndex].Cells[4].Value.ToString()), m_xmlDociment.Default_Attributes_str1[5]);
                if (e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.PLCserial)//所属PLC的序号合法性检查
                {
                    Int32 PLCSel = int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    if (PLCSel < 0 || PLCSel >= Int32.Parse(MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, -1, m_xmlDociment.Default_Attributes_str1[0])))
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                        return;
                    }
                }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.ReadDevice
                    || e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.WriteDevice)//地址类型合法性检查
                {
                    bool find = false;
                    if (PLCSystem == m_xmlDociment.PLC_System[0])
                    {
                        if (m_xmlDociment.Default_MITSUBISHI_Device1.Contains(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                        {
                            find = true;
                        }
                        if (m_xmlDociment.Default_MITSUBISHI_Device2.Contains(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                        {
                            find = true;
                        }
                    }
                    else if (PLCSystem == m_xmlDociment.PLC_System[1])
                    {
                        if (m_xmlDociment.Default_HNC8_Device1.Contains(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                        {
                            find = true;
                        }
                        if (m_xmlDociment.Default_HNC8_Device2.Contains(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                        {
                            find = true;
                        }
                    }

                    if (!find)
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                        return;
                    }
                }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.ReadAddressStar ||
                    e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.WriteAddressStar)//起始地址
                {
                    String PLCDevice = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString();
                    String[] AdressType = MainForm.m_xml.FindDeviceAddress(ref PLCDevice, ref PLCSystem).Split('-');
                    if (PLCSystem == m_xmlDociment.PLC_System[0])//三菱地址合法性检查
                    {
//                         Int32 Adreaaii = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), Int32.Parse(AdressType[0]));
//                         Int32 Adreaaiinim = Convert.ToInt32(AdressType[1], Int32.Parse(AdressType[0]));
//                         Int32 Adreaaiimax = Convert.ToInt32(AdressType[2], Int32.Parse(AdressType[0]));
// 
//                         if (Adreaaii < Adreaaiinim || Adreaaii >= Adreaaiimax)
//                         {
//                             dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
//                             return;
//                         }
                    }
                    else if (PLCSystem == m_xmlDociment.PLC_System[1])
                    {
//                         String[] AdressSplit = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split('.');
//                         if (AdressSplit.Length == 1)
//                         {
//                             if (Int32.Parse(AdressSplit[0]) < Int32.Parse(AdressType[1]) ||
//                                 Int32.Parse(AdressSplit[0]) >= Int32.Parse(AdressType[2]))
//                             {
//                                 dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
//                                 return;
//                             }
//                         }
//                         else if (AdressSplit.Length == 2)
//                         {
//                             if (Int32.Parse(AdressSplit[1]) < 0 || Int32.Parse(AdressSplit[1]) >= Int32.Parse(AdressType[0]) ||
//                                 Int32.Parse(AdressSplit[0]) < Int32.Parse(AdressType[1]) ||
//                                 Int32.Parse(AdressSplit[0]) >= Int32.Parse(AdressType[2]))
//                             {
//                                 dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
//                                 return;
//                             }
//                         }
//                         else
//                         {
//                             dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
//                             return;
//                         }
                    }
                }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.ReadAddressSet
                    || e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.WriteAddressSet)//地址地址格式
                {
                    String[] str = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split(',');
                    String PLCDevice = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value.ToString();
                    String[] AdressType = MainForm.m_xml.FindDeviceAddress(ref PLCDevice, ref PLCSystem).Split('-');
                    Int32 Adreaaiinim = Convert.ToInt32(AdressType[1], Int32.Parse(AdressType[0]));
                    Int32 Adreaaiimax = Convert.ToInt32(AdressType[2], Int32.Parse(AdressType[0]));
                    Int32 add = 0;
                    for (int ii = 0; ii < str.Length; ii++)
                    {
                        int strii = Int32.Parse(str[ii]);
                        add += strii;
                        if (strii < Adreaaiinim || strii >= Adreaaiimax || add >= Adreaaiimax)
                        {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                            return;
                        }
                    }
                }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.MonitorBit)//监控位
                {
                    /*
                    String[] str = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split(',');
                    if (str.Length != 2)
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                        return;
                    }
                    String PLCDevice = str[0];
                    String[] AdressType = MainForm.m_xml.FindDeviceAddress(ref PLCDevice, ref PLCSystem).Split('-');

                    bool find = false;
                    if (PLCSystem == m_xmlDociment.PLC_System[0])
                    {
                        if (m_xmlDociment.Default_MITSUBISHI_Device1.Contains(PLCDevice))
                        {
                            find = true;
                        }
                        if (m_xmlDociment.Default_MITSUBISHI_Device2.Contains(PLCDevice))
                        {
                            find = true;
                        }

                        Int32 Adreaaiinim = Convert.ToInt32(AdressType[1], Int32.Parse(AdressType[0]));
                        Int32 Adreaaiimax = Convert.ToInt32(AdressType[2], Int32.Parse(AdressType[0]));
                        Int32 add = Int32.Parse(str[1]);
                        if (add < Adreaaiinim || add >= Adreaaiimax)//
                        {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                            return;
                        }
// 
//                         if (PLCDevice == m_xmlDociment.Default_MITSUBISHI_Device1[0] || PLCDevice == m_xmlDociment.Default_MITSUBISHI_Device2[0])
//                         {
//                             Adreaaiinim = 0;
//                             Adreaaiimax = 1;
//                         }
//                         else
//                         {
//                             Adreaaiinim = 0;
//                             Adreaaiimax = 16;
//                         }
//                         if (Int32.Parse(str[2]) < Adreaaiinim || Int32.Parse(str[2]) >= Adreaaiimax)
//                         {
//                             dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
//                             return;
//                         }
// 
                    }
                    else if (PLCSystem == m_xmlDociment.PLC_System[1])
                    {
                        if (m_xmlDociment.Default_HNC8_Device1.Contains(PLCDevice))
                        {
                            find = true;
                        }
                        if (m_xmlDociment.Default_HNC8_Device2.Contains(PLCDevice))
                        {
                            find = true;
                        }

                        Int32 Adreaaiinim = Int32.Parse(AdressType[1]);
                        Int32 Adreaaiimax = Int32.Parse(AdressType[2]);
                        Int32 add = Int32.Parse(str[1]);
                        if (add < Adreaaiinim || add >= Adreaaiimax)
                        {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                            return;
                        }
//                         if (PLCDevice == m_xmlDociment.Default_HNC8_Device1[0] || PLCDevice == m_xmlDociment.Default_HNC8_Device2[0])
//                         {
//                             Adreaaiinim = 0;
//                             Adreaaiimax = 1;
//                         }
//                         else
//                         {
//                             Adreaaiinim = 0;
//                             Adreaaiimax = int.Parse(AdressType[0]);
//                         }
//                         if (Int32.Parse(str[2]) < Adreaaiinim || Int32.Parse(str[2]) >= Adreaaiimax)
//                         {
//                             dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
//                             return;
//                         }
                    }
                    if (!find)
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                        return;
                    }*/
                }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.ip)//IP合法性检查
                {
                    System.Net.IPAddress ip;
                    if (!System.Net.IPAddress.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ip))
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                        return;
                    }
                }
                else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_RFID.port)//端口合法性检查
                {
                    if (int.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 1024)
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                        return;
                    }
                }

                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != dgvRFIDSet_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvRFIDSet_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region   DGVRobotInputSignalS数据合法性检查
        string DGVRobotInputSignalS_Cell_Old;
        private void DGVRobotInputSignalS_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, btnSaveROBOT))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotInputSignalS_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotInputSignalS_Cell_Old;
                    return;
                }

//                 if (e.ColumnIndex == 1)//16进制地址合法性检查
//                 {
//                     if (Convert.ToInt64(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), 16) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotInputSignalS_Cell_Old;
//                         return;
//                     }
//                 }
//                 else if (e.ColumnIndex == 4 || e.ColumnIndex == 5)//ID合法性检查
//                 {
//                     if (Int64.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotInputSignalS_Cell_Old;
//                         return;
//                     }
//                 }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != DGVRobotInputSignalS_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split('\t')[0];
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotInputSignalS_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        private void DGVRobotInputSignalS_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                DGVRobotInputSignalS_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                DGVRobotInputSignalS_Cell_Old = "";
            }
        }
        #endregion

        #region   DGVRobotOutputSignalS数据合法性检查
        string DGVRobotOutputSignalS_Cell_Old;
        private void DGVRobotOutputSignalS_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                DGVRobotOutputSignalS_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                DGVRobotOutputSignalS_Cell_Old = "";
            }
        }
        private void DGVRobotOutputSignalS_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, btnSaveROBOT))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotOutputSignalS_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotOutputSignalS_Cell_Old;
                    return;
                }

//                 if (e.ColumnIndex == 1)//16进制地址合法性检查
//                 {
//                     if (Convert.ToInt64(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), 16) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotOutputSignalS_Cell_Old;
//                         return;
//                     }
//                 }
//                 else if (e.ColumnIndex == 4 || e.ColumnIndex == 5)//ID合法性检查
//                 {
//                     if (Int64.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotOutputSignalS_Cell_Old;
//                         return;
//                     }
//                 }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()!= DGVRobotOutputSignalS_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DGVRobotOutputSignalS_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region   PLCInputSignalSDefine数据合法性检查
        string PLCInputSignalSDefine_Cell_Old;
        private void PLCInputSignalSDefine_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                PLCInputSignalSDefine_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                PLCInputSignalSDefine_Cell_Old = "";
            }
        }
        private void PLCInputSignalSDefine_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, btnSavePLC))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
                    return;
                }

                if (e.ColumnIndex == 1)//地址合法性检查
                {
                    /*
                    String PLCDevice = comboBoxPLCDevice1.Text;
                    String PLCSystem = comboBoxSelePLCSystem.Text;
                    String[] AdressType = MainForm.m_xml.FindDeviceAddress(ref PLCDevice, ref PLCSystem).Split('-');
                    if (PLCSystem == m_xmlDociment.PLC_System[0])//三菱地址合法性检查
                    {
                        Int32 Adreaaii = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), Int32.Parse(AdressType[0]));
                        Int32 Adreaaiinim = Convert.ToInt32(AdressType[1], Int32.Parse(AdressType[0]));
                        Int32 Adreaaiimax = Convert.ToInt32(AdressType[2], Int32.Parse(AdressType[0]));

                        if (Adreaaii < Adreaaiinim || Adreaaii >= Adreaaiimax)
                        {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
                            return;
                        }
                    }
                    else if (PLCSystem == m_xmlDociment.PLC_System[1])
                    {
                        String[] AdressSplit = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split('.');
                        if (AdressSplit.Length == 1)
                        {
                            if (Int32.Parse(AdressSplit[0]) < Int32.Parse(AdressType[1]) ||
                                Int32.Parse(AdressSplit[0]) >= Int32.Parse(AdressType[2]))
                            {
                                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
                                return;
                            }
                        }
                        else if (AdressSplit.Length == 2)
                        {
                            if (Int32.Parse(AdressSplit[1]) < 0 || Int32.Parse(AdressSplit[1]) >= Int32.Parse(AdressType[0])||
                                Int32.Parse(AdressSplit[0]) < Int32.Parse(AdressType[1]) || 
                                Int32.Parse(AdressSplit[0]) >= Int32.Parse(AdressType[2]))
                            {
                                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
                                return;
                            }
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
                            return;
                        }
                    }*/
                }
//                 else if (e.ColumnIndex == 4 || e.ColumnIndex == 5)//ID合法性检查
//                 {
//                     if (Int64.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
//                         return;
//                     }
//                 }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != PLCInputSignalSDefine_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region   PLCOutputSignalSDefine数据合法性检查
        string PLCOutputSignalSDefine_Cell_Old;
        private void PLCOutputSignalSDefine_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                PLCOutputSignalSDefine_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                PLCOutputSignalSDefine_Cell_Old = "";
            }
        }
        private void PLCOutputSignalSDefine_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (!CheckUserPassWord(sender, btnSavePLC))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCOutputSignalSDefine_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCOutputSignalSDefine_Cell_Old;
                    return;
                }

                if (e.ColumnIndex == 1)//地址合法性检查
                {
                    /*
                    String PLCDevice = comboBoxPLCDevice2.Text;
                    String PLCSystem = comboBoxSelePLCSystem.Text;
                    String[] AdressType = MainForm.m_xml.FindDeviceAddress(ref PLCDevice, ref PLCSystem).Split('-');
                    if (PLCSystem == m_xmlDociment.PLC_System[0])//三菱地址合法性检查
                    {
                        Int32 Adreaaii = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), Int32.Parse(AdressType[0]));
                        Int32 Adreaaiinim = Convert.ToInt32(AdressType[1], Int32.Parse(AdressType[0]));
                        Int32 Adreaaiimax = Convert.ToInt32(AdressType[2], Int32.Parse(AdressType[0]));

                        if (Adreaaii < Adreaaiinim || Adreaaii >= Adreaaiimax)
                        {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCOutputSignalSDefine_Cell_Old;
                            return;
                        }
                    }
                    else if (PLCSystem == m_xmlDociment.PLC_System[1])
                    {
                        String[] AdressSplit = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split('.');
                        if (AdressSplit.Length == 1)
                        {
                            if (Int32.Parse(AdressSplit[0]) < Int32.Parse(AdressType[1]) ||
                                Int32.Parse(AdressSplit[0]) >= Int32.Parse(AdressType[2]))
                            {
                                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCOutputSignalSDefine_Cell_Old;
                                return;
                            }
                        }
                        else if (AdressSplit.Length == 2)
                        {
                            if (Int32.Parse(AdressSplit[1]) < 0 || Int32.Parse(AdressSplit[1]) >= Int32.Parse(AdressType[0]) ||
                                Int32.Parse(AdressSplit[0]) < Int32.Parse(AdressType[1]) ||
                                Int32.Parse(AdressSplit[0]) >= Int32.Parse(AdressType[2]))
                            {
                                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCOutputSignalSDefine_Cell_Old;
                                return;
                            }
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCOutputSignalSDefine_Cell_Old;
                            return;
                        }
                    }*/
                }
//                 else if (e.ColumnIndex == 4 || e.ColumnIndex == 5)//ID合法性检查
//                 {
//                     if (Int64.Parse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) < 0)
//                     {
//                         dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCInputSignalSDefine_Cell_Old;
//                         return;
//                     }
//                 }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != PLCOutputSignalSDefine_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }
            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = PLCOutputSignalSDefine_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region   dgvPLCAlarmTb数据合法性检查
        string dgvPLCAlarmTb_Cell_Old;
        private void dgv_PLCAlarmTb_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dgvPLCAlarmTb_Cell_Old = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dgvPLCAlarmTb_Cell_Old = "";
            }
        }
        private void dgv_PLCAlarmTb_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!CheckUserPassWord(sender, button_savePLCAlarmTb))
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCAlarmTb_Cell_Old;
                return;
            }

            try
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCAlarmTb_Cell_Old;
                    return;
                }
                if (e.ColumnIndex == 1)//报警号
                {
                    int outint = 0;
                    if (!int.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(),out outint))
                    {
                        dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCAlarmTb_Cell_Old;
                        return;
                    }
                }
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != dgvPLCAlarmTb_Cell_Old && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    RefreshDgvDataChangeF(dgv);
                }

            }
            catch (System.Exception ex)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dgvPLCAlarmTb_Cell_Old;
                Console.WriteLine(ex.StackTrace);
            }
        }

        #endregion


        #region   dgv初始化
        private void InitdgvSet(ref DataGridView dgv, ref String[] dgvRFIDSetColumnAttributes_str)
        {
            dgv.ColumnCount = dgvRFIDSetColumnAttributes_str.Length;
            for (int ii = 0; ii < dgv.ColumnCount; ii++)
            {
                dgv.Columns[ii].HeaderText = dgvRFIDSetColumnAttributes_str[ii];
            }
        }
        #endregion

        private void dgvRobotSet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRobotSet.CurrentRow.Index != dgvRobotSet_CurrentRow_Index_Old)
            {
                String pathstr = m_xmlDociment.PathRoot_ROBOT_Item + dgvRobotSet.CurrentRow.Index.ToString()
                    + "/" + m_xmlDociment.Default_Path_str[6];
                if (dgvRobotSet.CurrentRow.Index < dgvRobotSet.RowCount - 1)
                {
                    if (!MainForm.m_xml.CheckNodeExist(pathstr))
                    {
                        SaveDgvData2XmlAndFile(1);
                    }
                }
                refreshXmlData2DGV(pathstr, DGVRobotInputSignalS);
                RefreshDgvXmlPath(DGVRobotInputSignalS, dgvRobotSet.CurrentRow.Index);
                BindingLisReadValue(DGVRobotInputSignalS);

                pathstr = m_xmlDociment.PathRoot_ROBOT_Item + dgvRobotSet.CurrentRow.Index.ToString()
                   + "/" + m_xmlDociment.Default_Path_str[7];
                refreshXmlData2DGV(pathstr, DGVRobotOutputSignalS);
                RefreshDgvXmlPath(DGVRobotOutputSignalS, dgvRobotSet.CurrentRow.Index);
                BindingLisReadValue(DGVRobotOutputSignalS);

                label_SeleRobotIntS.Text = MessageString.SetForms_SN_As + dgvRobotSet.CurrentRow.Index.ToString() +MessageString.SetForms_Robot_InputSignalCount;
                label_SeleRobotOutS.Text = MessageString.SetForms_SN_As + dgvRobotSet.CurrentRow.Index.ToString() + MessageString.SetForms_Robot_OutputSignalCount;
                dgvRobotSet_CurrentRow_Index_Old = dgvRobotSet.CurrentRow.Index;
            }
        }


        private void dgvPLCSet_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPLCSet.CurrentRow != null && dgvPLCSet_CurrentRow_Index_Old != e.RowIndex)
            {
                UpDataPLCItemMessge(e.RowIndex);
                dgvPLCSet_CurrentRow_Index_Old = e.RowIndex;
            }
        }
        private void comboBoxSelePLCSystem_Leave(object sender, EventArgs e)
        {
            if(dgvPLCSet.CurrentRow != null)
            {
                SaveDgvData2XmlAndFile(0);
            }
        }

        private Int32 PLCIdex_old = -1;
        private void UpDataPLCItemMessge(Int32 PLCIdex)
        {
            String Path = m_xmlDociment.PathRoot_PLC_Item +  PLCIdex;
            if(MainForm.m_xml.CheckNodeExist(Path))
            {
                String PlcSystem = MainForm.m_xml.m_Read(Path, -1, m_xmlDociment.Default_Attributes_str1[5]);
                UpDataPLCItemystemShow(PlcSystem);
                RefreshDgvXmlPath(PLCInputSignalSDefine, PLCIdex);
                RefreshDgvXmlPath(PLCOutputSignalSDefine, PLCIdex);

                String pathstr = Path + "/" + comboBoxPLCDevice1.Text;//MainForm.m_xml.Default_Path_str[6];
                refreshXmlData2DGV(pathstr, PLCInputSignalSDefine);
                BindingLisReadValue(PLCInputSignalSDefine);

                pathstr = Path + "/" + comboBoxPLCDevice2.Text;//MainForm.m_xml.Default_Path_str[7];
                refreshXmlData2DGV(pathstr, PLCOutputSignalSDefine);
                BindingLisReadValue(PLCOutputSignalSDefine);

                LablPLCXuHao1.Text = "PLC"+MessageString.SetForms_SN_As+":" + PLCIdex.ToString();
                if (PLCIdex_old != PLCIdex)
                {
                    comboBoxPLCDevice1_SelectedIndex_Old = 0;
                    comboBoxPLCDevice2_SelectedIndex_Old = 0;
                    PLCIdex_old = PLCIdex;
                }

                comboBoxPLCDevice1.SelectedIndex = comboBoxPLCDevice1_SelectedIndex_Old;
                comboBoxPLCDevice2.SelectedIndex = comboBoxPLCDevice2_SelectedIndex_Old;

                if (PlcSystem == m_xmlDociment.PLC_System[0])///初始化comboBoxPLCDevice1、comboBoxPLCDevice2
                {
                    comboBoxPLCDevice1.DataSource = m_xmlDociment.Default_MITSUBISHI_Device1;
                    comboBoxPLCDevice2.DataSource = m_xmlDociment.Default_MITSUBISHI_Device2;
                }
                else if (PlcSystem == m_xmlDociment.PLC_System[1])
                {
                    comboBoxPLCDevice1.DataSource = m_xmlDociment.Default_HNC8_Device1;
                    comboBoxPLCDevice2.DataSource = m_xmlDociment.Default_HNC8_Device2;
                }
                 
                if (comboBoxPLCDevice1.Text == m_xmlDociment.Default_MITSUBISHI_Device1[2])//Buffer
                {
                    labeliStartIO.Visible = true;
                    textBoxStartIO.Visible = true;
                }
                else
                {
                    labeliStartIO.Visible = false;
                    textBoxStartIO.Visible = false;
                }
            }
            else
            {
                SaveDgvData2XmlAndFile(0);
                UpDataPLCItemMessge(PLCIdex);
            }
        }

        /// <summary>
        /// 更新comboBoxSelePLCSystem选择数据
        /// </summary>
        /// <param name="PLCIdex"></param>
        private void UpDataPLCItemystemShow(String PLCSystem)
        {
            for (int ii = 0; ii < comboBoxSelePLCSystem.Items.Count; ii++)
            {
                comboBoxSelePLCSystem.SelectedIndex = ii;
                if (PLCSystem == comboBoxSelePLCSystem.Text)
                    break;
            }
        }


        /// <summary>
        /// 将Xml数据刷新到DGV
        /// </summary>
        /// <param name="Pathstr"></param>
        /// <param name="DGV"></param>
        private void refreshXmlData2DGV(string Pathstr, DataGridView DGV)
        {
            DGV.Rows.Clear();
            if (MainForm.m_xml.CheckNodeExist(Pathstr))
            {
                int ITSUM = int.Parse(MainForm.m_xml.m_Read(Pathstr, -1, m_xmlDociment.Default_Attributes_str1[0]));//SUM
                for (int ii = 0; ii < ITSUM; ii++)
                {
                    DataGridViewRow InserRows = new DataGridViewRow();
                    InserRows.CreateCells(DGV);
                    string inserPath = Pathstr + "/" + m_xmlDociment.Default_Path_str[5] + ii.ToString();
                    MainForm.m_xml.Attributes2GridViewRow(inserPath, ref InserRows);
                    DGV.Rows.Add(InserRows);
                }
            }
        }


        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="DGV"></param>
        /// <param name="Pathstr"></param>
        private void refreshDgvData2Xml(DataGridView DGV,string Pathstr)
        {
            if (!MainForm.m_xml.CheckNodeExist(Pathstr) && (DGV == PLCInputSignalSDefine || DGV == PLCOutputSignalSDefine))//节点不存在，新建
            {
                MainForm.m_xml.MakeXmlPLCDevicePath(Pathstr);
            }
            int ITemSUM = int.Parse(MainForm.m_xml.m_Read(Pathstr, -1, m_xmlDociment.Default_Attributes_str1[0]));//SUM
            if (DGV.Rows.Count > ITemSUM)//插入
            {
                for (int kk = 0; kk < (DGV.Rows.Count - ITemSUM); kk++)
                {
                    MainForm.m_xml.InserNode(Pathstr);
                }
            }
            else if (DGV.Rows.Count < ITemSUM)
            {
                for (int kk = 0; kk < (ITemSUM - DGV.Rows.Count ); kk++)
                {
                    MainForm.m_xml.DeleNode(Pathstr, int.Parse(MainForm.m_xml.m_Read(Pathstr, -1, m_xmlDociment.Default_Attributes_str1[0])) - 1);
                }
            }
            for (int jj = 0; jj < DGV.Rows.Count; jj++)
            {
                string Itempas = Pathstr + "/" + m_xmlDociment.Default_Path_str[5] + jj.ToString();
                DataGridViewRow r = DGV.Rows[jj];
                MainForm.m_xml.GridViewRow2XmlAttributes(Itempas , ref r);
            }
        }

        #region   右击弹出快捷菜单的响应函数
        /// <summary>
        /// 右击弹出快捷菜单的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            DataGridView dgv = (DataGridView)(this.Controls.Find(cms.Name, true)[0]);
            int OpCount = dgv.SelectedRows.Count;
            if (OpCount > 0)
            {
                int OpBegin = 0;
                for (int ii = 0; ii < dgv.Rows.Count; ii++)
                {
                    if (dgv.Rows[ii].Selected == true)
                    {
                        OpBegin = ii;
                        break;
                    }
                }


                if (dgv.Name == DGVRobotInputSignalS.Name || dgv.Name == DGVRobotOutputSignalS.Name)
                {
                    RefreshDgvXmlPath(dgv, dgvRobotSet.CurrentRow.Index);
                }
                else if (dgv.Name == PLCInputSignalSDefine.Name || dgv.Name == PLCOutputSignalSDefine.Name)
                {
                    RefreshDgvXmlPath(dgv, dgvPLCSet.CurrentRow.Index);
                }
                if (cms.Items[0] == e.ClickedItem)//添加
                {
                    DataGridViewRow InserRows = new DataGridViewRow();
                    InserRows.CreateCells(dgv);
                    if (dgv.ColumnCount == m_xmlDociment.Default_Attributesstr1_value.Length)
                    {
                        InserRows.SetValues(m_xmlDociment.Default_Attributesstr1_value);
                    }
                    else if (dgv.ColumnCount == m_xmlDociment.Default_Attributesstr2_value.Length)
                    {
                        InserRows.SetValues(m_xmlDociment.Default_Attributesstr2_value);
                    }
                    else if (dgv.ColumnCount == m_xmlDociment.Default_Attributes_RFID_value.Length)
                    {
                        InserRows.SetValues(m_xmlDociment.Default_Attributes_RFID_value);
                    }

                    dgv.Rows.Insert(OpBegin, InserRows);
                    for (int ii = OpBegin; ii < dgv.Rows.Count; ii++)
                    {
                        dgv.Rows[ii].Cells[0].Value = ii.ToString();
                    }
                    RefreshDgvDataChangeF(dgv);
                }
                else if (cms.Items[1] == e.ClickedItem && dgv.RowCount != 1)//删除
                {
                    if (OpCount == dgv.RowCount)
                    {
                        dgv.Rows[OpBegin].Selected = false;
                    }
                    for (int ii = dgv.SelectedRows.Count; ii > 0; ii--)
                    {
                        dgv.Rows.RemoveAt(dgv.SelectedRows[ii - 1].Index);
                    } 
                    for (int ii = OpBegin; ii < dgv.RowCount; ii++)
                    {
                        dgv.Rows[ii].Cells[0].Value = ii.ToString();
                    }
                    RefreshDgvDataChangeF(dgv);
                }
                BindingLisReadValue(dgv);
            }
            dgv.ClearSelection();
        }


        /// <summary>
        /// 鼠标右击时调用函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = ((DataGridView)sender);
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                for (int ii = 0; ii < dgv.Rows.Count; ii++)
                {
                    for (int jj = 0; jj < dgv.ColumnCount; jj++)
                    {
                        if (dgv.Rows[ii].Cells[jj].Selected)
                        {
                            dgv.Rows[ii].Selected = true;
                            break;
                        }
                    }
                }
                ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
                contextMenuStrip1.Items.Add(MessageString.SetForms_ContextMenu_Add);
                contextMenuStrip1.Items.Add(MessageString.SetForms_ContextMenu_Delete);
                contextMenuStrip1.Name = dgv.Name;
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);
            }
            else if (e.Button == MouseButtons.Left && e.RowIndex > -1
                && e.ColumnIndex == 3 && dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null
                && (dgv.Name == PLCInputSignalSDefine.Name || dgv.Name == PLCOutputSignalSDefine.Name
                || dgv.Name == DGVRobotInputSignalS.Name || dgv.Name == DGVRobotOutputSignalS.Name))
            {
                if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() ==MessageString.SetForms_No)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =MessageString.SetForms_Yes;
                }
                else
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =MessageString.SetForms_No;
                }
                RefreshDgvDataChangeF(dgv);
            }

        }

        /// <summary>
        /// 右击列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCInputSignalSDefine_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMouseClick(sender, e);
        }

        private void PLCOutputSignalSDefine_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMouseClick(sender, e);
        }

        private void dgvCNCSet_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMouseClick(sender, e);
        }

        private void dgvRobotSet_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
//             dgvMouseClick(sender, e);
        }

        private void DGVRobotInputSignalS_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMouseClick(sender, e);
        }

        private void DGVRobotOutputSignalS_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMouseClick(sender, e);
        }

        private void dgvPLCSet_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
//             dgvMouseClick(sender, e);
        }

        private void dgvRFIDSet_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMouseClick(sender, e);
        }
        private void dgv_PLCAlarmTb_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMouseClick(sender, e);
        }

        #endregion
        /// <summary>
        /// 离开设置时检查是否需要保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetForm_Leave(object sender, EventArgs e)
        {
            SaveDgvData2XmlAndFile(0);
        }

        #region comboBoxPLCDevice操作
        private int comboBoxPLCDevice1_SelectedIndex_Old = -1;
        private void comboBoxPLCDevice1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvPLCSet.CurrentRow != null && comboBoxPLCDevice1_SelectedIndex_Old != comboBoxPLCDevice1.SelectedIndex)
            {
                String pathstr = m_xmlDociment.PathRoot_PLC_Item + dgvPLCSet.CurrentRow.Index.ToString()
                                + "/" + comboBoxPLCDevice1.Text;
                if (comboBoxPLCDevice1.Text == m_xmlDociment.Default_MITSUBISHI_Device1[2])//Buffer
                {
                    labeliStartIO.Visible = true;
                    textBoxStartIO.Visible = true;
                    if (MainForm.m_xml.CheckNodeExist(pathstr))
                    {
                        if (textBoxStartIO_Old.ToString() != MainForm.m_xml.m_Read(pathstr, -1, m_xmlDociment.Default_Attributes_str2[4]))
                        {
                            textBoxStartIO.Text = MainForm.m_xml.m_Read(pathstr, -1, m_xmlDociment.Default_Attributes_str2[4]);
                        }
                    }
                }
                else
                {
                    labeliStartIO.Visible = false;
                    textBoxStartIO.Visible = false;
                }

                SaveDgvData2XmlAndFile(0);
                refreshXmlData2DGV(pathstr, PLCInputSignalSDefine);
                BindingLisReadValue(PLCInputSignalSDefine);
                RefreshDgvXmlPath(PLCInputSignalSDefine, dgvPLCSet.CurrentRow.Index);
            }
            comboBoxPLCDevice1_SelectedIndex_Old = comboBoxPLCDevice1.SelectedIndex;
        }
        private int comboBoxPLCDevice2_SelectedIndex_Old = -1;
        private void comboBoxPLCDevice2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvPLCSet.CurrentRow != null && comboBoxPLCDevice2_SelectedIndex_Old != comboBoxPLCDevice2.SelectedIndex)
            {
                SaveDgvData2XmlAndFile(0);
                String pathstr = m_xmlDociment.PathRoot_PLC_Item + dgvPLCSet.CurrentRow.Index.ToString()
                           + "/" + comboBoxPLCDevice2.Text;
                refreshXmlData2DGV(pathstr, PLCOutputSignalSDefine);
                BindingLisReadValue(PLCOutputSignalSDefine);
                RefreshDgvXmlPath(PLCOutputSignalSDefine, dgvPLCSet.CurrentRow.Index);
            }
            comboBoxPLCDevice2_SelectedIndex_Old = comboBoxPLCDevice2.SelectedIndex;
        }
        #endregion

        private int textBoxStartIO_Old = 0;
        private void textBoxStartIO_TextChanged(object sender, EventArgs e)
        {
            if (textBoxStartIO.Text != textBoxStartIO_Old.ToString())
            {
                try
                {
                    int StartIO = int.Parse(textBoxStartIO.Text);
                    if (StartIO >= 0)
                    {
                        textBoxStartIO_Old = StartIO;
                    }
                    else
                    {
                        textBoxStartIO.Text = textBoxStartIO_Old.ToString();
                    }
                }
                catch
                {
                    textBoxStartIO.Text = textBoxStartIO_Old.ToString();
                }
            }
        }

        #region 数量改变按回车事件
        private void textBox_CNCNUM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSaveCNC.Focus();
            }
        }
        private void textBox_ROBOTNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSaveROBOT.Focus();
            }
        }
        private void textBox_SeleRobotIntSNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSaveROBOT.Focus();
            }
        }
        private void textBox_SeleRobotOutSNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSaveROBOT.Focus();
            }
        }
        private void textBox_PLCDVGROSNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSavePLC.Focus();
            }
        }
        private void textBox__SelePLCIntDVGROSNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSavePLC.Focus();
            }
        }
        private void textBox__SelePLCOutDVGROSNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSavePLC.Focus();
            }
        }
        private void textBox_RFidDgvRowsNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSaveRFID.Focus();
            }
        }
        private void textBox_PLCAlarmNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_savePLCAlarmTb.Focus();
            }
        }

        #endregion 

        #region 用户权限管理操作
        /// <summary>
        /// 登陆或者注销响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_UserOnOrOff_Click(object sender, EventArgs e)
        {
            if (button_UserOnOrOff.Text == UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGIN])
            {
                if (textBox_UserPassword1.Visible)
                {
                    if (MainForm.m_xml.CheckUserPassword(comboBox_UserNmae.Text, textBox_UserPassword1.Text))
                    {
                        label_CurrentUsername.Text = comboBox_UserNmae.Text;
                        button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGOUT];
                        MsgTipsHandler.BeginInvoke(label_Tisp, comboBox_UserNmae.Text + MessageString.SetForms_Login_Succeed, null, null);
                        LogIn = true;
                        LogInUserName = comboBox_UserNmae.Text;
                    }
                    else
                    {
                        MsgTipsHandler.BeginInvoke(label_Tisp, comboBox_UserNmae.Text + MessageString.SetForms_Login_failed, null, null);
                    }
                }
                else
                {
                    if (comboBox_UserNmae.SelectedIndex > 0)
                    {
                        label_CurrentUsername.Text = comboBox_UserNmae.Items[comboBox_UserNmae.SelectedIndex - 1].ToString();
                    }
                    else
                    {
                        label_CurrentUsername.Text = comboBox_UserNmae.Items[0].ToString();
                    }
                    MsgTipsHandler.BeginInvoke(label_Tisp, comboBox_UserNmae.Text + MessageString.SetForms_Login_Succeed, null, null);
                    if (comboBox_UserNmae.SelectedIndex == 1)
                    {
                        LogIn = false;
                    }
                }
            }
            else if (button_UserOnOrOff.Text == UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGOUT])
            {
                label_CurrentUsername.Text = comboBox_UserNmae.Items[0].ToString();
                button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGIN];
                MsgTipsHandler.BeginInvoke(label_Tisp, comboBox_UserNmae.Text + MessageString.SetForms_Logout_Succeed, null, null);
                LogIn = false;
            }
            else if (button_UserOnOrOff.Text == UserPasswordStrArr[(int)UserPasswordStrArrIndex.CANCEL])//取消修改密码
            {
                button_ChangeUserPassword.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.CHANGE_PWD];
                button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGOUT];
            }
            SetUserPasswoedItemShowOrHide();
        }

        private void textBox_UserPassword1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (textBox_UserPassword2.Visible)
                {
                    textBox_UserPassword2.Focus();
                }
                else
                {
                    button_UserOnOrOff_Click(null, null);
                }
            }
        }
        private void textBox_UserPassword2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_UserOnOrOff_Click(null, null);
            }
        }


        /// <summary>
        /// 修改用户密码响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ChangeUserPassword_Click(object sender, EventArgs e)
        {
            if (button_ChangeUserPassword.Text == UserPasswordStrArr[(int)UserPasswordStrArrIndex.CHANGE_PWD])
            {
                button_ChangeUserPassword.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.CONFIRM];
                button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.CANCEL];
            }
            else if (button_ChangeUserPassword.Text == UserPasswordStrArr[(int)UserPasswordStrArrIndex.CONFIRM])
            {
                if (textBox_UserPassword1.Text == textBox_UserPassword2.Text && textBox_UserPassword1.Text.Length >= 8)
                {
                    button_ChangeUserPassword.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.CHANGE_PWD];
                    button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGOUT];
                    MainForm.m_xml.SetUserPassword(comboBox_UserNmae.Text, textBox_UserPassword1.Text);
                    MainForm.m_xml.SaveXml2File(MainForm.XMLSavePath);
                    MsgTipsHandler.BeginInvoke(label_Tisp, comboBox_UserNmae.Text + "：修改密码成功！", null, null);
                }
                else
                {
                    if (textBox_UserPassword1.Text == textBox_UserPassword2.Text && textBox_UserPassword1.Text.Length < 8)
                    {
                        MsgTipsHandler.BeginInvoke(label_Tisp, comboBox_UserNmae.Text + MessageString.SetForms_ChangePwd_verify, null, null);
                    }
                    else
                    {
                        MsgTipsHandler.BeginInvoke(label_Tisp, comboBox_UserNmae.Text + MessageString.SetForms_ChangePwd_SamePwd, null, null);
                    }
                }
            }
            SetUserPasswoedItemShowOrHide();
        }

        private void SetUserPasswoedItemShowOrHide()
        {
            if(comboBox_UserNmae.Items.Count > 0)
            {
                if (label_CurrentUsername.Text == comboBox_UserNmae.Text)
                {
                    if (comboBox_UserNmae.SelectedIndex == 0)
                    {
                        label_UserName.Visible = true;
                        comboBox_UserNmae.Visible = true;

                        label_UserPasswor1.Visible = false;
                        label_UserPasswor2.Visible = false;
                        textBox_UserPassword1.Visible = false;
                        textBox_UserPassword2.Visible = false;
                        button_UserOnOrOff.Visible = false;
                        button_ChangeUserPassword.Visible = false;
                    }
                    else
                    {
                        if (button_ChangeUserPassword.Text == UserPasswordStrArr[(int)UserPasswordStrArrIndex.CHANGE_PWD])//可修改密码状态
                        {
                            label_UserName.Visible = true;
                            comboBox_UserNmae.Visible = true;

                            label_UserPasswor1.Visible = false;
                            label_UserPasswor2.Visible = false;
                            textBox_UserPassword1.Visible = false;
                            textBox_UserPassword2.Visible = false;
                            if (comboBox_UserNmae.SelectedIndex == 0)
                            {
                                button_UserOnOrOff.Visible = false;
                            }
                            else
                            {
                                button_UserOnOrOff.Visible = true;
                            }
                            button_ChangeUserPassword.Visible = true;
                        }
                        else if (button_ChangeUserPassword.Text == UserPasswordStrArr[(int)UserPasswordStrArrIndex.CONFIRM])//修改密码中
                        {
                            label_UserName.Visible = false;
                            comboBox_UserNmae.Visible = false;

                            label_UserPasswor1.Visible = true;
                            label_UserPasswor2.Visible = true;
                            textBox_UserPassword1.Visible = true;
                            textBox_UserPassword2.Visible = true;
                            textBox_UserPassword1.Text = "";
                            textBox_UserPassword2.Text = "";

                            button_UserOnOrOff.Visible = true;
                            button_ChangeUserPassword.Visible = true;
                        }
                    }
                }
                else
                {
//                     int ii = 0 ;
//                     for(;ii < comboBox_UserNmae.Items.Count;ii++)
//                     {
//                         if(comboBox_UserNmae.Items[ii].ToString() == label_CurrentUsername.Text)
//                         {
//                             break;
//                         }
//                     }


                    if (comboBox_UserNmae.SelectedIndex == 0)
                    {
                        label_UserName.Visible = true;
                        comboBox_UserNmae.Visible = true;

                        label_UserPasswor1.Visible = false;
                        label_UserPasswor2.Visible = false;
                        textBox_UserPassword1.Visible = false;
                        textBox_UserPassword2.Visible = false;
                        button_UserOnOrOff.Visible = false;
                        button_ChangeUserPassword.Visible = false;
                    }
                    else
                    {
                        label_UserName.Visible = true;
                        comboBox_UserNmae.Visible = true;

                        label_UserPasswor1.Visible = true;
                        label_UserPasswor2.Visible = false;
                        textBox_UserPassword1.Visible = true;
                        textBox_UserPassword2.Visible = false;
                        textBox_UserPassword1.Text = "";
                        button_UserOnOrOff.Visible = true;
                        button_ChangeUserPassword.Visible = false;
                    }

//                     if (ii < comboBox_UserNmae.SelectedIndex)//权限提升
//                     {
//                         label_UserName.Visible = true;
//                         comboBox_UserNmae.Visible = true;
// 
//                         label_UserPasswor1.Visible = true;
//                         label_UserPasswor2.Visible = false;
//                         textBox_UserPassword1.Visible = true;
//                         textBox_UserPassword2.Visible = false;
//                         textBox_UserPassword1.Text = "";
//                         button_UserOnOrOff.Visible = true;
//                         button_ChangeUserPassword.Visible = false;
// 
//                     }
//                     else
//                     {
//                         label_UserName.Visible = true;
//                         comboBox_UserNmae.Visible = true;
// 
//                         label_UserPasswor1.Visible = false;
//                         label_UserPasswor2.Visible = false;
//                         textBox_UserPassword1.Visible = false;
//                         textBox_UserPassword2.Visible = false;
//                         button_UserOnOrOff.Visible = true;
//                         button_ChangeUserPassword.Visible = false;
//                     }

                }

                //
                if (label_CurrentUsername.Text == m_xmlDociment.Default_Username_value[1])//管理者
                {
                    groupBox_UserManerge.Visible = true;
                }
                else
                {
                    groupBox_UserManerge.Visible = false;
                }
            }
            else
            {
                    label_UserName.Visible = false;
                    comboBox_UserNmae.Visible = false;
                    label_UserPasswor1.Visible = false;
                    label_UserPasswor2.Visible = false;
                    textBox_UserPassword1.Visible = false;
                    textBox_UserPassword2.Visible = false;
                    button_UserOnOrOff.Visible = false;
                    button_ChangeUserPassword.Visible = false;

            }
        }

        private void comboBox_UserNmae_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_UserNmae.Text == label_CurrentUsername.Text)//选择当前用户
            {
                button_ChangeUserPassword.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.CHANGE_PWD];
                button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGOUT];
            }
            else
            {
                button_UserOnOrOff.Text = UserPasswordStrArr[(int)UserPasswordStrArrIndex.LOGIN];
            }
            SetUserPasswoedItemShowOrHide();
        }

        private void MsgTipsHandlerFuc(object ob,String Str)
        {
            Label lb = (Label)ob;
            ThreaSetLaBText(lb, Str);
            System.Threading.Thread.Sleep(1800);
            if (lb.Text == Str)//最后一次才清空
            {
                ThreaSetLaBText(lb, "");
            }
        }

        /// <summary>
        /// 委托设置文本控件显示
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="str"></param>
        private void ThreaSetLaBText(Label LB, String str)
        {
           // if (LB.Text != str)
            {
                if (LB.InvokeRequired)//等待异步
                {
                    // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                    Action<string> actionDelegate = (x) =>
                    {
                        LB.Text = x;
                    };
                    LB.Invoke(actionDelegate, str);
                }
                else
                {
                    LB.Text = str;
                }
                if (str.Length != 0)
                {
                    String[] arrstr =  {LogData.LogDataNode1Name[(int)LogData.Node1Name.System_security],
                            LogData.LogDataNode2Level[(int)LogData.Node2Level.AUDIT], ((int)LogData.Node2Level.AUDIT).ToString(), "用户管理操作",
                            str};
                    LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                    SendParm.LevelIndex = (int)LogData.Node2Level.AUDIT;
                    SendParm.EventID = ((int)LogData.Node2Level.AUDIT).ToString();
                    SendParm.Keywords =MessageString.SetForms_User_Manage;
                    SendParm.EventData = str;
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }
            }
        }
        #endregion

        #region 用户权限限制和提醒
        public static bool LogIn = false;
        public static String LogInUserName = "";
        private bool CheckUserPassWord(object ob,Button bt)
        {
            if (!LogIn)
            {
                MessageBox.Show(LogInMessage, MessageString.SetForms_Information, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                bt.Focus();
                return false;
            }
            return true;
        }

        private void txtWSSet_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, setTest);
        }

        private void txtPLSet_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, setTest);
        }

        private void comboBoxLineType_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, setTest);
        }

        private void comboBox_LocalIpAddr_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, setTest);
        }
        private void textBox_CNCNUM_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSaveCNC);
        }

        private void dgvCNCSet_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSaveCNC);
        }

        private void dgvRobotSet_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSaveROBOT);
        }

        private void textBox_ROBOTNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSaveROBOT);
        }

        private void textBox_SeleRobotIntSNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSaveROBOT);
        }

        private void textBox_SeleRobotOutSNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSaveROBOT);
        }

        private void DGVRobotInputSignalS_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSaveROBOT);
        }

        private void DGVRobotOutputSignalS_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSaveROBOT);
        }

        private void dgvPLCSet_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSavePLC);
        }

        private void comboBoxSelePLCSystem_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSavePLC);
        }

        private void textBox_PLCDVGROSNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSavePLC);
        }

        private void textBoxStartIO_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSavePLC);
        }

        private void comboBoxPLCDevice1_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSavePLC);
        }

        private void textBox__SelePLCIntDVGROSNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSavePLC);
        }

        private void comboBoxPLCDevice2_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSavePLC);
        }

        private void textBox__SelePLCOutDVGROSNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSavePLC);
        }

        private void PLCInputSignalSDefine_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSavePLC);
        }

        private void PLCOutputSignalSDefine_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSavePLC);
        }

        private void dgvRFIDSet_Enter(object sender, EventArgs e)
        {
//             CheckUserPassWord(sender, btnSaveRFID);
        }

        private void textBox_RFidDgvRowsNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, btnSaveRFID);
        }
        private void textBox_PLCAlarmNum_Enter(object sender, EventArgs e)
        {
            CheckUserPassWord(sender, button_savePLCAlarmTb);
        }

        private void radioButton_AddUser_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_AddUser.Checked)//添加用户
            {
                textBox_ChangeUserName.Visible = true;
                textBox_ChangeUserName.Text = "";
                label_ChangeUserPasswor1.Visible = true;
                textBox_ChangeUserPasswor1.Visible = true;
                textBox_ChangeUserPasswor1.Text = "";
                label_ChangeUserPasswor2.Visible = true;
                textBox_ChangeUserPasswor2.Visible = true;
                textBox_ChangeUserPasswor2.Text = "";
                mComboBox_DeleUser.Visible = false;
                button_EditUserOK.Text = MessageString.SetForms_ContextMenu_Add;
            }
            else
            {
                textBox_ChangeUserName.Visible = false;
                label_ChangeUserPasswor1.Visible = false;
                textBox_ChangeUserPasswor1.Visible = false;
                label_ChangeUserPasswor2.Visible = false;
                textBox_ChangeUserPasswor2.Visible = false;
                mComboBox_DeleUser.Visible = true;
                button_EditUserOK.Text = MessageString.SetForms_ContextMenu_Delete;
                String[] str = MainForm.m_xml.GetUserNameStrArr();
                if(str.Length > 2)
                {
                    String[] str1 = new String[str.Length - 2];
                    for(int ii = 2;ii < str.Length; ii++)
                    {
                        str1[ii - 2] = str[ii];
                    }
                    mComboBox_DeleUser.DataSource = str1;
                }
                
            }
        }

        private void button_EditUserOK_Click(object sender, EventArgs e)//添加或者删除用户按钮相应
        {
            if (radioButton_AddUser.Checked)//添加用户
            {
                if (CheckNewUserNameAndPassword())
                {
                    MainForm.m_xml.InserNode(m_xmlDociment.PathRoot_User);
                    MainForm.m_xml.m_UpdateAttribute(m_xmlDociment.PathRoot_User, -2, m_xmlDociment.Default_Attributes_User[0], textBox_ChangeUserName.Text);//用户名
                    MainForm.m_xml.m_UpdateAttribute(m_xmlDociment.PathRoot_User, -2, m_xmlDociment.Default_Attributes_User[1],
                        MainForm.MakeSingSn(textBox_ChangeUserPasswor1.Text));//用户密码
                    MainForm.m_xml.SaveXml2File(MainForm.XMLSavePath);
                    comboBox_UserNmae.DataSource = MainForm.m_xml.GetUserNameStrArr();
                    MsgTipsHandler.BeginInvoke(label_ChangeUserTips, MessageString.SetForms_User_Add + textBox_ChangeUserName.Text, null, null);
                }
            }
            else
            {
                if (mComboBox_DeleUser.Items.Count > 0)
                {
                    MainForm.m_xml.DeleNode(m_xmlDociment.PathRoot_User, mComboBox_DeleUser.SelectedIndex + 2);
                    MainForm.m_xml.SaveXml2File(MainForm.XMLSavePath);
                    String[] str = MainForm.m_xml.GetUserNameStrArr();
                    comboBox_UserNmae.DataSource = str;
                    MsgTipsHandler.BeginInvoke(label_ChangeUserTips, MessageString.SetForms_User_Del + mComboBox_DeleUser.Text, null, null);

                    if (str.Length > 2)
                    {
                        String[] str1 = new String[str.Length - 2];
                        for (int ii = 2; ii < str.Length; ii++)
                        {
                            str1[ii - 2] = str[ii];
                        }
                        mComboBox_DeleUser.DataSource = str1;
                    }
                }
            }
        }


        private bool CheckNewUserNameAndPassword()//检查新添加的用户合法性
        {
            bool flg = false;
            if (textBox_ChangeUserName.Text.Length < 1)
            {
                MsgTipsHandler.BeginInvoke(label_ChangeUserTips, MessageString.SetForms_User_Add_VerifyLen, null, null);
            }
            else if (textBox_ChangeUserPasswor1.Text.Length < 6)
            {
                MsgTipsHandler.BeginInvoke(label_ChangeUserTips, MessageString.SetForms_User_Add_VerifyPwd, null, null);
            }
            else if (textBox_ChangeUserPasswor1.Text != textBox_ChangeUserPasswor2.Text)
            {
                MsgTipsHandler.BeginInvoke(label_ChangeUserTips, MessageString.SetForms_User_Add_VerifyPwdSame, null, null);
            }
            else if (comboBox_UserNmae.Items.Count > 100)
            {
                MsgTipsHandler.BeginInvoke(label_ChangeUserTips, MessageString.SetForms_User_Add_VerifyCount, null, null);
            }
            else
            {
                int ii = 0;
                for (; ii < comboBox_UserNmae.Items.Count;ii++ )
                {
                    if (textBox_ChangeUserName.Text == comboBox_UserNmae.Items[ii].ToString())
                    {
                        break;
                    }
                }
                if (ii < comboBox_UserNmae.Items.Count)
                {
                    MsgTipsHandler.BeginInvoke(label_ChangeUserTips, MessageString.SetForms_User_Add_VerifyExistUser, null, null);
                }
                else
                {
                    flg = true;
                }
            }

            return flg;
        }
        #endregion
    }


}
