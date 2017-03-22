using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System;
using System.Runtime.InteropServices;

namespace INDNC
{
    public class TabControlEx : System.Windows.Forms.TabControl
    {
        private Image backImage;
        public TabControlEx() : base()
        {

            base.SetStyle(
                 ControlStyles.UserPaint |                      // 控件将自行绘制，而不是通过操作系统来绘制  
                 ControlStyles.OptimizedDoubleBuffer |          // 该控件首先在缓冲区中绘制，而不是直接绘制到屏幕上，这样可以减少闪烁  
                 ControlStyles.AllPaintingInWmPaint |           // 控件将忽略 WM_ERASEBKGND 窗口消息以减少闪烁  
                 ControlStyles.ResizeRedraw |                   // 在调整控件大小时重绘控件  
                 ControlStyles.SupportsTransparentBackColor,    // 控件接受 alpha 组件小于 255 的 BackColor 以模拟透明  
                 true);                                         // 设置以上值为 true  
            base.UpdateStyles();
            this.SizeMode = TabSizeMode.Fixed;  // 大小模式为固定  
            this.ItemSize = new Size(44, 55);   // 设定每个标签的尺寸  
            backImage = new Bitmap(this.GetType(), "TabBackground.bmp");   // 从资源文件（嵌入到程序集）里读取图片 
            this.Padding= new System.Drawing.Point(500);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (int i = 0; i < this.TabCount; i++)
            {
                e.Graphics.DrawRectangle(Pens.White, this.GetTabRect(i));
                if (this.SelectedIndex == i)
                {
                    e.Graphics.DrawImage(backImage, this.GetTabRect(i));
                }

                // Calculate text position  
                Rectangle bounds = this.GetTabRect(i);
                PointF textPoint = new PointF();
                SizeF textSize = TextRenderer.MeasureText(this.TabPages[i].Text, this.Font);

                // 注意要加上每个标签的左偏移量X  
                textPoint.X
                    = bounds.X + (bounds.Width - textSize.Width) / 2;
                textPoint.Y
                    = bounds.Bottom - textSize.Height - this.Padding.Y;

                // Draw highlights  
                e.Graphics.DrawString(
                    this.TabPages[i].Text,
                    this.Font,
                    SystemBrushes.ControlLightLight,    // 高光颜色  
                    textPoint.X,
                    textPoint.Y);

                // 绘制正常文字  
                textPoint.Y--;
                e.Graphics.DrawString(
                    this.TabPages[i].Text,
                    this.Font,
                    SystemBrushes.ControlText,    // 正常颜色  
                    textPoint.X,
                    textPoint.Y);
            }
        }
    }
}
