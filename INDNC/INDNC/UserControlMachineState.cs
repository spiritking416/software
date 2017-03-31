using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace INDNC
{
    public partial class UserControlMachineState : UserControl
    {
        public event btnOkClickEventHander ComboBoxMachineViewChange; //监测显示切换委托
        public event btnOkClickEventHander listViewItemMouseMove; //listview item选中委托
        public MachineView ComboBoxFlag;

        public UserControlMachineState()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            comboBoxMachineview.Items.Add("生产线设备");
            comboBoxMachineview.Items.Add("在线设备");
            comboBoxMachineview.Items.Add("离线设备");
            comboBoxMachineview.Items.Add("告警设备");
            comboBoxMachineview.Items.Add("参数错误设备");
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
                if (listView1.Items != null)
                    listView1.Items.Clear();
                if (listView1.Columns!=null)
                {
                    listView1.Columns.Clear();
                }

                int width = (this.listView1.Width - 60) / 4;
                this.listView1.Columns.Add("序号", 60, HorizontalAlignment.Left);
                this.listView1.Columns.Add("设备编号", width, HorizontalAlignment.Left);
                this.listView1.Columns.Add("设备状态", width, HorizontalAlignment.Left);
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

        private void comboBoxMachineview_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxMachineview.SelectedIndex)
            {
                case 0:
                    ComboBoxFlag = MachineView.Visiable;
                    if (ComboBoxMachineViewChange != null)
                        ComboBoxMachineViewChange(sender, e);
                    break;
                case 1:
                    ComboBoxFlag = MachineView.Online;
                    if (ComboBoxMachineViewChange != null)
                        ComboBoxMachineViewChange(sender, e);
                    break;
                case 2:
                    ComboBoxFlag = MachineView.Offline;
                    if (ComboBoxMachineViewChange != null)
                        ComboBoxMachineViewChange(sender, e);
                    break;
                case 3:
                    ComboBoxFlag = MachineView.Alarm;
                    if (ComboBoxMachineViewChange != null)
                        ComboBoxMachineViewChange(sender, e);
                    break;
                case 4:
                    ComboBoxFlag = MachineView.Invisiable;
                    if (ComboBoxMachineViewChange != null)
                        ComboBoxMachineViewChange(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listViewItemMouseMove != null)
                listViewItemMouseMove(sender, e);
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
        /*
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
        return string.Compare(xx, yy);//按字符比较  */
        return 0;
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
