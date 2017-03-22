namespace SCADA
{
    partial class UserControlParaman
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
            this.treeViewParaman = new System.Windows.Forms.TreeView();
            this.timerUpdataShow = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewParam = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParam)).BeginInit();
            this.SuspendLayout();
            // 
            // treeViewParaman
            // 
            this.treeViewParaman.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewParaman.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeViewParaman.Location = new System.Drawing.Point(0, 0);
            this.treeViewParaman.Name = "treeViewParaman";
            this.treeViewParaman.Size = new System.Drawing.Size(111, 364);
            this.treeViewParaman.TabIndex = 1;
            this.treeViewParaman.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewParaman_AfterSelect_1);
            // 
            // timerUpdataShow
            // 
            this.timerUpdataShow.Interval = 200;
            this.timerUpdataShow.Tick += new System.EventHandler(this.timerUpdataShow_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewParaman);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewParam);
            this.splitContainer1.Size = new System.Drawing.Size(625, 364);
            this.splitContainer1.SplitterDistance = 111;
            this.splitContainer1.TabIndex = 2;
            // 
            // dataGridViewParam
            // 
            this.dataGridViewParam.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewParam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewParam.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewParam.Name = "dataGridViewParam";
            this.dataGridViewParam.RowTemplate.Height = 23;
            this.dataGridViewParam.Size = new System.Drawing.Size(510, 364);
            this.dataGridViewParam.TabIndex = 0;
            // 
            // UserControlParaman
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UserControlParaman";
            this.Size = new System.Drawing.Size(625, 364);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewParaman;
        private System.Windows.Forms.Timer timerUpdataShow;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewParam;
    }
}
