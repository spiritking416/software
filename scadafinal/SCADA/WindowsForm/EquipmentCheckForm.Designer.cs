namespace SCADA
{
    partial class EquipmentCheckForm
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
            this.dataGridView_Checkcontent = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBox_YiChang = new System.Windows.Forms.CheckBox();
            this.checkBox_ZhengChang = new System.Windows.Forms.CheckBox();
            this.checkBox_DongTai = new System.Windows.Forms.CheckBox();
            this.button_UpDataTable = new System.Windows.Forms.Button();
            this.button_ClreaLine = new System.Windows.Forms.Button();
            this.button_StarLine = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Checkcontent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_Checkcontent
            // 
            this.dataGridView_Checkcontent.AllowUserToDeleteRows = false;
            this.dataGridView_Checkcontent.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_Checkcontent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Checkcontent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Checkcontent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dataGridView_Checkcontent.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Checkcontent.MultiSelect = false;
            this.dataGridView_Checkcontent.Name = "dataGridView_Checkcontent";
            this.dataGridView_Checkcontent.ReadOnly = true;
            this.dataGridView_Checkcontent.RowHeadersVisible = false;
            this.dataGridView_Checkcontent.RowTemplate.Height = 23;
            this.dataGridView_Checkcontent.Size = new System.Drawing.Size(863, 381);
            this.dataGridView_Checkcontent.TabIndex = 7;
            this.dataGridView_Checkcontent.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView_Checkcontent_CellPainting);
            this.dataGridView_Checkcontent.VisibleChanged += new System.EventHandler(this.dataGridView_Checkcontent_VisibleChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView_Checkcontent);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_YiChang);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_ZhengChang);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_DongTai);
            this.splitContainer1.Panel2.Controls.Add(this.button_UpDataTable);
            this.splitContainer1.Panel2.Controls.Add(this.button_ClreaLine);
            this.splitContainer1.Panel2.Controls.Add(this.button_StarLine);
            this.splitContainer1.Size = new System.Drawing.Size(863, 426);
            this.splitContainer1.SplitterDistance = 381;
            this.splitContainer1.TabIndex = 8;
            // 
            // checkBox_YiChang
            // 
            this.checkBox_YiChang.AutoSize = true;
            this.checkBox_YiChang.Checked = true;
            this.checkBox_YiChang.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_YiChang.Location = new System.Drawing.Point(222, 14);
            this.checkBox_YiChang.Name = "checkBox_YiChang";
            this.checkBox_YiChang.Size = new System.Drawing.Size(72, 16);
            this.checkBox_YiChang.TabIndex = 9;
            this.checkBox_YiChang.Text = "显示故障";
            this.checkBox_YiChang.UseVisualStyleBackColor = true;
            this.checkBox_YiChang.CheckedChanged += new System.EventHandler(this.checkBox_YiChang_CheckedChanged);
            // 
            // checkBox_ZhengChang
            // 
            this.checkBox_ZhengChang.AutoSize = true;
            this.checkBox_ZhengChang.Checked = true;
            this.checkBox_ZhengChang.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_ZhengChang.Location = new System.Drawing.Point(111, 14);
            this.checkBox_ZhengChang.Name = "checkBox_ZhengChang";
            this.checkBox_ZhengChang.Size = new System.Drawing.Size(72, 16);
            this.checkBox_ZhengChang.TabIndex = 9;
            this.checkBox_ZhengChang.Text = "显示正常";
            this.checkBox_ZhengChang.UseVisualStyleBackColor = true;
            this.checkBox_ZhengChang.CheckedChanged += new System.EventHandler(this.checkBox_ZhengChang_CheckedChanged);
            // 
            // checkBox_DongTai
            // 
            this.checkBox_DongTai.AutoSize = true;
            this.checkBox_DongTai.Location = new System.Drawing.Point(24, 14);
            this.checkBox_DongTai.Name = "checkBox_DongTai";
            this.checkBox_DongTai.Size = new System.Drawing.Size(48, 16);
            this.checkBox_DongTai.TabIndex = 9;
            this.checkBox_DongTai.Text = "动态";
            this.checkBox_DongTai.UseVisualStyleBackColor = true;
            this.checkBox_DongTai.CheckedChanged += new System.EventHandler(this.checkBox_DongTai_CheckedChanged);
            // 
            // button_UpDataTable
            // 
            this.button_UpDataTable.Location = new System.Drawing.Point(342, 5);
            this.button_UpDataTable.Name = "button_UpDataTable";
            this.button_UpDataTable.Size = new System.Drawing.Size(86, 32);
            this.button_UpDataTable.TabIndex = 0;
            this.button_UpDataTable.Text = "刷新";
            this.button_UpDataTable.UseVisualStyleBackColor = true;
            this.button_UpDataTable.Click += new System.EventHandler(this.button_UpDataTable_Click);
            // 
            // button_ClreaLine
            // 
            this.button_ClreaLine.Location = new System.Drawing.Point(606, 5);
            this.button_ClreaLine.Name = "button_ClreaLine";
            this.button_ClreaLine.Size = new System.Drawing.Size(86, 32);
            this.button_ClreaLine.TabIndex = 0;
            this.button_ClreaLine.Text = "清线";
            this.button_ClreaLine.UseVisualStyleBackColor = true;
            this.button_ClreaLine.Click += new System.EventHandler(this.button_ClreaLine_Click);
            // 
            // button_StarLine
            // 
            this.button_StarLine.Location = new System.Drawing.Point(474, 5);
            this.button_StarLine.Name = "button_StarLine";
            this.button_StarLine.Size = new System.Drawing.Size(86, 32);
            this.button_StarLine.TabIndex = 0;
            this.button_StarLine.Text = "开线";
            this.button_StarLine.UseVisualStyleBackColor = true;
            this.button_StarLine.Visible = false;
            this.button_StarLine.Click += new System.EventHandler(this.button_StarLine_Click);
            // 
            // EquipmentCheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 426);
            this.Controls.Add(this.splitContainer1);
            this.Name = "EquipmentCheckForm";
            this.Text = "EquipmentCheckForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Checkcontent)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_Checkcontent;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button_StarLine;
        private System.Windows.Forms.CheckBox checkBox_DongTai;
        private System.Windows.Forms.CheckBox checkBox_YiChang;
        private System.Windows.Forms.CheckBox checkBox_ZhengChang;
        private System.Windows.Forms.Button button_UpDataTable;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button_ClreaLine;
    }
}