using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LineDevice;
using System.Xml;
using ScadaHncData;
using System.IO;



namespace SCADA
{

    public partial class UserControlTaskData : UserControl
    {
        public ShareData TaskDataObj; //2015.10.24
        const string taskListPathXml = ".\\TaskList"; //2015.10.25
        const string cncTaskListXml = "\\CNCTaskList.xml";
        const string oldcncTaskListXml = "\\oldCNCTaskList.xml";

        const int resendNum = 3; //文件发送失败后的重发次数
        const int maxFileSize = 2; //发送单个文件大小最大默认为2M

        //2015.11.22   生产订单和机台编号 对换位置
        string[] STR_WORKORDER = {"编号", "机台编号", "派工单号", "物料编码", "生产数量", "计划开始时间", "计划完成时间", "NC编号", "NC版本号", "作业指导号", "作业指导版本号", "线体", "工序编码", "生产订单", "机台组", "操作标示", "序号", "时间戳", "日期", "NC文件路径", "NC作业指导书" };
        string[] STR_dataGridView_CNCGCode_Columns = { "CNC机台号","下载情况", "备注" };
        string[] STR_dataGridView_GCodeSele_Columns = { "G代码路径", "备注"};
        string[] MessageboxStr_登录 = {"你的操作权限不够，请先登录","提示" };
        private Color sendGcodeBackColor = Color.Chocolate;
        private Color frezenBackColor = Color.BurlyWood;

        System.Data.DataTable TaskDb;
        System.Data.DataTable dataGridView_CNCGCodeDb = new DataTable();
        System.Data.DataTable dataGridView_GCodeSeleDb = new DataTable();

        public EventHandler NCTaskMessageChangeHand;
        public UserControlTaskData()
        {
            InitializeComponent();
            NCTaskMessageChangeHand = new System.EventHandler(this.NCTaskMessageChangeHandFc);

            dataGridViewTask.AllowUserToAddRows = false;
            dataGridViewTask.ReadOnly = false;
            dataGridViewTask.RowHeadersVisible = false;

            dataGridView_CNCGCode.AllowUserToAddRows = false;
            dataGridView_CNCGCode.ReadOnly = false;
            dataGridView_CNCGCode.RowHeadersVisible = false;

            dataGridView_GCodeSele.AllowUserToAddRows = false;
            dataGridView_GCodeSele.ReadOnly = false;
            dataGridView_GCodeSele.RowHeadersVisible = false;

            DataGridViewCheckBoxColumn dtCheck = new DataGridViewCheckBoxColumn();
            dtCheck.HeaderText = "选择";
            dtCheck.ReadOnly = false;
            dtCheck.Selected = false;
            dataGridViewTask.Columns.Insert(0, dtCheck);
            dataGridViewTask.Columns[0].Frozen = true;
            this.dataGridViewTask.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置自动换行
            this.dataGridViewTask.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //设置自动调整高度

            dtCheck = new DataGridViewCheckBoxColumn();
            dtCheck.HeaderText = "选择";
            dtCheck.ReadOnly = false;
            dtCheck.Selected = false;
            dataGridView_CNCGCode.Columns.Insert(0, dtCheck);
            dataGridView_CNCGCode.Columns[0].Frozen = true;
            this.dataGridView_CNCGCode.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置自动换行
            this.dataGridView_CNCGCode.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //设置自动调整高度
            for (int ii = 0; ii < STR_dataGridView_CNCGCode_Columns.Length; ii++)
            {
                dataGridView_CNCGCodeDb.Columns.Add(STR_dataGridView_CNCGCode_Columns[ii]);
            }
            dataGridView_CNCGCode.DataSource = dataGridView_CNCGCodeDb;

            dtCheck = new DataGridViewCheckBoxColumn();
            dtCheck.HeaderText = "选择";
            dtCheck.ReadOnly = false;
            dtCheck.Selected = false;
            dataGridView_GCodeSele.Columns.Insert(0, dtCheck);
            dataGridView_GCodeSele.Columns[0].Frozen = true;
            this.dataGridView_GCodeSele.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置自动换行
            this.dataGridView_GCodeSele.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //设置自动调整高度
            for (int ii = 0; ii < STR_dataGridView_GCodeSele_Columns.Length; ii++)
            {
                dataGridView_GCodeSeleDb.Columns.Add(STR_dataGridView_GCodeSele_Columns[ii]);
            }
            dataGridView_GCodeSele.DataSource = dataGridView_GCodeSeleDb;
        }
        private void UserControlTaskData_Load(object sender, EventArgs e)
        {
//             if (TaskDataObj.m_workorderlist.Count == 0)
//             {
//                 TaskDb = SCADA.RFIDDATAT.DBReadFromXml(taskListPathXml + cncTaskListXml);
//                 DB2workorderlist(ref TaskDb, TaskDataObj.m_workorderlist);
//                 dataGridViewTask.DataSource = null;
//                 dataGridViewTask.DataSource = TaskDb;
//             }
//             TaskDb = null;
            
            if (MainForm.cnclist != null && MainForm.cnclist.Count > 0)
            {
                string[] TastarrList = new string[MainForm.cnclist.Count + 1];
                TastarrList[0] = "全部CNC任务";
                for (int ii = 0; ii < MainForm.cnclist.Count; ii++)
                {
                    string[] array = new string[STR_dataGridView_CNCGCode_Columns.Length];
                    array[0] = MainForm.cnclist[ii].BujianID;
                    array[1] = "";
                    array[2] = "";
                    dataGridView_CNCGCodeDb.Rows.Add(array);

                    TastarrList[ii + 1] = MainForm.cnclist[ii].BujianID + "任务";
                }
                mComboBox_CNCTaskList.DataSource = TastarrList;
            }
//             for (int ii = 0; ii < TaskDataObj.m_workorderlist.Count; ii++)
//             {
//                 LineDevice.CNC cnc = MainForm.cnclist.Find(
//                                 delegate(LineDevice.CNC temp)
//                                 {
//                                     return (temp.BujianID == TaskDataObj.m_workorderlist[ii].EQUIP_CODE);
//                                 }
//                                 );
//                 if (cnc != null)
//                 {
//                     cnc.NcTaskManage.AddMCTask(TaskDataObj.m_workorderlist[ii]);
//                 }
//             }

         }

