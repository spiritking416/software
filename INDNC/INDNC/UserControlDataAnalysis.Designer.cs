namespace INDNC
{
    partial class UserControlDataAnalysis
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButtonDianliu = new System.Windows.Forms.RadioButton();
            this.radioButtonAlarm = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.radioButtonDianliu);
            this.panel1.Controls.Add(this.radioButtonAlarm);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(954, 29);
            this.panel1.TabIndex = 0;
            // 
            // radioButtonDianliu
            // 
            this.radioButtonDianliu.AutoSize = true;
            this.radioButtonDianliu.Location = new System.Drawing.Point(209, 7);
            this.radioButtonDianliu.Name = "radioButtonDianliu";
            this.radioButtonDianliu.Size = new System.Drawing.Size(119, 16);
            this.radioButtonDianliu.TabIndex = 1;
            this.radioButtonDianliu.TabStop = true;
            this.radioButtonDianliu.Text = "机床加工电流分析";
            this.radioButtonDianliu.UseVisualStyleBackColor = true;
            // 
            // radioButtonAlarm
            // 
            this.radioButtonAlarm.AutoSize = true;
            this.radioButtonAlarm.Location = new System.Drawing.Point(68, 7);
            this.radioButtonAlarm.Name = "radioButtonAlarm";
            this.radioButtonAlarm.Size = new System.Drawing.Size(71, 16);
            this.radioButtonAlarm.TabIndex = 1;
            this.radioButtonAlarm.TabStop = true;
            this.radioButtonAlarm.Text = "告警分析";
            this.radioButtonAlarm.UseVisualStyleBackColor = true;
            this.radioButtonAlarm.CheckedChanged += new System.EventHandler(this.radioButtonAlarm_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(3, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(954, 484);
            this.panel2.TabIndex = 1;
            // 
            // UserControlDataAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "UserControlDataAnalysis";
            this.Size = new System.Drawing.Size(960, 522);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.RadioButton radioButtonDianliu;
        public System.Windows.Forms.RadioButton radioButtonAlarm;
        private System.Windows.Forms.Panel panel2;
    }
}
