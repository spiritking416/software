namespace SCADA
{
    partial class RfidForm
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
            this.comboBoxRFIDSelet = new System.Windows.Forms.ComboBox();
            this.dataGridViewRFIDReadMessage = new System.Windows.Forms.DataGridView();
            this.groupBox_IP = new System.Windows.Forms.GroupBox();
            this.labelRFIDIPADRESSVULE = new System.Windows.Forms.Label();
            this.groupBox_Poort = new System.Windows.Forms.GroupBox();
            this.labelRFIDPOORTVELU = new System.Windows.Forms.Label();
            this.labelStadeText = new System.Windows.Forms.Label();
            this.comboBoxRFIDPLCSele = new System.Windows.Forms.ComboBox();
            this.labelReadAdressStarValue = new System.Windows.Forms.Label();
            this.labelWriteAdressStarValue = new System.Windows.Forms.Label();
            this.groupBoxSelePLC = new System.Windows.Forms.GroupBox();
            this.groupBoxSeleRFID = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBoxConnet = new System.Windows.Forms.GroupBox();
            this.labelLinckText = new System.Windows.Forms.Label();
            this.pictureBox_LinkStade = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_UpDataMessage = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox_ReadDStar = new System.Windows.Forms.GroupBox();
            this.groupBox_WriteDStar = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.RFIDDataDataTable_RemoveAll_bt = new System.Windows.Forms.Button();
            this.groupBoxMsshow = new System.Windows.Forms.GroupBox();
            this.comboBoxMSGType = new System.Windows.Forms.ComboBox();
            this.timer_UpData = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRFIDReadMessage)).BeginInit();
            this.groupBox_IP.SuspendLayout();
            this.groupBox_Poort.SuspendLayout();
            this.groupBoxSelePLC.SuspendLayout();
            this.groupBoxSeleRFID.SuspendLayout();
            this.groupBoxConnet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LinkStade)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox_ReadDStar.SuspendLayout();
            this.groupBox_WriteDStar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxMsshow.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxRFIDSelet
            // 
            this.comboBoxRFIDSelet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRFIDSelet.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBoxRFIDSelet.FormattingEnabled = true;
            this.comboBoxRFIDSelet.Location = new System.Drawing.Point(10, 22);
            this.comboBoxRFIDSelet.Name = "comboBoxRFIDSelet";
            this.comboBoxRFIDSelet.Size = new System.Drawing.Size(103, 24);
            this.comboBoxRFIDSelet.TabIndex = 18;
            this.comboBoxRFIDSelet.TextChanged += new System.EventHandler(this.comboBoxRFIDSelet_TextChanged);
            // 
            // dataGridViewRFIDReadMessage
            // 
            this.dataGridViewRFIDReadMessage.AllowUserToDeleteRows = false;
            this.dataGridViewRFIDReadMessage.AllowUserToOrderColumns = true;
            this.dataGridViewRFIDReadMessage.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRFIDReadMessage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRFIDReadMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRFIDReadMessage.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewRFIDReadMessage.Name = "dataGridViewRFIDReadMessage";
            this.dataGridViewRFIDReadMessage.ReadOnly = true;
            this.dataGridViewRFIDReadMessage.RowHeadersVisible = false;
            this.dataGridViewRFIDReadMessage.RowTemplate.Height = 23;
            this.dataGridViewRFIDReadMessage.Size = new System.Drawing.Size(790, 628);
            this.dataGridViewRFIDReadMessage.TabIndex = 5;
            this.dataGridViewRFIDReadMessage.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewRFIDReadMessage_DataError);
            this.dataGridViewRFIDReadMessage.VisibleChanged += new System.EventHandler(this.dataGridViewRFIDReadMessage_VisibleChanged);
            this.dataGridViewRFIDReadMessage.MouseEnter += new System.EventHandler(this.dataGridViewRFIDReadMessage_MouseEnter);
            this.dataGridViewRFIDReadMessage.MouseLeave += new System.EventHandler(this.dataGridViewRFIDReadMessage_MouseLeave);
            // 
            // groupBox_IP
            // 
            this.groupBox_IP.Controls.Add(this.labelRFIDIPADRESSVULE);
            this.groupBox_IP.Location = new System.Drawing.Point(3, 504);
            this.groupBox_IP.Name = "groupBox_IP";
            this.groupBox_IP.Size = new System.Drawing.Size(119, 53);
            this.groupBox_IP.TabIndex = 30;
            this.groupBox_IP.TabStop = false;
            this.groupBox_IP.Text = "IP地址";
            this.groupBox_IP.Visible = false;
            // 
            // labelRFIDIPADRESSVULE
            // 
            this.labelRFIDIPADRESSVULE.AutoSize = true;
            this.labelRFIDIPADRESSVULE.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelRFIDIPADRESSVULE.Location = new System.Drawing.Point(10, 21);
            this.labelRFIDIPADRESSVULE.Name = "labelRFIDIPADRESSVULE";
            this.labelRFIDIPADRESSVULE.Size = new System.Drawing.Size(96, 16);
            this.labelRFIDIPADRESSVULE.TabIndex = 19;
            this.labelRFIDIPADRESSVULE.Text = "192.168.1.1";
            // 
            // groupBox_Poort
            // 
            this.groupBox_Poort.Controls.Add(this.labelRFIDPOORTVELU);
            this.groupBox_Poort.Location = new System.Drawing.Point(3, 445);
            this.groupBox_Poort.Name = "groupBox_Poort";
            this.groupBox_Poort.Size = new System.Drawing.Size(119, 53);
            this.groupBox_Poort.TabIndex = 30;
            this.groupBox_Poort.TabStop = false;
            this.groupBox_Poort.Text = "端口";
            this.groupBox_Poort.Visible = false;
            // 
            // labelRFIDPOORTVELU
            // 
            this.labelRFIDPOORTVELU.AutoSize = true;
            this.labelRFIDPOORTVELU.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelRFIDPOORTVELU.Location = new System.Drawing.Point(20, 22);
            this.labelRFIDPOORTVELU.Name = "labelRFIDPOORTVELU";
            this.labelRFIDPOORTVELU.Size = new System.Drawing.Size(48, 16);
            this.labelRFIDPOORTVELU.TabIndex = 19;
            this.labelRFIDPOORTVELU.Text = "10001";
            // 
            // labelStadeText
            // 
            this.labelStadeText.AutoSize = true;
            this.labelStadeText.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelStadeText.Location = new System.Drawing.Point(9, 21);
            this.labelStadeText.Name = "labelStadeText";
            this.labelStadeText.Size = new System.Drawing.Size(40, 16);
            this.labelStadeText.TabIndex = 19;
            this.labelStadeText.Text = "状态";
            // 
            // comboBoxRFIDPLCSele
            // 
            this.comboBoxRFIDPLCSele.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRFIDPLCSele.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBoxRFIDPLCSele.FormattingEnabled = true;
            this.comboBoxRFIDPLCSele.Location = new System.Drawing.Point(10, 21);
            this.comboBoxRFIDPLCSele.Name = "comboBoxRFIDPLCSele";
            this.comboBoxRFIDPLCSele.Size = new System.Drawing.Size(103, 24);
            this.comboBoxRFIDPLCSele.TabIndex = 18;
            this.comboBoxRFIDPLCSele.TextChanged += new System.EventHandler(this.comboBoxRFIDPLCSele_TextChanged);
            // 
            // labelReadAdressStarValue
            // 
            this.labelReadAdressStarValue.AutoSize = true;
            this.labelReadAdressStarValue.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelReadAdressStarValue.Location = new System.Drawing.Point(23, 20);
            this.labelReadAdressStarValue.Name = "labelReadAdressStarValue";
            this.labelReadAdressStarValue.Size = new System.Drawing.Size(16, 16);
            this.labelReadAdressStarValue.TabIndex = 19;
            this.labelReadAdressStarValue.Text = "0";
            // 
            // labelWriteAdressStarValue
            // 
            this.labelWriteAdressStarValue.AutoSize = true;
            this.labelWriteAdressStarValue.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelWriteAdressStarValue.Location = new System.Drawing.Point(23, 19);
            this.labelWriteAdressStarValue.Name = "labelWriteAdressStarValue";
            this.labelWriteAdressStarValue.Size = new System.Drawing.Size(16, 16);
            this.labelWriteAdressStarValue.TabIndex = 19;
            this.labelWriteAdressStarValue.Text = "0";
            // 
            // groupBoxSelePLC
            // 
            this.groupBoxSelePLC.Controls.Add(this.comboBoxRFIDPLCSele);
            this.groupBoxSelePLC.Location = new System.Drawing.Point(3, 121);
            this.groupBoxSelePLC.Name = "groupBoxSelePLC";
            this.groupBoxSelePLC.Size = new System.Drawing.Size(119, 53);
            this.groupBoxSelePLC.TabIndex = 30;
            this.groupBoxSelePLC.TabStop = false;
            this.groupBoxSelePLC.Text = "选择PLC";
            this.groupBoxSelePLC.Visible = false;
            // 
            // groupBoxSeleRFID
            // 
            this.groupBoxSeleRFID.Controls.Add(this.comboBoxRFIDSelet);
            this.groupBoxSeleRFID.Location = new System.Drawing.Point(3, 180);
            this.groupBoxSeleRFID.Name = "groupBoxSeleRFID";
            this.groupBoxSeleRFID.Size = new System.Drawing.Size(119, 53);
            this.groupBoxSeleRFID.TabIndex = 30;
            this.groupBoxSeleRFID.TabStop = false;
            this.groupBoxSeleRFID.Text = "选择RFID";
            this.groupBoxSeleRFID.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 592);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(47, 23);
            this.button1.TabIndex = 31;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBoxConnet
            // 
            this.groupBoxConnet.Controls.Add(this.labelLinckText);
            this.groupBoxConnet.Controls.Add(this.pictureBox_LinkStade);
            this.groupBoxConnet.Location = new System.Drawing.Point(3, 62);
            this.groupBoxConnet.Name = "groupBoxConnet";
            this.groupBoxConnet.Size = new System.Drawing.Size(119, 53);
            this.groupBoxConnet.TabIndex = 30;
            this.groupBoxConnet.TabStop = false;
            this.groupBoxConnet.Text = "RFID连接状态";
            this.groupBoxConnet.Visible = false;
            // 
            // labelLinckText
            // 
            this.labelLinckText.AutoSize = true;
            this.labelLinckText.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelLinckText.Location = new System.Drawing.Point(13, 24);
            this.labelLinckText.Name = "labelLinckText";
            this.labelLinckText.Size = new System.Drawing.Size(16, 16);
            this.labelLinckText.TabIndex = 19;
            this.labelLinckText.Text = "0";
            // 
            // pictureBox_LinkStade
            // 
            this.pictureBox_LinkStade.Location = new System.Drawing.Point(90, 24);
            this.pictureBox_LinkStade.Name = "pictureBox_LinkStade";
            this.pictureBox_LinkStade.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_LinkStade.TabIndex = 0;
            this.pictureBox_LinkStade.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelStadeText);
            this.groupBox4.Location = new System.Drawing.Point(14, 511);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(119, 61);
            this.groupBox4.TabIndex = 30;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "RFID读写状态";
            this.groupBox4.Visible = false;
            // 
            // button_UpDataMessage
            // 
            this.button_UpDataMessage.Location = new System.Drawing.Point(3, 400);
            this.button_UpDataMessage.Name = "button_UpDataMessage";
            this.button_UpDataMessage.Size = new System.Drawing.Size(94, 39);
            this.button_UpDataMessage.TabIndex = 20;
            this.button_UpDataMessage.Text = "刷新";
            this.button_UpDataMessage.UseVisualStyleBackColor = true;
            this.button_UpDataMessage.Click += new System.EventHandler(this.button_UpDataMessage_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(58, 592);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 32;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox_ReadDStar
            // 
            this.groupBox_ReadDStar.Controls.Add(this.labelReadAdressStarValue);
            this.groupBox_ReadDStar.Location = new System.Drawing.Point(3, 239);
            this.groupBox_ReadDStar.Name = "groupBox_ReadDStar";
            this.groupBox_ReadDStar.Size = new System.Drawing.Size(119, 52);
            this.groupBox_ReadDStar.TabIndex = 30;
            this.groupBox_ReadDStar.TabStop = false;
            this.groupBox_ReadDStar.Text = "读起始地址";
            this.groupBox_ReadDStar.Visible = false;
            // 
            // groupBox_WriteDStar
            // 
            this.groupBox_WriteDStar.Controls.Add(this.labelWriteAdressStarValue);
            this.groupBox_WriteDStar.Location = new System.Drawing.Point(3, 297);
            this.groupBox_WriteDStar.Name = "groupBox_WriteDStar";
            this.groupBox_WriteDStar.Size = new System.Drawing.Size(119, 52);
            this.groupBox_WriteDStar.TabIndex = 30;
            this.groupBox_WriteDStar.TabStop = false;
            this.groupBox_WriteDStar.Text = "写起始地址";
            this.groupBox_WriteDStar.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.button2);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewRFIDReadMessage);
            this.splitContainer1.Size = new System.Drawing.Size(936, 628);
            this.splitContainer1.SplitterDistance = 142;
            this.splitContainer1.TabIndex = 33;
            // 
            // RFIDDataDataTable_RemoveAll_bt
            // 
            this.RFIDDataDataTable_RemoveAll_bt.Location = new System.Drawing.Point(3, 355);
            this.RFIDDataDataTable_RemoveAll_bt.Name = "RFIDDataDataTable_RemoveAll_bt";
            this.RFIDDataDataTable_RemoveAll_bt.Size = new System.Drawing.Size(94, 39);
            this.RFIDDataDataTable_RemoveAll_bt.TabIndex = 33;
            this.RFIDDataDataTable_RemoveAll_bt.Text = "清除";
            this.RFIDDataDataTable_RemoveAll_bt.UseVisualStyleBackColor = true;
            this.RFIDDataDataTable_RemoveAll_bt.Click += new System.EventHandler(this.RFIDDataDataTable_RemoveAll_bt_Click);
            // 
            // groupBoxMsshow
            // 
            this.groupBoxMsshow.Controls.Add(this.comboBoxMSGType);
            this.groupBoxMsshow.Location = new System.Drawing.Point(3, 3);
            this.groupBoxMsshow.Name = "groupBoxMsshow";
            this.groupBoxMsshow.Size = new System.Drawing.Size(119, 53);
            this.groupBoxMsshow.TabIndex = 30;
            this.groupBoxMsshow.TabStop = false;
            this.groupBoxMsshow.Text = "信息显示方式";
            // 
            // comboBoxMSGType
            // 
            this.comboBoxMSGType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMSGType.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBoxMSGType.FormattingEnabled = true;
            this.comboBoxMSGType.Location = new System.Drawing.Point(10, 21);
            this.comboBoxMSGType.Name = "comboBoxMSGType";
            this.comboBoxMSGType.Size = new System.Drawing.Size(103, 24);
            this.comboBoxMSGType.TabIndex = 18;
            this.comboBoxMSGType.TextChanged += new System.EventHandler(this.comboBoxMSGType_TextChanged);
            // 
            // timer_UpData
            // 
            this.timer_UpData.Enabled = true;
            this.timer_UpData.Interval = 300;
            this.timer_UpData.Tick += new System.EventHandler(this.timer_UpData_Tick);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.groupBoxMsshow);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxConnet);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxSelePLC);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxSeleRFID);
            this.flowLayoutPanel1.Controls.Add(this.groupBox_ReadDStar);
            this.flowLayoutPanel1.Controls.Add(this.groupBox_WriteDStar);
            this.flowLayoutPanel1.Controls.Add(this.RFIDDataDataTable_RemoveAll_bt);
            this.flowLayoutPanel1.Controls.Add(this.button_UpDataMessage);
            this.flowLayoutPanel1.Controls.Add(this.groupBox_Poort);
            this.flowLayoutPanel1.Controls.Add(this.groupBox_IP);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(142, 628);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // RfidForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(936, 628);
            this.Controls.Add(this.splitContainer1);
            this.Name = "RfidForm";
            this.Text = "RfidForm";
            this.Load += new System.EventHandler(this.RfidForm_Load);
            this.SizeChanged += new System.EventHandler(this.RfidForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRFIDReadMessage)).EndInit();
            this.groupBox_IP.ResumeLayout(false);
            this.groupBox_IP.PerformLayout();
            this.groupBox_Poort.ResumeLayout(false);
            this.groupBox_Poort.PerformLayout();
            this.groupBoxSelePLC.ResumeLayout(false);
            this.groupBoxSeleRFID.ResumeLayout(false);
            this.groupBoxConnet.ResumeLayout(false);
            this.groupBoxConnet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LinkStade)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox_ReadDStar.ResumeLayout(false);
            this.groupBox_ReadDStar.PerformLayout();
            this.groupBox_WriteDStar.ResumeLayout(false);
            this.groupBox_WriteDStar.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxMsshow.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxRFIDSelet;
        private System.Windows.Forms.Label labelStadeText;
        private System.Windows.Forms.ComboBox comboBoxRFIDPLCSele;
        private System.Windows.Forms.Label labelReadAdressStarValue;
        private System.Windows.Forms.Label labelRFIDIPADRESSVULE;
        private System.Windows.Forms.Label labelRFIDPOORTVELU;
        private System.Windows.Forms.Label labelWriteAdressStarValue;
        private System.Windows.Forms.DataGridView dataGridViewRFIDReadMessage;
        private System.Windows.Forms.GroupBox groupBoxSelePLC;
        private System.Windows.Forms.GroupBox groupBoxSeleRFID;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBoxConnet;
        private System.Windows.Forms.PictureBox pictureBox_LinkStade;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox_IP;
        private System.Windows.Forms.GroupBox groupBox_Poort;
        private System.Windows.Forms.GroupBox groupBox_ReadDStar;
        private System.Windows.Forms.GroupBox groupBox_WriteDStar;
        private System.Windows.Forms.Label labelLinckText;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBoxMsshow;
        private System.Windows.Forms.ComboBox comboBoxMSGType;
        private System.Windows.Forms.Timer timer_UpData;
        private System.Windows.Forms.Button RFIDDataDataTable_RemoveAll_bt;
        private System.Windows.Forms.Button button_UpDataMessage;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
