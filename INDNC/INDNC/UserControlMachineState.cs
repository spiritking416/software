using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INDNC
{
    public partial class UserControlMachineState : UserControl
    {
        public UserControlMachineState()
        {
            InitializeComponent();
        }

        public void ListViewDraw()
        {
            this.listView1.Columns.Add("机床号",274, HorizontalAlignment.Left);

            this.listView1.Columns.Add("机床状态", 274, HorizontalAlignment.Left);

            this.listView1.Columns.Add("当前告警", 274, HorizontalAlignment.Left);
            //this.listView1.Columns.Add("列标题1", 120, HorizontalAlignment.Left); //一步添加  
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
