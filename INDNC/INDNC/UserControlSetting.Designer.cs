namespace INDNC
{
    partial class UserControlSetting
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
            this.tabControlSetting = new INDNC.CustomTabControl();
            this.tabPageServerSetting = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxMysqlDB = new System.Windows.Forms.TextBox();
            this.textBoxMysqlPW = new System.Windows.Forms.TextBox();
            this.textBoxMysqlserver = new System.Windows.Forms.TextBox();
            this.textBoxMysqlID = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRedisPort = new System.Windows.Forms.TextBox();
            this.textBoxRedisPw = new System.Windows.Forms.TextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.buttonServerConnect = new System.Windows.Forms.Button();
            this.tabPageLineSetting = new System.Windows.Forms.TabPage();
            this.buttonlinesettingsave = new System.Windows.Forms.Button();
            this.textBoxline = new System.Windows.Forms.TextBox();
            this.textBoxworkshop = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPageCNC = new System.Windows.Forms.TabPage();
            this.dataGridViewCNC = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControlSetting.SuspendLayout();
            this.tabPageServerSetting.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageLineSetting.SuspendLayout();
            this.tabPageCNC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCNC)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlSetting
            // 
            this.tabControlSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlSetting.Controls.Add(this.tabPageServerSetting);
            this.tabControlSetting.Controls.Add(this.tabPageLineSetting);
            this.tabControlSetting.Controls.Add(this.tabPageCNC);
            this.tabControlSetting.Controls.Add(this.tabPage2);
            this.tabControlSetting.Controls.Add(this.tabPage3);
            this.tabControlSetting.ItemSize = new System.Drawing.Size(0, 16);
            this.tabControlSetting.Location = new System.Drawing.Point(0, 0);
            this.tabControlSetting.Name = "tabControlSetting";
            this.tabControlSetting.Padding = new System.Drawing.Point(20, 20);
            this.tabControlSetting.SelectedIndex = 0;
            this.tabControlSetting.Size = new System.Drawing.Size(884, 502);
            this.tabControlSetting.TabIndex = 0;
            this.tabControlSetting.SelectedIndexChanged += new System.EventHandler(this.tabControlSetting_SelectedIndexChanged);
            // 
            // tabPageServerSetting
            // 
            this.tabPageServerSetting.BackColor = System.Drawing.Color.AliceBlue;
            this.tabPageServerSetting.Controls.Add(this.checkBox1);
            this.tabPageServerSetting.Controls.Add(this.panel2);
            this.tabPageServerSetting.Controls.Add(this.panel1);
            this.tabPageServerSetting.Controls.Add(this.buttonServerConnect);
            this.tabPageServerSetting.Location = new System.Drawing.Point(4, 20);
            this.tabPageServerSetting.Name = "tabPageServerSetting";
            this.tabPageServerSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageServerSetting.Size = new System.Drawing.Size(876, 478);
            this.tabPageServerSetting.TabIndex = 0;
            this.tabPageServerSetting.Text = "服务器设置";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(403, 379);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 16);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "记住参数";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxMysqlDB);
            this.panel2.Controls.Add(this.textBoxMysqlPW);
            this.panel2.Controls.Add(this.textBoxMysqlserver);
            this.panel2.Controls.Add(this.textBoxMysqlID);
            this.panel2.Controls.Add(this.label22);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Location = new System.Drawing.Point(499, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(308, 356);
            this.panel2.TabIndex = 1;
            // 
            // textBoxMysqlDB
            // 
            this.textBoxMysqlDB.AcceptsTab = true;
            this.textBoxMysqlDB.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxMysqlDB.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "localMysqlDatabase", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMysqlDB.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxMysqlDB.Location = new System.Drawing.Point(102, 211);
            this.textBoxMysqlDB.MaxLength = 50;
            this.textBoxMysqlDB.Name = "textBoxMysqlDB";
            this.textBoxMysqlDB.Size = new System.Drawing.Size(173, 23);
            this.textBoxMysqlDB.TabIndex = 11;
            this.textBoxMysqlDB.Text = global::INDNC.Properties.Settings.Default.localMysqlDatabase;
            this.textBoxMysqlDB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxMysqlPW
            // 
            this.textBoxMysqlPW.AcceptsTab = true;
            this.textBoxMysqlPW.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxMysqlPW.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "localserverpassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMysqlPW.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxMysqlPW.Location = new System.Drawing.Point(102, 250);
            this.textBoxMysqlPW.MaxLength = 50;
            this.textBoxMysqlPW.Name = "textBoxMysqlPW";
            this.textBoxMysqlPW.PasswordChar = '*';
            this.textBoxMysqlPW.Size = new System.Drawing.Size(173, 23);
            this.textBoxMysqlPW.TabIndex = 11;
            this.textBoxMysqlPW.Text = global::INDNC.Properties.Settings.Default.localserverpassword;
            this.textBoxMysqlPW.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxMysqlserver
            // 
            this.textBoxMysqlserver.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "localserver", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMysqlserver.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxMysqlserver.Location = new System.Drawing.Point(102, 134);
            this.textBoxMysqlserver.MaxLength = 50;
            this.textBoxMysqlserver.Name = "textBoxMysqlserver";
            this.textBoxMysqlserver.Size = new System.Drawing.Size(173, 23);
            this.textBoxMysqlserver.TabIndex = 10;
            this.textBoxMysqlserver.Text = global::INDNC.Properties.Settings.Default.localserver;
            this.textBoxMysqlserver.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxMysqlID
            // 
            this.textBoxMysqlID.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "localserverid", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMysqlID.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxMysqlID.Location = new System.Drawing.Point(102, 173);
            this.textBoxMysqlID.MaxLength = 50;
            this.textBoxMysqlID.Name = "textBoxMysqlID";
            this.textBoxMysqlID.Size = new System.Drawing.Size(173, 23);
            this.textBoxMysqlID.TabIndex = 10;
            this.textBoxMysqlID.Text = global::INDNC.Properties.Settings.Default.localserverid;
            this.textBoxMysqlID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label22.Location = new System.Drawing.Point(37, 251);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(35, 14);
            this.label22.TabIndex = 28;
            this.label22.Text = "密码";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(37, 213);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 14);
            this.label6.TabIndex = 28;
            this.label6.Text = "Database";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(37, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 14);
            this.label5.TabIndex = 29;
            this.label5.Text = "User ID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(37, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 14);
            this.label4.TabIndex = 27;
            this.label4.Text = "Server";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(33, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(204, 19);
            this.label7.TabIndex = 23;
            this.label7.Text = "本地MySQL服务器参数";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label50);
            this.panel1.Controls.Add(this.textBox4);
            this.panel1.Controls.Add(this.label52);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxRedisPort);
            this.panel1.Controls.Add(this.textBoxRedisPw);
            this.panel1.Controls.Add(this.label51);
            this.panel1.Location = new System.Drawing.Point(74, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(308, 356);
            this.panel1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "ip2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Location = new System.Drawing.Point(131, 134);
            this.textBox2.MaxLength = 3;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(36, 23);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = global::INDNC.Properties.Settings.Default.ip2;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(34, 56);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(184, 19);
            this.label12.TabIndex = 0;
            this.label12.Text = "云Redis服务器参数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(35, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 14);
            this.label3.TabIndex = 10;
            this.label3.Text = "密码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(37, 195);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 11;
            this.label2.Text = "端口";
            // 
            // textBox3
            // 
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "ip3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox3.Location = new System.Drawing.Point(177, 134);
            this.textBox3.MaxLength = 3;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(36, 23);
            this.textBox3.TabIndex = 2;
            this.textBox3.Text = global::INDNC.Properties.Settings.Default.ip3;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "ip1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(86, 134);
            this.textBox1.MaxLength = 3;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(36, 23);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = global::INDNC.Properties.Settings.Default.ip1;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label50.Location = new System.Drawing.Point(122, 143);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(14, 14);
            this.label50.TabIndex = 2;
            this.label50.Text = ".";
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "ip4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox4.Location = new System.Drawing.Point(223, 134);
            this.textBox4.MaxLength = 3;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(36, 23);
            this.textBox4.TabIndex = 3;
            this.textBox4.Text = global::INDNC.Properties.Settings.Default.ip4;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label52.Location = new System.Drawing.Point(213, 143);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(14, 14);
            this.label52.TabIndex = 4;
            this.label52.Text = ".";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(37, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 14);
            this.label1.TabIndex = 6;
            this.label1.Text = "IP";
            // 
            // textBoxRedisPort
            // 
            this.textBoxRedisPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "port", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxRedisPort.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxRedisPort.Location = new System.Drawing.Point(86, 192);
            this.textBoxRedisPort.MaxLength = 6;
            this.textBoxRedisPort.Name = "textBoxRedisPort";
            this.textBoxRedisPort.Size = new System.Drawing.Size(173, 23);
            this.textBoxRedisPort.TabIndex = 4;
            this.textBoxRedisPort.Text = global::INDNC.Properties.Settings.Default.port;
            this.textBoxRedisPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxRedisPw
            // 
            this.textBoxRedisPw.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "serverpassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxRedisPw.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxRedisPw.Location = new System.Drawing.Point(86, 250);
            this.textBoxRedisPw.MaxLength = 20;
            this.textBoxRedisPw.Name = "textBoxRedisPw";
            this.textBoxRedisPw.PasswordChar = '*';
            this.textBoxRedisPw.Size = new System.Drawing.Size(173, 23);
            this.textBoxRedisPw.TabIndex = 5;
            this.textBoxRedisPw.Text = global::INDNC.Properties.Settings.Default.serverpassword;
            this.textBoxRedisPw.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label51.Location = new System.Drawing.Point(167, 143);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(14, 14);
            this.label51.TabIndex = 3;
            this.label51.Text = ".";
            // 
            // buttonServerConnect
            // 
            this.buttonServerConnect.BackColor = System.Drawing.Color.AliceBlue;
            this.buttonServerConnect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonServerConnect.Location = new System.Drawing.Point(391, 414);
            this.buttonServerConnect.Name = "buttonServerConnect";
            this.buttonServerConnect.Size = new System.Drawing.Size(96, 28);
            this.buttonServerConnect.TabIndex = 3;
            this.buttonServerConnect.Text = "测试连接";
            this.buttonServerConnect.UseVisualStyleBackColor = false;
            this.buttonServerConnect.Click += new System.EventHandler(this.buttonServerConnect_Click);
            // 
            // tabPageLineSetting
            // 
            this.tabPageLineSetting.BackColor = System.Drawing.Color.AliceBlue;
            this.tabPageLineSetting.Controls.Add(this.buttonlinesettingsave);
            this.tabPageLineSetting.Controls.Add(this.textBoxline);
            this.tabPageLineSetting.Controls.Add(this.textBoxworkshop);
            this.tabPageLineSetting.Controls.Add(this.label18);
            this.tabPageLineSetting.Controls.Add(this.label14);
            this.tabPageLineSetting.Controls.Add(this.label13);
            this.tabPageLineSetting.Controls.Add(this.label19);
            this.tabPageLineSetting.Controls.Add(this.label17);
            this.tabPageLineSetting.Controls.Add(this.label16);
            this.tabPageLineSetting.Controls.Add(this.label24);
            this.tabPageLineSetting.Controls.Add(this.label23);
            this.tabPageLineSetting.Controls.Add(this.label21);
            this.tabPageLineSetting.Controls.Add(this.label20);
            this.tabPageLineSetting.Controls.Add(this.label15);
            this.tabPageLineSetting.Controls.Add(this.label11);
            this.tabPageLineSetting.Controls.Add(this.label10);
            this.tabPageLineSetting.Controls.Add(this.label9);
            this.tabPageLineSetting.Controls.Add(this.label8);
            this.tabPageLineSetting.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPageLineSetting.Location = new System.Drawing.Point(4, 20);
            this.tabPageLineSetting.Name = "tabPageLineSetting";
            this.tabPageLineSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLineSetting.Size = new System.Drawing.Size(876, 478);
            this.tabPageLineSetting.TabIndex = 1;
            this.tabPageLineSetting.Text = "产线设置";
            // 
            // buttonlinesettingsave
            // 
            this.buttonlinesettingsave.Location = new System.Drawing.Point(399, 380);
            this.buttonlinesettingsave.Name = "buttonlinesettingsave";
            this.buttonlinesettingsave.Size = new System.Drawing.Size(81, 30);
            this.buttonlinesettingsave.TabIndex = 3;
            this.buttonlinesettingsave.Text = "保存";
            this.buttonlinesettingsave.UseVisualStyleBackColor = true;
            this.buttonlinesettingsave.Click += new System.EventHandler(this.buttonlinesettingsave_Click);
            // 
            // textBoxline
            // 
            this.textBoxline.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "LineIndex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxline.Location = new System.Drawing.Point(533, 290);
            this.textBoxline.Name = "textBoxline";
            this.textBoxline.Size = new System.Drawing.Size(75, 23);
            this.textBoxline.TabIndex = 2;
            this.textBoxline.Text = global::INDNC.Properties.Settings.Default.LineIndex;
            this.textBoxline.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxworkshop
            // 
            this.textBoxworkshop.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::INDNC.Properties.Settings.Default, "workshopno", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxworkshop.Location = new System.Drawing.Point(533, 230);
            this.textBoxworkshop.Name = "textBoxworkshop";
            this.textBoxworkshop.Size = new System.Drawing.Size(75, 23);
            this.textBoxworkshop.TabIndex = 2;
            this.textBoxworkshop.Text = global::INDNC.Properties.Settings.Default.workshopno;
            this.textBoxworkshop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.Location = new System.Drawing.Point(251, 297);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(16, 16);
            this.label18.TabIndex = 1;
            this.label18.Text = "4";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(251, 237);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(16, 16);
            this.label14.TabIndex = 1;
            this.label14.Text = "3";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(251, 177);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(16, 16);
            this.label13.TabIndex = 1;
            this.label13.Text = "2";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.Location = new System.Drawing.Point(396, 297);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(40, 16);
            this.label19.TabIndex = 1;
            this.label19.Text = "产线";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(396, 237);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(40, 16);
            this.label17.TabIndex = 1;
            this.label17.Text = "车间";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(398, 177);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 16);
            this.label16.TabIndex = 1;
            this.label16.Text = "Robot";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.Color.AliceBlue;
            this.label24.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.ForeColor = System.Drawing.Color.Red;
            this.label24.Location = new System.Drawing.Point(634, 301);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(83, 12);
            this.label24.TabIndex = 1;
            this.label24.Text = "#号加产线索引";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.Color.AliceBlue;
            this.label23.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.ForeColor = System.Drawing.Color.Red;
            this.label23.Location = new System.Drawing.Point(634, 241);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(83, 12);
            this.label23.TabIndex = 1;
            this.label23.Text = "#号加车间索引";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label21.Location = new System.Drawing.Point(554, 177);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(32, 16);
            this.label21.TabIndex = 1;
            this.label21.Text = "CNC";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.Location = new System.Drawing.Point(554, 117);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(32, 16);
            this.label20.TabIndex = 1;
            this.label20.Text = "CNC";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(404, 117);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 16);
            this.label15.TabIndex = 1;
            this.label15.Text = "CNC";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(251, 117);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 16);
            this.label11.TabIndex = 1;
            this.label11.Text = "1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(554, 57);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 16);
            this.label10.TabIndex = 0;
            this.label10.Text = "数量";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(404, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 16);
            this.label9.TabIndex = 0;
            this.label9.Text = "设备";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(254, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "序号";
            // 
            // tabPageCNC
            // 
            this.tabPageCNC.Controls.Add(this.dataGridViewCNC);
            this.tabPageCNC.Location = new System.Drawing.Point(4, 20);
            this.tabPageCNC.Name = "tabPageCNC";
            this.tabPageCNC.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCNC.Size = new System.Drawing.Size(876, 478);
            this.tabPageCNC.TabIndex = 2;
            this.tabPageCNC.Text = "CNC设置";
            this.tabPageCNC.UseVisualStyleBackColor = true;
            // 
            // dataGridViewCNC
            // 
            this.dataGridViewCNC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCNC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCNC.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewCNC.Name = "dataGridViewCNC";
            this.dataGridViewCNC.RowTemplate.Height = 23;
            this.dataGridViewCNC.Size = new System.Drawing.Size(870, 472);
            this.dataGridViewCNC.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 20);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(876, 478);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Robot设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 20);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(876, 478);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "用户管理";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // UserControlSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.Controls.Add(this.tabControlSetting);
            this.Name = "UserControlSetting";
            this.Size = new System.Drawing.Size(884, 502);
            this.tabControlSetting.ResumeLayout(false);
            this.tabPageServerSetting.ResumeLayout(false);
            this.tabPageServerSetting.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPageLineSetting.ResumeLayout(false);
            this.tabPageLineSetting.PerformLayout();
            this.tabPageCNC.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCNC)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonServerConnect;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBoxRedisPw;
        private System.Windows.Forms.TextBox textBoxRedisPort;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label12;
        private CustomTabControl tabControlSetting;
        private System.Windows.Forms.TabPage tabPageServerSetting;
        private System.Windows.Forms.TabPage tabPageLineSetting;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMysqlPW;
        private System.Windows.Forms.TextBox textBoxMysqlID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TabPage tabPageCNC;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBoxline;
        private System.Windows.Forms.TextBox textBoxworkshop;
        private System.Windows.Forms.DataGridView dataGridViewCNC;
        private System.Windows.Forms.TextBox textBoxMysqlserver;
        private System.Windows.Forms.TextBox textBoxMysqlDB;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button buttonlinesettingsave;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
    }
}
