using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace INDNC
{
    public partial class UserControlMachineState : UserControl
    {
        public Thread threadRefrush;

        public UserControlMachineState()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            comboBoxMachineview.Items.Add("可连接设备");
            comboBoxMachineview.Items.Add("在线设备");
            comboBoxMachineview.Items.Add("离线设备");
            comboBoxMachineview.Items.Add("告警设备数目");
            comboBoxMachineview.Items.Add("未显示设备数目");
            comboBoxMachineview.SelectedIndex = 0;

            this.listView1.View = View.Details;
            //选择总行  
            this.listView1.FullRowSelect = true;
            //设置 listview 的排序方法，在列标头事件中可以取出用来判断  
            this.listView1.Sorting = SortOrder.Ascending;
            //设置按第1列排序  
            this.listView1.ListViewItemSorter = new ListViewItemComparer(this.listView1.Sorting, 1);
        }

        public bool ListViewTitleDraw()
        {
            try
            {
                this.listView1.BeginUpdate();

                int width = (this.listView1.Width - 60) / 4;
                this.listView1.Columns.Add("序号", 60, HorizontalAlignment.Left);
                this.listView1.Columns.Add("机床编号", width, HorizontalAlignment.Left);
                this.listView1.Columns.Add("机床状态", width, HorizontalAlignment.Left);
                this.listView1.Columns.Add("当前告警", width, HorizontalAlignment.Left);
                this.listView1.Columns.Add("发生时间", width, HorizontalAlignment.Left);

                this.listView1.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                return false;
            }

            return true;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {/*
            if (this.listView1.Sorting == SortOrder.Ascending)
            {
                this.listView1.ListViewItemSorter = new ListViewItemComparer(SortOrder.Descending, e.Column);
                this.listView1.Sorting = SortOrder.Descending;
            }
            else
            {
                this.listView1.ListViewItemSorter = new ListViewItemComparer(SortOrder.Ascending, e.Column);
                this.listView1.Sorting = SortOrder.Ascending;
            }*/
        }
    }
}

//定义一个类，这个类实现 IComparer 接口  
class ListViewItemComparer : IComparer
{
    private int col;
    private SortOrder sort = SortOrder.None;
    public ListViewItemComparer(SortOrder sortOrder)
    {
        col = 0;
    }
    public ListViewItemComparer(SortOrder sortOrder, int column)
    {
        sort = sortOrder;
        col = column;
    }

    //实现接口的 Compare 方法，x、y 为要比较的两个对象   
    public int Compare(object x, object y)
    {
        //默认升序。判断传入的排序枚举，如果为降序就对换要比较的对象  
        if (sort == SortOrder.Descending)
        {
            object temp = x;
            x = y;
            y = temp;
        }

        string xx = ((ListViewItem)x).SubItems[col].Text;
        string yy = ((ListViewItem)y).SubItems[col].Text;

        int xxx = 0;
        int yyy = 0;

        //判断是否可以转换为数字，如果可以就按数字比较  
        if (int.TryParse(xx.ToString(), out xxx) && int.TryParse(yy.ToString(), out yyy))
            return xxx.CompareTo(yyy);//按数字比较  
        return string.Compare(xx, yy);//按字符比较  
    }
}

public class DoubleBufferListView : ListView
{
    public DoubleBufferListView()
    {
        SetStyle(ControlStyles.DoubleBuffer |
          ControlStyles.OptimizedDoubleBuffer |
          ControlStyles.AllPaintingInWmPaint, true);
        UpdateStyles();
    }
}
