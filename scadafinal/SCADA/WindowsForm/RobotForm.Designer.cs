namespace SCADA
{
    partial class RobotForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RobotForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.switchRobot = new System.Windows.Forms.ComboBox();
            this.spContainerRobot = new System.Windows.Forms.SplitContainer();
            this.groupBoxRobotSele = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.robotDGVInPut = new System.Windows.Forms.DataGridView();
            this.input_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.input_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.input_getPicture = new System.Windows.Forms.DataGridViewImageColumn();
            this.robotDGVOutPut = new System.Windows.Forms.DataGridView();
            this.output_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.output_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.output_getPicture = new System.Windows.Forms.DataGridViewImageColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.spContainerRobot)).BeginInit();
            this.spContainerRobot.Panel1.SuspendLayout();
            this.spContainerRobot.Panel2.SuspendLayout();
            this.spContainerRobot.SuspendLayout();
            this.groupBoxRobotSele.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.robotDGVInPut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.robotDGVOutPut)).BeginInit();
            this.SuspendLayout();
            // 
            // switchRobot
            // 
            this.switchRobot.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.switchRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.switchRobot.FormattingEnabled = true;
            this.switchRobot.Location = new System.Drawing.Point(7, 28);
            this.switchRobot.Margin = new System.Windows.Forms.Padding(4);
            this.switchRobot.Name = "switchRobot";
            this.switchRobot.Size = new System.Drawing.Size(103, 20);
            this.switchRobot.TabIndex = 0;
            this.switchRobot.SelectedIndexChanged += new System.EventHandler(this.switchRobot_SelectedIndexChanged);
            // 
            // spContainerRobot
            // 
            this.spContainerRobot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spContainerRobot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spContainerRobot.Location = new System.Drawing.Point(0, 0);
            this.spContainerRobot.Name = "spContainerRobot";
            // 
            // spContainerRobot.Panel1
            // 
            this.spContainerRobot.Panel1.Controls.Add(this.groupBoxRobotSele);
            // 
            // spContainerRobot.Panel2
            // 
            this.spContainerRobot.Panel2.Controls.Add(this.splitContainer1);
            this.spContainerRobot.Size = new System.Drawing.Size(854, 566);
            this.spContainerRobot.SplitterDistance = 151;
            this.spContainerRobot.TabIndex = 35;
            // 
            // groupBoxRobotSele
            // 
            this.groupBoxRobotSele.Controls.Add(this.switchRobot);
            this.groupBoxRobotSele.Location = new System.Drawing.Point(7, 10);
            this.groupBoxRobotSele.Name = "groupBoxRobotSele";
            this.groupBoxRobotSele.Size = new System.Drawing.Size(127, 64);
            this.groupBoxRobotSele.TabIndex = 1;
            this.groupBoxRobotSele.TabStop = false;
            this.groupBoxRobotSele.Text = "Robot选择";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.robotDGVInPut);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.robotDGVOutPut);
            this.splitContainer1.Size = new System.Drawing.Size(695, 562);
            this.splitContainer1.SplitterDistance = 358;
            this.splitContainer1.TabIndex = 1;
            // 
            // robotDGVInPut
            // 
            this.robotDGVInPut.AllowUserToAddRows = false;
            this.robotDGVInPut.AllowUserToDeleteRows = false;
            this.robotDGVInPut.BackgroundColor = System.Drawing.Color.White;
            this.robotDGVInPut.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.robotDGVInPut.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.robotDGVInPut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.robotDGVInPut.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.input_id,
            this.input_name,
            this.input_getPicture});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.robotDGVInPut.DefaultCellStyle = dataGridViewCellStyle5;
            this.robotDGVInPut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotDGVInPut.Location = new System.Drawing.Point(0, 0);
            this.robotDGVInPut.Name = "robotDGVInPut";
            this.robotDGVInPut.ReadOnly = true;
            this.robotDGVInPut.RowHeadersVisible = false;
            this.robotDGVInPut.RowTemplate.Height = 23;
            this.robotDGVInPut.Size = new System.Drawing.Size(358, 562);
            this.robotDGVInPut.TabIndex = 0;
            // 
            // input_id
            // 
            this.input_id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.input_id.DataPropertyName = "address";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.input_id.DefaultCellStyle = dataGridViewCellStyle2;
            this.input_id.HeaderText = "   输入信号地址";
            this.input_id.Name = "input_id";
            this.input_id.ReadOnly = true;
            // 
            // input_name
            // 
            this.input_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.input_name.DataPropertyName = "name";
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.input_name.DefaultCellStyle = dataGridViewCellStyle3;
            this.input_name.HeaderText = "   定义信号";
            this.input_name.Name = "input_name";
            this.input_name.ReadOnly = true;
            // 
            // input_getPicture
            // 
            this.input_getPicture.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.input_getPicture.DataPropertyName = "getPicture";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle4.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle4.NullValue")));
            this.input_getPicture.DefaultCellStyle = dataGridViewCellStyle4;
            this.input_getPicture.HeaderText = "   信号状态";
            this.input_getPicture.Image = global::SCADA.Properties.Resources.top_bar_green;
            this.input_getPicture.Name = "input_getPicture";
            this.input_getPicture.ReadOnly = true;
            this.input_getPicture.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.input_getPicture.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // robotDGVOutPut
            // 
            this.robotDGVOutPut.AllowUserToAddRows = false;
            this.robotDGVOutPut.AllowUserToDeleteRows = false;
            this.robotDGVOutPut.BackgroundColor = System.Drawing.Color.White;
            this.robotDGVOutPut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.robotDGVOutPut.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.output_id,
            this.output_name,
            this.output_getPicture});
            this.robotDGVOutPut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotDGVOutPut.Location = new System.Drawing.Point(0, 0);
            this.robotDGVOutPut.Name = "robotDGVOutPut";
            this.robotDGVOutPut.ReadOnly = true;
            this.robotDGVOutPut.RowHeadersVisible = false;
            this.robotDGVOutPut.RowTemplate.Height = 23;
            this.robotDGVOutPut.Size = new System.Drawing.Size(333, 562);
            this.robotDGVOutPut.TabIndex = 0;
            // 
            // output_id
            // 
            this.output_id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.output_id.DataPropertyName = "address";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.output_id.DefaultCellStyle = dataGridViewCellStyle6;
            this.output_id.HeaderText = "输出信号地址";
            this.output_id.Name = "output_id";
            this.output_id.ReadOnly = true;
            // 
            // output_name
            // 
            this.output_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.output_name.DataPropertyName = "name";
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.output_name.DefaultCellStyle = dataGridViewCellStyle7;
            this.output_name.HeaderText = "   定义信号";
            this.output_name.Name = "output_name";
            this.output_name.ReadOnly = true;
            // 
            // output_getPicture
            // 
            this.output_getPicture.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.output_getPicture.DataPropertyName = "getPicture";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle8.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle8.NullValue")));
            this.output_getPicture.DefaultCellStyle = dataGridViewCellStyle8;
            this.output_getPicture.HeaderText = "   信号状态";
            this.output_getPicture.Image = ((System.Drawing.Image)(resources.GetObject("output_getPicture.Image")));
            this.output_getPicture.Name = "output_getPicture";
            this.output_getPicture.ReadOnly = true;
            this.output_getPicture.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.output_getPicture.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewImageColumn1.DataPropertyName = "getPicture";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle9.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle9.NullValue")));
            this.dataGridViewImageColumn1.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewImageColumn1.HeaderText = "   信号状态";
            this.dataGridViewImageColumn1.Image = global::SCADA.Properties.Resources.top_bar_green;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewImageColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewImageColumn2
            // 
            this.dataGridViewImageColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewImageColumn2.DataPropertyName = "getPicture";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle10.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle10.NullValue")));
            this.dataGridViewImageColumn2.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewImageColumn2.HeaderText = "   信号状态";
            this.dataGridViewImageColumn2.Image = ((System.Drawing.Image)(resources.GetObject("dataGridViewImageColumn2.Image")));
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            this.dataGridViewImageColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewImageColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // RobotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(854, 566);
            this.Controls.Add(this.spContainerRobot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "RobotForm";
            this.Text = "RobotForm";
            this.Load += new System.EventHandler(this.RobotForm_Load);
            this.SizeChanged += new System.EventHandler(this.RobotForm_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.RobotForm_VisibleChanged);
            this.spContainerRobot.Panel1.ResumeLayout(false);
            this.spContainerRobot.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spContainerRobot)).EndInit();
            this.spContainerRobot.ResumeLayout(false);
            this.groupBoxRobotSele.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.robotDGVInPut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.robotDGVOutPut)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spContainerRobot;
        private System.Windows.Forms.DataGridView robotDGVOutPut;
        private System.Windows.Forms.ComboBox switchRobot;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridView robotDGVInPut;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBoxRobotSele;
        private System.Windows.Forms.DataGridViewTextBoxColumn output_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn output_name;
        private System.Windows.Forms.DataGridViewImageColumn output_getPicture;
        private System.Windows.Forms.DataGridViewTextBoxColumn input_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn input_name;
        private System.Windows.Forms.DataGridViewImageColumn input_getPicture;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn2;

    }
}
