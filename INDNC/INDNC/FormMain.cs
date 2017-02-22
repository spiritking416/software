using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INDNC
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("2");
        }
    }
    public partial class ServerLink
    {
        public String ServerIPAddress = null;
        public UInt32 ServerPort = 0;
        public String ServerPassword = null;

    }
}
