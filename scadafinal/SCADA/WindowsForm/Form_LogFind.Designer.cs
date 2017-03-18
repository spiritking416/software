namespace SCADA.WindowsForm
{
    partial class Form_LogFind
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_FindStr = new System.Windows.Forms.TextBox();
            this.button_FindNext = new System.Windows.Forms.Button();
            this.button_Cans = new System.Windows.Forms.Button();
            this.button_FindUp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "查找内容：";
            // 
            // textBox_FindStr
            // 
            this.textBox_FindStr.Location = new System.Drawing.Point(83, 38);
            this.textBox_FindStr.Name = "textBox_FindStr";
            this.textBox_FindStr.Size = new System.Drawing.Size(189, 21);
            this.textBox_FindStr.TabIndex = 1;
            this.textBox_FindStr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_FindStr_KeyDown);
            // 
            // button_FindNext
            // 
            this.button_FindNext.Location = new System.Drawing.Point(34, 81);
            this.button_FindNext.Name = "button_FindNext";
            this.button_FindNext.Size = new System.Drawing.Size(95, 42);
            this.button_FindNext.TabIndex = 2;
            this.button_FindNext.Text = "查找下一个";
            this.button_FindNext.UseVisualStyleBackColor = true;
            this.button_FindNext.Click += new System.EventHandler(this.button_FindNext_Click);
            // 
            // button_Cans
            // 
            this.button_Cans.Location = new System.Drawing.Point(306, 81);
            this.button_Cans.Name = "button_Cans";
            this.button_Cans.Size = new System.Drawing.Size(95, 42);
            this.button_Cans.TabIndex = 3;
            this.button_Cans.Text = "取消";
            this.button_Cans.UseVisualStyleBackColor = true;
            this.button_Cans.Click += new System.EventHandler(this.button_Cans_Click);
            // 
            // button_FindUp
            // 
            this.button_FindUp.Location = new System.Drawing.Point(177, 81);
            this.button_FindUp.Name = "button_FindUp";
            this.button_FindUp.Size = new System.Drawing.Size(95, 42);
            this.button_FindUp.TabIndex = 2;
            this.button_FindUp.Text = "查找上一个";
            this.button_FindUp.UseVisualStyleBackColor = true;
            this.button_FindUp.Click += new System.EventHandler(this.button_FindNext_Click);
            // 
            // Form_LogFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 135);
            this.ControlBox = false;
            this.Controls.Add(this.button_Cans);
            this.Controls.Add(this.button_FindUp);
            this.Controls.Add(this.button_FindNext);
            this.Controls.Add(this.textBox_FindStr);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_LogFind";
            this.Text = "查找";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_FindStr;
        private System.Windows.Forms.Button button_FindNext;
        private System.Windows.Forms.Button button_Cans;
        private System.Windows.Forms.Button button_FindUp;
    }
}