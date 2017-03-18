using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA
{
    public partial class FormSetNumber : Form
    {
        Point point;
        public FormSetNumber(Point point)
        {
            this.point = point;
            InitializeComponent();
        }

        private void button_Cans_Click(object sender, EventArgs e)
        {
            Number = 0;
            this.Close();
        }
        private int m_Number = 0;

        public int Number
        {
            get
            {
                return m_Number;
            }

            set
            {
                m_Number = value;
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            Int32 _numer = 0;
            bool flg = int.TryParse(textBox_Number.Text,out _numer);
            Number = _numer;
            if (flg && Number <= 10 && Number >= 1)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("请输入1到10的数字！");
                Number = 0;
            }
        }

        private void FormSetNumber_Load(object sender, EventArgs e)
        {
            point.X += 100;
            point.Y += 100;
            this.Location = point;
        }
    }
}
