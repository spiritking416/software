using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA
{
    public partial class EquipmentCheckForm : Form
    {
        public EquipmentCheckForm()
        {
            InitializeComponent();
            dataGridView_Checkcontent.AllowUserToAddRows = false;
            dataGridView_Checkcontent.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dataGridView_Checkcontent.ReadOnly = true;
            dataGridView_Checkcontent.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            timer1.Enabled = false;
        }

        System.Data.DataTable CheckcontentDataSource;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MainForm.m_CheckHander.CheckdataGridView_DB_ChangeFlg)
            {
                UpDatadataGridView();
            }
            if (!checkBox_DongTai.Checked)
            {
                timer1.Enabled = false;
            }
        }

        private void dataGridView_Checkcontent_VisibleChanged(object sender, EventArgs e)
        {
            UpDatadataGridView();
        }

        private void checkBox_DongTai_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DongTai.Checked)
            {
                button_UpDataTable.Visible = false;
                timer1.Enabled = true;
            }
            else
            {
                button_UpDataTable.Visible = true;
                timer1.Enabled = false;
            }
        }

        private void button_UpDataTable_Click(object sender, EventArgs e)
        {
            UpDatadataGridView();
        }

        System.Data.DataTable CheckcontentDataSource1;
        private void UpDatadataGridView()
        {
            if (dataGridView_Checkcontent.Visible && MainForm.m_CheckHander != null)
            {
                CheckcontentDataSource = MainForm.m_CheckHander.GetCheckdataGridView_DB();
                if (checkBox_ZhengChang.Checked && checkBox_YiChang.Checked)
                {
                    dataGridView_Checkcontent.DataSource = null;
                    dataGridView_Checkcontent.DataSource = CheckcontentDataSource;
                }
                else if (checkBox_ZhengChang.Checked)
                {
                    CheckcontentDataSource1 = CheckcontentDataSource.Copy();
                    for (int ii = 0; ii < CheckcontentDataSource1.Rows.Count; ii++)
                    {
                        if ((CheckcontentDataSource1.Rows[ii][(int)EquipmentCheck.CheckdataGridView_titleArr_Index.Content].ToString().Length == 0
                                || CheckcontentDataSource1.Rows[ii][(int)EquipmentCheck.CheckdataGridView_titleArr_Index.Content].ToString() == "正常")
                            && CheckcontentDataSource1.Rows[ii][(int)EquipmentCheck.CheckdataGridView_titleArr_Index.Status].ToString() != "离线")
                        {
                        }
                        else
                        {
                            CheckcontentDataSource1.Rows[ii].Delete();
                            ii--;
                        }
                    }
                    dataGridView_Checkcontent.DataSource = null;
                    dataGridView_Checkcontent.DataSource = CheckcontentDataSource1;
                }
                else if (checkBox_YiChang.Checked)
                {
                    CheckcontentDataSource1 = CheckcontentDataSource.Copy();
                    for (int ii = 0; ii < CheckcontentDataSource1.Rows.Count; ii++)
                    {
                        if ((CheckcontentDataSource1.Rows[ii][(int)EquipmentCheck.CheckdataGridView_titleArr_Index.Status].ToString().Length == 0
                                || CheckcontentDataSource1.Rows[ii][(int)EquipmentCheck.CheckdataGridView_titleArr_Index.Status].ToString() == "正常")
                            && CheckcontentDataSource1.Rows[ii][(int)EquipmentCheck.CheckdataGridView_titleArr_Index.Status].ToString() != "离线")
                        {
                            CheckcontentDataSource1.Rows[ii].Delete();
                            ii--;
                        }
                        else
                        {
                        }
                    }
                    dataGridView_Checkcontent.DataSource = null;
                    dataGridView_Checkcontent.DataSource = CheckcontentDataSource1;
                }
                else
                {
                    dataGridView_Checkcontent.DataSource = null;
                }
            }
        }

        private void checkBox_ZhengChang_CheckedChanged(object sender, EventArgs e)
        {
            UpDatadataGridView();
        }

        private void checkBox_YiChang_CheckedChanged(object sender, EventArgs e)
        {
            UpDatadataGridView();
        }

        private void dataGridView_Checkcontent_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 )
            {
               if (e.ColumnIndex == (int)EquipmentCheck.CheckdataGridView_titleArr_Index.Content)
                {
                    if ((this.dataGridView_Checkcontent.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == DBNull.Value
                        || this.dataGridView_Checkcontent.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length == 0
                        || this.dataGridView_Checkcontent.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "正常")
                        && this.dataGridView_Checkcontent.Rows[e.RowIndex].Cells[(int)EquipmentCheck.CheckdataGridView_titleArr_Index.Status].Value.ToString() != "离线")
                    {
                        if (this.dataGridView_Checkcontent.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.Green)
                        {
                            this.dataGridView_Checkcontent.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                        }
                    }
                    else
                    {
                        if (this.dataGridView_Checkcontent.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.Red)
                        {
                            this.dataGridView_Checkcontent.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                        }
                    }
                }
            } 
        }

        private void button_StarLine_Click(object sender, EventArgs e)//开线
        {

        }

        private void button_ClreaLine_Click(object sender, EventArgs e)//清线
        {
            if (SetForm.LogIn)
            {
                if (MessageBox.Show("是否确定执行清线操作！", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    for (int ii = 0; ii < MainForm.cnclist.Count; ii++)
                    {
                        MainForm.cnclist[ii].NcTaskManage.ClearNCTask();
                    }
                    LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                    SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                    SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                    SendParm.Keywords = "清线操作";
                    SendParm.EventData = "用户:" + SetForm.LogInUserName;
                    SCADA.MainForm.m_Log.AddLogMsgHandler.Invoke(this, SendParm);
                }
            }
            else
            {
                MessageBox.Show(MessageString.SetForms_RightNotEnough, MessageString.SetForms_Information, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
