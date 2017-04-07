using System.Windows.Forms;

namespace INDNC
{
    partial class FormMain
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonDataAnalysis = new System.Windows.Forms.Button();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.PowderBlue;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(4, 107);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(959, 522);
            this.panel1.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.buttonDataAnalysis);
            this.panel3.Controls.Add(this.buttonSetting);
            this.panel3.Controls.Add(this.buttonCheck);
            this.panel3.Controls.Add(this.buttonHome);
            this.panel3.Location = new System.Drawing.Point(4, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(959, 104);
            this.panel3.TabIndex = 11;
            // 
            // buttonDataAnalysis
            // 
            this.buttonDataAnalysis.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonDataAnalysis.FlatAppearance.BorderSize = 0;
            this.buttonDataAnalysis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDataAnalysis.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonDataAnalysis.Image = ((System.Drawing.Image)(resources.GetObject("buttonDataAnalysis.Image")));
            this.buttonDataAnalysis.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonDataAnalysis.Location = new System.Drawing.Point(234, 15);
            this.buttonDataAnalysis.Name = "buttonDataAnalysis";
            this.buttonDataAnalysis.Size = new System.Drawing.Size(68, 73);
            this.buttonDataAnalysis.TabIndex = 1;
            this.buttonDataAnalysis.Text = "数据分析";
            this.buttonDataAnalysis.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonDataAnalysis.UseVisualStyleBackColor = true;
            this.buttonDataAnalysis.Click += new System.EventHandler(this.buttonDataAnalysis_Click);
            // 
            // buttonSetting
            // 
            this.buttonSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonSetting.FlatAppearance.BorderSize = 0;
            this.buttonSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSetting.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSetting.Image = ((System.Drawing.Image)(resources.GetObject("buttonSetting.Image")));
            this.buttonSetting.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonSetting.Location = new System.Drawing.Point(160, 15);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(68, 73);
            this.buttonSetting.TabIndex = 0;
            this.buttonSetting.Text = "设置";
            this.buttonSetting.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonSetting.UseVisualStyleBackColor = true;
            this.buttonSetting.Click += new System.EventHandler(this.buttonSetting_Click);
            // 
            // buttonCheck
            // 
            this.buttonCheck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonCheck.FlatAppearance.BorderSize = 0;
            this.buttonCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCheck.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCheck.Image = ((System.Drawing.Image)(resources.GetObject("buttonCheck.Image")));
            this.buttonCheck.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonCheck.Location = new System.Drawing.Point(86, 15);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(68, 73);
            this.buttonCheck.TabIndex = 0;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonHome.FlatAppearance.BorderSize = 0;
            this.buttonHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHome.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonHome.Image = ((System.Drawing.Image)(resources.GetObject("buttonHome.Image")));
            this.buttonHome.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonHome.Location = new System.Drawing.Point(12, 15);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(68, 73);
            this.buttonHome.TabIndex = 0;
            this.buttonHome.Text = "Home";
            this.buttonHome.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.ClientSize = new System.Drawing.Size(966, 637);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INDNC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Panel panel1;
        private Panel panel3;
        private Button buttonHome;
        private Button buttonCheck;
        private Button buttonSetting;
        private Button buttonDataAnalysis;
    }
}

