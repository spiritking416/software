namespace SCADA
{
    partial class TaskDataForm
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
            this.panelTaskData = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelTaskData
            // 
            this.panelTaskData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTaskData.Location = new System.Drawing.Point(0, 0);
            this.panelTaskData.Name = "panelTaskData";
            this.panelTaskData.Size = new System.Drawing.Size(702, 421);
            this.panelTaskData.TabIndex = 0;
            // 
            // TaskDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 421);
            this.Controls.Add(this.panelTaskData);
            this.Name = "TaskDataForm";
            this.Text = "派发任务";
            this.Load += new System.EventHandler(this.TaskDataForm_Load);
            this.SizeChanged += new System.EventHandler(this.TaskDataForm_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTaskData;
    }
}