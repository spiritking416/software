namespace SCADA
{
    partial class UserControlTaskData
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_XuNiSendTask = new System.Windows.Forms.CheckBox();
            this.checkBox_AoutoSendTask = new System.Windows.Forms.CheckBox();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnCurTask = new System.Windows.Forms.Button();
            this.btnHisTask = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_GCodequanxuan = new System.Windows.Forms.Button();
            this.button_jitaiquanxuan = new System.Windows.Forms.Button();
            this.button_GCodequanbuxuan = new System.Windows.Forms.Button();
            this.button_DeletGCode = new System.Windows.Forms.Button();
            this.button_jitaiquanbuxuan = new System.Windows.Forms.Button();
            this.button_AddGCode = new System.Windows.Forms.Button();
            this.button_DowLoadGCode = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewTask = new System.Windows.Forms.DataGridView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.dataGridView_CNCGCode = new System.Windows.Forms.DataGridView();
            this.dataGridView_GCodeSele = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mComboBox_CNCTaskList = new SCADA.mComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTask)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CNCGCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GCodeSele)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(803, 475);
            this.splitContainer1.SplitterDistance = 168;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer4.Size = new System.Drawing.Size(168, 475);
            this.splitContainer4.SplitterDistance = 238;
            this.splitContainer4.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mComboBox_CNCTaskList);
            this.groupBox1.Controls.Add(this.checkBox_XuNiSendTask);
            this.groupBox1.Controls.Add(this.checkBox_AoutoSendTask);
            this.groupBox1.Controls.Add(this.btnSelectNone);
            this.groupBox1.Controls.Add(this.btnSend);
            this.groupBox1.Controls.Add(this.btnSelectAll);
            this.groupBox1.Controls.Add(this.btnCurTask);
            this.groupBox1.Controls.Add(this.btnHisTask);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(152, 188);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "派工操作";
            // 
            // checkBox_XuNiSendTask
            // 
            this.checkBox_XuNiSendTask.AutoSize = true;
            this.checkBox_XuNiSendTask.Location = new System.Drawing.Point(6, 150);
            this.checkBox_XuNiSendTask.Name = "checkBox_XuNiSendTask";
            this.checkBox_XuNiSendTask.Size = new System.Drawing.Size(72, 16);
            this.checkBox_XuNiSendTask.TabIndex = 9;
            this.checkBox_XuNiSendTask.Text = "虚拟派发";
            this.checkBox_XuNiSendTask.UseVisualStyleBackColor = true;
            this.checkBox_XuNiSendTask.Click += new System.EventHandler(this.checkBox_XuNiSendTask_Click);
            // 
            // checkBox_AoutoSendTask
            // 
            this.checkBox_AoutoSendTask.AutoSize = true;
            this.checkBox_AoutoSendTask.Location = new System.Drawing.Point(6, 166);
            this.checkBox_AoutoSendTask.Name = "checkBox_AoutoSendTask";
            this.checkBox_AoutoSendTask.Size = new System.Drawing.Size(72, 16);
            this.checkBox_AoutoSendTask.TabIndex = 9;
            this.checkBox_AoutoSendTask.Text = "自动派发";
            this.checkBox_AoutoSendTask.UseVisualStyleBackColor = true;
            this.checkBox_AoutoSendTask.Visible = false;
            this.checkBox_AoutoSendTask.Click += new System.EventHandler(this.checkBox_AoutoSendTask_Click);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Location = new System.Drawing.Point(77, 82);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(63, 23);
            this.btnSelectNone.TabIndex = 7;
            this.btnSelectNone.Text = "全不选";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(76, 146);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(69, 32);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "手动派发";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(8, 82);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(63, 23);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "全选";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnCurTask
            // 
            this.btnCurTask.Location = new System.Drawing.Point(77, 112);
            this.btnCurTask.Name = "btnCurTask";
            this.btnCurTask.Size = new System.Drawing.Size(62, 23);
            this.btnCurTask.TabIndex = 4;
            this.btnCurTask.Text = "当前任务";
            this.btnCurTask.UseVisualStyleBackColor = true;
            this.btnCurTask.Visible = false;
            this.btnCurTask.Click += new System.EventHandler(this.btnCurTask_Click);
            // 
            // btnHisTask
            // 
            this.btnHisTask.Location = new System.Drawing.Point(8, 112);
            this.btnHisTask.Name = "btnHisTask";
            this.btnHisTask.Size = new System.Drawing.Size(63, 23);
            this.btnHisTask.TabIndex = 3;
            this.btnHisTask.Text = "历史任务";
            this.btnHisTask.UseVisualStyleBackColor = true;
            this.btnHisTask.Visible = false;
            this.btnHisTask.Click += new System.EventHandler(this.btnHisTask_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_GCodequanxuan);
            this.groupBox2.Controls.Add(this.button_jitaiquanxuan);
            this.groupBox2.Controls.Add(this.button_GCodequanbuxuan);
            this.groupBox2.Controls.Add(this.button_DeletGCode);
            this.groupBox2.Controls.Add(this.button_jitaiquanbuxuan);
            this.groupBox2.Controls.Add(this.button_AddGCode);
            this.groupBox2.Controls.Add(this.button_DowLoadGCode);
            this.groupBox2.Location = new System.Drawing.Point(6, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(155, 184);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "手动下载g代码操作";
            // 
            // button_GCodequanxuan
            // 
            this.button_GCodequanxuan.Location = new System.Drawing.Point(4, 102);
            this.button_GCodequanxuan.Name = "button_GCodequanxuan";
            this.button_GCodequanxuan.Size = new System.Drawing.Size(70, 23);
            this.button_GCodequanxuan.TabIndex = 1;
            this.button_GCodequanxuan.Text = "代码全选";
            this.button_GCodequanxuan.UseVisualStyleBackColor = true;
            this.button_GCodequanxuan.Click += new System.EventHandler(this.button_GCodequanxuan_Click);
            // 
            // button_jitaiquanxuan
            // 
            this.button_jitaiquanxuan.Location = new System.Drawing.Point(4, 64);
            this.button_jitaiquanxuan.Name = "button_jitaiquanxuan";
            this.button_jitaiquanxuan.Size = new System.Drawing.Size(70, 23);
            this.button_jitaiquanxuan.TabIndex = 1;
            this.button_jitaiquanxuan.Text = "机台全选";
            this.button_jitaiquanxuan.UseVisualStyleBackColor = true;
            this.button_jitaiquanxuan.Click += new System.EventHandler(this.button_jitaiquanxuan_Click);
            // 
            // button_GCodequanbuxuan
            // 
            this.button_GCodequanbuxuan.Location = new System.Drawing.Point(76, 102);
            this.button_GCodequanbuxuan.Name = "button_GCodequanbuxuan";
            this.button_GCodequanbuxuan.Size = new System.Drawing.Size(76, 23);
            this.button_GCodequanbuxuan.TabIndex = 1;
            this.button_GCodequanbuxuan.Text = "代码全不选";
            this.button_GCodequanbuxuan.UseVisualStyleBackColor = true;
            this.button_GCodequanbuxuan.Click += new System.EventHandler(this.button_GCodequanbuxuan_Click);
            // 
            // button_DeletGCode
            // 
            this.button_DeletGCode.Location = new System.Drawing.Point(4, 24);
            this.button_DeletGCode.Name = "button_DeletGCode";
            this.button_DeletGCode.Size = new System.Drawing.Size(70, 23);
            this.button_DeletGCode.TabIndex = 1;
            this.button_DeletGCode.Text = "删除g代码";
            this.button_DeletGCode.UseVisualStyleBackColor = true;
            this.button_DeletGCode.Click += new System.EventHandler(this.button_DeletGCode_Click);
            // 
            // button_jitaiquanbuxuan
            // 
            this.button_jitaiquanbuxuan.Location = new System.Drawing.Point(76, 63);
            this.button_jitaiquanbuxuan.Name = "button_jitaiquanbuxuan";
            this.button_jitaiquanbuxuan.Size = new System.Drawing.Size(76, 23);
            this.button_jitaiquanbuxuan.TabIndex = 1;
            this.button_jitaiquanbuxuan.Text = "机台全不选";
            this.button_jitaiquanbuxuan.UseVisualStyleBackColor = true;
            this.button_jitaiquanbuxuan.Click += new System.EventHandler(this.button_jitaiquanbuxuan_Click);
            // 
            // button_AddGCode
            // 
            this.button_AddGCode.Location = new System.Drawing.Point(76, 24);
            this.button_AddGCode.Name = "button_AddGCode";
            this.button_AddGCode.Size = new System.Drawing.Size(73, 23);
            this.button_AddGCode.TabIndex = 1;
            this.button_AddGCode.Text = "添加g代码";
            this.button_AddGCode.UseVisualStyleBackColor = true;
            this.button_AddGCode.Click += new System.EventHandler(this.button_AddGCode_Click);
            // 
            // button_DowLoadGCode
            // 
            this.button_DowLoadGCode.Location = new System.Drawing.Point(31, 144);
            this.button_DowLoadGCode.Name = "button_DowLoadGCode";
            this.button_DowLoadGCode.Size = new System.Drawing.Size(92, 26);
            this.button_DowLoadGCode.TabIndex = 1;
            this.button_DowLoadGCode.Text = "下载";
            this.button_DowLoadGCode.UseVisualStyleBackColor = true;
            this.button_DowLoadGCode.Click += new System.EventHandler(this.button_DowLoadGCode_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataGridViewTask);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(631, 475);
            this.splitContainer2.SplitterDistance = 237;
            this.splitContainer2.TabIndex = 1;
            // 
            // dataGridViewTask
            // 
            this.dataGridViewTask.AllowUserToDeleteRows = false;
            this.dataGridViewTask.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridViewTask.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTask.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTask.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTask.Font = new System.Drawing.Font("宋体", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridViewTask.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewTask.Name = "dataGridViewTask";
            this.dataGridViewTask.RowTemplate.Height = 23;
            this.dataGridViewTask.Size = new System.Drawing.Size(631, 237);
            this.dataGridViewTask.TabIndex = 0;
            this.dataGridViewTask.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewTask_CellBeginEdit);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dataGridView_CNCGCode);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.dataGridView_GCodeSele);
            this.splitContainer3.Size = new System.Drawing.Size(631, 234);
            this.splitContainer3.SplitterDistance = 366;
            this.splitContainer3.TabIndex = 1;
            // 
            // dataGridView_CNCGCode
            // 
            this.dataGridView_CNCGCode.AllowUserToDeleteRows = false;
            this.dataGridView_CNCGCode.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView_CNCGCode.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_CNCGCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_CNCGCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_CNCGCode.Font = new System.Drawing.Font("宋体", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridView_CNCGCode.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_CNCGCode.Name = "dataGridView_CNCGCode";
            this.dataGridView_CNCGCode.RowTemplate.Height = 23;
            this.dataGridView_CNCGCode.Size = new System.Drawing.Size(366, 234);
            this.dataGridView_CNCGCode.TabIndex = 0;
            // 
            // dataGridView_GCodeSele
            // 
            this.dataGridView_GCodeSele.AllowUserToDeleteRows = false;
            this.dataGridView_GCodeSele.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView_GCodeSele.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_GCodeSele.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_GCodeSele.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_GCodeSele.Font = new System.Drawing.Font("宋体", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridView_GCodeSele.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_GCodeSele.Name = "dataGridView_GCodeSele";
            this.dataGridView_GCodeSele.RowTemplate.Height = 23;
            this.dataGridView_GCodeSele.Size = new System.Drawing.Size(261, 234);
            this.dataGridView_GCodeSele.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mComboBox_CNCTaskList
            // 
            this.mComboBox_CNCTaskList.FormattingEnabled = true;
            this.mComboBox_CNCTaskList.Location = new System.Drawing.Point(5, 20);
            this.mComboBox_CNCTaskList.Name = "mComboBox_CNCTaskList";
            this.mComboBox_CNCTaskList.Size = new System.Drawing.Size(135, 20);
            this.mComboBox_CNCTaskList.TabIndex = 10;
            this.mComboBox_CNCTaskList.SelectedIndexChanged += new System.EventHandler(this.mComboBox_CNCTaskList_SelectedIndexChanged);
            // 
            // UserControlTaskData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UserControlTaskData";
            this.Size = new System.Drawing.Size(803, 475);
            this.Load += new System.EventHandler(this.UserControlTaskData_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTask)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CNCGCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GCodeSele)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewTask;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnHisTask;
        private System.Windows.Forms.Button btnCurTask;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.DataGridView dataGridView_CNCGCode;
        private System.Windows.Forms.Button button_DowLoadGCode;
        private System.Windows.Forms.DataGridView dataGridView_GCodeSele;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_DeletGCode;
        private System.Windows.Forms.Button button_AddGCode;
        private System.Windows.Forms.Button button_jitaiquanxuan;
        private System.Windows.Forms.Button button_jitaiquanbuxuan;
        private System.Windows.Forms.Button button_GCodequanxuan;
        private System.Windows.Forms.Button button_GCodequanbuxuan;
        private System.Windows.Forms.CheckBox checkBox_XuNiSendTask;
        private System.Windows.Forms.CheckBox checkBox_AoutoSendTask;
        private mComboBox mComboBox_CNCTaskList;
        private System.Windows.Forms.Timer timer1;
    }
}
