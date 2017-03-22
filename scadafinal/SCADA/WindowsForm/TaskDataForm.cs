using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SCADA
{
    public partial class TaskDataForm : Form
    {
        public static UserControlTaskData ctrlTaskData;
        //1.声明自适应类
//         AutoSizeFormClass asc = new AutoSizeFormClass();

        public TaskDataForm()
        {
            InitializeComponent();
        }

        public static void AddTaskData2Form(int taskii)
        {
            ctrlTaskData.AddTaskFormMes(taskii);
        }

        private void TaskDataForm_Load(object sender, EventArgs e)
        {
            panelTaskData.Controls.Clear();
            panelTaskData.Controls.Add(ctrlTaskData);
            ctrlTaskData.Dock = DockStyle.Fill;
//             asc.controllInitializeSize(this);
        }

        private void TaskDataForm_SizeChanged(object sender, EventArgs e)
        {
            //记录控件的初始位置和大小后,再最大化
//             asc.controlAutoSize(this);
            this.WindowState = (System.Windows.Forms.FormWindowState)(2);
        }
    }
}