        private void DB2workorderlist(ref System.Data.DataTable DB, List<MES_DISPATCH> MES_DISPATCH)
        {
            MES_DISPATCH.Clear();
            for (int index = 0; index < DB.Rows.Count; index++)
            {
                ScadaHncData.MES_DISPATCH nede = new ScadaHncData.MES_DISPATCH();
                nede.EQUIP_CODE = DB.Rows[index][1].ToString();
                nede.DISPATCH_CODE = DB.Rows[index][2].ToString();
                nede.MATERIAL_CODE = DB.Rows[index][3].ToString();
                nede.QTY = Convert.ToInt32(DB.Rows[index][4].ToString()); 
                nede.PLAN_START_DATE = Convert.ToDateTime(DB.Rows[index][5].ToString());
                nede.PLAN_END_DATE = Convert.ToDateTime(DB.Rows[index][6].ToString());
                nede.NC_ID = DB.Rows[index][7].ToString();
                nede.NC_VER = DB.Rows[index][8].ToString();
                nede.OP_DOC = DB.Rows[index][9].ToString();
                nede.OP_DOC_VER = DB.Rows[index][10].ToString();
                nede.LINE = DB.Rows[index][11].ToString();
                nede.OP_CODE = DB.Rows[index][12].ToString();
                nede.ORDER_CODE = DB.Rows[index][13].ToString();
                nede.EQUIP_GRP_CODE = DB.Rows[index][14].ToString();
                nede.FLAG = Convert.ToSByte(DB.Rows[index][15].ToString());
                nede.SN = Convert.ToDecimal(DB.Rows[index][16].ToString());
                nede.MARK_TIME = Convert.ToDateTime(DB.Rows[index][17].ToString());
                nede.MARK_DATE = DB.Rows[index][18].ToString();
                nede.NC_PATH = DB.Rows[index][19].ToString(); 
                nede.OP_DOC_PATH = DB.Rows[index][20].ToString();
                MES_DISPATCH.Add(nede);
              }
        }

