using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.WindowsForm
{
    public partial class Form_LogFind : Form
    {
        public EventHandler<string> FindEventHandler;
        public Form_LogFind()
        {
            InitializeComponent();
        }

        public void SettextBox_FindStr(String str)
        {
            textBox_FindStr.Text = str;
        }
        private void button_FindNext_Click(object sender, EventArgs e)
        {
            if (FindEventHandler != null)
            {
                FindEventHandler.Invoke(sender, textBox_FindStr.Text);
            }
        }

        private void button_Cans_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            //Close();
        }

        private void textBox_FindStr_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (FindEventHandler != null)
                {
                    FindEventHandler.Invoke(this, textBox_FindStr.Text);
                }
            }
        }
    }
}
