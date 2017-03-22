namespace SCADA
{
    partial class HomeForm
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
            this.HomeTimer = new System.Windows.Forms.Timer(this.components);
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.labelUnconnettext = new System.Windows.Forms.Label();
            this.labelKongXianText = new System.Windows.Forms.Label();
            this.labelRuningText = new System.Windows.Forms.Label();
            this.labelAlarText = new System.Windows.Forms.Label();
            this.pictureBoxUnConnetColor = new System.Windows.Forms.PictureBox();
            this.pictureBoxKongXianColor = new System.Windows.Forms.PictureBox();
            this.pictureBoxRuningColor = new System.Windows.Forms.PictureBox();
            this.pictureBoxArrColor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUnConnetColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKongXianColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRuningColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrColor)).BeginInit();
            this.SuspendLayout();
            // 
            // HomeTimer
            // 
            this.HomeTimer.Tick += new System.EventHandler(this.HomeTimer_Tick);
            // 
            // pictureBox15
            // 
            this.pictureBox15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox15.Location = new System.Drawing.Point(919, 527);
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.Size = new System.Drawing.Size(50, 58);
            this.pictureBox15.TabIndex = 154;
            this.pictureBox15.TabStop = false;
            // 
            // labelUnconnettext
            // 
            this.labelUnconnettext.AutoSize = true;
            this.labelUnconnettext.Location = new System.Drawing.Point(13, 13);
            this.labelUnconnettext.Name = "labelUnconnettext";
            this.labelUnconnettext.Size = new System.Drawing.Size(41, 12);
            this.labelUnconnettext.TabIndex = 155;
            this.labelUnconnettext.Text = "离线：";
            // 
            // labelKongXianText
            // 
            this.labelKongXianText.AutoSize = true;
            this.labelKongXianText.Location = new System.Drawing.Point(111, 13);
            this.labelKongXianText.Name = "labelKongXianText";
            this.labelKongXianText.Size = new System.Drawing.Size(41, 12);
            this.labelKongXianText.TabIndex = 155;
            this.labelKongXianText.Text = "空闲：";
            // 
            // labelRuningText
            // 
            this.labelRuningText.AutoSize = true;
            this.labelRuningText.Location = new System.Drawing.Point(206, 13);
            this.labelRuningText.Name = "labelRuningText";
            this.labelRuningText.Size = new System.Drawing.Size(41, 12);
            this.labelRuningText.TabIndex = 155;
            this.labelRuningText.Text = "运行：";
            // 
            // labelAlarText
            // 
            this.labelAlarText.AutoSize = true;
            this.labelAlarText.Location = new System.Drawing.Point(307, 13);
            this.labelAlarText.Name = "labelAlarText";
            this.labelAlarText.Size = new System.Drawing.Size(41, 12);
            this.labelAlarText.TabIndex = 155;
            this.labelAlarText.Text = "报警：";
            // 
            // pictureBoxUnConnetColor
            // 
            this.pictureBoxUnConnetColor.Location = new System.Drawing.Point(61, 9);
            this.pictureBoxUnConnetColor.Name = "pictureBoxUnConnetColor";
            this.pictureBoxUnConnetColor.Size = new System.Drawing.Size(29, 21);
            this.pictureBoxUnConnetColor.TabIndex = 156;
            this.pictureBoxUnConnetColor.TabStop = false;
            // 
            // pictureBoxKongXianColor
            // 
            this.pictureBoxKongXianColor.Location = new System.Drawing.Point(161, 9);
            this.pictureBoxKongXianColor.Name = "pictureBoxKongXianColor";
            this.pictureBoxKongXianColor.Size = new System.Drawing.Size(29, 21);
            this.pictureBoxKongXianColor.TabIndex = 156;
            this.pictureBoxKongXianColor.TabStop = false;
            // 
            // pictureBoxRuningColor
            // 
            this.pictureBoxRuningColor.Location = new System.Drawing.Point(254, 9);
            this.pictureBoxRuningColor.Name = "pictureBoxRuningColor";
            this.pictureBoxRuningColor.Size = new System.Drawing.Size(29, 21);
            this.pictureBoxRuningColor.TabIndex = 156;
            this.pictureBoxRuningColor.TabStop = false;
            // 
            // pictureBoxArrColor
            // 
            this.pictureBoxArrColor.Location = new System.Drawing.Point(360, 9);
            this.pictureBoxArrColor.Name = "pictureBoxArrColor";
            this.pictureBoxArrColor.Size = new System.Drawing.Size(29, 21);
            this.pictureBoxArrColor.TabIndex = 156;
            this.pictureBoxArrColor.TabStop = false;
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1141, 469);
            this.Controls.Add(this.pictureBoxRuningColor);
            this.Controls.Add(this.pictureBoxArrColor);
            this.Controls.Add(this.pictureBoxKongXianColor);
            this.Controls.Add(this.pictureBoxUnConnetColor);
            this.Controls.Add(this.labelAlarText);
            this.Controls.Add(this.labelRuningText);
            this.Controls.Add(this.labelKongXianText);
            this.Controls.Add(this.labelUnconnettext);
            this.Controls.Add(this.pictureBox15);
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            this.Load += new System.EventHandler(this.HomeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUnConnetColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKongXianColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRuningColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer HomeTimer;
        private System.Windows.Forms.PictureBox pictureBox15;
        private System.Windows.Forms.Label labelUnconnettext;
        private System.Windows.Forms.Label labelKongXianText;
        private System.Windows.Forms.Label labelRuningText;
        private System.Windows.Forms.Label labelAlarText;
        private System.Windows.Forms.PictureBox pictureBoxUnConnetColor;
        private System.Windows.Forms.PictureBox pictureBoxKongXianColor;
        private System.Windows.Forms.PictureBox pictureBoxRuningColor;
        private System.Windows.Forms.PictureBox pictureBoxArrColor;
    }
}