        private System.Data.DataTable workorderlist2DB(List<MES_DISPATCH> MES_DISPATCH)
        {
            System.Data.DataTable DB = new DataTable();
            for (int index = 0; index < STR_WORKORDER.Length; index++)
            {
                DB.Columns.Add(STR_WORKORDER[index]);
            }
            for (int index = 0; index < MES_DISPATCH.Count; index++)
            {
                string[] array = new string[STR_WORKORDER.Length];
                array[0] = index.ToString();
                array[1] = MES_DISPATCH[index].EQUIP_CODE;
                array[2] = MES_DISPATCH[index].DISPATCH_CODE;
                array[3] = MES_DISPATCH[index].MATERIAL_CODE;
                array[4] = MES_DISPATCH[index].QTY.ToString();
                array[5] = MES_DISPATCH[index].PLAN_START_DATE.ToString();
                array[6] = MES_DISPATCH[index].PLAN_END_DATE.ToString();
                array[7] = MES_DISPATCH[index].NC_ID;
                array[8] = MES_DISPATCH[index].NC_VER;
                array[9] = MES_DISPATCH[index].OP_DOC;
                array[10] = MES_DISPATCH[index].OP_DOC_VER;
                array[11] = MES_DISPATCH[index].LINE;
                array[12] = MES_DISPATCH[index].OP_CODE;
                array[13] = MES_DISPATCH[index].ORDER_CODE;
                array[14] = MES_DISPATCH[index].EQUIP_GRP_CODE;
                array[15] = MES_DISPATCH[index].FLAG.ToString();
                array[16] = MES_DISPATCH[index].SN.ToString();
                array[17] = MES_DISPATCH[index].MARK_TIME.ToString();
                array[18] = MES_DISPATCH[index].MARK_DATE;
                array[19] = MES_DISPATCH[index].NC_PATH;
                array[20] = MES_DISPATCH[index].OP_DOC_PATH;
                array[21] = MES_DISPATCH[index].Short_name;
                DB.Rows.Add(array);
            }
            return DB;
        }


        public void AddTaskFormMes(int taskii)
        {
            if (taskii == 0)//派工单
            {
                for (int ii = 0; ii < TaskDataObj.m_workorderlist.Count; ii++)
                {
                    LineDevice.CNC cnc = MainForm.cnclist.Find(
                                    delegate(LineDevice.CNC temp)
                                    {
                                        return (temp.BujianID == TaskDataObj.m_workorderlist[ii].EQUIP_CODE);
                                    }
                                    );
                    if (cnc != null)
                    {
                        cnc.NcTaskManage.AddMCTask(TaskDataObj.m_workorderlist[ii]);
                    }
                }
            }
            else if (taskii == 1)//CNC切入切出命令
            {
                for (int ii = 0; ii < TaskDataObj.m_equipcmdlist.Count; ii++)
                {
                    LineDevice.CNC cnc = MainForm.cnclist.Find(
                                    delegate(LineDevice.CNC temp)
                                    {
                                        return (temp.BujianID == TaskDataObj.m_equipcmdlist[ii].EQUIP_CODE);
                                    }
                                    );
                    if (cnc != null)
                    {
                        cnc.CNCCMDChange(TaskDataObj.m_equipcmdlist[ii].EQUIP_CMD.ToString());
                    }
                }
            }
        }

/********************************************************************
	created:	2015/10/15
	created:	15:10:2015   14:45
	filename: 	F:\SCADA\SCADA\WindowsForm\UserControlTaskData.cs
	file path:	F:\SCADA\SCADA\WindowsForm
	file base:	UserControlTaskData
	file ext:	cs
	author:		
	
	purpose:	读取所有派工单中对应需要发送的G代码和作业指导书等 
*********************************************************************/
        private bool getCNCTaskData()  
        {
            if (TaskDb == null || TaskDb.Rows.Count < 0)
            {
                return false;
            }

            for (int i = 0; i < TaskDb.Rows.Count; i++)
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                SendParm.Keywords = "";
//                 if (checkBox_AoutoSendTask.Checked)//自动派发
//                 {
//                     if (sendTaskData2CNC(TaskDataObj.m_workorderlist[i].EQUIP_CODE, TaskDataObj.m_workorderlist[i].OP_DOC_PATH, i, checkBox_AoutoSendTask.Checked))
//                     {
//                         SendParm.Keywords = "派工单自动派发成功";
//                     }
//                     else
//                     {
//                         SendParm.Keywords = "派工单自动派发失败";
//                     }
//                 }
                if (gridRowIsChecked(i)) //选中发送
                {
                    System.Data.DataRow node = null;
                    LineDevice.CNC cnc = MainForm.cnclist.Find(
                                    delegate(LineDevice.CNC temp)
                                    {
                                        return (temp.BujianID == TaskDb.Rows[i][(int)NCTaskDel.NodeName.机台编号].ToString());
                                    }
                                    );
                    if (cnc != null)
                    {
                        cnc.NcTaskManage.GetDISPATCHSendOrRepor(true, out node);
                        if (node != null)
                        {
                            if (!sendTaskData2CNC(node[(int)NCTaskDel.NodeName.机台编号].ToString(), node[(int)NCTaskDel.NodeName.NC作业指导书路径].ToString(), i, checkBox_AoutoSendTask.Checked))
                            {
                                SendParm.Keywords = "派工单手动派发失败";
                            }
                            else
                            {
                                SendParm.Keywords = "派工单手动派发成功";
                                dataGridViewTask.Rows[i].Selected = true;
                            }
                        }
                        else
                        {
                            SendParm.Keywords = "任务已经派发";
                        }
                    }
                    else
                    {
                        SendParm.Keywords = "没有找到派工单对应机台 派工单手动派发失败";
                    }

                    
                }
                if (SendParm.Keywords.Length > 0)
                {
                    if (checkBox_XuNiSendTask.Checked)
                    {
                        SendParm.EventData = TaskDb.Rows[i][(int)NCTaskDel.NodeName.机台编号].ToString() + "虚拟派工；NC =" + TaskDb.Rows[i][(int)NCTaskDel.NodeName.NC文件路径].ToString();
                    }
                    else
                    {
                        SendParm.EventData = TaskDb.Rows[i][(int)NCTaskDel.NodeName.机台编号].ToString() + "实际派工；NC =" + TaskDb.Rows[i][(int)NCTaskDel.NodeName.NC文件路径].ToString();
                    }
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                }

            }
            return true;
        }

/********************************************************************
	created:	2015/10/15
	created:	15:10:2015   14:45
	filename: 	F:\SCADA\SCADA\WindowsForm\UserControlTaskData.cs
	file path:	F:\SCADA\SCADA\WindowsForm
	file base:	UserControlTaskData
	file ext:	cs
	author:		
	
	purpose:	全部选中所有派工单中 
*********************************************************************/
        private void selectAllCNCTaskData(bool bSelect = true)
        {
            for (int i = 0; i < dataGridViewTask.RowCount; i++)
            {
                if (sendlist.Contains(i))
                {
                    if (bSelect)
                        dataGridViewTask.Rows[i].Cells[0].Value = true;
                    else
                        dataGridViewTask.Rows[i].Cells[0].Value = false;
                }
            }
        }

