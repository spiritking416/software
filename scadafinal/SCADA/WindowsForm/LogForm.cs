using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SCADA
{
    public partial class LogForm : Form
    {
//         AutoSizeFormClass asc = new AutoSizeFormClass();
        public static IntPtr LogPtr;
        public LogForm()
        {
            InitializeComponent();
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
//             asc.controllInitializeSize(this);

//             if (Localization.HasLang)
//             {
//                 Localization.RefreshLanguage(this);
//                 changeDGVLog();
//             }
            LogPtr = Handle;

            dataGridView_ShowLogData.AllowUserToAddRows = false;
            dataGridView_ShowLogData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dataGridView_ShowLogData.ReadOnly = true;
//             dataGridView_ShowLogData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            button_ReFshLog_Click(null, null);
        }

        private void LogForm_SizeChanged(object sender, EventArgs e)
        {
//             asc.controlAutoSize(this);
//             this.WindowState = (System.Windows.Forms.FormWindowState)(2);
        }
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MainForm.LanguageChangeMsg:
//                     Localization.RefreshLanguage(this);
// 					changeDGVLog();
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }
        private void changeDGVLog()
        {
//             dgvLog.Columns[0].HeaderText = Localization.Forms["LogForm"]["colNum"];
//             dgvLog.Columns[1].HeaderText = Localization.Forms["LogForm"]["colAlarmNum"];
//             dgvLog.Columns[2].HeaderText = Localization.Forms["LogForm"]["colAlarmContent"];
//             dgvLog.Columns[3].HeaderText = Localization.Forms["LogForm"]["colAlarmTime"];

//             dgvLog.Columns[4].HeaderText = Localization.Forms["LogForm"]["colClearTime"];
//             dgvLog.Columns[5].HeaderText = Localization.Forms["LogForm"]["colEquipment"];
        }

        private System.Data.DataTable ShowDataTable;
        String treeView_LogTree_Pathstr = LogData.LogDataNode0Name[1];
        private void treeView_LogTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((e.Node != null && e.Node.FirstNode != null) || e.Node.Text == "网络日志")
            {
                treeView_LogTree_Pathstr = LogData.LogDataNode0Name[e.Node.Index + 1];
            }
            else if (e.Node != null && e.Node.FirstNode == null)
            {
                treeView_LogTree_Pathstr = LogData.LogDataNode0Name[e.Node.Parent.Index + 1];
                if (e.Node.Parent.Index == 0)
                {
                    treeView_LogTree_Pathstr += "\\" + LogData.LogDataNode1Name[e.Node.Index];
                }
                else if (e.Node.Parent.Index == 1)
                {
                    treeView_LogTree_Pathstr += "\\" + LogData.LogDataNode1Name[e.Node.Index + 2];
                }
            }
            button_ReFshLog_Click(null, null);
        }

        static string[] PictrueFile = { "..\\picture\\Cross.ico",
        "..\\picture\\Key.ico","..\\picture\\Warning.ico","..\\picture\\yanzhong.ico", "..\\picture\\Mesg.ico"};//状态图片路径
        private void dataGridView_ShowLogData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >=0 && e.ColumnIndex == 0)
            {
                if (this.dataGridView_ShowLogData.Rows[e.RowIndex].Cells[LogData.DataDataTableShowColumnStr[(int)LogData.Node2Attributes.Level]].Value == DBNull.Value)
                    return;

                String vulestr = this.dataGridView_ShowLogData.Rows[e.RowIndex].Cells[LogData.DataDataTableShowColumnStr[(int)LogData.Node2Attributes.Level]].Value.ToString();
                Bitmap m_bitmap ;//= new Bitmap(System.Drawing.Image.FromFile(PictrueFile[0]));
                if (vulestr == LogData.LogDataNode2Level[(int)LogData.Node2Level.MESSAGE])
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[4]));
                }
                else if (vulestr == LogData.LogDataNode2Level[(int)LogData.Node2Level.WARNING])
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[2]));
                }
                else if (vulestr == LogData.LogDataNode2Level[(int)LogData.Node2Level.AUDIT])
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[1]));
                }
                else // (vulestr == LogData.LogDataNode2Level[(int)LogData.Node2Level.严重])
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[3]));
                }
                Rectangle newRect = new Rectangle(e.CellBounds.X + 3, e.CellBounds.Y + 3, e.CellBounds.Height - 5, 
                    e.CellBounds.Height - 5);

                using (Brush gridBrush = new SolidBrush(this.dataGridView_ShowLogData.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush, 2))
                    {
                        // Erase the cell.
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                        //划线
                        Point p1 = new Point(e.CellBounds.Left + e.CellBounds.Width, e.CellBounds.Top);
                        Point p2 = new Point(e.CellBounds.Left + e.CellBounds.Width, e.CellBounds.Top + e.CellBounds.Height);
                        Point p3 = new Point(e.CellBounds.Left, e.CellBounds.Top + e.CellBounds.Height);
                        Point[] ps = new Point[] { p1, p2, p3 };
                        e.Graphics.DrawLines(gridLinePen, ps);

                        //画图标
                        e.Graphics.DrawImage(m_bitmap, newRect);
                        //画字符串
                        e.Graphics.DrawString(vulestr, e.CellStyle.Font, Brushes.Crimson, 
                            e.CellBounds.Left + 20, e.CellBounds.Top + 5, StringFormat.GenericDefault);
                        e.Handled = true;
                    }
                }
            }
        }

        private void button_CurentLog_Click(object sender, EventArgs e)
        {
            LogOldNew = false;
            button_ReFshLog_Click(null, null);
        }

        private LogData m_OldLog = new LogData();
        bool LogOldNew = false;
        private void button_OpenOldLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
                openFileDialog1.Filter = "(*.xml)|*.xml";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    m_OldLog.m_load(openFileDialog1.FileName, false);
                    LogOldNew = true;
                    button_ReFshLog_Click(null, null);
                }
            }
            catch
            {

            }
        }


        private String FindStr;
        SCADA.WindowsForm.Form_LogFind m_FindForm = new WindowsForm.Form_LogFind();
        private void button_LogFind_Click(object sender, EventArgs e)
        {
            if (m_FindForm.FindEventHandler == null)
            {
                m_FindForm.FindEventHandler = new EventHandler<String>(FindEventHandlerFuc);
                m_FindForm.SettextBox_FindStr(FindStr);
                m_FindForm.Show();
            }
            else
            {
                m_FindForm.Visible = true;
            }
        }

        //查找
        int FindRowsIndex = -1;
        private void FindEventHandlerFuc(object ob,String FindStr)
        {
            String BtText = ((Button)ob).Text;
            if (this.FindStr != FindStr)
            {
                FindRowsIndex = -1;
            }
            this.FindStr = FindStr;
            if (FindStr.Length != 0)
            {
                if (BtText == "查找下一个")
                {
                    for (int ii = FindRowsIndex + 1; ii < dataGridView_ShowLogData.RowCount; ii++)
                    {
                        foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                        {
                            if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                            {
                                dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                //                         dataGridView_ShowLogData.Rows[ii].DefaultCellStyle.ForeColor = Color.Red;//红色
                                FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                return;
                            }
                        }
                    }
                    for (int ii = 0; ii < FindRowsIndex; ii++)
                    {
                        foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                        {
                            if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                            {
                                dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                return;
                            }
                        }
                    }
                }
                else if (BtText == "查找上一个")
                {
                    if (FindRowsIndex == -1)
                    {
                        for (int ii = dataGridView_ShowLogData.RowCount - 1; ii >= 0; ii--)
                        {
                            foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                            {
                                if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                                {
                                    dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                    dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                    FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int ii = FindRowsIndex - 1; ii >= 0; ii--)
                        {
                            foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                            {
                                if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                                {
                                    dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                    dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                    FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                    return;
                                }
                            }
                        }
                        for (int ii = dataGridView_ShowLogData.RowCount - 1; ii > FindRowsIndex; ii--)
                        {
                            foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                            {
                                if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                                {
                                    dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                    dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                    FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button_ReFshLog_Click(object sender, EventArgs e)
        {
            if (LogOldNew)
            {
                ShowDataTable = m_OldLog.ReadFromXml(treeView_LogTree_Pathstr);
            }
            else
            {
                ShowDataTable = MainForm.m_Log.ReadFromXml(treeView_LogTree_Pathstr);
            }
            dataGridView_ShowLogData.DataSource = null;
            dataGridView_ShowLogData.DataSource = ShowDataTable;
        }

    }
}

