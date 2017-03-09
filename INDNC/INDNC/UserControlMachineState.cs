using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceStack.Redis;

namespace INDNC
{
    public partial class UserControlMachineState : UserControl
    {
        public UserControlMachineState()
        {
            InitializeComponent();
        }

        public bool ListViewDraw(ref RedisClient Client,ref UInt16 lineno)
        {

            try
            {
                if (lineno == 0)
                    throw new Exception("未选择生产线路！");
                this.listView1.BeginUpdate();

                this.listView1.Columns.Add("机床号", 273, HorizontalAlignment.Left);
                this.listView1.Columns.Add("机床状态", 273, HorizontalAlignment.Left);
                this.listView1.Columns.Add("当前告警", 273, HorizontalAlignment.Left);
                this.listView1.Columns.Add("发生时间", 273, HorizontalAlignment.Left);


                ListViewItem listviewitem = new ListViewItem("A1");
                listviewitem.SubItems.Add("A2");

                this.listView1.Items.Add(listviewitem);




                this.listView1.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                return false;
            }


            return true;
            /*
            for(int i=0; i<16; ++i)
            {
                try
                {
                    serverpara.DBNoName = i;
                    Client.Dispose();
                    if (!serverlink.Link(ref (Client)))
                        return;
                    string MachineSNC = Client.Get<string>("Machine");

                    ListViewItem listitem = new ListViewItem(MachineSNC);
                    this.listView1.Items.Add(listitem);
                }
                catch(Exception)
                {
                    break;
                }
            }*/

            

            /*this.listView1.Items.Add("A1");
            this.listView1.Items[0].SubItems.Add("A2");
            this.listView1.Items[0].SubItems.Add("A3");
            this.listView1.Items[0].SubItems.Add("A4");

            this.listView1.Items.Add("B1");
            this.listView1.Items[1].SubItems.Add("B2");
            this.listView1.Items[1].SubItems.Add("B3");
            this.listView1.Items[1].SubItems.Add("B4");*/

            // lvi.SubItems.Add("第3列,第" + i + "行");

            //this.listView1.Items[0].BackColor = Color.White;
            /*this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度  
            
            for (int i = 0; i < 10; i++)   //添加10行数据  
                {
                ListViewItem lvi = new ListViewItem();
                
                lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标  
                
                lvi.Text = "subitem" + i;
                
                lvi.SubItems.Add("第2列,第" + i + "行");
                
                lvi.SubItems.Add("第3列,第" + i + "行");
                
                     this.listView1.Items.Add(lvi);
                 }
            
                this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。  */


        }

    }
}