        private void selectAllStrayData(bool bSelect = true)
        {
            if (TaskDataObj.m_straylist.Count < 1)
            {
                MessageBox.Show("小料盘无数据", "警告");
                return;
            }

            for (int i = 0; i < TaskDataObj.m_straylist.Count; i++)
            {
                if (bSelect)
                    dataGridViewTask.Rows[i].Cells[0].Value = true;
                else
                    dataGridViewTask.Rows[i].Cells[0].Value = false;
            }
        }




        private void selectAllQtestData(bool bSelect = true)
        {
            if (TaskDataObj.m_qtestlist.Count < 1)
            {
                MessageBox.Show("质检无数据", "警告");
                return;
            }

            for (int i = 0; i < TaskDataObj.m_qtestlist.Count; i++)
            {
                if (bSelect)
                    dataGridViewTask.Rows[i].Cells[0].Value = true;
                else
                    dataGridViewTask.Rows[i].Cells[0].Value = false;
            }
        }

        bool gridRowIsChecked(int rowIndex)
        {
            return (bool)dataGridViewTask.Rows[rowIndex].Cells[0].EditedFormattedValue; //当前行是否被选中
        }

         private bool sendTaskData2CNC( string equipCode, string filePath, int index,bool isauto)
        {
            if (checkBox_XuNiSendTask.Checked)//虚拟派工
            {
                for(int jj = 0;jj < MainForm.cnclist.Count;jj++)
                {
                    if (equipCode == MainForm.cnclist[jj].BujianID)
                    {
                        for (int ii = 0; ii < dataGridView_CNCGCodeDb.Rows.Count; ii++)
                        {
                            if (dataGridView_CNCGCodeDb.Rows[ii][0].ToString() == equipCode)
                            {
                                dataGridView_CNCGCodeDb.Rows[ii][2] = DateTime.Now.ToString() + ":" + "NC代码虚拟发送成功!";
                            }
                        }
//                         TaskDataObj.m_workorderlist[index].PLAN_SENDED = 2;
//                         MainForm.cnclist[jj].mesDispatch = TaskDataObj.m_workorderlist[index];
                        MainForm.cnclist[jj].NcTaskManage.SendCNCTaskOKOrNot(true);
                        return true;
                    }
                }
                return false;
            }


            int sendGcodeOK =  -1;
            if ((equipCode == ""|| filePath == "") && !isauto )
            {
                string alamMsg = "派工单发送失败,机床编号为空或NC代码路径为空"; //是否在CNC链表里
                MessageBox.Show(alamMsg, "警告");
                return false;
            }
            filePath = filePath.Replace('/', '\\');
            if (!System.IO.File.Exists(filePath) && !isauto)
             {
                 string alamMsg = "文件:" + filePath + "不存在";
                 MessageBox.Show(alamMsg, "警告");
                 return false;
             }
            string[] Pathstr = filePath.Split('\\');
            Pathstr[0] = filePath.Replace(Pathstr[Pathstr.Length - 1], "");
            Pathstr[0] = Pathstr[0].Substring(0, Pathstr[0].Length - 1);

            DirectoryInfo progFolder = new DirectoryInfo(Pathstr[0]);
             string str4 = "";
            for (int j = 0; j < MainForm.cnclist.Count; j++)
            {
                if (equipCode == MainForm.cnclist[j].BujianID)
                {
                    string str1 = "", str2 = "", str3 = "";
                    foreach (FileInfo NextFile in progFolder.GetFiles())
                    {
                        //string strFileName = NextFile.FullName;
                        //sendGcodeOK = MainForm.cnclist[j].sendGCode(NextFile.FullName);
                       // MainForm.cnclist[j].
                       // MainForm.cnclist[j].netFileRemove("./prog/" + NextFile.Name); //删除下位机PROG下所有文件
                        if (!MainForm.cnclist[j].isConnected())
                        {
                            str2 += NextFile.Name + "网络故障";
                            continue;
                        }
                        if (NextFile.Length / 1024 / 1024 > maxFileSize)
                        {
                            str2 += NextFile.Name + "文件大小超过" + maxFileSize + "M" + "；";
                            continue;
                        }
                        CNC.CNCState sta = 0;
                        MainForm.cnclist[j].Checkcnc_state(ref sta);
                        if (sta != CNC.CNCState.RUNING)
                        {
                            int sendNum = 0;
                            while (sendNum < resendNum)
                            {
                                sendNum++;
                                string sendfilename = NextFile.FullName.Replace('\\', '/');
                                string sendfilenamesr = filePath + "/" + filePath.Split('/')[filePath.Split('/').Length - 1].Split('.')[0] + ".nc";
                                if (sendfilenamesr == sendfilename)
                                    sendGcodeOK = MainForm.cnclist[j].sendFile(sendfilename, "../prog/" + NextFile.Name, 0, true);
                                else
                                    sendGcodeOK = MainForm.cnclist[j].sendFile(sendfilename, "../prog/" + NextFile.Name, 0, false);
                                sendGcodeOK = 0;
                                if (sendGcodeOK == 0)
                                {
                                    str1 += NextFile.Name + "；";
                                    break;
                                }
                                else
                                {
                                    if (sendNum == resendNum)
                                    {
                                        str2 += NextFile.Name + "；";
                                    }
                                }
                            }
                        }
                        else
                        {
                            sendGcodeOK = -1;
                            str2 = "机床正在运行！";
                        }

                        if (sendGcodeOK != 0)
                        {
                            str3 = "机床网络没连接或工作状态不为空闲!";
                        }
                    }

                    if (sendGcodeOK != 0)
                    {
                        str3 = "NC代码发送失败!";
                        MainForm.cnclist[j].NcTaskManage.SendCNCTaskOKOrNot(false);
                    }
                    else
                    {
                        str3 = "NC代码发送成功!";
//                         TaskDataObj.m_workorderlist[index].PLAN_SENDED = 2;
//                         MainForm.cnclist[j].mesDispatch = TaskDataObj.m_workorderlist[index];
                        MainForm.cnclist[j].NcTaskManage.SendCNCTaskOKOrNot(true);
                    }
                    if (str1.Length > 0)
                    {
                        str1 = "下载成功：" + str1;
                    }
                    if (str2.Length > 0)
                    {
                        str2 = "下载失败：" + str2;
                    }

                    for (int ii = 0; ii < dataGridView_CNCGCodeDb.Rows.Count;ii++ )
                    {
                        if (dataGridView_CNCGCodeDb.Rows[ii][0].ToString() == equipCode)
                        {
                            dataGridView_CNCGCodeDb.Rows[ii][1] = str1 + "\r\n" + str2;
                            dataGridView_CNCGCodeDb.Rows[ii][2] = DateTime.Now.ToString() + ":" + str3;
                            if(str2.Length > 0)
                            {
                                str4 += dataGridView_CNCGCodeDb.Rows[ii][0] + ":" + str2;;
                            }
                        }
                    }
                }
            }
            dataGridView_CNCGCode.DataSource = null;
            dataGridView_CNCGCode.DataSource = dataGridView_CNCGCodeDb;
            if (str4.Length > 0)
            {
                MessageBox.Show(str4, MessageboxStr_登录[1], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }

         public  void readTaskDataFromXML(string taskFileName)
         {
             XmlDocument xmlDoc = new XmlDocument();
             if (taskFileName == "") //如果为空，选择默认文件，读取XML文件中的数据并显示
             {
                 if (!System.IO.Directory.Exists(taskListPathXml))//2014.10.25
                     System.IO.Directory.CreateDirectory(taskListPathXml);
                 taskFileName = taskListPathXml + cncTaskListXml + ".XML"; //2015.10.25
                 if (!System.IO.File.Exists(taskFileName))
                     return;
             }
             TaskDb = SCADA.RFIDDATAT.DBReadFromXml(taskFileName);
//              dataGridViewTask.DataSource = null;
//              dataGridViewTask.DataSource = TaskDb;
         }


         private void btnSend_Click(object sender, EventArgs e)
         {
             if (SetForm.LogIn)
             {
                 getCNCTaskData();
//                  if (UserControlTaskData.NCTaskMessageChangeHand != null)
//                  {
//                      UserControlTaskData.NCTaskMessageChangeHand.Invoke(null, null);
//                  }
             }
             else
             {
                 MessageBox.Show(MessageboxStr_登录[0], MessageboxStr_登录[1], MessageBoxButtons.OK, MessageBoxIcon.Warning);
             }
         }

         private void btnSelectAll_Click(object sender, EventArgs e)
         {
            selectAllCNCTaskData(true);
         }

        //限制除第1列以外的列编辑
         private void dataGridViewTask_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
         {
             if (e.ColumnIndex == 0)
             {
                 if(!sendlist.Contains(e.RowIndex))
                 {
                    e.Cancel = true;// 取消编辑
                 }
             }
             else
             {                 
                 e.Cancel = true;// 取消编辑
             }
         }

        //历史任务，如果没有选择，则加载默认保存的最后的历史文件
         private void btnHisTask_Click(object sender, EventArgs e)
         {
             OpenFileDialog taskFile = new OpenFileDialog();
             taskFile.InitialDirectory = Application.StartupPath + taskListPathXml;
             taskFile.Filter = "XML files (*.xml)|*.xml";
             taskFile.RestoreDirectory = true;
             if (taskFile.ShowDialog() == DialogResult.OK && taskFile.FileName != null)
             {
                 TaskDb = SCADA.RFIDDATAT.DBReadFromXml(taskFile.FileName);
                 DB2workorderlist(ref TaskDb, TaskDataObj.m_workorderlist);
                 dataGridViewTask.DataSource = null;
                 dataGridViewTask.DataSource = TaskDb;
             }
         }

        //当前任务
         private void btnCurTask_Click(object sender, EventArgs e)
         {
             if (System.IO.File.Exists(taskListPathXml + cncTaskListXml))
             {
                 TaskDb = SCADA.RFIDDATAT.DBReadFromXml(taskListPathXml + cncTaskListXml);
                 DB2workorderlist(ref TaskDb, TaskDataObj.m_workorderlist);
//                  dataGridViewTask.DataSource = null;
//                  dataGridViewTask.DataSource = TaskDb;
             }
         }


         private void btnSelectNone_Click(object sender, EventArgs e)
         {
             selectAllCNCTaskData(false);
         }


         private void button_DeletGCode_Click(object sender, EventArgs e)
         {
             System.Data.DataTable Db = new DataTable();
             for (int ii = 0; ii < STR_dataGridView_GCodeSele_Columns.Length; ii++)
             {
                 Db.Columns.Add(STR_dataGridView_GCodeSele_Columns[ii]);
             }
             for (int ii = 0; ii < dataGridView_GCodeSeleDb.Rows.Count; ii++)
             {
                 if (!(bool)dataGridView_GCodeSele.Rows[ii].Cells[0].EditedFormattedValue)
                 {
                     string[] array = new string[STR_dataGridView_GCodeSele_Columns.Length];
                     array[0] = dataGridView_GCodeSeleDb.Rows[ii][0].ToString();
                     array[1] = dataGridView_GCodeSeleDb.Rows[ii][1].ToString();
                     Db.Rows.Add(array);
                 }
             }
             dataGridView_GCodeSeleDb = Db;
             dataGridView_GCodeSele.DataSource = null;
             dataGridView_GCodeSele.DataSource = dataGridView_GCodeSeleDb;
         }
         private void button_AddGCode_Click(object sender, EventArgs e)
         {
             try
             {
                 OpenFileDialog fileDialog = new OpenFileDialog();
                 fileDialog.RestoreDirectory = true; //记忆上次浏览路径
                 fileDialog.Multiselect = true;
                 fileDialog.Title = "请选择文件";
                 fileDialog.Filter = "nc文件(*.nc)|*.nc";
                 if (fileDialog.ShowDialog() == DialogResult.OK)
                 {
                     dataGridView_GCodeSele.DataSource = null;
                     foreach (string filaname in fileDialog.FileNames)
                     {
                         string[] array = new string[STR_dataGridView_GCodeSele_Columns.Length];
                         array[0] = filaname;
                         array[1] = "";
                         dataGridView_GCodeSeleDb.Rows.Add(array);
                     }
                     dataGridView_GCodeSele.DataSource = null;
                     dataGridView_GCodeSele.DataSource = dataGridView_GCodeSeleDb;
                 }
             }
             catch
             {

             }
         }

         private void button_jitaiquanxuan_Click(object sender, EventArgs e)
         {
             for (int ii = 0; ii < dataGridView_CNCGCode.Rows.Count; ii++)
             {
                 dataGridView_CNCGCode.Rows[ii].Cells[0].Value = true;
             }
         }
         private void button_jitaiquanbuxuan_Click(object sender, EventArgs e)
         {
             for (int ii = 0; ii < dataGridView_CNCGCode.Rows.Count; ii++)
             {
                 dataGridView_CNCGCode.Rows[ii].Cells[0].Value = false;
             }
         }
         private void button_GCodequanbuxuan_Click(object sender, EventArgs e)
         {
             for (int ii = 0; ii < dataGridView_GCodeSele.Rows.Count; ii++)
             {
                 dataGridView_GCodeSele.Rows[ii].Cells[0].Value = false;
             }
         }
         private void button_GCodequanxuan_Click(object sender, EventArgs e)
         {
             for (int ii = 0; ii < dataGridView_GCodeSele.Rows.Count; ii++)
             {
                 dataGridView_GCodeSele.Rows[ii].Cells[0].Value = true;
             }
         }
         private void button_DowLoadGCode_Click(object sender, EventArgs e)
         {
             if (SetForm.LogIn)
             {
                 string str3 = "";
                 for (int ii = 0; ii < dataGridView_CNCGCode.Rows.Count;ii++ )
                 {
                     if ((bool)dataGridView_CNCGCode.Rows[ii].Cells[0].EditedFormattedValue)
                     {
                         string str1 = "",str2 = "";
                         for (int jj = 0; jj < dataGridView_GCodeSele.Rows.Count; jj++)
                         {
                             if ((bool)dataGridView_GCodeSele.Rows[jj].Cells[0].EditedFormattedValue)
                             {
                                 string[] filename = dataGridView_GCodeSeleDb.Rows[jj][0].ToString().Split('\\');
                                 if (MainForm.cnclist[ii].sendFile(dataGridView_GCodeSeleDb.Rows[jj][0].ToString(), "../prog/" + filename[filename.Length - 1], 0, false) == 0)
                                 {
                                     str1 += filename[filename.Length - 1] + "；";
                                 }
                                 else
                                 {
                                     str2 += filename[filename.Length - 1] + "；";
                                 }
                             }
                         }
                         if (str1.Length > 0)
                         {
                             str1 = "下载成功：" + str1;
                         }
                         if (str2.Length > 0)
                         {
                             str2 = "下载失败：" + str2;
                         }
                         dataGridView_CNCGCodeDb.Rows[ii][1] = str1 + "\r\n" + str2;
                         if(str2.Length > 0)
                         {
                             dataGridView_CNCGCodeDb.Rows[ii][2] = DateTime.Now.ToString() + ":" + "下载失败！";
                             str3 += dataGridView_CNCGCodeDb.Rows[ii][0] + ":" + str2;
                         }
                         else
                         {
                             dataGridView_CNCGCodeDb.Rows[ii][2] = DateTime.Now.ToString() + ":" + "下载成功！";
                         }
                     }
                 }
                 if (str3.Length > 0)
                 {
                     MessageBox.Show(str3, MessageboxStr_登录[1], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 }
             }
             else
             {
                 MessageBox.Show(MessageboxStr_登录[0], MessageboxStr_登录[1], MessageBoxButtons.OK, MessageBoxIcon.Warning);
             }
         }

         private void checkBox_AoutoSendTask_Click(object sender, EventArgs e)
         {
             if (!SetForm.LogIn)
             {
                 checkBox_AoutoSendTask.Checked = !checkBox_AoutoSendTask.Checked;
                 MessageBox.Show(MessageboxStr_登录[0], MessageboxStr_登录[1], MessageBoxButtons.OK, MessageBoxIcon.Warning);
             }
         }

         private void checkBox_XuNiSendTask_Click(object sender, EventArgs e)
         {
             if (!SetForm.LogIn)
             {
                 checkBox_XuNiSendTask.Checked = !checkBox_XuNiSendTask.Checked;
                 MessageBox.Show(MessageboxStr_登录[0], MessageboxStr_登录[1], MessageBoxButtons.OK, MessageBoxIcon.Warning);
             }
         }

         List<int> sendlist = new List<int>();
         private void mComboBox_CNCTaskList_SelectedIndexChanged(object sender, EventArgs e)
         {
             System.Data.DataTable TaskDbtemp = new DataTable();
             TaskDb = null;
             sendlist.Clear();
             if (mComboBox_CNCTaskList.SelectedIndex == 0)
             {
                 for (int ii = 0; ii < MainForm.cnclist.Count;ii++ )
                 {
                     TaskDbtemp = MainForm.cnclist[ii].NcTaskManage.GetTaskDb();
                     if (TaskDbtemp != null && TaskDb == null)
                     {
                         sendlist.Add(0);
                         TaskDb = TaskDbtemp.Copy();
                     }
                     else
                     {
                         for (int jj = 0; jj < TaskDbtemp.Rows.Count; jj++)
                         {
                             TaskDb.Rows.Add(TaskDb.NewRow());
                             if(jj == 0)
                             {
                                 sendlist.Add(TaskDb.Rows.Count - 1);
                             }
                             for (int tt = 0; tt < TaskDbtemp.Columns.Count; tt++)
                             {
                                 TaskDb.Rows[TaskDb.Rows.Count - 1][tt] = TaskDbtemp.Rows[jj][tt];
                             }
                         }
                     }
                 }
             }
             else
             {
                 TaskDb = MainForm.cnclist[mComboBox_CNCTaskList.SelectedIndex - 1].NcTaskManage.GetTaskDb();
                 sendlist.Add(0);
             }
             dataGridViewTask_UpData = true; 
             mComboBox_CNCTaskList_STR = mComboBox_CNCTaskList.Text;
         }
         String mComboBox_CNCTaskList_STR;
        private void NCTaskMessageChangeHandFc(object sender, EventArgs e)
        {
            if (sender == null)
            {
                mComboBox_CNCTaskList_SelectedIndexChanged(null, null);
            }
            else
            {
                NCTaskDel Task = (NCTaskDel)sender;
                if (Task != null && (mComboBox_CNCTaskList_STR == "全部CNC任务" || mComboBox_CNCTaskList_STR == Task.CNCBuJianID + "任务"))
                {
                    System.Data.DataTable TaskDbtemp;
                    TaskDb = null;
                    sendlist.Clear();
                    if (mComboBox_CNCTaskList_STR == "全部CNC任务")
                    {
                        for (int ii = 0; ii < MainForm.cnclist.Count; ii++)
                        {
                            TaskDbtemp = MainForm.cnclist[ii].NcTaskManage.GetTaskDb();
                            if (TaskDbtemp != null && TaskDb == null)
                            {
                                sendlist.Add(0);
                                TaskDb = TaskDbtemp.Copy();
                            }
                            else
                            {
                                for (int jj = 0; jj < TaskDbtemp.Rows.Count; jj++)
                                {
                                    TaskDb.Rows.Add(TaskDb.NewRow());
                                    if (jj == 0)
                                    {
                                        sendlist.Add(TaskDb.Rows.Count - 1);
                                    }
                                    for (int tt = 0; tt < TaskDbtemp.Columns.Count; tt++)
                                    {
                                        TaskDb.Rows[TaskDb.Rows.Count - 1][tt] = TaskDbtemp.Rows[jj][tt];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        TaskDb = Task.GetTaskDb();
                        sendlist.Add(0);
                    }
                    dataGridViewTask_UpData = true;
                }
            }
        }

        bool dataGridViewTask_UpData = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dataGridViewTask_UpData && dataGridViewTask.Visible&& TaskDb!=null)
            {
                dataGridViewTask.DataSource = null;
                dataGridViewTask.DataSource = TaskDb.Copy();
                dataGridViewTask_UpData = false;
            }
        }

    } 
}


